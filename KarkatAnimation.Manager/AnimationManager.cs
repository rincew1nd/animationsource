using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Timers;
using KarkatAnimation.Audio;
using KarkatAnimation.WebServer;
using KarkatAnimation.Settings;

namespace KarkatAnimation.Manager
{
    public class AnimationManager
    {
        private readonly SettingsObj _settings;
        private readonly AudioRecorder _recorder;
        private readonly Timer _pictureUpdateTimer;
        private readonly KarkatServer _karkatWebServer;

        private KeyValuePair<VolumeType, int> _pictureState;
        private int _currentPictureCount;
        private decimal _voiceVolume;

        public AnimationManager()
        {
            _settings = SettingsManager.Load();
            _karkatWebServer = new KarkatServer();

            _recorder = new AudioRecorder();
            _recorder.SampleAggregator.MaximumCalculated += OnRecorderMaximumCalculated;

            _pictureUpdateTimer = new Timer();
            _pictureUpdateTimer.Elapsed += UpdateImage;
        }

        public void StartMonitoring()
        {
            Task.Factory.StartNew(() => _karkatWebServer.StartServer(@"http://localhost:8734/karkat/"));
            
            _pictureState = new KeyValuePair<VolumeType, int>(VolumeType.Stopped, 0);
            _recorder.BeginMonitoring(_settings.LastUsedDevice);
            _pictureUpdateTimer.Interval = _settings.UpdateTime;
            _pictureUpdateTimer.Start();
        }

        public void StopMonitoring()
        {
            _karkatWebServer.StopServer();

            _recorder.StopMonitoring();
            _pictureUpdateTimer.Stop();
            _voiceVolume = 0;
        }

        private void OnRecorderMaximumCalculated(object sender, MaxSampleEventArgs e)
        {
            var lastPeak = Math.Max(e.MaxSample, Math.Abs(e.MinSample));
            var lastPeakInPercent = (decimal) (lastPeak * 100);

            if (_voiceVolume < lastPeakInPercent)
                _voiceVolume = lastPeakInPercent;
            else if (_voiceVolume > 0)
                _voiceVolume = _voiceVolume > _settings.PeakDelta ?
                    _voiceVolume - _settings.PeakDelta : 0;
        }

        private void UpdateImage(object s, ElapsedEventArgs e)
        {
            var voiceVolume = _voiceVolume;

            VolumeType type = voiceVolume <= _settings.Silence ?
                VolumeType.Silence : voiceVolume <= _settings.Speaking ?
                    VolumeType.Speaking : voiceVolume <= _settings.Shouting ? 
                        VolumeType.Shouting : VolumeType.Stopped;

            if (_pictureState.Key != type)
            {
                _pictureState = new KeyValuePair<VolumeType, int>(type, 0);
                _currentPictureCount = GetMaximumOrder(type);
            }
            else
            {
                _pictureState = new KeyValuePair<VolumeType, int>(
                    type,
                    (_pictureState.Value == _currentPictureCount)
                        ? 0
                        : _pictureState.Value + 1
                );
            }
            
            Task.Factory.StartNew(() =>
            {
                var image = _settings.Images
                    .FirstOrDefault(im => im.Type == _pictureState.Key &&
                                          im.Order == _pictureState.Value);
                if (image != null)
                {
                    var request = WebRequest.Create("http://localhost:8734/karkat/set");
                    request.ContentType = "application/json";
                    request.Method = "POST";
                    request.ContentLength = 0;
                    request.Headers.Add("ImagePath", image.Path.Replace("\\", "\\\\"));
                    request.Headers.Add("Volume", voiceVolume.ToString("F2"));
                    request.GetResponse();
                }
            });
        }

        /// <summary>
        /// Получить максимальный порядок по VolumeType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetMaximumOrder(VolumeType type)
        {
            if (_settings.Images.Count(im => im.Type == type) == 0)
                return -1;

            return _settings.Images
                .Where(im => im.Type == type)
                .Select(im => im.Order)
                .Max();
        }
    }
}
