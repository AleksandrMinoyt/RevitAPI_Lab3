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

namespace RevitAPI_CreateAndAddParameter
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var categorySet = new CategorySet();
            categorySet.Insert(Category.GetCategory(doc, BuiltInCategory.OST_PipeCurves));

            var listRef = uidoc.Selection.PickObjects(ObjectType.Element, "Выберите трубы");

            List<Pipe> pipes = new List<Pipe>();

            foreach (var item in listRef)
            {
                Pipe pipe = doc.GetElement(item) as Pipe;
                if (pipe != null)
                {
                    pipes.Add(pipe);
                }
            }


            using (Transaction ts = new Transaction(doc, "Добавляем параметр"))
            {
                ts.Start();
                CreateSharedParameter(uiapp.Application, doc, "Длина с запасом", categorySet, BuiltInParameterGroup.PG_LENGTH, true);

                foreach (var pipe in pipes)
                {
                    pipe.LookupParameter("Длина с запасом").Set(pipe.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble()*1.1);
                }

                ts.Commit();
            }
            return Result.Succeeded;
        }

        private void CreateSharedParameter(Application application,
                                           Document doc,
                                           string parameterName,
                                           CategorySet categorySet,
                                           BuiltInParameterGroup builtInParameterGroup,
                                           bool isInstance)
        {
            DefinitionFile definitionFile = application.OpenSharedParameterFile();
            if (definitionFile == null)
            {
                TaskDialog.Show("Ошибка", "Не найден файл общих параметров");
                return;
            }

            Definition definition = definitionFile.Groups
                .SelectMany(group => group.Definitions)
                .FirstOrDefault(def => def.Name.Equals(parameterName));
            if (definition == null)
            {
                TaskDialog.Show("Ошибка", "Не найден указанный параметр");
                return;
            }

            Binding binding = application.Create.NewTypeBinding(categorySet);
            if (isInstance)
                binding = application.Create.NewInstanceBinding(categorySet);

            BindingMap map = doc.ParameterBindings;
            map.Insert(definition, binding, builtInParameterGroup);
        }
    }
}
