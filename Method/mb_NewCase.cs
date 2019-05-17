using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Do_An_2.Method
{
    class mb_NewCase : methodBase
    {
        private static int num;
        private RadioButton ischoose;

        public mb_NewCase()
        {

            num = 0;
            Text = "New Case";

            AddRadioButton("Set lower case");
            AddRadioButton("Set upper case");
            AddRadioButton("Set lower case first letter");
            AddRadioButton("Set upper case first letter");
            AddRadioButton("Set lower case first letter in every word");
            AddRadioButton("Set upper case first letter in every word");
            AddRadioButton("Set pattern to lower case");
            AddRadioButton("Set pattern to upper case");
            AddRadioButton("Set inverted case");

            MyAddControl(new Label { Name = "lbpat", Text = "Pattern (RegExp):", Left = 8, Top = 180, AutoSize = true, Visible = false });

            TextBox pat = new TextBox { Name = "pat", Left = 104, Top = 178, Visible = false };
            pat.TextChanged += CheckedChangedCall;
            MyAddControl(pat);

            (Controls.Find("num0", true)[0] as RadioButton).Checked = true;

            AddFooter(180);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Controls.Find("pat", true)[0].Width = Width - 115;
        }

        //RADIOBUTTON
        private void AddRadioButton(string text)
        {
            RadioButton rb = new RadioButton { Name = string.Format($"num{num}"), Text = text, Tag = num, Left = 11, AutoSize = true, };
            rb.Top = 8 + (num++) * 18;
            rb.CheckedChanged += radiobuttonchecked;
            MyAddControl(rb);
        }
        private void radiobuttonchecked(object sender, EventArgs e)
        {
            var cx = sender as Control;

            if (!(cx as RadioButton).Checked) return;

            var numx = cx.Tag as int?;
            if (numx == 6 || numx == 7)
                Controls.Find("pat", true)[0].Visible = Controls.Find("lbpat", true)[0].Visible = true;
            else
                Controls.Find("pat", true)[0].Visible = Controls.Find("lbpat", true)[0].Visible = false;

            ischoose = cx as RadioButton;
            CheckedChangedCall(this, e);
        }

        protected override string converter(string lastname, ref bool ispatternerror)
        {
            switch (ischoose.Tag as int?)
            {
                case 0:
                    lastname = lastname.ToLower();
                    break;
                case 1:
                    lastname = lastname.ToUpper();
                    break;
                case 2:
                    lastname = lastname.First().ToString().ToLower() + lastname.Substring(1);
                    break;
                case 3:
                    lastname = lastname.First().ToString().ToUpper() + lastname.Substring(1);
                    break;
                case 4:
                    var listword = lastname.Split(new char[] { ' ' }, StringSplitOptions.None);
                    lastname = "";
                    foreach (var word in listword)
                    {
                        string newword = word.First().ToString().ToLower() + word.Substring(1);
                        lastname += string.Format($"{newword} ");
                    }
                    lastname = lastname.Remove(lastname.Length - 1);
                    break;
                case 5:
                    listword = lastname.Split(new char[] { ' ' }, StringSplitOptions.None);
                    lastname = "";
                    foreach (var word in listword)
                    {
                        string newword = word.First().ToString().ToUpper() + word.Substring(1);
                        lastname += string.Format($"{newword} ");
                    }
                    lastname = lastname.Remove(lastname.Length - 1);
                    break;
                case 6:
                    string regxstring = Controls.Find("pat", true)[0].Text;
                    try
                    {
                        Regex regx = new Regex(regxstring, RegexOptions.IgnoreCase);
                        lastname = regx.Replace(lastname, m => m.Value.ToLower());
                    }
                    catch (Exception) { ispatternerror = true; }
                    break;
                case 7:
                    regxstring = Controls.Find("pat", true)[0].Text;
                    try
                    {
                        Regex regx = new Regex(regxstring, RegexOptions.IgnoreCase);
                        lastname = regx.Replace(lastname, m => m.Value.ToUpper());
                    }
                    catch (Exception) { ispatternerror = true; }
                    break;
                case 8:
                    var listchar = lastname.ToCharArray();
                    lastname = "";
                    foreach (var charac in listchar)
                    {
                        lastname += char.IsLower(charac) ? char.ToUpper(charac) : char.ToLower(charac);
                    }
                    break;
                default:
                    break;
            }
            return lastname;
        }

        //DATA
        public override XElement SaveData()
        {
            return new XElement("newcase",
                new XElement("numactive", ischoose.Tag.ToString()),
                new XElement("regexp", Controls.Find("pat", true)[0].Text),

                new XElement("active", IsChecked),

                base.SaveData()
            );
        }
        public override void LoadData(XElement xE)
        {
            if (xE == null) return;
            var root = xE.Elements();
            (Controls.Find(string.Format($"num{root.ElementAt(0).Value}"), true)[0] as RadioButton).Checked = true;
            Controls.Find("pat", true)[0].Text = root.ElementAt(1).Value;

            IsChecked = bool.Parse(root.ElementAt(2).Value);

            base.LoadData(root.ElementAt(3));
        }
    }
}
