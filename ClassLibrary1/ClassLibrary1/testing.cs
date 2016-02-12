using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq;
using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers.RapidDomain;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;




namespace Dynamo_TORO
{
    /// <summary>
    /// testing
    /// </summary>
    public class testing
    {


        /// <summary>
        /// Create a spot-welding routine.
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="planes">Targets as planes</param>
        /// <param name="waitTime">Duration between welds (sec) </param>
        /// <param name="weldTime">Duration of weld (sec)</param>
        /// <param name="retract">Distance offset (Z) from target in tool-space</param>
        /// <returns></returns>
        [MultiReturn(new[] { "filePath", "robotCode" })]
        public static Dictionary<string, string> createWeldRoutine0(string filePath, [DefaultArgumentAttribute("{Plane.ByOriginNormal(Point.ByCoordinates(0,0,0),Vector.ByCoordinates(0,0,-1))}")] List<Plane> planes, double waitTime = 2.9, double weldTime = 0.9, double retract = -10)
        {
            // setup
            var targBuilder = new StringBuilder();
            int ct = planes.Count;
            foreach (Plane plane in planes)
            {
                RobTarget targ = Dynamo_TORO.DataTypes.RobTargetAtPlane(plane);
                string targ2 = "\n\t\t" + "[" + targ.ToString() + "],";
                if (plane == planes[planes.Count - 1]) { targ2 = "\n\t\t" + "[" + targ.ToString() + "]"; }
                targBuilder.Append(targ2);
            }

            // create rapid
            string r = "";
            using (var tw = new StreamWriter(filePath, false))
            {
                r =                         "MODULE MainModule" +
                                            "\n" +

                                            "\n\t" + "! variables" +
                                            "\n\t" + "CONST num wt:=" + waitTime + ";" +
                                            "\n\t" + "CONST num wd:=" + weldTime + ";" +
                                            "\n\t" + "CONST num dz:=" + retract + ";" +
                                            "\n" +

                                            "\n\t" + "! targets" +
                                            "\n\t" + "VAR robtarget targets{" + ct + ",1}:=" +
                                            "\n\t" + "[" + targBuilder.ToString() +
                                            "\n\t" + "];" +
                                            "\n" +

                                            "\n\t" + "! weld routine" +
                                            "\n\t" + "PROC wStart(robtarget target)" +
                                            "\n\t\t" + "MoveL RelTool(target,0,0,dz),v300,z5,t_welder\\WObj:=w_plate;" +
                                            "\n\t\t" + "MoveL target,v300,fine,t_welder\\WObj:=w_plate;" +
                                            "\n\t\t" + "SetDO welder,1;" +
                                            "\n\t\t" + "WaitTime\\InPos,wd;" +
                                            "\n\t\t" + "SetDO welder,0;" +
                                            "\n\t\t" + "MoveL RelTool(target,0,0,dz),v300,z5,t_welder\\WObj:=w_plate;" +
                                            "\n\t\t" + "WaitTime\\InPos,wt;" +
                                            "\n\t" + "ENDPROC\n" +

                                            "\n\t" + "! main routine" +
                                            "\n\t" + "PROC main()" +
                                            "\n\t\t" + "ConfL\\On;" +
                                            "\n\t\t" + "SingArea\\Wrist;" +
                                            "\n\t\t" + "FOR i FROM 1 TO " + ct + " DO" +
                                            "\n\t\t\t" + "TPWrite(valtostr(i))" + " + \" of " + ct + " \" + \"(\" + valtostr(100*i/" + ct + ") + \"%)\";" +
                                            "\n\t\t\t" + "wStart(targets{i,1});" +
                                            "\n\t\t" + "ENDFOR" +
                                            "\n\t\t" + "Stop;" +
                                            "\n\t" + "ENDPROC" +
                                            "\n" +

                                            "\n" + "ENDMODULE"
                                            ;
                tw.Write(r);
                tw.Flush();
            }

            // end step
            return new Dictionary<string, string>
            {
                {"filePath", filePath},
                {"robotCode", r}
            };
        }






        /// <summary>
        /// Create a spot-welding routine (with exit routine).
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="planes">Targets as planes</param>
        /// <param name="waitTime">Duration between welds (sec) </param>
        /// <param name="weldTime">Duration of weld (sec)</param>
        /// <param name="retract_tool">Distance offset (Z) from target in tool-space</param>
        /// <param name="retract_world">Distance offset (Z) from target in world-space</param>
        /// <returns></returns>
        [MultiReturn(new[] { "filePath", "robotCode" })]
        public static Dictionary<string, string> createWeldRoutine1(string filePath, [DefaultArgumentAttribute("{Plane.ByOriginNormal(Point.ByCoordinates(0,0,0),Vector.ByCoordinates(0,0,-1))}")] List<Plane> planes, double waitTime = 2.9, double weldTime = 0.9, double retract_tool = -10, double retract_world = 100)
        {
            // setup
            var targBuilder = new StringBuilder();
            int ct = planes.Count;
            foreach (Plane plane in planes)
            {
                RobTarget targ = Dynamo_TORO.DataTypes.RobTargetAtPlane(plane);
                string targ2 = "\n\t\t" + "[" + targ.ToString() + "],";
                if (plane == planes[planes.Count - 1]) { targ2 = "\n\t\t" + "[" + targ.ToString() + "]"; }
                targBuilder.Append(targ2);
            }

            // create rapid
            string r = "";
            using (var tw = new StreamWriter(filePath, false))
            {
                r = "MODULE MainModule" +
                                            "\n" +

                                            "\n\t" + "! variables" +
                                            "\n\t" + "CONST num wt:=" + waitTime + ";" +
                                            "\n\t" + "CONST num wd:=" + weldTime + ";" +
                                            "\n\t" + "CONST num rt:=" + retract_tool + ";" +
                                            "\n\t" + "CONST num rw:=" + retract_world + ";" +
                                            "\n" +

                                            "\n\t" + "! targets" +
                                            "\n\t" + "VAR robtarget targets{" + ct + ",1}:=" +
                                            "\n\t" + "[" + targBuilder.ToString() +
                                            "\n\t" + "];" +
                                            "\n" +

                                            "\n\t" + "! weld routine" +
                                            "\n\t" + "PROC wStart(robtarget target)" +
                                            "\n\t\t" + "MoveL Offs(RelTool(target,0,0,rt),0,0,rw),v300,z5,t_welder\\WObj:=w_plate;" +
                                            "\n\t\t" + "MoveL RelTool(target,0,0,rt),v300,z5,t_welder\\WObj:=w_plate;" +
                                            "\n\t\t" + "MoveL target,v300,fine,t_welder\\WObj:=w_plate;" +
                                            "\n\t\t" + "SetDO welder,1;" +
                                            "\n\t\t" + "WaitTime\\InPos,wd;" +
                                            "\n\t\t" + "SetDO welder,0;" +
                                            "\n\t\t" + "MoveL RelTool(target,0,0,rt),v300,z5,t_welder\\WObj:=w_plate;" +
                                            "\n\t\t" + "MoveL Offs(RelTool(target,0,0,rt),0,0,rw),v300,z5,t_welder\\WObj:=w_plate;" +
                                            "\n\t\t" + "WaitTime\\InPos,wt;" +
                                            "\n\t" + "ENDPROC\n" +

                                            "\n\t" + "! main routine" +
                                            "\n\t" + "PROC main()" +
                                            "\n\t\t" + "ConfL\\On;" +
                                            "\n\t\t" + "SingArea\\Wrist;" +
                                            "\n\t\t" + "FOR i FROM 1 TO " + ct + " DO" +
                                            "\n\t\t\t" + "TPWrite(valtostr(i))" + " + \" of " + ct + " \" + \"(\" + valtostr(100*i/" + ct + ") + \"%)\";" +
                                            "\n\t\t\t" + "wStart(targets{i,1});" +
                                            "\n\t\t" + "ENDFOR" +
                                            "\n\t\t" + "Stop;" +
                                            "\n\t" + "ENDPROC" +
                                            "\n" +

                                            "\n" + "ENDMODULE"
                                            ;
                tw.Write(r);
                tw.Flush();
            }

            // end step
            return new Dictionary<string, string>
            {
                {"filePath", filePath},
                {"robotCode", r}
            };
        }



        /// <summary>
        /// Create a spot-welding routine (with exit routine, with waittime-per-distance function).
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="planes">Targets as planes</param>
        /// <param name="waitTime">Duration between welds (sec) </param>
        /// <param name="weldTime">Duration of weld (sec)</param>
        /// <param name="retract_tool">Distance offset (Z) from target in tool-space</param>
        /// <param name="retract_world">Distance offset (Z) from target in world-space</param>
        /// <returns></returns>
        [MultiReturn(new[] { "filePath", "robotCode" })]
        public static Dictionary<string, string> createWeldRoutine2(string filePath, [DefaultArgumentAttribute("{Plane.ByOriginNormal(Point.ByCoordinates(0,0,0),Vector.ByCoordinates(0,0,-1))}")] List<Plane> planes, double waitTime = 2.9, double weldTime = 0.9, double retract_tool = -10, double retract_world = 100)
        {
            // setup
            var targBuilder = new StringBuilder();
            int ct = planes.Count;
            foreach (Plane plane in planes)
            {
                RobTarget targ = Dynamo_TORO.DataTypes.RobTargetAtPlane(plane);
                string targ2 = "\n\t\t" + "[" + targ.ToString() + "],";
                if (plane == planes[planes.Count - 1]) { targ2 = "\n\t\t" + "[" + targ.ToString() + "]"; }
                targBuilder.Append(targ2);
            }

            // create rapid
            string r = "";
            using (var tw = new StreamWriter(filePath, false))
            {
                r = "MODULE MainModule" +
                                            "\n" +

                                            "\n\t" + "! variables" +
                                            "\n\t" + "CONST num wt:=" + waitTime + ";" +
                                            "\n\t" + "CONST num wd:=" + weldTime + ";" +
                                            "\n\t" + "CONST num rt:=" + retract_tool + ";" +
                                            "\n\t" + "CONST num rw:=" + retract_world + ";" +
                                            "\n" +

                                            "\n\t" + "! targets" +
                                            "\n\t" + "VAR robtarget targets{" + ct + ",1}:=" +
                                            "\n\t" + "[" + targBuilder.ToString() +
                                            "\n\t" + "];" +
                                            "\n" +

                                            "\n\t" + "! weld routine" +
                                            "\n\t" + "PROC wStart(num i)" +
                                            "\n\t\t" + "IF (i=1) THEN" +
                                            "\n\t\t\t" + "WaitTime\\InPos,wt;" +
                                            "\n\t\t" + "ELSE" +
                                            "\n\t\t\t" + "WaitTime\\InPos,(wt/(Distance(targets{i,1}.trans,targets{i-1,1}.trans)+0.01));" +
                                            "\n\t\t" + "ENDIF" +
                                            "\n\t\t" + "MoveL Offs(RelTool(targets{i,1},0,0,-rt),0,0,rw),v300,z5,t_welder\\WObj:=w_plate;" +
                                            "\n\t\t" + "MoveL RelTool(targets{i,1},0,0,-rt),v300,z5,t_welder\\WObj:=w_plate;" +
                                            "\n\t\t" + "MoveL targets{i,1},v300,fine,t_welder\\WObj:=w_plate;" +
                                            "\n\t\t" + "SetDO welder,1;" +
                                            "\n\t\t" + "WaitTime\\InPos,wd;" +
                                            "\n\t\t" + "SetDO welder,0;" +
                                            "\n\t\t" + "MoveL RelTool(targets{i,1},0,0,-rt),v300,z5,t_welder\\WObj:=w_plate;" +
                                            "\n\t\t" + "MoveL Offs(RelTool(targets{i,1},0,0,-rt),0,0,rw),v300,z5,t_welder\\WObj:=w_plate;" +
                                            "\n\t" + "ENDPROC\n" +

                                            "\n\t" + "! main routine" +
                                            "\n\t" + "PROC main()" +
                                            "\n\t\t" + "ConfL\\On;" +
                                            "\n\t\t" + "SingArea\\Wrist;" +
                                            "\n\t\t" + "FOR i FROM 1 TO " + ct + " DO" +
                                            "\n\t\t\t" + "TPWrite(valtostr(i))" + " + \" of " + ct + " \" + \"(\" + valtostr(100*i/" + ct + ") + \"%)\";" +
                                            "\n\t\t\t" + "wStart(i);" +
                                            "\n\t\t" + "ENDFOR" +
                                            "\n\t\t" + "Stop;" +
                                            "\n\t" + "ENDPROC" +
                                            "\n" +

                                            "\n" + "ENDMODULE"
                                            ;
                tw.Write(r);
                tw.Flush();
            }

            // end step
            return new Dictionary<string, string>
            {
                {"filePath", filePath},
                {"robotCode", r}
            };
        }





        /// <summary>
        /// Sort points by Z value with indices; version 2.
        /// </summary>
        /// <param name="points">Point list</param>
        /// <returns></returns>
        [MultiReturn(new[] { "sortedPoints", "sortedIndices" })]
        public static Dictionary<string, List<object>> sortPointsByZ_2(List<Point> points)
        {
            var sorted = points
                .Select((coord, i) => new KeyValuePair<Point, int>(coord, i))
                .OrderBy(coord => coord.Key.Z)
                .ToList();

            List<object> sortedPoints = sorted.Select(coord => (object) coord.Key).ToList();
            List<object> sortedIndices = sorted.Select(i => (object) i.Value).ToList();

            // end step
            return new Dictionary<string, List<object>>
            {
                {"sortedPoints", sortedPoints},
                {"sortedIndices", sortedIndices}
            };
        }


        /// <summary>
        /// Sort planes by Z value with indices; version 2.
        /// </summary>
        /// <param name="planes">Plane list</param>
        /// <returns></returns>
        [MultiReturn(new[] { "sortedPlanes", "sortedIndices" })]
        public static Dictionary<string, List<object>> sortPlanesByZ_2(List<Plane> planes)
        {
            var sorted = planes
                .Select((coord, i) => new KeyValuePair<Point, int>(coord.Origin, i))
                .OrderBy(coord => coord.Key.Z)
                .ToList();

            List<object> sortedPlanes = sorted.Select(coord => (object)coord.Key).ToList();
            List<object> sortedIndices = sorted.Select(i => (object)i.Value).ToList();

            // end step
            return new Dictionary<string, List<object>>
            {
                {"sortedPlanes", sortedPlanes},
                {"sortedIndices", sortedIndices}
            };
        }


        private static string Tool(Plane p)
        {
            List<double> q = Utilities.QuatListAtPlane(p);
            Point o = p.Origin;
            string t = "[TRUE, [["+o.X + "," + o.Y + "," + o.Z + "], [" +q[0] + "," + q[1] + "," + q[2] + "," + q[3] + "]], [1,[0,0,0.001],[1,0,0,0],0,0,0]]";
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

        private static string WobjOU2(Plane pO, Plane pU, double offset)
        {
            //Point oO = pO.Origin;
            Point oU = pU.Origin;
            //List<double> qO = Utilities.QuatListAtPlane(pO);
            List<double> qU = Utilities.QuatListAtPlane(pU);
            string w = "[TRUE, TRUE, \"\", [[0,0,0], [1,0,0,0]], [[" + oU.X + "," + oU.Y + "," + Math.Abs(offset) + "], [" + qU[0] + "," + qU[1] + "," + qU[2] + "," + qU[3] + "]]]";
            return w;
        }


        /// <summary>
        /// Create a drilling routine for an ABB robot.
        /// </summary>
        /// <param name="directory">Directory to create files (ex: "C:\Users\You\Desktop")</param>
        /// <param name="filenamePrefix">Naming convention for file prefix (ex: "Group_Title")</param>
        /// <param name="cubeCenterFrame">Location of cube center as a plane (centroid and orientation)</param>
        /// <param name="faceFrames">List of planes to drill for each cube (position and orientation)</param>
        /// <param name="drillFrame">Location of drill bit frame</param>
        /// <param name="calibFrame">Location of calibration frame</param>
        /// <param name="mountOffset">Distance from mounting flange to centroid of block</param>
        /// <returns></returns>
        [MultiReturn(new[] { "filePaths" })]
        public static Dictionary<string, List<string>> createDrillRoutine(string directory, string filenamePrefix, Plane cubeCenterFrame, List<List<Plane>> faceFrames, Plane drillFrame, Plane calibFrame, double mountOffset)
        {
            // create list of filenames
            List<string> outputFiles = new List<string>();

            // create tools
            var toolBuilder = new StringBuilder();
            string tCalibFrame = string.Format("\n\tTASK PERS tooldata {0} := {1};", "calibBox", ToolRobHold(calibFrame,false));
            string tDrillFrame = string.Format("\n\tTASK PERS tooldata {0} := {1};", "drillBit", ToolRobHold(drillFrame,false));
            toolBuilder.Append(tCalibFrame);
            toolBuilder.Append(tDrillFrame);

            /*
            //create targets
            var targBuilder = new StringBuilder();
            string p0 = "\n\tVAR robtarget p0 := [[0,0,-400], [1,0,0,0], [0,1,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];";
            string p1 = "\n\tVAR robtarget p1 := [[0,0,-30], [1,0,0,0], [0,1,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];";
            string p2 = "\n\tVAR robtarget p2 := [[0,0,0], [1,0,0,0], [0,1,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];";
            targBuilder.Append(p0);
            targBuilder.Append(p1);
            targBuilder.Append(p2);
            */

            // loop through cubePlanes, facePlanes
            //using (var e1 = cubeCenterFrame.GetEnumerator())
            using (var e2 = faceFrames.GetEnumerator())
            {
                while (/*e1.MoveNext() &&*/ e2.MoveNext())
                {
                    //var wCubeFrame = e1.Current;
                    var wFaceFrameList = e2.Current;

                    // create filename
                    int index = faceFrames.IndexOf(wFaceFrameList);
                    string paddedIndex = index.ToString().PadLeft(3, '0');
                    string filename = string.Format("{0}\\{1}_{2}.prg", directory, filenamePrefix, paddedIndex);
                    outputFiles.Add(filename);

                    // create wobjdata, drilling program
                    int jndex = 0;
                    var wobjBuilder = new StringBuilder();
                    var drillBuilder = new StringBuilder();
                    var targBuilder = new StringBuilder();

                    foreach (Plane this_wFaceFrame in wFaceFrameList)
                    {
                        // create wobjdata
                        string wobj = WobjOU2(cubeCenterFrame, this_wFaceFrame, mountOffset);
                        string wobjOut = string.Format("\n\tTASK PERS wobjdata face{0} := {1};", jndex, wobj);
                        wobjBuilder.Append(wobjOut);

                        // create target data
                        string p0ex = "\n\tTASK PERS robtarget := [[0,0,-400], [1,0,0,0], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];";
                        string p1ex = "\n\tTASK PERS robtarget := [[0,0,-30], [1,0,0,0], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];";
                        string p2ex = "\n\tTASK PERS robtarget := [[0,0,0], [1,0,0,0], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];";
                        targBuilder.Append(p0ex);
                        targBuilder.Append(p1ex);
                        targBuilder.Append(p2ex);

                        // test dot product
                        string p0 = "[[0,0,-400], [1,0,0,0], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];";
                        string p1 = "[[0,0,-30], [1,0,0,0], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];";
                        string p2 = "[[0,0,0], [1,0,0,0], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];";
                        if (this_wFaceFrame.XAxis.Dot(Vector.ByCoordinates(1, 0, 0)) < 1)
                        {
                            p0 = "[[0,0,-400], [1,0,0,0], [0,1,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]]";
                            p1 = "[[0,0,-30], [1,0,0,0], [0,1,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]]";
                            p2 = "[[0,0,0], [1,0,0,0], [0,1,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]]";
                        }

                        // create drilling program
                        string w0 = string.Format("\n\n\t\tTPWrite(\"DRILLING {0} of {1}\");", jndex+1, wFaceFrameList.Count);
                        string d0 = string.Format("\n\t\tMoveL {0}, {1}, {2}, {3}\\WObj:=face{4};", p0, "v200", "z5", "drillBit", jndex);
                        string d1 = string.Format("\n\t\tMoveL {0}, {1}, {2}, {3}\\WObj:=face{4};", p1, "v100", "z5", "drillBit", jndex);
                        string d2 = string.Format("\n\t\tMoveL {0}, {1}, {2}, {3}\\WObj:=face{4};", p2, "peckSpeed", "z5", "drillBit", jndex);
                        string d3 = string.Format("\n\t\tMoveL {0}, {1}, {2}, {3}\\WObj:=face{4};", p1, "peckSpeed", "z5", "drillBit", jndex);
                        string d4 = string.Format("\n\t\tMoveL {0}, {1}, {2}, {3}\\WObj:=face{4};", p0, "v200", "z5", "drillBit", jndex);
                        drillBuilder.Append(w0);
                        drillBuilder.Append(d0);
                        drillBuilder.Append(d1);
                        drillBuilder.Append(d2);
                        drillBuilder.Append(d3);
                        drillBuilder.Append(d4);
                        jndex++;
                    }

                    // create calibration wobj
                    string wBaseFrame = string.Format("\n\tTASK PERS wobjdata baseFrame := [TRUE, TRUE, \"\", [[0,0,{0}], [1,0,0,0]], [[0,0,19], [1,0,0,0]]];", (mountOffset));
                    wobjBuilder.Append(wBaseFrame);

                    // create rapid
                    string r = "";
                    using (var tw = new StreamWriter(filename, false))
                    {
                        r =
                            "MODULE MainModule" +
                            "\n" +

                            "\n\t" + "! origin of calibration box and drillbit" + toolBuilder.ToString() +
                            "\n" +

                            "\n\t" + "! origin of work-planes on cuboid" + wobjBuilder.ToString() +
                            "\n" +

                            "\n\t" + "! targets for reset" +
                            "\n\t" + "VAR jointtarget j0 := [[-90,0,0,90,90,0], [9E9,9E9,9E9,9E9,9E9,9E9]];" +
                            "\n\t" + "VAR jointtarget j1 := [[0,0,0,0,90,0], [9E9,9E9,9E9,9E9,9E9,9E9]];" + targBuilder +
                            "\n" +

                            "\n\t" + "! speed for drilling" +
                            "\n\t" + "TASK PERS speeddata peckSpeed := [4,500,5000,1000];" +
                            "\n" +

                            "\n\t" + "! drilling instructions" +
                            "\n\t" + "PROC main()" +
                            "\n\t\t" + "ConfL\\Off;" +
                            "\n\t\t" + "SingArea\\Wrist;" +
                            "\n" +

                            "\n\t\t" + "TPWrite(\"THIS IS: " + filenamePrefix + "_" + paddedIndex + "\");" +
                            "\n\t\t" + "TPWrite(\"RESETTING AXES...\");" +
                            "\n\t\t" + "MoveAbsJ j0, v100, fine, tool0;" +
                            "\n\t\t" + "MoveAbsJ j1, v100, fine, tool0;" +
                            "\n\t\t" + "MoveL p2, v50, fine, calibBox\\WObj:=baseFrame;" +
                            "\n\t\t" + "MoveL p1, v10, fine, calibBox\\WObj:=baseFrame;" +
                            "\n\t\t" + "TPWrite(\"LOAD A BLOCK AND SWITCH DRILL TO ON\");" +
                            "\n\t\t" + "WaitTime 3;" +
                            "\n" +

                            "\n\t\t" + "MoveL p2, v50, fine, calibBox\\WObj:=baseFrame;" +
                            "\n\t\t" + "MoveAbsJ j1, v100, fine, tool0;" +
                            "\n\t\t" + "MoveAbsJ j0, v100, fine, tool0;" +

                            drillBuilder +

                            "\n\n\t\t" + "TPWrite(\"RESETTING AXES...\");" +
                            "\n\t\t" + "MoveAbsJ j0, v100, fine, tool0;" +
                            "\n\t\t" + "Stop;" +
                            "\n\t" + "ENDPROC" +
                            "\n" +

                            "\n" + "ENDMODULE"
                            ;
                        tw.Write(r);
                        tw.Flush();
                    }

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
