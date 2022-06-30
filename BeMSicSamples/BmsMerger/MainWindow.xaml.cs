using BeMSic.BmsFileOperator;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows;

namespace BmsMerger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BmsConverter? _bms1Converter;
        BmsConverter? _bms2Converter;
        readonly string _outputFileName = @"out.bms";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MergeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ReadFiles();
            }
            catch
            {
                MessageBox.Show("Invalid file path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                _bms1Converter!.DeleteUnusedWav().ArrangeWav();
                _bms2Converter!.DeleteUnusedWav().ArrangeWav();
                _bms1Converter.AddRange(_bms2Converter);

                // Output
                File.WriteAllText(OutputFileTextBox.Text, _bms1Converter.Bms);
                MessageBox.Show("Completed.");
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("The #WAV definition is out of range.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            {
                MessageBox.Show("Error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void File1BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var openDialog1 = new OpenFileDialog();
            openDialog1.Filter = "BMS file 1 (*.*)|*.*";
            if (openDialog1.ShowDialog() == false)
            {
                return;
            }
            File1BrowseTextBox.Text = openDialog1.FileName;
            if (OutputFileTextBox.Text == "")
            {
                OutputFileTextBox.Text = Path.GetDirectoryName(File1BrowseTextBox.Text) + "\\" + _outputFileName;
                OutputFileCautionLabel.Content = GetOutputFileCautionText(OutputFileTextBox.Text);
            }
        }

        private void File2BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var openDialog2 = new OpenFileDialog();
            openDialog2.Filter = "BMS file 2 (*.*)|*.*";
            if (openDialog2.ShowDialog() == false)
            {
                return;
            }
            File2BrowseTextBox.Text = openDialog2.FileName;
        }

        private void OutputFileBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Output BMS file(*.bms)|*.bms|All files(*.*)|*.*";
            saveDialog.FileName = OutputFileTextBox.Text;
            if (saveDialog.ShowDialog() == false)
            {
                return;
            }
            OutputFileTextBox.Text = saveDialog.FileName;
            OutputFileCautionLabel.Content = GetOutputFileCautionText(OutputFileTextBox.Text);
        }

        private void OutputFileTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            OutputFileCautionLabel.Content = GetOutputFileCautionText(OutputFileTextBox.Text);
        }

        private void ReadFiles()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var bms1Text = File.ReadAllText(File1BrowseTextBox.Text, Encoding.GetEncoding("shift_jis"));
            _bms1Converter = new BmsConverter(bms1Text);

            var bms2Text = File.ReadAllText(File2BrowseTextBox.Text, Encoding.GetEncoding("shift_jis"));
            _bms2Converter = new BmsConverter(bms2Text);
        }

        private string GetOutputFileCautionText(string filePath)
        {
            if (File.Exists(filePath))
            {
                return "The output file exists.";
            }
            else
            {
                return "";
            }
        }
    }
}
