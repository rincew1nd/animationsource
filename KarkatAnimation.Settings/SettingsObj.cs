using System.Collections.Generic;
using Newtonsoft.Json;

namespace KarkatAnimation.Settings
{
    /// <summary>
    /// Settings object
    /// </summary>
    public class SettingsObj
    {
        /// <summary>
        /// Input device order
        /// </summary>
        public int LastUsedDevice;

        /// <summary>
        /// Silence maximum voice volume
        /// </summary>
        public int Silence;

        /// <summary>
        /// Speaking maximum voice volume
        /// </summary>
        public int Speaking;

        /// <summary>
        /// Shouting maximum voice volume
        /// </summary>
        public int Shouting;

        /// <summary>
        /// Anmation update time
        /// </summary>
        public int UpdateTime;

        /// <summary>
        /// Sample smooth value per update
        /// </summary>
        public decimal SampleDelta;

        /// <summary>
        /// Animation frames store
        /// </summary>
        public Dictionary<VolumeType,List<AnimationImage>> Images;
    }
}
