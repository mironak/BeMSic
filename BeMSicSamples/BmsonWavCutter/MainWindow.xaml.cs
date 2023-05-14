using BeMSic.Bmson;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;

namespace BmsonWavCutter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BmsonParser? _bmson;
        string _bmsonFileName = "";

        private class WavList
        {
            public int ID { get; set; }
            public string? Name { get; set; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // タイトル
            var assembly = Assembly.GetExecutingAssembly().GetName();
            Window.Title = assembly.Name + " " + assembly.Version.ToString();

            OutputButton.IsEnabled = false;
            OutputAllButton.IsEnabled = false;
            WavComboBox.IsEnabled = false;
        }

        /// <summary>
        /// bmsonファイル読み込み
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenBmsonButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "bmson file|*.bmson|All file|*.*"
            };

            try
            {
                if (dialog.ShowDialog() != true)
                {
                    return;
                }

                OpenBmson(dialog.FileName);
            }
            catch
            {
                MessageBox.Show("Please read the bmson file.");
            }
        }

        /// <summary>
        /// 選択したwavを切断する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutputButton_Click(object sender, RoutedEventArgs e)
        {
            // 出力するwavを取得
            var chIndex = WavComboBox.SelectedIndex;
            WriteWavs(chIndex);
            MessageBox.Show("Completed");
        }

        /// <summary>
        /// すべてのwavを切断する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutputAllButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < _bmson!.Bmson!.sound_channels.Length; i++)
            {
                WriteWavs(i);
            }

            MessageBox.Show("Completed");
        }

        /// <summary>
        /// bmson内のwav一覧を作成
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<WavList> GetWavObservableCollection()
        {
            var list = new ObservableCollection<WavList>();
            for (int i = 0; i < _bmson?.Bmson!.sound_channels.Length; i++)
            {
                list.Add(new WavList { ID = i, Name = _bmson.Bmson.sound_channels[i].name });
            }
            return list;
        }

        /// <summary>
        /// wavを切断する
        /// </summary>
        /// <param name="chIndex"></param>
        private void WriteWavs(int chIndex)
        {
            string readWavFilePath = GetReadWavFilePath(_bmson!.Bmson!.sound_channels[chIndex].name);
            if (!File.Exists(readWavFilePath))
            {
                MessageBox.Show(".wav file not found");
                return;
            }

            // wavを切断し、保存する
            string saveDirectory = GetOutputBmsDirectoryName(_bmson.Bmson.sound_channels[chIndex].name);
            Directory.CreateDirectory(saveDirectory);

            bool bgmOnlyIsChecked = false;
            if(BgmLaneOnlyCheckBox.IsChecked != null)
            {
                bgmOnlyIsChecked = (bool)BgmLaneOnlyCheckBox.IsChecked;
            }

            var bmsText = _bmson.CutWav(saveDirectory, readWavFilePath, chIndex, bgmOnlyIsChecked, int.Parse(FeedinTextBox.Text), int.Parse(FeedoutTextBox.Text));
            File.WriteAllText(saveDirectory + "\\" + "out.bms", bmsText);
        }

        /// <summary>
        /// 切断前wavファイル名を取得
        /// </summary>
        /// <param name="wavFileName"></param>
        /// <returns></returns>
        private string GetReadWavFilePath(string wavFileName)
        {
            return Path.ChangeExtension(Path.GetDirectoryName(_bmsonFileName) + "\\" + wavFileName, ".wav");
        }

        /// <summary>
        /// 切断後wavの保存ディレクトリ名を取得( (bmsonファイルのディレクトリ)\(wavファイル名) )
        /// </summary>
        /// <param name="wavFileName"></param>
        /// <returns></returns>
        private string GetOutputBmsDirectoryName(string wavFileName)
        {
            return Path.GetDirectoryName(_bmsonFileName) + "\\" + Path.GetFileNameWithoutExtension(wavFileName);
        }

        /// <summary>
        /// Windowへのドロップ時処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                MessageBox.Show("Please read the bmson file.");
                return;
            }

            var fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
            OpenBmson(fileNames[0]);
        }

        /// <summary>
        /// WindowへのDragOver処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        /// <summary>
        /// bmsonファイルを開く
        /// </summary>
        /// <param name="fileName"></param>
        private void OpenBmson(string fileName)
        {
            _bmsonFileName = fileName;
            FileNameLabel.Content = Path.GetFileName(_bmsonFileName);

            var bmsonText = File.ReadAllText(_bmsonFileName);
            _bmson = new BmsonParser(bmsonText);

            WavComboBox.ItemsSource = GetWavObservableCollection();
            WavComboBox.SelectedIndex = 0;

            OutputButton.IsEnabled = true;
            OutputAllButton.IsEnabled = true;
            WavComboBox.IsEnabled = true;
        }
    }
}
