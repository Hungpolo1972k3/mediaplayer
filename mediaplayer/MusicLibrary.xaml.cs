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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaPlayerWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MusicLibrary : Page
    {
        public MusicLibrary()
        {
            this.InitializeComponent();
            var items = new List<GridViewItem>
            {
                new GridViewItem { Title_Album_name = "Nguyễn Thọ Hùng", Title_Album_artist = "Unknown artist"},
                new GridViewItem { Title_Album_name = "Nguyễn Thọ Hùng", Title_Album_artist = "Unknown artist"},
                new GridViewItem { Title_Album_name = "Nguyễn Thọ Hùng", Title_Album_artist = "Unknown artist"},
                new GridViewItem { Title_Album_name = "Nguyễn Thọ Hùng", Title_Album_artist = "Unknown artist"},
                new GridViewItem { Title_Album_name = "Nguyễn Thọ Hùng", Title_Album_artist = "Unknown artist"},
                new GridViewItem { Title_Album_name = "Nguyễn Thọ Hùng", Title_Album_artist = "Unknown artist"},
                new GridViewItem { Title_Album_name = "Nguyễn Thọ Hùng", Title_Album_artist = "Unknown artist"},
                new GridViewItem { Title_Album_name = "Nguyễn Thọ Hùng", Title_Album_artist = "Unknown artist"},
                new GridViewItem { Title_Album_name = "Nguyễn Thọ Hùng", Title_Album_artist = "Unknown artist"},
                new GridViewItem { Title_Album_name = "Nguyễn Thọ Hùng", Title_Album_artist = "Unknown artist"},
                new GridViewItem { Title_Album_name = "Nguyễn Thọ Hùng", Title_Album_artist = "Unknown artist"},
                new GridViewItem { Title_Album_name = "Nguyễn Thọ Hùng", Title_Album_artist = "Unknown artist"},
                new GridViewItem { Title_Album_name = "Nguyễn Thọ Hùng", Title_Album_artist = "Unknown artist"},
                new GridViewItem { Title_Album_name = "Nguyễn Thọ Hùng", Title_Album_artist = "Unknown artist"},
                new GridViewItem { Title_Album_name = "Nguyễn Thọ Hùng", Title_Album_artist = "Unknown artist"},
                new GridViewItem { Title_Album_name = "Nguyễn Thọ Hùng", Title_Album_artist = "Unknown artist"},
            };

            // Gán danh sách cho GridView
            MyGridView_Playlist.ItemsSource = items;
            MyGridView_Artist.ItemsSource = items;
            ListArtist_Songs.ItemsSource = items;

            List<Song> songs = new List<Song>
        {
            new Song{PlayName="nguyeakjbsdfjsdfljkdsjklfdlsjkflkjdsflhkdslfk", PlayArtist="Unknown artist", PlayAlbum="Unknown Album", PlayGenre="Unknown Genre", PlayTime= "2:12"},
            new Song{PlayName="nguyeakjbsdfjsdfljkdsjklfdlsjkflkjdsflhkdslfk", PlayArtist="Unknown artist", PlayAlbum="Unknown Album", PlayGenre="Unknown Genre", PlayTime= "2:12"},
            new Song{PlayName="nguyeakjbsdfjsdfljkdsjklfdlsjkflkjdsflhkdslfk", PlayArtist="Unknown artist", PlayAlbum="Unknown Album", PlayGenre="Unknown Genre", PlayTime= "2:12"},
            new Song{PlayName="nguyeakjbsdfjsdfljkdsjklfdlsjkflkjdsflhkdslfk", PlayArtist="Unknown artist", PlayAlbum="Unknown Album", PlayGenre="Unknown Genre", PlayTime= "2:12"},
            new Song{PlayName="nguyeakjbsdfjsdfljkdsjklfdlsjkflkjdsflhkdslfk", PlayArtist="Unknown artist", PlayAlbum="Unknown Album", PlayGenre="Unknown Genre", PlayTime= "2:12"},
            new Song{PlayName="nguyeakjbsdfjsdfljkdsjklfdlsjkflkjdsflhkdslfk", PlayArtist="Unknown artist", PlayAlbum="Unknown Album", PlayGenre="Unknown Genre", PlayTime= "2:12"},
            new Song{PlayName="nguyeakjbsdfjsdfljkdsjklfdlsjkflkjdsflhkdslfk", PlayArtist="Unknown artist", PlayAlbum="Unknown Album", PlayGenre="Unknown Genre", PlayTime= "2:12"},
            new Song{PlayName="nguyeakjbsdfjsdfljkdsjklfdlsjkflkjdsflhkdslfk", PlayArtist="Unknown artist", PlayAlbum="Unknown Album", PlayGenre="Unknown Genre", PlayTime= "2:12"},
            new Song{PlayName="nguyeakjbsdfjsdfljkdsjklfdlsjkflkjdsflhkdslfk", PlayArtist="Unknown artist", PlayAlbum="Unknown Album", PlayGenre="Unknown Genre", PlayTime= "2:12"},
            new Song{PlayName="nguyeakjbsdfjsdfljkdsjklfdlsjkflkjdsflhkdslfk", PlayArtist="Unknown artist", PlayAlbum="Unknown Album", PlayGenre="Unknown Genre", PlayTime= "2:12"},
            new Song{PlayName="nguyeakjbsdfjsdfljkdsjklfdlsjkflkjdsflhkdslfk", PlayArtist="Unknown artist", PlayAlbum="Unknown Album", PlayGenre="Unknown Genre", PlayTime= "2:12"},
            new Song{PlayName="nguyeakjbsdfjsdfljkdsjklfdlsjkflkjdsflhkdslfk", PlayArtist="Unknown artist", PlayAlbum="Unknown Album", PlayGenre="Unknown Genre", PlayTime= "2:12"},
            new Song{PlayName="nguyeakjbsdfjsdfljkdsjklfdlsjkflkjdsflhkdslfk", PlayArtist="Unknown artist", PlayAlbum="Unknown Album", PlayGenre="Unknown Genre", PlayTime= "2:12"},
            new Song{PlayName="nguyeakjbsdfjsdfljkdsjklfdlsjkflkjdsflhkdslfk", PlayArtist="Unknown artist", PlayAlbum="Unknown Album", PlayGenre="Unknown Genre", PlayTime= "2:12"},
            new Song{PlayName="nguyeakjbsdfjsdfljkdsjklfdlsjkflkjdsflhkdslfk", PlayArtist="Unknown artist", PlayAlbum="Unknown Album", PlayGenre="Unknown Genre", PlayTime= "2:12"},
            new Song{PlayName="nguyeakjbsdfjsdfljkdsjklfdlsjkflkjdsflhkdslfk", PlayArtist="Unknown artist", PlayAlbum="Unknown Album", PlayGenre="Unknown Genre", PlayTime= "2:12"},
            new Song{PlayName="nguyeakjbsdfjsdfljkdsjklfdlsjkflkjdsflhkdslfk", PlayArtist="Unknown artist", PlayAlbum="Unknown Album", PlayGenre="Unknown Genre", PlayTime= "2:12"},
            new Song{PlayName="nguyeakjbsdfjsdfljkdsjklfdlsjkflkjdsflhkdslfk", PlayArtist="Unknown artist", PlayAlbum="Unknown Album", PlayGenre="Unknown Genre", PlayTime= "2:12"},
            new Song{PlayName="nguyeakjbsdfjsdfljkdsjklfdlsjkflkjdsflhkdslfk", PlayArtist="Unknown artist", PlayAlbum="Unknown Album", PlayGenre="Unknown Genre", PlayTime= "2:12"},
            new Song{PlayName="nguyeakjbsdfjsdfljkdsjklfdlsjkflkjdsflhkdslfk", PlayArtist="Unknown artist", PlayAlbum="Unknown Album", PlayGenre="Unknown Genre", PlayTime= "2:12"},
            new Song{PlayName="nguyeakjbsdfjsdfljkdsjklfdlsjkflkjdsflhkdslfk", PlayArtist="Unknown artist", PlayAlbum="Unknown Album", PlayGenre="Unknown Genre", PlayTime= "2:12"},
        };
            // Gán nguồn dữ liệu cho ListBox
            MyListSongs.ItemsSource = songs;
            ListAlbum_Songs.ItemsSource = songs;

        } 

        public class Song
        {
            public string PlayName { get; set; }
            public string PlayArtist { get; set; }
            public string PlayAlbum { get; set; }
            public string PlayGenre { get; set; }
            public string PlayTime { get; set; }
            public bool IsChecked { get; set; }
        }

        public class GridViewItem
        {
            public string Title_Album_name { get; set; }
            public string Title_Album_artist { get; set; }
            public bool IsChecked { get; set; }

            public static explicit operator GridViewItem(DependencyObject v)
            {
                throw new NotImplementedException();
            }

            internal CheckBox FindName(string v)
            {
                throw new NotImplementedException();
            }
        }
    
        private void SongChoose(object sender, TappedRoutedEventArgs e)
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
        }


        private void CheckBox_Song_Checked(object sender, RoutedEventArgs e)
        {
            int checkedCount_Song = GetCheckedCheckBoxCount_Song();
            CheckedCount_Song.Text = checkedCount_Song.ToString();
            Shuffle_Play_btn.Visibility = Visibility.Collapsed;
            CheckBoxBar_Song.Visibility = Visibility.Visible;
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
        }
        private int GetCheckedCheckBoxCount_Song()
        {
            int checkedCount_Song = 0;

            foreach (var item in MyListSongs.Items)
            {
                var songItem = item as Song;
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

            foreach (var item in MyGridView_Playlist.Items)
            {
                var gridViewItem = item as GridViewItem;
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
                var gridViewItem = item as GridViewItem;
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
                var gridViewItem = item as GridViewItem;
                if (gridViewItem != null && gridViewItem.IsChecked)
                {
                    checkedCount_Album_Song++;
                }
            }
            return checkedCount_Album_Song;
        }

        private void ShowArtistElement(object sender, TappedRoutedEventArgs e)
        {
            Shuffle_Play_btn.Visibility = Visibility.Collapsed;
            SortElements.Visibility = Visibility.Collapsed;
            Artist.Visibility = Visibility.Collapsed;
            ArtistElement.Visibility = Visibility.Visible;
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

            foreach (var item in ListArtist_Songs.Items)
            {
                var gridViewItem = item as GridViewItem;
                if (gridViewItem != null && gridViewItem.IsChecked)
                {
                    checkedCount_Artist_Song++;
                }
            }
            return checkedCount_Artist_Song;
        }

        private void Artist_Song_elements_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel; // Lấy StackPanel hiện tại
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
