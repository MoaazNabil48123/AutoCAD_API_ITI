using Addin;
using Autodesk.AutoCAD.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_Project.tabCreator.ButtonClass
{
    public class ColumnTag : ICadCommand
    {
        public override void Execute()
        {
            WPFCADAPI.ViewModel.CadLayers.layername();
        }
    }
}
