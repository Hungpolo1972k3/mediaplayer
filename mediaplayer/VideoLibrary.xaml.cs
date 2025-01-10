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
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static MediaPlayerWinUI.MusicLibrary;
using Windows.Media.Core;
using System.Security.Cryptography.X509Certificates;

namespace MediaPlayerWinUI
{
    public sealed partial class VideoLibrary : Page
    {
        public static VideoLibrary InstanceVideoLibrary { get; private set; }
        public VideoLibrary()
        {
            this.InitializeComponent();
            InstanceVideoLibrary = this;
            LoadVideoToGridView();
        }

        public class GridViewItemVideo
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

            public string Folder_Url { get; set; }
            public string Folder_Name { get; set; }
            public string DateCreated { get; set; }
            public string DateModified { get; set; }

            public bool IsChecked { get; set; }
        }
        public class GridViewVideoFolder
        {
            public string FolderUrl { get; set; }
            public string FolderName { get; set; }

            public List<GridViewItemVideo> Videos { get; set; }
           
        }



        private async void AddFolderButton_Click_Video(object sender, RoutedEventArgs e)
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
                    string videosJsonFile = Path.Combine(targetDirectory, "Videos.json");

                    List<object> newVideoFileList = new List<object>(); // Danh sách dữ liệu mới

                    var files = await GetFilesFromFolderAsync(folder);

                    foreach (var file in files)
                    {
                        if (file.FileType == ".mp4" || file.FileType == ".avi" || file.FileType == ".mov" || file.FileType == ".mkv")
                        {
                            // Lấy thông tin file video mà không sao chép file
                            var tagFile = TagLib.File.Create(file.Path);
                            var videoFileInfo = new GridViewItemVideo
                            {
                                FileId = Guid.NewGuid().ToString(),
                                FileName = file.Name,
                                FilePath = file.Path, // Sử dụng filepath gốc
                                FileType = file.FileType,
                                Duration = tagFile.Properties.Duration.ToString(@"hh\:mm\:ss"),
                                Title = tagFile.Tag.Title ?? "Unknown Title",
                                Artist = tagFile.Tag.FirstPerformer ?? "Unknown Artist",
                                Genre = tagFile.Tag.FirstGenre ?? "Unknown Genre",
                                Album = tagFile.Tag.Album ?? "Unknown Album",
                                Year = tagFile.Tag.Year > 0 ? tagFile.Tag.Year.ToString() : "Unknown Year",
                                Folder_Url = folder.Path,
                                Folder_Name = folder.Name,
                                DateCreated = File.GetCreationTime(file.Path).ToString("yyyy-MM-dd HH:mm:ss"),
                                DateModified = File.GetLastWriteTime(file.Path).ToString("yyyy-MM-dd HH:mm:ss")
                            };
                            newVideoFileList.Add(videoFileInfo);
                        }
                    }

                    // Đọc dữ liệu cũ từ Videos.json (nếu có)
                    List<object> existingVideoFileList = new List<object>();
                    if (File.Exists(videosJsonFile))
                    {
                        try
                        {
                            string existingJson = File.ReadAllText(videosJsonFile);
                            existingVideoFileList = JsonConvert.DeserializeObject<List<object>>(existingJson) ?? new List<object>();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error reading Videos.json: {ex.Message}");
                        }
                    }

                    // Hợp nhất dữ liệu cũ và mới
                    existingVideoFileList.AddRange(newVideoFileList);

                    // Ghi dữ liệu hợp nhất vào Videos.json
                    string json = JsonConvert.SerializeObject(existingVideoFileList, Formatting.Indented);
                    File.WriteAllText(videosJsonFile, json);

                    // Load dữ liệu lên giao diện
                    LoadVideoToGridView();
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

        public void LoadVideoToGridView()
        {
            try
            {
                string targetDirectory = @"C:\MediaFiles";
                string jsonFilePath = Path.Combine(targetDirectory, "Videos.json");

                if (File.Exists(jsonFilePath))
                {
                    string jsonContent = File.ReadAllText(jsonFilePath);

                    var fileList = JsonConvert.DeserializeObject<List<GridViewItemVideo>>(jsonContent);

                    if (fileList != null)
                    {
                        MyGridView_AllVideos.ItemsSource = fileList;
                        var folderVideo = fileList
                            .GroupBy(file => file.Folder_Url)
                            .Select(group =>
                            {
                                var folderUrl = group.Key;
                                return new GridViewVideoFolder
                                {
                                    FolderUrl = group.Key,
                                    Videos = group.Select(file => new GridViewItemVideo 
                                    {
                                        FileId = file.FileId,
                                        FileName = file.FileName,
                                        FilePath = file.FilePath,
                                        FileType = file.FileType,
                                        Duration = file.Duration,
                                        Title = file.Title,
                                        Artist = file.Artist,
                                        Genre = file.Genre,
                                        Album = file.Album,
                                        Year = file.Year,
                                        Folder_Url = file.Folder_Url,
                                        Folder_Name = file.Folder_Name,
                                        DateCreated = file.DateCreated.ToString(),
                                        DateModified = file.DateModified.ToString(),
                                        IsChecked = file.IsChecked
                                    }).ToList()
                                };
                            })
                            .ToList(); ;

                        MyGridView_VideoFolders.ItemsSource = folderVideo;
                        if (MyGridView_AllVideos.Items.Count == 0)
                        {
                            VideoLibrary_Empty.Visibility = Visibility.Visible;
                            VideoLibrary_Header.Visibility = Visibility.Collapsed;
                            VideoLibrary_PlayAll_btn.Visibility = Visibility.Collapsed;
                            VideoLibrary_AllVideos.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            VideoLibrary_Empty.Visibility = Visibility.Collapsed;
                            VideoLibrary_Header.Visibility = Visibility.Visible;
                            VideoLibrary_PlayAll_btn.Visibility = Visibility.Visible;
                            VideoLibrary_AllVideos.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading media files: {ex.Message}");
            }
        }

        private void OnClickPlayVideo(object sender, RoutedEventArgs e)
        {
            var selectedItem = sender is Button button ? (GridViewItemVideo)button.DataContext : sender is MenuFlyoutItem menuFlyoutItem ? (GridViewItemVideo)menuFlyoutItem.DataContext : null;

            if (selectedItem != null)
            {
                string filePath = selectedItem.FilePath;

                if (File.Exists(filePath))
                {
                    var mediaPlayerElement = MainWindow.PlayerElement;
                    mediaPlayerElement.Source = MediaSource.CreateFromUri(new Uri(filePath));
                    mediaPlayerElement.Height = double.NaN;
                    mediaPlayerElement.Width = double.NaN;
                }
            }
        }

        public void AllVideosChoose(object sender, TappedRoutedEventArgs e)
        {
            VideoLibrary_AllVideos.Visibility = Visibility.Visible;
            VideoLibrary_VideoFolders.Visibility = Visibility.Collapsed;
        }
        private void VideoFoldersChoose(object sender, TappedRoutedEventArgs e)
        {
            VideoLibrary_AllVideos.Visibility = Visibility.Collapsed;
            VideoLibrary_VideoFolders.Visibility = Visibility.Visible;
        }
        private void CheckBox_AllVideos_Checked(object sender, RoutedEventArgs e)
        {
            int checkedCount_AllVideos = GetCheckedCheckBoxCount_AllVideos();
            CheckedCount_AllVideos.Text = checkedCount_AllVideos.ToString();
            VideoLibrary_PlayAll_btn.Visibility = Visibility.Collapsed;
            CheckBoxBar_AllVideos.Visibility = Visibility.Visible;
        }
        private void CheckBox_AllVideos_Unchecked(object sender, RoutedEventArgs e)
        {
            int checkedCount_AllVideos = GetCheckedCheckBoxCount_AllVideos();
            CheckedCount_AllVideos.Text = checkedCount_AllVideos.ToString();
            if (checkedCount_AllVideos == 0)
            {
                VideoLibrary_PlayAll_btn.Visibility = Visibility.Visible;
                CheckBoxBar_AllVideos.Visibility = Visibility.Collapsed;
            }
        }
        private int GetCheckedCheckBoxCount_AllVideos()
        {
            int checkedCount_AllVideos = 0;

            // Duyệt qua tất cả các item trong ItemsSource của GridView
            var items = MyGridView_AllVideos.ItemsSource as IEnumerable<GridViewItemVideo>;
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (item.IsChecked)
                    {
                        checkedCount_AllVideos++;
                    }
                }
            }

            return checkedCount_AllVideos;
        }


        private void AllVideos_elements_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel; // Lấy StackPanel hiện tại
            if (stackPanel != null)
            {
                var checkBox = stackPanel.FindName("CheckBox_AllVideos_VideoLibrary") as CheckBox;
                var buttonLeft = stackPanel.FindName("AllVideos_Play_btn") as Button;
                var buttonRight = stackPanel.FindName("AllVideos_Info_btn") as Button;

                if (checkBox != null) checkBox.Visibility = Visibility.Visible;
                if (buttonLeft != null) buttonLeft.Visibility = Visibility.Visible;
                if (buttonRight != null) buttonRight.Visibility = Visibility.Visible;
            }
        }

        private void AllVideos_elements_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel;
            if (stackPanel != null)
            {
                var checkBox = stackPanel.FindName("CheckBox_AllVideos_VideoLibrary") as CheckBox;
                var buttonLeft = stackPanel.FindName("AllVideos_Play_btn") as Button;
                var buttonRight = stackPanel.FindName("AllVideos_Info_btn") as Button;
                if (checkBox.IsChecked == false)
                {
                    if (checkBox != null) checkBox.Visibility = Visibility.Collapsed;
                    if (buttonLeft != null) buttonLeft.Visibility = Visibility.Collapsed;
                    if (buttonRight != null) buttonRight.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void CheckBox_VideoFolders_Checked(object sender, RoutedEventArgs e)
        {
            int checkedCount_VideoFolders = GetCheckedCheckBoxCount_VideoFolders();
            CheckedCount_VideoFolders.Text = checkedCount_VideoFolders.ToString();
            VideoLibrary_PlayAll_btn.Visibility = Visibility.Collapsed;
            CheckBoxBar_VideoFolders.Visibility = Visibility.Visible;
        }
        private void CheckBox_VideoFolders_Unchecked(object sender, RoutedEventArgs e)
        {
            int checkedCount_VideoFolders = GetCheckedCheckBoxCount_VideoFolders();
            CheckedCount_AllVideos.Text = checkedCount_VideoFolders.ToString();
            if (checkedCount_VideoFolders == 0)
            {
                VideoLibrary_PlayAll_btn.Visibility = Visibility.Visible;
                CheckBoxBar_AllVideos.Visibility = Visibility.Collapsed;
            }
        }
        private int GetCheckedCheckBoxCount_VideoFolders()
        {
            int checkedCount_VideoFolders = 0;
            var items = MyGridView_VideoFolders.ItemsSource as IEnumerable<GridViewItemVideo>;
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (item.IsChecked)
                    {
                        checkedCount_VideoFolders++;
                    }
                }
            }

            return checkedCount_VideoFolders;
        }


        private void VideoFolders_elements_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel; // Lấy StackPanel hiện tại
            if (stackPanel != null)
            {
                var checkBox = stackPanel.FindName("CheckBox_VideoFolders_VideoLibrary") as CheckBox;
                var buttonLeft = stackPanel.FindName("VideoFolders_Play_btn") as Button;
                var buttonRight = stackPanel.FindName("VideoFolders_Info_btn") as Button;

                if (checkBox != null) checkBox.Visibility = Visibility.Visible;
                if (buttonLeft != null) buttonLeft.Visibility = Visibility.Visible;
                if (buttonRight != null) buttonRight.Visibility = Visibility.Visible;
            }
        }

        private void VideoFolders_elements_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel;
            if (stackPanel != null)
            {
                var checkBox = stackPanel.FindName("CheckBox_VideoFolders_VideoLibrary") as CheckBox;
                var buttonLeft = stackPanel.FindName("VideoFolders_Play_btn") as Button;
                var buttonRight = stackPanel.FindName("VideoFolders_Info_btn") as Button;
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
