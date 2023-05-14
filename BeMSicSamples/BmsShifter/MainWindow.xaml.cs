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

                // Output wav shifted bms
                var offset = new WavDefinition(WavStartTextBox.Text);
                var shift = int.Parse(BgmShiftSizeTextBox.Text);
                targetBmsConverter.Offset(offset.Num - 1).Shift(shift);
                File.WriteAllText(_targetOutputName, targetBmsConverter.Bms);

                MessageBox.Show("Completed.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
