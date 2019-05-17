using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Do_An_2.Utils
{
    public enum Direction
    {
        top, up, down, bottom
    };
    class listviewMoveItems
    {
        public static void MoveItems(ListView lv, Direction di)
        {
            lv.Focus();

            if (lv.SelectedItems.Count == 0) return;

            List<ListViewItem> MySelected = new List<ListViewItem>();
            foreach (ListViewItem lvi in lv.SelectedItems)
                MySelected.Add(lvi);

            int type = -1;

            if ((int)di > 1)
                MySelected.Reverse();

            while (true)
            {
                if ((int)di < 2)
                {
                    if (MySelected[0].Index == 0) return;
                }
                else
                {
                    if (MySelected[0].Index == lv.Items.Count - 1) return;
                    type = 1;
                }

                foreach (ListViewItem lvi in MySelected)
                {
                    int newindex = lvi.Index + type;
                    lv.Items.Remove(lvi);
                    lv.Items.Insert(newindex, lvi);

                }
                if (di == Direction.up || di == Direction.down) return;
            }
        }
    }
}
