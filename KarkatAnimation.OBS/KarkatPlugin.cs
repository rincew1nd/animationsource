using CLROBS;
using KarkatAnimation.OBS.Animation;

namespace KarkatAnimation.OBS
{
    class KarkatPlugin : AbstractPlugin
    {
        public KarkatPlugin()
        {
            Name = "Karkat animation plugin";
            Description = "Plugin for visualizing animation based on microphone volume";
        }
        
        public override bool LoadPlugin()
        {
            API.Instance.AddImageSourceFactory(new AnimationSourceFactory());
            return true;
        }
    }
}
