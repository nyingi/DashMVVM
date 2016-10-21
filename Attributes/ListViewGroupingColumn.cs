using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DashMvvm.Attributes
{
    public class ListViewGroupingColumn : Attribute
    {
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Left;
    }
}
