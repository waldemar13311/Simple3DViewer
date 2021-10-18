using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GlmSharp;
using QuantumConcepts.Formats.StereoLithography;
using SharpGL.WPF;
using Simple3DViewer.ModelTypes;
using SharpGL.Shaders;
using SharpGL.VertexBuffers;

namespace Simple3DViewer.RenderEngines
{
    class OpenGLRenderEngine : IRenderEngine
    {
        private ShaderProgram shaderProgram;
        private VertexBufferArray vertexBufferArray;
        private GetterDataFromModelForShaders dataGetterForShader = new GetterDataFromModelForShaders();

        private SharpGL.OpenGL gl;

        public IModel Model { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        private float _cameraXPos = 0.0f;
        private float _cameraZPos = 0.0f;
        
        public float RotationToX { get; set; } = 0.0f;
        public float RotationToY { get; set; } = 0.0f;

        public float CameraYPos { get; set; } = -1.0f;

        public void Init(System.Windows.Controls.UserControl renderControl)
        {
            if (Model != null)
            {
                dataGetterForShader.Model = Model;
                RotationToX = 0.0f;
                RotationToY = 0.0f;
                CameraYPos = -(dataGetterForShader.LenghZ / (2.0f * glm.Tan(glm.Radians(45.0f / 2.0f)))) - dataGetterForShader.LenghZ;

                var vertexShaderSource = @"#version 330 core
layout(location = 0)in vec3 position;
layout(location = 1)in vec3 normal;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform mat4 normalMatrix;

out vec3 n;

void main(void) {
    //n = normalize(normalMatrix * normal);
    n = vec3(normalMatrix * vec4(normal, 0));

    gl_Position = projection * view * model * vec4(position, 1.0);
}";
                var fragmentShaderSource = @"#version 330 core
in vec3 n;

out vec4 FragColor;

void main(void) {
    float ambient = 0.1;
	vec3 colorOfLight = vec3(1, 1, 1);
	vec3 colorOfObject = vec3(0.9, 0.9, 0.9);
	
	vec3 lightDir = vec3(0, -1, 0); 
	vec3 normal = normalize(n);
	float diff = max(dot(normal, lightDir), 0.0);
	
	vec3 res = colorOfLight * colorOfObject * (diff + ambient);
	
	FragColor = vec4(res, 1);
}";

                shaderProgram = new ShaderProgram();
                shaderProgram.Create(gl, vertexShaderSource, fragmentShaderSource, null);
                shaderProgram.AssertValid(gl);

                vertexBufferArray = new VertexBufferArray();
                vertexBufferArray.Create(gl);
                vertexBufferArray.Bind(gl);

                var vertices = dataGetterForShader.GetVertexes();
                var vertexDataBuffer = new VertexBuffer();
                vertexDataBuffer.Create(gl);
                vertexDataBuffer.Bind(gl);
                vertexDataBuffer.SetData(gl, 0, vertices, false, 3);

                var normals = dataGetterForShader.GetNormals();
                var normalDataBuffer = new VertexBuffer();
                normalDataBuffer.Create(gl);
                normalDataBuffer.Bind(gl);
                normalDataBuffer.SetData(gl, 1, normals, false, 3);

                vertexBufferArray.Unbind(gl);
            }

            gl = (renderControl as OpenGLControl)?.OpenGL;

            SetViewPort((int)Width, (int)Height);
            gl.Enable(SharpGL.OpenGL.GL_MULTISAMPLE);
            gl.ClearColor(0.4f, 0.6f, 0.9f, 1.0f);
        }

        public void Draw()
        {
            
            gl.Enable(SharpGL.OpenGL.GL_DEPTH_TEST);
            gl.Clear(SharpGL.OpenGL.GL_COLOR_BUFFER_BIT | SharpGL.OpenGL.GL_DEPTH_BUFFER_BIT);

            if (Model == null)
            {
                return;
            }


            // model block
            var model = mat4.Identity;
            model *= mat4.Rotate(glm.Radians(RotationToX * (dataGetterForShader.MaxLengh / 100.0f)),
                new vec3(1.0f, 0.0f, 0.0f));
            model *= mat4.Rotate(glm.Radians(RotationToY * (dataGetterForShader.MaxLengh / 100.0f)),
                new vec3(0.0f, 0.0f, 1.0f));

            var vecOffsetToCenter = new vec3(
                dataGetterForShader.VecOffsetToCenter.X,
                dataGetterForShader.VecOffsetToCenter.Y,
                dataGetterForShader.VecOffsetToCenter.Z);
            model *= mat4.Translate(-vecOffsetToCenter);

            // view
            var cameraPosition = new vec3(_cameraXPos, CameraYPos, _cameraZPos);
            var cameraDirection = new vec3(_cameraXPos, 0.0f, _cameraZPos);
            var cameraDirectionUp = new vec3(0.0f, 0.0f, 1.0f);

            var view = mat4.LookAt(cameraPosition, cameraDirection, cameraDirectionUp);

            // projection
            var projection = mat4.Perspective(glm.Radians(45.0f),
                Width / Height,
                0.1f,
                Math.Abs(CameraYPos) + dataGetterForShader.MaxLengh);


            //var normalMatrix = mat4.Identity; // new mat3(new mat4(new mat4(model.Inverse).Transposed));
            var normalMatrix = model.Inverse.Transposed;


            shaderProgram.Bind(gl);

            shaderProgram.SetUniformMatrix4(gl, "model", model.Values1D);
            shaderProgram.SetUniformMatrix4(gl, "view", view.Values1D);
            shaderProgram.SetUniformMatrix4(gl, "projection", projection.Values1D);
            shaderProgram.SetUniformMatrix4(gl, "normalMatrix", normalMatrix.Values1D);

            vertexBufferArray.Bind(gl);

            gl.DrawArrays(SharpGL.OpenGL.GL_TRIANGLES, 0, Model.Facets.Length * 3);

            vertexBufferArray.Unbind(gl);
            shaderProgram.Unbind(gl);

        }

        public void SetViewPort(int width, int height)
        {
            gl.Viewport(0, 0, width, height);
        }
    }
}
