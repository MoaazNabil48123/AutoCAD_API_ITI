using Addin;
using Autodesk.AutoCAD.ApplicationServices;
using ClassLibrary1;

namespace final_Project.tabCreator.ButtonClass
{
    public class QuantitySurvey : ICadCommand
    {
        public override void Execute()
        {
            ClassLibrary1.cadmethods.g();
        }
    }
}
