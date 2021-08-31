using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using eBdb.EpubReader;
using iTextSharp;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace Antivirus
{
    class Docs
    {
        string path = null;
        string text = null;
        string first = null;
        string pretext = null;
        int start = 0;

        public Docs(string p, int format, string fir)
        {
            path = p;
            first = fir;
         
            if(path != null)
            {
                try
                {
                    switch(format)
                    {
                        //pdf
                        case 0:
                            PdfReader reader = new PdfReader(path);
                            for (int countPdfPage = 1; countPdfPage <= reader.NumberOfPages; countPdfPage++)
                            {
                                string s = PdfTextExtractor.GetTextFromPage(reader, countPdfPage, new LocationTextExtractionStrategy());
                                pretext += s.Replace(" ", string.Empty).Replace('\t', ' ');

                            }
                            Application.DoEvents();
                            start = pretext.IndexOf(first);
                            text = pretext.Substring(start);
                            break;

                        //epub
                        case 1:
                            Epub epub = new Epub(path);
                            pretext = epub.GetContentAsPlainText(); Application.DoEvents();
                            start = pretext.IndexOf(first);
                            text = pretext.Substring(start); 
                            break;

                        //fb2
                        case 2:
                            XElement el = XElement.Load(path); Application.DoEvents();
                            pretext = el.Value;
                            start = pretext.IndexOf(first);
                            text = pretext.Substring(start);


                                break;

                        //txt
                        case 3:
                            using (StreamReader sr = new StreamReader(path, Encoding.Default))
                            {
                                pretext = sr.ReadToEnd(); Application.DoEvents();
                                start = pretext.IndexOf(first);
                                text = pretext.Substring(start);
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public void Normalization(ref string result)
        {
            
            text = text.Replace("\t", " ");
            text = text.Replace("\n", " ");
            text = text.Replace("\r", " ");
            text = text.Replace(",", " ");
            text = text.Replace(";", " ");
            text = text.Replace("\"", " ");
            text = text.Replace("ъ", "");
            text = text.Replace("ќ", "");
            text = text.Replace("–", " ");
            text = text.Replace("«", "");
            text = text.Replace(".", " ");
            text = text.Replace("»", " ");
            text = text.Replace("’", " ");
            text = text.Replace("[", " ");
            text = text.Replace("]", " ");
            text = text.Replace("ь", ""); 
            text = text.Replace(":", " ");
            text = text.Replace("&", " ");
            text = text.Replace("!", " ");
            text = text.Replace("?", " ");
            text = text.Replace("...", " ");
            text = text.Replace("-", " ");
            text = text.Replace("(", "");
            text = text.Replace(")", "");
            text = text.Replace("…", " ");
            text = text.Replace(@"\", "");
            text = text.Replace("й", "и");
            text = text.Replace("ё", "е");
            text = text.Replace("*", "");
            text = text.Replace("***", "");
            text = Regex.Replace(text, "[1234567890]", "");


          
            string[] words = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                string word = null;
                if(words[i].Length > 6)
                {
                    string t = words[i].ToLower();
                    string tt = Regex.Replace(t, "[аеиоуыэюяьЪ]", "");
                    tt = Regex.Replace(tt, "[qwertyuiopasdfghjklzxcvbnm]", "");
                    word = tt;
                    char[] ar = word.ToCharArray();
                    for (int j = 0; j < ar.Length; j++)
                    {
                        char c = ar[j];
                        if (j > 5)
                        {
                            ar[j] = '8';
                        }
                        else
                        {
                            if (ar[j] == 'б' || ar[j] == 'п' || ar[j] == 'ф' || ar[j] == 'в') ar[j] = '1';
                            if (ar[j] == 'с' || ar[j] == 'ц' || ar[j] == 'з' || ar[j] == 'к' || ar[j] == 'г' || ar[j] == 'х') ar[j] = '2';
                            if (ar[j] == 'т' || ar[j] == 'д') ar[j] = '3';
                            if (ar[j] == 'л') ar[j] = '4';
                            if (ar[j] == 'м' || ar[j] == 'н') ar[j] = '5';
                            if (ar[j] == 'р') ar[j] = '6';
                            if (ar[j] == 'ж' || ar[j] == 'ш' || ar[j] == 'щ' || ar[j] == 'ч') ar[j] = '7';
                        }
                    }
                    word = new string(ar);
                    result = result + word;
                    result = result.Replace(" ", "");
                }
                
            }
            
             
        }

        public uint[] HashDjenkins(List<string> nums)
        {
            uint[] hashes = new uint[nums.Count];
            int i = 0;
            foreach(var t in nums)
            {
                byte[] data = Encoding.Default.GetBytes(t);
                uint hash = 0;
                for (int j = 0; j < data.Length; j++)
                {
                    hash += data[j];
                    hash += hash << 10;
                    hash ^= hash >> 6;
                }
                hash += hash << 3;
                hash ^= hash >> 11;
                hash += hash << 15;
                hashes[i] = hash;
                i++;
            }
            return hashes;
        }

        public string SplitAndSave(string data)
        {
            int blockLength = 300;

            List<string> nums = new List<string>(data.Length / blockLength + 1);
            for (int i = 0; i < data.Length; i += blockLength)
            {
                nums.Add(data.Substring(i, data.Length - i > blockLength ? blockLength : data.Length - i));
            }

            string razdel = null;
            foreach(var t in nums)
            {
                razdel += ' ' + t.Substring(t.Length - 10);
            }
            return razdel;


        }

        public List<string> SplitByRazdel(string data, string razdel)
        {
            string[] razdeliteli = razdel.Split(new char[] { ' ' });
            List<string> result = new List<string>(razdeliteli.Length + 1);
            int curIndex = 0;
            foreach(var t in razdeliteli)
            {
                int indexEndFragPlus = 0;
                indexEndFragPlus = data.IndexOf(t);
                if (indexEndFragPlus > 0)
                {
                    if((indexEndFragPlus - curIndex) < 300)
                    {
                        string frag = data.Substring(curIndex, 296 > (data.Length - curIndex) ? (data.Length - curIndex) : 296);
                        curIndex = indexEndFragPlus + 10;
                        result.Add(frag);
                    }
                    else if((indexEndFragPlus - curIndex) < 600)
                    {
                        string frag = data.Substring(curIndex, 296 > (data.Length - curIndex) ? (data.Length - curIndex) : 296);
                        string frag2 = data.Substring(curIndex + 296, 296 > (data.Length - curIndex + 296) ? (data.Length - curIndex + 296) : 296);
                        curIndex = indexEndFragPlus + 10;
                        result.Add(frag);
                        result.Add(frag2);
                    }
                    else
                    {
                        string frag = data.Substring(curIndex, 296 > (data.Length - curIndex) ? (data.Length - curIndex) : 296);
                        string frag2 = data.Substring(curIndex + 296, 296 > (data.Length - curIndex + 296) ? (data.Length - curIndex + 296) : 296);
                        string frag3 = data.Substring(curIndex + 592, 296 > (data.Length - curIndex + 592) ? (data.Length - curIndex + 592) : 296);
                        curIndex = indexEndFragPlus + 10;
                        result.Add(frag);
                        result.Add(frag2);
                        result.Add(frag3);
                    }
                }
            }

            return result;
        }
    }
}
