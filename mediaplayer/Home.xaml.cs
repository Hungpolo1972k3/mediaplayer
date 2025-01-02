using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Media.Core;
using Windows.System;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Media.Playback;


namespace MediaPlayerWinUI
{
    public sealed partial class Home : Page
    {
        private FileService _fileService;
        public Home()
        {
            this.InitializeComponent();
            _fileService = new FileService();
            var items = new List<FileInformation>
            {
                new FileInformation {Title = "nguyễn thọ hùng" },
                new FileInformation {Title = "ABCD" },
            };
            MyGridView_Home.ItemsSource = items;
        }
        private class FileInformation
        {
            public string FilePath { get; set; }
            public string Role { get; set; }
            public string Title { get; set; }
            public string Artist { get; set; }
            public string AlbumId { get; set; }
            public string PlaylistId { get; set; }
            public string Genre { get; set; }
            public TimeSpan Runtime { get; set; }
            public bool IsChecked { get; set; }

        }
        private async void OnAddFileClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
                var picker = new FileOpenPicker();

                picker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
                picker.FileTypeFilter.Add(".mp3");
                picker.FileTypeFilter.Add(".mp4");
                picker.FileTypeFilter.Add(".wav");
                picker.FileTypeFilter.Add(".avi");
                picker.FileTypeFilter.Add(".mkv");

                StorageFile file = await picker.PickSingleFileAsync();

                var fileInfo = await GetFileMetadataAsync(file);
                string fileId = await _fileService.AddFileAsync(
                    fileInfo.FilePath,
                    fileInfo.Role,
                    fileInfo.Artist,
                    fileInfo.Title,
                    fileInfo.AlbumId,
                    fileInfo.PlaylistId,
                    fileInfo.Genre,
                    fileInfo.Runtime
                );
                System.Diagnostics.Debug.WriteLine($"File added successfully with ID: {fileId}");
                LoadFiles();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding file: {ex.Message}");
            }
        }
        private async Task<FileInformation> GetFileMetadataAsync(StorageFile file)
        {
            var fileInfo = new FileInformation
            {
                FilePath = file.Path,
                Role = file.FileType == ".mp3" || file.FileType == ".wav" ? "audio" : "video"
            };

            var properties = await file.Properties.GetMusicPropertiesAsync();

            fileInfo.Title = string.IsNullOrEmpty(properties.Title) ? file.DisplayName : properties.Title;
            fileInfo.Artist = string.IsNullOrEmpty(properties.Artist) ? "Unknown Artist" : properties.Artist;
            fileInfo.AlbumId = null;
            fileInfo.Genre = properties.Genre.Count > 0 ? string.Join(", ", properties.Genre) : "Unknown Genre";
            fileInfo.Runtime = properties.Duration;
            fileInfo.PlaylistId = null;

            return fileInfo;
        }
        private async void LoadFiles()
        {
            try
            {
                var files = await _fileService.GetAllFilesAsync();

                var gridViewItems = new List<FileInformation>();

                foreach (var file in files)
                {
                    gridViewItems.Add(new FileInformation
                    {
                        Title = file.Title,
                        IsChecked = false
                    });
                }
                MyGridView_Home.ItemsSource = gridViewItems;
                if(MyGridView_Home.ItemsSource != null)
                {
                    Home_empty.Visibility = Visibility.Collapsed;
                    RecentMediaText.Visibility = Visibility.Visible;
                    MyGridView_Home.Visibility = Visibility.Visible;
                }
                else
                {
                    Home_empty.Visibility = Visibility.Visible;
                    RecentMediaText.Visibility = Visibility.Collapsed;
                    MyGridView_Home.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading files: {ex.Message}");
            }
        }


        private void Home_elements_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel; // Lấy StackPanel hiện tại
            if (stackPanel != null)
            {
                var checkBox = stackPanel.FindName("CheckBox_Home") as CheckBox;
                var buttonLeft = stackPanel.FindName("Button_Left") as Button;
                var buttonRight = stackPanel.FindName("Button_Right") as Button;
        
                if (checkBox != null) checkBox.Visibility = Visibility.Visible;
                if (buttonLeft != null) buttonLeft.Visibility = Visibility.Visible;
                if (buttonRight != null) buttonRight.Visibility = Visibility.Visible;
            }
        }

        private void Home_elements_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel; // Lấy StackPanel hiện tại
            if (stackPanel != null)
            {
                var checkBox = stackPanel.FindName("CheckBox_Home") as CheckBox;
                var buttonLeft = stackPanel.FindName("Button_Left") as Button;
                var buttonRight = stackPanel.FindName("Button_Right") as Button;

                if (checkBox.IsChecked == false)
                {
                    if (checkBox != null) checkBox.Visibility = Visibility.Collapsed;
                    if (buttonLeft != null) buttonLeft.Visibility = Visibility.Collapsed;
                    if (buttonRight != null) buttonRight.Visibility = Visibility.Collapsed;
                }
            }
        }
 
        private void CheckBox_Home_Checked(object sender, RoutedEventArgs e)
        {
            int checkedCount = GetCheckedCheckBoxCount();
            CheckedCount.Text = checkedCount.ToString();
            RecentMediaText.Visibility = Visibility.Collapsed;
            CheckBoxBar.Visibility = Visibility.Visible;
        }

        private void CheckBox_Home_Unchecked(object sender, RoutedEventArgs e)
        {
            int checkedCount = GetCheckedCheckBoxCount();
            CheckedCount.Text = checkedCount.ToString();
            if (checkedCount == 0)
            {
                RecentMediaText.Visibility = Visibility.Visible;
                CheckBoxBar.Visibility = Visibility.Collapsed;
            }
        }

        private int GetCheckedCheckBoxCount()
        {
            int checkedCount = 0;

            foreach (var item in MyGridView_Home.Items)
            {
                var gridViewItem = item as FileInformation;
                if (gridViewItem != null && gridViewItem.IsChecked)
                {
                    checkedCount++;
                }
            }

            return checkedCount;
        }

    }
}
