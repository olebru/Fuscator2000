using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fuscator2000
{
    public partial class Fuscator2000 : Form
    {
        public Fuscator2000()
        {
            InitializeComponent();
        }

        private  void button1_Click(object sender, EventArgs e)
        {
            var o = new JCLib();
            label1.Text = "Fuscating";
            DateTime start = DateTime.Now;
            textBox2.Text = o.Fuscate(textBox1.Text);
            label1.Text = (start - DateTime.Now).Milliseconds.ToString()+"ms";
            label1.Text = label1.Text.Replace("-", "");
            label1.Text = label1.Text + "\n" + textBox2.TextLength.ToString();
         


        }

        private void button2_Click(object sender, EventArgs e)
        {
            var o = new JCLib();
            if (textBox1.Text.Substring(0,1) == "z" | textBox1.Text.Substring(0,1)== "x")
            {
                  label1.Text = "Defuscating";
            DateTime start = DateTime.Now;
            textBox2.Text = o.Defuscate(textBox1.Text);
            label1.Text = (start - DateTime.Now).Milliseconds.ToString() + "ms";
            label1.Text = label1.Text.Replace("-", "");
            }
            else
            {
                MessageBox.Show("Invalid fuscated text");
            }
          
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";

        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox2.Text;
            textBox2.Text = "";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Clipboard.SetText(textBox2.Text);
        }
    }

    class JCLib
    {

        public string Defuscate(string Text)
        {

            string output = "";

            for (int i = 0; i < Text.Length - 1; i++)
            {
                if (Text.Substring(i, 1) == "z") // 0
                {
                    int digits = 0;
                    while ((int)Text.Substring(i + digits + 1, 1).ToCharArray()[0] < 60)
                    {
                        digits++;
                        if (i + digits + 1 == Text.Length)
                        {
                            break;
                        }

                    }
                    int bits = int.Parse(Text.Substring(i + 1, digits));
                    string bitgroup = "";
                    for (int j = 0; j < bits; j++)
                    {
                        bitgroup += "0";
                    }
                    output += bitgroup;

                }
                if (Text.Substring(i, 1) == "x") // 1
                {
                    int digits = 0;
                    while ((int)Text.Substring(i + digits + 1, 1).ToCharArray()[0] < 60)
                    {
                        digits++;
                        if (i + digits + 1 == Text.Length)
                        {
                            break;
                        }
                    }
                    int bits = int.Parse(Text.Substring(i + 1, digits));
                    string bitgroup = "";
                    for (int j = 0; j < bits; j++)
                    {
                        bitgroup += "1";
                    }
                    output += bitgroup;
                }

            }

            string binnum0s = output.Substring(output.Length - 64, 32);
            string binnum1s = output.Substring(output.Length - 32, 32);
            int num0s = Convert.ToInt32(binnum0s, 2);
            int num1s = Convert.ToInt32(binnum1s, 2);

            string instr = output.Substring(0, output.Length - 64);
            List<string> instrlist = new List<string>();
            for (int i = 0; i < instr.Length; i += 32)
            {
                instrlist.Add(instr.Substring(i, 32));
            }

            string[] realbits = new string[num0s + num1s + 64];

            for (int i = 0; i < realbits.Length; i++)
            {
                realbits[i] = "0";
            }
            foreach (var item in instrlist)
            {
                int pos = Convert.ToInt32(item, 2);
                realbits[pos] = "1";
            }

            output = "";

            for (int i = 0; i < realbits.Length - 64; i = i + 8)
            {
                string binasc = "";
                for (int j = i; j < i + 8; j++)
                {
                    binasc = binasc + realbits[j];
                }
                output = output + ((char)Convert.ToInt32(binasc, 2)).ToString();

            }
            return output;
        }

        public string Fuscate(string Text)
        {
            bitlist bits = new bitlist();
            int letternumber = 0;
            foreach (char item in Text.ToCharArray())
            {
                int asciidec = (int)item;
                string asciibin = Convert.ToString(asciidec, 2);

                while (asciibin.Length < 8)
                {
                    asciibin = "0" + asciibin;
                }
                int bitnumber = 0;
                foreach (char bit in asciibin.ToCharArray())
                {
                 
                    if (bit.ToString() == "1")
                    {
                        bits.Add(new Bit("1", letternumber + bitnumber));
                        bitnumber++;
                        
                    }
                    else
                    {
                        bits.Add(new Bit("0", letternumber + bitnumber));
                        bitnumber++;
                    }


                }
                letternumber = letternumber + bitnumber;
            }

            string output = "";

            var ones = from bit in bits where bit.value == "1" select bit;
      
            foreach (var bit in ones.OrderBy(b => b.sortby))
            {
                output += bit.binPOS;

            }
         


            output += bits.binpostfix;

            output = output.Replace("0", "a");
            output = output.Replace("1", "b");

  

            for (int i = 32; i > 0; i--)
            {
                string maska = "";
                string maskb = "";
                for (int j = 0; j < i; j++)
                {
                    maska = maska + "a";
                    maskb = maskb + "b";
                }
                output = output.Replace(maska, "z" + i.ToString());
                output = output.Replace(maskb, "x" + i.ToString());

            }


            return output;

        }


        static class fubar
        {
            public const int bitlentgh = 32;

        }

        class Bit
        {
            public Bit(string Value)
            {

                this._value = Value;
                this.sortby = Bit.rnd.Next(int.MaxValue);
            }
            public Bit(string Value, int Pos)
            {

                this._value = Value;
                this.sortby = Bit.rnd.Next(int.MaxValue);
                this.Pos = Pos;
            }

            public int sortby { get; set; }
            private string _value;
            public string value
            {
                get
                {
                    return this._value;

                }
            }


            public string binPOS
            {
                get
                {
                    string _binPOS = Convert.ToString(this.Pos, 2);
                    while (_binPOS.Length < fubar.bitlentgh)
                    {
                        _binPOS = "0" + _binPOS;

                    }
                    return _binPOS;

                }
            }

            private static Random rnd = new Random();
            public int Pos { get; set; }
        }

        class bitlist : List<Bit>
        {

            public int num0s
            {
                get
                {
                    return this.Count(b => b.value == "0");
                }
            }
            public int num1s
            {
                get
                {
                    return this.Count(b => b.value == "1");
                }
            }

            public string binpostfix
            {
                get
                {
                    string _binPostFix0s = Convert.ToString(this.num0s, 2);
                    while (_binPostFix0s.Length < fubar.bitlentgh)
                    {
                        _binPostFix0s = "0" + _binPostFix0s;

                    }
                    string _binPostFix1s = Convert.ToString(this.num1s, 2);
                    while (_binPostFix1s.Length < fubar.bitlentgh)
                    {
                        _binPostFix1s = "0" + _binPostFix1s;

                    }

                    return _binPostFix0s + _binPostFix1s;

                }
            }

        }


    }
}
