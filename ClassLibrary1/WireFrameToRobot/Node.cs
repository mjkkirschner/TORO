using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Geometry;
using WireFrameToRobot.Extensions;
using WireFrameToRobot.Topology;
using Autodesk.DesignScript.Interfaces;
using WireFrameToRobot;

namespace WireFrameToRobot
{

    public enum OrientationStrategy
    {
        AllNodesOrientedToWorldXYZ, AllNodesSameAsBaseGeo, AverageStrutsVector, OrientationProvided
    }

    public class Node: ILabelAble,IDisposable
    {

        public Point Center { get; private set; }
        public Solid NodeGeometry { get
            {
                var strutsgeo = this.Struts.Select(x => x.StrutGeometry);

                var accum = this.OrientedNodeGeometry.Translate(0,0,0) as Solid;
                Solid next;
                foreach (var strutgeo in strutsgeo)
                {
                    next = accum.Difference(strutgeo);
                    accum.Dispose();
                    accum = next;
                }

               foreach(var strut in strutsgeo)
                {
                    strut.Dispose();
                }
                return accum;

            }
        }

        public List<Strut> Struts { get; private set; }
        public string ID { get; private set; }

        private Solid originalGeometry;
        public Solid OrientedNodeGeometry { get; private set; }
        /// <summary>
        /// gets the holder face of the node - this is assumed to be the top most surface of the node before orientation occurs
        /// </summary>
        public Surface HolderFace
        {
            get
            {
                var orientation = OrientedNodeGeometry.ContextCoordinateSystem;
                var output = holderFacePreTransform.Transform(orientation) as Surface;
                orientation.Dispose();
                return output;
            }
        }
        public Solid holderRep
        {
            get
            {

                var surfPlane = Plane.ByOriginNormal(HolderFace.PointAtParameter(.5, .5), HolderFace.NormalAtParameter(.5, .5));
                var circle = Circle.ByPlaneRadius(surfPlane, 5);
                var output = circle.ExtrudeAsSolid(5);

                //dispose old stuff
                surfPlane.Dispose();
                circle.Dispose();

                return output;
            }
        }

        public Solid GeometryToLabel
        {
            get
            {
               return NodeGeometry;
            }
        }

        private Surface holderFacePreTransform;


        public static void DebugFailure(List<Point> nodeCenters, List<Line> struts, Solid baseNode)
        {
            //prune all duplicate inputs from wireframe
            var prunedPoints = Point.PruneDuplicates(nodeCenters);
            var prunedLines = GeometryExtensions.PruneDuplicates(struts);

            foreach (var centerPoint in prunedPoints)
            {
                //find adjacent struts for this node
                var intersectingLines = findAdjacentLines(centerPoint, prunedLines);
            }

        }

        /// <summary>
        /// construct list of nodes from a list of points and lines, this method finds the struts that belong 
        /// to each node, orient them correctly, and constructs a geometric representation of the individual nodes
        /// from some base geometry which is oriented in a variety of ways
        /// </summary>
        /// <param name="nodeCenters"></param>
        /// <param name="Struts"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public static List<Node> ByPointsLinesAndGeoOrientationStrategy(List<Point> nodeCenters, List<Line> struts, double strutDiameter, Solid baseNode, OrientationStrategy nodeOrientationStrategy)
        {
            int currentId = 1;
            //prune all duplicate inputs from wireframe
            var prunedPoints = Point.PruneDuplicates(nodeCenters);
            var prunedLines = GeometryExtensions.PruneDuplicates(struts);

            //find the adjacentLines for each node
            var output = new List<Node>();
            foreach (var centerPoint in prunedPoints)
            {
                //find adjacent struts for this node
                var intersectingLines = findAdjacentLines(centerPoint, prunedLines);
                var currentNode = new Node("N"+currentId.ToString().PadLeft(4,'0'), centerPoint, baseNode, intersectingLines, strutDiameter, nodeOrientationStrategy);
               
                //get the most z face and store it as the holder face
                var surfaces = baseNode.Explode().OfType<Surface>().OrderBy(x => x.PointAtParameter(.5, .5).Z).ToList();
                currentNode.holderFacePreTransform = surfaces.Last();
                surfaces.Remove(currentNode.holderFacePreTransform);
                output.Add(currentNode);

                surfaces.ForEach(x => x.Dispose());

                //increment the string ID
                var i = 0;
                currentId = currentId + 1;
                i++;
            }
           
            //from the set of nodes, find the unique struts and use these to update the ids of the struts
            var graphEdges = UniqueStruts(output);
            foreach(var edge in graphEdges)
            {
                var strutA = edge.GeometryEdges.First();
                var strutB = edge.GeometryEdges.Last();

                var nodeA = strutA.OwnerNode;
                var nodeB = strutB.OwnerNode;

                var indA = edge.GeometryEdges.First().OwnerNode.Struts.IndexOf(strutA);
                var indB = edge.GeometryEdges.Last().OwnerNode.Struts.IndexOf(strutB);

                var id = nodeA.ID + 'S'+indA.ToString() + "_" + nodeB.ID + 'S'+indB.ToString();

                //update each struts id to a combination of both nodes and the strut index
                foreach (var strut in edge.GeometryEdges)
                {
                    strut.SetId(id);
                }
            }
           
            return output;
        }

        public static List<Node> ByPointsLinesGeometries(List<Point> nodeCenters, List<Line> struts, List<Solid> Nodes)
        {
            return ByPointsLinesAndGeoOrientationStrategy(nodeCenters, struts, 6, Cuboid.ByLengths(19, 19, 19), OrientationStrategy.AverageStrutsVector);
        }


        public List<Curve> GetLabels(double scale =30)
        {
           var label = new Label<Node>(this, scale);
            var output = label.AlignedLabelGeometry;
            label.Dispose();
            return output ;
        }

        private static List<Line> findAdjacentLines(Point center, IEnumerable<Line> allLines)
        {

            var sphere = Sphere.ByCenterPointRadius(center, 3);
            var intersectingLines = new List<Line>(); 

            foreach(var line in allLines)
            { var results = sphere.Intersect(line);
                if (results.Count() > 0)
                {
                    intersectingLines.Add(line);
                }
                results.ForEach(x => x.Dispose());
            }
            //var intersectingLines = allLines.Where(x => sphere.DoesIntersect(x)).ToList();
            sphere.Dispose();
            return intersectingLines;
        }

        private Node(string id, Point center, Solid nodeBaseGeo, List<Line> lines, double strutDiameter, OrientationStrategy strategy)
        {
            ID = id;
            originalGeometry = nodeBaseGeo;
            Center = center;

            //orient the struts so they all point away from the node
            var newlines = lines.Select(x => pointAway(center, x)).ToList();
            //construct stuts from these new lines
            this.Struts = newlines.Select(x => new Strut(x, strutDiameter, this)).ToList();
            //calculate the orientation of the node based on the orientation strategy
            this.OrientedNodeGeometry = orientNode(strategy, this.Struts);           

        }

        private Solid orientNode(OrientationStrategy strategy, List<Strut> struts)
        {
            switch (strategy)
            {
                case OrientationStrategy.AllNodesOrientedToWorldXYZ:

                    var plane = Plane.ByOriginNormal(Center, Vector.ZAxis());
                    var newCs = CoordinateSystem.ByPlane(plane);

                    var output = originalGeometry.Transform(newCs) as Solid;
                    plane.Dispose();
                    newCs.Dispose();

                    return output;
                    break;

                case OrientationStrategy.AllNodesSameAsBaseGeo:

                    var orgCs = originalGeometry.ContextCoordinateSystem;
                     plane = Plane.ByOriginNormal(Center,orgCs.ZAxis);
                     newCs = CoordinateSystem.ByPlane(plane);

                     output = originalGeometry.Transform(newCs) as Solid;
                    plane.Dispose();
                    newCs.Dispose();
                    orgCs.Dispose();

                    return output;
                    break;

                case OrientationStrategy.AverageStrutsVector:

                    //orient the cube based on the average normal of the incoming struts
                    var spoints = struts.Select(x => x.LineRepresentation.StartPoint).ToList();
                    var epoints = struts.Select(x => x.LineRepresentation.EndPoint).ToList();
                    var vectors = spoints.Zip(epoints, (x, y) => Vector.ByTwoPoints(x, y)).ToList();
                    var averageNorm = averageVector(vectors);
                    var revd = averageNorm.Reverse();
                    //reverse the normal so the top face is correct
                     plane = Plane.ByOriginNormal(Center, revd);
                     newCs = CoordinateSystem.ByPlane(plane);
                   
                     output = originalGeometry.Transform(newCs) as Solid;
                    plane.Dispose();
                    newCs.Dispose();
                    spoints.ForEach(x => x.Dispose());
                    epoints.ForEach(x => x.Dispose());
                    vectors.ForEach(x => x.Dispose());
                    averageNorm.Dispose();
                    revd.Dispose();


                    return output;

                    break;

                case OrientationStrategy.OrientationProvided:

                    break;
                default:
                    return NodeGeometry;
                    break;
            }
            //eh?
            return NodeGeometry;
        }

        private Vector averageVector(List<Vector> vectors)
        {
            var sum = Vector.ByCoordinates(0, 0, 0);
            Vector next;
            foreach (var vector in vectors)
            {
                next = sum.Add(vector);
                sum.Dispose();
                sum = next;
              
            }
            var output = sum.Scale(1.0 / vectors.Count);
            sum.Dispose();
            return output;
        }

        private Line pointAway(Point point, Line line)
        {
            if (line.EndPoint.IsAlmostEqualTo(point))
            {
                var output = line.Reverse() as Line;
                output.Tags.AddTag("dispose", true);
                return output ;
            }
            return line;
        }

        public static List<Strut> FindUniqueStruts (List<Node>nodes)
        {
           var edges = UniqueStruts(nodes);
            return edges.Select(x => x.GeometryEdges.First()).ToList();
        }

        public static double TotalStrutLength(List<Node> nodes)
        {
         var struts = FindUniqueStruts(nodes);
            var lines = struts.Select(x => x.LineRepresentation);
            var sum = 0.0;
            foreach(var line in lines)
            {
                sum = sum + line.Length;
            }

            return sum;
        }
        private static List<GraphEdge<Node, Strut>> UniqueStruts(List<Node> nodes)
        {
            //create a list to store all the edges we have seen
            var seenStruts = new List<Strut>();
            //our output of graphEdges, this edges represent a single real strut edge
            //in the final model
            var output = new List<GraphEdge<Node, Strut>>();

            //iterate all the nodes
            foreach (var node in nodes)
            {
                //iterate each nodes subStruts
                foreach (var strut in node.Struts)
                {
                    //(TODO mike replace with hashset//
                    //if we have never seen this strut, then add it to the list of seen struts
                    if ((seenStruts.All(x => !strut.LineRepresentation.SameLine(x.LineRepresentation))))
                    {
                        seenStruts.Add(strut);
                    }
                    else
                    {
                        //if we have seen it, then construct an edge that represents the two struts we've see
                        var otherStrut = seenStruts.Where(x => x.LineRepresentation.SameLine(strut.LineRepresentation)).First();
                        var edge = new GraphEdge<Node, Strut>(new List<Strut>() { strut, otherStrut }, strut.OwnerNode, otherStrut.OwnerNode);
                        output.Add(edge);
                    }
                }
            }

            //there might be some orphans in the seenStruts that we never turned into graphEdges, this would happen if we 
            //have some strut that only has one parent node (it leads to no other node)


            if (seenStruts.Count > output.Count)
            {
                throw new NotImplementedException("there are some orphan struts implement this case");
            }

            return output;
        }

        public void Dispose()
        {
            this.OrientedNodeGeometry.Dispose();
            this.holderFacePreTransform.Dispose();
            this.Struts.ForEach(x => x.Dispose());

        }
    }

}
