using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Do_An_2.Method
{
    class mb_Remove : methodBase
    {
        private static int num;
        public mb_Remove()
        {
            num = 0;
            Text = "Remove";

            AddGroup("Remove count: ");
            AddGroup("Starting at: ");

            CheckBox cb = new CheckBox { Name = "backwards", Text = "Backwards", AutoSize = true, Top = 56, Left = 11 };
            cb.CheckedChanged += CheckedChangedCall;
            MyAddControl(cb);

            AddFooter(56);
        }

        //GROUP
        void AddGroup(string text)
        {
            Label lb = new Label { Text = text, Left = 8, Top = 10 + (num) * 23, AutoSize = true };
            NumericUpDown nud = new NumericUpDown { Name = string.Format($"num{num}"), Left = 95, Top = 8 + (num++) * 23, Maximum = 1000, Width = 170 };
            nud.TextChanged += CheckedChangedCall;
            MyAddControl(lb);
            MyAddControl(nud);
        }

        protected override string converter(string lastname, ref bool ispatternerror)
        {
            decimal removecount = (Controls.Find("num0", true)[0] as NumericUpDown).Value;
            decimal startingat = (Controls.Find("num1", true)[0] as NumericUpDown).Value - 1;

            if (removecount == 0 || startingat == -1 || startingat > lastname.Length) return lastname;

            bool backward = (Controls.Find("backwards", true)[0] as CheckBox).Checked;
            lastname = backward ? Reverse(lastname) : lastname;

            int soluongconlai = lastname.Length - (int)startingat;
            if (removecount > soluongconlai) removecount = soluongconlai;

            lastname = lastname.Remove((int)startingat, (int)removecount);

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
            return new XElement("remove",
                new XElement("removecount", (Controls.Find("num0", true)[0] as NumericUpDown).Value.ToString()),
                new XElement("startingat", (Controls.Find("num1", true)[0] as NumericUpDown).Value.ToString()),
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
            (Controls.Find("backwards", true)[0] as CheckBox).Checked = bool.Parse(root.ElementAt(2).Value);

            IsChecked = bool.Parse(root.ElementAt(3).Value);

            base.LoadData(root.ElementAt(4));
        }
    }
}
