using Autodesk.DesignScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireFrameToRobot.Topology
{
   
        [SupressImportIntoVM]
        /// <summary>
        /// a graph edge_ stores head and tail and the Strut
        /// </summary>
        public class GraphEdge<NodeType, EdgeType> : IDisposable
                  where NodeType : Node
                  where EdgeType : Strut
        {

            public NodeType Tail { get; set; }
            public NodeType Head { get; set; }
            public List<EdgeType> GeometryEdges { get; set; }

            public GraphEdge(List<EdgeType> edges, NodeType tail, NodeType head)
            {
                Tail = tail;
                Head = head;
                GeometryEdges = edges;
            if (GeometryEdges.Count > 2)
            {
                throw new ArgumentOutOfRangeException("this graphEdge should not represent more than 2 struts");
            }
        }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }


            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
# if DEBUG
                    Console.WriteLine("disposing a geo edge represented by graphedge");
#endif
                    GeometryEdges.Cast<IDisposable>().ToList().ForEach(x=>x.Dispose());


                }

            }



        }


    }
