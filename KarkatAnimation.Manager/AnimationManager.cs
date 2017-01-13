using System;
using System.Collections.Generic;
using System.Timers;
using KarkatAnimation.Audio;
using KarkatAnimation.Settings;
using Timer = System.Timers.Timer;

namespace KarkatAnimation.Manager
{
    public class AnimationManager
    {
        private readonly SettingsObj _settings;
        private readonly AudioRecorder _recorder;
        private readonly Timer _pictureUpdateTimer;
        public KeyValuePair<VolumeType, int> AnimationState;

        private int _currentPictureCount;
        private decimal _voiceVolume;

        public AnimationManager()
        {
            _settings = SettingsManager.Load();

            _recorder = new AudioRecorder();
            _recorder.SampleAggregator.MaximumCalculated += OnRecorderMaximumCalculated;

            _pictureUpdateTimer = new Timer();
            _pictureUpdateTimer.Elapsed += UpdateImage;
        }

        public void StartMonitoring()
        {
            AnimationState = new KeyValuePair<VolumeType, int>(VolumeType.Stopped, 0);
            _recorder.BeginMonitoring(_settings.LastUsedDevice);
            _pictureUpdateTimer.Interval = _settings.UpdateTime;
            _pictureUpdateTimer.Start();
        }

        public void StopMonitoring()
        {
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
            CLROBS.API.Instance.Log($"Voice volume - {_voiceVolume}");

            VolumeType type = _voiceVolume <= _settings.Silence ?
                VolumeType.Silence : _voiceVolume <= _settings.Speaking ?
                    VolumeType.Speaking : _voiceVolume <= _settings.Shouting ? 
                        VolumeType.Shouting : VolumeType.Stopped;

            if (AnimationState.Key != type)
            {
                AnimationState = new KeyValuePair<VolumeType, int>(type, 0);
                _currentPictureCount = _settings.Images[type].Count-1;
            }
            else
            {
                AnimationState = new KeyValuePair<VolumeType, int>(
                    type,
                    (AnimationState.Value == _currentPictureCount) ?
                        0 : AnimationState.Value + 1
                );
            }
        }
    }
}
