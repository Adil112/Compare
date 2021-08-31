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

namespace Antivirus
{
    public partial class Form2 : Form
    {
        string path;
        int format;

        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) // выбор
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
            }
        }

        private void button2_Click(object sender, EventArgs e) // добавление
        {
            Antivirus.Docs d = new Docs(path, format, textBox3.Text);

            string data = null;
            d.Normalization(ref data);


            List<string> nums = new List<string>();
            string razdel = d.SplitAndSave(data); // разделители

            nums = d.SplitByRazdel(data, razdel); // деление
              

            uint[] hashes = d.HashDjenkins(nums);


            Book newBook = new Book();
            BookContext db = new BookContext();

            newBook.Name = textBox1.Text;
            newBook.Author = textBox2.Text;
            newBook.Phrase = textBox3.Text;

            newBook.Razdel = razdel;

            string hashto = null;
            for(int i =0; i < hashes.Length; i++)
            {
                if (i != 0)
                hashto = hashto + ' ' + hashes[i];
                else hashto = hashes[i].ToString();

            }
            newBook.Hashes = hashto;
            newBook.IdBook = Guid.NewGuid();

            db.Books.Add(newBook);
            db.SaveChanges();
            MessageBox.Show("Сохранилось");
        }
    }
}
