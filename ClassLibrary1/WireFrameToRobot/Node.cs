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
    /// <summary>
    /// a strategy for orientation of the node
    /// </summary>
    public enum OrientationStrategy
    {
        AllNodesOrientedToWorldXYZ, AllNodesSameAsBaseGeo, AverageStrutsVector, OrientationProvided
    }

    public class Node: ILabelAble,IDisposable
    {
        /// <summary>
        /// the center of the node
        /// </summary>
        public Point Center { get; private set; }
        /// <summary>
        /// the fully differenced geometry of the node oriented in space 
        /// with the struts subtracted out
        /// </summary>
        public Solid NodeGeometry { get
            {
                var strutsgeo = this.Struts.Select(x => x.StrutGeometry).ToList();

                var node = this.OrientedNodeGeometry;
                var accum = node.DifferenceAll(strutsgeo);

                return accum;

            }
        }
        /// <summary>
        /// a list of strut objects that belong to this node
        /// these objects wont be unique if this method is run on
        /// two nodes that share a strut, that strut will be retrieved twice.
        /// </summary>
        public List<Strut> Struts { get; private set; }
        /// <summary>
        /// the ID of the node is a simple number with the prefix N
        /// </summary>
        public string ID { get; private set; }

        private Solid originalGeometry;
        /// <summary>
        /// the node geometry oriented in space, but before any boolean operations
        /// </summary>
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
        /// <summary>
        /// a simple representation of the holder - can be used as a reference point
        /// </summary>
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


       
        /// <summary>
        /// this method groups nodes by type defined by their cut planes
        /// </summary>
        /// <returns></returns>
        public static List<List<Node>> FindNodeTypes(List<Node> nodesToGroup)
        {
            //create buckets of nodes based on strut number
            var groups = nodesToGroup.GroupBy(x => x.Struts.Count);
            //our output structure...almost, (TODO use dict after we implement has hash)
            var nodeTypes = new List<Tuple<Node,List<Node>>>();

            foreach(var group in groups)
            {
                foreach( var node in group)
                {
                    //if this type exists in the nodeTypes list, then we can just add our nodes
                    var matchingTypes = nodeTypes.Where(x => SameNode(node, x.Item1));
                    if (matchingTypes.Count() > 0)
                    {
                        //there should be only match
                        matchingTypes.First().Item2.Add(node);
                    }
                    else
                    {
                        //we have not seen this type so add it
                        nodeTypes.Add(Tuple.Create(node, new List<Node>() { node}));
                        
                    }

                }
            }

            return nodeTypes.Select(x => x.Item2).ToList();
        }

        /// <summary>
        /// this method finds types of nodes by hashing their cut planes axes to 4 digits of tolerance
        /// </summary>
        /// <param name="nodesToGroup"></param>
        /// <returns></returns>
        public static List<List<Node>> FindNodeTypesUsingHash(List<Node> nodesToGroup, int digits = 4)
        {
            //create buckets of nodes based on strut number
            var groups = nodesToGroup.GroupBy(x => x.Struts.Count);
            //our output structure
            var nodeTypes = new Dictionary<int, List<Node>>();

            foreach (var group in groups)
            {
                foreach (var node in group)
                {
                    var key = node.NodeTypeHash(digits);
                   if(nodeTypes.ContainsKey(key))
                    {
                        nodeTypes[key].Add(node);
                    }
                    else
                    {
                        //we have not seen this type so add it
                        nodeTypes.Add(node.NodeTypeHash(digits), new List<Node>() { node });

                    }

                }
            }

            return nodeTypes.Select(x=>x.Value).ToList();
        }

        /// <summary>
        /// compares nodes for similarity by their planes
        /// </summary>
        /// <param name="nodea"></param>
        /// <param name="nodeb"></param>
        /// <returns></returns>
        private static bool SameNode(Node nodea,Node nodeb)
        {
            if(nodea.Struts.Count != nodeb.Struts.Count)
            {
                return false;
            }
            var nodebPlanes = nodeb.Struts.Select(x => x.TransformedCutPlane);
            return nodea.Struts.Select(x => x.TransformedCutPlane).All(x => nodebPlanes.Any(y => x.IsAlmostEqualTo(y)));
        }
        /// <summary>
        /// hash a node using the xor of their planes hash
        /// </summary>
        /// <returns></returns>
        private int NodeTypeHash(int digits)
        {
            unchecked
            {
                int hash = 13;
                foreach (var strut in Struts)
                {
                    var plane = strut.TransformedCutPlane;
                    hash = hash ^ PlaneTypeHash(plane, digits);
                }
                return hash;
            }
        }

        /// <summary>
        /// hash a plane by its axes rounded to x digits
        /// </summary>
        /// <param name="pln"></param>
        /// <returns></returns>
        private static int PlaneTypeHash(Plane pln,int digits)
        {
            unchecked
            {
                var hash = 13;
                hash = (hash * 7) + VectorRoundedString(pln.XAxis, digits).GetHashCode();
                hash = (hash * 7) + VectorRoundedString(pln.YAxis, digits).GetHashCode();
                hash = (hash * 7) + VectorRoundedString(pln.Normal, digits).GetHashCode();

                return hash;
            }
        }

        private static string VectorRoundedString(Vector vec,int digits)
        {
            return "X" + Math.Round(vec.X, digits).ToString() + "Y" + Math.Round(vec.X, digits).ToString() + "Z" + Math.Round(vec.X, digits).ToString();
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

          
            var intersectingLines = new List<Line>(); 

            foreach(var line in allLines)
            {
               var ep = line.EndPoint;
               var  sp = line.StartPoint;
              if (sp.IsAlmostEqualTo(center) || ep.IsAlmostEqualTo(center))
                {
                    intersectingLines.Add(line);
                }
                ep.Dispose();
                sp.Dispose();
            }
            
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
                    if (revd.IsAlmostEqualTo(Vector.ByCoordinates(0, 0, 0)))
                        {
                        revd = Vector.ByCoordinates(0,0,1);
                    }
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
            var seenStruts = new Dictionary<int, Strut>();
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
                    if (!seenStruts.ContainsKey(strut.SpatialHash()))
                    {
                        seenStruts.Add(strut.SpatialHash(), strut);
                    }
                    else
                    {
                        //if we have seen it, then construct an edge that represents the two struts we've see
                        var otherStrut = seenStruts[strut.SpatialHash()];
                        var edge = new GraphEdge<Node, Strut>(new List<Strut>() { strut, otherStrut }, strut.OwnerNode, otherStrut.OwnerNode);
                        output.Add(edge);
                    }
                }
            }

            //there might be some orphans in the seenStruts that we never turned into graphEdges, this would happen if we 
            //have some strut that only has one parent node (it leads to no other node)


          //  if (seenStruts.Count > output.Count)
         //   {
         //       throw new NotImplementedException("there are some orphan struts implement this case");
         //   }

            return output;
        }

        public void Dispose()
        {
            this.OrientedNodeGeometry.Dispose();
            this.holderFacePreTransform.Dispose();
            this.Struts.ForEach(x => x.Dispose());

        }


        //debug
        private static void DebugFailure(List<Point> nodeCenters, List<Line> struts, Solid baseNode)
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


        private Solid DebugDifferenceFailure(Solid geo,List<Solid> sub)
        {
           
            var output = geo.DifferenceAll(sub);
        
            return output;
        }
    }

}
