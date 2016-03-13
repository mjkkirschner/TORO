using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using WireFrameToRobot;

namespace Dynamo_TORO
{
    /// <summary>
    /// Container for RobArch workflow
    /// </summary>
    public class WIP_ROBARCH
    {



        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////



        /// <summary>
        /// Create geometry representation of tool.
        /// </summary>
        /// <param name="frame">Tool frame at drill tip</param>
        /// <returns></returns>
        private static Solid vis_tool(Plane frame)
        {
            List<Solid> model = new List<Solid>();
            CoordinateSystem cs = CoordinateSystem.ByPlane(frame);
            Vector x = frame.XAxis;
            Vector y = frame.YAxis;
            Vector z = frame.Normal.Reverse();
            double h = frame.Origin.Z;

            Solid bit0 = Cone.ByCoordinateSystemHeightRadii(cs.Translate(z, 2), 2, 3, 0.1);
            Solid bit1 = Cone.ByCoordinateSystemHeightRadii(cs.Translate(z, 68 + 2), 68, 3, 3);
            Solid bit2 = Cone.ByCoordinateSystemHeightRadii(cs.Translate(z, 5 + 68 + 2), 5, 5, 5);
            Solid bit3 = Cone.ByCoordinateSystemHeightRadii(cs.Translate(z, 5 + 5 + 68 + 2), 5, 4, 4);
            Solid bod0 = Cone.ByCoordinateSystemHeightRadii(cs.Translate(z, 3 + 5 + 5 + 68 + 2), 3, 12, 12);
            Solid bod1 = Cone.ByCoordinateSystemHeightRadii(cs.Translate(z, 80 + 3 + 5 + 5 + 68 + 2), 80, 24, 24);
            Solid bod2 = Cone.ByCoordinateSystemHeightRadii(cs.Translate(z, 80 + 80 + 3 + 5 + 5 + 68 + 2), 80, 34, 34);
            Solid bod3 = Cuboid.ByLengths(cs.Translate(z, 40 + 80 + 3 + 5 + 5 + 68 + 2).Translate(y, 20), 40, 40, 40);
            Solid leg0 = Cone.ByCoordinateSystemHeightRadii(cs.Translate(z, h).Translate(x, 40).Translate(y, 40), h - (80 + 3 + 5 + 5 + 68 + 2), 4, 4);
            Solid leg1 = Cone.ByCoordinateSystemHeightRadii(cs.Translate(z, h).Translate(x, 40).Translate(y, -40), h - (80 + 3 + 5 + 5 + 68 + 2), 4, 4);
            Solid leg2 = Cone.ByCoordinateSystemHeightRadii(cs.Translate(z, h).Translate(x, -40).Translate(y, -40), h - (80 + 3 + 5 + 5 + 68 + 2), 4, 4);
            Solid leg3 = Cone.ByCoordinateSystemHeightRadii(cs.Translate(z, h).Translate(x, -40).Translate(y, 40), h - (80 + 3 + 5 + 5 + 68 + 2), 4, 4);
            Solid box0 = Cuboid.ByLengths(cs.Translate(z, 3 + 80 + 3 + 5 + 5 + 68 + 2), 96, 96, 10);
            Solid box1 = Cuboid.ByLengths(cs.Translate(z, h - 5), 96, 96, 10);
            
            model.Add(bit0);
            model.Add(bit1);
            model.Add(bit2);
            model.Add(bit3);
            model.Add(bod0);
            model.Add(bod1);
            model.Add(bod2);
            model.Add(bod3);
            model.Add(leg0);
            model.Add(leg1);
            model.Add(leg2);
            model.Add(leg3);
            model.Add(box0);
            model.Add(box1);

            Solid solid = Solid.ByUnion(model);
            return solid;
        }



        /// <summary>
        /// Create geometry representation of wobj.
        /// </summary>
        /// <param name="frame">Wobj frame at cube centroid</param>
        /// <returns></returns>
        private static Solid vis_wobj(Plane frame)
        {
            List<Solid> model = new List<Solid>();
            CoordinateSystem cs = CoordinateSystem.ByPlane(frame);
            Vector z = frame.Normal.Reverse();
            double h = frame.Origin.Z;

            //Solid cube = Cuboid.ByLengths(cs, 40, 40, 40);
            Solid chu0 = Cylinder.ByCoordinateSystemHeightRadii(cs.Translate(cs.ZAxis.Reverse(), 2 + 20), 2, 10, 10);
            Solid bod0 = Cylinder.ByCoordinateSystemHeightRadii(cs.Translate(cs.ZAxis.Reverse(), 18 + 2 + 20), 18, 22, 22);
            Solid bod1 = Cuboid.ByLengths(cs.Translate(z, h - 18 - (h - 18 - 18 - 2 - 20) / 2), 46, 46, h - 18 - 18 - 2 - 20);
            Solid bod2 = Cylinder.ByCoordinateSystemHeightRadii(cs.Translate(z, h), 18, 22, 22);

            //model.Add(cube);
            model.Add(chu0);
            model.Add(bod0);
            model.Add(bod1);
            model.Add(bod2);

            Solid solid = Solid.ByUnion(model);
            return solid;
        }



        // NOT WORKING
        // NOT WORKING
        // NOT WORKING
        // NOT WORKING
        // NOT WORKING

        /// <summary>
        /// Create geometry representation of targets using wobj and tool.
        /// </summary>
        /// <param name="holeFrames">List of target frames</param>
        /// <param name="blockFrame">Wobj frame</param>
        /// <param name="drillFrame">Tool frame</param>
        /// <param name="nodeIndex">Index of node</param>
        /// <param name="poseIndex">Index of pose</param>
        /// <returns></returns>
        [MultiReturn(new[] { "tool", "wobj", "block", "holes" })]
        public static Dictionary<string, List<Object>> vis_transform2(List<List<Plane>> holeFrames, Plane blockFrame, Plane drillFrame, int nodeIndex = 0, int poseIndex = 0)
        {

            // setup
            List<Object> outputTool = new List<Object>();
            List<Object> outputWobj = new List<Object>();
            List<Object> outputBloc = new List<Object>();
            List<Object> outputCSysHoles = new List<Object>();

            // create wobj and tool solids
            Solid wobj = vis_wobj(blockFrame);
            Solid tool = vis_tool(drillFrame);
            CoordinateSystem worldCS = CoordinateSystem.ByOrigin(0, 0, 0);
            CoordinateSystem toolCS = CoordinateSystem.ByPlane(drillFrame);

            // transform and translate targets on wobj to tool
            List<Object> holeCSList = new List<Object>();
            foreach (Plane hole in holeFrames[nodeIndex])
            {
                CoordinateSystem holeCS = CoordinateSystem.ByPlane(hole);
                CoordinateSystem holeCSTransformed = holeCS.Transform(worldCS, toolCS);
                CoordinateSystem holeCSTransformedTranslated = holeCSTransformed.Translate(holeCSTransformed.ZAxis.Reverse(), 10);
                holeCSList.Add((Object) holeCSTransformedTranslated);
            }

            // transform wobj to target in tool space
            CoordinateSystem wobjCS = CoordinateSystem.ByPlane(blockFrame);
            CoordinateSystem wobjCSTransformed = wobjCS.Transform(wobjCS, (CoordinateSystem) holeCSList[poseIndex]);
            Geometry wobjTransformed = wobj.Transform(wobjCS, wobjCSTransformed);
            
            // create wireframe model of block
            Edge[] blocTransformed = (Cuboid.ByLengths(wobjCSTransformed, 40, 40, 40).Edges);
            foreach (Edge edge in blocTransformed) { outputBloc.Add(edge.CurveGeometry); }

            // output
            outputTool.Add((Object)tool);
            outputWobj.Add((Object)wobjTransformed);
            return new Dictionary<string, List<Object>>
            {
                { "tool", outputTool },
                { "wobj", outputWobj },
                { "block", outputBloc },
                { "holes", holeCSList }
            };
        }

        // NOT WORKING
        // NOT WORKING
        // NOT WORKING
        // NOT WORKING
        // NOT WORKING



        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Sort points by directionality about arbitrary pole.
        /// </summary>
        /// <param name="pointList"></param>
        /// <returns></returns>
        [MultiReturn(new[] { "sorted", "indices" })]
        public static Dictionary<string, List<Object>> sortPolar1_Point(List<Point> pointList)
        {
            List<double> angList = new List<double>();
            List<Object> newList = new List<Object>();

            foreach (Point p in pointList)
            {
                double x = p.X;
                double y = p.Y;
                double d = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
                double xnorm = x / d;
                double ynorm = y / d;
                double t = Math.Atan2(ynorm, xnorm);
                if (y < 0) { t = t - 180; }
                angList.Add(t);
            }

            var sortedAng = angList
                .Select((x, i) => new KeyValuePair<int, int>((int)x, i))
                .OrderBy(x => x.Key)
                .ToList();

            List<Object> indices = sortedAng.Select(x => (Object)x.Value).ToList();
            foreach (int i in indices)
            {
                newList.Add(pointList[i]);
            }
            return new Dictionary<string, List<Object>>
            {
                {"sorted", newList},
                {"indices", indices}
            };
        }

        
        
        /// <summary>
        /// Sort vectors by directionality about arbitrary pole.
        /// </summary>
        /// <param name="vecList"></param>
        /// <returns></returns>
        [MultiReturn(new[] { "sorted", "indices" })]
        public static Dictionary<string, List<Object>> sortPolar1_Vector(List<Vector> vecList)
        {
            List<double> angList = new List<double>();
            List<Object> newList = new List<Object>();

            foreach (Vector v in vecList)
            {
                double x = v.X;
                double y = v.Y;
                double d = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
                double xnorm = x / d;
                double ynorm = y / d;
                double t = Math.Atan2(ynorm, xnorm);
                if (y < 0) { t = t - 180; }
                angList.Add(t);
            }

            var sortedAng = angList
                .Select((x, i) => new KeyValuePair<int, int>((int)x, i))
                .OrderBy(x => x.Key)
                .ToList();

            List<Object> indices = sortedAng.Select(x => (Object) x.Value).ToList();
            foreach (int i in indices)
            {
                newList.Add(vecList[i]);
            }
            return new Dictionary<string, List<Object>>
            {
                {"sorted", newList},
                {"indices", indices}
            };
        }



        /// <summary>
        /// Sort planes directionality about arbitrary pole.
        /// </summary>
        /// <param name="planeList">List of Planes</param>
        /// <returns></returns>
        [MultiReturn(new[] { "sorted", "indices" })]
        public static Dictionary<string, List<Object>> sortPolar1_Plane(List<Plane> planeList)
        {
            List<double> angList = new List<double>();
            List<Object> newList = new List<Object>();

            foreach (Plane p in planeList)
            {
                double x = p.Normal.X;
                double y = p.Normal.Y;
                double d = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
                double xnorm = x / d;
                double ynorm = y / d;
                double t = Math.Atan2(ynorm, xnorm);
                if (y < 0) { t = t - 180; }
                angList.Add(t);
            }

            var sortedAng = angList
                .Select((x, i) => new KeyValuePair<int, int>((int)x, i))
                .OrderBy(x => x.Key)
                .ToList();

            List<Object> indices = sortedAng.Select(x => (Object) x.Value).ToList();
            foreach (int i in indices)
            {
                newList.Add(planeList[i]);
            }
            return new Dictionary<string, List<Object>>
            {
                {"sorted", newList},
                {"indices", indices},
            };
        }



        /// <summary>
        /// Sort coordinate systems directionality about arbitrary pole.
        /// </summary>
        /// <param name="csList">List of coordinate systems</param>
        /// <returns></returns>
        [MultiReturn(new[] { "sorted", "indices" })]
        public static Dictionary<string, List<Object>> sortPolar1_CoordSys(List<CoordinateSystem> csList)
        {
            List<double> angList = new List<double>();
            List<Object> newList = new List<Object>();
            
            foreach (CoordinateSystem cs in csList)
            {
                double x = cs.ZAxis.X;
                double y = cs.ZAxis.Y;
                double d = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
                double xnorm = x / d;
                double ynorm = y / d;
                double t = Math.Atan2(ynorm, xnorm);
                if (y < 0) { t = t - 180; }

                angList.Add(t);
            }

            var sortedAng = angList
                .Select((x, i) => new KeyValuePair<int, int>((int) x, i))
                .OrderBy(x => x.Key)
                .ToList();

            List<Object> indices = sortedAng.Select(x => (Object) x.Value).ToList();
            foreach (int i in indices)
            {
                newList.Add(csList[i]);
            }
            return new Dictionary<string, List<Object>>
            {
                {"sorted", newList},
                {"indices", indices}
            };
        }



        /// <summary>
        /// Sort points by directionality about average pole and shift.
        /// </summary>
        /// <param name="pointList">List of points</param>
        /// <param name="shift">Shift value</param>
        /// <returns></returns>
        [MultiReturn(new[] { "sorted", "indices" })]
        public static Dictionary<string, List<Object>> sortPolar2_Vector(List<Point> pointList, int shift = 0)
        {
            List<Object> newList = new List<Object>();
            List<double> paramList = new List<double>();

            List<double> vx = pointList.Select(p => p.X).ToList();
            List<double> vy = pointList.Select(p => p.Y).ToList();
            List<double> vz = pointList.Select(p => p.Z).ToList();

            Vector vecAvg = Vector.ByCoordinates(vx.Average(), vy.Average(), vz.Average());
            Circle guide = Circle.ByCenterPointRadiusNormal(Point.ByCoordinates(0, 0, 0), 1, vecAvg);
            for (int i = 0; i < pointList.Count(); i++)
            {
                Point v = Point.ByCoordinates(vx[i], vy[i], vz[i]);
                double param = guide.ParameterAtPoint(guide.ClosestPointTo(v));
                paramList.Add(param);
            }

            var sortedParams = paramList
                .Select((x, i) => new KeyValuePair<int, int>((int)x, i))
                .OrderBy(x => x.Key)
                .ToList();

            List<Object> indices = sortedParams.Select(x => (Object)x.Value).ToList();
            foreach (int i in indices) { newList.Add(pointList[i]); }

            List<Object> shifted = new List<Object>();
            if (shift < 0) { shift = newList.Count - shift % newList.Count - 1; }
            if (Math.Abs(shift) >= newList.Count) { shift = shift % newList.Count; }
            shifted = newList.GetRange(shift, newList.Count - shift);
            shifted.AddRange(newList.GetRange(0, shift));

            return new Dictionary<string, List<Object>>
            {
                {"sorted", shifted},
                {"indices", indices},
            };
        }



        /// <summary>
        /// Sort vectors by directionality about average pole and shift.
        /// </summary>
        /// <param name="vecList">List of vectors</param>
        /// <param name="shift">Shift value</param>
        /// <returns></returns>
        [MultiReturn(new[] { "sorted", "indices" })]
        public static Dictionary<string, List<Object>> sortPolar2_Vector(List<Vector> vecList, int shift = 0)
        {
            List<Object> newList = new List<Object>();
            List<double> paramList = new List<double>();

            List<double> vx = vecList.Select(p => p.X).ToList();
            List<double> vy = vecList.Select(p => p.Y).ToList();
            List<double> vz = vecList.Select(p => p.Z).ToList();

            Vector vecAvg = Vector.ByCoordinates(vx.Average(), vy.Average(), vz.Average());
            Circle guide = Circle.ByCenterPointRadiusNormal(Point.ByCoordinates(0, 0, 0), 1, vecAvg);
            for (int i = 0; i < vecList.Count(); i++)
            {
                Point v = Point.ByCoordinates(vx[i], vy[i], vz[i]);
                double param = guide.ParameterAtPoint(guide.ClosestPointTo(v));
                paramList.Add(param);
            }

            var sortedParams = paramList
                .Select((x, i) => new KeyValuePair<int, int>((int)x, i))
                .OrderBy(x => x.Key)
                .ToList();

            List<Object> indices = sortedParams.Select(x => (Object)x.Value).ToList();
            foreach (int i in indices) { newList.Add(vecList[i]); }

            List<Object> shifted = new List<Object>();
            if (shift < 0) { shift = newList.Count - shift % newList.Count - 1; }
            if (Math.Abs(shift) >= newList.Count) { shift = shift % newList.Count; }
            shifted = newList.GetRange(shift, newList.Count - shift);
            shifted.AddRange(newList.GetRange(0, shift));

            return new Dictionary<string, List<Object>>
            {
                {"sorted", shifted},
                {"indices", indices},
            };
        }



        /// <summary>
        /// Sort planes by directionality about average pole and shift.
        /// </summary>
        /// <param name="planeList">List of planes</param>
        /// <param name="shift">Shift value</param>
        /// <returns></returns>
        [MultiReturn(new[] { "sorted", "indices" })]
        public static Dictionary<string, List<Object>> sortPolar2_Plane(List<Plane> planeList, int shift = 0)
        {
            List<Object> newList = new List<Object>();
            List<double> paramList = new List<double>();

            List<double> vx = planeList.Select(p => p.Normal.X).ToList();
            List<double> vy = planeList.Select(p => p.Normal.Y).ToList();
            List<double> vz = planeList.Select(p => p.Normal.Z).ToList();

            Vector vecAvg = Vector.ByCoordinates(vx.Average(), vy.Average(), vz.Average());
            Circle guide = Circle.ByCenterPointRadiusNormal(Point.ByCoordinates(0, 0, 0), 1, vecAvg);
            for (int i = 0; i < planeList.Count(); i++)
            {
                Point v = Point.ByCoordinates(vx[i], vy[i], vz[i]);
                double param = guide.ParameterAtPoint(guide.ClosestPointTo(v));
                paramList.Add(param);
            }

            var sortedParams = paramList
                .Select((x, i) => new KeyValuePair<int, int>((int)x, i))
                .OrderBy(x => x.Key)
                .ToList();

            List<Object> indices = sortedParams.Select(x => (Object)x.Value).ToList();
            foreach (int i in indices) { newList.Add(planeList[i]); }

            List<Object> shifted = new List<Object>();
            if (shift < 0) { shift = newList.Count - shift % newList.Count - 1; }
            if (Math.Abs(shift) >= newList.Count) { shift = shift % newList.Count; }
            shifted = newList.GetRange(shift, newList.Count - shift);
            shifted.AddRange(newList.GetRange(0, shift));

            return new Dictionary<string, List<Object>>
            {
                {"sorted", shifted},
                {"indices", indices},
            };
        }



        /// <summary>
        /// Sort coordinate systems by directionality about average pole and shift.
        /// </summary>
        /// <param name="csList">List of coordinate systems</param>
        /// <param name="shift">Shift value</param>
        /// <returns></returns>
        [MultiReturn(new[] { "sorted", "indices" })]
        public static Dictionary<string, List<Object>> sortPolar2_CoordSys(List<CoordinateSystem> csList, int shift = 0)
        {
            List<Object> newList = new List<Object>();
            List<double> paramList = new List<double>();

            List<double> vx = csList.Select(p => p.ZAxis.X).ToList();
            List<double> vy = csList.Select(p => p.ZAxis.Y).ToList();
            List<double> vz = csList.Select(p => p.ZAxis.Z).ToList();

            Vector vecAvg = Vector.ByCoordinates(vx.Average(), vy.Average(), vz.Average());
            Circle guide = Circle.ByCenterPointRadiusNormal(Point.ByCoordinates(0, 0, 0), 1, vecAvg);
            for (int i = 0; i < csList.Count(); i++)
            {
                Point v = Point.ByCoordinates(vx[i], vy[i], vz[i]);
                double param = guide.ParameterAtPoint(guide.ClosestPointTo(v));
                paramList.Add(param);
            }

            var sortedParams = paramList
                .Select((x, i) => new KeyValuePair<int, int>((int)x, i))
                .OrderBy(x => x.Key)
                .ToList();

            List<Object> indices = sortedParams.Select(x => (Object)x.Value).ToList();
            foreach (int i in indices) { newList.Add(csList[i]); }

            List<Object> shifted = new List<Object>();
            if (shift < 0) { shift = newList.Count - shift % newList.Count - 1; }
            if (Math.Abs(shift) >= newList.Count) { shift = shift % newList.Count; }
            shifted = newList.GetRange(shift, newList.Count - shift);
            shifted.AddRange(newList.GetRange(0, shift));

            return new Dictionary<string, List<Object>>
            {
                {"sorted", shifted},
                {"indices", indices},
            };
        }



        /// <summary>
        /// Shift the contents of a list by value.
        /// </summary>
        /// <param name="lst">List of objects</param>
        /// <param name="shift">Shift value</param>
        /// <returns></returns>
        public static List<Object> shiftList(List<Object> lst, int shift = 0)
        {
            List<Object> shifted = new List<Object>();
            if (shift < 0) { shift = lst.Count - shift % lst.Count - 1; }
            if (Math.Abs(shift) >= lst.Count) { shift = shift % lst.Count; }
            shifted = lst.GetRange(shift, lst.Count - shift);
            shifted.AddRange(lst.GetRange(0, shift));
            return shifted;
        }



        /// <summary>
        /// Get item at index in range 0 to 1.
        /// </summary>
        /// <param name="lst">List of objects</param>
        /// <param name="index">Index value from 0 to 1</param>
        /// <returns></returns>
        public static Object smartGetItem(List<Object> lst, double index = 0)
        {
            if (index == 1) { index = 0; }
            int i = (int) (index * lst.Count());
            Object item = lst[i];
            return item;
        }

        

        /// <summary>
        /// Create a set from only the unique items in a list of points.
        /// </summary>
        /// <param name="points">List of points</param>
        /// <returns></returns>
        [MultiReturn(new[] { "passed", "failed" })]
        public static Dictionary<string, List<Point>> getUnique_Point(List<Point> points)
        {
            List<Point> passed = new List<Point>();
            List<Point> failed = new List<Point>();
            List<double[]> checkList = new List<double[]>();

            foreach (Point p in points)
            {
                double[] check = new double[] { p.X, p.Y, p.Z };
                check = check.Select(val => Math.Round(val, 4)).ToArray();
                if (!checkList.Any(check.SequenceEqual))
                {
                    passed.Add(p);
                    checkList.Add(check);
                }
                else
                {
                    failed.Add(p);
                }
            }

            return new Dictionary<string, List<Point>>
            {
                {"passed", passed},
                {"failed", failed}
            };
        }



        /// <summary>
        /// Create a set from only the unique items in a list of vectors.
        /// </summary>
        /// <param name="vectors">List of vectors</param>
        /// <returns></returns>
        [MultiReturn(new[] { "passed", "failed" })]
        public static Dictionary<string, List<Vector>> getUnique_Vector(List<Vector> vectors)
        {
            List<Vector> passed = new List<Vector>();
            List<Vector> failed = new List<Vector>();
            List<double[]> checkList = new List<double[]>();

            foreach (Vector v in vectors)
            {
                double[] check = new double[] { v.X, v.Y, v.Z };
                check = check.Select(val => Math.Round(val, 4)).ToArray();
                if (!checkList.Any(check.SequenceEqual))
                {
                    passed.Add(v);
                    checkList.Add(check);
                }
                else
                {
                    failed.Add(v);
                }

            }

            return new Dictionary<string, List<Vector>>
            {
                {"passed", passed},
                {"failed", failed}
            };
        }



        /// <summary>
        /// Create a set from only the unique items in a list of planes.
        /// </summary>
        /// <param name="planes">List of planes</param>
        /// <param name="option">Test origin and axes (true) or just axes (false)</param>
        /// <returns></returns>
        [MultiReturn(new[] { "passed", "failed" })]
        public static Dictionary<string, List<Plane>> getUnique_Plane(List<Plane> planes, bool option = false)
        {
            List<Plane> passed = new List<Plane>();
            List<Plane> failed = new List<Plane>();
            List<double[]> checkList = new List<double[]>();

            foreach (Plane p in planes)
            {
                double[] check = new double[] { };
                if (option)
                {
                    check = new double[] { p.Origin.X, p.Origin.Y, p.Origin.Z, p.Normal.X, p.Normal.Y, p.Normal.Z, p.XAxis.X, p.XAxis.Y, p.XAxis.Z };
                }
                else
                {
                    check = new double[] { p.Normal.X, p.Normal.Y, p.Normal.Z, p.XAxis.X, p.XAxis.Y, p.XAxis.Z };
                }
                check = check.Select(val => Math.Round(val, 4)).ToArray();
                if (!checkList.Any(check.SequenceEqual))
                {
                    passed.Add(p);
                    checkList.Add(check);
                }
                else
                {
                    failed.Add(p);
                }
            }

            return new Dictionary<string, List<Plane>>
            {
                {"passed", passed},
                {"failed", failed}
            };
        }



        /// <summary>
        /// Create a set from only the unique items in a list of coordinate systems.
        /// </summary>
        /// <param name="coordSys">List of coordinate systems</param>
        /// <param name="option">Test origin and axes (true) or just axes (false)</param>
        /// <returns></returns>
        [MultiReturn(new[] { "passed", "failed" })]
        public static Dictionary<string, List<CoordinateSystem>> getUnique_CoordSys(List<CoordinateSystem> coordSys, bool option = false)
        {
            List<CoordinateSystem> passed = new List<CoordinateSystem>();
            List<CoordinateSystem> failed = new List<CoordinateSystem>();
            List<double[]> checkList = new List<double[]>();

            foreach (CoordinateSystem c in coordSys)
            {
                double[] check = new double[] { };
                if (option)
                {
                    check = new double[] { c.Origin.X, c.Origin.Y, c.Origin.Z, c.ZAxis.X, c.ZAxis.Y, c.ZAxis.Z, c.XAxis.X, c.XAxis.Y, c.XAxis.Z };
                }
                else
                {
                    check = new double[] { c.ZAxis.X, c.ZAxis.Y, c.ZAxis.Z, c.XAxis.X, c.XAxis.Y, c.XAxis.Z };
                }
                check = check.Select(v => Math.Round(v, 4)).ToArray();
                if (!checkList.Any(check.SequenceEqual))
                {
                    passed.Add(c);
                    checkList.Add(check);
                }
                else
                {
                    failed.Add(c);
                }
            }

            return new Dictionary<string, List<CoordinateSystem>>
            {
                {"passed", passed},
                {"failed", failed}
            };
        }




        /// <summary>
        /// Test plane normals against guide vector using angular tolerance.
        /// </summary>
        /// <param name="planeList">Planes to test</param>
        /// <param name="guide">Guide vector (Default: World -ZAxis)</param>
        /// <param name="tolerance">Angular tolerance (degrees)</param>
        /// <returns></returns>
        [MultiReturn(new[] { "passed", "failed" })]
        public static Dictionary<string, List<Plane>> testAngular1_Plane(List<Plane> planeList, [DefaultArgumentAttribute("{Vector.ByCoordinates(0,0,-1)}")] Vector guide, double tolerance = 120)
        {
            List<Plane> passed = new List<Plane>();
            List<Plane> failed = new List<Plane>();

            foreach (Plane p in planeList)
            {
                double dot = p.Normal.Dot(guide);
                double angle = Math.Acos(dot) * (180 / Math.PI);
                if (angle > tolerance)
                {
                    failed.Add(p);
                }
                else
                {
                    passed.Add(p);
                }
            }
            return new Dictionary<string, List<Plane>>
            {
                {"passed", passed},
                {"failed", failed}
            };
        }



        /// <summary>
        /// Test coordinate system Z-axes against guide vector using angular tolerance.
        /// </summary>
        /// <param name="csList">Coordinate systems to test</param>
        /// <param name="guide">Guide vector (Default: World -ZAxis)</param>
        /// <param name="tolerance">Angular tolerance (degrees)</param>
        /// <returns></returns>
        [MultiReturn(new[] { "passed", "failed" })]
        public static Dictionary<string, List<CoordinateSystem>> testAngular1_CoordSys(List<CoordinateSystem> csList, [DefaultArgumentAttribute("{Vector.ByCoordinates(0,0,-1)}")] Vector guide, double tolerance = 120)
        {
            List<CoordinateSystem> passed = new List<CoordinateSystem>();
            List<CoordinateSystem> failed = new List<CoordinateSystem>();

            foreach (CoordinateSystem cs in csList)
            {
                double dot = cs.ZAxis.Dot(guide);
                double angle = Math.Acos(dot) * (180 / Math.PI);
                if (angle > tolerance)
                {
                    failed.Add(cs);
                }
                else
                {
                    passed.Add(cs);
                }
            }
            return new Dictionary<string, List<CoordinateSystem>>
            {
                {"passed", passed},
                {"failed", failed}
            };
        }



        /// <summary>
        /// Test strut normals of nodes against guide vector using angular tolerance.
        /// </summary>
        /// <param name="nodeList">List of nodes to test</param>
        /// <param name="guide">Guide vector (Default: World +ZAxis)</param>
        /// <param name="tolerance">Angular tolerance (degrees)</param>
        /// <returns></returns>
        [MultiReturn(new[] { "passedNodes", "failedNodes", "passedStruts", "failedStruts" })]
        public static Dictionary<string, Object> testAngular1_Node(List<Node> nodeList, [DefaultArgumentAttribute("{Vector.ByCoordinates(0,0,1)}")] Vector guide, double tolerance = 120)
        {
            List<Node> passedNodes = new List<Node>();
            List<Node> failedNodes = new List<Node>();
            List<List<Strut>> passedStrutsList = new List<List<Strut>>();
            List<List<Strut>> failedStrutsList = new List<List<Strut>>();

            foreach (Node node in nodeList)
            {
                List<Strut> passedStruts = new List<Strut>();
                List<Strut> failedStruts = new List<Strut>();
                foreach (Strut s in node.Struts)
                {
                    Plane p = s.CutPlaneAtOrigin;
                    double dot = p.Normal.Dot(guide);
                    double angle = Math.Acos(dot) * (180 / Math.PI);
                    if (angle > tolerance)
                    {
                        failedStruts.Add(s);
                    }
                    else
                    {
                        passedStruts.Add(s);
                    }
                }

                passedStrutsList.Add(passedStruts);
                failedStrutsList.Add(failedStruts);

                if (failedStruts.Count() > 0)
                {
                    failedNodes.Add(node);
                }
                else
                {
                    passedNodes.Add(node);
                }
            }

            return new Dictionary<string, Object>
            {
                {"passedNodes", passedNodes },
                {"failedNodes", failedNodes },
                {"passedStruts", passedStrutsList},
                {"failedStruts", failedStrutsList}
            };
        }



        /// <summary>
        /// Test plane normals against each other using angular tolerance.
        /// </summary>
        /// <param name="planeList">Planes to test</param>
        /// <param name="tolerance">Angular tolerance (degrees)</param>
        /// <returns></returns>
        [MultiReturn(new[] { "passed", "failed" })]
        public static Dictionary<string, List<Plane>> testAngular2_Plane(List<Plane> planeList, double tolerance = 35)
        {
            List<Plane> passed = new List<Plane>();
            List<Plane> failed = new List<Plane>();

            List<Vector> normals = planeList.Select(p => p.Normal).ToList();

            for (int i = 0; i < planeList.Count(); i++)
            {
                for (int j = 0; j < planeList.Count(); j++)
                {
                    if (normals[i] != normals[j])
                    {
                        double dot = normals[i].Dot(normals[j]);
                        double angle = Math.Acos(dot) * (180 / Math.PI);

                        if (angle < tolerance && !failed.Contains(planeList[i]))
                        {
                            failed.Add(planeList[i]);
                        }
                    }
                }
            }

            passed = planeList.Except(failed).ToList();

            return new Dictionary<string, List<Plane>>
            {
                {"passed", passed},
                {"failed", failed}
            };
        }



        /// <summary>
        /// Test coordinate system Z-axes against each other using angular tolerance.
        /// </summary>
        /// <param name="csList">Coordinate systems to test</param>
        /// <param name="tolerance">Angular tolerance (degrees)</param>
        /// <returns></returns>
        [MultiReturn(new[] { "passed", "failed" })]
        public static Dictionary<string, List<CoordinateSystem>> testAngular2_CoordSys(List<CoordinateSystem> csList, double tolerance = 35)
        {
            List<CoordinateSystem> passed = new List<CoordinateSystem>();
            List<CoordinateSystem> failed = new List<CoordinateSystem>();

            List<Vector> normals = csList.Select(p => p.ZAxis).ToList();

            for (int i = 0; i < csList.Count(); i++)
            {
                for (int j = 0; j < csList.Count(); j++)
                {
                    if (normals[i] != normals[j])
                    {
                        double dot = normals[i].Dot(normals[j]);
                        double angle = Math.Acos(dot) * (180 / Math.PI);

                        if (angle < tolerance && !failed.Contains(csList[i]))
                        {
                            failed.Add(csList[i]);
                        }
                    }
                }
            }

            passed = csList.Except(failed).ToList();

            return new Dictionary<string, List<CoordinateSystem>>
            {
                {"passed", passed},
                {"failed", failed}
            };
        }



        /// <summary>
        /// Test strut normals of node against each other using angular tolerance.
        /// </summary>
        /// <param name="nodeList">List of nodes to test</param>
        /// <param name="tolerance">Angular tolerance (degrees)</param>
        /// <returns></returns>
        [MultiReturn(new[] { "passedNodes", "failedNodes", "passedStruts", "failedStruts" })]
        public static Dictionary<string, Object> testAngular2_Node(List<Node> nodeList, double tolerance = 35)
        {
            List<Node> passedNodes = new List<Node>();
            List<Node> failedNodes = new List<Node>();
            List<List<Strut>> passedStrutsList = new List<List<Strut>>();
            List<List<Strut>> failedStrutsList = new List<List<Strut>>();

            foreach (Node node in nodeList)
            {
                List<Strut> passedStruts = new List<Strut>();
                List<Strut> failedStruts = new List<Strut>();

                List<Strut> strutList = node.Struts;
                List<Vector> normals = strutList.Select(p => p.CutPlaneAtOrigin.Normal).ToList();

                for (int i = 0; i < strutList.Count(); i++)
                {
                    for (int j = 0; j < strutList.Count(); j++)
                    {
                        if (normals[i] != normals[j])
                        {
                            double dot = normals[i].Dot(normals[j]);
                            double angle = Math.Acos(dot) * (180 / Math.PI);

                            if (angle < tolerance && !failedStruts.Contains(strutList[i]))
                            {
                                failedStruts.Add(strutList[i]);
                            }
                        }
                    }
                }

                passedStruts = strutList.Except(failedStruts).ToList();

                if (failedStruts.Count > 0)
                {
                    failedNodes.Add(node);
                }
                else
                {
                    passedNodes.Add(node);
                }
            }

            return new Dictionary<string, Object>
            {
                {"passedNodes", passedNodes },
                {"failedNodes", failedNodes },
                {"passedStruts", passedStrutsList},
                {"failedStruts", failedStrutsList}
            };
        }



        /// <summary>
        /// Aligns XAxis of plane to guide vector (default: World XAxis)
        /// </summary>
        /// <param name="planeList">List of planes</param>
        /// <param name="vec">Alignment vector for XAxes</param>
        /// <param name="degree">Angle multiplier</param>
        /// <returns></returns>
        public static List<Plane> alignByXAxis_Plane(List<Plane> planeList, [DefaultArgumentAttribute("{Vector.ByCoordinates(1,0,0)}")] Vector vec, double degree = 1)
        {
            List<Plane> newPlList = new List<Plane>();

            foreach (Plane p in planeList)
            {
                Vector projectedVector = Vector.ByCoordinates(p.XAxis.X, p.XAxis.Y, 0);
                double dot = p.XAxis.Dot(projectedVector);
                double angle = Math.Acos(dot) * (-1) * degree * (180 / Math.PI);
                Plane newPl = (Plane)p.Rotate(p.Origin, p.Normal, angle);
                newPlList.Add(newPl);
            }
            return newPlList;
        }



        /// <summary>
        /// Aligns XAxis of coordinate systems to guide vector (default: World XAxis)
        /// </summary>
        /// <param name="csList">List of coordinate systems</param>
        /// <param name="vec">Alignment vector for XAxes</param>
        /// <param name="degree">Angle multiplier</param>
        /// <returns></returns>
        public static List<CoordinateSystem> alignByXAxis_CoordSys(List<CoordinateSystem> csList, [DefaultArgumentAttribute("{Vector.ByCoordinates(1,0,0)}")] Vector vec, double degree = 1)
        {
            List<CoordinateSystem> newCSList = new List<CoordinateSystem>();

            foreach (CoordinateSystem cs in csList)
            {
                Vector projectedVector = Vector.ByCoordinates(cs.XAxis.X, cs.XAxis.Y, 0);
                double dot = cs.XAxis.Dot(projectedVector);
                double angle = Math.Acos(dot) * (-1) * degree * (180 / Math.PI);
                CoordinateSystem newCS = cs.Rotate(cs.Origin, cs.ZAxis, angle);
                newCSList.Add(newCS);
            }
            return newCSList;
        }


        /*
        /// <summary>
        /// Aligns XAxis of strut frame on node to guide vector (default: World XAxis)
        /// </summary>
        /// <param name="nodeList">List of nodes</param>
        /// <param name="vec">Alignment vector for XAxes</param>
        /// <param name="degree">Angle multiplier</param>
        /// <returns></returns>
        public static List<Node> alignByXAxis_Node(List<Node> nodeList, [DefaultArgumentAttribute("{Vector.ByCoordinates(1,0,0)}")] Vector vec, double degree = 1)
        {
            //List<Node> newNodeList = new List<Node>();

            foreach (Node node in nodeList)
            {
                List<Line> newLineList = new List<Line>();
                foreach (Strut strut in node.Struts)
                {
                    Plane p = strut.TransformedCutPlane;
                    Vector projectedVector = Vector.ByCoordinates(p.XAxis.X, p.XAxis.Y, 0);
                    double dot = p.XAxis.Dot(projectedVector);
                    double angle = Math.Acos(dot) * (-1) * degree * (180 / Math.PI);

                    // rebuild node
                    Line newLine = (Line) strut.LineRepresentation.Rotate(p.Origin, p.Normal, angle);
                    newLineList.Add(newLine);
                }

                Node newNode = Node.ByPointsLinesAndGeoOrientationStrategy(node.Center, newLineList, 6, node.NodeGeometry);
                newNodeList.Add(newNode);
            }

            return nodeList;
        }
        */



        /// <summary>
        /// Reverse normal and retain rotation of plane.
        /// </summary>
        /// <param name="plane">Plane to flip</param>
        /// <param name="retain">Retain X-Axis?</param>
        /// <returns></returns>
        private static Plane flip_Plane2(Plane plane, bool retain = true)
        {
            switch (retain)
            {
                case true:
                    {
                        Plane newPl = Plane.ByOriginNormalXAxis(plane.Origin, plane.Normal.Reverse(), plane.XAxis);
                        plane = newPl;
                        break;
                    }
                case false:
                    {
                        Plane newPl = Plane.ByOriginNormalXAxis(plane.Origin, plane.Normal.Reverse(), plane.XAxis.Reverse());
                        plane = newPl;
                        break;
                    }
            }
            return plane;
        }



        /// <summary>
        /// Reverse normal and retain rotation of plane.
        /// </summary>
        /// <param name="planeList">List of planes</param>
        /// <param name="retain">Retain X-Axis?</param>
        /// <returns></returns>
        public static List<Plane> flip_Plane(List<Plane> planeList, bool retain = false)
        {
            List<Plane> newPlaneList = new List<Plane>();

            switch (retain)
            {
                case true:
                    {
                        foreach (Plane pl in planeList)
                        {
                            Plane newPl = Plane.ByOriginNormalXAxis(pl.Origin, pl.Normal.Reverse(), pl.XAxis);
                            newPlaneList.Add(newPl);
                        }
                        break;
                    }
                case false:
                    {
                        foreach (Plane pl in planeList)
                        {
                            Plane newPl = Plane.ByOriginNormalXAxis(pl.Origin, pl.Normal.Reverse(), pl.XAxis.Reverse());
                            newPlaneList.Add(newPl);
                        }
                        break;
                    }
            }

            return newPlaneList;
        }



        /// <summary>
        /// Reverse normal and retain rotation of coordinate system.
        /// </summary>
        /// <param name="csList">List of coordinate systems</param>
        /// <param name="retain">Retain X-Axis?</param>
        /// <returns></returns>
        public static List<CoordinateSystem> flip_CoordSys(List<CoordinateSystem> csList, bool retain)
        {
            List<CoordinateSystem> newCSList = new List<CoordinateSystem>();

            switch (retain)
            {
                case true:
                    {
                        foreach (CoordinateSystem cs in csList)
                        {
                            CoordinateSystem newCS = CoordinateSystem.ByPlane(Plane.ByOriginNormalXAxis(cs.Origin, cs.ZAxis.Reverse(), cs.XAxis));
                            newCSList.Add(newCS);
                        }
                        break;
                    }
                case false:
                    {
                        foreach (CoordinateSystem cs in csList)
                        {
                            CoordinateSystem newCS = CoordinateSystem.ByPlane(Plane.ByOriginNormalXAxis(cs.Origin, cs.ZAxis.Reverse(), cs.XAxis.Reverse()));
                            newCSList.Add(newCS);
                        }
                        break;
                    }
            }
            return newCSList;
        }



        /// <summary>
        /// For each point get list of and reorient all lines and get list of planes.
        /// </summary>
        /// <param name="lines">List of lines</param>
        /// <param name="points">List of points</param>
        /// <returns></returns>
        [MultiReturn(new[] { "lineList", "planeList" })]
        public static Dictionary<string, List<List<Object>>> getLinesPlaneAtPoint(List<Line> lines, List<Point> points)
        {
            List<List<Object>> lineListList = new List<List<Object>>();
            List<List<Object>> planeListList = new List<List<Object>>();
            List<Point> errPoints = new List<Point>();

            foreach (Point p in points)
            {
                List<Object> lineList = new List<Object>();
                List<Object> planeList = new List<Object>();

                List<bool> boolList = new List<bool>();

                foreach (Line l in lines)
                {
                    bool intersection = p.DoesIntersect(l);
                    if (intersection)
                    {
                        bool meets = l.StartPoint.IsAlmostEqualTo(p);
                        if (meets)
                        {
                            lineList.Add(l);
                            Plane pl = l.PlaneAtParameter(0);
                            planeList.Add((Object)pl);
                        }
                        else
                        {
                            Line lRev = (Line) l.Reverse();
                            lineList.Add((Object) lRev);
                            Plane pl = lRev.PlaneAtParameter(0);
                            planeList.Add((Object) pl);
                        }
                    }
                }

                lineListList.Add(lineList);
                planeListList.Add(planeList);
            }

            return new Dictionary<string, List<List<Object>>>
            {
                {"lineList", lineListList},
                {"planeList", planeListList}
            };
        }



        /// <summary>
        /// For each point get list of and reorient all lines and get list of planes.
        /// </summary>
        /// <param name="lines">List of lines</param>
        /// <param name="points">List of points</param>
        /// <returns></returns>
        [MultiReturn(new[] { "lineList", "planeList" })]
        public static Dictionary<string, List<List<Object>>> getLinesCoordSysAtPoint(List<Line> lines, List<Point> points)
        {
            List<List<Object>> lineListList = new List<List<Object>>();
            List<List<Object>> CSListList = new List<List<Object>>();
            List<Point> errPoints = new List<Point>();

            foreach (Point p in points)
            {
                List<Object> lineList = new List<Object>();
                List<Object> csList = new List<Object>();

                List<bool> boolList = new List<bool>();

                foreach (Line l in lines)
                {
                    bool intersection = p.DoesIntersect(l);
                    if (intersection)
                    {
                        bool meets = l.StartPoint.IsAlmostEqualTo(p);
                        if (meets)
                        {
                            lineList.Add(l);
                            Plane pl = l.PlaneAtParameter(0);
                            CoordinateSystem cs = CoordinateSystem.ByPlane(pl);
                            csList.Add((Object)cs);
                        }
                        else
                        {
                            Line lRev = (Line)l.Reverse();
                            lineList.Add((Object)lRev);
                            Plane pl = lRev.PlaneAtParameter(0);
                            CoordinateSystem cs = CoordinateSystem.ByPlane(pl);
                            csList.Add((Object)cs);
                        }
                    }
                }

                lineListList.Add(lineList);
                CSListList.Add(csList);
            }

            return new Dictionary<string, List<List<Object>>>
            {
                {"lineList", lineListList},
                {"planeList", CSListList}
            };
        }



        /// <summary>
        /// Transform each sublist of planes to World XY plane using average ZAxis.
        /// </summary>
        /// <param name="planes">List of list of planes</param>
        /// <param name="guide">Guide plane</param>
        /// <returns></returns>
        public static List<List<Plane>> transform_Plane(List<List<Plane>> planes, [DefaultArgumentAttribute("{Plane.ByOriginNormalXAxis(Point.ByCoordinates(0,0,0), Vector.ByCoordinates(0,0,1), Vector.ByCoordinates(1,0,0))}")] Plane guide)
        {
            List<List<Plane>> newPlanes = new List<List<Plane>>();

            foreach (List<Plane> sub in planes)
            {
                Point origin = sub.Select(p => p.Origin).ToList()[0];
                List<Vector> zAxis = sub.Select(p => p.Normal).ToList();
                Vector zAxisAvg = Vector.ByCoordinates(zAxis.Average(v => v.X), zAxis.Average(v => v.Y), zAxis.Average(v => v.Z));
                Plane subFrame = Plane.ByOriginNormal(origin, zAxisAvg);

                CoordinateSystem subFrameTransform = subFrame.ContextCoordinateSystem;
                CoordinateSystem guideFrame = guide.ContextCoordinateSystem;

                List<Plane> newSub = new List<Plane>();
                foreach (Plane plane in sub)
                {
                    newSub.Add((Plane) plane.Transform(subFrameTransform, guideFrame));
                }
                newPlanes.Add(newSub);
            }
            return newPlanes;
        }



        /// <summary>
        /// Transform each sublist of coordinate systems to World XY coordinate systems using average ZAxis.
        /// </summary>
        /// <param name="coordSys">List of list of planes</param>
        /// <param name="guide">Guide coordinate system</param>
        /// <returns></returns>
        public static List<List<CoordinateSystem>> transform_CoordSys(List<List<CoordinateSystem>> coordSys, [DefaultArgumentAttribute("{CoordinateSystem.ByPlane(Plane.ByOriginNormalXAxis(Point.ByCoordinates(0,0,0), Vector.ByCoordinates(0,0,1), Vector.ByCoordinates(1,0,0)))}")] CoordinateSystem guide)
        {
            List<List<CoordinateSystem>> newCoordSys = new List<List<CoordinateSystem>>();

            foreach (List<CoordinateSystem> sub in coordSys)
            {
                Point origin = sub.Select(p => p.Origin).ToList()[0];
                List<Vector> zAxis = sub.Select(p => p.ZAxis).ToList();
                Vector zAxisAvg = Vector.ByCoordinates(zAxis.Average(v => v.X), zAxis.Average(v => v.Y), zAxis.Average(v => v.Z));
                Plane subFrame = Plane.ByOriginNormal(origin, zAxisAvg);

                CoordinateSystem subFrameTransform = subFrame.ContextCoordinateSystem;

                List<CoordinateSystem> newSub = new List<CoordinateSystem>();
                foreach (CoordinateSystem cs in sub)
                {
                    newSub.Add((CoordinateSystem) cs.Transform(subFrameTransform, guide));
                }
                newCoordSys.Add(newSub);
            }
            return newCoordSys;
        }



        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////



        private static string ToolRobHold(Plane p, bool r)
        {
            List<double> q = Utilities.QuatListAtPlane(p);
            Point o = p.Origin;
            string rB = r.ToString().ToUpper();
            string t = "[" + rB + ", [[" + o.X + "," + o.Y + "," + o.Z + "], [" + q[0] + "," + q[1] + "," + q[2] + "," + q[3] + "]], [1,[0,0,0.001],[1,0,0,0],0,0,0]]";
            return t;
        }

        private static string WobjRobHold(Plane p, bool r)
        {
            List<double> q = Utilities.QuatListAtPlane(p);
            Point o = p.Origin;
            string rB = r.ToString().ToUpper();
            string w = "[" + rB + ", " + rB + ", \"\", [[" + o.X + "," + o.Y + "," + o.Z + "], [" + q[0] + "," + q[1] + "," + q[2] + "," + q[3] + "]], [[0,0,0], [1,0,0,0]]]";
            return w;
        }

        private static string rtarget(Plane p)
        {
            List<double> quatDoubles = RobotUtils.PlaneToQuaternian(p);
            string target = string.Format(
                    "[[{0},{1},{2}], [{3},{4},{5},{6}], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]]",
                    p.Origin.X, p.Origin.Y, p.Origin.Z, quatDoubles[0], quatDoubles[1], quatDoubles[2], quatDoubles[3]);
            return target;
        }

        private static string jtarget(double j0, double j1, double j2, double j3, double j4, double j5)
        {
            string target = string.Format(
                "[[{0},{1},{2},{3},{4},{5}], [9E9,9E9,9E9,9E9,9E9,9E9]]", j0, j1, j2, j3, j4, j5);
            return target;
        }



        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////



        /// <summary>
        /// Create routine for an ABB robot with stationary drill and mobile workpiece.
        /// </summary>
        /// <param name="directory">Directory to write files ("C:\")</param>
        /// <param name="filePrefix">Convention for filename prefix ("Group_A")</param>
        /// <param name="holeFrames">Position of all holes per block</param>
        /// <param name="blockFrame">Position of block centroid</param>
        /// <param name="drillFrame">Position of drill tip</param>
        /// <returns></returns>
        [MultiReturn(new[] { "filePaths" })]
        public static Dictionary<string, List<string>> createDrillRoutine1(string directory, string filePrefix, List<List<Plane>> holeFrames, Plane blockFrame, Plane drillFrame)
        {
            // create list of filenames
            List<string> outputFiles = new List<string>();

            // create tool, wobj, rate
            var dataBuilder = new StringBuilder();
            dataBuilder.Append(string.Format("\n\tTASK PERS tooldata drill := {0};", ToolRobHold(drillFrame, false)));
            dataBuilder.Append(string.Format("\n\tTASK PERS wobjdata block := {0};", WobjRobHold(blockFrame, true)));
            dataBuilder.Append(string.Format("\n\tTASK PERS speeddata rate := {0};", "[3, 500, 5000, 1000]"));

            // begin main
            foreach (List<Plane> holes in holeFrames)
            {
                // name files
                int indexPlusOne = holeFrames.IndexOf(holes) + 1;
                string paddedIndex = indexPlusOne.ToString().PadLeft(3, '0');
                string filename = string.Format("{0}\\{1}_{2}.prg", directory, filePrefix, paddedIndex);
                outputFiles.Add(filename);

                // setup sub
                int index = 0;
                var targBuilder = new StringBuilder();
                var moveBuilder = new StringBuilder();
                targBuilder.Append(string.Format("\n\tVAR jointtarget j0 := {0};", jtarget(-90, 0, 0, 90, 90, 0)));
                targBuilder.Append(string.Format("\n\tVAR jointtarget j1 := {0};", jtarget(-41, 0, 0, 0, 90, 0)));
                foreach (Plane hole in holes)
                {
                    // create targets
                    targBuilder.Append(string.Format("\n\tVAR robtarget p{0}0 := {1};", index, rtarget((Plane)hole.Translate(hole.Normal, -120))));
                    targBuilder.Append(string.Format("\n\tVAR robtarget p{0}1 := {1};", index, rtarget((Plane) hole.Translate(hole.Normal, -50))));
                    targBuilder.Append(string.Format("\n\tVAR robtarget p{0}2 := {1};", index, rtarget((Plane) hole.Translate(hole.Normal, -10))));

                    // create movement instructions
                    moveBuilder.Append(string.Format("\n"));
                    moveBuilder.Append(string.Format("\n\t\tTPWrite(\"Drilling hole {0} of {1}!\");", index + 1, holes.Count()));
                    moveBuilder.Append(string.Format("\n\t\tMoveL p{0}0, {1}, {2}, drill\\WObj:=block;", index, "v100", "z30"));
                    moveBuilder.Append(string.Format("\n\t\tMoveL p{0}1, {1}, {2}, drill\\WObj:=block;", index, "v100", "z5"));
                    moveBuilder.Append(string.Format("\n\t\tMoveL p{0}2, {1}, {2}, drill\\WObj:=block;", index, "rate", "fine"));
                    moveBuilder.Append(string.Format("\n\t\tMoveL p{0}1, {1}, {2}, drill\\WObj:=block;", index, "rate", "fine"));
                    moveBuilder.Append(string.Format("\n\t\tMoveL p{0}0, {1}, {2}, drill\\WObj:=block;", index, "v100", "z30"));
                    moveBuilder.Append(string.Format("\n\t\tMoveL RelTool(p{0}0, 0, 50, 0), {1}, {2}, drill\\WObj:=block;", index, "v100", "z5"));

                    // create safe movement to next
                    if (index < holes.Count() - 1)
                    {
                        moveBuilder.Append(string.Format("\n\t\tMoveAbsJ CalcJointT(RelTool(p{0}0, 0, 50, 0), drill\\WObj:=block), {1}, {2}, drill\\WObj:=block;", index + 1, "v100", "z5"));
                    }

                    // update index
                    index += 1;
                }

                // create rapid
                string r = "";
                using (var tw = new StreamWriter(filename, false))
                {
                    r =
                        "MODULE MainModule" +
                        "\n" +
                        "\n\t" + "! " + filePrefix + "_" + paddedIndex +
                        "\n" +
                        "\n\t" + "! variables" + dataBuilder.ToString() +
                        "\n" +
                        "\n\t" + "! targets" + targBuilder.ToString() +
                        "\n" +
                        "\n\t" + "! drilling instructions" +
                        "\n\t" + "PROC main()" +
                        "\n\t\t" + "ConfL\\Off;" +
                        "\n\t\t" + "SingArea\\Wrist;" +
                        "\n" +
                        "\n\t\t" + "TPWrite(\"This is: " + filePrefix + "_" + paddedIndex + "\");" +
                        "\n\t\t" + "TPWrite(\"Check block and drill\");" +
                        "\n\t\t" + "MoveAbsJ j0, v100, z5, tool0;" + 
                        "\n\t\t" + "MoveAbsJ j1, v100, z5, tool0;" + moveBuilder.ToString() +
                        "\n\n\t\t" + "TPWrite(\"Resetting axes...\");" +
                        "\n\t\t" + "MoveAbsJ j1, v100, z5, tool0;" +
                        "\n\t\t" + "MoveAbsJ j0, v100, z5, tool0;" +
                        "\n" +
                        "\n\t\t" + "Stop;" +
                        "\n\t" + "ENDPROC" +
                        "\n" +
                        "\n" + "ENDMODULE"
                        ;

                    tw.Write(r);
                    tw.Flush();
                }
            }

            // end step
            return new Dictionary<string, List<string>>
            {
                {"filePaths", outputFiles}
            };
        }






        /// <summary>
        /// Create routine for an ABB robot with stationary drill and mobile workpiece.
        /// </summary>
        /// <param name="directory">Directory to write files ("C:\")</param>
        /// <param name="filePrefix">Convention for filename prefix ("Group_A")</param>
        /// <param name="holeFrames">Position of all holes per block</param>
        /// <returns></returns>
        [MultiReturn(new[] { "filePaths" })]
        public static Dictionary<string, List<string>> createDrillRoutine2(string directory, string filePrefix, List<List<Plane>> holeFrames)
        {
            // create list of filenames
            List<string> outputFiles = new List<string>();

            // begin main
            foreach (List<Plane> holes in holeFrames)
            {
                // name files
                int indexPlusOne = holeFrames.IndexOf(holes) + 1;
                string paddedIndex = indexPlusOne.ToString().PadLeft(3, '0');
                string filename = string.Format("{0}\\{1}_{2}.prg", directory, filePrefix, paddedIndex);
                outputFiles.Add(filename);

                // setup sub
                int index = 0;
                var targBuilder = new StringBuilder();
                var moveBuilder = new StringBuilder();
                targBuilder.Append(string.Format("\n\tVAR jointtarget j0 := {0};", jtarget(-90, 0, 0, 90, 90, 0)));
                targBuilder.Append(string.Format("\n\tVAR jointtarget j1 := {0};", jtarget(-41, 0, 0, 0, 90, 0)));
                foreach (Plane hole in holes)
                {
                    // create targets
                    targBuilder.Append(string.Format("\n\tVAR robtarget p{0}0 := {1};", index, rtarget((Plane)hole.Translate(hole.Normal, -120))));
                    targBuilder.Append(string.Format("\n\tVAR robtarget p{0}1 := {1};", index, rtarget((Plane)hole.Translate(hole.Normal, -50))));
                    targBuilder.Append(string.Format("\n\tVAR robtarget p{0}2 := {1};", index, rtarget((Plane)hole.Translate(hole.Normal, -10))));

                    // create movement instructions
                    moveBuilder.Append(string.Format("\n"));
                    moveBuilder.Append(string.Format("\n\t\tTPWrite(\"Drilling hole {0} of {1}!\");", index + 1, holes.Count()));
                    moveBuilder.Append(string.Format("\n\t\tMoveL p{0}0, {1}, {2}, drill\\WObj:=block;", index, "v100", "z30"));
                    moveBuilder.Append(string.Format("\n\t\tMoveL p{0}1, {1}, {2}, drill\\WObj:=block;", index, "v100", "z5"));
                    moveBuilder.Append(string.Format("\n\t\tMoveL p{0}2, {1}, {2}, drill\\WObj:=block;", index, "rate", "fine"));
                    moveBuilder.Append(string.Format("\n\t\tMoveL p{0}1, {1}, {2}, drill\\WObj:=block;", index, "rate", "fine"));
                    moveBuilder.Append(string.Format("\n\t\tMoveL p{0}0, {1}, {2}, drill\\WObj:=block;", index, "v100", "z30"));
                    moveBuilder.Append(string.Format("\n\t\tMoveL RelTool(p{0}0, 0, 50, 0), {1}, {2}, drill\\WObj:=block;", index, "v100", "z5"));

                    // create safe movement to next
                    if (index < holes.Count() - 1)
                    {
                        moveBuilder.Append(string.Format("\n\t\tMoveAbsJ CalcJointT(RelTool(p{0}0, 0, 50, 0), drill\\WObj:=block), {1}, {2}, drill\\WObj:=block;", index + 1, "v100", "z5"));
                    }

                    // update index
                    index += 1;
                }

                // create rapid
                string r = "";
                using (var tw = new StreamWriter(filename, false))
                {
                    r =
                        "MODULE MainModule" +
                        "\n" +
                        "\n\t" + "! " + filePrefix + "_" + paddedIndex +
                        "\n" +
                        "\n\t" + "! targets" + targBuilder.ToString() +
                        "\n" +
                        "\n\t" + "! drilling instructions" +
                        "\n\t" + "PROC main()" +
                        "\n\t\t" + "ConfL\\Off;" +
                        "\n\t\t" + "SingArea\\Wrist;" +
                        "\n" +
                        "\n\t\t" + "TPWrite(\"This is " + filePrefix + "_" + paddedIndex + "\");" +
                        "\n\t\t" + "TPWrite(\"Check block and drill\");" +
                        "\n\t\t" + "MoveAbsJ j0, v100, z5, tool0;" +
                        "\n\t\t" + "MoveAbsJ j1, v100, z5, tool0;" + moveBuilder.ToString() +
                        "\n\n\t\t" + "TPWrite(\"Resetting axes...\");" +
                        "\n\t\t" + "MoveAbsJ j1, v100, z5, tool0;" +
                        "\n\t\t" + "MoveAbsJ j0, v100, z5, tool0;" +
                        "\n" +
                        "\n\t\t" + "Stop;" +
                        "\n\t" + "ENDPROC" +
                        "\n" +
                        "\n" + "ENDMODULE"
                        ;

                    tw.Write(r);
                    tw.Flush();
                }
            }

            // end step
            return new Dictionary<string, List<string>>
            {
                {"filePaths", outputFiles}
            };
        }





        /// <summary>
        /// Create routine for an ABB robot with stationary drill and mobile workpiece.
        /// </summary>
        /// <param name="directory">Directory to write files ("C:\")</param>
        /// <param name="nodes">A list of all your favorite nodes \m| </param>
        /// <returns>filePaths</returns>
        public static List<string> createDrillRoutine3(string directory, List<Node> nodes)
        {
            // create list of filenames
            List<string> outputFiles = new List<string>();

            // begin main
            foreach (Node node in nodes)
            {
                // name files
                string filename = string.Format("{0}\\{1}.prg", directory, node.ID.ToString());
                outputFiles.Add(filename);

                // setup  sub
                int index = 0;
                var targBuilder = new StringBuilder();
                var moveBuilder = new StringBuilder();

                foreach (Strut strut in node.Struts)
                {
                    // setup target
                    Plane hole = strut.AlignedCutPlaneAtOrigin(Vector.ByCoordinates(0, 1, 0));

                    // perform correction
                    hole = flip_Plane2(hole, false);
                    if (hole.Normal.IsAlmostEqualTo(Vector.XAxis()))
                    {
                        hole = Plane.ByOriginNormalXAxis(hole.Origin, hole.Normal, hole.YAxis.Reverse());
                    }
                    if (hole.Normal.IsAlmostEqualTo(Vector.XAxis().Reverse()))
                    {
                        hole = Plane.ByOriginNormalXAxis(hole.Origin, hole.Normal, hole.YAxis);
                    }

                    // create targets
                    targBuilder.Append(string.Format("\n"));
                    targBuilder.Append(string.Format("\n\t! {0};", strut.ID));
                    targBuilder.Append(string.Format("\n\tVAR robtarget S{0}0 := {1};", index, rtarget((Plane)(hole.Translate(hole.Normal, -100)))));
                    targBuilder.Append(string.Format("\n\tVAR robtarget S{0}1 := {1};", index, rtarget((Plane)(hole.Translate(hole.Normal, -50)))));
                    targBuilder.Append(string.Format("\n\tVAR robtarget S{0}2 := {1};", index, rtarget((Plane)(hole.Translate(hole.Normal, -10)))));

                    // create movement instructions
                    moveBuilder.Append(string.Format("\n"));
                    moveBuilder.Append(string.Format("\n\t\tTPWrite(\"Drilling... {0}, {1} of {2}.\");", strut.ID, index + 1, node.Struts.Count()));
                    moveBuilder.Append(string.Format("\n\t\tMoveL S{0}0, {1}, {2}, drill\\WObj:=block;", index, "v200", "z30"));
                    moveBuilder.Append(string.Format("\n\t\tMoveL S{0}1, {1}, {2}, drill\\WObj:=block;", index, "v100", "z5"));
                    moveBuilder.Append(string.Format("\n\t\tMoveL S{0}2, {1}, {2}, drill\\WObj:=block;", index, "rate", "fine"));
                    moveBuilder.Append(string.Format("\n\t\tMoveL S{0}1, {1}, {2}, drill\\WObj:=block;", index, "rate", "fine"));
                    moveBuilder.Append(string.Format("\n\t\tMoveL S{0}0, {1}, {2}, drill\\WObj:=block;", index, "v100", "z5"));
                    //moveBuilder.Append(string.Format("\n\t\tMoveL RelTool(S{0}0, 0, 50, 0), {1}, {2}, drill\\WObj:=block;", index, "v200", "z30"));

                    // create safe movement to next
                    if (index < node.Struts.Count() - 1)
                    {
                        //moveBuilder.Append(string.Format("\n\t\tMoveAbsJ CalcJointT(RelTool(S{0}0, 0, 50, 0), drill\\WObj:=block), {1}, {2}, drill\\WObj:=block;", index + 1, "v200", "z5"));
                        moveBuilder.Append(string.Format("\n\t\tMoveAbsJ CalcJointT(S{0}0, drill\\WObj:=block), {1}, {2}, drill\\WObj:=block;", index + 1, "v200", "z5"));
                    }

                    // update index
                    index += 1;
                }

                targBuilder.Append(string.Format("\n"));
                targBuilder.Append(string.Format("\n\tVAR jointtarget j0 := {0};", jtarget(-135, 0, 0, 90, 90, 0)));
                targBuilder.Append(string.Format("\n\tVAR jointtarget j1 := {0};", jtarget(-45, 0, 0, 0, 90, 0)));

                // create rapid
                string r = "";
                using (var tw = new StreamWriter(filename, false))
                {
                    r =
                        "MODULE MainModule" +
                        "\n" +
                        "\n\t" + "! " + node.ID.ToString() + "\n" + targBuilder.ToString() +
                        "\n" +
                        "\n" +
                        "\n\t" + "! ROUTINE" +
                        "\n\t" + "PROC main()" +
                        "\n\t\t" + "ConfL\\Off;" +
                        "\n\t\t" + "SingArea\\Wrist;" +
                        "\n" +
                        "\n\t\t" + "TPErase;" +
                        "\n\t\t" + "TPWrite(\"This is " + node.ID.ToString() + "\");" +
                        "\n" +
                        "\n\t\t" + "TPWrite(\"Check block && drill!\");" +
                        "\n\t\t" + "MoveAbsJ j0, v200, z5, tool0;" +
                        "\n\t\t" + "MoveAbsJ j1, v200, z5, tool0;" + moveBuilder.ToString() +
                        "\n" +
                        "\n\t\t" + "TPWrite(\"Returning to start...\");" +
                        "\n\t\t" + "MoveAbsJ j1, v200, z5, tool0;" +
                        "\n\t\t" + "MoveAbsJ j0, v200, z5, tool0;" +
                        "\n" +
                        "\n\t\t" + "TPWrite(\"Node " + node.ID.ToString() + " complete!\");" +
                        "\n" +
                        "\n\t\t" + "Stop;" +
                        "\n\t" + "ENDPROC" +
                        "\n" +
                        "\n" + "ENDMODULE"
                        ;

                    tw.Write(r);
                    tw.Flush();
                }
            }

            // end step
            return outputFiles;
        }









        /// <summary>
        /// Create routine for an ABB robot with stationary drill and mobile workpiece.
        /// </summary>
        /// <param name="directory">Directory to write files ("C:\")</param>
        /// <param name="nodes">A list of all your favorite nodes \m| </param>
        /// <returns>filePaths</returns>
        public static List<string> createDrillRoutine4(string directory, List<Node> nodes)
        {
            // create list of filenames
            List<string> outputFiles = new List<string>();

            // begin main
            foreach (Node node in nodes)
            {
                // name files
                string filename = string.Format("{0}\\{1}.prg", directory, node.ID.ToString());
                outputFiles.Add(filename);

                // setup  sub
                int index = 0;
                var targBuilder = new StringBuilder();
                var moveBuilder = new StringBuilder();

                foreach (Strut strut in node.Struts)
                {
                    // setup target
                    Plane hole = strut.AlignedCutPlaneAtOrigin(Vector.ByCoordinates(0, 1, 0));

                    // perform correction
                    hole = flip_Plane2(hole, false);
                    if (hole.Normal.IsAlmostEqualTo(Vector.XAxis()))
                    {
                        hole = Plane.ByOriginNormalXAxis(hole.Origin, hole.Normal, hole.YAxis.Reverse());
                    }
                    if (hole.Normal.IsAlmostEqualTo(Vector.XAxis().Reverse()))
                    {
                        hole = Plane.ByOriginNormalXAxis(hole.Origin, hole.Normal, hole.YAxis);
                    }

                    // create targets
                    targBuilder.Append(string.Format("\n"));
                    targBuilder.Append(string.Format("\n\t! {0};", strut.ID));
                    targBuilder.Append(string.Format("\n\tVAR robtarget S{0}0 := {1};", index, rtarget((Plane)(hole.Translate(hole.Normal, -100)))));
                    targBuilder.Append(string.Format("\n\tVAR robtarget S{0}1 := {1};", index, rtarget((Plane)(hole.Translate(hole.Normal, -50)))));
                    targBuilder.Append(string.Format("\n\tVAR robtarget S{0}2 := {1};", index, rtarget((Plane)(hole.Translate(hole.Normal, -10)))));

                    // create movement instructions
                    moveBuilder.Append(string.Format("\n"));
                    moveBuilder.Append(string.Format("\n\t\tTPWrite(\"Drilling... {0}, {1} of {2}.\");", strut.ID, index + 1, node.Struts.Count()));
                    moveBuilder.Append(string.Format("\n\t\tMoveL S{0}0, {1}, {2}, drill\\WObj:=block;", index, "v200", "z30"));
                    moveBuilder.Append(string.Format("\n\t\tMoveL S{0}1, {1}, {2}, drill\\WObj:=block;", index, "v100", "z5"));
                    moveBuilder.Append(string.Format("\n\t\tMoveL S{0}2, {1}, {2}, drill\\WObj:=block;", index, "rate", "fine"));
                    moveBuilder.Append(string.Format("\n\t\tMoveL S{0}1, {1}, {2}, drill\\WObj:=block;", index, "rate", "fine"));
                    moveBuilder.Append(string.Format("\n\t\tMoveL S{0}0, {1}, {2}, drill\\WObj:=block;", index, "v100", "z5"));
                    //moveBuilder.Append(string.Format("\n\t\tMoveL RelTool(S{0}0, 0, 50, 0), {1}, {2}, drill\\WObj:=block;", index, "v200", "z30"));

                    // create safe movement to next
                    if (index < node.Struts.Count() - 1)
                    {
                        //moveBuilder.Append(string.Format("\n\t\tMoveAbsJ CalcJointT(RelTool(S{0}0, 0, 50, 0), drill\\WObj:=block), {1}, {2}, drill\\WObj:=block;", index + 1, "v200", "z5"));
                        moveBuilder.Append(string.Format("\n\t\tMoveAbsJ CalcJointT(S{0}0, drill\\WObj:=block), {1}, {2}, drill\\WObj:=block;", index + 1, "v200", "z5"));
                    }

                    // update index
                    index += 1;
                }

                //targBuilder.Append(string.Format("\n"));
                //targBuilder.Append(string.Format("\n\tVAR jointtarget j0 := {0};", jtarget(-135, 0, 0, 90, 90, 0)));
                //targBuilder.Append(string.Format("\n\tVAR jointtarget j1 := {0};", jtarget(-45, 0, 0, 0, 90, 0)));

                // create rapid
                string r = "";
                using (var tw = new StreamWriter(filename, false))
                {
                    r =
                        "MODULE MainModule" +
                        "\n" +
                        "\n\t" + "! " + node.ID.ToString() + "\n" + targBuilder.ToString() +
                        "\n" +
                        "\n" +
                        "\n\t" + "! ROUTINE" +
                        "\n\t" + "PROC main()" +
                        "\n\t\t" + "ConfL\\Off;" +
                        "\n\t\t" + "SingArea\\Wrist;" +
                        "\n" +
                        "\n\t\t" + "TPErase;" +
                        "\n\t\t" + "TPWrite(\"This is " + node.ID.ToString() + "\");" +
                        "\n" +
                        "\n\t\t" + "TPWrite(\"Check block && drill!\");" +
                        "\n\t\t" + "MoveAbsJ j0, v200, z5, tool0;" +
                        "\n\t\t" + "MoveAbsJ j1, v200, z5, tool0;" + moveBuilder.ToString() +
                        "\n" +
                        "\n\t\t" + "TPWrite(\"Returning to start...\");" +
                        "\n\t\t" + "MoveAbsJ j1, v200, z5, tool0;" +
                        "\n\t\t" + "MoveAbsJ j0, v200, z5, tool0;" +
                        "\n" +
                        "\n\t\t" + "TPWrite(\"Node " + node.ID.ToString() + " complete!\");" +
                        "\n" +
                        "\n\t\t" + "Stop;" +
                        "\n\t" + "ENDPROC" +
                        "\n" +
                        "\n" + "ENDMODULE"
                        ;

                    tw.Write(r);
                    tw.Flush();
                }
            }

            // end step
            return outputFiles;
        }












    }
}
