using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Xml.Linq;
using System.IO;

namespace Do_An_2.Method
{
    public class methodBase : Button
    {
        public void ExecuteData(string oldname, ref string newname, ref string error)
        {
            if (IsDisposed) return;

            bool ispatternerror = false;
            newname = converter(oldname, ref ispatternerror);

            if (ispatternerror)
                error = "Invalid regular expression";
        }

        public virtual void LoadData(XElement xE)
        {
            if (xE == null) return;
            (Controls.Find("applytype", true)[0] as ComboBox).SelectedIndex = int.Parse(xE.Value);
        }
        public virtual XElement SaveData()
        {
            if (Controls.Find("applytype", true).Length < 1) return null;
            return new XElement("applyto", (Controls.Find("applytype", true)[0] as ComboBox).SelectedIndex);
        }

        public event Action ExpandedChanged;
        public event Action CheckedChanged; // cung la ham thay doi data
        public event Action DisposeControls;

        public bool Expaned
        {
            get { return IsExpand; }
            set
            {
                IsExpand = value;
                Refresh();
                if (Expaned)
                {
                    foreach (Control co in Parent.Controls)
                    {
                        var mb = co as methodBase;
                        if (mb != this)
                            mb.Expaned = false;
                    }
                }
                ExpandedChanged?.Invoke();
            }
        }
        public int ApplytoIndex
        {
            get
            {
                if (Controls.Find("applytype", true).Length < 1) return 2;
                return (Controls.Find("applytype", true)[0] as ComboBox).SelectedIndex;
            }
        }
        public bool IsChecked { get { return cbischecked.Checked; } set { cbischecked.Checked = value; } }
        //
        protected void CheckedChangedCall(object sender, EventArgs e) { CheckedChanged?.Invoke(); }
        protected void AddFooter(int top)
        {
            MyAddControl(new Label { Text = "Apply to:", Left = 8, Top = top + 24, AutoSize = true });

            var cbb = new ComboBox { Name = "applytype", Left = 60, Top = top + 21 };
            cbb.Items.Add("Name");
            cbb.Items.Add("Extension");
            cbb.Items.Add("Name and extension");
            cbb.SelectedIndex = 0;
            cbb.SelectedIndexChanged += CheckedChangedCall;
            MyAddControl(cbb);

            BodyHeight = top + 50;
        }
        protected virtual string converter(string lastname, ref bool ispatternerror) { return lastname; }
        protected int BodyHeight
        {
            get { return bodyheight; }
            set
            {
                bodyheight = value;
                MyBodyBounds.Height = value;
            }
        }
        //
        //SYSTEM
        //
        private CheckBox cbischecked = new CheckBox { CheckAlign = ContentAlignment.MiddleCenter, AutoSize = true, Checked = true };
        private SolidBrush ControlC = new SolidBrush(SystemColors.Control);
        private SolidBrush Inactive = new SolidBrush(Color.LightGray);
        private SolidBrush Active = new SolidBrush(Color.LightCyan);
        private SolidBrush NotChoose = new SolidBrush(Color.LightPink);
        private Rectangle MyHeadBounds = new Rectangle(0, 0, 0, 30);
        private Rectangle MyBodyBounds = new Rectangle(0, 30, 0, 0);
        private Rectangle StatusImage = new Rectangle(2, 2, 24, 24);
        private Rectangle DeleteImage = new Rectangle(0, 2, 24, 24);

        private Font Title = new Font("Microsoft Sans Serif", 10);
        private Bitmap plus, sub, del;
        private Point TextPos = new Point(40, 5);
        private int bodyheight = 0;
        private bool IsExpand = false;
        //
        public methodBase()
        {
            plus = Properties.Resources.icons8_plus_math_32;
            sub = Properties.Resources.icons8_subtract_32;
            del = Properties.Resources.icons8_delete_32;

            cbischecked.CheckedChanged += (s, e) => { CheckedChanged?.Invoke(); Refresh(); };
            Controls.Add(cbischecked);
        }
        protected override void CreateHandle()
        {
            base.CreateHandle();
            Expaned = true;
            Width = Parent.ClientSize.Width - Parent.Margin.Horizontal;
            Parent.ClientSizeChanged += (s, e) => { if (Parent != null) Width = Parent.ClientSize.Width - Parent.Margin.Horizontal; };
            CheckedChanged?.Invoke(); //tao moi thi phai cap nhat lai du lieu
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            MyHeadBounds.Width = MyBodyBounds.Width = Width;
            DeleteImage.X = Width - 26;
            cbischecked.Left = MyHeadBounds.Right - 52;
            cbischecked.Top = 7;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var graf = e.Graphics;

            if (Expaned)
            {
                Height = MyHeadBounds.Height + MyBodyBounds.Height;
                graf.FillRectangle(Active, MyHeadBounds);
                graf.FillRectangle(ControlC, MyBodyBounds);

                Rectangle BorderR = MyBodyBounds;
                BorderR.X++; BorderR.Y++; BorderR.Width--; BorderR.Height--;
                ControlPaint.DrawBorder(graf, BorderR, Color.Black, ButtonBorderStyle.Solid);

                if (!IsChecked) graf.FillRectangle(NotChoose, MyHeadBounds);

                graf.DrawImage(sub, StatusImage);
            }
            else
            {
                Height = MyHeadBounds.Height;
                graf.FillRectangle(Inactive, MyHeadBounds);

                if (!IsChecked) graf.FillRectangle(NotChoose, MyHeadBounds);

                graf.DrawImage(plus, StatusImage);
            }

            graf.DrawImage(del, DeleteImage);
            TextRenderer.DrawText(graf, Text, Title, TextPos, ForeColor);
            ControlPaint.DrawBorder(graf, MyHeadBounds, Color.Black, ButtonBorderStyle.Solid);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (DeleteImage.Contains(e.Location)) Dispose();
            else if (MyHeadBounds.Contains(e.Location)) Expaned = !Expaned;
        }
        protected override void OnMouseMove(MouseEventArgs mevent)
        {
            base.OnMouseMove(mevent);
            if (MyHeadBounds.Contains(mevent.Location)) Cursor = Cursors.Hand;
            else Cursor = Cursors.Default;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            DisposeControls?.Invoke();
        }
        //
        protected void MyAddControl(Control x)
        {
            x.Left += MyBodyBounds.Left;
            x.Top += MyBodyBounds.Top;
            Controls.Add(x);
        }
    }
}
