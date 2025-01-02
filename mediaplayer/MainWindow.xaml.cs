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


namespace MediaPlayerWinUI
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }
        
        private void OnHomeClick(object sender, RoutedEventArgs e)    
        {
            MusicLibrary.Visibility = Visibility.Collapsed;
            VideoLibrary.Visibility = Visibility.Collapsed;
            PlayQueue.Visibility = Visibility.Collapsed;
            PlayList.Visibility = Visibility.Collapsed;
            PlaylistElement.Visibility = Visibility.Collapsed;
            Home.Visibility = Visibility.Visible;
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
        }
        private void OnMusicLibraryClick(object sender, RoutedEventArgs e)
        {
            Home.Visibility = Visibility.Collapsed;
            VideoLibrary.Visibility = Visibility.Collapsed;
            PlayQueue.Visibility = Visibility.Collapsed;
            PlayList.Visibility = Visibility.Collapsed;
            PlaylistElement.Visibility = Visibility.Collapsed;
            MusicLibrary.Visibility = Visibility.Visible;
        }
        private void OnVideoLibraryClick(object sender, RoutedEventArgs e)
        {
            Home.Visibility = Visibility.Collapsed;
            MusicLibrary.Visibility = Visibility.Collapsed;
            PlayQueue.Visibility = Visibility.Collapsed;
            PlayList.Visibility = Visibility.Collapsed;
            PlaylistElement.Visibility = Visibility.Collapsed;
            VideoLibrary.Visibility = Visibility.Visible;
        }
        private void OnPlayQueueClick(object sender, RoutedEventArgs e)
        {
            Home.Visibility = Visibility.Collapsed;
            MusicLibrary.Visibility = Visibility.Collapsed;
            VideoLibrary.Visibility = Visibility.Collapsed;
            PlayList.Visibility = Visibility.Collapsed;
            PlaylistElement.Visibility = Visibility.Collapsed;
            PlayQueue.Visibility = Visibility.Visible;
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
            else {
                Playlist_elements.Visibility = Visibility.Collapsed;
            }
        }
        private void OnPlaylistElementsClick(object sender, RoutedEventArgs e)
        {
            Home.Visibility = Visibility.Collapsed;
            MusicLibrary.Visibility = Visibility.Collapsed;
            VideoLibrary.Visibility = Visibility.Collapsed;
            PlayQueue.Visibility = Visibility.Collapsed;
            PlayList.Visibility = Visibility.Collapsed;
            PlaylistElement.Visibility = Visibility.Visible;
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

                // Open the file picker for user to select a file
                StorageFile file = await picker.PickSingleFileAsync();

                if (file != null)
                {
                    // Update the media player source
                    mediaPlayerElement.Source = MediaSource.CreateFromStorageFile(file);
                    System.Diagnostics.Debug.WriteLine($"File selected: {file.Path}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening file: {ex.Message}");
            }
        }


        // Hàm phát
        private void OnPlayClick(object sender, RoutedEventArgs e)
        {
            if (mediaPlayerElement.MediaPlayer != null)
            {
                mediaPlayerElement.MediaPlayer.Play();
            }
        }

        // Hàm tạm dừng
        private void OnPauseClick(object sender, RoutedEventArgs e)
        {
            if (mediaPlayerElement.MediaPlayer != null)
            {
                mediaPlayerElement.MediaPlayer.Pause();
            }
        }

        // Hàm dừng
        private void OnStopClick(object sender, RoutedEventArgs e)
        {
            if (mediaPlayerElement.MediaPlayer != null)
            {
                mediaPlayerElement.MediaPlayer.Pause();
                mediaPlayerElement.MediaPlayer.Position = TimeSpan.Zero; // Đặt về đầu video
            }
        }
    }
}