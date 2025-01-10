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
using static MediaPlayerWinUI.PlayQueue;
using Windows.Storage.Pickers;
using Windows.Storage;
using Newtonsoft.Json;
using System.Threading.Tasks;
using static MediaPlayerWinUI.Home;
using Windows.Media.Core;
using SharpCompress.Common;

namespace MediaPlayerWinUI
{
    
    public sealed partial class MusicLibrary : Page
    {
        public static MusicLibrary InstanceMusicLibrary { get; set; }
        public MusicLibrary()
        {
            this.InitializeComponent();
            InstanceMusicLibrary = this;
            LoadAudioToGridView();
        }

        public class GridViewItemMusic
        {
            public string FileId { get; set; }
            public string FileName { get; set; }
            public string FilePath { get; set; }
            public string FileType { get; set; }
            public string Duration { get; set; }
            public string Title { get; set; }
            public string Artist { get; set; }
            public string Genre { get; set; }
            public string Album { get; set; }
            public string Year { get; set; }
            public string DateCreated { get; set; }
            public string DateModified { get; set; }

            public bool IsChecked { get; set; }
        }
        public class GridViewAlbum
        {
            public string Album { get; set; }
            public int Songs { get; set; }
            public string Duration { get; set; }

        }
        public class GridViewArtist
        {
            public string Artist { get; set; }
            public string Albums { get; set; }
            public string Genre { get; set; }
            public int Songs { get; set; }
            public string Duration { get; set; }

        }

        private async void AddFolderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(MainWindow.Instance);

                var picker = new FolderPicker();
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

                picker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
                StorageFolder folder = await picker.PickSingleFolderAsync();

                if (folder != null)
                {
                    string targetDirectory = @"C:\MediaFiles";
                    Directory.CreateDirectory(targetDirectory);

                    string musicsJsonFile = Path.Combine(targetDirectory, "Musics.json");

                    List<object> audioFileList = new List<object>();

                    var files = await GetFilesFromFolderAsync(folder);

                    foreach (var file in files)
                    {
                        if (file.FileType == ".mp3")
                        {
                            var tagFile = TagLib.File.Create(file.Path);
                            var audioFileInfo = new GridViewItemMusic
                            {
                                FileId = Guid.NewGuid().ToString(),
                                FileName = file.Name,
                                FilePath = file.Path,
                                FileType = file.FileType,
                                Duration = tagFile.Properties.Duration.ToString(@"hh\:mm\:ss"),
                                Title = tagFile.Tag.Title ?? "Unknown Title",
                                Artist = tagFile.Tag.FirstPerformer ?? "Unknown Artist",
                                Genre = tagFile.Tag.FirstGenre ?? "Unknown Genre",
                                Album = tagFile.Tag.Album ?? "Unknown Album",
                                Year = tagFile.Tag.Year > 0 ? tagFile.Tag.Year.ToString() : "Unknown Year",
                                DateCreated = File.GetCreationTime(file.Path).ToString("yyyy-MM-dd HH:mm:ss"),
                                DateModified = File.GetLastWriteTime(file.Path).ToString("yyyy-MM-dd HH:mm:ss")
                            };
                            audioFileList.Add(audioFileInfo);
                        }
                    }

                    string existingJson = string.Empty;
                    List<object> existingAudioFileList = new List<object>();
                    try
                    {
                        existingJson = File.ReadAllText(musicsJsonFile);
                        if (!string.IsNullOrEmpty(existingJson))
                        {
                            existingAudioFileList = JsonConvert.DeserializeObject<List<object>>(existingJson);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error reading Musics.json: {ex.Message}");
                    }
                    existingAudioFileList.AddRange(audioFileList);
                    string json = JsonConvert.SerializeObject(existingAudioFileList, Formatting.Indented);
                    File.WriteAllText(musicsJsonFile, json);
                    LoadAudioToGridView();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening folder: {ex.Message}");
            }
        }

        private async Task<List<StorageFile>> GetFilesFromFolderAsync(StorageFolder folder)
        {
            var files = new List<StorageFile>();

            var folderItems = await folder.GetItemsAsync();

            foreach (var item in folderItems)
            {
                if (item is StorageFile file)
                {
                    files.Add(file);
                }
                else if (item is StorageFolder subfolder)
                {
                    var subfolderFiles = await GetFilesFromFolderAsync(subfolder);
                    files.AddRange(subfolderFiles);
                }
            }

            return files;
        }

        public void LoadAudioToGridView()
        {
            try
            {
                string targetDirectory = @"C:\MediaFiles";
                string jsonFilePath = Path.Combine(targetDirectory, "Musics.json");

                if (File.Exists(jsonFilePath))
                {
                    string jsonContent = File.ReadAllText(jsonFilePath);

                    var fileList = JsonConvert.DeserializeObject<List<GridViewItemMusic>>(jsonContent);

                    if (fileList != null)
                    {
                        MyListSongs.ItemsSource = fileList;
                        if (MyListSongs.Items.Count == 0)
                        {
                            MusicLibrary_empty.Visibility = Visibility.Visible;
                            MusicLibrary_Header.Visibility = Visibility.Collapsed;
                            Shuffle_Play_btn.Visibility = Visibility.Collapsed;
                            Song_Show.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            MusicLibrary_empty.Visibility = Visibility.Collapsed;
                            MusicLibrary_Header.Visibility = Visibility.Visible;
                            Shuffle_Play_btn.Visibility = Visibility.Visible;
                            Song_Show.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading media files: {ex.Message}");
            }
        }

        private void LoadAlbumsToGridView()
        {
            try
            {
                string targetDirectory = @"C:\MediaFiles";
                string jsonFilePath = Path.Combine(targetDirectory, "Musics.json");
                if (File.Exists(jsonFilePath))
                {
                    string jsonContent = File.ReadAllText(jsonFilePath);
                    var fileList = JsonConvert.DeserializeObject<List<GridViewItemMusic>>(jsonContent);
                    if (fileList != null)
                    {
                        var distinctAlbums = fileList
                            .GroupBy(file => file.Album)
                            .Select(group =>
                            {
                                int songCount = group.Count();
                                TimeSpan totalDuration = TimeSpan.Zero;
                                foreach (var file in group)
                                {
                                    if (TimeSpan.TryParse(file.Duration, out var duration))
                                    {
                                        totalDuration += duration;
                                    }
                                }
                                return new GridViewAlbum
                                {
                                    Album = group.Key,
                                    Songs = songCount,
                                    Duration = totalDuration.ToString(@"hh\:mm\:ss")
                                };
                            })
                            .ToList();
                        MyGridView_Album.ItemsSource = distinctAlbums;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading media files: {ex.Message}");
            }
        }

        private void LoadArtistsToGridView()
        {
            try
            {
                string targetDirectory = @"C:\MediaFiles";
                string jsonFilePath = Path.Combine(targetDirectory, "Musics.json");

                if (File.Exists(jsonFilePath))
                {
                    string jsonContent = File.ReadAllText(jsonFilePath);

                    var fileList = JsonConvert.DeserializeObject<List<GridViewItemMusic>>(jsonContent);

                    if (fileList != null)
                    {

                        var distinctAlbums = fileList
                            .GroupBy(file => file.Artist)
                            .Select(group =>
                            {
                                int albumCount = group.Select(file => file.Album).Distinct().Count();
                                int songCount = group.Count();
                                var genres = group.Select(file => file.Genre).Distinct();

                                TimeSpan totalDuration = TimeSpan.Zero;
                                foreach (var file in group)
                                {
                                    if (TimeSpan.TryParse(file.Duration, out var duration))
                                    {
                                        totalDuration += duration;
                                    }
                                }
                                return new GridViewArtist
                                {
                                    Artist = group.Key,
                                    Albums = albumCount.ToString(),
                                    Genre = string.Join(" ", genres),
                                    Songs = songCount,
                                    Duration = totalDuration.ToString(@"hh\:mm\:ss")
                                };
                            })
                            .ToList();

                        MyGridView_Artist.ItemsSource = distinctAlbums;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading media files: {ex.Message}");
            }
        }



        private void OnClickPlaySong(object sender, RoutedEventArgs e)
        {
            var selectedItem = (GridViewItemMusic)((Button)sender).DataContext;

            if (selectedItem != null)
            {
                string filePath = selectedItem.FilePath;

                if (File.Exists(filePath))
                {
                    var mediaPlayerElement = MainWindow.PlayerElement;
                    mediaPlayerElement.Source = MediaSource.CreateFromUri(new Uri(filePath));
                }
            }
        }

        private void OnClickPlayAlbum(object sender, RoutedEventArgs e)
        {
            string targetDirectory = @"C:\MediaFiles";
            string jsonFilePath = Path.Combine(targetDirectory, "Musics.json");

            if (File.Exists(jsonFilePath))
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                var fileList = JsonConvert.DeserializeObject<List<GridViewItemMusic>>(jsonContent);
                var selectedItem = sender is MenuFlyoutItem menuFlyoutItem ? (GridViewAlbum)menuFlyoutItem.DataContext : sender is Button button ? (GridViewAlbum)button.DataContext : null;

                if (selectedItem != null)
                {
                    var ListSongs = fileList.Where(file => file.Album == selectedItem.Album).ToList();
                    Random random = new Random();
                    int randomIndex = random.Next(ListSongs.Count); // Chỉ số ngẫu nhiên
                    var randomSong = ListSongs[randomIndex];
                    string filePath = randomSong.FilePath;

                    if (File.Exists(filePath))
                    {
                        var mediaPlayerElement = MainWindow.PlayerElement;
                        mediaPlayerElement.Source = MediaSource.CreateFromUri(new Uri(filePath));
                        string extension = Path.GetExtension(filePath).ToLower();
                    }
                }
            }
        }

        private void OnClickPlayArtist(object sender, RoutedEventArgs e)
        {
            string targetDirectory = @"C:\MediaFiles";
            string jsonFilePath = Path.Combine(targetDirectory, "Musics.json");

            if (File.Exists(jsonFilePath))
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                var fileList = JsonConvert.DeserializeObject<List<GridViewItemMusic>>(jsonContent);
                var selectedItem = sender is MenuFlyoutItem menuFlyoutItem ? (GridViewArtist)menuFlyoutItem.DataContext : sender is Button button ? (GridViewArtist)button.DataContext : null;

                if (selectedItem != null)
                {
                    var ListSongs = fileList.Where(file => file.Artist == selectedItem.Artist).ToList();
                    Random random = new Random();
                    int randomIndex = random.Next(ListSongs.Count);
                    var randomSong = ListSongs[randomIndex];
                    string filePath = randomSong.FilePath;

                    if (File.Exists(filePath))
                    {
                        var mediaPlayerElement = MainWindow.PlayerElement;
                        mediaPlayerElement.Source = MediaSource.CreateFromUri(new Uri(filePath));
                    }
                }
            }
        }

        private void ShuffleAndPlay(object sender, RoutedEventArgs e)
        {
            var songList = MyListSongs.ItemsSource as List<GridViewItemMusic>;

            if (songList != null && songList.Count > 0)
            {
                Random random = new Random();
                int randomIndex = random.Next(songList.Count);
                GridViewItemMusic selectedSong = songList[randomIndex];

                if (File.Exists(selectedSong.FilePath))
                {
                    var mediaPlayerElement = MainWindow.PlayerElement;
                    mediaPlayerElement.Source = MediaSource.CreateFromUri(new Uri(selectedSong.FilePath));
                }
            }
        }

        public void SongChoose(object sender, TappedRoutedEventArgs e)
        {
            Album.Visibility = Visibility.Collapsed;
            Artist.Visibility = Visibility.Collapsed;
            SortElements.Visibility = Visibility.Visible;
            CheckBoxBar_Album.Visibility = Visibility.Collapsed;
            CheckBoxBar_Artist.Visibility = Visibility.Collapsed;
            AlbumElement.Visibility = Visibility.Collapsed;
            ArtistElement.Visibility = Visibility.Collapsed;
            Shuffle_Play_btn.Visibility = Visibility.Visible;
            SortElements.Visibility = Visibility.Visible;
            Song_Show.Visibility = Visibility.Visible;
        }
        private void AlbumChoose(object sender, TappedRoutedEventArgs e)
        {
            Song_Show.Visibility = Visibility.Collapsed;
            Artist.Visibility = Visibility.Collapsed;
            CheckBoxBar_Song.Visibility = Visibility.Collapsed;
            CheckBoxBar_Artist.Visibility = Visibility.Collapsed;
            AlbumElement.Visibility = Visibility.Collapsed;
            ArtistElement.Visibility = Visibility.Collapsed;
            SortElements.Visibility = Visibility.Visible;
            Shuffle_Play_btn.Visibility = Visibility.Visible;
            Album.Visibility = Visibility.Visible;
            LoadAlbumsToGridView();
        }
        private void ArtistChoose(object sender, TappedRoutedEventArgs e)
        {
            Song_Show.Visibility = Visibility.Collapsed;
            Album.Visibility = Visibility.Collapsed;
            SortElements.Visibility = Visibility.Collapsed;
            CheckBoxBar_Album.Visibility = Visibility.Collapsed;
            CheckBoxBar_Song.Visibility = Visibility.Collapsed;
            AlbumElement.Visibility = Visibility.Collapsed;
            ArtistElement.Visibility = Visibility.Collapsed;
            Shuffle_Play_btn.Visibility = Visibility.Visible;
            Artist.Visibility = Visibility.Visible;
            LoadArtistsToGridView();
        }
        private void SaveMediaFilesToJson()
        {
            string targetDirectory = @"C:\MediaFiles";
            string jsonFilePath = Path.Combine(targetDirectory, "Musics.json");

            var fileList = MyListSongs.ItemsSource as List<GridViewItemMusic>;
            if (fileList != null)
            {
                string json = JsonConvert.SerializeObject(fileList, Formatting.Indented);
                File.WriteAllText(jsonFilePath, json);
            }
        }

        private void CheckBox_Song_Checked(object sender, RoutedEventArgs e)
        {
            int checkedCount_Song = GetCheckedCheckBoxCount_Song();
            CheckedCount_Song.Text = checkedCount_Song.ToString();
            Shuffle_Play_btn.Visibility = Visibility.Collapsed;
            CheckBoxBar_Song.Visibility = Visibility.Visible;
            var item = (GridViewItemMusic)((CheckBox)sender).DataContext;
            item.IsChecked = true;
            SaveMediaFilesToJson();
        }
        private void CheckBox_Song_Unchecked(object sender, RoutedEventArgs e)
        {
            int checkedCount_Song = GetCheckedCheckBoxCount_Song();
            CheckedCount_Song.Text = checkedCount_Song.ToString();
            if (checkedCount_Song == 0)
            {
                Shuffle_Play_btn.Visibility = Visibility.Visible;
                CheckBoxBar_Song.Visibility = Visibility.Collapsed;
            }
            var item = (GridViewItemMusic)((CheckBox)sender).DataContext;
            item.IsChecked = false;
            SaveMediaFilesToJson();
        }
        private int GetCheckedCheckBoxCount_Song()
        {
            int checkedCount_Song = 0;

            foreach (var item in MyListSongs.Items)
            {
                var songItem = item as GridViewItemMusic;
                if (songItem != null && songItem.IsChecked)
                {
                    checkedCount_Song++;
                }
            }
            return checkedCount_Song;
        }

        private void CheckBox_Album_Checked(object sender, RoutedEventArgs e)
        {
            int checkedCount_Album = GetCheckedCheckBoxCount_Album();
            CheckedCount_Album.Text = checkedCount_Album.ToString();
            Shuffle_Play_btn.Visibility = Visibility.Collapsed;
            CheckBoxBar_Album.Visibility = Visibility.Visible;
        }
        private void CheckBox_Album_Unchecked(object sender, RoutedEventArgs e)
        {
            int checkedCount_Album = GetCheckedCheckBoxCount_Album();
            CheckedCount_Album.Text = checkedCount_Album.ToString();
            if (checkedCount_Album == 0)
            {
                Shuffle_Play_btn.Visibility = Visibility.Visible;
                CheckBoxBar_Album.Visibility = Visibility.Collapsed;
            }
        }

        private int GetCheckedCheckBoxCount_Album()
        {
            int checkedCount_Album = 0;

            foreach (var item in MyGridView_Album.Items)
            {
                var gridViewItem = item as GridViewItemMusic;
                if (gridViewItem != null && gridViewItem.IsChecked)
                {
                    checkedCount_Album++;
                }
            }
            return checkedCount_Album;
        }

        private void CheckBox_Artist_Checked(object sender, RoutedEventArgs e)
        {
            int checkedCount_Artist = GetCheckedCheckBoxCount_Artist();
            CheckedCount_Artist.Text = checkedCount_Artist.ToString();
            Shuffle_Play_btn.Visibility = Visibility.Collapsed;
            CheckBoxBar_Artist.Visibility = Visibility.Visible;
        }
        private void CheckBox_Artist_Unchecked(object sender, RoutedEventArgs e)
        {
            int checkedCount_Artist = GetCheckedCheckBoxCount_Artist();
            CheckedCount_Artist.Text = checkedCount_Artist.ToString();
            if (checkedCount_Artist == 0)
            {
                Shuffle_Play_btn.Visibility = Visibility.Visible;
                SortElements.Visibility = Visibility.Collapsed;
                CheckBoxBar_Artist.Visibility = Visibility.Collapsed;
            }
        }
        private int GetCheckedCheckBoxCount_Artist()
        {
            int checkedCount_Artist = 0;

            foreach (var item in MyGridView_Artist.Items)
            {
                var gridViewItem = item as  GridViewItemMusic;
                if (gridViewItem != null && gridViewItem.IsChecked)
                {
                    checkedCount_Artist++;
                }
            }
            return checkedCount_Artist;
        }
        private void Album_elements_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel; // Lấy StackPanel hiện tại
            if (stackPanel != null)
            {
                var checkBox = stackPanel.FindName("CheckBox_Album") as CheckBox;
                var buttonLeft = stackPanel.FindName("Album_Play_btn") as Button;
                var buttonRight = stackPanel.FindName("Album_Info_btn") as Button;

                if (checkBox != null) checkBox.Visibility = Visibility.Visible;
                if (buttonLeft != null) buttonLeft.Visibility = Visibility.Visible;
                if (buttonRight != null) buttonRight.Visibility = Visibility.Visible;
            }
        }

        private void Album_elements_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel; 
            if (stackPanel != null)
            {
                var checkBox = stackPanel.FindName("CheckBox_Album") as CheckBox;
                var buttonLeft = stackPanel.FindName("Album_Play_btn") as Button;
                var buttonRight = stackPanel.FindName("Album_Info_btn") as Button;
                if (checkBox.IsChecked == false)
                {
                    if (checkBox != null) checkBox.Visibility = Visibility.Collapsed;
                    if (buttonLeft != null) buttonLeft.Visibility = Visibility.Collapsed;
                    if (buttonRight != null) buttonRight.Visibility = Visibility.Collapsed;
                }
            }
        }
        private void Artist_elements_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel; // Lấy StackPanel hiện tại
            if (stackPanel != null)
            {
                var checkBox = stackPanel.FindName("CheckBox_Artist") as CheckBox;
                var buttonLeft = stackPanel.FindName("Artist_Play_btn") as Button;
                var buttonRight = stackPanel.FindName("Artist_Info_btn") as Button;

                if (checkBox != null) checkBox.Visibility = Visibility.Visible;
                if (buttonLeft != null) buttonLeft.Visibility = Visibility.Visible;
                if (buttonRight != null) buttonRight.Visibility = Visibility.Visible;
            }
        }

        private void Artist_elements_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel;
            if (stackPanel != null)
            {
                var checkBox = stackPanel.FindName("CheckBox_Artist") as CheckBox;
                var buttonLeft = stackPanel.FindName("Artist_Play_btn") as Button;
                var buttonRight = stackPanel.FindName("Artist_Info_btn") as Button;
                if (checkBox.IsChecked == false)
                {
                    if (checkBox != null) checkBox.Visibility = Visibility.Collapsed;
                    if (buttonLeft != null) buttonLeft.Visibility = Visibility.Collapsed;
                    if (buttonRight != null) buttonRight.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ShowAlbumElement(object sender, RoutedEventArgs e)
        {
            Shuffle_Play_btn.Visibility = Visibility.Collapsed;
            SortElements.Visibility = Visibility.Collapsed;
            Album.Visibility = Visibility.Collapsed;
            ArtistElement.Visibility = Visibility.Collapsed;
            ArtistElement.Visibility = Visibility.Collapsed;
            AlbumElement.Visibility = Visibility.Visible;
            string targetDirectory = @"C:\MediaFiles";
            string jsonFilePath = Path.Combine(targetDirectory, "Musics.json");

            if (File.Exists(jsonFilePath))
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                var fileList = JsonConvert.DeserializeObject<List<GridViewItemMusic>>(jsonContent);
                var button = sender as Button;

                if (button != null)
                {
                    var album = button.DataContext as GridViewAlbum;
                    if (album != null)
                    {
                        AlbumElement_Album.Text = album.Album.ToString() ;
                        AlbumElement_Songs.Text = album.Songs.ToString() + (album.Songs == 1 ? " song * " : " songs * ") + album.Duration.ToString() + " run time";
                        var distinctAlbums = fileList
                            .Where(file => file.Album == album.Album)
                            .ToList();

                      ListAlbum_Songs.ItemsSource = distinctAlbums;
                    }
                }
            }
        }



        private void CheckBox_Album_Song_Checked(object sender, RoutedEventArgs e)
        {
            int checkedCount_Album_Song = GetCheckedCheckBoxCount_Album_Song();
            CheckedCount_Album_Song.Text = checkedCount_Album_Song.ToString();
            CheckBoxBar_Album_Song.Visibility = Visibility.Visible;
        }
        private void CheckBox_Album_Song_Unchecked(object sender, RoutedEventArgs e)
        {
            int checkedCount_Album_Song = GetCheckedCheckBoxCount_Album_Song();
            CheckedCount_Song.Text = checkedCount_Album_Song.ToString();
            if (checkedCount_Album_Song == 0)
            {
                CheckBoxBar_Album_Song.Visibility = Visibility.Collapsed;
            }
        }
        private int GetCheckedCheckBoxCount_Album_Song()
        {
            int checkedCount_Album_Song = 0;

            foreach (var item in ListAlbum_Songs.Items)
            {
                var gridViewItem = item as GridViewItemMusic;
                if (gridViewItem != null && gridViewItem.IsChecked)
                {
                    checkedCount_Album_Song++;
                }
            }
            return checkedCount_Album_Song;
        }

        private void ShowArtistElement(object sender, RoutedEventArgs e)
        {
            Shuffle_Play_btn.Visibility = Visibility.Collapsed;
            SortElements.Visibility = Visibility.Collapsed;
            Artist.Visibility = Visibility.Collapsed;
            ArtistElement.Visibility = Visibility.Visible;
            string targetDirectory = @"C:\MediaFiles";
            string jsonFilePath = Path.Combine(targetDirectory, "Musics.json");

            if (File.Exists(jsonFilePath))
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                var fileList = JsonConvert.DeserializeObject<List<GridViewItemMusic>>(jsonContent);
                var button = sender as Button;

                if (button != null)
                {
                    var artist = button.DataContext as GridViewArtist;
                    if (artist != null)
                    {
                        ArtistElement_Info.Text = artist.Albums.ToString() + (artist.Albums == "1" ? " album " : " albums ") + artist.Songs.ToString() + (artist.Songs == 1 ? " song * " : " songs * ") + artist.Duration.ToString();
                        ArtistElement_Genre.Text = "Artist * " + artist.Genre.ToString();
                        var distinctAlbums = fileList
                            .Where(file => file.Artist == artist.Artist)
                            .GroupBy(file => file.Album)
                            .Select(group =>
                            {
                                int songCount = group.Count();

                                TimeSpan totalDuration = TimeSpan.Zero;
                                foreach (var file in group)
                                {
                                    if (TimeSpan.TryParse(file.Duration, out var duration))
                                    {
                                        totalDuration += duration;
                                    }
                                }
                                return new GridViewAlbum
                                {
                                    Album = group.Key,
                                    Songs = songCount,
                                    Duration = totalDuration.ToString(@"hh\:mm\:ss")
                                };
                            })
                            .ToList();

                        ListArtist_Albums.ItemsSource = distinctAlbums;
                    }
                }
            }
        }

        private void CheckBox_Artist_Song_Checked(object sender, RoutedEventArgs e)
        {
            int checkedCount_Artist_Song = GetCheckedCheckBoxCount_Artist_Song();
            CheckedCount_Artist_Song.Text = checkedCount_Artist_Song.ToString();
            InYourLibrary.Visibility = Visibility.Collapsed;
            CheckBoxBar_Artist_Song.Visibility = Visibility.Visible;
        }
        private void CheckBox_Artist_Song_Unchecked(object sender, RoutedEventArgs e)
        {
            int checkedCount_Artist_Song = GetCheckedCheckBoxCount_Artist_Song();
            CheckedCount_Artist_Song.Text = checkedCount_Artist_Song.ToString();
            if (checkedCount_Artist_Song == 0)
            {
                InYourLibrary.Visibility = Visibility.Visible;
                CheckBoxBar_Artist_Song.Visibility = Visibility.Collapsed;
            }
        }

        private int GetCheckedCheckBoxCount_Artist_Song()
        {
            int checkedCount_Artist_Song = 0;

            foreach (var item in ListArtist_Albums.Items)
            {
                var gridViewItem = item as GridViewItemMusic;
                if (gridViewItem != null && gridViewItem.IsChecked)
                {
                    checkedCount_Artist_Song++;
                }
            }
            return checkedCount_Artist_Song;
        }

        private void Artist_Song_elements_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel;
            if (stackPanel != null)
            {
                var checkBox = stackPanel.FindName("CheckBox_Artist_Song") as CheckBox;
                var buttonLeft = stackPanel.FindName("Artist_Song_Play_btn") as Button;
                var buttonRight = stackPanel.FindName("Artist_Song_Info_btn") as Button;

                if (checkBox != null) checkBox.Visibility = Visibility.Visible;
                if (buttonLeft != null) buttonLeft.Visibility = Visibility.Visible;
                if (buttonRight != null) buttonRight.Visibility = Visibility.Visible;
            }
        }

        private void Artist_Song_elements_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel;
            if (stackPanel != null)
            {
                var checkBox = stackPanel.FindName("CheckBox_Artist_Song") as CheckBox;
                var buttonLeft = stackPanel.FindName("Artist_Song_Play_btn") as Button;
                var buttonRight = stackPanel.FindName("Artist_Song_Info_btn") as Button;
                if (checkBox.IsChecked == false)
                {
                    if (checkBox != null) checkBox.Visibility = Visibility.Collapsed;
                    if (buttonLeft != null) buttonLeft.Visibility = Visibility.Collapsed;
                    if (buttonRight != null) buttonRight.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
