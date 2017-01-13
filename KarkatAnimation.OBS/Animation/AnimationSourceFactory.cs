using System;
using CLROBS;
using KarkatAnimation.Manager;
using KarkatAnimation.Settings;
using KarkatAnimation.OBS.Windows;

namespace KarkatAnimation.OBS.Animation
{
    public class AnimationSourceFactory : AbstractImageSourceFactory
    {
        /// <summary>
        /// Manager for calculating animation state based on microphone volume
        /// </summary>
        private readonly AnimationManager _animationManager;

        /// <summary>
        /// Define source class name and display name
        /// Create animation manager
        /// </summary>
        public AnimationSourceFactory()
        {
            ClassName = "Animation Source";
            DisplayName = "Karkat Animation Source";
            
            _animationManager = new AnimationManager();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override ImageSource Create(XElement data)
        {
            return new AnimationSource(_animationManager);
        }

        /// <summary>
        /// Show WPF settings form (on source doubleclick)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool ShowConfiguration(XElement data)
        {
            var settingsControlDialog = new SettingsControl();
            return settingsControlDialog.ShowDialog().GetValueOrDefault(false);
        }
    }
}
