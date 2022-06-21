using BeMSic.BmsFileOperator;
using BeMSic.Core.BmsDefinition;
using BeMSic.Core.Helpers;
using BeMSic.Wave.DefinitionReductor;
using BmsDefinitionReductorDemo.Class;
using Microsoft.Win32;
using NAudio.Wave;
using System;
using System.Collections.Generic;
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
        List<WavFileUnit>? _files;
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
                        StatusLabel.Content = "完了しました。";
                        DefinitionReductButton.IsEnabled = true;
                        LoadBmsButton.IsEnabled = true;
                        break;

                    default:
                        StatusLabel.Content = "実行中...";
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
            var dialog = new OpenFileDialog();
            dialog.Filter = "BMSファイル (*.*)|*.*";

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
                MessageBox.Show("BMSファイルを読み込んでください。");
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
                MessageBox.Show("wavファイルがありません。");
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
            var dialog = new SaveFileDialog();
            dialog.Filter = "出力BMSファイル(*.bms)|*.bms|全てのファイル(*.*)|*.*";
            dialog.FileName = OutputFileName;
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
                MessageBox.Show("相関係数は0から1の間で入力してください。");
                return;
            }

            // 処理実行開始
            StateViewBusy(true);
            int start = RadixConvert.ZZToInt(Definition_Start.Text);
            int end = RadixConvert.ZZToInt(Definition_End.Text);
            try
            {
                await Task.Run(() =>
                {
                    var partialFiles = WavFileUnitUtility.GetPartialWavList(_files!, start, end);
                    var replaces = DefinitionReductor.GetWavReplaces(partialFiles, _progress, r2Val);

                    _bmsConverter!.Replace(replaces).DeleteUnusedWav().ArrangeWav();
                    File.WriteAllText(dialog.FileName, _bmsConverter.Bms);
                });
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("定義は2桁の01-ZZで、右の値が大きくなるように入力してください。");
                StatusLabel.Content = "BMSファイルを読み込んでください。";
            }
            catch
            {
                MessageBox.Show("ファイルが見つかりませんでした。");
                StatusLabel.Content = "BMSファイルを読み込んでください。";
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
        private static ObservableCollection<WavFileUnitEx> GetDisplayedValuesList(List<WavFileUnit> fileListBase)
        {
            ObservableCollection<WavFileUnitEx> fileList = new ObservableCollection<WavFileUnitEx>();

            foreach (var file in fileListBase)
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
