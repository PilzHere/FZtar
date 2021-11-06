using System;
using FZtarOGL.Box;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FZtarOGL.Utilities
{
    public class BoundingBoxBuilder
    {
        public static BoundingBoxFiltered BuildBoundingBoxFiltered(Entity.Entity parent, ModelMesh mesh, Matrix meshTransform, short filter, short mask)
        {
            // Create initial variables to hold min and max xyz values for the mesh
            Vector3 meshMax = new Vector3(float.MinValue);
            Vector3 meshMin = new Vector3(float.MaxValue);
 
            foreach (ModelMeshPart part in mesh.MeshParts)
            {
                // The stride is how big, in bytes, one vertex is in the vertex buffer
                // We have to use this as we do not know the make up of the vertex
                int stride = part.VertexBuffer.VertexDeclaration.VertexStride;
 
                VertexPositionNormalTexture[] vertexData = new VertexPositionNormalTexture[part.NumVertices];
                part.VertexBuffer.GetData(part.VertexOffset * stride, vertexData, 0, part.NumVertices, stride);
 
                // Find minimum and maximum xyz values for this mesh part
                Vector3 vertPosition = new Vector3();
 
                for (int i = 0; i < vertexData.Length; i++)
                {
                    vertPosition = vertexData[i].Position;
 
                    // update our values from this vertex
                    meshMin = Vector3.Min(meshMin, vertPosition);
                    meshMax = Vector3.Max(meshMax, vertPosition);
                }
            }
 
            // transform by mesh bone matrix
            meshMin = Vector3.Transform(meshMin, meshTransform);
            meshMax = Vector3.Transform(meshMax, meshTransform);
            
            //Console.WriteLine("MIN: " + meshMin + " MAX: " + meshMax);
 
            // Create the bounding box
            BoundingBox box = new BoundingBox(meshMin, meshMax);

            BoundingBoxFiltered boxFitlered = new BoundingBoxFiltered(parent, box, filter, mask);
            return boxFitlered;
        }
        
        public static BoundingBoxFiltered BuildBoundingBoxFiltered(Entity.Entity parent, ModelMesh mesh, Matrix meshTransform)
        {
            // Create initial variables to hold min and max xyz values for the mesh
            Vector3 meshMax = new Vector3(float.MinValue);
            Vector3 meshMin = new Vector3(float.MaxValue);
 
            foreach (ModelMeshPart part in mesh.MeshParts)
            {
                // The stride is how big, in bytes, one vertex is in the vertex buffer
                // We have to use this as we do not know the make up of the vertex
                int stride = part.VertexBuffer.VertexDeclaration.VertexStride;
 
                VertexPositionNormalTexture[] vertexData = new VertexPositionNormalTexture[part.NumVertices];
                part.VertexBuffer.GetData(part.VertexOffset * stride, vertexData, 0, part.NumVertices, stride);
 
                // Find minimum and maximum xyz values for this mesh part
                Vector3 vertPosition = new Vector3();
 
                for (int i = 0; i < vertexData.Length; i++)
                {
                    vertPosition = vertexData[i].Position;
 
                    // update our values from this vertex
                    meshMin = Vector3.Min(meshMin, vertPosition);
                    meshMax = Vector3.Max(meshMax, vertPosition);
                }
            }
 
            // transform by mesh bone matrix
            meshMin = Vector3.Transform(meshMin, meshTransform);
            meshMax = Vector3.Transform(meshMax, meshTransform);
            
            //Console.WriteLine("MIN: " + meshMin + " MAX: " + meshMax);
 
            // Create the bounding box
            BoundingBox box = new BoundingBox(meshMin, meshMax);

            BoundingBoxFiltered boxFitlered = new BoundingBoxFiltered(parent, box);
            return boxFitlered;
        }
        
        public static BoundingBox BuildBoundingBox(ModelMesh mesh, Matrix meshTransform)
        {
            // Create initial variables to hold min and max xyz values for the mesh
            Vector3 meshMax = new Vector3(float.MinValue);
            Vector3 meshMin = new Vector3(float.MaxValue);
 
            foreach (ModelMeshPart part in mesh.MeshParts)
            {
                // The stride is how big, in bytes, one vertex is in the vertex buffer
                // We have to use this as we do not know the make up of the vertex
                int stride = part.VertexBuffer.VertexDeclaration.VertexStride;
 
                VertexPositionNormalTexture[] vertexData = new VertexPositionNormalTexture[part.NumVertices];
                part.VertexBuffer.GetData(part.VertexOffset * stride, vertexData, 0, part.NumVertices, stride);
 
                // Find minimum and maximum xyz values for this mesh part
                Vector3 vertPosition = new Vector3();
 
                for (int i = 0; i < vertexData.Length; i++)
                {
                    vertPosition = vertexData[i].Position;
 
                    // update our values from this vertex
                    meshMin = Vector3.Min(meshMin, vertPosition);
                    meshMax = Vector3.Max(meshMax, vertPosition);
                }
            }
 
            // transform by mesh bone matrix
            meshMin = Vector3.Transform(meshMin, meshTransform);
            meshMax = Vector3.Transform(meshMax, meshTransform);
            
            //Console.WriteLine("MIN: " + meshMin + " MAX: " + meshMax);
 
            // Create the bounding box
            BoundingBox box = new BoundingBox(meshMin, meshMax);
            return box;
        }
    }
}