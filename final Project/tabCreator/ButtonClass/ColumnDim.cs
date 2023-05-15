using Addin;
using Autodesk.AutoCAD.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_Project.tabCreator.ButtonClass
{
    internal class ColumnDim:ICadCommand
    {
        public override void Execute()
        {
            View.ColumnDim m = new View.ColumnDim();
            Application.ShowModalWindow(m);
        }

    }
}
