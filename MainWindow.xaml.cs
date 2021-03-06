using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using QuantumConcepts.Formats.StereoLithography;
using SharpGL;
using SharpGL.Shaders;
using SharpGL.WPF;
using Simple3DViewer.ModelsReaders;
using Simple3DViewer.ModelTypes;
using Simple3DViewer.RenderEngines;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using OpenGLControl = SharpGL.WPF.OpenGLControl;

namespace Simple3DViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IModelReader modelReader = new StlModelReader();
        private IRenderEngine renderEngine = new OpenGLRenderEngine();
        private bool modelIsLoaded = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CommandBinding_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog(){ Filter = "STL files |*.stl" };
            
            if (true == openFileDialog.ShowDialog())
            {
                var filePathToModel = openFileDialog.FileName;

                try
                {
                    var model = modelReader.Read(filePathToModel);
                    renderEngine.Model = model;
                    renderEngine.Init(glControl);
                    modelIsLoaded = true;
                }
                catch (FormatException)
                {
                    MessageBox.Show(
                        "Невозможно прочитать данный файл",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
        }

        private void GlControl_OnOpenGLInitialized(object? sender, EventArgs e)
        {
            renderEngine.Width = (float) Width;
            renderEngine.Height = (float) Height;
            renderEngine.Init(glControl);
        }

        private void GlControl_OnOpenGLDraw(object sender, OpenGLRoutedEventArgs args)
        {
            renderEngine.Draw();
        }

        private void MainWindow_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
            {
                ((OpenGLRenderEngine) renderEngine).CameraYPos *= 1.25f;
            }

            if (e.Delta > 0)
            {
                ((OpenGLRenderEngine)renderEngine).CameraYPos /= 1.25f;
            }
        }

        private void MainWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (modelIsLoaded)
            {
                p = e.GetPosition(this);
            }
        }

        private Point p;

        private void MainWindow_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && modelIsLoaded)
            {
                var currentPos = e.GetPosition(this);

                ((OpenGLRenderEngine)renderEngine).RotationToY += (float)(currentPos.X - p.X);
                ((OpenGLRenderEngine)renderEngine).RotationToX += (float)(currentPos.Y - p.Y);

                p = currentPos;
            }

        }

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            renderEngine.Width = (float)Width;
            renderEngine.Height = (float)Height;
            ((OpenGLRenderEngine)renderEngine).SetViewPort((int)Width, (int)Height);
        }
    }
}