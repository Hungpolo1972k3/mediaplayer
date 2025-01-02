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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaPlayerWinUI
{
 
    public sealed partial class PlayList : Page
    {
        public PlayList()
        {
            this.InitializeComponent();
            var items= new List<GridViewItem>
            {
                new GridViewItem { Title_Playlist_name = "Nguyễn Thọ Hùng", Title_Playlist_quantity = "10 items"},
                new GridViewItem { Title_Playlist_name = "Nguyễn Thọ Hùng", Title_Playlist_quantity = "1 item" },
                new GridViewItem { Title_Playlist_name = "Nguyễn Thọ Hùng", Title_Playlist_quantity = "2 items" },
                new GridViewItem { Title_Playlist_name = "Nguyễn Thọ Hùng", Title_Playlist_quantity = "3 items" },
                new GridViewItem { Title_Playlist_name = "Nguyễn Thọ Hùng", Title_Playlist_quantity = "4 items"},
                new GridViewItem { Title_Playlist_name = "Nguyễn Thọ Hùng", Title_Playlist_quantity = "5 items" },
                new GridViewItem { Title_Playlist_name = "Nguyễn Thọ Hùng", Title_Playlist_quantity = "6 items"},
                new GridViewItem { Title_Playlist_name = "Nguyễn Thọ Hùng", Title_Playlist_quantity = "7 items"},
                new GridViewItem { Title_Playlist_name = "Nguyễn Thọ Hùng", Title_Playlist_quantity = "28 items"},
                new GridViewItem { Title_Playlist_name = "Nguyễn Thọ Hùng", Title_Playlist_quantity = "9 items"},
                new GridViewItem { Title_Playlist_name = "Nguyễn Thọ Hùng", Title_Playlist_quantity = "10 items" },
                new GridViewItem { Title_Playlist_name = "Nguyễn Thọ Hùng", Title_Playlist_quantity = "11 items"},
                new GridViewItem { Title_Playlist_name = "Nguyễn Thọ Hùng", Title_Playlist_quantity = "12 items"},
                new GridViewItem { Title_Playlist_name = "Nguyễn Thọ Hùng", Title_Playlist_quantity = "13 items" },
                new GridViewItem { Title_Playlist_name = "Nguyễn Thọ Hùng", Title_Playlist_quantity = "2 items" },
                new GridViewItem { Title_Playlist_name = "Nguyễn Thọ Hùng", Title_Playlist_quantity = "2 items" },
                new GridViewItem { Title_Playlist_name = "Nguyễn Thọ Hùng", Title_Playlist_quantity = "2 items" },
                new GridViewItem { Title_Playlist_name = "Nguyễn Thọ Hùng", Title_Playlist_quantity = "2 items" },
            };

            // Gán danh sách cho GridView
            MyGridView_Playlist.ItemsSource = items;

        }
        public class GridViewItem
        {
            public string Title_Playlist_name { get; set; }
            public string Title_Playlist_quantity { get; set; } 
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
                var gridViewItem = item as GridViewItem;
                if (gridViewItem != null && gridViewItem.IsChecked)
                {
                    checkedCount_Playlist++;
                }
            }
            return checkedCount_Playlist;
        }
        private void Playlist_elements_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel; // Lấy StackPanel hiện tại
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
        private void Show_Playlist_Element(object sender, DoubleTappedRoutedEventArgs e)
        {
            Playlist_Main.Visibility = Visibility.Collapsed;
            //PlaylistElement.Visibility = Visibility.Visible;
        }
    }
}
