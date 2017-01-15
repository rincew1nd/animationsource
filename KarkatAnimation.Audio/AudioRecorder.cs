using System;
using NAudio.Wave;

namespace KarkatAnimation.Audio
{
    /// <summary>
    /// Audio recorder that monitor input device
    /// </summary>
    public class AudioRecorder : IAudioRecorder
    {
        /// <summary>
        /// Recording audio in non-gui application
        /// </summary>
        private WaveInEvent _waveIn;

        /// <summary>
        /// Access to SampleAggregator outside of class
        /// </summary>
        public SampleAggregator SampleAggregator => _sampleAggregator;

        /// <summary>
        /// Sample aggregator for event about peak values
        /// </summary>
        private readonly SampleAggregator _sampleAggregator;

        /// <summary>
        /// Wave format for WaveInEvent
        /// </summary>
        private WaveFormat _recordingFormat;

        /// <summary>
        /// Is audio recorder works?
        /// </summary>
        public bool IsMonitoring;

        /// <summary>
        /// Init SampleAggregator, WaveFormat
        /// </summary>
        public AudioRecorder()
        {
            _sampleAggregator = new SampleAggregator();
            RecordingFormat = new WaveFormat(10000, 1);
            IsMonitoring = false;
        }

        /// <summary>
        /// Wave format for audio recording
        /// </summary>
        public WaveFormat RecordingFormat
        {
            get { return _recordingFormat; }
            set
            {
                _recordingFormat = value;
                _sampleAggregator.NotificationCount = value.SampleRate / 10;
            }
        }

        /// <summary>
        /// Begin monitoring input device
        /// </summary>
        /// <param name="recordingDevice">Index of input device</param>
        public void BeginMonitoring(int recordingDevice)
        {
            if (IsMonitoring) return;
            
            _waveIn = new WaveInEvent()
            {
                DeviceNumber = recordingDevice
            };
            _waveIn.DataAvailable += OnDataAvailable;
            _waveIn.WaveFormat = _recordingFormat;
            _waveIn.StartRecording();
            IsMonitoring = true;
        }

        /// <summary>
        /// Begin monitoring input device
        /// </summary>
        public void StopMonitoring()
        {
            if (IsMonitoring)
            {
                _waveIn.StopRecording();
                _waveIn.Dispose();
                _waveIn = null;
                _sampleAggregator.Reset();
                IsMonitoring = false;
            }
        }
        
        /// <summary>
        /// Method for getting raw data from input device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            var buffer = e.Buffer;

            for (var index = 0; index < e.BytesRecorded; index += 2)
            {
                var sample = (short) ((buffer[index + 1] << 8) |
                                        buffer[index + 0]);
                var sample32 = sample / 32768f;
                _sampleAggregator.Add(sample32);
            }
        }
    }
}