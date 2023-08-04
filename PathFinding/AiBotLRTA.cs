using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinder
{
    class AiBotLRTA : AiBotBase
    {
        double[,] graph;
        int gridsize;
        Coord2 source;
        Coord2 target;
        public IDictionary<int, LRTANode> nodeList;
        int currentVertex;
        int targetVertex;

        public AiBotLRTA(double[,] graph, Coord2 source, Coord2 target, int gridsize) : base(source.X, source.Y)
        {
            this.graph = graph;
            this.gridsize = gridsize;
            this.source = source;
            this.target = target;
            nodeList = new Dictionary<int, LRTANode>();
            initialization();
        }

        protected override void ChooseNextGridLocation(Level level, Player plr)
        {
            if (currentVertex != targetVertex)
            {
                int nextVertex = Lookahead(currentVertex);
                Coord2 nextPosition = Vertex2GridPosition(nextVertex);

                SetNextGridPosition(nextPosition, level);
                currentVertex = nextVertex;
            }
            else
            {
                SetNextGridPosition(source, level);
                currentVertex = GridPosition2Vertex(source);
            }
        }

        /*******************************************  
        * Convert input vertex to grid position (x,y) 
        *******************************************/
        private Coord2 Vertex2GridPosition(int vertexIndex)
        {
            Coord2 c;
            c.X = (vertexIndex % gridsize);
            c.Y = (vertexIndex / gridsize);
            return c;
        }

        /*****************************************  
        * Convert grid position (x,y) to graph vertex 
        ******************************************/
        public int GridPosition2Vertex(Coord2 c)
        {
            int v = c.Y * gridsize + c.X;
            return v;
        }

        /******************************************************
        * Populate & Initialize every vertex statecost & parent  
        *******************************************************/
        public void initialization()
        {
            LRTANode newNode;
            for (int i = 0; i < graph.GetLength(0); i++)
            {
                newNode = new LRTANode();

                newNode.parent = -1;
                newNode.stateCost = 0;
                newNode.gridPosition = Vertex2GridPosition(i);
                newNode.VertexIndex = i;

                nodeList.Add(i, newNode);
            }
            currentVertex = GridPosition2Vertex(source);
            targetVertex = GridPosition2Vertex(target);

        }

        /**************************************** 
        * Update the LRTA cost for current vertex 
        *****************************************/
        private void LRTA_Cost(int currentVertex, int nextVertex)
        {
            LRTANode tmp;
            nodeList.TryGetValue(currentVertex, out tmp);
            tmp.stateCost += graph[currentVertex, nextVertex];

            nodeList[currentVertex] = tmp;
        }

        /***************************************
        * Returns the state cost of given vertex 
        ****************************************/
        public double LRTA_vertex_statecost(int currentVertex)
        {
            LRTANode tmp;
            nodeList.TryGetValue(currentVertex, out tmp);
            return tmp.stateCost;
        }

        /*********************************************************** 
        * Calculate the next move from current vertex given as input 
        ************************************************************/
        private int Lookahead(int vertexIndex)
        {
            double min = int.MaxValue;
            List<int> minVertex = new List<int>();
            LRTANode temp;

            for (int i = 0; i < graph.GetLength(1); i++)
            {
                nodeList.TryGetValue(i, out temp);
                if (graph[vertexIndex, i] >= 1) // 1 or 1.4 if the adjacent
                {
                    if (min >= graph[vertexIndex, i] + temp.stateCost)
                    {
                        if (min > graph[vertexIndex, i] + temp.stateCost)
                        {
                            minVertex.Clear();
                        }
                        minVertex.Add(i);
                        min = graph[vertexIndex, i] + temp.stateCost;
                    }
                }
            }

            int next;
            if (minVertex.Count > 1)
            {
                Random rnd = new Random();
                int randomVertex = rnd.Next(minVertex.Count());

                next = minVertex[randomVertex];
            }
            else
            {
                next = minVertex[0];
            }

            
            // Update state value of vertexIndex
            LRTA_Cost(vertexIndex, next);
            return next;
        }

        public void SaveTrailInfo()
        {
            
            using (StreamWriter sw = new StreamWriter("trailinfo.txt"))
            {
                foreach (KeyValuePair<int, LRTANode> n in nodeList)
                {
                    LRTANode tmp = n.Value;
                    sw.WriteLine(tmp.VertexIndex + "," + tmp.stateCost + "," + tmp.gridPosition.X.ToString() + "-" + tmp.gridPosition.X.ToString() + "," + tmp.parent);
                }
            }
        }

        public void LoadTrailInfo()
        {
            // Make sure node list is empty
            nodeList.Clear();
            string line;
            using (StreamReader sr = new StreamReader("trailinfo.txt"))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    LRTANode tmp = new LRTANode();
                    string[] values = line.Split(',');
                    tmp.VertexIndex = Int32.Parse(values[0]);
                    tmp.stateCost = Double.Parse(values[1]);
                    string[] x_y = values[2].Split('-');

                    tmp.gridPosition.X = Int32.Parse(x_y[0]);
                    tmp.gridPosition.Y = Int32.Parse(x_y[1]);

                    tmp.parent = Int32.Parse(values[3]);

                    // Store it in nodeList
                    nodeList.Add(tmp.VertexIndex, tmp);
                }
            }
        }
    }
}
