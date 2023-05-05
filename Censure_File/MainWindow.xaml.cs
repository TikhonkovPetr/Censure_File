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

namespace Censure_File
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        public static string path_File = "";
        public static List<string> censure_word = new List<string>();
        public static List<string> List_path = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
            openFileDialog.Filter = "Текстовые файлы|*.txt";
        }

        private void Button_Click_Prov(object sender, RoutedEventArgs e)
        {
            Censure censure =new Censure();
            censure.ShowDialog();
        }
        private void Button_Click_Vibr(object sender, RoutedEventArgs e)
        {
            if (openFileDialog.ShowDialog() == true)
            {
                path_File = openFileDialog.FileName;
                for (int i = path_File.Count() - 1; i > 1; i--)
                {
                    if (path_File[i] == '\\') break;
                    path_File = path_File.Remove(path_File.Count() - 1);
                }
                Path.Text = path_File;
                DirSearchFile(path_File);
            }
        }
        private void DirSearchFile(string path)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                foreach (var item in dir.GetFiles())
                {
                    if (item.Extension == ".txt")
                    {
                        List_path.Add(item.FullName);
                    }
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        private void TxtBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        { 
        }

        private void TxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string word = "";
            censure_word.Clear();
            for (int i = 0; i < TxtBox.Text.Count(); i++)
            {
                if (TxtBox.Text[i] == ' ' || TxtBox.Text[i] == '\n' || TxtBox.Text[i] == '\0' || TxtBox.Text[i] == '.' || TxtBox.Text[i] == '\r' || TxtBox.Text.Count() == i + 1)
                {
                    if (TxtBox.Text.Count() == i + 1)
                    {
                        word = word + TxtBox.Text[i];
                    }
                    word = word.ToLower();
                    censure_word.Add(word);
                    word = "";
                }
                else
                {
                    word = word + TxtBox.Text[i];
                }
            }
        }
    }
}
