using BeMSic.Bmson;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace WavCutterBmsonToBmsDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BmsonMain? _bmson;
        string _bmsonFileName = "";

        class WavList
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
            OutputButton.IsEnabled = false;
            OutputAllButton.IsEnabled = false;
            WavComboBox.IsEnabled = false;
        }

        /// <summary>
        /// bmsonファイル読み込み
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileOpenButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "BMSONファイル (*.*)|*.*";

            try
            {
                if (dialog.ShowDialog() == true)
                {
                    _bmsonFileName = dialog.FileName;

                    var bmsonText = File.ReadAllText(_bmsonFileName);
                    _bmson = new BmsonMain(bmsonText);

                    WavComboBox.ItemsSource = GetWavObservableCollection();
                    WavComboBox.SelectedIndex = 0;

                    OutputButton.IsEnabled = true;
                    OutputAllButton.IsEnabled = true;
                    WavComboBox.IsEnabled = true;
                }
            }
            catch
            {
                MessageBox.Show("BMSONファイルを読み込んでください。");
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
            MessageBox.Show("終了しました。");
        }

        /// <summary>
        /// すべてのwavを切断する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutputAllButton_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < _bmson!.Bmson!.sound_channels.Length; i++)
            {
                WriteWavs(i);
            }
            MessageBox.Show("終了しました。");
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
                return;
            }

            // wavを切断し、保存する
            string saveDirectory = GetOutputBmsDirectoryName(_bmson.Bmson.sound_channels[chIndex].name);
            Directory.CreateDirectory(saveDirectory);
            var bmsText = _bmson.CutWav(saveDirectory, readWavFilePath, chIndex);
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
    }
}
