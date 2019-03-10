using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;


namespace Quasar
{
    /// <summary>
    /// Class contains dynamo nodes.
    /// </summary>
    public static class DynamoNodes
    {
        /// <summary>
        /// Surface and U, V divisions 
        /// </summary>
        /// <param name="Surface">Surface</param>
        /// <param name="Udivision">Number of division</param>
        /// <param name="Vdivision">Number of division</param>
        /// <returns>Returns Quad Panels and Polygons</returns>
        [IsVisibleInDynamoLibrary(true)]
        [MultiReturn(new[] { "Panels","Polygons"})]
        public static Dictionary<string,object> QuadPanel(Surface Surface, double Udivision, double Vdivision)
        {
            
            var panels = new List<Surface>();
            var polygons = new List<Polygon>();

            for (var i = 0; i < Udivision; i++)
            {
                for (var j = 0; j < Vdivision; j++)
                {
                    var points = new List<Point>();

                    var ustep = 1.0 / Udivision;
                    var vstep = 1.0 / Vdivision;

                    var pA = Surface.PointAtParameter(i * ustep, j * vstep);
                    var pB = Surface.PointAtParameter((i + 1) * ustep, j * vstep);
                    var pC = Surface.PointAtParameter((i + 1) * ustep, (j + 1) * vstep);
                    var pD = Surface.PointAtParameter(i * ustep, (j + 1) * vstep);

                    points.Add(pA);
                    points.Add(pB);
                    points.Add(pC);
                    points.Add(pD);

                    panels.Add(Surface.ByPerimeterPoints(points));
                    polygons.Add(Polygon.ByPoints(points));

                    pA.Dispose();
                    pB.Dispose();
                    pC.Dispose();
                    pD.Dispose();
                }              
            }

            return new Dictionary<string, object> { { "Panels", panels }, { "Polygons", polygons } };

        }
    }
}
