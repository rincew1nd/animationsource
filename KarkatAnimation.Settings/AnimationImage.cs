using CLROBS;
using Newtonsoft.Json;

namespace KarkatAnimation.Settings
{
    public class AnimationImage
    {
        /// <summary>
        /// Path to image
        /// </summary>
        public string Path;

        /// <summary>
        /// Type of animation
        /// </summary>
        public VolumeType Type;

        /// <summary>
        /// Animation order in animation cycle
        /// </summary>
        public int Order;

        /// <summary>
        /// Image information for OBS
        /// </summary>
        [JsonIgnore]
        public Texture Texture;

        /// <summary>
        /// №{Order} - {Type}
        /// Example: №1 - Shouting
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"№{Order} - {Type}";
        }
    }
}
