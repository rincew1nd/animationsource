using CLROBS;
using Newtonsoft.Json;

namespace KarkatAnimation.Settings
{
    public class AnimationImage
    {
        public string Path;
        public VolumeType Type;
        public int Order;
        [JsonIgnore]
        public Texture Texture;

        public override string ToString()
        {
            return $"№{Order} - {Type}";
        }
    }
}
