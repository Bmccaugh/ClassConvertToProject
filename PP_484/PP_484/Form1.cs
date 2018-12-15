using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using System.IO;
namespace PP_484
{
    public partial class Form1 : Form
    {
        List<Item> items = new List<Item>();
        public Form1()
        {
            InitializeComponent();
        }
        public class Item
        {
            public String name = "";
            public List<string> operations = new List<string>();
            public List<string> attributes = new List<string>();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            List<Item> items = new List<Item>(); 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Stream myStream;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            if(openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if ((myStream = openFileDialog1.OpenFile()) != null)
                {
                    string strf = openFileDialog1.FileName;
                    //XmlDocument doc = new XmlDocument();
                    //doc.Load(strf);
                    string filetext = File.ReadAllText(strf);
                    JConvert(filetext);
                    var i = JSave(items[0]);
                    TBox.Text = i;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            //sfd.Filter = "Text File|*.txt";
            sfd.Filter = "All Files|*.*";
            sfd.DefaultExt = "cs";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (Stream s = File.Open(sfd.FileName, FileMode.CreateNew))
                using (StreamWriter sw = new StreamWriter(s))
                {
                    sw.Write(TBox.Text);
                }
            }
        }
        private void JConvert(string str)
        {
            string s = "";
            Item tempItem = new Item();
            var type = str.GetType();
            var json = JObject.Parse(str);
            type = json.GetType();
            if (json.SelectToken("_type").ToString() == "Project")
            {
                var temp = JArray.Parse(json.SelectToken("ownedElements").ToString())[0];
                s = temp.ToString();
                var temp2 = JObject.Parse(s);
                if (temp2.SelectToken("_type").ToString() == "UMLModel")
                {
                    var temp3 = JArray.Parse(temp2.SelectToken("ownedElements").ToString());

                    s = temp3.ToString();
                    foreach (var n in temp3)
                    {
                        var t = n.ToString();
                        temp2 = JObject.Parse(t);
                        if (temp2.SelectToken("_type").ToString() == "UMLClass")
                        {
                            var tmp = JObject.Parse(temp2.ToString());
                            foreach (var y in tmp)
                            {
                                if (y.Key == "name")
                                {
                                    tempItem.name = y.Value.ToString();
                                }

                                else if (y.Key == "operations")
                                {
                                    var u = JArray.Parse(y.Value.ToString());
                                    foreach (var tmp2 in u)
                                    {
                                        s = tmp2.ToString();
                                        var tmp3 = JObject.Parse(s);
                                        tempItem.operations.Add(tmp3.SelectToken("name").ToString());
                                    }
                                }

                                else if (y.Key == "attributes")
                                {
                                    var u = JArray.Parse(y.Value.ToString());
                                    foreach (var tmp2 in u)
                                    {
                                        s = tmp2.ToString();
                                        var tmp3 = JObject.Parse(s);
                                        tempItem.attributes.Add(tmp3.SelectToken("name").ToString());
                                    }
                                } //*/
                            }
                            s = temp2.ToString();
                            items.Add(tempItem);
                            tempItem = new Item();
                        }
                    }
                }//*/
            }
            //return s;
        }
        private string JSave(Item it)
        {
            string s = "";

            s = "using System; \nusing System.Collections.Generic;\n";
            s += "using System.Linq; \n";
            s += "using System.Text; \nusing System.Threading.Tasks; \n \n \n";//*/
            s += "namespace Temporary \n{ \n \tclass ";
            s += it.name;
            s += "\n\t{\n";
            foreach (var f in it.attributes)
            {
                string f2;
                s += "\t\t";
                if (f.Substring(0, 3) == "int" || f.Substring(0, 3) == "Int")
                {
                    s += "int ";
                    f2 = f.TrimStart('i', 'I', 'n', 't', 'e', 'g', 'e', 'r');
                    s += f2;
                }
                else if (f.Substring(0, 3) == "str" || f.Substring(0, 3) == "Str")
                {
                    s += "string ";
                    f2 = f.TrimStart('s', 'S', 't', 'r', 'i', 'n', 'g');
                    s += f2;
                }
                else if (f.Substring(0, 3) == "dou" || f.Substring(0, 3) == "Dou" || f.Substring(0, 3) == "dbl")
                {
                    s += "double ";
                    f2 = f.TrimStart('d', 'D', 'o', 'u', 'b', 'l', 'e');
                    s += f2;
                }
                else
                {
                    s += "var ";
                    s += f;
                }
                s += ";\n";
            }
            s += "\n";
            foreach (var f in it.operations)
            {
                string f2;
                s += "\t\tprivate ";
                if (f.Substring(0, 3) == "int" || f.Substring(0, 3) == "Int")
                {
                    s += "int ";
                    f2 = f.TrimStart('i', 'I', 'n', 't', 'e', 'g', 'e', 'r');
                    s += f2;
                }
                else
                {
                    s += "void ";
                    s += f;
                    s += "() \n\t\t" +
                        "{\n\t\t\t\n" +
                        "\t\t}\n";
                }
            }
            s += "\t}\n}";

            return s;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(searchBox.Text != null && searchBox.Text.Length >= 3)
            {
                foreach (var v in items)
                {
                    if(v.name == searchBox.Text)
                    {
                        TBox.Text = JSave(v);
                    }
                }
            }
            else
            {
                MessageBox.Show("Class Not Fount");
                searchBox.Text = string.Empty;
                searchBox.Focus();

            }
        }
    }
}
