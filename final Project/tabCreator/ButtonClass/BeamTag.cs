using final_Project.View;
using Addin;
using Autodesk.AutoCAD.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_Project.tabCreator.ButtonClass
{
    internal class BeamTag : ICadCommand
    {
        public override void Execute()
        {
            final_Project.View.BeamTag m = new View.BeamTag();
            Application.ShowModalWindow(m);
        }
    }
}
