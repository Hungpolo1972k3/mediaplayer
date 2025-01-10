using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage.Pickers;
using System;
using Windows.Storage;
using Windows.Media.Core;
using Windows.System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Input;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Media.Playback;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.BulkAccess;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Newtonsoft.Json;
using System.Linq;

namespace MediaPlayerWinUI
{
    public sealed partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }
        public static MediaPlayerElement PlayerElement { get; private set; }
        public static ListBox PlaylistInstance { get; private set; }
        public MainWindow()
        {
            this.InitializeComponent();
            Instance = this;
            PlayerElement = this.mediaPlayerElement;
            PlaylistInstance = this.ListBox_Playlist;
            LoadPlaylistsMainWindow();
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
        public class GridViewItemModel
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
            public List<GridViewItemModel> MediaItems { get; set; } = new List<GridViewItemModel>();
            public int TotalItems => MediaItems.Count;

            public bool IsChecked { get; set; }
        }
        public void LoadPlaylistsMainWindow()
        {
            try
            {
                string targetDirectory = @"C:\MediaFiles";
                string jsonFilePath = Path.Combine(targetDirectory, "Playlists.json");
                if (File.Exists(jsonFilePath))
                {
                    string jsonContent = File.ReadAllText(jsonFilePath);
                    var playlists = JsonConvert.DeserializeObject<List<Playlist>>(jsonContent);
                    if (playlists != null && playlists.Count > 0)
                    {
                        ListBox_Playlist.ItemsSource = playlists;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("No playlists found in JSON file.");
                    }
                }
                else
                {
                    // Nếu file không tồn tại, tạo mới và ghi vào
                    System.Diagnostics.Debug.WriteLine("Playlists.json not found. Creating a new file.");

                    // Tạo danh sách Playlist rỗng
                    List<Playlist> emptyPlaylists = new List<Playlist>();

                    // Chuyển danh sách Playlist thành chuỗi JSON
                    string emptyJsonContent = JsonConvert.SerializeObject(emptyPlaylists, Formatting.Indented);

                    // Tạo thư mục nếu chưa có
                    Directory.CreateDirectory(targetDirectory);

                    // Ghi dữ liệu vào file Playlists.json
                    File.WriteAllText(jsonFilePath, emptyJsonContent);

                    // Ghi vào Debug để biết rằng file mới đã được tạo
                    System.Diagnostics.Debug.WriteLine("New Playlists.json file has been created.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading playlists: {ex.Message}");
            }
        }

        private void OnHomeClick(object sender, RoutedEventArgs e)
        {
            MusicLibrary.Visibility = Visibility.Collapsed;
            VideoLibrary.Visibility = Visibility.Collapsed;
            PlayQueue.Visibility = Visibility.Collapsed;
            PlayList.Visibility = Visibility.Collapsed;
            PlaylistElement.Visibility = Visibility.Collapsed;
            Home.Visibility = Visibility.Visible;
            Home.LoadMediaFilesToGridView();
        }
        private void ShowPlaylist_elements(object sender, RoutedEventArgs e)
        {
            if (Playlist_elements.Visibility == Visibility.Visible)
            {
                Playlist_elements.Visibility = Visibility.Collapsed;
            }
            else
            {
                Playlist_elements.Visibility = Visibility.Visible;
            }
            PlayList.Visibility = Visibility.Visible;
        }
        private void OnMusicLibraryClick(object sender, RoutedEventArgs e)
        {
            Home.Visibility = Visibility.Collapsed;
            VideoLibrary.Visibility = Visibility.Collapsed;
            PlayQueue.Visibility = Visibility.Collapsed;
            PlayList.Visibility = Visibility.Collapsed;
            PlaylistElement.Visibility = Visibility.Collapsed;
            MusicLibrary.Visibility = Visibility.Visible;
            MusicLibrary.LoadAudioToGridView();
            TappedRoutedEventArgs dummyEventArgs = new TappedRoutedEventArgs();
            MusicLibrary.SongChoose(sender, dummyEventArgs);
        }
        private void OnVideoLibraryClick(object sender, RoutedEventArgs e)
        {
            Home.Visibility = Visibility.Collapsed;
            MusicLibrary.Visibility = Visibility.Collapsed;
            PlayQueue.Visibility = Visibility.Collapsed;
            PlayList.Visibility = Visibility.Collapsed;
            PlaylistElement.Visibility = Visibility.Collapsed;
            VideoLibrary.Visibility = Visibility.Visible;
            VideoLibrary.LoadVideoToGridView();
            TappedRoutedEventArgs dummyEventArgs = new TappedRoutedEventArgs();
            VideoLibrary.AllVideosChoose(sender, dummyEventArgs);
        }
        private void OnPlayQueueClick(object sender, RoutedEventArgs e)
        {
            Home.Visibility = Visibility.Collapsed;
            MusicLibrary.Visibility = Visibility.Collapsed;
            VideoLibrary.Visibility = Visibility.Collapsed;
            PlayList.Visibility = Visibility.Collapsed;
            PlaylistElement.Visibility = Visibility.Collapsed;
            PlayQueue.Visibility = Visibility.Visible;
            PlayQueue.LoadPlaylistPlayQueue();
        }
        private void OnPlayListClick(object sender, RoutedEventArgs e)
        {
            Home.Visibility = Visibility.Collapsed;
            MusicLibrary.Visibility = Visibility.Collapsed;
            VideoLibrary.Visibility = Visibility.Collapsed;
            PlayQueue.Visibility = Visibility.Collapsed;
            PlaylistElement.Visibility = Visibility.Collapsed;
            PlayList.Visibility = Visibility.Visible;
            if (Playlist_elements.Visibility == Visibility.Collapsed)
            {
                Playlist_elements.Visibility = Visibility.Visible;
            }
            else
            {
                Playlist_elements.Visibility = Visibility.Collapsed;
            }
            LoadPlaylistsMainWindow();
            PlayList.LoadPlaylists();
            if (ListBox_Playlist.Items.Count == 0)
            {
                PlayList.PlaylistElementEmpty.Visibility = Visibility.Visible;
                PlayList.MainPlaylist.Visibility = Visibility.Collapsed;
                PlayList.PlaylistElementUI.Visibility = Visibility.Collapsed;
            }
            else
            {
                PlayList.PlaylistElementEmpty.Visibility = Visibility.Collapsed;
                PlayList.MainPlaylist.Visibility = Visibility.Visible;
                PlayList.PlaylistElementUI.Visibility = Visibility.Collapsed;
            }
        }
        private void OnPlaylistElementsClick(object sender, RoutedEventArgs e)
        {
            Home.Visibility = Visibility.Collapsed;
            MusicLibrary.Visibility = Visibility.Collapsed;
            VideoLibrary.Visibility = Visibility.Collapsed;
            PlayQueue.Visibility = Visibility.Collapsed;
            PlayList.Visibility = Visibility.Collapsed;
            PlaylistElement.Visibility= Visibility.Visible;
            string targetDirectory = @"C:\MediaFiles";
            string jsonFilePath = Path.Combine(targetDirectory, "Playlists.json");

            if (File.Exists(jsonFilePath))
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                var fileList = JsonConvert.DeserializeObject<List<Playlist>>(jsonContent);
                var selectedPlaylist = (sender as ListBox).SelectedItem as Playlist;
                var playlistInfo = fileList.FirstOrDefault(p => p.PlaylistName == selectedPlaylist.PlaylistName);
                if (selectedPlaylist != null)
                {
                    PlaylistNameText.Text = selectedPlaylist.PlaylistName;
                    TotalItemsText.Text = playlistInfo.TotalItems.ToString() + (playlistInfo.TotalItems.ToString() == "1" ? " item" : " items");
                    MyListBox_Playlist.ItemsSource = playlistInfo.MediaItems;
                    LoadPlaylistsMainWindow();
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("MediaFiles.json not found.");
            }
        }

        private async void OpenFile(object sender, RoutedEventArgs e)
        {
            try
            {
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
                var picker = new FileOpenPicker();
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

                picker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
                picker.FileTypeFilter.Add(".mp3");
                picker.FileTypeFilter.Add(".mp4");
                picker.FileTypeFilter.Add(".wav");
                picker.FileTypeFilter.Add(".avi");
                picker.FileTypeFilter.Add(".mkv");

                StorageFile file = await picker.PickSingleFileAsync();
                mediaPlayerElement.Source = MediaSource.CreateFromStorageFile(file);
                mediaPlayerElement.Height = double.NaN;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening file: {ex.Message}");
            }
        }
        private void OnHideVideoButtonClick(object sender, RoutedEventArgs e)
        {
            if (mediaPlayerElement.MediaPlayer != null)
            {
                var mediaPlayer = mediaPlayerElement.MediaPlayer;
                mediaPlayer.Pause();
                mediaPlayerElement.HorizontalAlignment = HorizontalAlignment.Left;
                mediaPlayerElement.VerticalAlignment = VerticalAlignment.Bottom;
                mediaPlayerElement.Width = 140;
                mediaPlayerElement.Height = 95;
                mediaPlayerElement.Margin = new Thickness(20);
                Control_btn.Visibility = Visibility.Collapsed;
            }
            ShowVideo_btn.Visibility = Visibility.Visible;
        }
        private void OnShowVideoButtonClick(object sender, RoutedEventArgs e)
        {
            if (mediaPlayerElement.MediaPlayer != null)
            {
                var mediaPlayer = mediaPlayerElement.MediaPlayer;
                mediaPlayer.Play();
                mediaPlayerElement.HorizontalAlignment = HorizontalAlignment.Center;
                mediaPlayerElement.VerticalAlignment = VerticalAlignment.Center;
                mediaPlayerElement.Width = double.NaN;
                mediaPlayerElement.Height = double.NaN;
                mediaPlayerElement.Margin = new Thickness(0);
                Control_btn.Visibility = Visibility.Visible;
            }
            ShowVideo_btn.Visibility = Visibility.Collapsed;
        }
        private void OnRepeatButtonClick(object sender, RoutedEventArgs e)
        {
            if (mediaPlayerElement.MediaPlayer != null)
            {
                mediaPlayerElement.MediaPlayer.Position = TimeSpan.Zero;
                mediaPlayerElement.MediaPlayer.Play();
            }

        }
        private void OnStopButtonClick(object sender, RoutedEventArgs e)
        {
            if (mediaPlayerElement.MediaPlayer != null)
            {
                mediaPlayerElement.MediaPlayer.Pause();
                mediaPlayerElement.MediaPlayer.Position = TimeSpan.Zero;
            }
        }
        private void OnForwardButtonClick(object sender, RoutedEventArgs e)
        {
            if (mediaPlayerElement.MediaPlayer != null)
            {
                var mediaPlayer = mediaPlayerElement.MediaPlayer;
                var currentPosition = mediaPlayer.PlaybackSession.Position;
                var newPosition = currentPosition + TimeSpan.FromSeconds(10);
                if (newPosition < mediaPlayer.PlaybackSession.NaturalDuration)
                {
                    mediaPlayer.PlaybackSession.Position = newPosition;
                }
                else
                {
                    mediaPlayer.PlaybackSession.Position = mediaPlayer.PlaybackSession.NaturalDuration;
                }
            }
        }
        private void OnRewindButtonClick(object sender, RoutedEventArgs e)
        {
            if (mediaPlayerElement.MediaPlayer != null)
            {
                var mediaPlayer = mediaPlayerElement.MediaPlayer;
                var currentPosition = mediaPlayer.PlaybackSession.Position;
                var newPosition = currentPosition - TimeSpan.FromSeconds(10);
                if (newPosition > TimeSpan.Zero)
                {
                    mediaPlayer.PlaybackSession.Position = newPosition;
                }
                else
                {
                    mediaPlayer.PlaybackSession.Position = TimeSpan.Zero;
                }
            }
        }

        private void OnClickPlaySong(object sender, RoutedEventArgs e)
        {
            var selectedItem = (GridViewItemModel)((Button)sender).DataContext;

            if (selectedItem != null)
            {
                string filePath = selectedItem.FilePath;

                if (File.Exists(filePath))
                {
                    mediaPlayerElement.Source = MediaSource.CreateFromUri(new Uri(filePath));
                    string extension = Path.GetExtension(filePath).ToLower();

                    if (extension == ".mp4" || extension == ".avi" || extension == ".mkv" || extension == ".mov" || extension == ".wmv")
                    {
                        mediaPlayerElement.Height = double.NaN;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"File not found: {filePath}");
                }
            }
        }

        private void CheckBox_Playlist_Element_Checked(object sender, RoutedEventArgs e)
        {
            int checkedCount_Playlist_Element = GetCheckedCheckBoxCount_Playlist_Element();
            CheckedCount_Playlist_Element.Text = checkedCount_Playlist_Element.ToString();
            CheckBoxBar_Playlist_Element.Visibility = Visibility.Visible;
        }
        private void CheckBox_Playlist_Element_Unchecked(object sender, RoutedEventArgs e)
        {
            int checkedCount_Playlist_Element = GetCheckedCheckBoxCount_Playlist_Element();
            CheckedCount_Playlist_Element.Text = checkedCount_Playlist_Element.ToString();
            if (checkedCount_Playlist_Element == 0)
            {
                CheckBoxBar_Playlist_Element.Visibility = Visibility.Collapsed;
            }
        }
        private int GetCheckedCheckBoxCount_Playlist_Element()
        {
            int checkedCount_Playlist_Element = 0;

            foreach (var item in MyListBox_Playlist.Items)
            {
                var gridViewItem = item as GridViewItemModel;
                if (gridViewItem != null && gridViewItem.IsChecked)
                {
                    checkedCount_Playlist_Element++;
                }
            }
            return checkedCount_Playlist_Element;
        }
        private void StackPanel_Playlist_Element_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel;
            if (stackPanel != null)
            {
                var Icon1 = stackPanel.FindName("Icon1") as FontIcon;
                var CheckBox1 = stackPanel.FindName("CheckBox1") as CheckBox;
                var Icon2 = stackPanel.FindName("Icon2") as FontIcon;
                var PlayButton = stackPanel.FindName("PlayButton") as Button;

                if (Icon1 != null) Icon1.Visibility = Visibility.Collapsed;
                if (CheckBox1 != null) CheckBox1.Visibility = Visibility.Visible;
                if (Icon2 != null) Icon2.Visibility = Visibility.Collapsed;
                if (PlayButton != null) PlayButton.Visibility = Visibility.Visible;
            }
        }
        private void StackPanel_Playlist_Element_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel;
            if (stackPanel != null)
            {
                var Icon1 = stackPanel.FindName("Icon1") as FontIcon;
                var CheckBox1 = stackPanel.FindName("CheckBox1") as CheckBox;
                var Icon2 = stackPanel.FindName("Icon2") as FontIcon;
                var PlayButton = stackPanel.FindName("PlayButton") as Button;
                if (CheckBox1.IsChecked == false)
                {
                    if (Icon1 != null) Icon1.Visibility = Visibility.Visible;
                    if (CheckBox1 != null) CheckBox1.Visibility = Visibility.Collapsed;
                    if (Icon2 != null) Icon2.Visibility = Visibility.Visible;
                    if (PlayButton != null) PlayButton.Visibility = Visibility.Collapsed;
                }
            }
        }

        private async void AddFileToPlayList(object sender, RoutedEventArgs e)
        {
            try
            {
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
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
                    string targetDirectory = @"C:\Playlist";
                    Directory.CreateDirectory(targetDirectory);
                    string targetFilePath = Path.Combine(targetDirectory, file.Name);
                    using (var sourceStream = await file.OpenStreamForReadAsync())
                    using (var destinationStream = new FileStream(targetFilePath, FileMode.Create, FileAccess.Write))
                    {
                        await sourceStream.CopyToAsync(destinationStream);
                    }

                    var tagFile = TagLib.File.Create(targetFilePath);
                    var fileInfo = new GridViewItemModel
                    {
                        FileId = Guid.NewGuid().ToString(),
                        FileName = file.Name,
                        FilePath = targetFilePath,
                        FileType = file.FileType,
                        Duration = tagFile.Properties.Duration.ToString(@"hh\:mm\:ss"),
                        Title = tagFile.Tag.Title ?? "Unknown Title",
                        Artist = tagFile.Tag.FirstPerformer ?? "Unknown Artist",
                        Genre = tagFile.Tag.FirstGenre ?? "Unknown Genre",
                        Year = tagFile.Tag.Year > 0 ? tagFile.Tag.Year.ToString() : "Unknown Year",
                        DateCreated = File.GetCreationTime(targetFilePath).ToString("yyyy-MM-dd HH:mm:ss"),
                        DateModified = File.GetLastWriteTime(targetFilePath).ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    string jsonFilePath = Path.Combine(targetDirectory, "Playlists.json");
                    List<Playlist> playlistList = new List<Playlist>();

                    if (File.Exists(jsonFilePath))
                    {
                        string existingJson = File.ReadAllText(jsonFilePath);
                        playlistList = JsonConvert.DeserializeObject<List<Playlist>>(existingJson) ?? new List<Playlist>();
                    }

                    var existingPlaylist = playlistList.FirstOrDefault(p => p.PlaylistName == PlaylistNameText.Text);
                    if (existingPlaylist != null)
                    {
                        existingPlaylist.MediaItems.Add(fileInfo);
                        MyListBox_Playlist.ItemsSource = existingPlaylist.MediaItems;
                        TotalItemsText.Text = existingPlaylist.TotalItems.ToString() + (existingPlaylist.TotalItems.ToString() == "1" ? " item" : " items");
                    }
          
                    string json = JsonConvert.SerializeObject(playlistList, Formatting.Indented);
                    File.WriteAllText(jsonFilePath, json);
                    mediaPlayerElement.Source = MediaSource.CreateFromUri(new Uri(targetFilePath));
                    string extension = Path.GetExtension(targetFilePath).ToLower();
                    if (extension != ".mp3")
                    {
                        mediaPlayerElement.Height = double.NaN;
                    }
                    LoadPlaylistsMainWindow();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No file selected.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening file: {ex.Message}");
            }
        }

        private void OnClickPlayAll(object sender, RoutedEventArgs e)
        {
            var PlaylistName = PlaylistNameText.Text.Trim();
            string targetDirectory = @"C:\Playlist";
            string jsonFilePath = Path.Combine(targetDirectory, "Playlists.json");

            if (File.Exists(jsonFilePath))
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                var fileList = JsonConvert.DeserializeObject<List<Playlist>>(jsonContent);
                var selectedPlaylist = fileList.FirstOrDefault(p => p.PlaylistName == PlaylistName);
                if (selectedPlaylist != null)
                {
                    foreach (var playlist in selectedPlaylist.MediaItems)
                    {
                        var filePath = playlist.FilePath;
                        mediaPlayerElement.Source = MediaSource.CreateFromUri(new Uri(filePath));
                        string extension = Path.GetExtension(filePath).ToLower();

                        if (extension == ".mp4" || extension == ".avi" || extension == ".mkv" || extension == ".mov" || extension == ".wmv")
                        {
                            mediaPlayerElement.Height = double.NaN;
                        }
                    }
                }
            }
        }
    }
}