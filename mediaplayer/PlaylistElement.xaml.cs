using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using static MediaPlayerWinUI.PlayList;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaPlayerWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlaylistElement : Page
    {
        public PlaylistElement()
        {
            this.InitializeComponent();
            List<Playlist_Item> list = new List<Playlist_Item>
            {
                new Playlist_Item {PlayName = "íuerfuweroejkwẻtwerterterterterter"},
            };

            MyListBox_Playlist.ItemsSource = list;
        }

        public class Playlist_Item
        {
            public string PlayName { get; set; }
            public bool IsChecked { get; set; }
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
                var gridViewItem = item as Playlist_Item;
                if (gridViewItem != null && gridViewItem.IsChecked)
                {
                    checkedCount_Playlist_Element++;
                }
            }
            return checkedCount_Playlist_Element;
        }
        private void StackPanel_Playlist_Element_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel; // Lấy StackPanel hiện tại
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
            var stackPanel = sender as StackPanel; // Lấy StackPanel hiện tại
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
    }
}
