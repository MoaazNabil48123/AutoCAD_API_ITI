using Addin;
using Autodesk.AutoCAD.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_Project.tabCreator.ButtonClass
{
    internal class AxesDim : ICadCommand
    {
        public override void Execute()
        {
           View.AxesDim m = new View.AxesDim();
            Application.ShowModalWindow(m);
        }
    }
}
