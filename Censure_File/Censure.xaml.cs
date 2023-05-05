using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Censure_File
{
    /// <summary>
    /// Логика взаимодействия для Censure.xaml
    /// </summary>
    public partial class Censure : Window
    {
        Thread thread;
        List<object> list = new List<object>();
        int obsh_colvo_zap = 0;
        public Censure()
        {
            InitializeComponent();
            string str = "";
            string h = "";
            int number_path = 0;
            thread = new Thread(NachCensure);
            list.Add(MainWindow.censure_word);
            list.Add(str);
            list.Add(h);
            list.Add(MainWindow.List_path);
            list.Add(number_path);
            BT2.IsEnabled = false;
            thread.Start(list);
        }
        public void NachCensure(object obj)
        {
            for (int i = 0; i < ((obj as List<object>)[3] as List<string>).Count(); i++)
            {
                Thread thread2 = new Thread(Proverka);
                Progress2.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => Progress2.Value = 0));
                Progress1.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => Progress1.Value += 100 / ((obj as List<object>)[3] as List<string>).Count()));
                thread2.Start(obj);
                thread2.Join();
                if(i!= ((obj as List<object>)[3] as List<string>).Count()-1)     (obj as List<object>)[4] =i + 1;
                Thread.Sleep(100);
            }
            string path = ((obj as List<object>)[3] as List<string>)[Convert.ToInt32((obj as List<object>)[4].ToString())];
            for (int i = path.Count() - 1; i > 1; i--)
            {
                if (path[i] == '\\')
                {
                    break;
                }
                path = path.Remove(path.Count() - 1);
            }
            using (StreamWriter StrWrite = new StreamWriter(path + "Obsh_Zap.txt"))
            {
                StrWrite.WriteLine($"Общее количество запрещённых слов {obsh_colvo_zap}");
            }
            Thread.Sleep(500);
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => this.Close()));
        }
        public void Proverka(object obj)
        {
            string path = ((obj as List<object>)[3] as List<string>)[Convert.ToInt32((obj as List<object>)[4].ToString())];
            for (int i = path.Count() - 1; i > 1; i--)
            {
                if (path[i] == '.')
                {
                    path = path.Remove(path.Count() - 1);
                    break;
                }
                path = path.Remove(path.Count() - 1);
            }
            using (StreamReader StrRead = new StreamReader(path+".txt"))
            {
                (obj as List<object>)[1] = StrRead.ReadToEnd();
            }
            (obj as List<object>)[1] = ((obj as List<object>)[1] as string).ToLower();
            string text = ((obj as List<object>)[1] as string);
            string word = "";
            int numZap = 0;
            if (text.Length <= 30000)
            { 
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == ' ' || text[i] == '\n' || text[i] == '\0' || text[i] == '.' || text[i] == '\r' || text.Count() == i + 1)
                    {
                        if (text.Count() == i + 1)
                        {
                            word = word + text[i];
                        }
                        word = word.ToLower();
                        foreach (string s in ((obj as List<object>)[0] as List<string>))
                        {
                            if (word == s)
                            {
                                numZap++;
                            }
                        }
                        word = "";
                    }
                    else
                    {
                        word = word + text[i];
                    }
                }
            }
            else
            {
                MessageBox.Show("Символов больше 30000, к сожалению подсчитать число запрещённых слов слишком затратно", "Большой текст", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.Yes);
            }
            obsh_colvo_zap += numZap;
            foreach (string s in ((obj as List<object>)[0] as List<string>))
            {
                Progress2.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,new Action(() => Progress2.Value += 100 / ((obj as List<object>)[0] as List<string>).Count()));
                (obj as List<object>)[1] = ((obj as List<object>)[1] as string).Replace(s, "******");
            }
            using (StreamWriter StrWrite = new StreamWriter(path + "_CountZapret.txt"))
            {
                StrWrite.WriteLine($"Количество запрещённых слов {numZap}");
            }
            using (StreamWriter StrWrite = new StreamWriter(path + "_Itog.txt"))
            {
                StrWrite.WriteLine((obj as List<object>)[1]);
            }
            using (StreamReader StrRead = new StreamReader(path + "_Itog.txt"))
            {
                (obj as List<object>)[2] = StrRead.ReadToEnd();
            }
        }
        private void Button_Click_END(object sender, RoutedEventArgs e)
        {
            if (thread.ThreadState == ThreadState.Suspended)
            {
            thread.Resume();
            }
            thread.Abort();
            this.Close();
        }

        private void Button_Click_START(object sender, RoutedEventArgs e)
        {
            thread.Resume();
            BT1.IsEnabled= true;
            BT2.IsEnabled= false;
        }

        private void Button_Click_STOP(object sender, RoutedEventArgs e)
        {
            if (thread.ThreadState == ThreadState.Running || thread.ThreadState == ThreadState.WaitSleepJoin)
            {
                thread.Suspend();
                BT1.IsEnabled = false;
                BT2.IsEnabled = true;
            }
        }
    }
}
