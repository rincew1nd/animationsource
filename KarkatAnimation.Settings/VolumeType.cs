using System.ComponentModel;

namespace KarkatAnimation.Settings
{
    /// <summary>
    /// Animation type enum
    /// </summary>
    public enum VolumeType
    {
        [DefaultValue("Silence")]
        Silence = 0,
        [DefaultValue("Speaking")]
        Speaking = 1,
        [DefaultValue("Shouting")]
        Shouting = 2,
        Stopped = 3
    }
}