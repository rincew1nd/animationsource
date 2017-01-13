using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using KarkatAnimation.Settings;
using Microsoft.Win32;

namespace KarkatAnimation.OBS.Windows
{
    /// <summary>
    /// Interaction logic for ImagesSettings.xaml
    /// </summary>
    public partial class ImagesSettings : Window
    {
        private readonly SettingsObj _settings;
        private readonly Dictionary<string, int> _volumes;
        private string _tempPath;

        public ImagesSettings()
        {
            InitializeComponent();

            _settings = SettingsManager.Settings;
            ImageList.IsEnabled = VolumeTypeList.IsEnabled = Order.IsEnabled = SaveImageInfoButton.IsEnabled = false;

            if (_settings.Images == null)
                _settings.Images = new Dictionary<VolumeType, List<AnimationImage>>()
                {
                    {VolumeType.Silence, new List<AnimationImage>()},
                    {VolumeType.Speaking, new List<AnimationImage>()},
                    {VolumeType.Shouting, new List<AnimationImage>()}
                };

            _volumes = new Dictionary<string, int>()
            {
                { "Silence", 0 }, { "Speaking", 1 }, { "Shouting", 2 }
            };
            foreach (var volumesKey in _volumes.Keys)
                VolumeTypeList.Items.Add(volumesKey);
            VolumeTypeList.SelectedIndex = 0;
        }

        /// <summary>
        /// Событие открытия окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddImages_Load(object sender, RoutedEventArgs e)
        {
            UpdateImageList();
        }

        /// <summary>
        /// Событие нажатия на кнопку добавления изображения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddImageButton_Click(object sender, EventArgs e)
        {
            VolumeTypeList.IsEnabled = Order.IsEnabled = 
                SaveImageInfoButton.IsEnabled = OpenImageButton.IsEnabled = true;

            ImageList.SelectedIndex = -1;
            VolumeTypeList_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// Событие нажатия на кнопку удаления изображения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteImageButton_Click(object sender, EventArgs e)
        {
            var image = GetSelectedImage();
            _settings.Images[image.Type].RemoveAt(image.Order);

            SettingsManager.Save();
            UpdateImageList();

            if (ImageList.Items.Count <= 0)
            {
                ImageList.IsEnabled = VolumeTypeList.IsEnabled = Order.IsEnabled = false;
                VolumeTypeList.SelectedIndex = 0;
                Order.Value = 0;
                ImagePreview.Source = null;
            }
            else
                ImageList.SelectedIndex = 0;

            VolumeTypeList_SelectedIndexChanged(null, null);
        }

        private void OpenImage_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (!dialog.ShowDialog().GetValueOrDefault()) return;

            _tempPath = dialog.FileName;
            ImagePreview.Source = new BitmapImage(new Uri(_tempPath));
        }

        /// <summary>
        /// Событие нажатия на кнопку сохранения редактированного/нового изображения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, EventArgs e)
        {
            var originalImage = GetSelectedImage();
            var animationImage = new AnimationImage
            {
                Type = GetVolumeType(),
                Order = (int)Order.Value,
                Path = _tempPath
            };

            if (originalImage != null && _settings.Images[originalImage.Type].Count - 1 >= originalImage.Order)
                _settings.Images[originalImage.Type].RemoveAt(originalImage.Order);
            _settings.Images[animationImage.Type].Insert(animationImage.Order, animationImage);

            if (originalImage != null)
            for (var i = 0; i < _settings.Images[originalImage.Type].Count; i++)
                _settings.Images[originalImage.Type][i].Order = i;
            for (var i = 0; i < _settings.Images[animationImage.Type].Count; i++)
                _settings.Images[animationImage.Type][i].Order = i;

            SettingsManager.Save();
            UpdateImageList();
        }

        /// <summary>
        /// Событие изменения выбранной картинки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageList_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadImageInfo(GetSelectedImage());
        }

        /// <summary>
        /// Обновление порядка и максимального значения порядка при смене типа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VolumeTypeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var image = GetSelectedImage();
            var type = GetVolumeType();
            //Нет элементов = -1, Есть элементы = максимальный порядок.
            var maximum = _settings.Images.ContainsKey(type) ? _settings.Images[type].Count-1 : -1;

            if (maximum != -1)
            {
                if (image == null || image.Type != type)
                    maximum += 1;

                Order.IsEnabled = true;
                Order.Maximum = maximum;
                Order.Value = (image == null || image.Type != type) ?
                    maximum : image.Order;
            }
            else
            {
                Order.IsEnabled = false;
                Order.Maximum = 0;
                Order.Value = 0;
            }
        }

        /// <summary>
        /// Загрузка значений картинки в компоненты
        /// </summary>
        /// <param name="image"></param>
        private void LoadImageInfo(AnimationImage image)
        {
            ImageList.IsEnabled = VolumeTypeList.IsEnabled = Order.IsEnabled = true;

            if (image == null)
            {
                VolumeTypeList.SelectedIndex = 0;
                Order.Value = 0;
                _tempPath = "";
                ImagePreview.Source = null;
            }
            else
            {
                VolumeTypeList.SelectedIndex = _volumes[image.Type.ToString()];
                Order.Maximum = _settings.Images[image.Type].Count-1;
                Order.Value = image.Order;
                _tempPath = image.Path;
                ImagePreview.Source = new BitmapImage(new Uri(image.Path));
            }
        }

        /// <summary>
        /// Обновление списка картинок после изменения значений
        /// </summary>
        private void UpdateImageList()
        {
            ImageList.Items.Clear();
            if (_settings.Images.Count > 0)
            {
                var lists = _settings.Images
                    .ToList()
                    .Select(im => im.Value)
                    .ToList();

                var list = new List<AnimationImage>();
                foreach (var listElem in lists)
                    list.AddRange(listElem);

                list.OrderBy(im => im.Type)
                    .ThenBy(im => im.Order)
                    .ToList()
                    .ForEach(im => ImageList.Items.Add(im));

                ImageList.SelectedIndex = 0;
                ImageList.IsEnabled = OpenImageButton.IsEnabled = true;
                SaveImageInfoButton.IsEnabled = true;
            }
            else
            {
                ImageList.IsEnabled = OpenImageButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Получить VolumeType из компонента
        /// </summary>
        /// <returns></returns>
        private AnimationImage GetSelectedImage()
        {
            var selectedItem = ImageList.SelectedItem as AnimationImage;
            if (selectedItem == null && ImageList.SelectedIndex != -1)
                throw new ArgumentException("Выбран неверный объект из ImageList!");
            return selectedItem;
        }

        /// <summary>
        /// Получить VolumeType из компонента
        /// </summary>
        /// <returns></returns>
        private VolumeType GetVolumeType()
        {
            var stringType = VolumeTypeList.SelectedItem as string;
            if (stringType == null)
                throw new ArgumentException("Неверный тип VolumeType!");
            return (VolumeType)_volumes[stringType];
        }
    }
}
