using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPI_CalcPipeLength
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var listRef = uidoc.Selection.PickObjects(ObjectType.Element, "Выберите трубы");

            double lengthPipe = 0;

            foreach (var item in listRef)
            {
               Pipe pipe = doc.GetElement(item) as Pipe;
                if (pipe != null)
                {
                    lengthPipe += UnitUtils.ConvertFromInternalUnits(pipe.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble(), UnitTypeId.Meters);
                }
            }

            TaskDialog.Show("Общая длина выбранных труб", lengthPipe.ToString());

            return Result.Succeeded;
        }
    }
}
