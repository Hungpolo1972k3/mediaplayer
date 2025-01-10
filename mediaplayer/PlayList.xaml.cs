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
using Newtonsoft.Json;
using static MediaPlayerWinUI.MusicLibrary;
using Windows.Media.Core;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Data;
using Google.Protobuf;
using Microsoft.UI.Xaml.Media;
using System.Net.Http.Json;
using SharpCompress.Common;
using CommunityToolkit.WinUI.UI.Controls;

namespace MediaPlayerWinUI
{

    public sealed partial class PlayList : Page
    {
        public static PlayList Instance { get; private set; }

        public static Grid MainPlaylist { get; private set; }
        public static Grid PlaylistElementEmpty { get; private set; }
        public static Grid PlaylistElementUI { get; private set; }
        public static TextBlock PlaylistName { get; private set; }
        public static TextBlock TotalItems { get; private set; }
        public static GridView PlaylistGridView {  get; private set; }
        public PlayList()
        {
            this.InitializeComponent();
            PlayList.Instance = this;
            PlaylistName = this.PlaylistNameText;
            TotalItems = this.TotalItemsText;
            MainPlaylist = this.Playlist_Main;
            PlaylistElementEmpty = this.Playlist_Empty;
            PlaylistElementUI = this.PlaylistElement;
            PlaylistGridView = this.MyGridView_Playlist;
            LoadPlaylists();
            if (MyGridView_Playlist.Items.Count == 0)
            {
                Playlist_Empty.Visibility = Visibility.Visible;
                Playlist_Main.Visibility = Visibility.Collapsed;
            }
            else
            {
                Playlist_Empty.Visibility = Visibility.Collapsed;
                Playlist_Main.Visibility = Visibility.Visible;
            }
        }
        public class GridViewItem
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
            public List<GridViewItem> MediaItems { get; set; } = new List<GridViewItem>();
            public int TotalItems => MediaItems.Count;
            public bool IsChecked { get; set; }
        }

        public void LoadPlaylists()
        {
            try
            {
                string targetDirectory = @"C:\MediaFiles";
                string jsonFilePath = Path.Combine(targetDirectory, "Playlists.json");
                if (File.Exists(jsonFilePath))
                {
                    string jsonContent = File.ReadAllText(jsonFilePath);
                    var playlists = JsonConvert.DeserializeObject<List<Playlist>>(jsonContent);
                    MyGridView_Playlist.ItemsSource = playlists;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading playlists: {ex.Message}");
            }
        }

        private void AddNewPlaylist(object sender, RoutedEventArgs e)
        {
            string targetDirectory = @"C:\MediaFiles";
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
            string playlistName = InputTextBox.Text.Trim();
            string playlistName1 = InputTextBox1.Text.Trim();
            string finalPlaylistName = !string.IsNullOrWhiteSpace(playlistName) ? playlistName : playlistName1;
            string jsonFilePath = Path.Combine(targetDirectory, "Playlists.json");
            List<Playlist> fileList = new List<Playlist>();
            if (File.Exists(jsonFilePath))
            {
                string existingJson = File.ReadAllText(jsonFilePath);
                fileList = JsonConvert.DeserializeObject<List<Playlist>>(existingJson) ?? new List<Playlist>();
            }
            bool isDuplicate = fileList.Any(p => p.PlaylistName.Equals(finalPlaylistName, StringComparison.OrdinalIgnoreCase));
            if (isDuplicate == false)
            {
                var newPlaylist = new Playlist
                {
                    PlaylistName = finalPlaylistName,
                    MediaItems = new List<GridViewItem>(),
                    IsChecked = false
                };

                fileList.Add(newPlaylist);
                string json = JsonConvert.SerializeObject(fileList, Formatting.Indented);
                File.WriteAllText(jsonFilePath, json);
                LoadPlaylists();
                Playlist_Empty.Visibility = Visibility.Collapsed;
                Playlist_Main.Visibility = Visibility.Visible;
                InputTextBox.Text = "";
                InputTextBox1.Text = "";
            }
        }

        private void OnClickPlayPlaylist(object sender, RoutedEventArgs e)
        {
            string targetDirectory = @"C:\MediaFiles";
            string jsonFilePath = Path.Combine(targetDirectory, "Playlists.json");

            if (File.Exists(jsonFilePath))
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                var selectedItem = sender is MenuFlyoutItem menuFlyoutItem ? (Playlist)menuFlyoutItem.DataContext : sender is Button button ? (Playlist)button.DataContext : null;

                if (selectedItem != null)
                {
                    var ListSongs = selectedItem.MediaItems;
                    Random random = new Random();
                    int randomIndex = random.Next(ListSongs.Count);
                    var randomSong = ListSongs[randomIndex];
                    string filePath = randomSong.FilePath;

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
        }

        private void CheckBox_Playlist_Checked(object sender, RoutedEventArgs e)
        {
            int checkedCount_Playlist = GetCheckedCheckBoxCount();
            CheckedCount_Playlist.Text = checkedCount_Playlist.ToString();
            AddPlaylist_btn.Visibility = Visibility.Collapsed;
            CheckBoxBar_Playlist.Visibility = Visibility.Visible;
        }
        private void CheckBox_Playlist_Unchecked(object sender, RoutedEventArgs e)
        {
            int checkedCount_Playlist = GetCheckedCheckBoxCount();
            CheckedCount_Playlist.Text = checkedCount_Playlist.ToString();
            if (checkedCount_Playlist == 0)
            {
                AddPlaylist_btn.Visibility = Visibility.Visible;
                CheckBoxBar_Playlist.Visibility = Visibility.Collapsed;
            }
        }

        private int GetCheckedCheckBoxCount()
        {
            int checkedCount_Playlist = 0;

            foreach (var item in MyGridView_Playlist.Items)
            {
                var gridViewItem = item as Playlist;
                if (gridViewItem != null && gridViewItem.IsChecked)
                {
                    checkedCount_Playlist++;
                }
            }
            return checkedCount_Playlist;
        }
        private void Playlist_elements_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel; 
            if (stackPanel != null)
            {
                var checkBox = stackPanel.FindName("CheckBox_PlayList") as CheckBox;
                var buttonLeft = stackPanel.FindName("Playlist_Play_btn") as Button;
                var buttonRight = stackPanel.FindName("Playlist_Info_btn") as Button;

                if (checkBox != null) checkBox.Visibility = Visibility.Visible;
                if (buttonLeft != null) buttonLeft.Visibility = Visibility.Visible;
                if (buttonRight != null) buttonRight.Visibility = Visibility.Visible;
            }
        }

        private void Playlist_elements_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel;
            if (stackPanel != null)
            {
                var checkBox = stackPanel.FindName("CheckBox_PlayList") as CheckBox;
                var buttonLeft = stackPanel.FindName("Playlist_Play_btn") as Button;
                var buttonRight = stackPanel.FindName("Playlist_Info_btn") as Button;
                if (checkBox.IsChecked == false)
                {
                    if (checkBox != null) checkBox.Visibility = Visibility.Collapsed;
                    if (buttonLeft != null) buttonLeft.Visibility = Visibility.Collapsed;
                    if (buttonRight != null) buttonRight.Visibility = Visibility.Collapsed;
                }
            }
        }
        private void ShowPlaylistElement(object sender, RoutedEventArgs e)
        {
            Playlist_Main.Visibility = Visibility.Collapsed;
            PlaylistElement.Visibility = Visibility.Visible;
            string targetDirectory = @"C:\MediaFiles";
            string jsonFilePath = Path.Combine(targetDirectory, "Playlists.json");

            if (File.Exists(jsonFilePath))
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                var fileList = JsonConvert.DeserializeObject<List<Playlist>>(jsonContent);
                var button = sender as Button;

                if (button != null)
                {
                    var playlistinfo = button.DataContext as Playlist;
                    var playlist_Info = fileList.FirstOrDefault(p => p.PlaylistName == playlistinfo.PlaylistName);
                    PlaylistNameText.Text = playlist_Info.PlaylistName;
                    TotalItemsText.Text = playlist_Info.TotalItems.ToString() + (playlist_Info.TotalItems == 1 ? " item" : " items");
                    MyListBox_Playlist.ItemsSource = playlist_Info.MediaItems;
                    LoadPlaylists();
                    PlaylistElement.Visibility = Visibility.Visible;
                    Playlist_Main.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void OnClickPlaySongPlaylist(object sender, RoutedEventArgs e)
        {
            var selectedItem = (GridViewItem)((Button)sender).DataContext;

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
                var gridViewItem = item as GridViewItem;
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
        private void OnClickPlayAll(object sender, RoutedEventArgs e)
        {
            var PlaylistName = PlaylistNameText.Text.Trim();
            string targetDirectory = @"C:\MediaFiles";
            string jsonFilePath = Path.Combine(targetDirectory, "Playlists.json");

            if (File.Exists(jsonFilePath))
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                var fileList = JsonConvert.DeserializeObject<List<Playlist>>(jsonContent);
                var selectedPlaylist = fileList.FirstOrDefault(p => p.PlaylistName == PlaylistName);
                if (selectedPlaylist != null) {
                    foreach (var playlist in selectedPlaylist.MediaItems)
                    {
                        var filePath = playlist.FilePath;
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
        }

        private async void AddFileToPlayList(object sender, RoutedEventArgs e)
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
                    Directory.CreateDirectory(targetDirectory);

                    var tagFile = TagLib.File.Create(file.Path);
                    var fileInfo = new GridViewItem
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

                    var mediaPlayerElement = MainWindow.PlayerElement;
                    mediaPlayerElement.Source = MediaSource.CreateFromUri(new Uri(file.Path));
                    string extension = Path.GetExtension(file.Path).ToLower();
                    if (extension != ".mp3")
                    {
                        mediaPlayerElement.Height = double.NaN;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening file: {ex.Message}");
            }
        }
        private void OnClickRenamePlaylist(object sender, RoutedEventArgs e)
        {
            var selectedItem = (Playlist)((MenuFlyoutItem)sender).DataContext;
            RenamePlaylistUI.Visibility = Visibility.Visible;
            NewPlaylistText.Text = selectedItem.PlaylistName;
            PreviousPlaylistName.Text = selectedItem.PlaylistName;
            Playlist_Main.IsHitTestVisible = false;
            Playlist_Main.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0xee, 0xee, 0xee));
        }
        private void HideRenamePlaylistUI(object sender, RoutedEventArgs e)
        {
            RenamePlaylistUI.Visibility = Visibility.Collapsed;
            Playlist_Main.IsHitTestVisible = true;
            Playlist_Main.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
       
        }
        private void OnClickSubmitRename(object sender, RoutedEventArgs e)
        {
            RenamePlaylistUI.Visibility = Visibility.Collapsed;
            string targetDirectory = @"C:\MediaFiles";
            string jsonFilePath = Path.Combine(targetDirectory, "Playlists.json");
            string previousPlaylistName = !string.IsNullOrEmpty(PreviousPlaylistName.Text.Trim()) ? PreviousPlaylistName.Text.Trim() : PlaylistNameText.Text.Trim();
            string newPlaylistName = NewPlaylistText.Text.Trim();
            string jsonContent = File.ReadAllText(jsonFilePath);
            var fileList = JsonConvert.DeserializeObject<List<Playlist>>(jsonContent);

            var playlistToRename = fileList.FirstOrDefault(p => p.PlaylistName == previousPlaylistName);
            if (playlistToRename != null)
            {
                playlistToRename.PlaylistName = newPlaylistName;
                string updatedJson = JsonConvert.SerializeObject(fileList, Formatting.Indented);
                File.WriteAllText(jsonFilePath, updatedJson);
            }
            Playlist_Main.IsHitTestVisible = true;
            Playlist_Main.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
            LoadPlaylists();
        }
        private void OnClickDeletePlaylist(object sender, RoutedEventArgs e)
        {
            var selectedItem = (Playlist)((MenuFlyoutItem)sender).DataContext;
            string targetDirectory = @"C:\MediaFiles";
            string jsonFilePath = Path.Combine(targetDirectory, "Playlists.json");
            string jsonContent = File.ReadAllText(jsonFilePath);
            var fileList = JsonConvert.DeserializeObject<List<Playlist>>(jsonContent);
            var playlistToRemove = fileList.FirstOrDefault(p => p.PlaylistName == selectedItem.PlaylistName);
            fileList.Remove(playlistToRemove);
            string updatedJson = JsonConvert.SerializeObject(fileList, Formatting.Indented);
            File.WriteAllText(jsonFilePath, updatedJson);
            LoadPlaylists();
            if (MyGridView_Playlist.Items.Count == 0)
            {
                Playlist_Empty.Visibility = Visibility.Visible;
                Playlist_Main.Visibility = Visibility.Collapsed;
            }
            else
            {
                Playlist_Empty.Visibility = Visibility.Collapsed;
                Playlist_Main.Visibility = Visibility.Visible;
            }
        }

        private void OnClickSubmitRename1(object sender, RoutedEventArgs e)
        {
            string targetDirectory = @"C:\MediaFiles";
            string jsonFilePath = Path.Combine(targetDirectory, "Playlists.json");
            string newPlaylistName = NewPlaylistText1.Text.Trim();
            string jsonContent = File.ReadAllText(jsonFilePath);
            var fileList = JsonConvert.DeserializeObject<List<Playlist>>(jsonContent);

            var playlistToRename = fileList.FirstOrDefault(p => p.PlaylistName == PlaylistNameText.Text);
            if (playlistToRename != null)
            {
                playlistToRename.PlaylistName = newPlaylistName;
                string updatedJson = JsonConvert.SerializeObject(fileList, Formatting.Indented);
                File.WriteAllText(jsonFilePath, updatedJson);
                PlaylistNameText.Text = newPlaylistName;
            }
            LoadPlaylists(); 
        }
    }
}
