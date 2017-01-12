using System;
using NAudio.Wave;

namespace KarkatAnimation.Audio
{
    public class AudioRecorder : IAudioRecorder
    {
        private WaveInEvent _waveIn;
        private readonly SampleAggregator _sampleAggregator;
        private WaveFormat _recordingFormat;
        public bool IsMonitoring;

        public AudioRecorder()
        {
            _sampleAggregator = new SampleAggregator();
            RecordingFormat = new WaveFormat(44100, 1);
            IsMonitoring = false;
        }

        public WaveFormat RecordingFormat
        {
            get { return _recordingFormat; }
            set
            {
                _recordingFormat = value;
                _sampleAggregator.NotificationCount = value.SampleRate / 10;
            }
        }

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

        public SampleAggregator SampleAggregator => _sampleAggregator;
        
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