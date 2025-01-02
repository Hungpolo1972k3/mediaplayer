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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaPlayerWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlayQueue : Page
    {
        public PlayQueue()
        {
            this.InitializeComponent();
            List<PlayItem> playItems = new List<PlayItem>
        {
            new PlayItem { PlayName = "Song 135435dfhfghfghgfhfgh dfghjofstnriotnsdg fsdpoitfgjspotehnroiswejrpsbgniodfsthyserknlesfisdlok", PlayArtist = "Artist 1", PlayAlbum = "Album 1", PlayTime = "3:45" },
            new PlayItem { PlayName = "Song 2", PlayArtist = "Artist 2", PlayAlbum = "Album 2", PlayTime = "4:00" },
            new PlayItem { PlayName = "Song 3", PlayArtist = "Artist 3", PlayAlbum = "Album 3", PlayTime = "5:30" },
            new PlayItem { PlayName = "Song 3", PlayArtist = "Artist 3", PlayAlbum = "Album 3", PlayTime = "5:30" },
            new PlayItem { PlayName = "Song 3", PlayArtist = "Artist 3", PlayAlbum = "Album 3", PlayTime = "5:30" },
            new PlayItem { PlayName = "Song 3", PlayArtist = "Artist 3", PlayAlbum = "Album 3", PlayTime = "5:30" },
            new PlayItem { PlayName = "Song 3", PlayArtist = "Artist 3", PlayAlbum = "Album 3", PlayTime = "5:30" },
            new PlayItem { PlayName = "Song 3", PlayArtist = "Artist 3", PlayAlbum = "Album 3", PlayTime = "5:30" },
            new PlayItem { PlayName = "Song 3", PlayArtist = "Artist 3", PlayAlbum = "Album 3", PlayTime = "5:30" },
            new PlayItem { PlayName = "Song 3", PlayArtist = "Artist 3", PlayAlbum = "Album 3", PlayTime = "5:30" },
            new PlayItem { PlayName = "Song 3", PlayArtist = "Artist 3", PlayAlbum = "Album 3", PlayTime = "5:30" },
            new PlayItem { PlayName = "Song 3", PlayArtist = "Artist 3", PlayAlbum = "Album 3", PlayTime = "5:30" },
            new PlayItem { PlayName = "Song 3", PlayArtist = "Artist 3", PlayAlbum = "Album 3", PlayTime = "5:30" },
            new PlayItem { PlayName = "Song 3", PlayArtist = "Artist 3", PlayAlbum = "Album 3", PlayTime = "5:30" },
            new PlayItem { PlayName = "Song 3", PlayArtist = "Artist 3", PlayAlbum = "Album 3", PlayTime = "5:30" },
            new PlayItem { PlayName = "Song 3", PlayArtist = "Artist 3", PlayAlbum = "Album 3", PlayTime = "5:30" }
        };

            // Gán nguồn dữ liệu cho ListBox
            MyListBox.ItemsSource = playItems;
        }
        public class PlayItem
        {
            public string PlayName { get; set; }
            public string PlayArtist { get; set; }
            public string PlayAlbum { get; set; }
            public string PlayTime { get; set; }

            public bool IsChecked { get; set; }
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

            var items = MyListBox.ItemsSource as IEnumerable<PlayItem>;
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
