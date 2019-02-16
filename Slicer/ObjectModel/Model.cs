using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slicer.ObjectModel
{
    class Model
    {
        public string Name { get; set; } = string.Empty;
        public List<Facet> Facets { get; } = new List<Facet>();

        public void Clear() {
            Name = String.Empty;
            Facets.Clear();
        }
    }
}
