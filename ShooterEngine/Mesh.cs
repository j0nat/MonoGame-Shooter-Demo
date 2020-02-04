using HLMapFileLoader;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

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
                listVertex.Add(GetVertexFormat(poly.Vertices[1], poly.TextureScales[1], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[3], poly.TextureScales[3], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[4], poly.TextureScales[4], normal));

                listVertex.Add(GetVertexFormat(poly.Vertices[0], poly.TextureScales[0], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[1], poly.TextureScales[1], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[4], poly.TextureScales[4], normal));

                listVertex.Add(GetVertexFormat(poly.Vertices[1], poly.TextureScales[1], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[2], poly.TextureScales[2], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[3], poly.TextureScales[3], normal));
            }
            else
            if (poly.Vertices.Count == 3)
            {
                listVertex.Add(GetVertexFormat(poly.Vertices[0], poly.TextureScales[0], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[1], poly.TextureScales[1], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[2], poly.TextureScales[2], normal));
            }
            else
            if (poly.Vertices.Count == 4)
            {
                listVertex.Add(GetVertexFormat(poly.Vertices[3], poly.TextureScales[3], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[0], poly.TextureScales[0], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[1], poly.TextureScales[1], normal));

                listVertex.Add(GetVertexFormat(poly.Vertices[2], poly.TextureScales[2], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[3], poly.TextureScales[3], normal));
                listVertex.Add(GetVertexFormat(poly.Vertices[1], poly.TextureScales[1], normal));
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
            }

            return listVertex;
        }

        private VertexBuffer GetVertexBuffer(List<CustomVertexFormat> listVertex)
        {
            VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice, CustomVertexFormat.VertexDeclaration, listVertex.Count, BufferUsage.WriteOnly);

            vertexBuffer.SetData<CustomVertexFormat>(listVertex.ToArray());

            return vertexBuffer;
        }

        public void Draw(BasicEffect effect, Camera camera)
        {
            for (int i = 0; i < brush.polygons.Count; i++)
            {
                Matrix transform = Matrix.CreateTranslation(new Vector3(0, 0, 0));

                effect.World = transform;
                effect.View = camera.View;
                effect.Projection = camera.Projection;
                effect.TextureEnabled = true;
                effect.Texture = brush.polygons[i].Texture;

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    graphicsDevice.SetVertexBuffer(listVertexBuffer[i]);
                    graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, listVertexBuffer[i].VertexCount / 3);
                }
            }
        }
    }
}