﻿using Dormitory.Models;
using System;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Dormitory.Views
{
    public sealed partial class WriteJournal : Page
    {
        JournalItem jItem;

        public WriteJournal()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
        }

        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            jItem = null;
            Frame.Navigate(typeof(Info), jItem);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //返回的条件设置
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame.CanGoBack)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            }
            else
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }
            jItem = ((JournalItem)e.Parameter);
            if (jItem == null)
            {
                jItem = new JournalItem();

                createButton.Content = "发布";
                cancelButton.Content = "清空";
            }
            else
            {
                createButton.Content = "更新";
                cancelButton.Content = "删除";
                createButton.Click -= CreateButton_Clicked;
                createButton.Click += UpdateButton_Clicked;

                cancelButton.Click -= ClearButton_Clicked;
                cancelButton.Click += DeleteButton_Clicked;

                Details.Text = jItem.content;
                photo.Source = new BitmapImage(jItem.pic);
            }
        }

        private void HomeAppButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Info), jItem);
        }

        private void CheckAppButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Checkbook));
        }

        private void DutyAppButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Duty));
        }

        private async void selectPhoto(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.DecodePixelWidth = 600;
                    await bitmapImage.SetSourceAsync(fileStream);
                    photo.Source = bitmapImage;
                    // 保存到临时文件夹
                    var fileToSave = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("temp.png", CreationCollisionOption.ReplaceExisting);
                    var stream = await file.OpenReadAsync();
                    var bytes = await  Temp.GetBytesFromStream(stream);
                    await FileIO.WriteBytesAsync(fileToSave, bytes);
                }
            }
            else
            {
                var i = new MessageDialog("select picture operation cancelled!").ShowAsync();
            }
        }

        private void CreateButton_Clicked(object sender, RoutedEventArgs e)
        {
            if(Details.Text == "")
            {
                var WrongMessage = new MessageDialog("内容不能为空!").ShowAsync();
            }else
            {
                var uri = (photo.Source as BitmapImage).UriSource;
                if(uri == null)
                {
                    jItem.ImageChange = false;
                    uri = new Uri("ms-appdata:///temp/temp.png");
                }
                var content = Details.Text;
                jItem.pic = uri;
                jItem.content = content;
                jItem.message = "create";
                Frame.Navigate(typeof(Info), jItem);
            }
        }

        private void UpdateButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (Details.Text == "")
            {
                var WrongMessage = new MessageDialog("内容不能为空!").ShowAsync();
            }
            else
            {
                var uri = (photo.Source as BitmapImage).UriSource;
                if (uri == null)
                {
                    jItem.ImageChange = false;
                    uri = new Uri("ms-appdata:///temp/temp.png");
                }
                var content = Details.Text;
                jItem.pic = uri;
                jItem.content = content;
                jItem.message = "update";
                Frame.Navigate(typeof(Info), jItem);
            }
        }

        private void DeleteButton_Clicked(object sender, RoutedEventArgs e)
        {
            jItem.message = "delete";
            Frame.Navigate(typeof(Info), jItem);
        }

        private void ClearButton_Clicked(object sender, RoutedEventArgs e)
        {
            Details.Text = "";
            Image image = photo as Image;
            BitmapImage bitmapImage = new BitmapImage();
            image.Width = bitmapImage.DecodePixelWidth = 350;
            bitmapImage.UriSource = new Uri(image.BaseUri, "Assets/default.jpg");
            photo.Source = bitmapImage;
        }
    }
}
