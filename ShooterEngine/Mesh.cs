using HLMapFileLoader;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using ShooterEngine.Lighting.Effects;

namespace ShooterEngine
{
    class Mesh
    {
        private GraphicsDevice graphicsDevice;
        private List<VertexBuffer> listVertexBuffer;
        private Brush brush;
        private List<Vector3> vertices;

        public Mesh(GraphicsDevice graphicsDevice, Brush brush)
        {
            this.vertices = new List<Vector3>();
            this.listVertexBuffer = new List<VertexBuffer>();
            this.graphicsDevice = graphicsDevice;
            this.brush = brush;

            BrushToVertices(brush);
        }

        public List<Vector3> GetVertices()
        {
            return vertices;
        }

        private void BrushToVertices(Brush brush)
        {
            foreach (Polygon poly in brush.polygons)
            {
                List<CustomVertexFormat> listVertex = CompleteVertex(poly);

                listVertexBuffer.Add(GetVertexBuffer(listVertex));
            }
        }

        private CustomVertexFormat GetVertexFormat(Vector3 positon, Vector2 texCoord, Vector3 normal)
        {
            vertices.Add(positon);

            return new CustomVertexFormat(positon, texCoord, normal);
        }

        private List<CustomVertexFormat> CompleteVertex(Polygon poly)
        {
            List<CustomVertexFormat> listVertex = new List<CustomVertexFormat>();
            Vector3 normal = new Vector3(0, 1, 0);

            if (poly.Vertices.Count == 5)
            {
                listVertex.Add(GetVertexFormat(poly.Vertices[1], poly.TextureScales[1], CalculateNormal(poly.Vertices[1], poly.Vertices[3], poly.Vertices[4])));
                listVertex.Add(GetVertexFormat(poly.Vertices[3], poly.TextureScales[3], CalculateNormal(poly.Vertices[1], poly.Vertices[3], poly.Vertices[4])));
                listVertex.Add(GetVertexFormat(poly.Vertices[4], poly.TextureScales[4], CalculateNormal(poly.Vertices[1], poly.Vertices[3], poly.Vertices[4])));

                listVertex.Add(GetVertexFormat(poly.Vertices[0], poly.TextureScales[0], CalculateNormal(poly.Vertices[0], poly.Vertices[1], poly.Vertices[4])));
                listVertex.Add(GetVertexFormat(poly.Vertices[1], poly.TextureScales[1], CalculateNormal(poly.Vertices[0], poly.Vertices[1], poly.Vertices[4])));
                listVertex.Add(GetVertexFormat(poly.Vertices[4], poly.TextureScales[4], CalculateNormal(poly.Vertices[0], poly.Vertices[1], poly.Vertices[4])));

                listVertex.Add(GetVertexFormat(poly.Vertices[1], poly.TextureScales[1], CalculateNormal(poly.Vertices[1], poly.Vertices[2], poly.Vertices[3])));
                listVertex.Add(GetVertexFormat(poly.Vertices[2], poly.TextureScales[2], CalculateNormal(poly.Vertices[1], poly.Vertices[2], poly.Vertices[3])));
                listVertex.Add(GetVertexFormat(poly.Vertices[3], poly.TextureScales[3], CalculateNormal(poly.Vertices[1], poly.Vertices[2], poly.Vertices[3])));
                System.Diagnostics.Debug.WriteLine("5");
            }
            else
            if (poly.Vertices.Count == 3)
            {
                listVertex.Add(GetVertexFormat(poly.Vertices[0], poly.TextureScales[0], CalculateNormal(poly.Vertices[0], poly.Vertices[1], poly.Vertices[2])));
                listVertex.Add(GetVertexFormat(poly.Vertices[1], poly.TextureScales[1], CalculateNormal(poly.Vertices[0], poly.Vertices[1], poly.Vertices[2])));
                listVertex.Add(GetVertexFormat(poly.Vertices[2], poly.TextureScales[2], CalculateNormal(poly.Vertices[0], poly.Vertices[1], poly.Vertices[2])));
                System.Diagnostics.Debug.WriteLine("3");
            }
            else
            if (poly.Vertices.Count == 4)
            {
                listVertex.Add(GetVertexFormat(poly.Vertices[3], poly.TextureScales[3], CalculateNormal(poly.Vertices[3], poly.Vertices[0], poly.Vertices[1])));
                listVertex.Add(GetVertexFormat(poly.Vertices[0], poly.TextureScales[0], CalculateNormal(poly.Vertices[3], poly.Vertices[0], poly.Vertices[1])));
                listVertex.Add(GetVertexFormat(poly.Vertices[1], poly.TextureScales[1], CalculateNormal(poly.Vertices[3], poly.Vertices[0], poly.Vertices[1])));

                listVertex.Add(GetVertexFormat(poly.Vertices[2], poly.TextureScales[2], CalculateNormal(poly.Vertices[2], poly.Vertices[3], poly.Vertices[1])));
                listVertex.Add(GetVertexFormat(poly.Vertices[3], poly.TextureScales[3], CalculateNormal(poly.Vertices[2], poly.Vertices[3], poly.Vertices[1])));
                listVertex.Add(GetVertexFormat(poly.Vertices[1], poly.TextureScales[1], CalculateNormal(poly.Vertices[2], poly.Vertices[3], poly.Vertices[1])));
            }
            else
            if (poly.Vertices.Count == 6)
            {
                listVertex.Add(GetVertexFormat(poly.Vertices[0], poly.TextureScales[0], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[1], poly.TextureScales[1], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[5], poly.TextureScales[5], normal));

                listVertex.Add(GetVertexFormat(poly.Vertices[1], poly.TextureScales[1], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[4], poly.TextureScales[4], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[5], poly.TextureScales[5], normal));

                listVertex.Add(GetVertexFormat(poly.Vertices[1], poly.TextureScales[1], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[2], poly.TextureScales[2], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[3], poly.TextureScales[3], normal));

                listVertex.Add(GetVertexFormat(poly.Vertices[1], poly.TextureScales[1], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[3], poly.TextureScales[3], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[4], poly.TextureScales[4], normal));

                System.Diagnostics.Debug.WriteLine("6");
            }
            else
            if (poly.Vertices.Count == 7)
            {
                listVertex.Add(GetVertexFormat(poly.Vertices[0], poly.TextureScales[0], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[5], poly.TextureScales[5], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[6], poly.TextureScales[6], normal));

                listVertex.Add(GetVertexFormat(poly.Vertices[0], poly.TextureScales[0], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[1], poly.TextureScales[1], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[5], poly.TextureScales[5], normal));

                listVertex.Add(GetVertexFormat(poly.Vertices[1], poly.TextureScales[1], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[4], poly.TextureScales[4], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[5], poly.TextureScales[5], normal));

                listVertex.Add(GetVertexFormat(poly.Vertices[1], poly.TextureScales[1], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[2], poly.TextureScales[2], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[4], poly.TextureScales[4], normal));

                listVertex.Add(GetVertexFormat(poly.Vertices[2], poly.TextureScales[2], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[3], poly.TextureScales[3], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[4], poly.TextureScales[4], normal));

                System.Diagnostics.Debug.WriteLine("7");
            }
            else
            if (poly.Vertices.Count == 8)
            {
                listVertex.Add(GetVertexFormat(poly.Vertices[2], poly.TextureScales[2], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[3], poly.TextureScales[3], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[7], poly.TextureScales[7], normal));

                listVertex.Add(GetVertexFormat(poly.Vertices[0], poly.TextureScales[0], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[1], poly.TextureScales[1], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[7], poly.TextureScales[7], normal));

                listVertex.Add(GetVertexFormat(poly.Vertices[1], poly.TextureScales[1], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[2], poly.TextureScales[2], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[6], poly.TextureScales[6], normal));

                listVertex.Add(GetVertexFormat(poly.Vertices[3], poly.TextureScales[3], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[4], poly.TextureScales[4], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[7], poly.TextureScales[7], normal));

                listVertex.Add(GetVertexFormat(poly.Vertices[4], poly.TextureScales[4], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[6], poly.TextureScales[6], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[7], poly.TextureScales[7], normal));

                listVertex.Add(GetVertexFormat(poly.Vertices[1], poly.TextureScales[1], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[2], poly.TextureScales[2], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[7], poly.TextureScales[7], normal));

                System.Diagnostics.Debug.WriteLine("8");
            }

            return listVertex;
        }

        private Vector3 CalculateNormal(Vector3 A, Vector3 B, Vector3 C)
        {
            var ab = A - B;
            var cb = C - B;

            ab.Normalize();
            cb.Normalize();

            return Vector3.Cross(ab, cb);
        }

        private VertexBuffer GetVertexBuffer(List<CustomVertexFormat> listVertex)
        {
            VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice, CustomVertexFormat.VertexDeclaration, listVertex.Count, BufferUsage.WriteOnly);

            vertexBuffer.SetData<CustomVertexFormat>(listVertex.ToArray());

            return vertexBuffer;
        }

        public void Draw()
        {
            for (int i = 0; i < brush.polygons.Count; i++)
            {
                graphicsDevice.SetVertexBuffer(listVertexBuffer[i]);
                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, listVertexBuffer[i].VertexCount / 3);
            }
        }

        public void Draw(MeshEffect effect)
        {
            for (int i = 0; i < brush.polygons.Count; i++)
            {
                effect.Texture = brush.polygons[i].Texture;
                effect.Apply();

                graphicsDevice.SetVertexBuffer(listVertexBuffer[i]);
                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, listVertexBuffer[i].VertexCount / 3);
            }
        }
    }
}