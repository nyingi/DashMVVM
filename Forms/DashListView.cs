using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DashMvvm.Forms
{
    public class DashListView : ListView
    {
        public DashListView()
        {
            //Activate double buffering
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            //Enable the OnNotifyMessage event so we get a chance to filter out 
            // Windows messages before they get to the form's WndProc
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        protected override void OnNotifyMessage(Message m)
        {
            //Filter out the WM_ERASEBKGND message
            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }

        /// <summary>
        /// This property caches the type of object that supplies column headers. It is set by the library and any value you set in your code is overridden.
        /// </summary>
        public Type ColumnSourceCache { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if the newest added item is scrolled to.
        /// </summary>
        public bool ScrollToNewest { get; set; } = false;
    }
}
