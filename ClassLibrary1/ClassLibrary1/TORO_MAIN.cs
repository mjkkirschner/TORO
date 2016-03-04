using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers.RapidDomain;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;



//////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////

//authored by Nick Cote, 2015
//authored by Nick Cote, 2015

//implemented CreateRapid based on logic from Dynamo_ABB from Autodesk, Inc. Waltham at Virginia Tech Robotics Summit, 2015
//with contributions from Matt Jezyk and Mike Dewberry

//implemented PlaneToQuaternian based on logic from the Design Robotics Group at Harvard Gsd
//with contributions from Sola Grantham, Anthony Kane, Nathan King, Jonathan Grinham, and others. 



//////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////



namespace Dynamo_TORO
{


    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////


    /// <summary>
    /// Create datatype.
    /// </summary>
    public class DataTypes
    {


        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Create robot target from coordinate and quaternion values.
        /// </summary>
        /// <param name="ptX">Coordinate</param>
        /// <param name="ptY">Coordinate</param>
        /// <param name="ptZ">Coordinate</param>
        /// <param name="q1">Quaternion</param>
        /// <param name="q2">Quaternion</param>
        /// <param name="q3">Quaternion</param>
        /// <param name="q4">Quaternion</param>
        /// <returns></returns>
        public static RobTarget RobTargetAtVals(double ptX = 0, double ptY = 0, double ptZ = 0, double q1 = 1, double q2 = 0, double q3 = 0, double q4 = 0)
        {
            var target = new RobTarget();
            {
                target.FillFromString2(
                    string.Format(
                        "[[{0},{1},{2}],[{3},{4},{5},{6}],[0,0,0,0],[9.999999999E09,9.999999999E09,9.999999999E09,9.999999999E09,9.999999999E09,9.999999999E09]];",
                        ptX, ptY, ptZ, q1, q2, q3, q4));
            }
            return target;
        }

        /// <summary>
        /// Create robot target from point and quaternion values.
        /// </summary>
        /// <param name="point">Point</param>
        /// <param name="q1">Quaternion</param>
        /// <param name="q2">Quaternion</param>
        /// <param name="q3">Quaternion</param>
        /// <param name="q4">Quaternion</param>
        /// <returns></returns>
        public static RobTarget RobTargetAtPoint([DefaultArgumentAttribute("Point.ByCoordinates(0,0,0)")] Point point, double q1 = 1, double q2 = 0, double q3 = 0, double q4 = 0)
        {
            var target = new RobTarget();
            if (point != null)
            {
                target.FillFromString2(
                    string.Format(
                        "[[{0},{1},{2}],[{3},{4},{5},{6}],[0,0,0,0],[9.999999999E09,9.999999999E09,9.999999999E09,9.999999999E09,9.999999999E09,9.999999999E09]];",
                        point.X, point.Y, point.Z, q1, q2, q3, q4));
            }
            return target;
        }

        /// <summary>
        /// Create robot target from plane.
        /// </summary>
        /// <param name="plane">Plane</param>
        /// <returns></returns>
        public static RobTarget RobTargetAtPlane([DefaultArgumentAttribute("Plane.ByOriginNormal(Point.ByCoordinates(0,0,0),Vector.ByCoordinates(0,0,1))")] Plane plane)
        {
            var target = new RobTarget();
            if (plane != null)
            {
                List<double> quatDoubles = RobotUtils.PlaneToQuaternian(plane);
                target.FillFromString2(
                    string.Format(
                        "[[{0},{1},{2}],[{3},{4},{5},{6}],[0,0,0,0],[9.999999999E09,9.999999999E09,9.999999999E09,9.999999999E09,9.999999999E09,9.999999999E09]];",
                        plane.Origin.X, plane.Origin.Y, plane.Origin.Z, quatDoubles[0], quatDoubles[1], quatDoubles[2], quatDoubles[3]));
            }
            return target;
        }

        /// <summary>
        /// Create joint target from rotational values per axis.
        /// </summary>
        /// <param name="j1">Degrees</param>
        /// <param name="j2">Degrees</param>
        /// <param name="j3">Degrees</param>
        /// <param name="j4">Degrees</param>
        /// <param name="j5">Degrees</param>
        /// <param name="j6">Degrees</param>
        /// <returns></returns>
        public static JointTarget JointTargetAtVals(double j1 = 0, double j2 = 0, double j3 = 0, double j4 = 0, double j5 = 0, double j6 = 0)
        {
            var target = new JointTarget();
            target.FillFromString2(
                string.Format("[[{0},{1},{2},{3},{4},{5}],[9.999999999E09,9.999999999E09,9.999999999E09,9.999999999E09,9.999999999E09,9.999999999E09]];",
                j1, j2, j3, j4, j5, j6));
            return target;
        }



        /// <summary>
        /// Define speeddata.
        /// </summary>
        /// <param name="varName">Name of speeddata variable</param>
        /// <param name="v_tcp">Speed at tool center point in mm/s</param>
        /// <param name="v_ori">Reorientation speed of the TCP in deg</param>
        /// <param name="v_leax">Linear speed of external axes in mm/s</param>
        /// <param name="v_reax">Rotational speed of external axes in deg</param>
        /// <returns></returns>
        public static string Speeddata(string varName = "speed", double v_tcp = 250, double v_ori = 500, double v_leax = 5000, double v_reax = 1000)
        {
            string speed = string.Format("VAR speeddata {0}:=[{1},{2},{3},{4}];", varName, v_tcp, v_ori, v_leax, v_reax);
            return speed;
        }

        /// <summary>
        /// Define zonedata.
        /// </summary>
        /// <param name="varName">Name of zonedata variable</param>
        /// <param name="pfine">T: stop-point | F: fly-by-point</param>
        /// <param name="pzone_tcp">Radius for TCP</param>
        /// <param name="pzone_ori">Radius for reorientation</param>
        /// <param name="pzone_eax">Radius for external axes</param>
        /// <param name="zone_ori">Radius for tool reorientation in deg</param>
        /// <param name="zone_leax">Radius for linear external axes in mm</param>
        /// <param name="zone_reax">Radius for rotating external axes in deg</param>
        /// <returns></returns>
        public static string Zonedata(string varName = "zone", bool pfine = false, double pzone_tcp = 10, double pzone_ori = 15, double pzone_eax = 15, double zone_ori = 1.5, double zone_leax = 15, double zone_reax = 1.5)
        {
            string zone = string.Format("VAR zonedata {0}:=[{1},{2},{3},{4},{5},{6},{7}];", varName, pfine, pzone_tcp, pzone_ori, pzone_eax, zone_ori, zone_leax, zone_reax);
            return zone;
        }

        /// <summary>
        /// Define loaddata.
        /// </summary>
        /// <param name="varName">Name of loaddata variable</param>
        /// <param name="load">Load in kg</param>
        /// <param name="cog_x">Center of gravity coordinate</param>
        /// <param name="cog_y">Center of gravity coordinate</param>
        /// <param name="cog_z">Center of gravity coordinate</param>
        /// <param name="aom_q1">Axes of moment quaternion</param>
        /// <param name="aom_q2">Axes of moment quaternion</param>
        /// <param name="aom_q3">Axes of moment quaternion</param>
        /// <param name="aom_q4">Axes of moment quaternion</param>
        /// <param name="ix">Inertia in kgm^2</param>
        /// <param name="iy">Inertia in kgm^2</param>
        /// <param name="iz">Inertia in kgm^2</param>
        /// <returns></returns>
        public static string Loaddata(string varName = "load", double load = 1, double cog_x = 0, double cog_y = 0, double cog_z = 0.001, double aom_q1 = 1, double aom_q2 = 0, double aom_q3 = 0, double aom_q4 = 0, double ix = 0, double iy = 0, double iz = 0)
        {
            string loadData = string.Format("PERS loaddata {0}:=[{1},[{2},{3},{4}],[{5},{6},{7},{8}],{9},{10},{11}];", varName, load, cog_x, cog_y, cog_z, aom_q1, aom_q2, aom_q3, aom_q4, ix, iy, iz);
            return loadData;
        }

        /// <summary>
        /// Define confdata.
        /// </summary>
        /// <param name="varName">Name of confdata variable</param>
        /// <param name="cf1">Current quadrant or meter interval of axis 1</param>
        /// <param name="cf4">Current quadrant or meter interval of axis 4</param>
        /// <param name="cf6">Current quadrant or meter interval of axis 6</param>
        /// <param name="cfx">Current quadrant or meter interval of axis 2 | X</param>
        /// <returns></returns>
        public static string Confdata(string varName = "conf", double cf1 = 0, double cf4 = 0, double cf6 = 0, double cfx = 0)
        {
            string conf = string.Format("PERS confdata {0}:=[{1},{2},{3},{4}];", varName, cf1, cf4, cf6, cfx);
            return conf;
        }




        /*
        /// <summary>
        /// Define motion set data.
        /// </summary>
        /// <param name="varName">Name of motsetdata variable</param>
        /// <param name="vel_oride">Velocity as a percentage of programmed velocity.</param>
        /// <param name="vel_max">Maximum velocity in mm/s.</param>
        /// <param name="acc_acc">Acceleration and deceleration as a percentage of the normal values.</param>
        /// <param name="acc_ramp">The rate by which acceleration and deceleration increases as a percentage of the normal values. </param>
        /// <param name="sing_wrist">The orientation of the tool is allowed to deviate somewhat in order to prevent wrist singularity. </param>
        /// <param name="sing_arm">The orientation of the tool is allowed to deviate somewhat in order to prevent arm singularity (not implemented).</param>
        /// <param name="sing_base">The orientation of the tool is not allowed to deviate. </param>
        /// <param name="conf_jsup">Supervision of joint configuration is active during joint movement. </param>
        /// <param name="conf_lsup">Supervision of joint configuration is active during linear and circular movement. </param>
        /// <param name="conf_ax1">Maximum permitted deviation in degrees for axis 1 (not used in this version). </param>
        /// <param name="conf_ax4">Maximum permitted deviation in degrees for axis 4 (not used in this version). </param>
        /// <param name="conf_ax6">Maximum permitted deviation in degrees for axis 6 (not used in this version). </param>
        /// <param name="pathresol">Current override in percentage of the configured path resolution.</param>
        /// <param name="motionsup">Mirror RAPID status (TRUE = On and FALSE = Off) of motion supervision function.</param>
        /// <param name="tunevalue">Current RAPID override as a percentage of the configured tunevalue for the motion supervision function.</param>
        /// <param name="acclim">Limitation of tool acceleration along the path. (TRUE = limitation and FALSE = no limitation).</param>
        /// <param name="accmax">TCP acceleration limitation in m/s2. If acclim is FALSE, the value is always set to -1.</param>
        /// <param name="decellim">Limitation of tool deceleration along the path. (TRUE = limitation and FALSE = no limitation).</param>
        /// <param name="decelmax">TCP deceleration limitation in m/s2. If decellim is FALSE, the value is always set to -1.</param>
        /// <param name="cirpathreori">Tool reorientation during circle path: 0 = Interpolation in path frame; 1 = Interpolation in object frame; 2 = Programmed tool orientation in CirPoint</param>
        /// <param name="worldacclim">Limitation of acceleration in world coordinate system. (TRUE = limitation and FALSE = no limitation).</param>
        /// <param name="worldaccmax">Limitation of acceleration in world coordinate system in m/s2. If worldacclim is FALSE, the value is always set to -1.</param>
        /// <param name="evtbufferact">Event buffer active or not active. (TRUE = event buffer active and FALSE = event buffer not active).</param>
        /// <returns></returns>
        public static string Motsetdata(string varName, double vel_oride, double vel_max, double acc_acc, double acc_ramp, bool sing_wrist, bool sing_arm, bool sing_base, bool conf_jsup, bool conf_lsup, double conf_ax1, double conf_ax4, double conf_ax6, double pathresol, bool motionsup, double tunevalue, bool acclim, double accmax, bool decellim, double decelmax, int cirpathreori, bool worldacclim, double worldaccmax, bool evtbufferact)
        {
            string motset = string.Format("\n\tVAR motsetdata {0}:=" + "[{1},{2}],\n" +
                                                                        "[{3},{4}],\n" +
                                                                        "[{5},{6},{7}],\n" +
                                                                        "[{8},{9},{10},{11},{12},{13}],\n" +
                                                                        "{14},\n" +
                                                                        "{15},\n" +
                                                                        "{16},\n" +
                                                                        "{17},\n" +
                                                                        "{18},\n" +
                                                                        "{19},\n" +
                                                                        "{20},\n" +
                                                                        "{21},\n" +
                                                                        "{22},\n" +
                                                                        "{23},\n" +
                                                                        "{24},\n",
                                                                        varName,
                                                                        vel_oride, vel_max,
                                                                        acc_acc, acc_ramp,
                                                                        sing_wrist, sing_arm, sing_base,
                                                                        conf_jsup, conf_lsup, conf_ax1, conf_ax4, conf_ax6,
                                                                        pathresol,
                                                                        motionsup,
                                                                        tunevalue,
                                                                        acclim,
                                                                        accmax,
                                                                        decellim,
                                                                        decelmax,
                                                                        cirpathreori,
                                                                        worldacclim,
                                                                        worldaccmax,
                                                                        evtbufferact);
            return motset;
        }

        /// <summary>
        /// Define stoppointdata
        /// </summary>
        /// <param name="varName">Name of stoppointdata variable</param>
        /// <param name="progsynch">Sychronization with RAPID program execution</param>
        /// <param name="type">1 = inpos; 2 = stoptime; 3 = followtime</param>
        /// <param name="inpos_position">Position condition for TCP</param>
        /// <param name="inpos_speed">Speed condition for TCP</param>
        /// <param name="inpos_mintime">Minimum wait time</param>
        /// <param name="inpos_maxtime">Maximum wait time</param>
        /// <param name="stoptime">Time stopped</param>
        /// <param name="followtime">Follow time</param>
        /// <returns></returns>
        public static string Stoppointdata(string varName, bool progsynch, string type, int inpos_position, int inpos_speed, double inpos_mintime, double inpos_maxtime, double stoptime, double followtime)
        {
            string stop = string.Format("\n\tVAR stoppointdata {0}:= [{1},{2},[{3},{4},{5},{6}],{7},{8},'',0,0];", varName);
            return stop;
        }

    */





        /// <summary>
        /// Define shapedata and create work-zone instruction for box.
        /// </summary>
        /// <param name="varName">Name of shapedata variable</param>
        /// <param name="Inside_Outside">Define as volume "Inside" | "Outside"</param>
        /// <param name="lo_x">Low point coordinate</param>
        /// <param name="lo_y">Low point coordinate</param>
        /// <param name="lo_z">Low point coordinate</param>
        /// <param name="hi_x">High point coordinate</param>
        /// <param name="hi_y">High point coordinate</param>
        /// <param name="hi_z">High point coordinate</param>
        /// <returns></returns>
        [MultiReturn(new[] { "cnst", "inst" })]
        public static Dictionary<string, string> WZBoxAtVals(string varName, string Inside_Outside, double lo_x, double lo_y, double lo_z, double hi_x, double hi_y, double hi_z)
        {
            string cnst = string.Format("VAR shapedata {0};",
                                        varName);
            string inst = string.Format("WZBoxDef \\{0},{1},[{2},{3},{4}],[{5},{6},{7}];",
                                        Inside_Outside, varName, lo_x, lo_y, lo_z, hi_x, hi_y, hi_z);

            return new Dictionary<string, string>
            {
                {"cnst", cnst},
                {"inst", inst},
             };
        }

        /// <summary>
        /// Define shapedata and create work-zone instruction for box.
        /// </summary>
        /// <param name="varName">Name of shapedata variable</param>
        /// <param name="Inside_Outside">Define as volume "Inside" | "Outside"</param>
        /// <param name="box">Cuboidic work-zone</param>
        /// <returns></returns>
        [MultiReturn(new[] { "cnst", "inst" })]
        public static Dictionary<string, string> WZBoxAtGeometry(string varName, string Inside_Outside, Cuboid box)
        {
            double lo_x = box.Vertices[6].PointGeometry.X;
            double lo_y = box.Vertices[6].PointGeometry.Y;
            double lo_z = box.Vertices[6].PointGeometry.Z;
            double hi_x = box.Vertices[1].PointGeometry.X;
            double hi_y = box.Vertices[1].PointGeometry.Y;
            double hi_z = box.Vertices[1].PointGeometry.Z;
            string cnst = string.Format("VAR shapedata {0};",
                                        varName);
            string inst = string.Format("WZBoxDef \\{0},{1},[{2},{3},{4}],[{5},{6},{7}];",
                                        Inside_Outside, varName, lo_x, lo_y, lo_z, hi_x, hi_y, hi_z);

            return new Dictionary<string, string>
            {
                {"cnst", cnst},
                {"inst", inst},
             };
        }


        /// <summary>
        /// Define shapedata and create work-zone instruction for cylinder.
        /// </summary>
        /// <param name="varName">Name of shapedata variable</param>
        /// <param name="Inside_Outside">Define as volume "Inside" | "Outside"</param>
        /// <param name="center_x">Coordinate</param>
        /// <param name="center_y">Coordinate</param>
        /// <param name="center_z">Coordinate</param>
        /// <param name="radius">Radius</param>
        /// <param name="height">Height</param>
        /// <returns></returns>
        [MultiReturn(new[] { "cnst", "inst" })]
        public static Dictionary<string, string> WZCylAtVals(string varName, string Inside_Outside, double center_x, double center_y, double center_z, double radius, double height)
        {
            if (radius < 5)
            { radius = 5; }
            string cnst = string.Format("VAR wzstationary wz{0};" +
                                        "\tVAR shapedata {0};\n",
                                        varName);
            string inst = string.Format("WZCylDef \\{0},{1},[{2},{3},{4}],{5},{6};\n" +
                                        "\t\tWZLimSup \\Stat, wz{1}, {1};\n",
                                       Inside_Outside, varName, center_x, center_y, center_z, radius, height);

            return new Dictionary<string, string>
            {
                {"cnst", cnst},
                {"inst", inst},
             };
        }

        /// <summary>
        /// Define shapedata and create work-zone instruction for cylinder.
        /// </summary>
        /// <param name="varName">Name of shapedata variable</param>
        /// <param name="Inside_Outside">Define as volume "Inside" | "Outside"</param>
        /// <param name="cylinder">Cylindrical work-zone</param>
        /// <returns></returns>
        [MultiReturn(new[] { "cnst", "inst" })]
        public static Dictionary<string, string> WZCylAtGeometry(string varName, string Inside_Outside, Cylinder cylinder)
        {
            double center_x = cylinder.StartPoint.X;
            double center_y = cylinder.StartPoint.Y;
            double center_z = cylinder.StartPoint.Z;
            double radius = cylinder.Radius;
            double height = cylinder.Height;
            if (radius < 5)
            { radius = 5; }
            string cnst = string.Format("VAR wzstationary wz{0};" +
                                        "\tVAR shapedata {0};\n",
                                        varName);
            string inst = string.Format("WZCylDef \\{0},{1},[{2},{3},{4}],{5},{6};\n" +
                                        "\t\tWZLimSup \\Stat, wz{1}, {1};\n",
                                       Inside_Outside, varName, center_x, center_y, center_z, radius, height);

            return new Dictionary<string, string>
            {
                {"cnst", cnst},
                {"inst", inst},
             };
        }

        /// <summary>
        /// Define shapedata and create work-zone instruction for sphere.
        /// </summary>
        /// <param name="varName">Name of shapedata variable</param>
        /// <param name="Inside_Outside">Define as volume "Inside" | "Outside"</param>
        /// <param name="center_x">Coordinate</param>
        /// <param name="center_y">Coordinate</param>
        /// <param name="center_z">Coordinate</param>
        /// <param name="radius">Radius</param>
        /// <returns></returns>
        [MultiReturn(new[] { "cnst", "inst" })]
        public static Dictionary<string, string> WZSphAtVals(string varName, string Inside_Outside, double center_x, double center_y, double center_z, double radius)
        {
            string cnst = string.Format("VAR shapedata {0};",
                                        varName);
            string inst = string.Format("WZBoxDef \\{0},{1},[{2},{3},{4}],{5};",
                                        Inside_Outside, varName, center_x, center_y, center_z, radius);

            return new Dictionary<string, string>
            {
                {"cnst", cnst},
                {"inst", inst},
             };
        }

        /// <summary>
        /// Define shapedata and create work-zone instruction for sphere.
        /// </summary>
        /// <param name="varName">Name of shapedata variable</param>
        /// <param name="Inside_Outside">Define as volume "Inside" | "Outside"</param>
        /// <param name="sphere">Spherical work-zone</param>
        /// <returns></returns>
        [MultiReturn(new[] { "cnst", "inst" })]
        public static Dictionary<string, string> WZSphAtGeometry(string varName, string Inside_Outside, Sphere sphere)
        {
            double center_x = sphere.CenterPoint.X;
            double center_y = sphere.CenterPoint.Y;
            double center_z = sphere.CenterPoint.Z;
            double radius = sphere.Radius;
            string cnst = string.Format("VAR shapedata {0};",
                                        varName);
            string inst = string.Format("WZBoxDef \\{0},{1},[{2},{3},{4}],{5};",
                                        Inside_Outside, varName, center_x, center_y, center_z, radius);

            return new Dictionary<string, string>
            {
                {"cnst", cnst},
                {"inst", inst},
             };
        }


        /// <summary>
        /// Define joint-targets for joint limits.
        /// </summary>
        /// <param name="varName">Name of variables</param>
        /// <param name="Inside_Outside">Define as volume "Inside" | "Outside"</param>
        /// <param name="loJointVal">JointTarget</param>
        /// <param name="hiJointVal">JointTarget</param>
        /// <returns></returns>
        [MultiReturn(new[] { "cnst", "inst" })]
        public static Dictionary<string, string> WZLimJointDef(string varName, string Inside_Outside, JointTarget loJointVal, JointTarget hiJointVal)
        {
            string cnst = string.Format("VAR wzstationary wl{0};" +
                                        "VAR shapedata js{0};" +
                                        "CONST jointtarget lo{0}:={1};" +
                                        "CONST jointtarget hi{0}:={2};",
                                        varName, loJointVal, hiJointVal);
            string inst = string.Format("WZLimJointDef \\{0},js{1},lo{1},hi{1};" +
                                        "WzLimSup \\Stat, wl{1},js{1};",
                                        Inside_Outside, varName);

            return new Dictionary<string, string>
            {
                {"cnst", cnst},
                {"inst", inst},
             };
        }

        /// <summary>
        /// Define tooldata from coordinate, quaternion, and load values.
        /// </summary>
        /// <param name="x">Coordinate</param>
        /// <param name="y">Coordinate</param>
        /// <param name="z">Coordinate</param>
        /// <param name="q1">Quaternion</param>
        /// <param name="q2">Quaternion</param>
        /// <param name="q3">Quaternion</param>
        /// <param name="q4">Quaternion</param>
        /// <param name="load">Load in kg</param>
        /// <param name="name">Name of tooldata variable</param>
        /// <returns></returns>
        public static List<string> ToolAtVals(double x = 0, double y = 0, double z = 0.001, double q1 = 1, double q2 = 0, double q3 = 0, double q4 = 0, double load = 0.001, string name = "t")
        {
            List<string> toolData = new List<string>();
            string tool = string.Format("PERS tooldata {0}:=[TRUE,[[{1},{2},{3}],[{4},{5},{6},{7}]],[{8},[0,0,0.001],[1,0,0,0],0,0,0]];", name, x, y, z, q1, q2, q3, q4, load);
            toolData.Add(tool);
            return toolData;
        }

        /// <summary>
        /// Define tooldata from point, quaternion, and load values.
        /// </summary>
        /// <param name="pt">Point</param>
        /// <param name="q1">Quaternion</param>
        /// <param name="q2">Quaternion</param>
        /// <param name="q3">Quaternion</param>
        /// <param name="q4">Quaternion</param>
        /// <param name="load">Load in kg</param>
        /// <param name="name">Name of tooldata variable</param>
        /// <returns></returns>
        public static List<string> ToolAtPoint([DefaultArgumentAttribute("Point.ByCoordinates(0,0,0.001)")] Point pt, double q1 = 1, double q2 = 0, double q3 = 0, double q4 = 0, double load = 0.001, string name = "t")
        {
            List<string> toolData = new List<string>();
            string tool = string.Format("PERS tooldata {0}:=[TRUE,[[{1},{2},{3}],[{4},{5},{6},{7}]],[{8},[0,0,0.001],[1,0,0,0],0,0,0]];", name, pt.X, pt.Y, pt.Z, q1, q2, q3, q4, load);
            toolData.Add(tool);
            return toolData;
        }

        /// <summary>
        /// Define tooldata from plane and load value.
        /// </summary>
        /// <param name="pl">Plane</param>
        /// <param name="load">Load in kg</param>
        /// <param name="name">Name of tooldata variable</param>
        /// <returns></returns>
        public static List<string> ToolAtPlane([DefaultArgumentAttribute("Plane.ByOriginNormal(Point.ByCoordinates(0,0,0.001),Vector.ByCoordinates(0,0,1))")] Plane pl, double load = 0.001, string name = "t")
        {
            List<string> toolData = new List<string>();
            List<double> quats = RobotUtils.PlaneToQuaternian(pl);
            string tool = string.Format("PERS tooldata {0}:=[TRUE,[[{1},{2},{3}],[{4},{5},{6},{7}]],[{8},[0,0,0.001],[1,0,0,0],0,0,0]];", name, pl.Origin.X, pl.Origin.Y, pl.Origin.Z, quats[0], quats[1], quats[2], quats[3], load);
            toolData.Add(tool);
            return toolData;
        }

        /// <summary>
        /// Define wobjdata from coordinate and quaternion values.
        /// </summary>
        /// <param name="x">Coordinate</param>
        /// <param name="y">Coordinate</param>
        /// <param name="z">Coordinate</param>
        /// <param name="q1">Quaternion</param>
        /// <param name="q2">Quaternion</param>
        /// <param name="q3">Quaternion</param>
        /// <param name="q4">Quaternion</param>
        /// <param name="name">Name of wobjdata variable</param>
        /// <returns></returns>
        public static List<string> WobjAtVals(double x = 0, double y = 0, double z = 0, double q1 = 1, double q2 = 0, double q3 = 0, double q4 = 0, string name = "w")
        {
            List<string> wobjData = new List<string>();
            string wobj = string.Format("TASK PERS wobjdata {0}:=[FALSE,TRUE," + @"""""" + ",[[{1},{2},{3}],[{4},{5},{6},{7}]],[[0,0,0],[1,0,0,0]]];", name, x, y, z, q1, q2, q3, q4);
            wobjData.Add(wobj);
            return wobjData;
        }

        /// <summary>
        /// Define wobjdata from point and quaternion values.
        /// </summary>
        /// <param name="pt">Point</param>
        /// <param name="q1">Quaternion</param>
        /// <param name="q2">Quaternion</param>
        /// <param name="q3">Quaternion</param>
        /// <param name="q4">Quaternion</param>
        /// <param name="name">Name of wobjdata variable</param>
        /// <returns></returns>
        public static List<string> WobjAtPoint([DefaultArgumentAttribute("Point.ByCoordinates(0,0,0)")] Point pt, double q1 = 1, double q2 = 0, double q3 = 0, double q4 = 0, string name = "w")
        {
            List<string> wobjData = new List<string>();
            string wobj = string.Format("TASK PERS wobjdata {0}:=[FALSE,TRUE," + @"""""" + ",[[{1},{2},{3}],[{4},{5},{6},{7}]],[[0,0,0],[1,0,0,0]]];", name, pt.X, pt.Y, pt.Z, q1, q2, q3, q4);
            wobjData.Add(wobj);
            return wobjData;
        }

        /// <summary>
        /// Define wobjdata from plane.
        /// </summary>
        /// <param name="pl">Plane</param>
        /// <param name="name">Name of wobjdata variable</param>
        /// <returns></returns>
        public static List<string> WobjAtPlane([DefaultArgumentAttribute("Plane.ByOriginNormal(Point.ByCoordinates(0,0,0),Vector.ByCoordinates(0,0,1))")] Plane pl, string name = "w")
        {
            List<string> wobjData = new List<string>();
            List<double> quats = RobotUtils.PlaneToQuaternian(pl);
            string wobj = string.Format("TASK PERS wobjdata {0}:=[FALSE,TRUE," + @"""""" + ",[[{1},{2},{3}],[{4},{5},{6},{7}]],[[0,0,0],[1,0,0,0]]];", name, pl.Origin.X, pl.Origin.Y, pl.Origin.Z, quats[0], quats[1], quats[2], quats[3]);
            wobjData.Add(wobj);
            return wobjData;
        }

    }

    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////


    /// <summary>
    /// Create instructions.
    /// </summary>
    public class Instructions
    {


        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Create a linear movement instruction.
        /// </summary>
        /// <param name="targets">Robot target</param>
        /// <param name="speed">Speed data (rounds to default in RobotStudio)</param>
        /// <param name="zone">Zone data (rounds to default in RobotStudio)</param>
        /// <param name="setName">Unique name for this instruction</param>
        /// <param name="toolName">Active tool</param>
        /// <param name="wobjName">Active work-object</param>
        /// <returns></returns>
        [MultiReturn(new[] { "cnstList", "instList" })]
        public static Dictionary<string, List<string>> MoveL(List<RobTarget> targets, [DefaultArgumentAttribute("{100}")] List<object> speed, [DefaultArgumentAttribute("{0}")] List<object> zone, string setName = "set0", string toolName = "tool0", string wobjName = "wobj0")
        {
            // setup
            List<string> cnstList = new List<string>();
            List<string> instList = new List<string>();
            int cnt;

            // target instructions
            cnt = 0;
            foreach (var target in targets)
            {
                if (cnt < targets.Count)
                {
                    if (cnt == speed.Count) { speed.Add(speed[0]); }
                    if (cnt == zone.Count) { zone.Add(zone[0]); }
                }
                if (speed[cnt] is int || speed[cnt] is double)
                {
                    speed[cnt] = RobotUtils.closestSpeed(Convert.ToDouble(speed[cnt]));
                    speed[cnt] = string.Format("v{0}", speed[cnt]);
                }
                if (zone[cnt] is int || zone[cnt] is double)
                {
                    zone[cnt] = RobotUtils.closestZone(Convert.ToDouble(zone[cnt]));
                    zone[cnt] = string.Format("z{0}", zone[cnt]);
                }

                cnstList.Add(string.Format("CONST robtarget {0}{1}:={2};", setName, cnt, target));
                instList.Add(string.Format("MoveL {0}{1},{2},{3},{4}\\WObj:={5};", setName, cnt, speed[cnt], zone[cnt], toolName, wobjName));
                cnt++;
            }

            //end step
            return new Dictionary<string, List<string>>
            {
                {"cnstList", cnstList},
                {"instList", instList},
                };
        }

        /// <summary>
        /// Create a joint movement instruction.
        /// </summary>
        /// <param name="targets">Robot target</param>
        /// <param name="speed">Speed data (rounds to default in RobotStudio)</param>
        /// <param name="zone">Zone data (rounds to default in RobotStudio)</param>
        /// <param name="setName">Unique name of this instruction</param>
        /// <param name="toolName">Active tool</param>
        /// <param name="wobjName">Active work-object</param>
        /// <returns></returns>
        [MultiReturn(new[] { "cnstList", "instList" })]
        public static Dictionary<string, List<string>> MoveJ(List<RobTarget> targets, [DefaultArgumentAttribute("{100}")] List<object> speed, [DefaultArgumentAttribute("{0}")] List<object> zone, string setName = "set0", string toolName = "tool0", string wobjName = "wobj0")
        {
            // setup
            List<string> cnstList = new List<string>();
            List<string> instList = new List<string>();
            int cnt;

            // target instructions
            cnt = 0;
            foreach (var target in targets)
            {
                if (cnt < targets.Count)
                {
                    if (cnt == speed.Count) { speed.Add(speed[0]); }
                    if (cnt == zone.Count) { zone.Add(zone[0]); }
                }
                if (speed[cnt] is int || speed[cnt] is double)
                {
                    speed[cnt] = RobotUtils.closestSpeed(Convert.ToDouble(speed[cnt]));
                    speed[cnt] = string.Format("v{0}", speed[cnt]);
                }
                if (zone[cnt] is int || zone[cnt] is double)
                {
                    zone[cnt] = RobotUtils.closestZone(Convert.ToDouble(zone[cnt]));
                    zone[cnt] = string.Format("z{0}", zone[cnt]);
                }

                cnstList.Add(string.Format("CONST robtarget {0}{1}:={2};", setName, cnt, target));
                instList.Add(string.Format("MoveJ {0}{1},{2},{3},{4}\\WObj:={5};", setName, cnt, speed[cnt], zone[cnt], toolName, wobjName));

                cnt++;
            }

            // end step
            return new Dictionary<string, List<string>>
            {
                {"cnstList", cnstList},
                {"instList", instList},
                };
        }

        /// <summary>
        /// Create an absolute joint movement instruction.
        /// </summary>
        /// <param name="targets">Joint target</param>
        /// <param name="speed">Speed data (rounds to default in RobotStudio)</param>
        /// <param name="zone">Zone data (rounds to default in RobotStudio)</param>
        /// <param name="setName">Unique name for this instruction</param>
        /// <param name="toolName">Active tool</param>
        /// <param name="wobjName">Active work-object</param>
        /// <returns></returns>
        [MultiReturn(new[] { "cnstList", "instList" })]
        public static Dictionary<string, List<string>> MoveAbsJ(List<JointTarget> targets, [DefaultArgumentAttribute("{100}")] List<object> speed, [DefaultArgumentAttribute("{0}")] List<object> zone, string setName = "set0", string toolName = "tool0", string wobjName = "wobj0")
        {
            // setup
            List<string> cnstList = new List<string>();
            List<string> instList = new List<string>();
            int cnt;

            // target instructions
            cnt = 0;
            foreach (var target in targets)
            {
                if (cnt < targets.Count)
                {
                    if (cnt == speed.Count) { speed.Add(speed[0]); }
                    if (cnt == zone.Count) { zone.Add(zone[0]); }
                }
                if (speed[cnt] is int || speed[cnt] is double)
                {
                    speed[cnt] = RobotUtils.closestSpeed(Convert.ToDouble(speed[cnt]));
                    speed[cnt] = string.Format("v{0}", speed[cnt]);
                }
                if (zone[cnt] is int || zone[cnt] is double)
                {
                    zone[cnt] = RobotUtils.closestZone(Convert.ToDouble(zone[cnt]));
                    zone[cnt] = string.Format("z{0}", zone[cnt]);
                }

                if (speed[cnt] is int)
                { speed[cnt] = string.Format("v{0}", speed[cnt]); }
                if (zone[cnt] is int)
                { zone[cnt] = string.Format("z{0}", zone[cnt]); }

                cnstList.Add(string.Format("CONST jointtarget {0}{1}:={2};", setName, cnt, target));
                instList.Add(string.Format("MoveAbsJ {0}{1},{2},{3},{4}\\WObj:={5};", setName, cnt, speed[cnt], zone[cnt], toolName, wobjName));

                cnt++;
            }

            // end step
            return new Dictionary<string, List<string>>
            {
                {"cnstList", cnstList},
                {"instList", instList},
                };
        }

        /// <summary>
        /// Create a circular movement instruction.
        /// </summary>
        /// <param name="cirTarget">Robot target (through point)</param>
        /// <param name="toTarget">Robot target (destination)</param>
        /// <param name="speed">Speed data (rounds to default in RobotStudio)</param>
        /// <param name="zone">Zone data (rounds to default in RobotStudio)</param>
        /// <param name="setName">Unique name for this instruction</param>
        /// <param name="toolName">Active tool</param>
        /// <param name="wobjName">Active work-object</param>
        /// <returns></returns>
        [MultiReturn(new[] { "cnstList", "instList" })]
        public static Dictionary<string, List<string>> MoveC(List<RobTarget> cirTarget, List<RobTarget> toTarget, [DefaultArgumentAttribute("{100}")] List<object> speed, [DefaultArgumentAttribute("{0}")] List<object> zone, string setName = "set0", string toolName = "tool0", string wobjName = "wobj0")
        {
            // setup
            List<string> cnstList = new List<string>();
            List<string> instList = new List<string>();
            int cnt;

            // target instructions
            cnt = 0;
            foreach (var target in toTarget)
            {
                if (cnt < toTarget.Count)
                {
                    if (cnt == speed.Count) { speed.Add(speed[0]); }
                    if (cnt == zone.Count) { zone.Add(zone[0]); }
                }
                if (speed[cnt] is int || speed[cnt] is double)
                {
                    speed[cnt] = RobotUtils.closestSpeed(Convert.ToDouble(speed[cnt]));
                    speed[cnt] = string.Format("v{0}", speed[cnt]);
                }
                if (zone[cnt] is int || zone[cnt] is double)
                {
                    zone[cnt] = RobotUtils.closestZone(Convert.ToDouble(zone[cnt]));
                    zone[cnt] = string.Format("z{0}", zone[cnt]);
                }

                cnstList.Add(string.Format("CONST robtarget cir{0}{1}:={2};" +
                                            "CONST robtarget to{0}{1}:={3};"
                                            , setName, cnt, cirTarget[cnt], target));
                instList.Add(string.Format("MoveC cir{0}{1}, to{0}{1}, {2},{3},{4}\\WObj:={5};", setName, cnt, speed[cnt], zone[cnt], toolName, wobjName));

                cnt++;
            }

            // end step
            return new Dictionary<string, List<string>>
            {
                {"cnstList", cnstList},
                {"instList", instList},
                };
        }


        /// <summary>
        /// Create custom instruction from string.
        /// </summary>
        /// <param name="instructions"></param>
        /// <returns></returns>
        public static List<string> customInstruction([DefaultArgumentAttribute("{\"WaitTime 3\"}")] List<string> instructions)
        {
            List<string> instList = new List<string>();
            foreach (string inst in instructions)
            {
                instList.Add(string.Format("{0};", inst));
            }
            return instList;
        }

        /// <summary>
        /// Insert instructions into list at specified index.
        /// </summary>
        /// <param name="instList">Initial list of instructions</param>
        /// <param name="instructions">List of instructions to insert</param>
        /// <param name="index">List of indices at which to insert</param>
        /// <returns></returns>
        public static List<string> insertInstAtIndex(List<string> instList, List<string> instructions, List<int> index)
        {
            int cnt = 0;
            foreach (var dex in index)
            {
                if (cnt == instructions.Count) { instructions.Add(instructions[0]); }
                instList.Insert(dex + cnt, string.Format("{0};", instructions[cnt]));
                cnt++;
            }
            return instList;
        }

    }

    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////


    /// <summary>
    /// Write program to file.
    /// </summary>
    public class Write
    {


        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////



        /// <summary>
        /// Merge and write data to a destination.
        /// </summary>
        /// <param name="filePath">"C:\...\myPath.prg"</param>
        /// <param name="cnstList">List of constants</param>
        /// <param name="instList">List of instructions</param>
        /// <param name="toolList">List of tooldata</param>
        /// <param name="wobjList">List of work-object data</param>
        /// <returns></returns>
        [MultiReturn(new[] { "filePath", "robotCode" })]
        public static Dictionary<string, string> createRapid0(string filePath, List<string> cnstList, List<string> instList, List<string> toolList, List<string> wobjList)
        {
            // setup
            var cnstBuilder = new StringBuilder();
            var instBuilder = new StringBuilder();
            var toolBuilder = new StringBuilder();
            var wobjBuilder = new StringBuilder();
            foreach (string cnst in cnstList)
            {
                string cnst2 = "\n\t" + cnst;
                if (!cnst2.EndsWith(";")) { cnst2 = cnst2 + ";"; }
                cnstBuilder.Append(cnst2);
            }
            foreach (string inst in instList)
            {
                string inst2 = "\n\t\t" + inst;
                if (!inst2.EndsWith(";")) { inst2 = inst2 + ";"; }
                instBuilder.Append(inst2);
            }
            foreach (string tool in toolList)
            {
                string tool2 = "\n\t" + tool;
                if (!tool2.EndsWith(";")) { tool2 = tool2 + ";"; }
                toolBuilder.Append(tool2);
            }
            foreach (string wobj in wobjList)
            {
                string wobj2 = "\n\t" + wobj;
                if (!wobj2.EndsWith(";")) { wobj2 = wobj2 + ";"; }
                wobjBuilder.Append(wobj2);
            }

            // create rapid
            string r = "";
            using (var tw = new StreamWriter(filePath, false))
            {
                r = string.Format("MODULE MainModule\n" +
                                            "\t! Program data\n" +
                                            "{0}" +
                                            "{1}" +
                                            "\n" +
                                            "\t! Target data" +
                                            "{2}\n" +
                                            "\n" +
                                            "\t! Routine\n" +
                                            "\tPROC main()\n" +
                                            "\t\tConfL\\Off;\n" +
                                            "\t\tSingArea\\Wrist;\n" +
                                            "\t\trStart;\n" +
                                            "\t\tStop;\n" +
                                            "\tENDPROC\n" +
                                            "\n" +
                                            "\tPROC rStart()\n" +
                                            "\t\t! instructions" +
                                            "{3}" +
                                            "\n" +
                                            "\t\tRETURN;\n" +
                                            "\tENDPROC\n" +
                                            "\n" +
                                            "ENDMODULE\n",
                toolBuilder.ToString(), wobjBuilder.ToString(), cnstBuilder.ToString(), instBuilder.ToString());

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
        /// Merge and write data to a destination.
        /// </summary>
        /// <param name="filePath">"C:\...\myPath.prg"</param>
        /// <param name="cnstList">List of constants</param>
        /// <param name="instList">List of instructions</param>
        /// <returns></returns>
        [MultiReturn(new[] { "filePath", "robotCode" })]
        public static Dictionary<string, string> createRapid1(string filePath, List<string> cnstList, List<string> instList)
        {
            // setup
            var cnstBuilder = new StringBuilder();
            var instBuilder = new StringBuilder();
            foreach (string cnst in cnstList)
            {
                string cnst2 = "\n\t" + cnst;
                if (!cnst2.EndsWith(";")) { cnst2 = cnst2 + ";"; }
                cnstBuilder.Append(cnst2);
            }
            foreach (string inst in instList)
            {
                string inst2 = "\n\t\t" + inst;
                if (!inst2.EndsWith(";")) { inst2 = inst2 + ";"; }
                instBuilder.Append(inst2);
            }

            // create rapid
            string r = "";
            using (var tw = new StreamWriter(filePath, false))
            {
                r = string.Format("MODULE MainModule\n" +
                                            "\n\t! Target data" +
                                            "{0}\n" +
                                            "\n" +
                                            "\t! Routine\n" +
                                            "\tPROC main()\n" +
                                            "\t\tConfL\\Off;\n" +
                                            "\t\tSingArea\\Wrist;\n" +
                                            "\t\trStart;\n" +
                                            "\t\tStop;\n" +
                                            "\tENDPROC\n" +
                                            "\n" +
                                            "\tPROC rStart()\n" +
                                            "\n\t\t! instructions" +
                                            "{1}\n" +
                                            "\t\tRETURN;\n" +
                                            "\tENDPROC\n" +
                                            "\n" +
                                            "ENDMODULE\n",
                cnstBuilder.ToString(), instBuilder.ToString());

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


    }

    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////


    /// <summary>
    /// Create  utility.
    /// </summary>
    public class Utilities
    {


        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////

        /*
        /// <summary>
        /// Get individual quaternions from a plane.
        /// </summary>
        /// <param name="plane">The plane</param>
        /// <returns></returns>
        [MultiReturn(new[] { "q1", "q2", "q3", "q4" })]
        public static Dictionary<string, double> QuatAtPlane([DefaultArgumentAttribute("Plane.ByOriginNormal(Point.ByCoordinates(0,0,0),Vector.ByCoordinates(0,0,1))")] Plane plane)
        {
            List<double> quats = new List<double>();

            if (plane != null)
            {
                List<double> quatDoubles = RobotUtils.PlaneToQuaternian(plane);
                quats.Add(quatDoubles[0]);
                quats.Add(quatDoubles[1]);
                quats.Add(quatDoubles[2]);
                quats.Add(quatDoubles[3]);
            }
            return new Dictionary<string, double>
        {
            {"q1", quats[0]},
            {"q2", quats[1]},
            {"q3", quats[2]},
            {"q4", quats[3]}
            };
        }
        */

        /// <summary>
        /// Get list of quaternions from a plane.
        /// </summary>
        /// <param name="plane">The plane</param>
        /// <returns></returns>
        public static List<double> QuatListAtPlane([DefaultArgumentAttribute("Plane.ByOriginNormal(Point.ByCoordinates(0,0,0),Vector.ByCoordinates(0,0,1))")] Plane plane)
        {
            List<double> quats = new List<double>();

            if (plane != null)
            {
                List<double> quatDoubles = RobotUtils.PlaneToQuaternian(plane);
                quats.Add(quatDoubles[0]);
                quats.Add(quatDoubles[1]);
                quats.Add(quatDoubles[2]);
                quats.Add(quatDoubles[3]);
            }
            return quats;
        }

        /// <summary>
        /// Insert an item into a list at specified index.
        /// </summary>
        /// <param name="list">Initial list</param>
        /// <param name="item">Item to insert</param>
        /// <param name="index">Index at which to insert</param>
        /// <returns></returns>
        public static List<object> InsertAtIndex(List<object> list, List<object> item, List<int> index)
        {
            int cnt = 0;
            foreach (var dex in index)
            {
                if (item.Count == cnt) { item.Add(item[0]); }
                if (dex <= list.Count + 1) { list.Insert(dex + cnt, item[cnt]); }
                if (dex > list.Count + 1) { list.Add(item[cnt]); }
                cnt++;
            }
            return list;
        }

        /// <summary>
        /// Insert a group of items into a list at specified index.
        /// </summary>
        /// <param name="list">Initial list</param>
        /// <param name="group">Group of items to insert</param>
        /// <param name="index">Index at which to insert</param>
        /// <returns></returns>
        public static List<object> InsertGroupAtIndex(List<object> list, List<object> group, List<int> index)
        {
            int cnt = 0;
            foreach (var dex in index)
            {
                int newdex = dex + cnt * group.Count;
                if (newdex <= list.Count + 1) { list.InsertRange(newdex, group); }
                if (newdex > list.Count + 1) { list.AddRange(group); }
                cnt++;
            }
            return list;
        }

        /// <summary>
        /// Combine two lists by items and indices.
        /// </summary>
        /// <param name="listA"></param>
        /// <param name="listB"></param>
        /// <param name="indexA"></param>
        /// <param name="indexB"></param>
        /// <returns></returns>
        public static object[] CombineListsByIndices(List<object> listA, List<object> listB, List<int> indexA, List<int> indexB)
        {
            object[] myList = new object[indexA.Count + indexB.Count];
            for (int i = 0; i < indexA.Count; i++)
            {
                myList[indexA[i]] = listA[i];
            }
            for (int j = 0; j < indexB.Count; j++)
            {
                myList[indexB[j]] = listB[j];
            }
            return myList;
        }

        /// <summary>
        /// Until specified length, zero-pad a value at left.
        /// </summary>
        /// <param name="val">Initial value</param>
        /// <param name="numDigits">Total number of digits</param>
        /// <returns></returns>
        public static string ZeroPadLeft(double val, int numDigits)
        {
            string valStr = val.ToString().PadLeft(numDigits, '0');
            return valStr;
        }

        /// <summary>
        /// Until specified length, zero-pad a value at right.
        /// </summary>
        /// <param name="val">Initial value</param>
        /// <param name="numDigits">Total number of digits</param>
        /// <returns></returns>
        public static string ZeroPadRight(double val, int numDigits)
        {
            string valStr = val.ToString().PadRight(numDigits, '0');
            return valStr;
        }

        /// <summary>
        /// Create file at destination from data in Dynamo.
        /// </summary>
        /// <param name="filePath">Write to destination</param>
        /// <param name="robData">Data to write</param>
        /// <returns></returns>
        public static string DataToFile(string filePath, List<string> robData)
        {
            var dataBuilder = new StringBuilder();
            foreach (string data in robData) { dataBuilder.Append(data); }

            using (var lines = new StreamWriter(filePath, false))
            {
                lines.Write(dataBuilder.ToString());
                lines.Flush();
            }
            return filePath;
        }

        /// <summary>
        /// Read data in Dynamo from file at destination.
        /// </summary>
        /// <param name="filePath">Read from destination</param>
        /// <returns></returns>
        public static string FileToData(string filePath)
        {
            string robData;
            using (var data = new StreamReader(filePath, false))
            {
                robData = data.ReadToEnd();
            }
            return robData;
        }

        /// <summary>
        /// Sort points by Z value.
        /// </summary>
        /// <param name="points">Point list</param>
        /// <returns></returns>
        public static List<Point> sortPointsByZ(List<Point> points)
        {
            points = points.OrderBy(a => a.Z).ToList();
            return points;
        }

        /// <summary>
        /// Sort vectors by Z value.
        /// </summary>
        /// <param name="vectors">Vectors list</param>
        /// <returns></returns>
        public static List<Vector> sortVectorsByZ(List<Vector> vectors)
        {
            vectors = vectors.OrderBy(a => a.Z).ToList();
            return vectors;
        }

        /// <summary>
        /// Sort planes by Z value.
        /// </summary>
        /// <param name="planes">Plane list</param>
        /// <returns></returns>
        public static List<Plane> sortPlanesByZ(List<Plane> planes)
        {
            planes = planes.OrderBy(a => a.Origin.Z).ToList();
            return planes;
        }

        /// <summary>
        /// Sort coordinate systems by Z value.
        /// </summary>
        /// <param name="coordSys">Coordinate system list</param>
        /// <returns></returns>
        public static List<CoordinateSystem> sortPlanesByZ(List<CoordinateSystem> coordSys)
        {
            coordSys = coordSys.OrderBy(a => a.Origin.Z).ToList();
            return coordSys;
        }

    }

    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////


    /// <summary>
    /// Communicate with robot controller.
    /// </summary>
    public class RobComm
    {

        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Scans network for controllers and returns SystemName, SystemID, Version, and IPAddress.
        /// </summary>
        /// <param name="run">True to run</param>
        /// <returns></returns>
        [MultiReturn(new[] { "robotController", "virtualController" })]
        public static Dictionary<string, List<string[]>> findControllers(bool run)
        {
            List<string[]> roboCtrl = new List<string[]> { };
            List<string[]> virtCtrl = new List<string[]> { };

            if (run == true)
            {
                NetworkScanner scanner = new NetworkScanner();
                scanner.Scan();

                ControllerInfoCollection controllers = scanner.Controllers;
                foreach (ControllerInfo controller in controllers)
                {
                    if (controller.IsVirtual == false)
                    {
                        string[] eachController1 = new string[5]
                        {
                                controller.SystemName.ToString(),
                                controller.SystemId.ToString(),
                                controller.Availability.ToString(),
                                controller.Version.ToString(),
                                controller.IPAddress.ToString()
                        };
                        roboCtrl.Add(eachController1);
                    }

                    else
                    {
                        string[] eachController2 = new string[5]
                        {
                                controller.SystemName.ToString(),
                                controller.SystemId.ToString(),
                                controller.Availability.ToString(),
                                controller.Version.ToString(),
                                controller.IPAddress.ToString()
                        };
                        virtCtrl.Add(eachController2);
                    }
                }
            }
            return new Dictionary<string, List<string[]>>
            {
                {"robotController", roboCtrl},
                {"virtualController", virtCtrl}
            };
        }

        /// <summary>
        /// Send a .PRG file to controller.
        /// </summary>
        /// <param name="run">True to run</param>
        /// <param name="controllerData">Controller data</param>
        /// <param name="filePath">File to send</param>
        public static void sendProgramToController(bool run, string[] controllerData, string filePath)
        {
            if (run == true)
            {
                Guid systemId = new Guid(controllerData[1]);
                Controller controller = new Controller(systemId);
                controller.Logon(UserInfo.DefaultUser);

                Task newTask = controller.Rapid.GetTask("T_ROB1");
                using (Mastership.Request(controller.Rapid))
                {
                    newTask.LoadProgramFromFile(filePath, RapidLoadMode.Replace);
                }

                controller.Logoff();
            }
        }

        /// <summary>
        /// Read RobTarget and JointTarget from current position.
        /// </summary>
        /// <param name="run">True to run</param>
        /// <param name="controllerData">Controller data</param>
        /// <returns></returns>
        [MultiReturn(new[] { "robotTarget", "jointTarget" })]
        public static Dictionary<string, string> getCurrentPosition(bool run, string[] controllerData)
        {
            string robotTarget = "";
            string jointTarget = "";

            if (run == true)
            {
                Guid systemId = new Guid(controllerData[1]);
                Controller controller = new Controller(systemId);
                controller.Logon(UserInfo.DefaultUser);

                Task newTask = controller.Rapid.GetTask("T_ROB1");
                using (Mastership.Request(controller.Rapid))
                {
                    robotTarget = newTask.GetRobTarget().ToString();
                    jointTarget = newTask.GetJointTarget().ToString();
                }
            }

            return new Dictionary<string, string>
            {
                {"robotTarget", robotTarget },
                {"jointTarget", jointTarget }
            };
        }

        /// <summary>
        /// Read RobTargets and JointTargets for MainModule on controller.
        /// </summary>
        /// <param name="run">True to run</param>
        /// <param name="controllerData">Controller data</param>
        /// <returns></returns>
        [MultiReturn(new[] { "robTargets", "jointTargets" })]
        public static Dictionary<string, List<string>> getTargetData(bool run, string[] controllerData)
        {
            List<string> rTargets = new List<string>();
            List<string> jTargets = new List<string>();

            if (run == true)
            {
                Guid systemId = new Guid(controllerData[1]);
                Controller controller = new Controller(systemId);
                controller.Logon(UserInfo.DefaultUser);

                Task newTask = controller.Rapid.GetTask("T_ROB1");
                using (Mastership.Request(controller.Rapid))
                {
                    RapidSymbolSearchProperties sProp = RapidSymbolSearchProperties.CreateDefault();
                    sProp.Types = SymbolTypes.Data;
                    RapidSymbol[] datas = newTask.GetModule("MainModule").SearchRapidSymbol(sProp);
                    foreach (RapidSymbol rs in datas)
                    {
                        RapidData rd = controller.Rapid.GetTask("T_ROB1").GetModule("MainModule").GetRapidData(rs);
                        if (rd.Value is RobTarget)
                        { rTargets.Add(rd.Value.ToString()); }
                        if (rd.Value is JointTarget)
                        { jTargets.Add(rd.Value.ToString()); }
                    }
                }
            }

            return new Dictionary<string, List<string>>
            {
                {"robTargets", rTargets },
                {"jointTargets", jTargets }
            };
        }

        /// <summary>
        /// Read tooldata and wobjdata for MainModule on controller.
        /// </summary>
        /// <param name="run">True to run</param>
        /// <param name="controllerData">Controller data</param>
        /// <param name="moduleName">Module name: MainModule | BASE | user (case sensitive)</param>
        /// <returns></returns>
        [MultiReturn(new[] { "programData" /*, "currentData"*/ })]
        public static Dictionary<string, List<string[]>> getProgramData(bool run, string[] controllerData, string moduleName)
        {
            List<string[]> progData = new List<string[]> { };
            //List<string[]> currData = new List<string[]> { };

            if (run == true)
            {
                Guid systemId = new Guid(controllerData[1]);
                Controller controller = new Controller(systemId);
                controller.Logon(UserInfo.DefaultUser);

                Task newTask = controller.Rapid.GetTask("T_ROB1");
                using (Mastership.Request(controller.Rapid))
                {
                    RapidSymbolSearchProperties sProp = RapidSymbolSearchProperties.CreateDefault();

                    sProp.Types = SymbolTypes.Data;
                    RapidSymbol[] progDatas = newTask.GetModule(moduleName).SearchRapidSymbol(sProp);
                    foreach (RapidSymbol rs in progDatas)
                    {
                        RapidData rd = controller.Rapid.GetTask("T_ROB1").GetModule(moduleName).GetRapidData(rs);
                        if ((rd.Value is ToolData) | (rd.Value is WobjData))
                        {
                            string[] eachProgData = new string[3]
                            {
                                rd.RapidType,
                                rd.Name,
                                rd.Value.ToString()
                            };
                            progData.Add(eachProgData);
                        }

                    }

                    /*
                    RapidSymbol[] currDatas = newTask.GetModule("MainModule").SearchRapidSymbol(sProp);
                    foreach (RapidSymbol rs in currDatas)
                    {
                        RapidData rd = controller.Rapid.GetTask("T_ROB1").GetModule("MainModule").GetRapidData(rs);
                        if ((rd.Value is ToolData) | (rd.Value is WobjData))
                        {
                            string[] eachCurr = new string[3]
                            {
                                rd.RapidType,
                                rd.Name,
                                rd.Value.ToString()
                            };
                            currData.Add(eachCurr);
                        }
                    }
                    */

                }
            }

            return new Dictionary<string, List<string[]>>
            {
                {"programData", progData } /*,{"currentData", currData}*/
            };
        }




        /// <summary>
        /// Set current program pointer on controller.
        /// </summary>
        /// <param name="run">True to run</param>
        /// <param name="controllerData">Controller data</param>
        /// <param name="value"></param>
        public static void setProgramPointer(bool run, string[] controllerData, int value)
        {
            if (run == true)
            {
                Guid systemId = new Guid(controllerData[1]);
                Controller controller = new Controller(systemId);
                controller.Logon(UserInfo.DefaultUser);

                Task newTask = controller.Rapid.GetTask("T_ROB1");
                using (Mastership.Request(controller.Rapid))
                {
                    newTask.SetProgramPointer("MainModule", value);
                }
            }
        }


        /// <summary>
        /// Return row of motion pointer [0] and program pointer [1].
        /// </summary>
        /// <param name="run">True to run.</param>
        /// <param name="controllerData">Controller data</param>
        /// <returns></returns>
        public static List<int> getExecutionStatus(bool run, string[] controllerData)
        {
            List<int> variables = new List<int>();
            if (run == true)
            {
                Guid systemId = new Guid(controllerData[1]);
                Controller controller = new Controller(systemId);
                controller.Logon(UserInfo.DefaultUser);

                Task newTask = controller.Rapid.GetTask("T_ROB1");
                Module newModule = newTask.GetModule("MainModule");

                ProgramPosition mPos = newTask.MotionPointer;
                ProgramPosition pPos = newTask.ProgramPointer;
                int mRow = mPos.Range.Begin.Row;
                int pRow = pPos.Range.Begin.Row;

                variables.Add(mRow);
                variables.Add(pRow);
            }
            return variables;
        }






    }

    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////

}
















