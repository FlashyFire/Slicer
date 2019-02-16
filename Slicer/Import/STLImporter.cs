using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slicer.ObjectModel;
using System.IO;

namespace Slicer.Import
{
    class STLImporter
    {
        private readonly Model model;

        public STLImporter(Model model) {
            if (model == null)
                throw new ArgumentNullException("model should be not null!");
            this.model = model;
        }

        public void Import(String fileName)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                Import(fileStream);
            }
        } 

        public void Import(Stream stream)
        {

        }
    }
}
