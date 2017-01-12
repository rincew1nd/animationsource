using CLROBS;
using KarkatAnimation.Manager;

namespace KarkatAnimation.OBS.Animation
{
    class AnimationSource : AbstractImageSource
    {
        private readonly AnimationManager _animationManager;

        public AnimationSource(AnimationManager animationManager)
        {
            _animationManager = animationManager;
        }

        public override void Render(float x, float y, float width, float height)
        {
            return;
        }

        public override void UpdateSettings()
        {
            return;
        }

        public override void BeginScene()
        {
            base.BeginScene();
            _animationManager.StartMonitoring();
        }

        public override void EndScene()
        {
            base.EndScene();
            _animationManager.StopMonitoring();
        }
    }
}
