using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Image_Steganography
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public class node
        {
            public node left;
            public node right;
            public string data;
        }
        public void sort(node[] tree, int l)
        {
            for (int i = 0; i < l - 1; i++)
            {
                for (int j = 0; j < l - i - 1; j++)
                {
                    int l1 = tree[j].data.Length;
                    int l2 = tree[j + 1].data.Length;
                    string s1 = "", s2 = "";
                    for (int p1 = l1 - 1; p1 >= 0; p1--)
                    {
                        if (tree[j].data[p1] >= '0' && tree[j].data[p1] <= '9')
                            s1 = tree[j].data[p1] + s1;
                        else
                            break;
                    }

                    for (int p1 = l2 - 1; p1 >= 0; p1--)
                    {
                        if (tree[j + 1].data[p1] >= '0' && tree[j + 1].data[p1] <= '9')
                            s2 = tree[j + 1].data[p1] + s2;
                        else
                            break;
                    }

                    int i1 = Int32.Parse(s1);
                    int i2 = Int32.Parse(s2);
                    if (i1 > i2)
                    {
                        node temp = tree[j];
                        tree[j] = tree[j + 1];
                        tree[j + 1] = temp;
                    }
                }
            }
        }


        string[] signs = new string[256];
        string[] codes = new string[256];
        int temp = 0;
        public void code(node root, char[] codec, int top)
        {

            if (root.left != null)
            {
                codec[top] = '0';
                code(root.left, codec, top + 1);
            }
            if (root.right != null)
            {
                codec[top] = '1';
                code(root.right, codec, top + 1);
            }
            if (root.left == null && root.right == null)
            {

                signs[temp] = "" + root.data[0];
                for (int i = 0; i < top; i++)
                    codes[temp] += codec[i];
                temp++;
            }
        }

        public void huffman(string s)
        {
            int l = s.Length;
            char[] arr = new char[256]; int[] freq = new int[256]; int x = 0;
            for (int i = 0; i < 256; i++)
            { freq[i] = 0; }

            for (int i = 0; i < l; i++)
            {
                char ch = s[i]; int test = 0;
                for (int j = 0; j < x; j++)
                {
                    if (arr[j] == ch)
                    {
                        test = 1; freq[j]++; break;
                    }
                }
                if (test == 0)
                { arr[x] = ch; freq[x++] = 1; }
            }
            
            node[] tree = new node[x];
            for (int i = 0; i < x; i++)
            {
                tree[i] = new node();
                tree[i].data = "" + arr[i] + " " + freq[i];
                tree[i].left = null; tree[i].right = null;
            }
            while (x > 1)
            {
                sort(tree, x);
                node test = new node();
                test.left = tree[0]; test.right = tree[1];
                int l1 = tree[0].data.Length;
                int l2 = tree[1].data.Length;
                string s1 = "", s2 = "";
                for (int p1 = l1 - 1; p1 >= 0; p1--)
                {
                    if (tree[0].data[p1] >= '0' && tree[0].data[p1] <= '9')
                        s1 = tree[0].data[p1] + s1;
                    else
                        break;
                }
                for (int p1 = l2 - 1; p1 >= 0; p1--)
                {
                    if (tree[1].data[p1] >= '0' && tree[1].data[p1] <= '9')
                        s2 = tree[1].data[p1] + s2;
                    else
                        break;
                }
                int i1 = Int32.Parse(s1);
                int i2 = Int32.Parse(s2);
                int sum = i1 + i2;
                test.data = "" + sum;
                tree[0] = test;
                tree[1] = tree[x - 1]; x--;
            }
            node root = tree[0];

            char[] codec = new char[256];
            int top = 0;
            code(root, codec, top);

            System.IO.File.WriteAllLines(@"C:\Users\Triloki\Desktop\signs.txt", signs);
            System.IO.File.WriteAllLines(@"C:\Users\Triloki\Desktop\codes.txt", codes);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string s = textBox2.Text;
            int length = s.Length;
            huffman(s);
            Bitmap img = new Bitmap(textBox1.Text);
            int count = 0;
            
            string huffcode = "";
            for(int i=0;i<length;i++)
            {
                string t1 = "" + s[i];
                for(int j=0;j<temp;j++)
                {
                    if (t1 == signs[j])
                    { huffcode = huffcode + codes[j]; break; }
                }
            }
            int codelength = huffcode.Length;
            Color pixel = img.GetPixel(img.Width - 1, img.Height - 1);
            img.SetPixel(img.Width - 1, img.Height - 1, Color.FromArgb(pixel.R, pixel.G, length));
            int finish = 0;
            for(int i=0;i<img.Height;i++)
            {
                for(int j=0;j<img.Width;j++)
                {
                    Color pixel1 = img.GetPixel(i, j);

                    int x1 = pixel1.R; int x2 = pixel1.G; int x3 = pixel1.B;

                    string check = "";int m;

                    if (count==codelength)
                    {finish = 1;break;}

                    check = "" + huffcode[count++];
                    m = Int32.Parse(check);

                    x1 = x1 >> 1; x1 = x1 << 1; x1 = x1 | m;

                    img.SetPixel(i, j, Color.FromArgb(x1, x2, x3));

                    if (count == codelength)
                    { finish = 1; break; }

                    check = "" + huffcode[count++];
                    m = Int32.Parse(check);

                    x2 = x2 >> 1; x2 = x2 << 1; x2 = x2 | m;

                    img.SetPixel(i, j, Color.FromArgb(x1, x2, x3));

                    if (count == codelength)
                    { finish = 1; break; }

                    check = "" + huffcode[count++];
                    m = Int32.Parse(check);

                    x3 = x3 >> 1; x3 = x3 << 1; x3 = x3 | m;

                    img.SetPixel(i, j, Color.FromArgb(x1, x2, x3));
                }
                if (finish == 1)
                    break;
            }
            
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Image Files (*.png, *.bmp) | *.png; *.bmp";
            save.InitialDirectory = @"C:\Users\Triloki\Desktop";

            if (save.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = save.FileName.ToString();
                pictureBox1.ImageLocation = textBox1.Text;
                img.Save(textBox1.Text);
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files (*.png, *.bmp) | *.png; *.bmp";
            open.InitialDirectory = @"C:\Users\Triloki\Desktop";
            if (open.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = open.FileName.ToString();
                pictureBox1.ImageLocation = textBox1.Text;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string[] signtext = System.IO.File.ReadAllLines(@"C:\Users\Triloki\Desktop\signs.txt");

            string[] codetext = System.IO.File.ReadAllLines(@"C:\Users\Triloki\Desktop\codes.txt");

            string[] finalcodes = new string[256];
            string[] finalsigns = new string[256];
            int cur = 0;
            for(int i=0;i<codetext.Length;i++)
            {
                if (codetext[i] == "")
                    break;
                else
                {
                    finalsigns[cur] = signtext[i];
                    finalcodes[cur] = codetext[i];
                    cur++;
                }
            }

            Bitmap img = new Bitmap(textBox1.Text);
            Color pixel = img.GetPixel(img.Width - 1, img.Height - 1);
            int length = pixel.B;
            int word = 0;string secret = "";
            string test = ""; int finish = 0;
            for(int i=0;i<img.Height;i++)
            {
                for(int j=0;j<img.Width;j++)
                {

                    Color pixel1 = img.GetPixel(i, j);
                    
                    int x1 = pixel1.R%2; int x2 = pixel1.G%2; int x3 = pixel1.B%2;
                    
                    test = test + x1;
                    for(int k=0;k<cur;k++)
                    {
                        if(finalcodes[k]==test)
                        {
                            secret = secret + finalsigns[k]; 
                            test = ""; word++;  break;
                        }
                    }
                    if(word==length)
                    {
                        finish = 1; break;
                    }
                    test = test + x2; 
                    for (int k = 0; k < cur; k++)
                    {
                        if (finalcodes[k] == test)
                        {
                            secret = secret + finalsigns[k];
                            test = ""; word++; break;
                        }
                    }
                    if (word == length)
                    {
                        finish = 1; break;
                    }
                    test = test + x3;
                    for (int k = 0; k < cur; k++)
                    {
                        if (finalcodes[k] == test)
                        {
                            secret = secret + finalsigns[k];
                            test = ""; word++; break;
                        }
                    }
                    if (word == length)
                    {
                        finish = 1; break;
                    }
                }
                if (finish == 1)
                    break;
            }
            textBox2.Text = secret;
        }
    }
}
