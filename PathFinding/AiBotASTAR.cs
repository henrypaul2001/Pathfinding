
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using System.Collections;

namespace Pathfinder
{
    class AiBotASTAR: AiBotBase
    {
        double[,] graph;
        Coord2 source, target;
        int gridsize;
        ArrayList nodeList;
        IDictionary<string, Node> pathTracking;
        bool pathCalculated;

        public AiBotASTAR(Coord2 source, Coord2 target, double[,] graph, int gridsize, Level level) : base(source.X, source.Y)
        {
            this.graph = graph;
            this.source = source;
            this.target = target;
            this.gridsize = gridsize;

            nodeList = new ArrayList();
            pathTracking = new Dictionary<string, Node>();

            pathCalculated = false;

            path = new List<Coord2>();

            Build(level);
        }

        protected override void ChooseNextGridLocation(Level level, Player plr)
        {
            if (pathCalculated)
            {
                for (int i = path.Count - 1; i >= 0; i--)
                {
                    SetNextGridPosition(path[i], level);
                }
            }
        }

        // A* Algorithm
        public void Build(Level pLevel)
        {
            // Starting node
            Node currentNode = new Node();

            currentNode.parent = -1;
            currentNode.gridPosition = source;
            currentNode.level = 0;
            currentNode.VertexIndex = GridPosition2Vertex(source);

            currentNode.g_n = 0;
            currentNode.h_n = Calculate_h_n(source, target);

            currentNode.f_n = currentNode.g_n + currentNode.h_n;

            nodeList.Add(currentNode);

            
            //Key = Node Vertex Number + Parent ID + level

            pathTracking.Add(currentNode.VertexIndex + "-" + currentNode.parent + "-" + currentNode.level, currentNode);
            

            while (nodeList.Count > 0)
            {
                currentNode = getMinVertex(pLevel);
                if (currentNode.gridPosition == target)
                {
                    break;
                }

                int length = graph.GetLength(1);
                for (int i = 0; i < graph.GetLength(1); i++)
                {
                    if (graph[currentNode.VertexIndex, i] >= 1) // 1 or 1.4 if the adjacent
                    {
                        Node newNode = new Node();

                        newNode.parent = currentNode.VertexIndex;
                        newNode.level = currentNode.level + 1;

                        newNode.VertexIndex = i;
                        newNode.gridPosition = Vertex2GridPosition(i);

                        newNode.g_n = currentNode.g_n + graph[currentNode.VertexIndex, i];
                        newNode.h_n = Calculate_h_n(newNode.gridPosition, target);

                        //f(n) = s(n) + h(n)
                        newNode.f_n = newNode.g_n + newNode.h_n;

                        // Path tracking
                        if (pathTracking.ContainsKey(newNode.VertexIndex + "-" + newNode.parent + "-" + newNode.level))
                        {
                            continue;
                        }
                        else
                        {
                            nodeList.Add(newNode);
                            pathTracking.Add(newNode.VertexIndex + "-" + newNode.parent + "-" + newNode.level, currentNode);
                        }
                    }
                }
                nodeList.Remove(currentNode);
            }

            while (currentNode.parent != -1)
            {
                path.Add(currentNode.gridPosition);
                Node temp;
                pathTracking.TryGetValue(currentNode.VertexIndex + "-" + currentNode.parent + "-" + currentNode.level, out temp);
                currentNode = temp;
            }

        pathCalculated = true;
        }

        /******************************************* 
        * Returns the vertex with minimum f_n
        *******************************************/
    
        private Node getMinVertex(Level pLevel)
        {
            double min = int.MaxValue;
            Node v = null;
            foreach (Node f in nodeList)
            {
                if (f.f_n < min && pLevel.ValidPosition(f.gridPosition))
                {
                    min = f.f_n;
                    v = f;
                }
            }
            return v;
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
    
        private int GridPosition2Vertex(Coord2 c)
        {
            int v = c.Y * gridsize + c.X;
            return v;
        }

        /******************************************* 
        * Convert h(n) - straight line distance 
        * B/W two points provided as input
        ********************************************/
    
        private double Calculate_h_n(Coord2 s, Coord2 t)
        {
            double h_n = Math.Sqrt(Convert.ToDouble(((t.X - s.X) * (t.X - s.X)) + ((t.Y - s.Y) * (t.Y - s.Y))));
            return h_n;
        }
    }

    }
    



