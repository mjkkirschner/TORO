using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Interfaces;
using WireFrameToRobot.Extensions;
using Color = DSCore.Color;
namespace WireFrameToRobot
{

    public class Material
    {
        public double ModulusElasticity { get; private set; }
        public string Name { get; private set; }
        public Color Color { get; private set; }

        public static Material ByModulusName(string name, double modulus, Color color)
        {
            var mat = new Material(name,modulus,color);
            return mat;
        }

        public Material(string name, double modulus, Color color)
        {
            this.ModulusElasticity = modulus;
            this.Name = name;
            this.Color = color;
        }
    }

    

    public class Strut:ILabelAble,IDisposable,IGraphicItem,ICloneable
    {
        /// <summary>
        /// an ID generated based on the nodes this strut is connected to
        /// </summary>
        public string ID { get; private set; }
        /// <summary>
        /// the line which represents this strut
        /// </summary>
        public Line LineRepresentation { get; private set; }
        /// <summary>
        /// a line representation which represents the trimmed strut taking into account the drill depth
        /// of the owner Node
        /// </summary>
        public Line TrimmedLineRepresentation { get; private set; }
        /// <summary>
        /// the node which this strut object belongs to
        /// </summary>
        public Node OwnerNode { get; private set; }
        /// <summary>
        /// a solid sweep of the profile along the line
        /// </summary>
        public Solid StrutGeometry { get; private set; }
        /// <summary>
        /// a 
        /// </summary>
        public Curve Profile { get; private set; }

        /// <summary>
        /// the material this strut is made of
        /// </summary>
        public Material Material { get; private set; }

        /// <summary>
        /// get labels for the strut - showing its ID as geometry
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        public List<Curve> GetLabels(double scale =30)
        {
            var label = new Label<Strut>(this, scale);
            var output = label.AlignedLabelGeometry;
            label.Dispose();
            return output;
        }

        /// <summary>
        /// this plane represents the cut vector on the strut, it's not transformed to the origin
        /// </summary>
        private Plane CutPlaneOnStrut { get
            {
                var coordinateSystemOnLine = LineRepresentation.CoordinateSystemAtParameter(0);
                //reverse the normal because we want the plane normal to point towards the node
                //TODO(mike + Nick) we need to verify this is correct

                var org = coordinateSystemOnLine.Origin;
                var yaxis = coordinateSystemOnLine.YAxis;
                var zaxis = coordinateSystemOnLine.ZAxis;
                var yrev = yaxis.Reverse();

                var output = Plane.ByOriginNormalXAxis(org, yrev, zaxis);
                org.Dispose();
                yaxis.Dispose();
                zaxis.Dispose();
                yrev.Dispose();
                coordinateSystemOnLine.Dispose();

                return output;
                    }
        }
       
        /// <summary>
        /// get the cut plane after it has been transformed around the origin (using the inverse transform of its owner node)
        /// </summary>
        public Plane CutPlaneAtOrigin
        {
            get
            {
                var cs = OwnerNode.OrientedNodeGeometry.ContextCoordinateSystem;
                var inverse = cs.Inverse();
                var output = CutPlaneOnStrut.Transform(inverse) as Plane;
                cs.Dispose();
                inverse.Dispose();
                return output;
            }
        }
        /// <summary>
        /// This returns the transformedCutPlane after its been rotated around the Z axis so that its X axis
        /// aligns with guide vector - NOTE, this does not seem to work in all cases but apparently solves the
        /// robot reach issues we were having
        /// </summary>
        /// <param name="alignTo"></param>
        /// <returns></returns>
        internal Plane AlignedCutPlaneWithACos([DefaultArgumentAttribute("Vector.ByCoordinates(1,0,0)")]Vector alignTo)
        {
            var p = this.CutPlaneAtOrigin;
            var dot = p.XAxis.Dot(alignTo);
            double angle = Math.Acos(dot) * (-1) * (180 / Math.PI);
            var output = p.Rotate(p.Origin, p.Normal, angle) as Plane;
            p.Dispose();
            return output;
        }

        /// <summary>
        /// This method returns a coordinateSystem from the TransformedAlignedCutPlane, this is useful for visualization
        /// The coordinateSystems will appear at the origin since they are transformed using the inverse
        /// </summary>
        /// <returns></returns>
        public CoordinateSystem AlignedCoordinateSystemAtOrigin([DefaultArgumentAttribute("Vector.ByCoordinates(1,0,0)")]Vector alignTo)
        {
            var plane = this.AlignedCutPlaneAtOrigin(alignTo);
            var output = CoordinateSystem.ByPlane(plane);
            plane.Dispose();
            return output;
        }
        
        /// <summary>
        /// attempts to find an aligned plane such that the X axis of the cut plane matches the guide vector using rotation marching, gets a nearly ~aligned~ plane
        /// </summary>
        /// <param name="alignTo"></param>
        /// <returns></returns>
        public Plane AlignedCutPlaneAtOrigin([DefaultArgumentAttribute("Vector.ByCoordinates(1,0,0)")]Vector alignTo)
        {
            var p = this.CutPlaneAtOrigin;
            p = WireFrameToRobot.Extensions.GeometryExtensions.alignPlaneViaMarching(alignTo, p,.01);
            return p;
        }

       

        public Solid GeometryToLabel
        {
            get
            {
                return StrutGeometry;
            }
        }

        /// <summary>
        /// construct a new strut object, this method computes the strut geometry and caches it on the strut
        /// </summary>
        /// <param name="line"></param>
        /// <param name="profile"> this must be a closed curve</param>
        /// <param name="owner"></param>
        internal Strut(Line line, Curve profile, Node owner)
        {
            LineRepresentation = line.Translate(0,0,0) as Line;
            OwnerNode = owner;
            Profile = profile.Translate(0,0,0) as Curve;
        }
        /// <summary>
        /// this constructor creates a list of new struts from an existing list of struts but modifies the 
        /// materials of the new struts using a list of materials - the indicies of the struts and materials
        /// are matched, so they must be the same length
        /// </summary>
        /// <param name="struts"></param>
        /// <param name="materials"></param>
        /// <returns></returns>
        public static List<Strut> ByStrutsAndMaterials (List<Strut> struts, List<Material> materials)
        {
            if (struts.Count != materials.Count)
            {
                throw new ArgumentOutOfRangeException("the strut list and material list do not have the same length");
            }
            //TODO a decision needs to be made -are we generating new struts or are we simply modifying the material properties
            //for now we CLONE them

            var newStruts = new List<Strut>();

            foreach (var index in Enumerable.Range(0, struts.Count))
            {
                var strut = struts[index];
                var mat = materials[index];
                //clone the strut, including geometry,
                //the only thing not cloned is a reference to the node and the original line
                //that created the original struts
                var clone = strut.Clone() as Strut;
                clone.Material = mat;

                newStruts.Add(clone);
            }
            return newStruts;
        }
        internal void computeStrutGeometry()
        {
            //construct a swept tube along the strut line
            var startPlane = TrimmedLineRepresentation.PlaneAtParameter(0);
            var ccs = this.Profile.ContextCoordinateSystem;
            var finalcs = CoordinateSystem.ByPlane(startPlane);

            var prof = this.Profile.Transform(ccs,finalcs) as Curve;
            var swept = prof.SweepAsSolid(TrimmedLineRepresentation);

            prof.Dispose();
            startPlane.Dispose();
            ccs.Dispose();
            finalcs.Dispose();
            StrutGeometry = swept;
        }

        internal void SetId(string id)
        {
            ID = id;
        }

        internal void computeTrimmedLine(Node node1, Node node2)
        {
            var line = LineRepresentation;
            var trim1 = line.Trim(node1.OrientedNodeGeometry, node1.Center).First() as Curve;
            var trim2 = trim1.Trim(node2.OrientedNodeGeometry, node2.Center).First() as Curve;

            var extend1 = trim2.ExtendStart(node1.DrillDepth);
            var extend2 = extend1.ExtendEnd(node2.DrillDepth);

            var output = Line.ByStartPointEndPoint(extend2.StartPoint, extend2.EndPoint);
            this.TrimmedLineRepresentation = output;

            trim1.Dispose();
            trim2.Dispose();
            extend1.Dispose();
            extend2.Dispose();
            
        }

        /// <summary>
        /// check if the cutplane normal angle is withing 30 degrees of the holder face of the node
        /// </summary>
        /// <returns></returns>
        public bool StrutInHolderExclusionZone()
        {
            var face = OwnerNode.HolderFace;
            var anglebetweenWorldZandCutPlaneZ = face.NormalAtParameter(.5,.5).AngleBetween(CutPlaneOnStrut.Normal);
            face.Dispose();
            if(anglebetweenWorldZandCutPlaneZ > 30)
            {
                return false;
            }
            return true;
        }

        public void Dispose()
        {
           if(LineRepresentation != null)
            {
                LineRepresentation.Dispose();
            }
            StrutGeometry.Dispose();
            if (TrimmedLineRepresentation != null){
                TrimmedLineRepresentation.Dispose();
            }
            if(Profile != null)
            {
                Profile.Dispose();
            }

        }

        [IsVisibleInDynamoLibrary(false)]
        /// <summary>
        /// a hashcode based on the string of the start and end point
        /// </summary>
        /// <returns></returns>
        public int SpatialHash()
        {
            unchecked
            {
                var hash = 0;
                hash = hash ^ LineRepresentation.StartPoint.ToString().GetHashCode();
                hash = hash ^ LineRepresentation.EndPoint.ToString().GetHashCode();

                return hash;
            }
        }
        [IsVisibleInDynamoLibrary(false)]
        public void Tessellate(IRenderPackage package, TessellationParameters parameters)
        {
            package.RequiresPerVertexColoration = true;
            StrutGeometry.Tessellate(package, parameters);
            var color = this.Material.Color;
            var colors = new List<byte>();
            foreach (var vert in Enumerable.Range(0,package.MeshVertexCount))
            {
                colors.AddRange(new byte[] {color.Red,color.Blue,color.Green,color.Alpha });
            }
            package.ApplyMeshVertexColors(colors.ToArray());
        }
        /// <summary>
        /// calculate the wasted strut length by finding the first remaining material we can use
        /// this does not give us the optimal solution, but a good general metric
        /// </summary>
        /// <param name="numberOfStrut"> number of struts of the strut length we have at the start of the operation</param>
        /// <param name="lengthOfStruts"> the length of a standard uncut strut</param>
        /// <param name="nodesToCut"> finds the unique struts of this list of nodes and calculates based on that</param>
        /// <returns></returns>
        public static double CalculateWastedStrutLengthByNodes(int numberOfStrut,double lengthOfStruts, List<Node> nodesToCut)
        {
           var struts=  Node.FindUniqueStruts(nodesToCut);
            return CalculateWastedStrutLengthByStruts(numberOfStrut, lengthOfStruts, struts);
        }
        /// <summary>
        /// calculate the wasted strut length by finding the first remaining material we can use
        /// this does not give us the optimal solution, but a good general metric
        /// </summary>
        /// <param name="numberOfStrut"> number of struts of the strut length we have at the start of the operation</param>
        /// <param name="lengthOfStruts"> the length of a standard uncut strut</param>
        /// <param name="strutsToCut">the struts to make, duplicates are not removed from this list</param>
        /// <returns></returns>
        public static double CalculateWastedStrutLengthByStruts(int numberOfStrut, double lengthOfStruts, List<Strut> strutsToCut)
        {
            //create a list of lengths which we will subtract from, these represent the raw struts we can cut from
            var lengths = new List<double>();
            foreach(var index in Enumerable.Range(0, numberOfStrut))
            {
                lengths.Add(lengthOfStruts);
            }

            //now iterate each strut which we need to construct,
            foreach(var strutToCut in strutsToCut)
            {
                //foreach one, subtract its real length from the first remaining strut in the lengths list in which it will fit 
                //that constraint means, the last length which has len > strut.len
                var strutLen = strutToCut.TrimmedLineRepresentation.Length;
                var lenIndex = lengths.FindIndex(x => x > strutLen);
                //if we cannot find a length - we need to throw an exception that we cant cut this many struts
                if (lenIndex == -1)
                {
                    throw new InvalidOperationException("could not find any remanining struts to cut this strut from");
                }
                lengths[lenIndex] = lengths[lenIndex] - strutLen;
                
            }
            //foreach length which is not equal to its original length  - sum them, and return this sum as waste
            return lengths.Where(x => Math.Abs(x - lengthOfStruts) > 0.000001).Sum();
        }

        public object Clone()
        {
            var NewStrut = new Strut(this.LineRepresentation,this.Profile, this.OwnerNode);
            NewStrut.SetId(this.ID);
            NewStrut.StrutGeometry = this.StrutGeometry.Translate(0, 0, 0) as Solid;
            NewStrut.TrimmedLineRepresentation = this.TrimmedLineRepresentation.Translate(0,0,0) as Line;
            return NewStrut;
        }
    }

   
}
namespace WireFrameToRobot.Extensions
{
    public static class GeometryExtensions
    {
        /// <summary>
        /// prune duplicate lines by using start and endpoints to check for equality
        /// </summary>
        /// <param name="allLines"></param>
        /// <returns></returns>
        public static IEnumerable<Line> PruneDuplicates (List<Line> allLines)
            {
            var output = new List<Line>();

            foreach(var line in allLines)
            {
                if (output.Any(x => line.SameLine(x)))
                {
                    continue;
                }
                else
                {
                    output.Add(line);
                }
            }
            return output;

            }

        /// <summary>
        /// checks equality between lines by checking if the start and endpoints match,
        /// even if these are reversed this method return true.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool SameLine (this Line a, Line b)
            {
            var astart = a.StartPoint;
            var aend = a.EndPoint;
            var bstart = b.StartPoint;
            var bend = b.EndPoint;

            var oldgeo = new List<Geometry>(){ astart,aend,bstart,bend};
            

            if ((astart.IsAlmostEqualTo(bstart) && aend.IsAlmostEqualTo(bend))
                || (aend.IsAlmostEqualTo(bstart) && (astart.IsAlmostEqualTo(bend))))
            {
                oldgeo.ForEach(x => x.Dispose());
                return true;
            }
            oldgeo.ForEach(x => x.Dispose());
            return false;
            }

        public static Plane alignPlaneViaMarching(Vector alignTo, Plane p, double tolerance)
        {
            var random = new Random();
            var max = 5.0;
            var min = .00001;
            var angle = random.NextDouble() * (max - min) + min;

            var parentFit = Math.Abs(p.XAxis.Y);
            //while the y component is not zero or the x alignment is facing opposite directions
            while (parentFit > tolerance || alignTo.Dot(p.XAxis) < 0)
            {
                angle = random.NextDouble() * (max - min) + min;
                var child = p.Rotate(p.Origin, p.Normal, angle) as Plane;
                //make sure to dispose the old plane
                p.Dispose();
                var childFit = Math.Abs(child.XAxis.Y);
                p = child;
                //recalculate parentFit
                parentFit = Math.Abs(p.XAxis.Y);
            }

            return p;
        }
    }
}
