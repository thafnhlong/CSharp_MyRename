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
    class mb_Replace : methodBase
    {
        private static int num;
        public mb_Replace()
        {
            num = 0;
            Text = "Replace";

            AddGroup("Regex: ");
            AddGroup("Replace with: ");

            AddFooter(35);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Controls.Find("num1", true)[0].Width = Controls.Find("num0", true)[0].Width = Width - 105;
        }

        //GROUP
        void AddGroup(string text)
        {
            Label lb = new Label { Text = text, Left = 8, Top = 10 + (num) * 23, AutoSize = true };
            TextBox tb = new TextBox { Name = string.Format($"num{num}"), Left = 95, Top = 8 + (num++) * 23 };

            tb.TextChanged += CheckedChangedCall;
            MyAddControl(lb);
            MyAddControl(tb);
        }

        protected override string converter(string lastname, ref bool ispatternerror)
        {
            string regxstring = Controls.Find("num0", true)[0].Text;
            string replacestring = Controls.Find("num1", true)[0].Text;
            try
            {
                Regex regx = new Regex(regxstring, RegexOptions.IgnoreCase);
                lastname = regx.Replace(lastname, replacestring);
            }
            catch (Exception) { ispatternerror = true; }
            return lastname;
        }

        //DATA
        public override XElement SaveData()
        {
            return new XElement("replace",
                new XElement("regexp", Controls.Find("num0", true)[0].Text),
                new XElement("replaceby", Controls.Find("num1", true)[0].Text),

                new XElement("active", IsChecked),

                base.SaveData()
            );
        }
        public override void LoadData(XElement xE)
        {
            if (xE == null) return;
            var root = xE.Elements();
            Controls.Find("num0", true)[0].Text = root.ElementAt(0).Value;
            Controls.Find("num1", true)[0].Text = root.ElementAt(1).Value;

            IsChecked = bool.Parse(root.ElementAt(2).Value);

            base.LoadData(root.ElementAt(3));
        }
    }
}
