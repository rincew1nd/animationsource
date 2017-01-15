using System;
using System.Windows;
using System.Windows.Controls;
using KarkatAnimation.Settings;
using NAudio.Wave;
using TAlex.WPF.Controls;

namespace KarkatAnimation.WPF.Windows
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsControl : Window
    {
        private readonly SettingsObj _settings;
        private bool _isInit;

        public SettingsControl()
        {
            _isInit = true;
            _settings = SettingsManager.Load();

            InitializeComponent();
            
            LoadInputAudioDevices();
            LoadOldValues();
            _isInit = false;
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
            PeakDeltaSlider.Value = (double)_settings.SampleDelta;
            PortTextbox.Value = _settings.Port;
            AudioHzTextbox.Value = _settings.AudioHz;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isInit) return;
            var slider = sender as Slider;
            if (slider == null) return;

            switch (slider.Name)
            {
                case "SilenceSlider":
                    _settings.Silence = (int) e.NewValue;
                    break;
                case "SpeakingSlider":
                    _settings.Speaking = (int)e.NewValue;
                    break;
                case "ShoutingSlider":
                    _settings.Shouting = (int)e.NewValue;
                    break;
                case "UpdateTimeSlider":
                    _settings.UpdateTime = (int)e.NewValue;
                    break;
                case "PeakDeltaSlider":
                    _settings.SampleDelta = (decimal)e.NewValue;
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
            _settings.Port = (int)PortTextbox.Value;
            _settings.AudioHz = (int)AudioHzTextbox.Value;

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
