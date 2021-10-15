using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Simple3DViewer.ModelTypes;

namespace Simple3DViewer
{
    internal class GetterDataFromModelForShaders
    {
        private IModel _model;

        private Vector3 _maxCoor;
        private Vector3 _minCoor;
        private Vector3 _vecOffsetToCenter;

        public IModel Model
        {
            get => _model;

            set
            {
                _model = value;

                _maxCoor.X = _model.Facets[0].Vertexes[0].X;
                _maxCoor.Y = _model.Facets[0].Vertexes[0].Y;
                _maxCoor.Z = _model.Facets[0].Vertexes[0].Z;

                _minCoor.X = _model.Facets[0].Vertexes[0].X;
                _minCoor.Y = _model.Facets[0].Vertexes[0].Y;
                _minCoor.Z = _model.Facets[0].Vertexes[0].Z;

                foreach (var facet in _model.Facets)
                {
                    foreach (var vertex in facet.Vertexes)
                    {
                        if (vertex.X > MaxCoordinate.X)
                        {
                            _maxCoor.X = vertex.X;
                        }

                        if (vertex.Y > MaxCoordinate.Y)
                        {
                            _maxCoor.Y = vertex.Y;
                        }

                        if (vertex.Z > MaxCoordinate.Z)
                        {
                            _maxCoor.Z = vertex.Z;
                        }

                        if (vertex.X < MinCoordinate.X)
                        {
                            _minCoor.X = vertex.X;
                        }

                        if (vertex.Y < MinCoordinate.Y)
                        {
                            _minCoor.Y = vertex.Y;
                        }

                        if (vertex.Z < MinCoordinate.Z)
                        {
                            _minCoor.Z = vertex.Z;
                        }
                    }
                }

                _vecOffsetToCenter.X = _maxCoor.X - ((_maxCoor.X - _minCoor.X) / 2.0f);
                _vecOffsetToCenter.Y = _maxCoor.Y - ((_maxCoor.Y - _minCoor.Y) / 2.0f);
                _vecOffsetToCenter.Z = _maxCoor.Z - ((_maxCoor.Z - _minCoor.Z) / 2.0f);


                LenghY = _maxCoor.Z - _minCoor.Z;
                LenghZ = _maxCoor.Z - _minCoor.Z;

                MaxLengh = new float[3]
                {
                    _maxCoor.X - _minCoor.X,
                    LenghY,
                    LenghZ
                }.Max();
            }
        }

        public Vector3 MaxCoordinate => _maxCoor;

        public Vector3 MinCoordinate => _minCoor;

        public float MaxLengh { get; private set; }

        public float LenghY { get; private set; }

        public float LenghZ { get; private set; }

        public Vector3 VecOffsetToCenter => _vecOffsetToCenter;

        public float[] GetVertexes()
        {
            var vertexes = new List<float>();
            
            foreach (var facet in Model.Facets)
            {
                foreach (var vertex in facet.Vertexes)
                {
                    vertexes.Add(vertex.X);
                    vertexes.Add(vertex.Y);
                    vertexes.Add(vertex.Z);
                }
            }

            return vertexes.ToArray();
        }

        public float[] GetNormals()
        {
            var normals = new List<float>();

            foreach (var facet in Model.Facets)
            {
                for (var i = 0; i < 3; i++)
                {
                    normals.Add(facet.Normal.X);
                    normals.Add(facet.Normal.Y);
                    normals.Add(facet.Normal.Z);
                }
            }

            return normals.ToArray();
        }
    }
}
