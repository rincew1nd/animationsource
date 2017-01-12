using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KarkatAnimation.WebServer
{
    public class KarkatServer
    {
        public HttpListener KarkatWebServer;
        public string ImagePath;
        public string Volume;
        
        public void StartServer(string url)
        {
            KarkatWebServer = new HttpListener();

            if (!HttpListener.IsSupported) return;
            KarkatWebServer.Prefixes.Add(url);
            KarkatWebServer.Start();

            while (KarkatWebServer.IsListening)
            {
                var context = KarkatWebServer.GetContext();
                Task.Factory.StartNew(() => ProceedRequest(context));
            }
        }

        public void StopServer()
        {
            KarkatWebServer.Stop();
        }

        private void ProceedRequest(HttpListenerContext context)
        {
            switch (context.Request.RawUrl)
            {
                case "/karkat":
                    ShowPage(context);
                    break;
                case "/karkat/set":
                    SetImagePath(context);
                    break;
                case "/karkat/get":
                    GetImagePath(context);
                    break;
                default:
                    break;
            }
        }

        private void ShowPage(HttpListenerContext context)
        {
            var responseString =
                "<!DOCTYPE html>" +
                "<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">" +
                "<head>" +
                "<meta charset=\"utf-8\" />" +
                "<title>Karkat Animation Page</title>" +
                "<style>body {background-color: #00FF00;}</style>" +
                "<script src=\"https://ajax.googleapis.com/ajax/libs/jquery/3.1.1/jquery.min.js\"></script>" +
                "<script>" +
                "(function updatePicture() {" +
                "$.getJSON('http://localhost:8734/karkat/get', function (response) {" +
                "document.getElementById(\"picture\").src = response.ImagePath;" +
                "console.log(response.ImagePath);" +
                "console.log(response.Volume);" +
                "setTimeout(updatePicture, 500);" +
                "});" +
                "}());" +
                "</script>" +
                "</head>" +
                "<body>" +
                "<img id=\"picture\" src=\"\"/>" +
                "</body>" +
                "</html>";
            var response = context.Response;
            response.ContentType = "text/html; charset=UTF-8";
            var buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            using (var output = response.OutputStream)
            {
                output.Write(buffer, 0, buffer.Length);
            }
        }

        private void SetImagePath(HttpListenerContext context)
        {
            var request = context.Request;
            var headers = request.Headers.GetValues("ImagePath");
            if (headers != null && headers.Length == 1)
                ImagePath = headers[0];
            Volume = request.Headers.GetValues("Volume").FirstOrDefault();

            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = 200;
            response.Close();
        }

        private void GetImagePath(HttpListenerContext context)
        {
            var responseString = "{\"ImagePath\":\"" + ImagePath + "\",\"Volume\":\"" + Volume + "\"}";
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = 200;
            var buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            using (var output = response.OutputStream)
            {
                output.Write(buffer, 0, buffer.Length);
            }
            response.Close();
        }

        private string ShowRequestData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody) return null;

            string text;
            using (var reader = new StreamReader(request.InputStream))
            {
                text = System.Web.HttpUtility.UrlDecode(
                    reader.ReadToEnd().Remove(0, 7),
                    Encoding.UTF8);
            }
            return text;
        }
    }
}
