﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinder

{
    class LRTANode
    {
        public int parent;
        public Coord2 gridPosition;
        public int VertexIndex;
        public double stateCost;
    }
}
