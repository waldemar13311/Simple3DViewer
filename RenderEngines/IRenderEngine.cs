using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Simple3DViewer.ModelTypes;

namespace Simple3DViewer.RenderEngines
{
    internal interface IRenderEngine
    {
        IModel Model { set; get; }
            
        float Width { set; }
        float Height { set; }

        void Init(System.Windows.Controls.UserControl renderControl);
        void Draw();
    }
}
