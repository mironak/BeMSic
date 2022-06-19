using BeMSic.BmsFileOperator;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;

namespace BmsDefinitionCompressor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BmsConverter? _bmsConverter;
        readonly string OutputFileName = @"out.bms";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog();
            openDialog.Filter = "BMSファイル (*.*)|*.*";
            if (openDialog.ShowDialog() == true)
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                var bms = File.ReadAllText(openDialog.FileName, Encoding.GetEncoding("shift_jis"));
                _bmsConverter = new BmsConverter(bms);
            }
            else
            {
                return;
            }

            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "出力BMSファイル(*.bms)|*.bms|全てのファイル(*.*)|*.*";
            saveDialog.FileName = OutputFileName;
            if (saveDialog.ShowDialog() == false)
            {
                return;
            }

            try
            {
                _bmsConverter.DeleteUnusedWav().ArrangeWav();
                File.WriteAllText(saveDialog.FileName, _bmsConverter.Bms);
            }
            catch
            {
                MessageBox.Show("ファイルが見つかりませんでした。");
            }
        }
    }
}
