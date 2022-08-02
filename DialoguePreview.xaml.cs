using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace HedgeHex
{
    /// <summary>
    /// Interaction logic for DialoguePreview.xaml
    /// </summary>
    public partial class DialoguePreview : Window
    {
        public DialoguePreview()
        {
            InitializeComponent();
        }

        public void PreviewChanges()
        {
            Text1.Text = Text1.Text.Replace("\\n", Environment.NewLine);
            Text1.Text = Text1.Text.Replace("\\q", "\"");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Windows.OfType<MainWindow>().FirstOrDefault().preview = null;
        }
    }
}
