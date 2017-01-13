using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using CLROBS;
using KarkatAnimation.Manager;
using KarkatAnimation.Settings;

namespace KarkatAnimation.OBS.Animation
{
    class AnimationSource : AbstractImageSource
    {
        private readonly object _textureLock = new object();
        private readonly AnimationManager _animationManager;
        private Dictionary<VolumeType, List<AnimationImage>> _images;

        public AnimationSource(AnimationManager animationManager)
        {
            _animationManager = animationManager;
        }

        public override void BeginScene()
        {
            base.BeginScene();
            
            lock (_textureLock)
            {
                _images = SettingsManager.Settings.Images;
                UpdateTextures();
                _animationManager.StartMonitoring();
            }
        }

        public override void EndScene()
        {
            base.EndScene();
            _animationManager.StopMonitoring();
        }

        public void UpdateTextures()
        {
            foreach (var imageKeyValue in _images)
                foreach (var animationImage in imageKeyValue.Value)
                    animationImage.Texture = LoadTexture(animationImage.Path);
        }

        private Texture LoadTexture(string imageFile)
        {
            Texture texture = null;

            if (File.Exists(imageFile))
            {
                var src = new BitmapImage();

                src.BeginInit();
                src.UriSource = new Uri(imageFile);
                src.EndInit();

                var wb = new WriteableBitmap(src);

                texture = GS.CreateTexture((uint)wb.PixelWidth, (uint)wb.PixelHeight, GSColorFormat.GS_BGRA, null, false, false);

                texture.SetImage(wb.BackBuffer, GSImageFormat.GS_IMAGEFORMAT_BGRA, (uint)(wb.PixelWidth * 4));

                Size.X = wb.PixelWidth;
                Size.Y = wb.PixelHeight;
            }

            return texture;
        }

        public override void Render(float x, float y, float width, float height)
        {
            lock (_textureLock)
            {
                GS.DrawSprite(
                    _images[_animationManager.AnimationState.Key][_animationManager.AnimationState.Value].Texture,
                    0xFFFFFFFF, x, y, x + width, y + height);
            }
        }

        public override void UpdateSettings()
        {
            return;
        }
    }
}
