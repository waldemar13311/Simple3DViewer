using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using QuantumConcepts.Formats.StereoLithography;
using Simple3DViewer.ModelTypes;
using Facet = Simple3DViewer.ModelTypes.Facet;

namespace Simple3DViewer.ModelsReaders
{
    class StlModelReader : IModelReader
    {
        public IModel Read(string pathToFile)
        {
            var stlDoc = STLDocument.Open(pathToFile);
            IModel model = new SimpleStlModel(stlDoc.Facets.Count);

            for (var i = 0; i < stlDoc.Facets.Count; i++)
            {
                var facet = stlDoc.Facets[i];

                var normal = new Vector3
                {
                    X = facet.Normal.X,
                    Y = facet.Normal.Y,
                    Z = facet.Normal.Z
                };

                var vertexes = new Vector3[3];

                for (var j = 0; j < facet.Vertices.Count; j++)
                {
                    vertexes[j].X = facet.Vertices[j].X;
                    vertexes[j].Y = facet.Vertices[j].Y;
                    vertexes[j].Z = facet.Vertices[j].Z;
                }

                model.Facets[i].Normal = normal;
                model.Facets[i].Vertexes = vertexes;
            }

            return model;
        }
    }
}
