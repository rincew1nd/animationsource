using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace KarkatAnimation.Settings
{
    public class SettingsObj
    {
        //Microphone
        public int LastUsedDevice;
        //Volume values
        public int Silence;
        public int Speaking;
        public int Shouting;
        //Peak
        public int UpdateTime;
        public decimal PeakDelta;
        //Images for animation
        public List<AnimationImage> Images;
    }
}
