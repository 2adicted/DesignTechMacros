/*
 * Created by SharpDevelop.
 * User: Deyan Nenov
 * Date: 1/27/2017
 * Time: 1:09 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;

namespace DTMacros
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.DB.Macros.AddInId("7DF2BFC9-DDA4-42F7-9AD6-C73353D5F9AF")]
	public partial class ThisApplication
	{
		private void Module_Startup(object sender, EventArgs e)
		{

		}

		private void Module_Shutdown(object sender, EventArgs e)
		{

		}

		#region Revit Macros generated code
		private void InternalStartup()
		{
			this.Startup += new System.EventHandler(Module_Startup);
			this.Shutdown += new System.EventHandler(Module_Shutdown);
		}
		#endregion
		public void DoorRenumbering()
		{
			UIDocument uidoc = ActiveUIDocument;
            Document doc = ActiveUIDocument.Document;
            View current = doc.ActiveView;
            
            ISelectionFilter filter = new LineSelectionFilter();
            
            Reference reference = uidoc.Selection.PickObject(ObjectType.Element, filter, "Select direction curve");
            
            Curve curve = (doc.GetElement(reference.ElementId) as ModelCurve).GeometryCurve;
        	      	
            FilteredElementCollector collector = new FilteredElementCollector(doc, current.Id);
            
            List<Element> elements = collector.OfCategory(BuiltInCategory.OST_Doors).ToList();
            
            List<LocationPoint> points = new List<LocationPoint>();
            
            string s = "";
            int numPoints = 0;
            Dictionary<Element, double> sort = new Dictionary<Element, double>();
            
            foreach(Element element in elements)
            {
            	FamilyInstance door = element as FamilyInstance;
            	
            	XYZ point = door.GetTransform().Origin;
            	
            	if(point == null)
            	{
            		continue;
            	}
            	
            	IntersectionResult closestPoint = curve.Project(point);
            	
            	sort.Add(element, curve.ComputeNormalizedParameter(closestPoint.Parameter));
            }
            
            List<Element> sorted = sort.OrderBy(x => x.Value).Select(x => x.Key).ToList();
            
            using(Transaction t = new Transaction(doc, "Rename Mark Values"))
            {
            	t.Start();
	            foreach(Element el in sorted)
	            {
	            	el.LookupParameter("Mark").Set(numPoints.ToString());
	            	numPoints ++;
	            }
            	t.Commit();
            }
		}
		
		public class LineSelectionFilter : ISelectionFilter
		{
		    // determine if the element should be accepted by the filter
		    public bool AllowElement(Element element)
		    {
		        // Convert the element to a ModelLine
		        ModelLine line = element as ModelLine;
		        ModelCurve curve = element as ModelCurve;
		        // line is null if the element is not a model line
		        if (line == null && curve == null)
		        {
		            return false;
		        }
		        // return true if the line is a model line
		        return true;
		    }
		
		    // references will never be accepeted by this filter, so always return false
		    public bool AllowReference(Reference refer, XYZ point)
		    {return false;}
		}		
	}
}