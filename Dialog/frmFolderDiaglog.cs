using Do_An_2.Utils;
using Do_An_2.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Do_An_2.Dialog
{
    public partial class frmFolderDiaglog : Form
    {
        public event Action<List<ioInfo>> pickdone;

        private List<TreeNode> selectednode = new List<TreeNode>();

        private bool AddFile = true;

        public frmFolderDiaglog(bool isaddfolder)
        {
            InitializeComponent();

            if (isaddfolder)
                AddFile = label1.Visible = txtMask.Visible = cb.Visible = false;

            StartPosition = FormStartPosition.CenterScreen;

            btnOK.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;

            tv.DrawMode = TreeViewDrawMode.OwnerDrawText;
            tv.DrawNode += Tv_DrawNode;
            tv.MouseDown += Tv_MouseDown;

            var imgS = new ImageList();
            imgS.ImageSize = new Size(16, 16);
            imgS.ColorDepth = ColorDepth.Depth32Bit;
            imgS.Images.Add("Computer", myShell.GetComputerIcon(true));
            imgS.Images.Add("Sys", myShell.GetFileIcon(@"C:\", true));
            imgS.Images.Add("Fixed", myShell.GetFileIcon(@"D:\", true));
            imgS.Images.Add("CDRom", myShell.GetFileIcon(@"Z:\", true));
            imgS.Images.Add("Folder", myShell.GetFolderIcon(true));
            tv.ImageList = imgS;

            Load += FrmFolderDiaglog_Load;
            tv.AfterExpand += Tv_AfterExpand;
            tv.NodeMouseDoubleClick += Tv_NodeMouseDoubleClick;
        }
        private void FrmFolderDiaglog_Load(object sender, EventArgs e)
        {
            tv.Nodes.Add(new TreeNode
            {
                Text = "Computer",
                ImageKey = "Computer"
            });
            var ld = xmlUtils.Drives();
            foreach (driveInfo di in ld)
            {
                TreeNode tn = new TreeNode
                {
                    Text = string.Format("{0} ({1})", di.VolumeLabel, di.Name.Replace(@"\", "")),
                    Tag = di,
                    ImageKey = di.Type,
                    SelectedImageKey = di.Type
                };
                tn.Nodes.Add("");
                tv.TopNode.Nodes.Add(tn);
            }
            tv.TopNode.Expand();

        }
        //TREEVIEW
        private void Tv_MouseDown(object sender, MouseEventArgs e)
        {
            TreeNode choose = tv.HitTest(e.Location).Node;
            if ((ModifierKeys & Keys.Control) != 0)
            {
                if (selectednode.Contains(choose))
                    selectednode.Remove(choose);
                else
                    selectednode.Add(choose);
            }
            else
            {
                selectednode.Clear();
                selectednode.Add(choose);
            }
            tv.Invalidate();
        }
        private void Tv_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if (e.Bounds.Width * e.Bounds.Height < 1) return;

            if (selectednode.Contains(e.Node))
            {
                e.Graphics.FillRectangle(Brushes.DeepSkyBlue, e.Bounds);
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(tv.BackColor), e.Bounds);
            }
            TextRenderer.DrawText(e.Graphics, e.Node.Text, e.Node.TreeView.Font, e.Bounds.Location, Color.Black);
        }
        private void Tv_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == tv.TopNode) return;
            if (!e.Node.IsExpanded) return;
            addchildnodeforTreeNode(e.Node, e.Node.Tag as ioInfo);
        }
        private void Tv_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node == tv.TopNode) return;
            if (!e.Node.IsExpanded) return;
            addchildnodeforTreeNode(e.Node, e.Node.Tag as ioInfo);
        }
        void addchildnodeforTreeNode(TreeNode Cur, ioInfo fi)
        {
            Cur.Nodes.Clear();
            foreach (folderInfo foi in xmlUtils.Folders(fi))
            {
                var tn = new TreeNode
                {
                    Text = foi.Name,
                    ImageKey = "Folder",
                    SelectedImageKey = "Folder",
                    Tag = foi
                };
                tn.Nodes.Add("");
                Cur.Nodes.Add(tn);
            }
            Cur.Expand();
        }
        //BUTTON
        private void BtnOK_Click(object sender, EventArgs e)
        {
            List<ioInfo> listiochoosen = new List<ioInfo>();


            if (AddFile)
            {
                List<driveInfo> listdisk = new List<driveInfo>();
                List<folderInfo> listfolder = new List<folderInfo>();
                List<fileInfo> listfile = new List<fileInfo>();

                foreach (TreeNode tn in selectednode)
                {
                    if (tn.Tag is folderInfo)
                        listfolder.Add(tn.Tag as folderInfo);
                    else if (tn.Tag is driveInfo)
                        listdisk.Add(tn.Tag as driveInfo);
                }

                if (cb.Checked)
                {
                    foreach (var di in listdisk)
                    {
                        for (int i = listfolder.Count - 1; i >= 0; i--)
                        {
                            var foi = listfolder[i];
                            ioInfo temp = foi;
                            while (temp.Parent != null)
                            {
                                temp = temp.Parent;
                            }
                            if (temp == (ioInfo)di)
                            {
                                listfolder.Remove(foi);
                            }
                        }

                        xmlUtils.GetRecursionFile(di, ref listfile);
                    }

                    foreach (folderInfo foi in listfolder)
                    {
                        xmlUtils.GetRecursionFile(foi, ref listfile);
                    }

                }
                else
                {
                    foreach (var di in listdisk)
                    {
                        listfile.AddRange(xmlUtils.Files(di));
                    }
                    foreach (folderInfo foi in listfolder)
                    {
                        listfile.AddRange(xmlUtils.Files(foi));
                    }
                }

                for (int i = listfile.Count - 1; i >= 0; i--)
                {
                    var fi = listfile[i];
                    string filename = string.Format($"{fi.Name}.{fi.Ext}");
                    var reg = new Regex(txtMask.Text.Replace(".", "[.]").Replace("*", ".*").Replace("?", "."));
                    if (!reg.IsMatch(filename))
                    {
                        listfile.Remove(fi);
                    }
                }

                foreach (fileInfo fi in listfile)
                {
                    listiochoosen.Add(fi);
                }
            }
            else
            {
                foreach (TreeNode tn in selectednode)
                {
                    listiochoosen.Add(tn.Tag as ioInfo);
                }
            }

            pickdone?.Invoke(listiochoosen);

            DialogResult = DialogResult.OK;
            Close();
        }
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
