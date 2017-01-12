using System;
using System.Diagnostics;

namespace KarkatAnimation.Audio
{
    public class SampleAggregator
    {
        public event EventHandler<MaxSampleEventArgs> MaximumCalculated;
        private float _maxValue;
        private float _minValue;
        private int _count;
        public int NotificationCount { get; set; }

        public void Reset()
        {
            _count = 0;
            _maxValue = _minValue = 0;
        }

        public void Add(float value)
        {
            _maxValue = Math.Max(_maxValue, value);
            _minValue = Math.Min(_minValue, value);
            _count++;
            if (_count >= NotificationCount && NotificationCount > 0)
            {
                MaximumCalculated?.Invoke(this, new MaxSampleEventArgs(_minValue, _maxValue));
                Reset();
            }
        }
    }

    public class MaxSampleEventArgs : EventArgs
    {
        [DebuggerStepThrough]
        public MaxSampleEventArgs(float minValue, float maxValue)
        {
            MaxSample = maxValue;
            MinSample = minValue;
        }
        public float MaxSample { get; private set; }
        public float MinSample { get; private set; }
    }
}
