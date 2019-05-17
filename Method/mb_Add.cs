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
    class mb_Add : methodBase
    {
        private static int num;
        public mb_Add()
        {
            num = 0;
            Text = "Add";

            AddGroup("Text add: ");
            AddGroup("At index: ", true);

            CheckBox cb = new CheckBox { Name = "backwards", Text = "Backwards", AutoSize = true, Top = 56, Left = 11 };
            cb.CheckedChanged += CheckedChangedCall;
            MyAddControl(cb);

            AddFooter(56);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Controls.Find("num1", true)[0].Width = Controls.Find("num0", true)[0].Width = Width - 105;
        }

        //GROUP
        void AddGroup(string text, bool isnumeric = false)
        {
            Label lb = new Label { Text = text, Left = 8, Top = 10 + (num) * 23, AutoSize = true };

            Control c;
            if (isnumeric)
                c = new NumericUpDown { Name = string.Format($"num{num}"), Left = 95, Top = 8 + (num++) * 23, Maximum = 1000 };
            else
                c = new TextBox { Name = string.Format($"num{num}"), Left = 95, Top = 8 + (num++) * 23 };

            c.TextChanged += CheckedChangedCall;
            MyAddControl(lb);
            MyAddControl(c);
        }

        protected override string converter(string lastname, ref bool ispatternerror)
        {
            string textadd = Controls.Find("num0", true)[0].Text;
            decimal atindex = (Controls.Find("num1", true)[0] as NumericUpDown).Value - 1;

            if (atindex == -1) return lastname;

            bool backward = (Controls.Find("backwards", true)[0] as CheckBox).Checked;

            if (backward)
            {
                lastname = Reverse(lastname);
                textadd = Reverse(textadd);
            }

            if (atindex > lastname.Length) atindex = lastname.Length;

            lastname = lastname.Insert((int)atindex, textadd);

            return backward ? Reverse(lastname) : lastname;
        }

        private string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        //DATA
        public override XElement SaveData()
        {
            return new XElement("add",
                new XElement("textadd", Controls.Find("num0", true)[0].Text),
                new XElement("atindex", (Controls.Find("num1", true)[0] as NumericUpDown).Value.ToString()),
                new XElement("backwards", (Controls.Find("backwards", true)[0] as CheckBox).Checked),
                new XElement("active", IsChecked),

                base.SaveData()
            );

        }
        public override void LoadData(XElement xE)
        {
            if (xE == null) return;
            var root = xE.Elements();
            Controls.Find("num0", true)[0].Text = root.ElementAt(0).Value;
            (Controls.Find("num1", true)[0] as NumericUpDown).Value = decimal.Parse(root.ElementAt(1).Value);
            (Controls.Find("backwards", true)[0] as CheckBox).Checked = bool.Parse(root.ElementAt(2).Value);

            IsChecked = bool.Parse(root.ElementAt(3).Value);

            base.LoadData(root.ElementAt(4));
        }
    }
}
