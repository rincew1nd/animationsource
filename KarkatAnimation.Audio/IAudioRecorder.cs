using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;

namespace KarkatAnimation.Audio
{
    public interface IAudioRecorder
    {
        void BeginMonitoring(int recordingDevice);
        void StopMonitoring();
        SampleAggregator SampleAggregator { get; }
    }
}
