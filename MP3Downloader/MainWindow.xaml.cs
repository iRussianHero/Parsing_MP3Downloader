using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MP3Downloader
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FolderBrowserDialog folder;
        List<Song> songs;
        public MainWindow()
        {
            InitializeComponent();
            folder = new FolderBrowserDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LB.Items.Clear();
            SiteToString siteToString = new SiteToString();
            string site = siteToString.GetSite("https://ru.hitmotop.com/search?q=" + TB.Text);
            SongsParser parser = new SongsParser();
            songs= parser.GetSongs(site);

            foreach(Song x in songs)
            {
                LB.Items.Add(x.Name);
               // LB.Items.Add(x.Url);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            folder.ShowDialog();
        }

        string GenerateFilename(string songName)
        {
            string result = "";
            for (int i=0; i<songName.Length;i++)
            {
                if (songName[i] == '?' || songName[i] == '\"' || songName[i] == '|'
                    || songName[i] == '\\' || songName[i] == ' ' || songName[i] == '*'
                    || songName[i] == '«'
                    || songName[i] == '»' || songName[i] == '>' || songName[i] == '<'
                    || songName[i] == ':' || songName[i] == '/' || songName[i] == '\n')
                    continue;
                result += songName[i];
            }
            if (result.Length == 0)
                result = "song";
            result += ".mp3";
            return result;
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (LB.SelectedIndex < 0)
                return;
            if (folder.SelectedPath.Length==0 || folder.SelectedPath==null)
            {
                if (folder.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;
            }
            WebClient client = new WebClient();
            string path = folder.SelectedPath+"\\"
                 + GenerateFilename(songs[LB.SelectedIndex].Name);
            
            client.DownloadFileAsync(new Uri(songs[LB.SelectedIndex].Url), path);
          //  System.Windows.Forms.MessageBox.Show("Скачано!");
        }

        async void DowloadAll()
        {
            WebClient client = new WebClient();
            if (folder.SelectedPath.Length == 0 || folder.SelectedPath == null)
            {
                if (folder.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;
            }
            PB.Value = 0;
            PB.Maximum = songs.Count;
            for (int i =0; i<songs.Count;i++)
            {
                string path = folder.SelectedPath + "\\"
                + GenerateFilename(songs[i].Name);
              LB1.Content = songs[i].Name;
              await Task.Run(() => client.DownloadFile(new Uri(songs[i].Url), path));
              await Task.Run(()=> Dispatcher.Invoke(new Action (()=> { PB.Value += 1; })));
            }
        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            DowloadAll();
        }
    }
}
