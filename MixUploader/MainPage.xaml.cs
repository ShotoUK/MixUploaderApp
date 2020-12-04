using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Win32;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI.Popups;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MixUploader
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void TextBlock_SelectionChanged_1(object sender, RoutedEventArgs e)
        {

        }

        private async void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openpicker = new FileOpenPicker();
            openpicker.ViewMode = PickerViewMode.Thumbnail;
            openpicker.FileTypeFilter.Add(".mp3");
            openpicker.FileTypeFilter.Add(".wav");

            StorageFile file = await openpicker.PickSingleFileAsync();
            if(file != null)
            {
                //adds file path selected to the browse textbox then turns that into byte array.
                //once that is done it assigns the file to UploadSite static member
                try
                {

                    FileLocationTextBox.Text = file.Path;


                    var filestream = await file.OpenStreamForReadAsync();
                    byte[] fileBytes = new byte[filestream.Length];

                    filestream.Read(fileBytes, 0, fileBytes.Length);
                    
                    //var Uploadfile = new UploadSite();
                    //await Uploadfile.UploadToMixcloudAsync(fileBytes);



                    

                    UploadSite.audiofile = fileBytes;
                    UploadSite.filename = file.Name;

                    filestream.Close();


                    //byte[] readbytes = filestream;

                    //await filestream.ReadAllBytesAsync(FileLocationTextBox.Text);

                    //UploadSite.audiofile = filestream;
                }
                catch (Exception)
                {

                    throw;
                }

            }



        }

        private async void SendFileButton_Click(object sender, RoutedEventArgs e)
        {
            UploadProgressBar.Visibility = Visibility.Visible;

            UploadSite post = new UploadSite();

            //Set Properties in UploadSite Class
            UploadSite.UploadTitle = UploadTitleTextbox.Text;

            string DescriptionBoxText = "";
            DescriptionTextBox.Document.GetText(Windows.UI.Text.TextGetOptions.None, out DescriptionBoxText);
            UploadSite.UploadDescription = DescriptionBoxText;

            //Attempt to send file via POST request.
            if (MixcloudCheckbox.IsChecked == true)
            {
                if (UploadUnlistedCheckbox.IsChecked == true)
                {
                    UploadSite.UploadUnlisted = "True";
                }
                else
                {
                    UploadSite.UploadUnlisted = "False";
                }

                await post.UploadToMixcloudAsync(UploadSite.audiofile);

                string responsebody = post.responsebody;

                UploadProgressBar.Visibility = Visibility.Collapsed;

                var msg = new MessageDialog(responsebody);

                await msg.ShowAsync();


                // Clear Text & Checkboxes
                FileLocationTextBox.Text = "";
                UploadTitleTextbox.Text = "";
                DescriptionTextBox.Document.SetText(Windows.UI.Text.TextSetOptions.None, "");
                UploadUnlistedCheckbox.IsChecked = false;

            }
            else
            {
                UploadProgressBar.Visibility = Visibility.Collapsed;

                //Returns message if no upload site checked.
                var msg = new MessageDialog("No site checked. Please check Mixcloud");
            }

        }

    }
}
