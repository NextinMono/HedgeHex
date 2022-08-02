using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Microsoft.Win32;
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace HedgeHex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        static string? ProgramPath
        {
            get
            {
                string exePath = AppDomain.CurrentDomain.BaseDirectory;
                string exeDir = Path.GetDirectoryName(exePath);
                return exeDir;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        string input;
        string tableFileName;
        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            input = Textbox.Text;
            SaveJson(new ConversionTable('A'));
            if (Directory.Exists(@"tables\"))
                Directory.CreateDirectory(@"tables\");
            if (!string.IsNullOrEmpty(tableFileName))
            {
                var table = LoadTable(tableFileName);

                string output = "";
                for (int a = 0; a < input.Length; a++)
                {
                    bool match = false;
                    //If current char is \ and index + 1 isnt over the list lenght (a.k.a check for newline)
                    if (input[a] == '\\' && a + 1 <= input.Length)
                    {
                        switch (input[a + 1])
                        {
                            case 'n': //newline
                                {
                                    output += table.FindHexStringByName("newline");
                                    break;
                                }
                            case 'q': //quotes (")
                                {
                                    output += table.FindHexStringByName("quote");
                                    break;
                                }
                            default:
                                break;
                        }
                        a++; //advances by 1 letter otherwise n or q would be added to output
                        continue;
                    }
                    //Check for letter in database
                    for (int b = 0; b < table.letters.Count; b++)
                    {
                        if (input[a] == table.letters[b].letter.ToCharArray()[0])
                        {
                            match = true;
                            output += table.letters[b].hexString;
                            output += " ";
                            break;
                        }
                    }
                    if (match == false)
                    {
                        MessageBox.Show($"Could not find translation for [{input[a]}]", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                MessageBox.Show($"Translation: {output}","HedgeHex",MessageBoxButton.OK);

                MessageBoxResult result = MessageBox.Show("Would you like to copy the output?", "HedgeHex", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);

                if (result == MessageBoxResult.Yes) { Clipboard.SetText(output); }
            }

        }

        private void TranslationTableButton_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog open = new OpenFileDialog()
            {
                InitialDirectory = Path.Combine(ProgramPath,"tables"),
                Title = "Select Translation Table",
                Filter = "JSON File (.json) | *.json",
                FileName = " "
            };
            if (open.ShowDialog() == true)
            {
                DirectoryInfo d = new DirectoryInfo(@"tables\");
                FileInfo[] translationTables = d.GetFiles("*.json", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < translationTables.Length; i++)
                {
                    if (translationTables[i].Name == open.SafeFileName)
                    {
                        tableFileName = open.SafeFileName;
                    }
                }
            }
        }

        private static void SaveJson(ConversionTable table)
        {
            using (FileStream fileStream = File.Create(Path.Combine(ProgramPath, "cpkfiles2.json")))
            {
                string jsonSerialized = JsonConvert.SerializeObject(table, Formatting.Indented);

                byte[] buffer = Encoding.Default.GetBytes(jsonSerialized);
                fileStream.Write(buffer, 0, buffer.Length);

                fileStream.Close();
            }

        }
        static ConversionTable LoadTable(string path)
        {
            string test = Path.Combine(ProgramPath, "tables", path);
            using (FileStream fileStream = File.OpenRead(test))
            {
                byte[] buffer = new byte[new FileInfo(test).Length];
                fileStream.Read(buffer, 0, buffer.Length);

                string json = Encoding.Default.GetString(buffer);
                var obj = JsonConvert.DeserializeObject<ConversionTable>(json);
                //for (int i = 0; i < obj.letters.Count; i++)
                //{
                //    var temp = Regex.Replace(obj.letters[i].hexString.ToString(), @"(.{2})", "$1 ").TrimEnd();
                //    var temp2 = obj.letters[i];
                //    temp2.temporaryDoNotChange = temp;
                //    obj.letters[i] = temp2;
                //}
                return obj;
            }
        }

        private void Import_Text_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog()
            {
                InitialDirectory = ProgramPath,
                Title = "Import Text File",
                Filter = "Text File (.txt) | *.txt",
                FileName = " "
            };
            if (open.ShowDialog() == true)
            {
                StreamReader stream = new StreamReader(open.FileName);
                Textbox.Text = stream.ReadToEnd();
                input = stream.ReadToEnd();
                MessageBox.Show($"Imported text file : {open.SafeFileName}", "HedgeHex", MessageBoxButton.OK);
            }
        }
        public DialoguePreview preview = null;
        private void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            if(preview == null)
            {
                preview = new DialoguePreview();
                preview.Show();
                preview.Text1.Text = Textbox.Text;
                preview.PreviewChanges();
            }
            else
            {
                preview.Text1.Text = Textbox.Text;
                preview.PreviewChanges();
            }
        }
    }
    public class ConversionTable
    {
        [JsonProperty("ConversionTable")]
        public List<LetterComparison> letters = new List<LetterComparison>();
        public ConversionTable(char letter)
        {
        }
        public string FindHexStringByName(string name)
        {
            for (int i = 0; i < letters.Count; i++)
            {
                if (letters[i].letter.ToString() == name)
                {
                    return letters[i].hexString;
                }
            }
            return "Not Found";
        }
    }
    public struct LetterComparison
    {
        public string letter;
        public string hexString;
        [JsonIgnore]
        public string temporaryDoNotChange;
        public LetterComparison(char l, string h, string n = "")
        {
            letter = l.ToString();
            hexString = h;
            temporaryDoNotChange = n;
        }
    }
}
