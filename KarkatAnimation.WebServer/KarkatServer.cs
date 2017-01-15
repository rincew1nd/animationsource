using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using KarkatAnimation.Settings;

namespace KarkatAnimation.WebServer
{
    public class KarkatServer
    {
        private readonly HttpListener _karkatWebServer;
        private readonly SettingsObj _settings;

        public KarkatServer()
        {
            _settings = SettingsManager.Settings;
            _karkatWebServer = new HttpListener();
        }

        public void StartServer()
        {
            if (!HttpListener.IsSupported)
                throw new Exception("Web server is not supported :<");

            _karkatWebServer.Prefixes.Clear();
            _karkatWebServer.Prefixes.Add($"http://localhost:{_settings.Port}/");

            _karkatWebServer.Start();

            while (_karkatWebServer.IsListening)
            {
                try
                {
                    var context = _karkatWebServer.GetContext();
                    Task.Factory.StartNew(() => ProceedRequest(context));
                }
                catch (HttpListenerException e)
                {
                    if (e.TargetSite.Name != "GetContext")
                        throw;
                }
            }
        }

        public void StopServer()
        {
            _karkatWebServer.Stop();
        }

        private void ProceedRequest(HttpListenerContext context)
        {
            switch (context.Request.RawUrl)
            {
                case "/karkat":
                    ShowPage(context);
                    break;
                case "/karkat/image.jpg":
                    GetImage(context);
                    break;
                default:
                    BadRequestResponse(context);
                    break;
            }
        }

        private void ShowPage(HttpListenerContext context)
        {
            if (!File.Exists("htmlpage.html"))
                throw new Exception("Cant find 'htmlpage.html' in root folder!");
            var responseString = File.ReadAllText("htmlpage.html")
                .Replace("{updateTime}", _settings.UpdateTime.ToString())
                .Replace("{port}", _settings.Port.ToString());

            context.Response.ContentType = "text/html; charset=UTF-8";
            context.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            context.Response.Headers.Add("Pragma", "no-cache");
            context.Response.Headers.Add("Expires", "0");

            var buffer = Encoding.UTF8.GetBytes(responseString);
            context.Response.ContentLength64 = buffer.Length;
            using (var output = context.Response.OutputStream)
            {
                output.Write(buffer, 0, buffer.Length);
            }

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.OutputStream.Flush();
            context.Response.OutputStream.Close();
        }

        private void GetImage(HttpListenerContext context)
        {
            var animationState = _settings.AnimationState;

            if (animationState.Key == VolumeType.Stopped)
            {
                BadRequestResponse(context);
                return;
            }

            try
            {
                var image = _settings.Images[animationState.Key][animationState.Value];
                if (image.Path == null)
                {
                    BadRequestResponse(context);
                    return;
                }

                context.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate"); // HTTP 1.1.
                context.Response.Headers.Add("Pragma", "no-cache"); // HTTP 1.0.
                context.Response.Headers.Add("Expires", "0"); // Proxies.

                var type = Path.GetExtension(image.Path).Substring(1);
                if (type == "jpg") type = "jpeg";
                context.Response.ContentType = $"image/{type}";

                Stream input = new FileStream(image.Path, FileMode.Open);

                context.Response.ContentLength64 = input.Length;

                byte[] buffer = new byte[1024 * 16];
                int nbytes;
                while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                    context.Response.OutputStream.Write(buffer, 0, nbytes);
                input.Close();
            
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.OutputStream.Flush();
                context.Response.OutputStream.Close();
            }
            catch (IOException e)
            {
                if (e.TargetSite.Name != "WinIOError")
                    throw;
            }
        }

        private void BadRequestResponse (HttpListenerContext context)
        {
            context.Response.ContentType = "text/html";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.OutputStream.Flush();
            context.Response.OutputStream.Close();
        }
    }
}
