using System;
using System.Diagnostics;

namespace KarkatAnimation.Audio
{
    /// <summary>
    /// Sample aggregator for calculating peak values
    /// </summary>
    public class SampleAggregator
    {
        /// <summary>
        /// Event about N sample calculation
        /// </summary>
        public event EventHandler<MaxSampleEventArgs> MaximumCalculated;

        /// <summary>
        /// Maximum sample value
        /// </summary>
        private float _maxValue;
        
        /// <summary>
        /// Minimum sample value
        /// </summary>
        private float _minValue;

        /// <summary>
        /// How much times Add method got called
        /// </summary>
        private int _count;

        /// <summary>
        /// How much times should Add method got called 
        /// for MaximumCalculated event triggering
        /// </summary>
        public int NotificationCount { get; set; }

        /// <summary>
        /// Reset data
        /// </summary>
        public void Reset()
        {
            _count = 0;
            _maxValue = _minValue = 0;
        }

        /// <summary>
        /// Add data about peak value
        /// </summary>
        /// <param name="value">peak value</param>
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

    /// <summary>
    /// SampleEvent peak calculation arguments
    /// </summary>
    public class MaxSampleEventArgs : EventArgs
    {
        [DebuggerStepThrough]
        public MaxSampleEventArgs(float minValue, float maxValue)
        {
            MaxSample = maxValue;
            MinSample = minValue;
        }

        /// <summary>
        /// Maximal sample value
        /// </summary>
        public float MaxSample { get; private set; }

        /// <summary>
        /// Minimal sample value
        /// </summary>
        public float MinSample { get; private set; }
    }
}
