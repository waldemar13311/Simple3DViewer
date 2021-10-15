using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Simple3DViewer.ModelTypes;

namespace Simple3DViewer.ModelsReaders
{
    internal interface IModelReader
    {
        IModel Read(string pathToFile);
    }
}
