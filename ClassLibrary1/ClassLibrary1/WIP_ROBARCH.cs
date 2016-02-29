﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Dynamo_TORO;

namespace Dynamo_TORO
{
    /// <summary>
    /// Container for RobArch workflow
    /// </summary>
    public class WIP_ROBARCH
    {


        /// <summary>
        /// Sort vectors by directionality about arbitrary pole.
        /// </summary>
        /// <param name="vecList"></param>
        /// <returns></returns>
        [MultiReturn(new[] { "sorted", "indices" })]
        public static Dictionary<string, List<Object>> sortPolar_Vector(List<Vector> vecList)
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
        public static Dictionary<string, List<Object>> sortPolar_Plane(List<Plane> planeList)
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
        public static Dictionary<string, List<Object>> sortPolar_CoordSys(List<CoordinateSystem> csList)
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
        /// Sort vectors directionality about average pole and shift.
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
        /// Create a set from only the unique items in a list.
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        public static List<Object> createSet(List<Object> lst)
        {
            List<Object> set = lst.Distinct().ToList();
            return set;
        }



        /// <summary>
        /// Test plane normals against guide vector and angular tolerance.
        /// </summary>
        /// <param name="planeList">Planes to test</param>
        /// <param name="guide">Guide vector</param>
        /// <param name="tolerance">Angular tolerance (degrees)</param>
        /// <returns></returns>
        [MultiReturn(new[] { "passed", "failed" })]
        public static Dictionary<string, List<Plane>> testAngular_Plane(List<Plane> planeList, [DefaultArgumentAttribute("{Vector.ByCoordinates(0,0,-1)}")] Vector guide, double tolerance = 120)
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
        /// Test coordinate system Z-axes against guide vector and angular tolerance.
        /// </summary>
        /// <param name="csList">Coordinate systems to test</param>
        /// <param name="guide">Guide vector</param>
        /// <param name="tolerance">Angular tolerance (degrees)</param>
        /// <returns></returns>
        [MultiReturn(new[] { "passed", "failed" })]
        public static Dictionary<string, List<CoordinateSystem>> testAngular_CoordSys(List<CoordinateSystem> csList, [DefaultArgumentAttribute("{Vector.ByCoordinates(0,0,-1)}")] Vector guide, double tolerance = 120)
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
        /// Aligns XAxis of plane to guide vector (default: World XAxis)
        /// </summary>
        /// <param name="planeList">List of coordinate systems</param>
        /// <param name="vec">Alignment vector for XAxes</param>
        /// <param name="degree">Angle multiplier</param>
        /// <returns></returns>
        public static List<Plane> alignByXAxis_Plane(List<Plane> planeList, [DefaultArgumentAttribute("{Vector.ByCoordinates(1,0,0)}")] Vector vec, double degree = 1)
        {
            List<Plane> newPlList = new List<Plane>();

            foreach (Plane p in planeList)
            {
                double dot = p.XAxis.Dot(vec);
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
                double dot = cs.XAxis.Dot(vec);
                double angle = Math.Acos(dot) * (-1) * degree * (180 / Math.PI);
                CoordinateSystem newCS = cs.Rotate(cs.Origin, cs.ZAxis, angle);
                newCSList.Add(newCS);
            }
            return newCSList;
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
        /// For each point get list of and reorient all lines.
        /// </summary>
        /// <param name="lines">List of lines</param>
        /// <param name="points">List of points</param>
        /// <returns></returns>
        public static List<List<Line>> getLinesAtPoint(List<Line> lines, List<Point> points)
        {
            List<List<Line>> lineListList = new List<List<Line>>();
            List<Point> errPoints = new List<Point>();

            foreach (Point p in points)
            {
                List<Line> lineList = new List<Line>();
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
                        }
                        else
                        {
                            Line lRev = (Line) l.Reverse();
                            lineList.Add(lRev);
                        }
                    }
                }
                lineListList.Add(lineList);
            }

            return lineListList;
        }



        private static string Tool(Plane p)
        {
            List<double> q = Utilities.QuatListAtPlane(p);
            Point o = p.Origin;
            string t = "[TRUE, [[" + o.X + "," + o.Y + "," + o.Z + "], [" + q[0] + "," + q[1] + "," + q[2] + "," + q[3] + "]], [1,[0,0,0.001],[1,0,0,0],0,0,0]]";
            return t;
        }



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

        private static string Wobj(Plane p)
        {
            List<double> q = Utilities.QuatListAtPlane(p);
            Point o = p.Origin;
            string w = "[FALSE,TRUE,\"\", [[" + o.X + "," + o.Y + "," + o.Z + "], [" + q[0] + "," + q[1] + "," + q[2] + "," + q[3] + "]], [[0,0,0.001], [1,0,0,0]]]";
            return w;
        }



        private static string WobjOU(Plane pO, Plane pU)
        {
            Point oO = pO.Origin;
            Point oU = pU.Origin;
            List<double> qO = Utilities.QuatListAtPlane(pO);
            List<double> qU = Utilities.QuatListAtPlane(pU);
            string w = "[TRUE, TRUE, \"\", [[" + oO.X + "," + oO.Y + "," + oO.Z + "], [" + qO[0] + "," + qO[1] + "," + qO[2] + "," + qO[3] + "]], [[" + oU.X + "," + oU.Y + "," + oU.Z + "], [" + qU[0] + "," + qU[1] + "," + qU[2] + "," + qU[3] + "]]]";
            return w;
        }



        private static string WobjOU2(Plane pU, double offset)
        {
            Point oU = pU.Origin;
            List<double> qU = Utilities.QuatListAtPlane(pU);
            string w = "[TRUE, TRUE, \"\", [[0,0,0], [1,0,0,0]], [[" + oU.X + "," + oU.Y + "," + Math.Abs(offset) + "], [" + qU[0] + "," + qU[1] + "," + qU[2] + "," + qU[3] + "]]]";
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
        public static Dictionary<string, List<string>> createDrillRoutine3(string directory, string filePrefix, List<List<Plane>> holeFrames, Plane blockFrame, Plane drillFrame)
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
                    targBuilder.Append(string.Format("\n\tVAR robtarget p{0}0 := {1};", index, rtarget((Plane) hole.Translate(hole.Normal, -100))));
                    targBuilder.Append(string.Format("\n\tVAR robtarget p{0}1 := {1};", index, rtarget((Plane) hole.Translate(hole.Normal, -30))));
                    targBuilder.Append(string.Format("\n\tVAR robtarget p{0}2 := {1};", index, rtarget((Plane) hole.Translate(hole.Normal, 0))));

                    // create movement instructions
                    moveBuilder.Append(string.Format("\n"));
                    moveBuilder.Append(string.Format("\n\t\tTPWrite(\"Drilling hole {0} of {1}!\");", index + 1, holes.Count()));
                    moveBuilder.Append(string.Format("\n\t\tMoveL p{0}0, {1}, {2}, drill\\WObj:=block;", index, "v100", "z5"));
                    moveBuilder.Append(string.Format("\n\t\tMoveL p{0}1, {1}, {2}, drill\\WObj:=block;", index, "v100", "z5"));
                    moveBuilder.Append(string.Format("\n\t\tMoveL p{0}2, {1}, {2}, drill\\WObj:=block;", index, "rate", "fine"));
                    moveBuilder.Append(string.Format("\n\t\tMoveL p{0}1, {1}, {2}, drill\\WObj:=block;", index, "rate", "fine"));
                    moveBuilder.Append(string.Format("\n\t\tMoveL p{0}0, {1}, {2}, drill\\WObj:=block;", index, "v100", "z5"));

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

        




    }
}
