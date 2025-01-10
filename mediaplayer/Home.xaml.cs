using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Diagnostics;
using Microsoft.UI.Xaml.Media;
using Windows.Media.Playback;
using Windows.Foundation;
using mediaplayer;
using System.IO;
using Windows.Media.Core;
using Newtonsoft.Json;
using SharpCompress.Common;
using System.Linq;


namespace MediaPlayerWinUI
{
    public sealed partial class Home : Page
    {
        public static Home InstanceHome { get; private set; }
        public Home()
        {
            this.InitializeComponent();
            InstanceHome = this;
            LoadMediaFilesToGridView();

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

            public string FontIcon => (FileType == ".mp3" ? "\uE8D6" : "\uE8B2");
        }

        private async void OpenFileButton_Click(object sender, RoutedEventArgs e)
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

                    string jsonFilePath = Path.Combine(targetDirectory, "MediaFiles.json");
                    List<object> fileList = new List<object>();

                    if (File.Exists(jsonFilePath))
                    {
                        string existingJson = File.ReadAllText(jsonFilePath);
                        fileList = JsonConvert.DeserializeObject<List<object>>(existingJson) ?? new List<object>();
                    }

                    fileList.Add(fileInfo);

                    string json = JsonConvert.SerializeObject(fileList, Formatting.Indented);
                    File.WriteAllText(jsonFilePath, json);

                    LoadMediaFilesToGridView();
                    if (MyGridView_Home.Items.Count == 0)
                    {
                        Home_empty.Visibility = Visibility.Visible;
                        MyGridView_Home.Visibility = Visibility.Collapsed;
                        RecentMediaText.Visibility = Visibility.Collapsed;
                        AddFiles_Btn.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        Home_empty.Visibility = Visibility.Collapsed;
                        MyGridView_Home.Visibility = Visibility.Visible;
                        RecentMediaText.Visibility = Visibility.Visible;
                        AddFiles_Btn.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening file: {ex.Message}");
            }
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
                    string musicsJsonFile = Path.Combine(targetDirectory, "MediaFiles.json");

                    List<object> audioFileList = new List<object>();

                    var files = await GetFilesFromFolderAsync(folder);

                    // Kiểm tra các file trong thư mục và thêm vào danh sách
                    foreach (var file in files)
                    {
                        if (file.FileType == ".mp3" || file.FileType == ".mp4" || file.FileType == ".avi" || file.FileType == ".mkv" || file.FileType == ".mov" || file.FileType == ".wmv")
                        {
                            var tagFile = TagLib.File.Create(file.Path);
                            var audioFileInfo = new
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

                    if (File.Exists(musicsJsonFile))
                    {
                        string existingJson = File.ReadAllText(musicsJsonFile);
                        List<object> existingFileList = JsonConvert.DeserializeObject<List<object>>(existingJson) ?? new List<object>();

                        existingFileList.AddRange(audioFileList);

                        string json = JsonConvert.SerializeObject(existingFileList, Formatting.Indented);
                        File.WriteAllText(musicsJsonFile, json);
                    }
                    else
                    {
                        string json = JsonConvert.SerializeObject(audioFileList, Formatting.Indented);
                        File.WriteAllText(musicsJsonFile, json);
                    }

                    LoadMediaFilesToGridView();
                    if (MyGridView_Home.Items.Count == 0)
                    {
                        Home_empty.Visibility = Visibility.Visible;
                        MyGridView_Home.Visibility = Visibility.Collapsed;
                        RecentMediaText.Visibility = Visibility.Collapsed;
                        AddFiles_Btn.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        Home_empty.Visibility = Visibility.Collapsed;
                        MyGridView_Home.Visibility = Visibility.Visible;
                        RecentMediaText.Visibility = Visibility.Visible;
                        AddFiles_Btn.Visibility = Visibility.Visible;
                    }
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
        public void LoadMediaFilesToGridView()
        {
            try
            {
                string targetDirectory = @"C:\MediaFiles";
                string jsonFilePath1 = Path.Combine(targetDirectory, "MediaFiles.json");
                string jsonFilePath2 = Path.Combine(targetDirectory, "Musics.json");
                string jsonFilePath3 = Path.Combine(targetDirectory, "Videos.json");
                if (!File.Exists(jsonFilePath1))
                {
                    File.WriteAllText(jsonFilePath1, JsonConvert.SerializeObject(new List<GridViewItemModel>(), Formatting.Indented));
                }
                if (!File.Exists(jsonFilePath2))
                {
                    File.WriteAllText(jsonFilePath2, JsonConvert.SerializeObject(new List<GridViewItemModel>(), Formatting.Indented));
                }
                if (!File.Exists(jsonFilePath3))
                {
                    File.WriteAllText(jsonFilePath3, JsonConvert.SerializeObject(new List<GridViewItemModel>(), Formatting.Indented));
                }

                string jsonContent1 = File.ReadAllText(jsonFilePath1);
                string jsonContent2 = File.ReadAllText(jsonFilePath2);
                string jsonContent3 = File.ReadAllText(jsonFilePath3);
                var fileList1 = JsonConvert.DeserializeObject<List<GridViewItemModel>>(jsonContent1);
                var fileList2 = JsonConvert.DeserializeObject<List<GridViewItemModel>>(jsonContent2);
                var fileList3 = JsonConvert.DeserializeObject<List<GridViewItemModel>>(jsonContent3);

                var combinedFileList = new List<GridViewItemModel>();
                combinedFileList.AddRange(fileList1);
                combinedFileList.AddRange(fileList2);
                combinedFileList.AddRange(fileList3);

                MyGridView_Home.ItemsSource = combinedFileList;
                if (MyGridView_Home.Items.Count == 0)
                {
                    Home_empty.Visibility = Visibility.Visible;
                    MyGridView_Home.Visibility = Visibility.Collapsed;
                    RecentMediaText.Visibility = Visibility.Collapsed;
                    AddFiles_Btn.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Home_empty.Visibility = Visibility.Collapsed;
                    MyGridView_Home.Visibility = Visibility.Visible;
                    RecentMediaText.Visibility = Visibility.Visible;
                    AddFiles_Btn.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading media files: {ex.Message}");
            }
        }

        private void OnClickPlayMediaFiles(object sender, RoutedEventArgs e)
        {
            var selectedItem = sender is Button button ? (GridViewItemModel)button.DataContext : sender is MenuFlyoutItem menuFlyoutItem ? (GridViewItemModel)menuFlyoutItem.DataContext : null;

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


        private void OnClickRemoveMediaFile(object sender, RoutedEventArgs e)
        {
            var selectedItem = (GridViewItemModel)((MenuFlyoutItem)sender).DataContext;

            if (selectedItem != null)
            {
                string fileId = selectedItem.FileId;
                string filePath = selectedItem.FilePath;
                string targetDirectory = @"C:\MediaFiles";
                string jsonFilePath1 = Path.Combine(targetDirectory, "MediaFiles.json");
                string jsonFilePath2 = Path.Combine(targetDirectory, "Musics.json");
                string jsonFilePath3 = Path.Combine(targetDirectory, "Videos.json");
                RemoveFileFromJson(jsonFilePath1, fileId);
                RemoveFileFromJson(jsonFilePath2, fileId);
                RemoveFileFromJson(jsonFilePath3, fileId);
                LoadMediaFilesToGridView();

                if (MyGridView_Home.Items.Count == 0)
                {
                    Home_empty.Visibility = Visibility.Visible;
                    MyGridView_Home.Visibility = Visibility.Collapsed;
                    RecentMediaText.Visibility = Visibility.Collapsed;
                    AddFiles_Btn.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Home_empty.Visibility = Visibility.Collapsed;
                    MyGridView_Home.Visibility = Visibility.Visible;
                    RecentMediaText.Visibility = Visibility.Visible;
                    AddFiles_Btn.Visibility = Visibility.Visible;
                }
            }
        }

        private void RemoveFileFromJson(string jsonFilePath, string fileId)
        {
            if (File.Exists(jsonFilePath))
            {
                try
                {
                    string jsonContent = File.ReadAllText(jsonFilePath);
                    var fileList = JsonConvert.DeserializeObject<List<GridViewItemModel>>(jsonContent);
                    var itemToRemove = fileList.FirstOrDefault(file => file.FileId == fileId);
                    if (itemToRemove != null)
                    {
                        fileList.Remove(itemToRemove);
                    }
                    string updatedJsonContent = JsonConvert.SerializeObject(fileList, Formatting.Indented);
                    File.WriteAllText(jsonFilePath, updatedJsonContent);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error removing file from {jsonFilePath}: {ex.Message}");
                }
            }
        }


        private void Home_elements_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel; 
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

        private void SaveMediaFilesToJson()
        {
            string targetDirectory = @"C:\MediaFiles";
            string jsonFilePath = Path.Combine(targetDirectory, "MediaFiles.json");

            var fileList = MyGridView_Home.ItemsSource as List<GridViewItemModel>;
            if (fileList != null)
            {
                string json = JsonConvert.SerializeObject(fileList, Formatting.Indented);
                File.WriteAllText(jsonFilePath, json);
            }
        }
        private void CheckBox_Home_Checked(object sender, RoutedEventArgs e)
        {
            int checkedCount = GetCheckedCheckBoxCount();
            CheckedCount.Text = checkedCount.ToString();
            RecentMediaText.Visibility = Visibility.Collapsed;
            CheckBoxBar.Visibility = Visibility.Visible;
            var item = (GridViewItemModel)((CheckBox)sender).DataContext;
            item.IsChecked = true;
            SaveMediaFilesToJson();
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
            var item = (GridViewItemModel)((CheckBox)sender).DataContext;
            item.IsChecked = false;
            SaveMediaFilesToJson();
        }

        private int GetCheckedCheckBoxCount()
        {
            int checkedCount = 0;

            foreach (var item in MyGridView_Home.Items)
            {
                var gridViewItem = item as GridViewItemModel;
                if (gridViewItem != null && gridViewItem.IsChecked)
                {
                    checkedCount++;
                }
            }
            return checkedCount;
        }

    }
}
