using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinder
{
    class Node
    {
        public int parent;
        public Coord2 gridPosition;
        public int VertexIndex;
        public double g_n;
        public double h_n;
        public double f_n;
        public int level;
    }
}
