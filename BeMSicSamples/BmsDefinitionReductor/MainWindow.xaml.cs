using BeMSic.BmsFileOperator;
using BeMSic.Core.BmsDefinition;
using BeMSic.Wave;
using BmsDefinitionReductor.Class;
using Microsoft.Win32;
using NAudio.Wave;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BmsDefinitionReductor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BmsConverter? _bmsConverter;
        WavFileUnitUtility? _files;
        readonly string OutputFileName = @"out.bms";
        private Progress<int> _progress;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            StateViewInitial();

            _progress = new Progress<int>((percent) =>
            {
                DefinitionReductProgressBar.Value = percent;
                switch (percent)
                {
                    case 100:
                        StatusLabel.Content = Properties.Resources.StatusLabelContentComplete;
                        DefinitionReductButton.IsEnabled = true;
                        LoadBmsButton.IsEnabled = true;
                        break;

                    default:
                        StatusLabel.Content = Properties.Resources.StatusLabelContentProcessing;
                        break;
                }
            });
        }

        /// <summary>
        /// 読み込み処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadBms_Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = Properties.Resources.LoadBmsDialogFilter,
            };

            try
            {
                if (dialog.ShowDialog() == false)
                {
                    return;
                }

                string? bmsDirectory = Path.GetDirectoryName(dialog.FileName);
                if (bmsDirectory == null)
                {
                    throw new FileNotFoundException();
                }

                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                var bms = File.ReadAllText(dialog.FileName, Encoding.GetEncoding("shift_jis"));
                _bmsConverter = new BmsConverter(bms);
                _files = FileList.GetWavsFullPath(_bmsConverter.Bms, bmsDirectory);

                FilesListView.ItemsSource = GetDisplayedValuesList(_files);
                DefinitionReductButton.IsEnabled = true;
            }
            catch
            {
                MessageBox.Show(Properties.Resources.StatusLabelContentWaitFile);
            }
        }

        /// <summary>
        /// wavリスト選択時処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WavFileUnit item = (WavFileUnit)FilesListView.SelectedItem;
            try
            {
                WaveFileReader reader = new WaveFileReader(item.Name);
                WaveOut waveOut = new WaveOut();
                waveOut.Init(reader);
                waveOut.Play();
            }
            catch
            {
                MessageBox.Show(Properties.Resources.MessageFileNotFound);
            }
        }

        /// <summary>
        /// 開始ボタン処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DefinitionReuseButton_Click(object sender, RoutedEventArgs e)
        {
            // ダイアログ表示
            var dialog = new SaveFileDialog
            {
                Filter = Properties.Resources.LoadBmsDialogFilter,
                FileName = OutputFileName
            };

            if (dialog.ShowDialog() == false)
            {
                return;
            }

            // 相関係数の取得
            float r2Val;
            try
            {
                r2Val = GetR2Val();
            }
            catch
            {
                MessageBox.Show(Properties.Resources.MessageR2valOutOfRange);
                return;
            }

            // 処理実行開始
            StateViewBusy(true);
            var start = new WavDefinition(Definition_Start.Text);
            var end = new WavDefinition(Definition_End.Text);
            bool lengthMatchIsChecked = false;
            if (LengthMatchCheckBox.IsChecked != null)
            {
                lengthMatchIsChecked = (bool)LengthMatchCheckBox.IsChecked;
            }

            try
            {
                await Task.Run(() =>
                {
                    var partialFiles = _files.GetPartialWavs(start, end);
                    var reductor = new DefinitionReductor(partialFiles, lengthMatchIsChecked, r2Val);
                    var replaces = reductor.GetWavReplaces(_progress);

                    _bmsConverter!.Replace(replaces).DeleteUnusedWav().ArrangeWav();
                    File.WriteAllText(dialog.FileName, _bmsConverter.Bms);
                });
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show(Properties.Resources.MessageWavOutOfRange);
                StatusLabel.Content = Properties.Resources.StatusLabelContentWaitFile;
            }
            catch
            {
                MessageBox.Show(Properties.Resources.MessageFileNotFound);
                StatusLabel.Content = Properties.Resources.StatusLabelContentWaitFile;
            }
            finally
            {
                StateViewBusy(false);
            }
        }

        /// <summary>
        /// 初期表示
        /// </summary>
        private void StateViewInitial()
        {
            DefinitionReductButton.IsEnabled = false;
            LoadBmsButton.IsEnabled = true;
            FilesListView.IsEnabled = true;
            R2TextBox.IsEnabled = true;
            Definition_Start.IsEnabled = true;
            Definition_End.IsEnabled = true;
        }

        /// <summary>
        /// 動作中表示
        /// </summary>
        /// <param name="isBusy"></param>
        private void StateViewBusy(bool isBusy)
        {
            bool isEnable = !isBusy;
            DefinitionReductButton.IsEnabled = isEnable;
            LoadBmsButton.IsEnabled = isEnable;
            FilesListView.IsEnabled = isEnable;
            R2TextBox.IsEnabled = isEnable;
            Definition_Start.IsEnabled = isEnable;
            Definition_End.IsEnabled = isEnable;
        }

        /// <summary>
        /// 表示用リストの取得
        /// </summary>
        /// <param name="fileListBase"></param>
        /// <returns></returns>
        private static ObservableCollection<WavFileUnitEx> GetDisplayedValuesList(WavFileUnitUtility fileListBase)
        {
            ObservableCollection<WavFileUnitEx> fileList = new ObservableCollection<WavFileUnitEx>();

            foreach (var file in fileListBase.Files)
            {
                fileList.Add(new WavFileUnitEx(file));
            }
            return fileList;
        }

        /// <summary>
        /// 相関係数のしきい値を取得する
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private float GetR2Val()
        {
            float r2val = float.Parse(R2TextBox.Text);
            if (1.0 < r2val)
            {
                throw new ArgumentOutOfRangeException();
            }
            return r2val;
        }
    }
}
