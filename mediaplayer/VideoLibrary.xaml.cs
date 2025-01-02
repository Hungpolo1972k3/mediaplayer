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

namespace MediaPlayerWinUI
{
    public sealed partial class VideoLibrary : Page
    {
        public VideoLibrary()
        {
            this.InitializeComponent();

            var allVideos = new List<Video>
                {
                    new Video { Title_Video_name = "Video 1", Video_Runtime = "02:30", IsChecked = false, FolderName = "Folder 1" },
                    new Video { Title_Video_name = "Video 2", Video_Runtime = "03:10", IsChecked = false, FolderName = "Folder 1" },
                    new Video { Title_Video_name = "Video 3", Video_Runtime = "01:45", IsChecked = false, FolderName = "Folder 2" },
                    new Video { Title_Video_name = "Video 4", Video_Runtime = "05:00", IsChecked = false, FolderName = "Folder 2" }
                };
            MyGridView_AllVideos.ItemsSource = allVideos;
            var folders = new List<VideoFolder>
                {
                    new VideoFolder
                    {
                        Title_VideoFolder_Name = "Folder 1",
                        Title_VideoFolder_Url = "http://example.com/folder1",
                        Videos = allVideos.Where(v => v.FolderName == "Folder 1").ToList()
                    },
                    new VideoFolder
                {
                    Title_VideoFolder_Name = "Folder 2",
                    Title_VideoFolder_Url = "http://example.com/folder2",
                    Videos = allVideos.Where(v => v.FolderName == "Folder 2").ToList()
                }
            };

            MyGridView_VideoFolders.ItemsSource = folders;
        }

        public class Video
        {
            public string Title_Video_name { get; set; }
            public string Video_Runtime { get; set; }
            public bool IsChecked { get; set; }
            public string FolderName { get; set; }
        }

        public class VideoFolder
        {
            public string Title_VideoFolder_Name { get; set; }
            public string Title_VideoFolder_Url { get; set; }
            public List<Video> Videos { get; set; } 
        }

        private void AllVideosChoose(object sender, TappedRoutedEventArgs e)
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
            var items = MyGridView_AllVideos.ItemsSource as IEnumerable<Video>;
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
            var items = MyGridView_VideoFolders.ItemsSource as IEnumerable<Video>;
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
