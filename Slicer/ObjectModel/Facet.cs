using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Slicer.ObjectModel
{
    class Facet
    {
        public Vector3 Normal;
        public Vector3 Vertex1;
        public Vector3 Vertex2;
        public Vector3 Vertex3;

        public Vector3 MinPoint => Vector3.ComponentMin(Vertex1, Vector3.ComponentMin(Vertex2, Vertex3));
        public Vector3 MaxPoint => Vector3.ComponentMax(Vertex1, Vector3.ComponentMax(Vertex2, Vertex3));
    }
}
