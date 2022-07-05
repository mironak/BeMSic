using BeMSic.BmsFileOperator;
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
                _bmsConverter = new BmsConverter(bmsText);

                _bmsConverter.Shift(3);


                File.WriteAllText("a.bms", _bmsConverter.Bms);
            }
            catch
            {
                MessageBox.Show("BMSファイルを読み込んでください。");
            }
        }
    }
}
