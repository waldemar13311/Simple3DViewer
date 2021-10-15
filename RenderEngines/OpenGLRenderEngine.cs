﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmNet;
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

        mat4 model;
        mat4 view;
        mat4 projection;

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
            dataGetterForShader.Model = Model;

            CameraYPos = -(dataGetterForShader.LenghZ / (2.0f * glm.tan(glm.radians(45.0f / 2.0f)))) - dataGetterForShader.LenghZ;
            gl = (renderControl as OpenGLControl)?.OpenGL;

            gl.Enable(SharpGL.OpenGL.GL_MULTISAMPLE);
            gl.Enable(SharpGL.OpenGL.GL_DEPTH_TEST);

            gl.ClearColor(0.4f, 0.6f, 0.9f, 0.0f);

            var vertexShaderSource = @"#version 330 core

in vec3 position;
in vec3 normal;  

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec3 n;

void main(void) {
    //n = normalize(normal);
	gl_Position = projection * view * model * vec4(position, 1.0);
}";
            var fragmentShaderSource = @"#version 330 core
in vec3 n;

out vec4 FragColor;

void main(void) {
    
    float ambient = 0.1;
	vec3 colorOfLight = vec3(1, 1, 1);
	vec3 colorOfObject = vec3(0.8, 0.8, 0.8);
	
	vec3 lightDir = vec3(0, -1, 0); 
	vec3 normal = normalize(n);
	float diff = max(dot(normal, lightDir), 0.0);
	
	vec3 res = colorOfLight * colorOfObject * (diff + ambient);
	
	FragColor = vec4(res, 1);
}";

            shaderProgram = new ShaderProgram();
            shaderProgram.Create(gl, vertexShaderSource, fragmentShaderSource, null);

            shaderProgram.BindAttributeLocation(gl, 0, "position");
            shaderProgram.BindAttributeLocation(gl, 1, "normal");
            shaderProgram.AssertValid(gl);

            var vertices = dataGetterForShader.GetVertexes();
            var normals = dataGetterForShader.GetNormals();

            vertexBufferArray = new VertexBufferArray();
            vertexBufferArray.Create(gl);
            vertexBufferArray.Bind(gl);

            var vertexDataBuffer = new VertexBuffer();
            vertexDataBuffer.Create(gl);
            vertexDataBuffer.Bind(gl);
            vertexDataBuffer.SetData(gl, 0, vertices, false, 3);

            var colourDataBuffer = new VertexBuffer();
            colourDataBuffer.Create(gl);
            colourDataBuffer.Bind(gl);
            colourDataBuffer.SetData(gl, 1, normals, false, 3);

            vertexBufferArray.Unbind(gl);
        }

        public void Draw()
        {
            gl.Clear(SharpGL.OpenGL.GL_COLOR_BUFFER_BIT | SharpGL.OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.ClearColor(0.4f, 0.6f, 0.9f, 0.0f);

            // model block
            model = new mat4(1.0f);
            model = glm.rotate(model, glm.radians(RotationToX), new vec3(1.0f, 0.0f, 0.0f));
            model = glm.rotate(model, glm.radians(RotationToY), new vec3(0.0f, 0.0f, 1.0f));
            var vecOffsetToCenter = new vec3(
                -dataGetterForShader.VecOffsetToCenter.X,
                -dataGetterForShader.VecOffsetToCenter.Y, 
                -dataGetterForShader.VecOffsetToCenter.Z);
            model = glm.translate(model, vecOffsetToCenter);
            
            // view
            var cameraPosition = new vec3(_cameraXPos, CameraYPos, _cameraZPos);
            var cameraDirection = new vec3(_cameraXPos, 0.0f, _cameraZPos);
            var cameraDirectionUp = new vec3(0.0f, 0.0f, 1.0f);
            //
            view = glm.lookAt(cameraPosition, cameraDirection, cameraDirectionUp);

            // projection
            projection = glm.perspective(glm.radians(45.0f), 
                Width / Height, 
                0.1f, 
                Math.Abs(CameraYPos) + dataGetterForShader.MaxLengh);



            shaderProgram.Bind(gl);
            shaderProgram.SetUniformMatrix4(gl, "model", model.to_array());
            shaderProgram.SetUniformMatrix4(gl, "view", view.to_array());
            shaderProgram.SetUniformMatrix4(gl, "projection", projection.to_array());

            vertexBufferArray.Bind(gl);

            gl.DrawArrays(SharpGL.OpenGL.GL_TRIANGLES, 0, Model.Facets.Length * 3);

            vertexBufferArray.Unbind(gl);
            shaderProgram.Unbind(gl);
        }
    }
}
