using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using static MediaPlayerWinUI.VideoLibrary;
using Newtonsoft.Json;
using Windows.Media.Core;
using Windows.Storage.Pickers;
using Windows.Storage;
using Google.Protobuf;
using SharpCompress.Common;

namespace MediaPlayerWinUI
{
    public sealed partial class PlayQueue : Page
    {
        public static PlayQueue InstancePlayQueue { get; private set; }
        public PlayQueue()
        {
            this.InitializeComponent();
            InstancePlayQueue = this;
            LoadMediaFilesToGridView();
            LoadPlaylistPlayQueue();
            if (MyListBox.Items.Count == 0)
            {
                ClearPlayQueue_btn.IsEnabled = false;
                AddPlayQueue_btn.IsEnabled = false;
            }
            else
            {
                ClearPlayQueue_btn.IsEnabled = true;
                AddPlayQueue_btn.IsEnabled = true;
            }
        }
        public class GridViewItemPlayQueue
        {
            public string FileId { get; set; }
            public string FileName { get; set; }
            public string FilePath { get; set; }
            public string FileType { get; set; }
            public string Duration { get; set; }
            public string Title { get; set; }
            public string Artist { get; set; }
            public string Genre { get; set; }
            public string Year { get; set; }
            public string DateCreated { get; set; }
            public string DateModified { get; set; }
            public bool IsChecked { get; set; }
        }

        public class Playlist
        {
            public string PlaylistName { get; set; }
            public List<GridViewItemPlayQueue> MediaItems { get; set; } = new List<GridViewItemPlayQueue>();
            public bool IsChecked { get; set; }
        }

        private void AddPlayQueueToPlaylist(object sender, RoutedEventArgs e)
        {
            try
            {
                var playlistName = (sender as TextBlock).Text;
                var songsToAdd = MyListBox.Items.Cast<GridViewItemPlayQueue>().ToList();
                string playlistsJsonFilePath = @"C:\MediaFiles\Playlists.json";
                List<Playlist> playlists = new List<Playlist>();
                if (File.Exists(playlistsJsonFilePath))
                {
                    string jsonContent = File.ReadAllText(playlistsJsonFilePath);
                    playlists = JsonConvert.DeserializeObject<List<Playlist>>(jsonContent) ?? new List<Playlist>();
                }
                var selectedPlaylist = playlists.FirstOrDefault(p => p.PlaylistName == playlistName);
                if (selectedPlaylist != null)
                {
                    selectedPlaylist.MediaItems.AddRange(songsToAdd);
                    string updatedJsonContent = JsonConvert.SerializeObject(playlists, Formatting.Indented);
                    File.WriteAllText(playlistsJsonFilePath, updatedJsonContent);
                }
            }
            catch (Exception ex)
            {
            }
        }
        public void LoadPlaylistPlayQueue()
        {
            try
            {
                string targetDirectory = @"C:\MediaFiles";
                string jsonFilePath = Path.Combine(targetDirectory, "Playlists.json");
                if (File.Exists(jsonFilePath))
                {
                    string jsonContent = File.ReadAllText(jsonFilePath);
                    var playlists = JsonConvert.DeserializeObject<List<Playlist>>(jsonContent);
                    PlaylistItems.ItemsSource = playlists;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading playlists: {ex.Message}");
            }
        }

        private async void OpenFilesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(MainWindow.Instance);
                var picker = new FileOpenPicker();

                WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

                picker.SuggestedStartLocation = PickerLocationId.MusicLibrary;

                picker.FileTypeFilter.Add(".mp4");
                picker.FileTypeFilter.Add(".mp3");
                picker.FileTypeFilter.Add(".avi");
                picker.FileTypeFilter.Add(".mkv");
                picker.FileTypeFilter.Add(".mov");
                picker.FileTypeFilter.Add(".wmv");

                StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    string targetDirectory = @"C:\MediaFiles";
                    var tagFile = TagLib.File.Create(file.Path);
                    var fileInfo = new
                    {
                        FileId = Guid.NewGuid().ToString(),
                        FileName = file.Name,
                        FilePath = file.Path,
                        FileType = file.FileType,
                        Duration = tagFile.Properties.Duration.ToString(@"hh\:mm\:ss"),
                        Title = tagFile.Tag.Title ?? "Unknown Title",
                        Artist = tagFile.Tag.FirstPerformer ?? "Unknown Artist",
                        Genre = tagFile.Tag.FirstGenre ?? "Unknown Genre",
                        Year = tagFile.Tag.Year > 0 ? tagFile.Tag.Year.ToString() : "Unknown Year",
                        DateCreated = File.GetCreationTime(file.Path).ToString("yyyy-MM-dd HH:mm:ss"),
                        DateModified = File.GetLastWriteTime(file.Path).ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    string jsonFilePath = Path.Combine(targetDirectory, "PlayQueue.json");
                    List<object> fileList = new List<object>();
                    if (File.Exists(jsonFilePath))
                    {
                        string existingJson = File.ReadAllText(jsonFilePath);
                        fileList = JsonConvert.DeserializeObject<List<object>>(existingJson) ?? new List<object>();
                    }
                    fileList.Add(fileInfo);
                    string json = JsonConvert.SerializeObject(fileList, Formatting.Indented);
                    File.WriteAllText(jsonFilePath, json);

                    var mediaPlayerElement = MainWindow.PlayerElement;
                    mediaPlayerElement.Source = MediaSource.CreateFromUri(new Uri(file.Path));
                    string extension = Path.GetExtension(file.Path).ToLower();
                    if (extension == ".mp4" || extension == ".avi" || extension == ".mkv" || extension == ".mov" || extension == ".wmv")
                    {
                        mediaPlayerElement.Height = double.NaN;
                    }
                    LoadMediaFilesToGridView();
                    if (MyListBox.Items.Count == 0)
                    {
                        ClearPlayQueue_btn.IsEnabled = false;
                        AddPlayQueue_btn.IsEnabled = false;
                    }
                    else
                    {
                        ClearPlayQueue_btn.IsEnabled = true;
                        AddPlayQueue_btn.IsEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening file: {ex.Message}");
            }
        }

        private void LoadMediaFilesToGridView()
        {
            try
            {
                string targetDirectory = @"C:\MediaFiles";
                string jsonFilePath = Path.Combine(targetDirectory, "PlayQueue.json");

                if (File.Exists(jsonFilePath))
                {
                    string jsonContent = File.ReadAllText(jsonFilePath);

                    var fileList = JsonConvert.DeserializeObject<List<GridViewItemPlayQueue>>(jsonContent);

                    if (fileList != null)
                    {
                        MyListBox.ItemsSource = fileList;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading media files: {ex.Message}");
            }
        }

        private void OnClickPlayMediaFiles(object sender, RoutedEventArgs e)
        {
            var selectedItem = (GridViewItemPlayQueue)((Button)sender).DataContext;

            if (selectedItem != null)
            {
                string filePath = selectedItem.FilePath;

                if (File.Exists(filePath))
                {
                    var mediaPlayerElement = MainWindow.PlayerElement;
                    mediaPlayerElement.Source = MediaSource.CreateFromUri(new Uri(filePath));
                    string extension = Path.GetExtension(filePath).ToLower();
                    if (extension == ".mp4" || extension == ".avi" || extension == ".mkv" || extension == ".mov" || extension == ".wmv")
                    {
                        mediaPlayerElement.Height = double.NaN;
                    }
                }
            }
        }

        private void StackPanel_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel; // Lấy StackPanel hiện tại
            if (stackPanel != null)
            {
                var Icon1 = stackPanel.FindName("Icon1") as FontIcon;
                var CheckBox1 = stackPanel.FindName("CheckBox_PlayQueue") as CheckBox;
                var Icon2 = stackPanel.FindName("Icon2") as FontIcon;
                var PlayButton = stackPanel.FindName("PlayButton") as Button;

                if (Icon1 != null) Icon1.Visibility = Visibility.Collapsed;
                if (CheckBox1 != null) CheckBox1.Visibility = Visibility.Visible;
                if (Icon2 != null) Icon2.Visibility = Visibility.Collapsed;
                if (PlayButton != null) PlayButton.Visibility = Visibility.Visible;
            }
        }
        private void StackPanel_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel;
            if (stackPanel != null)
            {
                var Icon1 = stackPanel.FindName("Icon1") as FontIcon;
                var CheckBox1 = stackPanel.FindName("CheckBox_PlayQueue") as CheckBox;
                var Icon2 = stackPanel.FindName("Icon2") as FontIcon;
                var PlayButton = stackPanel.FindName("PlayButton") as Button;
                if(CheckBox1.IsChecked == false)
                {
                    if (Icon1 != null) Icon1.Visibility = Visibility.Visible;
                    if (CheckBox1 != null) CheckBox1.Visibility = Visibility.Collapsed;
                    if (Icon2 != null) Icon2.Visibility = Visibility.Visible;
                    if (PlayButton != null) PlayButton.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void CheckBox_PlayQueue_Checked(object sender, RoutedEventArgs e)
        {
            int checkedCount_PlayQueue = GetCheckedCheckBoxCount_PlayQueue();
            PlayQueue_btn.Visibility = Visibility.Collapsed;
            CheckBoxBar_PlayQueue.Visibility = Visibility.Visible;
        }
        private void CheckBox_PlayQueue_UnChecked(object sender, RoutedEventArgs e)
        {
            int checkedCount_AllVideos = GetCheckedCheckBoxCount_PlayQueue();
            if (checkedCount_AllVideos == 0)
            {
                PlayQueue_btn.Visibility = Visibility.Visible;
                CheckBoxBar_PlayQueue.Visibility = Visibility.Collapsed;
            }
            
        }
        private int GetCheckedCheckBoxCount_PlayQueue()
        {
            int checkedCount_PlayQueue = 0;

            var items = MyListBox.ItemsSource as IEnumerable<GridViewItemPlayQueue>;
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (item.IsChecked)
                    {
                        checkedCount_PlayQueue++;
                    }
                }
            }

            return checkedCount_PlayQueue;
        }

    }
}
