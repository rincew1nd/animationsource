using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using KarkatAnimation.Settings;

namespace KarkatAnimation.OBS.Windows
{
    /// <summary>
    /// Interaction logic for ImagesSettings.xaml
    /// </summary>
    public partial class ImagesSettings : Window
    {
        private readonly SettingsObj _settings;

        private readonly Dictionary<string, int> _volumes;

        public ImagesSettings()
        {
            InitializeComponent();

            _settings = SettingsManager.Settings;
            ImageList.IsEnabled = VolumeTypeList.IsEnabled = Order.IsEnabled = SaveImageInfoButton.IsEnabled = false;
            
            if (_settings.Images == null)
                _settings.Images = new List<AnimationImage>();

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
            VolumeTypeList.IsEnabled = true;
            Order.IsEnabled = true;
            SaveImageInfoButton.IsEnabled = true;

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
            _settings.Images.Remove(GetSelectedImage());
            SettingsManager.Save();
            UpdateImageList();

            if (ImageList.Items.Count <= 0)
            {
                ImageList.IsEnabled = VolumeTypeList.IsEnabled = Order.IsEnabled = false;
                VolumeTypeList.SelectedIndex = 0;
                Order.Value = 0;
                ImagePathBox.Text = "";
            }
            else
                ImageList.SelectedIndex = 0;

            VolumeTypeList_SelectedIndexChanged(null, null);
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
                Path = ImagePathBox.Text
            };

            if (originalImage != null)
            {
                if (originalImage.Type != animationImage.Type)
                {
                    _settings.Images
                        .Where(
                            a => a.Type == originalImage.Type &&
                                 a.Order > originalImage.Order)
                        .ToList()
                        .ForEach(a => a.Order = a.Order - 1);
                    _settings.Images
                        .Where(
                            a => a.Type == animationImage.Type &&
                                 a.Order >= animationImage.Order)
                        .ToList()
                        .ForEach(a => a.Order = a.Order + 1);
                }
                else
                {
                    if (animationImage.Order != originalImage.Order)
                    {
                        if (animationImage.Order < originalImage.Order)
                        {
                            _settings.Images
                                .Where(
                                    a => a.Type == originalImage.Type &&
                                         a.Order >= animationImage.Order &&
                                         a.Order < originalImage.Order)
                                .ToList()
                                .ForEach(a => a.Order = a.Order + 1);
                        }
                        else
                        {
                            _settings.Images
                                .Where(
                                    a => a.Type == originalImage.Type &&
                                         a.Order <= animationImage.Order &&
                                         a.Order > originalImage.Order)
                                .ToList()
                                .ForEach(a => a.Order = a.Order - 1);
                        }
                    }
                }
            }
            else
            {
                var updateList = _settings.Images
                    .Where(
                        a => a.Type == animationImage.Type &&
                             a.Order == animationImage.Order)
                    .ToList();
                if (updateList.Count > 0)
                    updateList
                        .Where(im => im.Order >= animationImage.Order)
                        .ToList()
                        .ForEach(a => a.Order = a.Order + 1);
            }

            if (originalImage == null)
            {
                _settings.Images.Add(animationImage);
                ImageList.Items.Add(animationImage);
                ImageList.IsEnabled = true;
                ImageList.SelectedIndex = 0;
            }
            else
            {
                originalImage.Type = animationImage.Type;
                originalImage.Order = animationImage.Order;
                originalImage.Path = animationImage.Path;
            }

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
            var maximum = GetMaximumOrder(type);

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
                ImagePathBox.Text = "";
            }
            else
            {
                VolumeTypeList.SelectedIndex = _volumes[image.Type.ToString()];
                Order.Maximum = _settings.Images
                    .Where(im => im.Type == image.Type)
                    .Select(i => i.Order)
                    .Max();
                Order.Value = image.Order;
                ImagePathBox.Text = image.Path;
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
                _settings.Images
                    .OrderBy(im => im.Type)
                    .ThenBy(im => im.Order)
                    .ToList()
                    .ForEach(im => ImageList.Items.Add(im));
                ImageList.SelectedIndex = 0;
                ImageList.IsEnabled = true;
                SaveImageInfoButton.IsEnabled = true;
            }
        }

        /// <summary>
        /// Получить максимальный порядок по VolumeType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetMaximumOrder(VolumeType type)
        {
            if (_settings.Images.Count(im => im.Type == type) == 0)
                return -1;

            return _settings.Images
                .Where(im => im.Type == type)
                .Select(im => im.Order)
                .Max();
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
