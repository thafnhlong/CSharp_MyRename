using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Do_An_2.Dialog
{
    public partial class frmWaiting : Form
    {
        public Action Worker { get; set; }
        public frmWaiting(Action worker, string text = "Execute Method...")
        {
            InitializeComponent();
            if (worker == null) { throw new ArgumentException(); }

            Worker = worker;
            label1.Text = text;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Task.Factory.StartNew(Worker).ContinueWith(t => { Close(); TaskScheduler.FromCurrentSynchronizationContext(); });
        }
    }
}
