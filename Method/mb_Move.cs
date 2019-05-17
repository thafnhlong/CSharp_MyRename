using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Do_An_2.Method
{
    class mb_Move : methodBase
    {
        private static int num;
        public mb_Move()
        {
            num = 0;
            Text = "Move";

            AddGroup("Move from: ");
            AddGroup("Move count: ");
            AddGroup("Move to: ");

            CheckBox cb = new CheckBox { Name = "backwards", Text = "Backwards", AutoSize = true, Top = 79, Left = 11 };
            cb.CheckedChanged += CheckedChangedCall;
            MyAddControl(cb);

            AddFooter(79);
        }

        //GROUP
        void AddGroup(string text)
        {
            Label lb = new Label { Text = text, Left = 8, Top = 10 + (num) * 23, AutoSize = true };
            NumericUpDown nud = new NumericUpDown { Name = string.Format($"num{num}"), Left = 80, Top = 8 + (num++) * 23, Maximum = 1000, Width = 180 };
            nud.TextChanged += CheckedChangedCall;
            MyAddControl(lb);
            MyAddControl(nud);
        }
        
        protected override string converter(string lastname, ref bool ispatternerror)
        {
            decimal movefrom = (Controls.Find("num0", true)[0] as NumericUpDown).Value - 1;
            decimal movecount = (Controls.Find("num1", true)[0] as NumericUpDown).Value;
            decimal moveto = (Controls.Find("num2", true)[0] as NumericUpDown).Value - 1;

            if (movefrom == -1 || movecount == 0 || moveto == -1) return lastname;

            bool backward = (Controls.Find("backwards", true)[0] as CheckBox).Checked;

            lastname = backward ? Reverse(lastname) : lastname;

            if (movecount + movefrom > lastname.Length) return lastname;

            string dacat = lastname.Remove((int)movefrom, (int)movecount);
            string phancat = lastname.Substring((int)movefrom, (int)movecount);

            if (moveto > dacat.Length) moveto = dacat.Length;
            lastname = dacat.Insert((int)moveto, phancat);


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
            return new XElement("move",
                new XElement("movefrom", (Controls.Find("num0", true)[0] as NumericUpDown).Value.ToString()),
                new XElement("movecount", (Controls.Find("num1", true)[0] as NumericUpDown).Value.ToString()),
                new XElement("moveto", (Controls.Find("num2", true)[0] as NumericUpDown).Value.ToString()),
                new XElement("backwards", (Controls.Find("backwards", true)[0] as CheckBox).Checked),

                new XElement("active", IsChecked),

                base.SaveData()
            );

        }
        public override void LoadData(XElement xE)
        {
            if (xE == null) return;
            var root = xE.Elements();
            (Controls.Find("num0", true)[0] as NumericUpDown).Value = decimal.Parse(root.ElementAt(0).Value);
            (Controls.Find("num1", true)[0] as NumericUpDown).Value = decimal.Parse(root.ElementAt(1).Value);
            (Controls.Find("num2", true)[0] as NumericUpDown).Value = decimal.Parse(root.ElementAt(2).Value);
            (Controls.Find("backwards", true)[0] as CheckBox).Checked = bool.Parse(root.ElementAt(3).Value);

            IsChecked = bool.Parse(root.ElementAt(4).Value);

            base.LoadData(root.ElementAt(5));
        }
    }
}
