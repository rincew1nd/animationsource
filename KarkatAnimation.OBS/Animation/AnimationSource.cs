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
    /// <summary>
    /// Animation source for OBS
    /// </summary>
    class AnimationSource : AbstractImageSource
    {
        /// <summary>
        /// Lock for drawing\loading\updating texture
        /// </summary>
        private readonly object _textureLock = new object();

        /// <summary>
        /// Manager for calculating animation state based on microphone volume
        /// </summary>
        private readonly AnimationManager _animationManager;

        /// <summary>
        /// Animation frames store
        /// </summary>
        private Dictionary<VolumeType, List<AnimationImage>> _images;

        public AnimationSource(AnimationManager animationManager)
        {
            _animationManager = animationManager;
        }

        /// <summary>
        /// Method called on animation source activation
        /// Loading textures and start animation manager
        /// </summary>
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

        /// <summary>
        /// Method called on animation source deactivation
        /// Stop animation manager
        /// </summary>
        public override void EndScene()
        {
            base.EndScene();
            _animationManager.StopMonitoring();
        }

        /// <summary>
        /// Loading texture for every animation frames
        /// </summary>
        public void UpdateTextures()
        {
            foreach (var imageKeyValue in _images)
                foreach (var animationImage in imageKeyValue.Value)
                    animationImage.Texture = LoadTexture(animationImage.Path);
        }

        /// <summary>
        /// Load texture for animation from AnimationImage.Path
        /// </summary>
        /// <param name="imageFile"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Render texture to OBS stream
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="width">Animation width</param>
        /// <param name="height">Animation height</param>
        public override void Render(float x, float y, float width, float height)
        {
            lock (_textureLock)
            {
                GS.DrawSprite(
                    _images[_animationManager.AnimationState.Key][_animationManager.AnimationState.Value].Texture,
                    0xFFFFFFFF, x, y, x + width, y + height);
            }
        }

        /// <summary>
        /// IDK...
        /// </summary>
        public override void UpdateSettings()
        {
            return;
        }
    }
}
