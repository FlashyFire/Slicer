using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Slicer.ObjectModel
{
    class Model
    {
        private ModelDimensions dimensions = null;

        public string Name { get; set; } = string.Empty;
        public List<Facet> Facets { get; } = new List<Facet>();
        public ModelDimensions Dimensions {
            get {
                if (dimensions == null)
                {
                    CalcDimensions();
                }
                return dimensions;
            }
        }

        private void CalcDimensions()
        {
            Vector3 min = Vector3.Zero;
            Vector3 max = Vector3.Zero;
            if (Facets.Count > 0)
            {
                min = Facets[0].MinPoint;
                max = Facets[0].MaxPoint;
                for (int i = 1; i < Facets.Count; i++)
                {
                    min = Vector3.ComponentMin(min, Facets[i].MinPoint);
                    max = Vector3.ComponentMax(max, Facets[i].MaxPoint);
                }
            }
            dimensions = new ModelDimensions(min, max);
        }

        public void Clear() {
            Name = String.Empty;
            Facets.Clear();
            dimensions = null;
        }
    }
}
