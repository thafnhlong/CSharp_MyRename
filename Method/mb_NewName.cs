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
    public class mb_NewName : methodBase
    {
        private TextBox tbdata = new TextBox { Left = 11, Top = 24 };
        private static int num;

        public mb_NewName()
        {
            num = 0;
            Text = "New Name";

            MyAddControl(new Label { Text = "New Name:", Left = 8, Top = 8, AutoSize = true });

            tbdata.TextChanged += CheckedChangedCall;
            MyAddControl(tbdata);

            AddLinkLabel("<Inc Nr> - Incrementing numbers", "<Inc Nr:1>");
            AddLinkLabel("<Inc NrDir> Incrementing numbers per dir- ", "<Inc NrDir:1>");
            AddLinkLabel("<Inc Alpha> - Incrementing letters", "<Inc Alpha:A>");
            AddLinkLabel("<Name> - File name without extension", "<Name>");
            AddLinkLabel("<Ext> - Extension", "<Ext>");
            AddLinkLabel("<DirName> - Name of the directory", "<DirName>");
            AddLinkLabel("<Num Files> - Total number of files in the dir", "<Num Files>");
            AddLinkLabel("<Num Dirs> - Total number of subdirs in the dir", "<Num Dirs>");
            AddLinkLabel("<Num Items> - Total number of items in the list", "<Num Items>");
            AddLinkLabel("<Word> - Indexed word of the file name", "<Word:1>");

            AddFooter(180);
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            tbdata.Width = Width - 21;
        }

        //LABEL
        private void AddLinkLabel(string text, string tag)
        {
            LinkLabel ll = new LinkLabel { Text = text, Tag = tag, Left = 8, AutoSize = true };
            ll.Top = 47 + (num++) * 15;
            ll.LinkClicked += Linkclicked_Event;
            MyAddControl(ll);
        }
        private void Linkclicked_Event(object sender, LinkLabelLinkClickedEventArgs e)
        {
            tbdata.Text += ((LinkLabel)sender).Tag.ToString();
        }

        //DATA
        public override XElement SaveData()
        {
            return new XElement("newname",
                new XElement("movefrom", tbdata.Text),

                new XElement("active", IsChecked),

                base.SaveData()
            );

        }
        public override void LoadData(XElement xE)
        {
            if (xE == null) return;
            var root = xE.Elements();
            tbdata.Text = root.ElementAt(0).Value;

            IsChecked = bool.Parse(root.ElementAt(1).Value);

            base.LoadData(root.ElementAt(2));
        }

    }
}
