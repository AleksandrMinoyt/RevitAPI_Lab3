using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPI_CalcWallVolume
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            //  var listWallRef= uidoc.Selection.PickObjects(ObjectType.Element,new SelectWallsFilter(), "Выберите стены");  Не работает если указать ObjectType.Face
            var listRef = uidoc.Selection.PickObjects(ObjectType.Face, "Выберите стены");
            
            double volWall=0;

            foreach (var item in listRef)
            {
                Wall wall = doc.GetElement(item) as Wall;
                if(wall!=null)
                {                
                    volWall+= UnitUtils.ConvertFromInternalUnits(wall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble(), UnitTypeId.CubicMeters );
                }
            }

            TaskDialog.Show("Объём выбраных стен:", volWall.ToString());

            return Result.Succeeded;
        }
    }

    public class SelectWallsFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {       
           return elem is Wall;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

}
