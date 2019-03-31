using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.DesignScript.Runtime;
using RevitServices.Persistence;
using Revit.GeometryConversion;
using Revit.Elements;
using RevitServices.Transactions;

namespace Quasar
{

    /// <summary>
    /// Utility class for tools methods
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Utility class tool
        /// </summary>
        /// <param name="bbox"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        public static BoundingBoxXYZ Crop_box(BoundingBoxXYZ bbox, double offset)
        {
            var minx = bbox.Min.X - offset;
            var miny = bbox.Min.Y - offset;
            var minz = bbox.Min.Z - offset;
            var maxx = bbox.Max.X + offset;
            var maxy = bbox.Max.Y + offset;
            var maxz = bbox.Max.Z + offset;

            var newbox = new BoundingBoxXYZ
            {
                Min = new XYZ(minx, miny, minz),
                Max = new XYZ(maxx, maxy, maxz)
            };

            return newbox;

        }

    }

    /// <summary>
    /// Quasar ElementGraphics Class contains
    /// element visibility, view filters 
    /// </summary>
    public static class ElementGraphics
    {

        /// <summary>
        ///     Copy filters and override graphic settings from a view and paste its to views.
        /// </summary>
        /// <param name="ViewToCopy">
        ///     View to Copy
        /// </param>
        /// <param name="ViewToPaste">
        ///     Views to paste
        /// </param>
        /// <returns name="Views">Pasted Views</returns>

        [IsVisibleInDynamoLibrary(true)]
        public static List<Revit.Elements.Element> CopyPasteFilter(Revit.Elements.Views.View ViewToCopy, List<Revit.Elements.Views.View> ViewToPaste)
        {

            var filtersId = new List<ElementId>();
            var settings = new List<OverrideGraphicSettings>();
            var visibility = new List<Boolean>();
            var Views = new List<Revit.Elements.Element>();
            var viewtocopy = (Autodesk.Revit.DB.View)ViewToCopy.InternalElement;
            foreach (var id in viewtocopy.GetFilters())
            {
                filtersId.Add(id);
                settings.Add(viewtocopy.GetFilterOverrides(id));
                visibility.Add(viewtocopy.GetFilterVisibility(id));
            }
            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
            foreach (var pview in ViewToPaste)
            {
                var pasteview = (Autodesk.Revit.DB.View)pview.InternalElement;
                foreach (var i in filtersId.Zip(settings, Tuple.Create))
                {
                    pasteview.SetFilterOverrides(i.Item1, i.Item2);
                }
                foreach (var j in filtersId.Zip(visibility, Tuple.Create))
                {
                    pasteview.SetFilterVisibility(j.Item1, j.Item2);
                }
                Views.Add(pview);
            }

            TransactionManager.Instance.TransactionTaskDone();
            return Views;
        }


        /// <summary>
        /// Elements hide/unhide in given view. Default value is true(hide).
        /// </summary>
        /// <param name="Elements">Elements or Element</param>
        /// <param name="Views">Views or View</param>
        /// <param name="HideUnhide">true = hide , false = unhide</param>
        /// <returns>Return message</returns>
        [IsVisibleInDynamoLibrary(true)]
        public static String HideUnHideElement(List<Revit.Elements.Element> Elements, List<Revit.Elements.Element> Views, Boolean HideUnhide = false)
        {
            var ids = new List<ElementId>();
            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
            foreach (var elem in Elements)
            {
                var id = (Autodesk.Revit.DB.Element)elem.InternalElement;
                ids.Add(id.Id);
            }
            foreach (var view in Views)
            {
                if (HideUnhide == true)
                {
                    var v = (Autodesk.Revit.DB.View)view.InternalElement;
                    v.HideElements(ids);
                }
                else
                {
                    var v = (Autodesk.Revit.DB.View)view.InternalElement;
                    v.UnhideElements(ids);
                }
            }
            TransactionManager.Instance.TransactionTaskDone();
            return "Done!";
        }

        /// <summary>
        /// Hide/Unhide levels and grids from link documents.
        /// </summary>
        /// <param name="Hide"> Hide = true, Unhide = false</param>
        /// <returns>return message</returns>

        [IsVisibleInDynamoLibrary(true)]
        public static String LinkLevelGrid(Boolean Hide = true)
        {
            String ifilter = "LinkLevelGrid_QuasarPackage";
            var doc = DocumentManager.Instance.CurrentDBDocument;
            var activeView = doc.ActiveView;
            Boolean found = false;
            Boolean hide = Hide == true ? false : true;

            TransactionManager.Instance.EnsureInTransaction(doc);

            var allFilters = new FilteredElementCollector(doc).OfClass(typeof(FilterElement)).ToElements();

            var viewFilters = activeView.GetFilters();
            List<String> viewFiltersName = new List<String>();
            foreach (var v in viewFilters) { viewFiltersName.Add(doc.GetElement(v).Name.ToString()); }

            foreach (var fter in allFilters)
            {
                if (ifilter == fter.Name.ToString() && !viewFiltersName.Contains(ifilter))
                {
                    activeView.AddFilter(fter.Id);
                    activeView.SetFilterVisibility(fter.Id, hide);
                    found = true;
                }
                if (ifilter == fter.Name.ToString() && viewFiltersName.Contains(ifilter))
                {
                    activeView.SetFilterVisibility(fter.Id, hide);
                    found = true;
                }
            }

            if (!found)
            {
                var grids = new FilteredElementCollector(doc).OfClass(typeof(Autodesk.Revit.DB.Grid)).ToElements();
                var levels = new FilteredElementCollector(doc).OfClass(typeof(Autodesk.Revit.DB.Level)).ToElements();
                var cateIds = new List<ElementId> { grids.First().Category.Id, levels.First().Category.Id };
                var gridTypeIds = new HashSet<ElementId>();
                var levelTypeIds = new HashSet<ElementId>();
                foreach (var i in grids.Zip(levels, Tuple.Create)) { gridTypeIds.Add(i.Item1.GetTypeId()); levelTypeIds.Add(i.Item2.GetTypeId()); }

                var gtypeElements = new List<Autodesk.Revit.DB.Element>();
                var ltypeElements = new List<Autodesk.Revit.DB.Element>();
                foreach (var i in gridTypeIds.Zip(levelTypeIds, Tuple.Create)) { gtypeElements.Add(doc.GetElement(i.Item1)); ltypeElements.Add(doc.GetElement(i.Item2)); }
                gtypeElements.AddRange(ltypeElements);

                foreach (var e in gtypeElements)
                {
                    if (!e.LookupParameter("Type Name").AsString().Contains("_quasar"))
                    {
                        e.Name = e.LookupParameter("Type Name").AsString() + "_quasar";
                    }
                }
                var paramId = gtypeElements.First().LookupParameter("Type Name").Id;
                var ruleSet = new List<FilterRule>();
                var notEndsWith = ParameterFilterRuleFactory.CreateNotEndsWithRule(paramId, "_quasar", false);
                ruleSet.Add(notEndsWith);
                var paramFilterElem = ParameterFilterElement.Create(doc, ifilter, cateIds, ruleSet);
                var ogs = new OverrideGraphicSettings();
                activeView.SetFilterOverrides(paramFilterElem.Id, ogs);
                activeView.SetFilterVisibility(paramFilterElem.Id, hide);
            }
            TransactionManager.Instance.TransactionTaskDone();

            return "DONE!";
        }

    }
    /// <summary>
    /// Quasar BuiltIns Class contains revit built-ins nodes.
    /// </summary>
    public static class BuiltIns
    {
        /// <summary>
        /// Built-In Parameter Name by element and parameter(s).
        /// </summary>
        /// <param name="Element">Element input</param>
        /// <param name="Names">Parameter name or names (string)</param>
        /// <returns name = "NameList">Each list contains [0] ParameterName [1] BuiltIn ParameterName</returns>
        [IsVisibleInDynamoLibrary(true)]
        public static List<List<String>> GetBuiltInParameterName(Revit.Elements.Element Element, List<String> Names)
        {
            var NameList = new List<List<String>>();
            var builtInNames = new HashSet<String>();
            foreach (var i in System.Enum.GetValues(typeof(BuiltInParameter)))
            {
                var sub = new List<String>();
                foreach (var p in Names)
                {

                    if (Element.InternalElement.get_Parameter((BuiltInParameter)i) != null && Element.InternalElement.get_Parameter((BuiltInParameter)i).Definition.Name == p)
                    {
                        sub.Add(p); sub.Add(i.ToString());
                        NameList.Add(sub);
                    }
                }
            }

            return NameList;
        }
    }

    /// <summary>
    /// Quasar LinkDocumentUtility contains link documents
    /// operation nodes
    /// </summary>
    public static class LinkDocumentUtility
    {

        /// <summary>
        /// Collect elements by category and document.
        /// </summary>
        /// <param name="Category">Category object</param>
        /// <param name="Document">Document object</param>
        /// <returns name = "Element">List of elements</returns>
        [IsVisibleInDynamoLibrary(true)]
        public static List<Revit.Elements.Element> GetElementFromLinkDocument(Revit.Elements.Category Category, Autodesk.Revit.DB.Document Document)
        {
            var cate = (BuiltInCategory)Enum.ToObject(typeof(BuiltInCategory), Category.Id);
            var filter = new ElementCategoryFilter(cate);
            var Element = new FilteredElementCollector(Document).WherePasses(filter).WhereElementIsNotElementType().ToElements().Select(x => x.ToDSType(true)).ToList();
            return Element;
        }


    }
    /// <summary>
    /// Quasar WallElements contains revit wall element utility nodes
    /// </summary>

    public static class WallElements
    {
        /// <summary>
        /// Create WallSweep by wall.
        /// </summary>
        /// <param name="Walls">Wall Elements</param>
        /// <param name="TypeElement">Wall sweep type element</param>
        /// <param name="SweepOrReveal">String value "Sweep" or "Reveal"</param>
        /// <param name="IsVertical">Is vertical true or false</param>
        /// <param name="Offset">distance from wall base</param>
        /// <returns name="WallSweeps">WallSweep Elements</returns>

        [IsVisibleInDynamoLibrary(true)]
        public static List<Revit.Elements.Element> CreateWallSweep(List<Revit.Elements.Wall> Walls, Revit.Elements.Element TypeElement, string SweepOrReveal, bool IsVertical, double Offset = 1000)
        {
            var doc = DocumentManager.Instance.CurrentDBDocument;
            var WallSweeps = new List<Revit.Elements.Element>();
            var wallSweepTypes = new Dictionary<string, Autodesk.Revit.DB.WallSweepType>
            {
                { "Sweep", WallSweepType.Sweep },
                { "Reveal", WallSweepType.Reveal }
            };
            var wallSweepTypeId = TypeElement.InternalElement.Id;

            WallSweepInfo wallSweepInfo = new WallSweepInfo(wallSweepTypes[SweepOrReveal], IsVertical)
            {
                Distance = Offset / 304.8
            };

            TransactionManager.Instance.EnsureInTransaction(doc);

            foreach (var w in Walls)
            {
                var wall = w.InternalElement as Autodesk.Revit.DB.Wall;
                WallSweep wallSweep = WallSweep.Create(wall, wallSweepTypeId, wallSweepInfo);
                WallSweeps.Add(wallSweep.ToDSType(true));
            }

            TransactionManager.Instance.TransactionTaskDone();
            return WallSweeps;
        }


        /// <summary>
        /// Remove paint from walls.
        /// </summary>
        /// <param name="Walls">Wall Elements</param>
        /// <returns name="WallElements">Wall Elements</returns>
        [IsVisibleInDynamoLibrary(false)]
        public static List<Revit.Elements.Element> WallPaintRemove(List<Revit.Elements.Element> Walls)
        {
            var doc = DocumentManager.Instance.CurrentDBDocument;
            var WallElements = Walls;
            foreach (var wall in Walls)
            {
                var solid = wall.InternalElement.GetGeometryObjectFromReference(new Reference(wall.InternalElement)) as Autodesk.Revit.DB.Solid;

                foreach (Autodesk.Revit.DB.Face face in solid.Faces)
                {
                    doc.RemovePaint(wall.InternalElement.Id, face);
                }
            }

            return WallElements;
        }

    }


    /// <summary>
    /// Quasar RevitNodes Class
    ///
    /// </summary>
    public static class ElementUtility
    {
        
        

        /// <summary>
        /// Get parameter from first input and Set this value to second input
        /// </summary>
        /// <param name="FirstElements">Element to get</param>
        /// <param name="SecondElements">Element to set</param>
        /// <param name="ParamNames">Parameter names</param>
        /// <returns>Parameter value and boolean(true is set , false can't set)</returns>
        [IsVisibleInDynamoLibrary(true)]
        public static object GetAndSetParams(List<Revit.Elements.Element> FirstElements, List<Revit.Elements.Element> SecondElements, List<string> ParamNames)
        {
            var Result = new List<object>();
            var doc = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(doc);
            foreach (var i in FirstElements.Zip(SecondElements, Tuple.Create))
            {
                foreach (var j in ParamNames)
                {
                    var value = i.Item1.InternalElement.LookupParameter(j).AsString().ToString();
                    var assign = i.Item2.InternalElement.LookupParameter(j).Set(value).ToString();
                    var subList = new List<string>
                    {
                        value,
                        assign
                    };
                    Result.Add(subList);
                }
            }
            TransactionManager.Instance.TransactionTaskDone();
            return Result;
        }

       
        /// <summary>
        /// Get Type Element of input element.
        /// </summary>
        /// <param name="Elements">Element input</param>
        /// <returns name="ElementTypes">Return Type Element of input Element</returns>
        [IsVisibleInDynamoLibrary(true)]
        public static List<Revit.Elements.Element> GetElementType(List<Revit.Elements.Element> Elements)
        {
            var ElementTypes = new List<Revit.Elements.Element>();
            var doc = DocumentManager.Instance.CurrentDBDocument;
            foreach (var elem in Elements)
            {
                var id = elem.InternalElement.GetTypeId();
                var typeElement = doc.GetElement(id);
                ElementTypes.Add(typeElement.ToDSType(true));
            }
            return ElementTypes;
        }
        

    }
    /// <summary>
    /// Quasar RoomUtility class contains room operation nodes
    /// </summary>
    
    public static class RoomUtility
    {
        /// <summary>
        /// Create revit massings by room elements.
        /// </summary>
        /// <param name="Rooms"></param>
        /// <returns name="Massings">Mass elements</returns>
        [IsVisibleInDynamoLibrary(true)]
        public static List<Revit.Elements.Element> MassingsByRooms(List<Revit.Elements.Element> Rooms)
        {
            var doc = DocumentManager.Instance.CurrentDBDocument;
            var MassBICObj = (BuiltInParameter)System.Enum.ToObject(typeof(BuiltInCategory), BuiltInCategory.OST_Mass);
            var objId = new ElementId(MassBICObj);
            TransactionManager.Instance.EnsureInTransaction(doc);
            var Massings = new List<Revit.Elements.Element>();
            foreach (var room in Rooms)
            {
                var name = room.InternalElement.LookupParameter("Name").AsString();
                var number = room.InternalElement.LookupParameter("Number").AsString();

                var elem_name = number + "_" + name;
                var geoobj = room.InternalElement.get_Geometry(new Options());
                var get_enum = geoobj.GetEnumerator();
                var next = get_enum.MoveNext();
                var shape = get_enum.Current;
                var shape_list = new List<GeometryObject>
                {
                    shape
                };
                var shapeType = DirectShapeType.Create(doc, elem_name, objId);
                shapeType.SetShape(shape_list);
                var lib = DirectShapeLibrary.GetDirectShapeLibrary(doc);
                lib.AddDefinitionType(elem_name, shapeType.Id);
                var element = Autodesk.Revit.DB.DirectShape.CreateElementInstance(doc, shapeType.Id, objId, elem_name, Transform.Identity);
                element.SetTypeId(shapeType.Id);

                Massings.Add(element.ToDSType(true));

            }

            TransactionManager.Instance.TransactionTaskDone();
            return Massings;

        }
        /// <summary>
        /// Relocate room element to center of the room.
        /// </summary>
        /// <param name="Rooms">list of rooms</param>
        /// <returns name="RoomElemets">room elements</returns>
        [IsVisibleInDynamoLibrary(true)]
        public static List<Revit.Elements.Element> CenterRoom(List<Revit.Elements.Element> Rooms)
        {
            var RoomElements = Rooms;
            var doc = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(doc);
            foreach(var room in Rooms)
            {
                var level = (Autodesk.Revit.DB.Level)doc.GetElement(room.InternalElement.LevelId);
                var elevation = level.Elevation * 304.8;
                var geoobj = room.InternalElement.get_Geometry(new Options());
                var get_enum = geoobj.GetEnumerator();
                var next = get_enum.MoveNext();
                var shape = (Autodesk.Revit.DB.Solid)get_enum.Current;
                var room_geometry = shape.ToProtoType(true);
                var point = room_geometry.Centroid();
                var center = Autodesk.DesignScript.Geometry.Point.ByCoordinates(point.X, point.Y, elevation);
                var current_loc = (LocationPoint)room.InternalElement.Location;
                var current_point = current_loc.Point;

                var new_loc = center.ToXyz() - current_point;
                room.InternalElement.Location.Move(new_loc);
            
            }

            TransactionManager.Instance.TransactionTaskDone();
            return RoomElements;
        }


    }

    /// <summary>
    /// This class contains view related nodes.
    /// </summary>
    public static class ViewUtility
    {
        /// <summary>
        /// Current Document Active View
        /// </summary>
        /// <returns name = "ActiveView">Return ActiveView</returns>
        [IsVisibleInDynamoLibrary(true)]
        public static Revit.Elements.Element ActiveView()
        {
            var ActiveView = DocumentManager.Instance.CurrentDBDocument.ActiveView.ToDSType(true);
            return ActiveView;
        }

        /// <summary>
        /// Create floor plan views by rooms, names and offset.
        /// </summary>
        /// <param name="Level">Level element</param>
        /// <param name="Rooms"></param>
        /// <param name="Names">List of names for new views</param>
        /// <param name="Offset">Cropbox offset from room</param>
        /// <returns name="FloorPlanView">Created Ceiling Views</returns>
        public static List<Revit.Elements.Element> FloorPlanViewByRoom(Revit.Elements.Element Level, List<Revit.Elements.Room> Rooms, List<String> Names, double Offset = 500)
        {
            var FloorPlanView = new List<Revit.Elements.Element>();
            var doc = DocumentManager.Instance.CurrentDBDocument;
            var fViews = new FilteredElementCollector(doc).OfClass(typeof(View)).Cast<View>().Where(x => x.ViewType == ViewType.FloorPlan).ToList();
            var fview = from c in fViews where c.LookupParameter("Associated Level").AsString() == Level.Name.ToString() select c;
            var view = fview.First();
            TransactionManager.Instance.EnsureInTransaction(doc);
            foreach (var elem in Rooms.Zip(Names, Tuple.Create))
            {
                var v = view.Duplicate(ViewDuplicateOption.WithDetailing);
                BoundingBoxXYZ bbox = elem.Item1.InternalElement.get_BoundingBox(doc.ActiveView);
                var newbbox = Utility.Crop_box(bbox, Offset / 304.8);
                var dupview = (Autodesk.Revit.DB.View)doc.GetElement(v);
                dupview.Name = elem.Item2;
                dupview.CropBox = newbbox;
                dupview.CropBoxActive = true;
                dupview.CropBoxVisible = true;
                dupview.Scale = view.Scale;
                FloorPlanView.Add(dupview.ToDSType(true));

            }
            TransactionManager.Instance.TransactionTaskDone();

            return FloorPlanView;
        }

        /// <summary>
        /// Create 3D Views for given room.
        /// </summary>
        /// <param name="Rooms">Rooms elements</param>
        /// <param name="Names">Name for new views</param>
        /// <param name="Offset">Offset value for section box. Default is 500.</param>
        /// <returns name="ThreeDView">New 3D Views</returns>
        [IsVisibleInDynamoLibrary(true)]
        public static List<Revit.Elements.Element> ThreeDViewByRoom(List<Revit.Elements.Room> Rooms, List<String> Names, double Offset = 500)
        {
            var ThreeDViews = new List<Revit.Elements.Element>();
            var doc = DocumentManager.Instance.CurrentDBDocument;
            var vtype = new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>().FirstOrDefault(a => a.ViewFamily == ViewFamily.ThreeDimensional);
            TransactionManager.Instance.EnsureInTransaction(doc);
            foreach (var elem in Rooms.Zip(Names, Tuple.Create))
            {
                BoundingBoxXYZ bbox = elem.Item1.InternalElement.get_BoundingBox(doc.ActiveView);
                var newbbox = Utility.Crop_box(bbox, Offset / 304.8);
                View3D ThreeDView = View3D.CreateIsometric(doc, vtype.Id);
                ThreeDView.Name = elem.Item2;
                ThreeDView.SetSectionBox(newbbox);
                ThreeDView.CropBoxActive = true;
                ThreeDView.CropBoxVisible = true;
                ThreeDView.Scale = 50;
                ThreeDViews.Add(ThreeDView.ToDSType(true));

            }
            TransactionManager.Instance.TransactionTaskDone();

            return ThreeDViews;
        }

        /// <summary>
        /// Create ceiling views by rooms, names and offset.
        /// </summary>
        /// <param name="Level">Level element</param>
        /// <param name="Rooms"></param>
        /// <param name="Names">List of names for new views</param>
        /// <param name="Offset">Cropbox offset from room</param>
        /// <returns name="CeilingView">Created Ceiling Views</returns>
        public static List<Revit.Elements.Element> CeilingViewByRoom(Revit.Elements.Element Level, List<Revit.Elements.Room> Rooms, List<String> Names, double Offset = 500)
        {
            var CeilingView = new List<Revit.Elements.Element>();
            var doc = DocumentManager.Instance.CurrentDBDocument;
            var CViews = new FilteredElementCollector(doc).OfClass(typeof(View)).Cast<View>().Where(x => x.ViewType == ViewType.CeilingPlan).ToList();
            var ceiling = from c in CViews where c.LookupParameter("Associated Level").AsString() == Level.Name.ToString() select c;
            var view = ceiling.First();
            TransactionManager.Instance.EnsureInTransaction(doc);
            foreach (var elem in Rooms.Zip(Names, Tuple.Create))
            {
                var v = view.Duplicate(ViewDuplicateOption.WithDetailing);
                BoundingBoxXYZ bbox = elem.Item1.InternalElement.get_BoundingBox(doc.ActiveView);
                var newbbox = Utility.Crop_box(bbox, Offset / 304.8);
                var dupview = (Autodesk.Revit.DB.View)doc.GetElement(v);
                dupview.Name = elem.Item2;
                dupview.CropBox = newbbox;
                dupview.CropBoxActive = true;
                dupview.CropBoxVisible = true;
                dupview.Scale = view.Scale;
                CeilingView.Add(dupview.ToDSType(true));

            }
            TransactionManager.Instance.TransactionTaskDone();

            return CeilingView;
        }

        /// <summary>
        /// Create elevation views in room with crop offset by rooms , floorplan and offset.
        /// Default naming is - "RoomName_RoomNumber - A", "RoomName_RoomNumber - B",
        /// "RoomName_RoomNumber - C", "RoomName_RoomNumber - D".
        /// </summary>
        /// <param name="Rooms">Room elements and make sure all room are bounding</param>
        /// <param name="FloorPlan">Floor plan view</param>
        /// <param name="Offset">Offset from room , default is 500</param>
        /// <returns name="ElevationView"> New Elevation Views</returns>
        [IsVisibleInDynamoLibrary(true)]
        public static List<List<Revit.Elements.Element>> ElevationInRoom(List<Revit.Elements.Room> Rooms, Revit.Elements.Element FloorPlan, double Offset = 500)
        {
            var doc = DocumentManager.Instance.CurrentDBDocument;
            var vtype = new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>().FirstOrDefault(a => a.ViewFamily == ViewFamily.Elevation);
            var ElevationView = new List<List<Revit.Elements.Element>>();
            TransactionManager.Instance.EnsureInTransaction(doc);
            foreach (var r in Rooms)
            {
                var list = new List<Revit.Elements.Element>();
                var elevViews = new List<Revit.Elements.Element>();
                String rname = r.InternalElement.LookupParameter("Number").AsString() + "_" + r.InternalElement.LookupParameter("Name").AsString();

                LocationPoint elevPoint = (Autodesk.Revit.DB.LocationPoint)r.InternalElement.Location;
                XYZ point = elevPoint.Point;
                BoundingBoxXYZ bbox = r.InternalElement.get_BoundingBox(doc.ActiveView);
                ElevationMarker marker = Autodesk.Revit.DB.ElevationMarker.CreateElevationMarker(doc, vtype.Id, point, 50);

                BoundingBoxXYZ bcrop = Utility.Crop_box(bbox, Offset / 304.8);
                var surfaces = bcrop.ToProtoType(true).ToPolySurface().Surfaces().Skip(2).Take(4);

                var westElev = marker.CreateElevation(doc, FloorPlan.InternalElement.Id, 0);
                westElev.Name = rname + " - A";
                var westcrsm = westElev.GetCropRegionShapeManager();
                var west = surfaces.ElementAt(0).PerimeterCurves();
                var westcurve = new List<Autodesk.Revit.DB.Curve>();
                foreach (var w in west) { westcurve.Add(w.ToRevitType(true)); }
                CurveLoop wcloop = CurveLoop.Create(westcurve);
                westcrsm.SetCropShape(wcloop);

                var northElev = marker.CreateElevation(doc, FloorPlan.InternalElement.Id, 1);
                northElev.Name = rname + " - B";
                var northcrsm = northElev.GetCropRegionShapeManager();
                var north = surfaces.ElementAt(1).PerimeterCurves();
                var northcurve = new List<Autodesk.Revit.DB.Curve>();
                foreach (var w in north) { northcurve.Add(w.ToRevitType(true)); }
                CurveLoop ncloop = CurveLoop.Create(northcurve);
                northcrsm.SetCropShape(ncloop);

                var eastElev = marker.CreateElevation(doc, FloorPlan.InternalElement.Id, 2);
                eastElev.Name = rname + " - C";
                var eastcrsm = eastElev.GetCropRegionShapeManager();
                var east = surfaces.ElementAt(2).PerimeterCurves();
                var eastcurve = new List<Autodesk.Revit.DB.Curve>();
                foreach (var w in east) { eastcurve.Add(w.ToRevitType(true)); }
                CurveLoop ecloop = CurveLoop.Create(eastcurve);
                eastcrsm.SetCropShape(ecloop);

                var southElev = marker.CreateElevation(doc, FloorPlan.InternalElement.Id, 3);
                southElev.Name = rname + " - D";
                var southcrsm = southElev.GetCropRegionShapeManager();
                var south = surfaces.ElementAt(3).PerimeterCurves();
                var southcurve = new List<Autodesk.Revit.DB.Curve>();
                foreach (var w in south) { southcurve.Add(w.ToRevitType(true)); }
                CurveLoop scloop = CurveLoop.Create(southcurve);
                southcrsm.SetCropShape(scloop);

                list.Add(westElev.ToDSType(true));
                list.Add(northElev.ToDSType(true));
                list.Add(eastElev.ToDSType(true));
                list.Add(southElev.ToDSType(true));

                ElevationView.Add(list);
            }
            TransactionManager.Instance.TransactionTaskDone();
            return ElevationView;
        }

        /// <summary>
        /// Transfer View Templates from a link document to current document 
        /// with or without associate filters(including override settings). Default include filters.
        /// </summary>
        /// <param name="LinkDocument"> A Link document which includes view templates</param>
        /// <param name="IsIncludeFilters"> If true, filters and settings will include with view template.
        /// If false, filters and settings will not include, only view templates will tranfer.
        /// default value true.</param>
        /// <returns name="TemplateNames">Created template name list.</returns>
        [IsVisibleInDynamoLibrary(true)]
        public static List<string> TransferViewTemplateAndFilter(Document LinkDocument, bool IsIncludeFilters = true)
        {
            var TemplateNames = new List<string>();
            var doc = DocumentManager.Instance.CurrentDBDocument;
            var views = new FilteredElementCollector(LinkDocument).OfClass(typeof(View)).Cast<View>().Where(x => x.IsTemplate).ToList();
            var ids = new List<ElementId>();
            foreach (var view in views) { ids.Add(view.Id); }
            TransactionManager.Instance.EnsureInTransaction(doc);

            var templates = ElementTransformUtils.CopyElements(LinkDocument, ids, doc, Transform.Identity, new CopyPasteOptions());
            foreach (var i in templates) { TemplateNames.Add(doc.GetElement(i).Name); }

            if (!IsIncludeFilters)
            {
                foreach (ElementId v in templates)
                {
                    View view = doc.GetElement(v) as View;
                    var filters = view.GetFilters();
                    foreach (ElementId f in filters)
                    {
                        view.RemoveFilter(f);
                    }

                }
            }

            TransactionManager.Instance.TransactionTaskDone();
            return TemplateNames;


        }

    }

}
