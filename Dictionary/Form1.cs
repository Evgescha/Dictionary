using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dictionary
{
    public partial class Form1 : Form
    {

        Dictionary<string, Dictionary<string, string>> dictionaries = new Dictionary<string, Dictionary<string, string>>();
        public static string path = Directory.GetCurrentDirectory() + "\\dict\\";
        string[] dictFolders;
        public string folder = "";
        string useDict = "";

        FontDialog fontDlg;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            addFont();
            loadDictionaryFolder();
            checkFolders();
            addButtons();
        }

        // получение всех папок
        private void loadDictionaryFolder()
        {
            Console.WriteLine("Load " + path);
            dictFolders = Directory.GetFileSystemEntries(path);
        }
        //просмотр всех папок
        private void checkFolders()
        {
            for (int i = 0; i < dictFolders.Length; i++)
            {
                folder = dictFolders[i];
                Console.WriteLine("Folder with dict: " + folder);
                checkDict(folder);
            }
        }
        // проверка папки на содержание словаря
        private void checkDict(string path)
        {
            try
            {
                // файлы в папке
                string[] files = Directory.GetFiles(path);
                // ридеры для каждого файла
                StreamReader[] readers = new StreamReader[3];
                // имя текущего словаря
                string dictName = "";
                //слова и определения текучего словаря
                Dictionary<string, string> dic = new Dictionary<string, string>();

                //создаем ридеры для каждого файла
                for (int i = 0; i < files.Length; i++)
                {
                    readers[i] = new StreamReader(files[i], Encoding.UTF8);
                }
                // получаем слова и определения для каждого
                string desc, word;
                while ((word = readers[1].ReadLine()) != null)
                {
                    //Console.WriteLine(word);
                    // убираем лишние цифры
                    if (word.Contains('\t'))
                        word = word.Split('\t')[0];
                    word = word.Trim();
                    desc = readers[0].ReadLine();
                    //если у слова несколько определений, добавляем к уже написанным
                    if (dic.ContainsKey(word))
                    {

                        dic[word] = dic[word] + "\n\r" + desc;
                    }
                    else
                        dic.Add(word, desc);
                }
                //прочитали все слова, теперь читаем название словаря
                while ((dictName = readers[2].ReadLine()) != null)
                {
                    if (dictName.Contains("SHORT_NAME="))
                    {
                        dictName = dictName.Replace("SHORT_NAME=", "");
                        break;
                    }
                }
                SortedDictionary<string, string> sortedSections = new SortedDictionary<string, string>(dic);
                dic = new Dictionary<string, string>(sortedSections);
                dictionaries.Add(dictName, dic);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
        //добавление кнопок на форму для выбора словаря
        private void addButtons()
        {
            RadioButton rb=null;
            Console.WriteLine("LINES:" + dictionaries.Count);
            for (int i = 0; i < dictionaries.Count; i++)
            {
                rb = new RadioButton();
                rb.Text = dictionaries.ElementAt(i).Key;
                rb.Location = new Point(12 + 80 * i, 30);
                rb.AutoSize = true;
                rb.Font = SystemFonts.DefaultFont;
                rb.BackColor = Color.Transparent;
                rb.Click += radioButton_Click;
                this.Controls.Add(rb);
            }
            rb.Checked = true;

        }

        private void radioButton_Click(object sender, EventArgs e)
        {
            useDict = ((RadioButton)sender).Text;
            Console.WriteLine(useDict);
            find();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (useDict != "" && listBox1.Items.Count > 0)
                textBox1.Text = dictionaries[useDict][listBox1.SelectedItem.ToString()];
        }

        private void find() {
            if (useDict == "") return;
            Dictionary<string, string> tempD = dictionaries[useDict];
            int index, left, right;
            string str = textBox2.Text.ToUpper();

            textBox1.Text = "Не найдено";
            listBox1.Items.Clear();
            
            //находим хоть что-то из строки пользователя
            while (true) {
                if (str.Length == 0) break;
                if (tempD.ContainsKey(str)) break;
                str = str.Substring(0, str.Length - 1);
            }
            // проверяем, есть ли точно то, что вводит пользователь
            if (tempD.ContainsKey(textBox2.Text.ToUpper()))
            {
                index = Array.IndexOf(tempD.Keys.ToArray(), textBox2.Text.ToUpper());
                left = index > 25 ? index - 25 : 0;
                right = index + 25;
                for (int i = left; i < right; i++) {
                    listBox1.Items.Add(tempD.ElementAt(i).Key);
                }
                listBox1.SelectedIndex = index - left;
            }
            // если нет, выводим хоть часть того, что пользователь ввел
            else {
                if (str.Length == 0) return;
                index = Array.IndexOf(tempD.Keys.ToArray(), str);
                left = index > 25 ? index - 25 : 0;
                right = index + 25;
                for (int i = left; i < right; i++)
                {
                    listBox1.Items.Add(tempD.ElementAt(i).Key);
                }
                listBox1.SelectedIndex = index - left;
            }


        }

       

        private void comboBox1_KeyUp(object sender, KeyEventArgs e)
        {
            find();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            find();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            if (fontDlg.ShowDialog() != DialogResult.Cancel)
            {
                listBox1.Font = fontDlg.Font;
            }
        }

        private void шрифтОпределенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog fontDlg = new FontDialog();
            if (fontDlg.ShowDialog() != DialogResult.Cancel)
            {
                textBox1.Font = fontDlg.Font;
            }
        }
        private void addFont() {
            fontDlg = new FontDialog();
            fontDlg.ShowColor = true;
            fontDlg.ShowApply = true;
            fontDlg.ShowEffects = true;
            fontDlg.ShowHelp = true;
        }
    }
}
