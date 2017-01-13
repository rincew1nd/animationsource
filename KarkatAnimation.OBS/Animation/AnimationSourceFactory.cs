using System;
using CLROBS;
using KarkatAnimation.Manager;
using KarkatAnimation.Settings;
using KarkatAnimation.OBS.Windows;

namespace KarkatAnimation.OBS.Animation
{
    public class AnimationSourceFactory : AbstractImageSourceFactory
    {
        private readonly AnimationManager _animationManager;

        public AnimationSourceFactory()
        {
            ClassName = "Animation Source";
            DisplayName = "Karkat Animation Source";
            
            _animationManager = new AnimationManager();
            SettingsManager.Load();
        }

        public override ImageSource Create(XElement data)
        {
            return new AnimationSource(_animationManager);
        }

        public override bool ShowConfiguration(XElement data)
        {
            var settingsControlDialog = new SettingsControl();
            return settingsControlDialog.ShowDialog().GetValueOrDefault(false);
        }
    }
}
