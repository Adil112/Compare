using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Antivirus;

namespace Antivirus
{
    public partial class Form1 : Form
    {
        string path = null;
        int format = 0;
        Book bookFromBD = null;
        public Form1()
        {
            InitializeComponent();
            BookContext db = new BookContext();
            var books = db.Books.OrderByDescending(r=>r.Name);
            foreach (var t in books)
            {
                //string tolist = "Книга: " + t.Name + " Автор: " + t.Author;
                string tolist =  t.Name + " " + t.Author;
                listBox1.Items.Add(tolist);
            }
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox2.Text != "" && textBox3.Text != "")
            {
                if (path != null)
                {
                    string ff = bookFromBD.Phrase;
                    Antivirus.Docs d = new Docs(path, format, ff);

                    string data = null;
                    d.Normalization(ref data);
                    Application.DoEvents();
                    string razdel = bookFromBD.Razdel;

                    List<string> nums = new List<string>();
                    nums = d.SplitByRazdel(data, razdel);
                    //d.SplitAndSave(data);  для взятия разделителей
                    Application.DoEvents();
                    uint[] hashes = d.HashDjenkins(nums);
                    string[] prehashesFormBD = bookFromBD.Hashes.Split(new char[] { ' ' });
                    string[] hashesFormBD = prehashesFormBD.Take(prehashesFormBD.Length - 1).ToArray();
                    Application.DoEvents();
                    double counter = 0;
                    for (int i = 0; i < hashesFormBD.Length; i++)
                    {
                        if (hashes[i].ToString() == hashesFormBD[i]) counter++;
                    }
                    double result = counter / Convert.ToDouble(hashes.Length);
                    textBox1.Text = "Вероятность: " + (result*100) + '%';

                /*string H = null;
                foreach(var t in hashes)
                 {
                     H += t.ToString();
                     H = H + ' ';
                 }
                 textBox1.Text = H;

                 SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                 saveFileDialog1.DefaultExt = ".txt";
                 if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                     return;
                 if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                 {
                     using (var fsWrite = File.Open(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write))
                     {

                     }
                 }
                 System.IO.File.WriteAllText(saveFileDialog1.FileName, textBox1.Text);*/
                }
            }
            else
            {
                MessageBox.Show("Выберите оба экземпляра книг!");
            }
            

        }

        private void button2_Click(object sender, EventArgs e) // открыть
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Text files(*.PDF, *.EPUB, *.FB2, *.TXT)| *.pdf;*.epub;*.fb2;*.txt;";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                path = dialog.FileName;
                FileInfo file = new FileInfo(path);
                string f = file.Extension;
                if (f == ".pdf") format = 0;
                if (f == ".epub") format = 1;
                if (f == ".fb2") format = 2;
                if (f == ".txt") format = 3;

                textBox3.Text = "Книга для сравнения: " + file.Name;
            }
        }

        private void button3_Click(object sender, EventArgs e) // выбрать
        {
            BookContext db = new BookContext();
            var books = db.Books.OrderByDescending(r => r.Name);
            string[] vibor = listBox1.SelectedItem.ToString().Split(new char[] { ' ' });
            int dlina = vibor.Length;
            dlina -= 2;
            string stroka = null;
            for (int i = 0; i < dlina; i++)
            {
                if (i == 0)
                    stroka = vibor[i];
                else
                    stroka += ' ' + vibor[i];
            }
           
            if(stroka != null)
            {
                foreach(var t in books)
                {
                    if (stroka == t.Name) bookFromBD = t;
                }
                textBox2.Text = "Книга из БД: " + bookFromBD.Name;
            }
            else
            {
                MessageBox.Show("Выберите книгу с которой надо сравнивать!");
            }
        }

        private void button5_Click(object sender, EventArgs e) // добавление
        {
            Form2 f = new Form2();
            f.Show();

        }
    }
}
