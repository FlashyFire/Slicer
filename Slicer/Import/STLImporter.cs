using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slicer.ObjectModel;
using System.IO;
using System.Diagnostics;
using System.Globalization;

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
            model.Clear();
            if (IsBinary(stream))
                ReadBinary(stream);
            else
                ReadASCII(stream);
        }

        private bool IsBinary(Stream stream)
        {
            if (stream.Length < 84)
                return false;
            using (BinaryReader reader = new BinaryReader(stream, Encoding.Default, true))
            {
                reader.ReadBytes(80);
                UInt32 count = reader.ReadUInt32();
                stream.Position = 0;
                return stream.Length == count * 50 + 84;
            }
        }

        private void ReadBinary(Stream stream)
        {
            using(BinaryReader reader = new BinaryReader(stream, Encoding.Default, true))
            {
                reader.ReadBytes(80);
                UInt32 count = reader.ReadUInt32();
                for(UInt32 i = 0; i < count; i++)
                {
                    Facet facet = new Facet();
                    facet.Normal.X = reader.ReadSingle();
                    facet.Normal.Y = reader.ReadSingle();
                    facet.Normal.Z = reader.ReadSingle();

                    facet.Vertex1.X = reader.ReadSingle();
                    facet.Vertex1.Y = reader.ReadSingle();
                    facet.Vertex1.Z = reader.ReadSingle();

                    facet.Vertex2.X = reader.ReadSingle();
                    facet.Vertex2.Y = reader.ReadSingle();
                    facet.Vertex2.Z = reader.ReadSingle();

                    facet.Vertex3.X = reader.ReadSingle();
                    facet.Vertex3.Y = reader.ReadSingle();
                    facet.Vertex3.Z = reader.ReadSingle();

                    reader.ReadUInt16();
                    model.Facets.Add(facet);
                }
            }
        }

        private void ReadASCII(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream, Encoding.ASCII, false, 4096, true))
            {
                String line = reader.ReadLine()?.Trim();
                ParseASCIIHeader(line);
                Facet facet = null;
                int index = 0;
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine()?.Trim();
                    if (String.IsNullOrEmpty(line))
                    {
                        continue;
                    }
                    if (line.StartsWith("endsolid"))
                    {
                        return;
                    }
                    if (line.StartsWith("facet"))
                    {
                        facet = new Facet();
                        index = 0;
                        ParseFacet(line, facet);
                    }
                    else if (line.StartsWith("endfacet"))
                    {
                        if (facet == null || index < 2)
                        {
                            throw new InvalidDataException("Некорректное определение грани");
                        }
                        model.Facets.Add(facet);
                        facet = null;
                    }
                    else if (line.StartsWith("vertex"))
                    {
                        ParseVertex(line, facet, index);
                        index++;
                    }
                }
                throw new InvalidDataException("Неполный файл");
            }
        }

        private void ParseASCIIHeader(String line)
        {
            if (String.IsNullOrEmpty(line) || !line.StartsWith("solid"))
            {
                throw new InvalidDataException("Некорректный заголовок ASCII файла!");
            }
            if (line.Length > 6)
                model.Name = line.Substring(6);
        }

        private void ParseFacet(String line, Facet facet)
        {
            String[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 5)
            {
                throw new InvalidDataException("Некорректный формат записи facet");
            }
            facet.Normal.X = Convert.ToSingle(parts[2], CultureInfo.InvariantCulture);
            facet.Normal.Y = Convert.ToSingle(parts[3], CultureInfo.InvariantCulture);
            facet.Normal.Z = Convert.ToSingle(parts[4], CultureInfo.InvariantCulture);
        }

        private void ParseVertex(String line, Facet facet, int index)
        {
            String[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 4)
            {
                throw new InvalidDataException("Некорректный формат записи vertex");
            }
            if (index == 0)
            {
                facet.Vertex1.X = Convert.ToSingle(parts[1], CultureInfo.InvariantCulture);
                facet.Vertex1.Y = Convert.ToSingle(parts[2], CultureInfo.InvariantCulture);
                facet.Vertex1.Z = Convert.ToSingle(parts[3], CultureInfo.InvariantCulture);
            }
            else if (index == 1)
            {
                facet.Vertex2.X = Convert.ToSingle(parts[1], CultureInfo.InvariantCulture);
                facet.Vertex2.Y = Convert.ToSingle(parts[2], CultureInfo.InvariantCulture);
                facet.Vertex2.Z = Convert.ToSingle(parts[3], CultureInfo.InvariantCulture);
            }
            else
            {
                facet.Vertex3.X = Convert.ToSingle(parts[1], CultureInfo.InvariantCulture);
                facet.Vertex3.Y = Convert.ToSingle(parts[2], CultureInfo.InvariantCulture);
                facet.Vertex3.Z = Convert.ToSingle(parts[3], CultureInfo.InvariantCulture);
            }
        }
    }
}
