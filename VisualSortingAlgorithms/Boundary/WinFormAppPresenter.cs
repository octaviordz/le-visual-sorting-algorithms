using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VisualSortingAlgorithms.Boundary
{
    class WinFormAppPresenter
    {
        public WinFormAppPresenter(MainForm form)
        {
            var z = ZedGraphPresenter.CreateZedGraph();
            form.WindowState = FormWindowState.Maximized;
            form.Controls.Add(z);
        }
    }
}
