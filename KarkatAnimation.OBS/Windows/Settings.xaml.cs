using System;
using System.Windows;
using System.Windows.Controls;
using KarkatAnimation.Settings;
using NAudio.Wave;

namespace KarkatAnimation.OBS.Windows
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsControl : Window
    {
        private SettingsObj _settings;

        public SettingsControl()
        {
            InitializeComponent();

            _settings = SettingsManager.Settings;

            LoadInputAudioDevices();
            LoadOldValues();
        }

        private void LoadInputAudioDevices()
        {
            for (var waveInDevice = 0; waveInDevice < WaveIn.DeviceCount; waveInDevice++)
            {
                var deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                var comboboxItem = $"Device {waveInDevice}: {deviceInfo.ProductName}, {deviceInfo.Channels} channels";
                ListOfMicrophones.Items.Add(comboboxItem);
            }
        }

        private void LoadOldValues()
        {
            var lastUsedDevice = _settings.LastUsedDevice;
            if (WaveIn.DeviceCount >= lastUsedDevice)
                ListOfMicrophones.SelectedIndex = lastUsedDevice;
            else if (WaveIn.DeviceCount >= 0)
                ListOfMicrophones.SelectedIndex = 0;

            SilenceSlider.Value = _settings.Silence;
            SpeakingSlider.Value = _settings.Speaking;
            ShoutingSlider.Value = _settings.Shouting;
            UpdateTimeSlider.Value = _settings.UpdateTime;
            PeakDeltaSlider.Value = (double)_settings.PeakDelta;

            SilenceSliderLabel.Content = SilenceSlider.Value;
            SpeakingSliderLabel.Content = SpeakingSlider.Value;
            ShoutingSliderLabel.Content = ShoutingSlider.Value;
            UpdateTimeSliderLabel.Content = UpdateTimeSlider.Value;
            PeakDeltaSliderLabel.Content = PeakDeltaSlider.Value;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = sender as Slider;
            if (slider == null) return;

            switch (slider.Name)
            {
                case "SilenceSlider":
                    SilenceSliderLabel.Content = (int)e.NewValue;
                    _settings.Silence = (int) e.NewValue;
                    break;
                case "SpeakingSlider":
                    SpeakingSliderLabel.Content = (int)e.NewValue;
                    _settings.Speaking = (int)e.NewValue;
                    break;
                case "ShoutingSlider":
                    ShoutingSliderLabel.Content = (int)e.NewValue;
                    _settings.Shouting = (int)e.NewValue;
                    break;
                case "UpdateTimeSlider":
                    UpdateTimeSliderLabel.Content = (int)e.NewValue;
                    _settings.UpdateTime = (int)e.NewValue;
                    break;
                case "PeakDeltaSlider":
                    PeakDeltaSliderLabel.Content = e.NewValue;
                    _settings.PeakDelta = (decimal)e.NewValue;
                    break;
                default:
                    break;
            }
            
        }

        private void ImageSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.Save();
            new ImagesSettings().ShowDialog();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.Save();
            DialogResult = true;
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void ListOfMicrophones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _settings.LastUsedDevice = ListOfMicrophones.SelectedIndex;
        }
    }
}
