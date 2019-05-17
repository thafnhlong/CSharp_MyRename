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
    class mb_RemovePattern : methodBase
    {
        public mb_RemovePattern()
        {
            Text = "Remove pattern";

            MyAddControl(new Label { Name = "lbpat", Text = "Pattern (RegExp): ", Left = 8, Top = 8, AutoSize = true });

            TextBox pat = new TextBox { Name = "pat", Left = 104, Top = 6 };
            pat.TextChanged += CheckedChangedCall;
            MyAddControl(pat);

            AddFooter(8);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Controls.Find("pat", true)[0].Width = Width - 115;
        }

        protected override string converter(string lastname, ref bool ispatternerror)
        {
            string regxstring = Controls.Find("pat", true)[0].Text;
            try
            {
                Regex regx = new Regex(regxstring, RegexOptions.IgnoreCase);
                lastname = regx.Replace(lastname, m => "");
            }
            catch (Exception) { ispatternerror = true; }
            return lastname;
        }

        //DATA
        public override XElement SaveData()
        {
            return new XElement("removepattern",
                new XElement("regexp", Controls.Find("pat", true)[0].Text),

                new XElement("active", IsChecked),

                base.SaveData()
            );

        }
        public override void LoadData(XElement xE)
        {
            if (xE == null) return;
            var root = xE.Elements();
            Controls.Find("pat", true)[0].Text = root.ElementAt(0).Value;

            IsChecked = bool.Parse(root.ElementAt(1).Value);

            base.LoadData(root.ElementAt(2));
        }
    }
}
