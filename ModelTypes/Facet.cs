using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Simple3DViewer.ModelTypes
{
    public struct Facet
    {
        public Vector3[] Vertexes { get; set; }

        public Vector3 Normal { get; set; }

        public Facet(int numberOfVertexes = 3)
        {
            Vertexes = new Vector3[numberOfVertexes];
            Normal = new Vector3();
        }
    }
}
