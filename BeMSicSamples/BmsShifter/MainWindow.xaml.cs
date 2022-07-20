using BeMSic.BmsFileOperator;
using BeMSic.Core.BmsDefinition;
using Microsoft.Win32;
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
        BmsConverter? _targetBmsConverter;
        BmsConverter? _wavBmsConverter;
        string? _targetOutputName;
        string? _wavOutputName;

        public MainWindow()
        {
            InitializeComponent();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        private void OutputButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Output wav added bms
                if (_wavOutputName != null)
                {
                    _wavBmsConverter.WavMarge(_targetBmsConverter.Bms);
                    File.WriteAllText(_wavOutputName, _wavBmsConverter.Bms);
                }

                // Output wav shifted bms
                var offset = new WavDefinition(WavStartTextBox.Text);
                var shift = int.Parse(BgmShiftSizeTextBox.Text);
                _targetBmsConverter.Offset(offset.Num).Shift(shift);
                File.WriteAllText(_targetOutputName, _targetBmsConverter.Bms);

                MessageBox.Show("Completed.");
            }
            catch
            {
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
            var bmsText = File.ReadAllText(fileName, Encoding.GetEncoding("shift_jis"));
            _wavBmsConverter = new BmsConverter(bmsText);

            _wavBmsConverter.DeleteUnusedWav().ArrangeWav();
            var replace = new BmsDefinitionReplace(_wavBmsConverter.Bms);
            WavDefinitions wavs = replace.GetUsedWavList();
            WavStartTextBox.Text = wavs.GetMax().ZZ;
            _wavOutputName = Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName) + "_WavAdded.bms";
        }

        /// <summary>
        /// bmsファイルを開く
        /// </summary>
        /// <param name="fileName"></param>
        private void TargetBmsPath_OpenBms(string fileName)
        {
            var bmsText = File.ReadAllText(fileName, Encoding.GetEncoding("shift_jis"));
            _targetBmsConverter = new BmsConverter(bmsText);
            TargetBmsPathTextBox.Text = fileName;
            _targetOutputName = Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName) + "_WavShifted.bms";
        }
    }
}
