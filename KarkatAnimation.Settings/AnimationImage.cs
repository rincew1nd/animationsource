namespace KarkatAnimation.Settings
{
    public class AnimationImage
    {
        public string Path;
        public VolumeType Type;
        public int Order;

        public override string ToString()
        {
            return $"№{Order} - {Type}";
        }
    }
}
