using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple3DViewer.ModelTypes
{
    internal class SimpleStlModel : IModel
    {
        public SimpleStlModel(int facetsCount)
        {
            Facets = new Facet[facetsCount];
        }

        public Facet[] Facets { get; set; }
    }
}
