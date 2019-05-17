using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Do_An_2.Utils;
using Do_An_2.Model;

namespace Do_An_2.Dialog
{
    public partial class frmFileDialog : Form
    {
        ioInfo curIO;

        public event Action<List<fileInfo>> pickdone;

        public frmFileDialog()
        {
            InitializeComponent();

            var imgL = new ImageList();
            imgL.ImageSize = new Size(32, 32);
            imgL.ColorDepth = ColorDepth.Depth32Bit;
            imgL.Images.Add("Computer", myShell.GetComputerIcon(false));
            imgL.Images.Add("Sys", myShell.GetFileIcon(@"C:\", false));
            imgL.Images.Add("Fixed", myShell.GetFileIcon(@"D:\", false));
            imgL.Images.Add("CDRom", myShell.GetFileIcon(@"Z:\", false));
            imgL.Images.Add("Folder", myShell.GetFolderIcon(false));

            lv.LargeImageList = imgL;

            StartPosition = FormStartPosition.CenterScreen;

            lv.MouseDoubleClick += Lv_MouseDoubleClick;
            tsbtnBack.Click += TsbtnBack_Click;
            btnCancel.Click += BtnCancel_Click;
            btnOpen.Click += BtnOpen_Click;

            loadlistView(null);
        }
        //BUTTON
        private void BtnOpen_Click(object sender, EventArgs e)
        {
            List<fileInfo> listfilechoose = new List<fileInfo>();

            foreach (ListViewItem lvi in lv.SelectedItems)
            {
                if (lvi.Tag is folderInfo || lvi.Tag is driveInfo)
                {
                    loadlistView(lvi.Tag as ioInfo);
                    return;
                }
                listfilechoose.Add(lvi.Tag as fileInfo);
            }
            pickdone?.Invoke(listfilechoose);

            DialogResult = DialogResult.OK;
            Close();
        }
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void TsbtnBack_Click(object sender, EventArgs e)
        {
            loadlistView(curIO?.Parent);
        }
        //LIST
        void loadlistView(ioInfo cur)
        {
            tstAddress.Text = "";
            lv.Items.Clear();
            if (cur == null)
            {
                tsbtnBack.Enabled = false;
                var ld = xmlUtils.Drives();
                foreach (driveInfo di in ld)
                {
                    lv.Items.Add(new ListViewItem
                    {
                        Text = string.Format("{0} ({1})", di.VolumeLabel, di.Name.Replace(@"\", "")),
                        Tag = di,
                        ImageKey = di.Type
                    });
                }
            }
            else
            {
                tstAddress.Text = xmlUtils.Getlocation(cur, true);

                tsbtnBack.Enabled = true;
                curIO = cur;
                var lfo = xmlUtils.Folders(cur);
                foreach (folderInfo foi in lfo)
                {
                    lv.Items.Add(new ListViewItem
                    {
                        Text = foi.Name,
                        Tag = foi,
                        ImageKey = "Folder"
                    });
                }

                var lfi = xmlUtils.Files(cur);
                foreach (fileInfo fi in lfi)
                {
                    if (!lv.LargeImageList.Images.ContainsKey(fi.Ext))
                    {
                        lv.LargeImageList.Images.Add(fi.Ext, myShell.GetFileIcon(string.Format(".{0}", fi.Ext), false));
                    }
                    lv.Items.Add(new ListViewItem
                    {
                        Text = string.Format($"{fi.Name}.{fi.Ext}"),
                        Tag = fi,
                        ImageKey = fi.Ext
                    });
                }
            }
        }
        private void Lv_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var lvi = lv.HitTest(e.X, e.Y).Item;
            if (lvi == null)
                return;
            var ioi = lvi.Tag as ioInfo;
            if (ioi is fileInfo)
                return;
            loadlistView(ioi);
        }
    }
}
