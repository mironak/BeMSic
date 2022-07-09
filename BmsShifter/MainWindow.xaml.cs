using BeMSic.BmsFileOperator;
using BeMSic.Core.BmsDefinition;
using BeMSic.Core.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BmsShifter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BmsConverter? _bmsConverter;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var loadDialog = new OpenFileDialog
            {
                Filter = "BMSファイル (*.*)|*.*"
            };
            var saveDialog = new SaveFileDialog
            {
                Filter = "BMSファイル (*.*)|*.*"
            };

            try
            {
                if (loadDialog.ShowDialog() == false)
                {
                    return;
                }

                if (saveDialog.ShowDialog() == false)
                {
                    return;
                }

                var bmsText = File.ReadAllText(loadDialog.FileName, Encoding.GetEncoding("shift_jis"));
                _bmsConverter = new BmsConverter(bmsText);

                var offset = new WavDefinition(WavStartTextBox.Text);
                _bmsConverter.Offset(offset.Num);

                var shift = int.Parse(BgmShiftSizeTextBox.Text);
                _bmsConverter.Shift(shift);

                // Output
                File.WriteAllText(saveDialog.FileName, _bmsConverter.Bms);
                MessageBox.Show("Completed.");
            }
            catch
            {
                MessageBox.Show("BMSファイルを読み込んでください。");
            }
        }

        private void WavStartFromBmsButton_Click(object sender, RoutedEventArgs e)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var dialog = new OpenFileDialog
            {
                Filter = "BMSファイル (*.*)|*.*"
            };

            try
            {
                if (dialog.ShowDialog() == false)
                {
                    return;
                }
                var bmsFileName = dialog.FileName;

                var bmsText = File.ReadAllText(bmsFileName, Encoding.GetEncoding("shift_jis"));
                var bmsConverter = new BmsConverter(bmsText);

                bmsConverter.DeleteUnusedWav().ArrangeWav();
                var replace = new BmsDefinitionReplace(bmsConverter.Bms);
                List<WavDefinition> wavs = replace.GetUsedWavList();
                WavStartTextBox.Text = RadixConvert.IntToZZ(wavs.Max(x => x.Num));
            }
            catch
            {
                MessageBox.Show("BMSファイルを読み込んでください。");
            }
        }
    }
}
