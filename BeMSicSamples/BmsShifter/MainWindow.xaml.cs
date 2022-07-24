using BeMSic.BmsFileOperator;
using BeMSic.Core.BmsDefinition;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows;

namespace BmsShifter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string? _targetOutputName;

        public MainWindow()
        {
            InitializeComponent();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            OutputButton.IsEnabled = false;
        }

        private void OutputButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var bmsText = File.ReadAllText(TargetBmsPathTextBox.Text, Encoding.GetEncoding("shift_jis"));
                var targetBmsConverter = new BmsConverter(bmsText);

                // Output wav added bms
                if (WavBmsPathLabel.Text != String.Empty)
                {
                    var wavBmsText = File.ReadAllText(WavBmsPathLabel.Text, Encoding.GetEncoding("shift_jis"));
                    var wavBmsConverter = new BmsConverter(wavBmsText);
                    wavBmsConverter.DeleteUnusedWav().ArrangeWav();

                    wavBmsConverter.WavMarge(targetBmsConverter.Bms);
                    File.WriteAllText(GetWavAddedOutputName(WavBmsPathLabel.Text), wavBmsConverter.Bms);
                }

                // Output wav shifted bms
                var offset = new WavDefinition(WavStartTextBox.Text);
                var shift = int.Parse(BgmShiftSizeTextBox.Text);
                targetBmsConverter.Offset(offset.Num - 1).Shift(shift);
                File.WriteAllText(_targetOutputName, targetBmsConverter.Bms);
                WavBmsPathLabel.Text = String.Empty;

                MessageBox.Show("Completed.");
            }
            //catch (ArgumentOutOfRangeException)
            //{
            //    MessageBox.Show("Input is out of range.");
            //}
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show("Please read the bms file.");
            }
        }

        /// <summary>
        /// #WAV Startボタン処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WavStartFromBmsButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "bms file|*.bms;*.bme;*.bml;*.pms|All file|*.*"
            };

            try
            {
                if (dialog.ShowDialog() == false)
                {
                    return;
                }

                WavStart_OpenBms(dialog.FileName);
            }
            catch
            {
                MessageBox.Show("Please read the bms file.");
            }
        }

        /// <summary>
        /// TargetBmsPathボタン処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TargetBmsPathButton_Click(object sender, RoutedEventArgs e)
        {
            var loadDialog = new OpenFileDialog
            {
                Filter = "Input bms file|*.bms;*.bme;*.bml;*.pms|All file|*.*"
            };

            if (loadDialog.ShowDialog() == false)
            {
                return;
            }

            TargetBmsPath_OpenBms(loadDialog.FileName);
        }

        /// <summary>
        /// Windowへのドロップ時処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WavStart_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return;
            }

            var fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
            WavStart_OpenBms(fileNames[0]);
        }

        /// <summary>
        /// Windowへのドロップ時処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TargetBmsPath_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return;
            }

            var fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
            TargetBmsPath_OpenBms(fileNames[0]);
        }

        /// <summary>
        /// WindowへのDragOver処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragOver(object sender, DragEventArgs e)
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
        /// bmsファイルを開く
        /// </summary>
        /// <param name="fileName"></param>
        private void WavStart_OpenBms(string fileName)
        {
            WavBmsPathLabel.Text = fileName;
            var bmsText = File.ReadAllText(fileName, Encoding.GetEncoding("shift_jis"));
            var wavBmsConverter = new BmsConverter(bmsText);
            wavBmsConverter.DeleteUnusedWav().ArrangeWav();
            var replace = new BmsDefinitionReplace(wavBmsConverter.Bms);
            WavDefinitions wavs = replace.GetUsedWavList();
            var wavStart = new WavDefinition(wavs.GetMax().Num + 1);
            WavStartTextBox.Text = wavStart.ZZ;
        }

        /// <summary>
        /// bmsファイルを開く
        /// </summary>
        /// <param name="fileName"></param>
        private void TargetBmsPath_OpenBms(string fileName)
        {
            TargetBmsPathTextBox.Text = fileName;
            _targetOutputName = Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName) + "_WavShifted.bms";
            OutputButton.IsEnabled = true;
        }

        private string GetWavAddedOutputName(string fileName)
        {
            return Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName) + "_WavAdded.bms";
        }
    }
}
