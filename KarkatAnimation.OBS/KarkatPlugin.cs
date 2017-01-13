using CLROBS;
using KarkatAnimation.OBS.Animation;

namespace KarkatAnimation.OBS
{
    /// <summary>
    /// Karkat plugin
    /// </summary>
    class KarkatPlugin : AbstractPlugin
    {
        /// <summary>
        /// Define plugin name and description
        /// </summary>
        public KarkatPlugin()
        {
            Name = "Karkat animation plugin";
            Description = "Plugin for visualizing animation based on microphone volume";
        }
        
        /// <summary>
        /// Load plugin into OBS
        /// </summary>
        /// <returns></returns>
        public override bool LoadPlugin()
        {
            API.Instance.AddImageSourceFactory(new AnimationSourceFactory());
            return true;
        }
    }
}
