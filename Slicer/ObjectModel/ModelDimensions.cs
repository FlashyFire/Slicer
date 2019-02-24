using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Slicer.ObjectModel
{
    class ModelDimensions
    {
        public ModelDimensions(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }
        public Vector3 Min { get; private set; }
        public Vector3 Max { get; private set; }
    }
}
