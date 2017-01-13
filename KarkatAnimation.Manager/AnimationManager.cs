using System;
using System.Collections.Generic;
using System.Timers;
using KarkatAnimation.Audio;
using KarkatAnimation.Settings;
using Timer = System.Timers.Timer;

namespace KarkatAnimation.Manager
{
    /// <summary>
    /// Manager for calculating animation state based on microphone volume
    /// </summary>
    public class AnimationManager
    {
        /// <summary>
        /// Application settings
        /// </summary>
        private readonly SettingsObj _settings;

        /// <summary>
        /// Input device recorder
        /// </summary>
        private readonly AudioRecorder _recorder;

        /// <summary>
        /// Picture update timer
        /// </summary>
        private readonly Timer _pictureUpdateTimer;

        /// <summary>
        /// Current animation state
        /// </summary>
        public KeyValuePair<VolumeType, int> AnimationState;

        /// <summary>
        /// Current animation maximal frame count
        /// </summary>
        private int _currentPictureCount;

        /// <summary>
        /// Current voice volume
        /// </summary>
        private decimal _voiceVolume;

        /// <summary>
        /// Init settings, audio recorder, animation update time
        /// </summary>
        public AnimationManager()
        {
            _settings = SettingsManager.Load();

            _recorder = new AudioRecorder();
            _recorder.SampleAggregator.MaximumCalculated += OnRecorderMaximumCalculated;

            _pictureUpdateTimer = new Timer();
            _pictureUpdateTimer.Elapsed += UpdateImage;
        }

        /// <summary>
        /// Start input device monitoring and animation update 
        /// </summary>
        public void StartMonitoring()
        {
            AnimationState = new KeyValuePair<VolumeType, int>(VolumeType.Stopped, 0);
            _recorder.BeginMonitoring(_settings.LastUsedDevice);
            _pictureUpdateTimer.Interval = _settings.UpdateTime;
            _pictureUpdateTimer.Start();
        }

        /// <summary>
        /// Stop input device monitoring and animation update
        /// </summary>
        public void StopMonitoring()
        {
            _recorder.StopMonitoring();
            _pictureUpdateTimer.Stop();
            _voiceVolume = 0;
        }

        /// <summary>
        /// Method that called when sample aggregator calculate sample value
        /// Set and smooth voice volume for picture updating
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRecorderMaximumCalculated(object sender, MaxSampleEventArgs e)
        {
            var lastPeak = Math.Max(e.MaxSample, Math.Abs(e.MinSample));
            var lastPeakInPercent = (decimal) (lastPeak * 100);

            if (_voiceVolume < lastPeakInPercent)
                _voiceVolume = lastPeakInPercent;
            else if (_voiceVolume > 0)
                _voiceVolume = _voiceVolume > _settings.SampleDelta ?
                    _voiceVolume - _settings.SampleDelta : 0;
        }

        /// <summary>
        /// Update animation state based on voice volume
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
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
