using Do_An_2.Method;
using Do_An_2.Utils;
using Do_An_2.Model;
using Do_An_2.Dialog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Diagnostics;

namespace Do_An_2
{
    public partial class frmRename : Form
    {
        private class ComboboxItem
        {
            public string Text { get; set; }
            public string Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }
        private string RecentFile = "Myrecent.txt";
        private string LastMethod = "Lastmethod.txt";
        public static string XmlPath = @"C:\data.xml";

        private List<fileInfo> FileList = new List<fileInfo>();
        private List<folderInfo> FolderList = new List<folderInfo>();
        private List<driveInfo> DriveList = new List<driveInfo>();

        private methodBase Ischoose;
        private bool IsAdding = false;

        private ListView CurrentExec;

        public frmRename()
        {
            InitializeComponent();

            //remove horizontalscroll
            flpane.AutoScroll = false;
            flpane.HorizontalScroll.Maximum = 0;
            flpane.AutoScroll = true;

            CurrentExec = lvFile;
            tabControl1.SelectedIndexChanged += (s, e) =>
            {
                if (tabControl1.SelectedIndex == 0) CurrentExec = lvFile;
                else CurrentExec = lvFolder;
            };

            //disable
            ControlMethod(false, true);

            //list
            lvFile.SmallImageList = new ImageList();
            lvFile.SmallImageList.ColorDepth = ColorDepth.Depth32Bit;
            lvFile.ItemChecked += LvItem_CheckedChanged;
            lvFolder.SmallImageList = new ImageList();
            lvFolder.SmallImageList.ColorDepth = ColorDepth.Depth32Bit;
            lvFolder.SmallImageList.Images.Add("Computer", myShell.GetComputerIcon(true));
            lvFolder.SmallImageList.Images.Add("Sys", myShell.GetFileIcon(@"C:\", true));
            lvFolder.SmallImageList.Images.Add("Fixed", myShell.GetFileIcon(@"D:\", true));
            lvFolder.SmallImageList.Images.Add("CDRom", myShell.GetFileIcon(@"Z:\", true));
            lvFolder.SmallImageList.Images.Add("Folder", myShell.GetFolderIcon(true));
            lvFolder.ItemChecked += LvItem_CheckedChanged;

            //add
            addfile_1.Click += Addfile_event;
            addfile_2.Click += Addfilefromfolder_event; ;
            addfolder.Click += Addfolder_event;
            //move
            tsbtntop_file.Click += (s, e) =>
            {
                IsAdding = true;
                listviewMoveItems.MoveItems(lvFile, Direction.top);
                IsAdding = false;
            };
            tsbtnup_file.Click += (s, e) =>
            {
                IsAdding = true;
                listviewMoveItems.MoveItems(lvFile, Direction.up);
                IsAdding = false;
            };
            tsbtndown_file.Click += (s, e) =>
            {
                IsAdding = true;
                listviewMoveItems.MoveItems(lvFile, Direction.down);
                IsAdding = false;
            };
            tsbtnbottom_file.Click += (s, e) =>
            {
                IsAdding = true;
                listviewMoveItems.MoveItems(lvFile, Direction.bottom);
                IsAdding = false;
            };
            tsbtntop_folder.Click += (s, e) =>
            {
                IsAdding = true;
                listviewMoveItems.MoveItems(lvFolder, Direction.top);
                IsAdding = false;
            };
            tsbtnup_folder.Click += (s, e) =>
            {
                IsAdding = true;
                listviewMoveItems.MoveItems(lvFolder, Direction.up);
                IsAdding = false;
            };
            tsbtndown_folder.Click += (s, e) =>
            {
                IsAdding = true;
                listviewMoveItems.MoveItems(lvFolder, Direction.down);
                IsAdding = false;
            };
            tsbtnbottom_folder.Click += (s, e) =>
            {
                IsAdding = true;
                listviewMoveItems.MoveItems(lvFolder, Direction.bottom);
                IsAdding = false;
            };

            //delete
            tsbtnClearMethods.Click += (s, e) =>
            {
                for (int i = flpane.Controls.Count - 1; i >= 0; i--)
                {
                    flpane.Controls[i].Dispose();
                }
            };
            tsbtnRemoveMethod.Click += (s, e) => Ischoose.Dispose();
            //move
            tsbtntop_MT.Click += (s, e) => MethodBase_Move(Direction.top);
            tsbtnup_MT.Click += (s, e) => MethodBase_Move(Direction.up);
            tsbtndown_MT.Click += (s, e) => MethodBase_Move(Direction.down);
            tsbtnbottom_MT.Click += (s, e) => MethodBase_Move(Direction.bottom);
            //function
            newNameToolStripMenuItem.Click += (s, e) => Add_Method("newname");
            newCaseToolStripMenuItem.Click += (s, e) => Add_Method("newcase");
            moveToolStripMenuItem.Click += (s, e) => Add_Method("move");
            removeToolStripMenuItem.Click += (s, e) => Add_Method("remove");
            removePatternToolStripMenuItem.Click += (s, e) => Add_Method("removepattern");
            renumberToolStripMenuItem.Click += (s, e) => Add_Method("renumber");
            replaceToolStripMenuItem.Click += (s, e) => Add_Method("replace");
            addToolStripMenuItem.Click += (s, e) => Add_Method("add");
            listToolStripMenuItem.Click += (s, e) => Add_Method("list");
            swapToolStripMenuItem.Click += (s, e) => Add_Method("swap");
            trimToolStripMenuItem.Click += (s, e) => Add_Method("trim");

            //tab
            tabControl1.SelectedIndexChanged += (s, e) => Execute_Method();

            //refresh
            tsbtnRefresh.Click += (s, e) => Execute_Method();
            //about
            tsmAbout.Click += (s, e) => (new About()).ShowDialog();
            //doc
            tsmDoc.Click += (s, e) =>
            {
                Form Document = new Form
                {
                    Text = "Hướng dẫn sử dụng",
                    MaximizeBox = false,
                    MinimizeBox = false,
                    FormBorderStyle = FormBorderStyle.FixedSingle,
                    BackgroundImage = Properties.Resources.doc,
                    BackgroundImageLayout = ImageLayout.Stretch,
                    Size = new Size((int)(Properties.Resources.doc.Size.Width / 1.3), (int)(Properties.Resources.doc.Size.Height / 1.3)),
                    StartPosition = FormStartPosition.CenterScreen
                };
                Document.ShowDialog();
            };
            //startbatch
            tsbtnStartBatch.Click += (s, e) =>
            {
                if (CurrentExec.Items.Count == 0 || flpane.Controls.Count == 0)
                {
                    MessageBox.Show("Nothing change","Warning");
                }
                else if (MessageBox.Show("Do you want continue?", "My Rename", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    foreach (ListViewItem lvi in CurrentExec.Items)
                    {
                        if (!lvi.Checked) continue;
                        if (lvi.SubItems[3].Text != "OK") { MessageBox.Show("Please check error and try again", "My Rename"); return; }
                    }
                    foreach (ListViewItem lvi in CurrentExec.Items)
                    {
                        if (!lvi.Checked) continue;

                        XmlNode xn = (lvi.Tag as ioInfo).Node;

                        if (tabControl1.SelectedIndex == 0)
                        {
                            xn.Attributes["name"].Value = Path.GetFileNameWithoutExtension(lvi.SubItems[1].Text);
                            xn.Attributes["ext"].Value = Path.GetExtension(lvi.SubItems[1].Text).Replace(".", "");
                            lvi.SubItems[0].Text = lvi.SubItems[1].Text;
                        }
                        else lvi.SubItems[0].Text = xn.Attributes["name"].Value = lvi.SubItems[1].Text;
                    }
                    xmlUtils.Save();
                    MessageBox.Show("Complete", "My Rename");
                }
            };

            tsbtnSave.Click += Method_Save;
            tsbtnOpen.Click += Method_Open;
            tscRecent.SelectedIndexChanged += Recent_SelectChanged;

            Load += (s, e) =>
            {
                if (File.Exists(RecentFile)) MethodRecent_Load();
                if (File.Exists(LastMethod)) LoadMethod_FromFile(LastMethod);
            };
            FormClosing += (s, e) =>
            {
                IsAdding = true;
                SaveMethod_ToFile(LastMethod);
            };
        }


        //METHOD USER
        private void Recent_SelectChanged(object sender, EventArgs e)
        {
            LoadMethod_FromFile((tscRecent.SelectedItem as ComboboxItem).Value);
        }
        private void Method_Open(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Rename Project|*.rpj";
            ofd.Title = "Open batch method";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LoadMethod_FromFile(ofd.FileName);

                File.AppendAllText("Myrecent.txt", ofd.FileName + "\n");
                MethodRecent_Load();
            }
        }
        private void LoadMethod_FromFile(string path)
        {
            IsAdding = true;

            flpane.Controls.Clear();
            foreach (XElement xE in XElement.Load(path).Nodes())
                Add_Method(xE.Name.ToString(), xE);

            IsAdding = false;
            Execute_Method();
        }
        private void Method_Save(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Rename Project|*.rpj";
            sfd.Title = "Save batch method";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                SaveMethod_ToFile(sfd.FileName);

                File.AppendAllText(RecentFile, sfd.FileName + "\n");
                MethodRecent_Load();
            }
        }
        private void SaveMethod_ToFile(string path)
        {
            XElement xE = new XElement("Data");

            foreach (methodBase mb in flpane.Controls)
                xE.Add(mb.SaveData());

            xE.Save(path);
        }
        private void MethodRecent_Load()
        {
            string[] recentList = File.ReadAllLines(RecentFile);
            recentList = recentList.Distinct().ToArray();
            File.WriteAllLines(RecentFile, recentList);

            tscRecent.Items.Clear();
            foreach (string itempath in recentList)
            {
                if (string.IsNullOrEmpty(itempath)) continue;
                else tscRecent.Items.Add(new ComboboxItem { Text = Path.GetFileName(itempath), Value = itempath });
            }
        }
        //METHOD SYS
        void Execute_Method()
        {
            if (IsAdding) return;

            if (CurrentExec.Items.Count == 0) return;

            System.Threading.Thread.Sleep(200);//xoa cache

            CurrentExec.Visible = false;

            bool isrealexec = false;

            Action Query = () =>
            {
                foreach (ListViewItem lvi in CurrentExec.Items)
                {
                    if (lvi.Checked)
                        Execute_Method_Item(lvi, ref isrealexec);
                }
            };

            if (Debugger.IsAttached) Query();
            else
                using (frmWaiting fw = new frmWaiting(Query))
                {
                    fw.ShowDialog(this);
                }

            CurrentExec.Visible = true;

            //
            if (isrealexec)
                Execute_CheckSameName();



        }
        void Execute_Method_Item(ListViewItem lvi, ref bool isrealexec)
        {
            string lastname, error = "";
            if (!string.IsNullOrEmpty(lvi.SubItems[0].Text)) lastname = lvi.SubItems[0].Text;
            else goto done;

            foreach (methodBase mb in flpane.Controls)
            {
                if (!mb.IsChecked) continue; isrealexec = true;
                if (CurrentExec.Equals(lvFile))
                {
                    if (mb.ApplytoIndex == 2) mb.ExecuteData(lastname, ref lastname, ref error);
                    else
                    {
                        string FileName = Path.GetFileNameWithoutExtension(lastname);
                        string Ext = Path.GetExtension(lastname).Replace(".", "");

                        if (mb.ApplytoIndex == 0) mb.ExecuteData(FileName, ref FileName, ref error);
                        else mb.ExecuteData(Ext, ref Ext, ref error);

                        lastname = string.Format($"{FileName}.{Ext}");
                    }
                }
                else mb.ExecuteData(lastname, ref lastname, ref error);
            }
            lvi.SubItems[1].Text = lastname;

            done:

            lvi.ForeColor = Color.Red;
            if (!string.IsNullOrEmpty(error)) lvi.SubItems[3].Text = error;
            else if (string.IsNullOrEmpty(lvi.SubItems[1].Text))
                lvi.SubItems[3].Text = "File/Folder name is not specified";
            else if (lvi.SubItems[1].Text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                lvi.SubItems[3].Text = "File/Folder name is not allowed";
            else
            {
                lvi.ForeColor = Color.Black;
                lvi.SubItems[3].Text = "OK";
            }
        }
        void Execute_CheckSameName()
        {
            if (CurrentExec.Items.Count > 500)
            {
                if (MessageBox.Show("The list file/folder are too big, do you want to continue Check Same Name?", "Warning", MessageBoxButtons.YesNo) == DialogResult.No) return;
            }

            System.Threading.Thread.Sleep(200);//xoa cache

            CurrentExec.Visible = false;

            Action Query = () =>
            {
                for (int i = 0; i < CurrentExec.Items.Count - 1; i++)
                {
                    ListViewItem lvi = CurrentExec.Items[i];
                    if (!lvi.Checked || lvi.SubItems[3].Text != "OK") continue;
                    bool isfound = false;

                    for (int j = i + 1; j < CurrentExec.Items.Count; j++)
                    {
                        ListViewItem lvi2 = CurrentExec.Items[j];
                        if (!lvi2.Checked || lvi2.SubItems[3].Text != "OK") continue;
                        else if (lvi2.SubItems[1].Text == lvi.SubItems[1].Text)
                        {
                            isfound = true;
                            lvi2.ForeColor = Color.Red;
                            lvi2.SubItems[3].Text = "Possibility of multiple files with the same name";
                        }
                    }

                    if (isfound)
                    {
                        lvi.ForeColor = Color.Red;
                        lvi.SubItems[3].Text = "Possibility of multiple files with the same name";
                    }
                }
            };

            if (Debugger.IsAttached) Query();
            else
                using (frmWaiting fw = new frmWaiting(Query, "Check Same Name..."))
                {
                    fw.ShowDialog(this);
                }

            CurrentExec.Visible = true;

        }

        //METHOD CONTROL
        private void Add_Method(string type, XElement xE = null)
        {
            methodBase method;

            switch (type)
            {
                case "newname":
                    method = new mb_NewName();
                    break;
                case "newcase":
                    method = new mb_NewCase();
                    break;
                case "move":
                    method = new mb_Move();
                    break;
                case "remove":
                    method = new mb_Remove();
                    break;
                case "removepattern":
                    method = new mb_RemovePattern();
                    break;
                case "replace":
                    method = new mb_Replace();
                    break;
                case "add":
                    method = new mb_Add();
                    break;
                default:
                    method = new methodBase();
                    break;
            }

            method.ExpandedChanged += MethodBase_ExpandedChanged;
            method.CheckedChanged += Execute_Method;
            method.DisposeControls += MethodBase_DisposeControls;
            flpane.Controls.Add(method);
            method.LoadData(xE);
        }
        void MethodBase_Move(Direction di)
        {
            int type = -1;
            while (true)
            {
                var index = flpane.Controls.IndexOf(Ischoose);

                if ((int)di < 2) { if (index == 0) break; }
                else
                {
                    if (index == flpane.Controls.Count - 1) break;
                    type = 1;
                }

                flpane.Controls.SetChildIndex(Ischoose, index + type);
                if (di == Direction.up || di == Direction.down) break;
            }
            Execute_Method();
        }
        void MethodBase_DisposeControls()
        {
            if (flpane.Controls.Count == 0) ControlMethod(false, true);
            Execute_Method();
        }
        void MethodBase_ExpandedChanged()
        {
            ControlMethod(true, true);
            foreach (methodBase mb in flpane.Controls)
            {
                if (mb.Expaned)
                {
                    Ischoose = mb;
                    return;
                }
            }
            ControlMethod(false, false);
        }
        void ControlMethod(bool result, bool clear)
        {
            tsbtnRemoveMethod.Enabled = tsbtntop_MT.Enabled = tsbtnup_MT.Enabled = tsbtndown_MT.Enabled = tsbtnbottom_MT.Enabled = result;
            if (clear) tsbtnClearMethods.Enabled = tsbtnSave.Enabled = result;
        }
        //LISTVIEW
        private void LvItem_CheckedChanged(object sender, ItemCheckedEventArgs e)
        {
            if (IsAdding) return;

            if (e.Item.Checked)
            {
                bool isrealexec = false;
                Execute_Method_Item(e.Item, ref isrealexec);
                if (isrealexec)
                    Execute_CheckSameName();
            }
            else
            {
                e.Item.ForeColor = Color.Black;
                e.Item.SubItems[1].Text = e.Item.SubItems[3].Text = "";
            }
        }
        void parsetolistview(bool isfile)
        {
            IsAdding = true;

            Action Query = () =>
            {
                if (isfile)
                {
                    lvFile.Items.Clear();
                    lvFile.Visible = false;
                    foreach (var fi in FileList)
                    {
                        if (!lvFile.SmallImageList.Images.ContainsKey(fi.Ext))
                            lvFile.SmallImageList.Images.Add(fi.Ext, myShell.GetFileIcon(string.Format(".{0}", fi.Ext), true));

                        var lvi = new ListViewItem
                        {
                            Text = string.Format($"{fi.Name}.{fi.Ext}"),
                            ImageKey = fi.Ext,
                            Tag = fi,
                            Checked = true
                        };
                        lvi.SubItems.Add("");
                        lvi.SubItems.Add(xmlUtils.Getlocation(fi, false));
                        lvi.SubItems.Add("");
                        lvFile.Items.Add(lvi);
                    }
                    lvFile.Visible = true;
                }
                else
                {
                    lvFolder.Items.Clear();
                    lvFolder.Visible = false;
                    foreach (var di in DriveList)
                    {
                        var lvi = new ListViewItem
                        {
                            Text = "",
                            Tag = di,
                            ImageKey = di.Type,
                            Checked = true
                        };
                        lvi.SubItems.Add("");
                        lvi.SubItems.Add(di.Name);
                        lvi.SubItems.Add("");
                        lvFolder.Items.Add(lvi);
                    }
                    foreach (var foi in FolderList)
                    {
                        var lvi = new ListViewItem
                        {
                            Text = foi.Name,
                            Tag = foi,
                            ImageKey = "Folder",
                            Checked = true
                        };
                        lvi.SubItems.Add("");
                        lvi.SubItems.Add(xmlUtils.Getlocation(foi, false));
                        lvi.SubItems.Add("");
                        lvFolder.Items.Add(lvi);
                    }
                    lvFolder.Visible = true;
                }
            };

            if (Debugger.IsAttached) Query();
            else
                using (frmWaiting fw = new frmWaiting(Query, "Adding File/Folder..."))
                {
                    fw.ShowDialog(this);
                }

            IsAdding = false;
            Execute_Method();
        }
        private void Addfilefromfolder_event(object sender, EventArgs e)
        {
            var fdg = new frmFolderDiaglog(false);
            fdg.pickdone += (s) =>
            {
                FileList.Clear();
                foreach (ioInfo fi in s) FileList.Add(fi as fileInfo);
            };

            if (fdg.ShowDialog() == DialogResult.OK) parsetolistview(true);
        }
        private void Addfile_event(object sender, EventArgs e)
        {
            var fdg = new frmFileDialog();
            fdg.pickdone += (s) => FileList = s;
            if (fdg.ShowDialog() == DialogResult.OK) parsetolistview(true);
        }
        private void Addfolder_event(object sender, EventArgs e)
        {
            var fdg = new frmFolderDiaglog(true);
            fdg.pickdone += (s) =>
            {
                FolderList.Clear();
                DriveList.Clear();
                foreach (ioInfo io in s)
                {
                    if (io is folderInfo) FolderList.Add(io as folderInfo);
                    if (io is driveInfo) DriveList.Add(io as driveInfo);
                }
            };
            if (fdg.ShowDialog() == DialogResult.OK) parsetolistview(false);
        }
    }
}
