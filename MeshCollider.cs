using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheFold
{
    class MeshCollider
    {
        /// <summary>
        /// The list of verts from all meshes
        /// </summary>
        List<Vector3> verts;
        /// <summary>
        /// The list of normals from all meshes
        /// </summary>
        List<Vector3> norms;

        public MeshCollider(Model model, Matrix world)//the point of this constructor is to generate a list of verts and norms
        {
            verts = new List<Vector3>();
            norms = new List<Vector3>();
            foreach (ModelMesh mesh in model.Meshes)

            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    //numVertices == total number of 3D points
                    //PrimitiveCount == face count
                    //VertexBuffer == 1D array of [x,y,z,x,y,z,x,y,z,.....]
                    //IndexBuffer == 1D array of faces (e.g. triangles give 3 vertex indices)

                    //get how many bytes to jump to get to 
                    //next vertex in VertexBuffer
                    int stride = part.VertexBuffer.VertexDeclaration.VertexStride;

                    VertexPositionNormalTexture[] vertexData
                        = new VertexPositionNormalTexture[part.NumVertices];
                    part.VertexBuffer.GetData<VertexPositionNormalTexture>(vertexData);

                    //the array of vertices are unique.  If a triangle reuses an exisiting vertex, it will not "reappear" in the list
                    //therefore we need to make a copy of each vertex used in every triangle...
                    ushort[] indices = new ushort[part.IndexBuffer.IndexCount];
                    part.IndexBuffer.GetData<ushort>(indices);

                    //Make a new list of vertices that we can easily traverse through (faces * 3)
                    Vector3 v = new Vector3();
                    for (int i = 0; i < indices.Length; i++)
                    {
                        v.X = vertexData[indices[i]].Position.X;
                        v.Y = vertexData[indices[i]].Position.Y;
                        v.Z = vertexData[indices[i]].Position.Z;

                        verts.Add(Vector3.Transform(v, mesh.ParentBone.Transform * world));

                        //if we've added 3 new verts to the List of verts
                        //then add a new normal to the list
                        if (verts.Count % 3 == 0)
                        {
                            Vector3 Normal = Vector3.Cross(verts[verts.Count - 1] - verts[verts.Count - 3], verts[verts.Count - 2] - verts[verts.Count - 3]);
                            Normal.Normalize();
                            norms.Add(Normal);
                        }
                    }
                }
            }
        }
        public static bool SameSide(Vector3 p1, Vector3 p2, Vector3 a, Vector3 b)
        {
            Vector3 cp1 = Vector3.Cross(b - a, p1 - a);
            Vector3 cp2 = Vector3.Cross(b - a, p2 - a);
            if (Vector3.Dot(cp1, cp2) >= 0)
            {
                return true;
            }

            return false;
        }

        public static bool PointInTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
        {
            if (SameSide(p, a, b, c) && SameSide(p, b, a, c) && SameSide(p, c, a, b))
            {
                return true;
            }

            return false;
        }//is it on the same side of P
        /*
        public bool checkCollisionAndResponse(Sheep e)
        {
            //A quad is defined by 6 vertices (3 per triangle).  Since we assume both triangles are on the
            // same plane, it is not required to check the collision with the other triangle, so skip by 6.

            bool collision = false;
            for (int w = 0; w < verts.Count; w += 3)
            {

                //sign of dist will determine which side of the wall we are on
                float dist = Vector3.Dot(norms[w / 3], (e.Position - verts[w]));



                if (Math.Abs(dist) < e.boundingSphere.Radius
                      && PointInTriangle(e.Position, verts[w], verts[w + 1], verts[w + 2]))  //collision
                {

                    if (dist < e.boundingSphere.Radius)
                    {
                        e.Position = e.Position + (norms[w / 3] * (e.boundingSphere.Radius - dist));
                    }
                    else if (dist < -e.boundingSphere.Radius)
                    {
                        e.Position = e.Position + (-norms[w / 3] * (e.boundingSphere.Radius + dist));
                    }

                    Vector3 V = e.Vel;
                    V.Normalize();
                    V = 2 * (Vector3.Dot(-V, norms[w / 3]))
                             * norms[w / 3] + V;
                    e.Vel = (e.Vel.Length() * V)/2;

                    collision = true;

                }
            }
            return collision;
        }
        */
        public bool checkCollisionAndResponse(Sheep e)
        {
            //A quad is defined by 6 vertices (3 per triangle).  Since we assume both triangles are on the
            // same plane, it is not required to check the collision with the other triangle, so skip by 6.
            e.OnGround = false;
            bool collision = false;
            for (int w = 0; w < verts.Count; w += 3)
            {

                //sign of dist will determine which side of the wall we are on
                float dist = Vector3.Dot(norms[w / 3], (e.Position - verts[w]));



                if (Math.Abs(dist) < e.boundingSphere.Radius
                      && PointInTriangle(e.Position, verts[w], verts[w + 1], verts[w + 2]))  //collision
                                                                                             //e.Vel = new Vector3(0, 1, 0);
                {

                    e.TerrainY = e.Position.Y;  //hopefully the height of the point of collision
                    e.OnGround = true;
                    e.LastTouch = new Vector3(e.Position.X, e.Position.Y, e.Position.Z);
                    e.LastTouch = e.Position;

                    if (dist < e.boundingSphere.Radius)
                    {
                        e.Position = e.Position + (norms[w / 3] * (e.boundingSphere.Radius - dist));
                    }
                    else if (dist < -e.boundingSphere.Radius)
                    {
                        e.Position = e.Position + (-norms[w / 3] * (e.boundingSphere.Radius + dist));
                    }
                    //the below velocity adjustment should allow for 'sliding wall' movement
                    Vector3 V = e.Vel;
                    V.Normalize();
                    V = /*just took away the '*2' bit here*/(Vector3.Dot(-V, norms[w / 3]))
                             * norms[w / 3] + V;
                    e.Vel = e.Vel.Length() * V;
                    //next line should hopefully limit up ward bouncing
                    //actually I guess I should put this into sheep
                    



                    collision = true;

                }
            }
            return collision;
        }

        public bool checkCollisionAndResponse(Player e)
        {
            //A quad is defined by 6 vertices (3 per triangle).  Since we assume both triangles are on the
            // same plane, it is not required to check the collision with the other triangle, so skip by 6.
            e.OnGround = false;
            bool collision = false;
            for (int w = 0; w < verts.Count; w += 3)
            {

                //sign of dist will determine which side of the wall we are on
                float dist = Vector3.Dot(norms[w / 3], (e.Position - verts[w]));



                if (Math.Abs(dist) < e.boundingSphere.Radius
                      && PointInTriangle(e.Position, verts[w], verts[w + 1], verts[w + 2]))  //collision
                                                                                             //e.Vel = new Vector3(0, 1, 0);
                {

                    e.TerrainY = e.Position.Y;  //hopefully the height of the point of collision
                    e.OnGround = true;
                    
                    if (dist < e.boundingSphere.Radius)
                    {
                        e.Position = e.Position + (norms[w / 3] * (e.boundingSphere.Radius - dist));
                    }
                    else if (dist < -e.boundingSphere.Radius)
                    {
                        e.Position = e.Position + (-norms[w / 3] * (e.boundingSphere.Radius + dist));
                    }
                    collision = true;
                }
                /*
                Vector3 V = e.Vel;
                V.Normalize();
                V =  (Vector3.Dot(-V, norms[w / 3]))
                         * norms[w / 3] + V;
                e.Vel = e.Vel.Length() * V;
                */


            }
            return collision;
        }

        public bool checkCollisionAndResponse(Wolf e)
        {
            //A quad is defined by 6 vertices (3 per triangle).  Since we assume both triangles are on the
            // same plane, it is not required to check the collision with the other triangle, so skip by 6.
            e.OnGround = false;
            bool collision = false;
            for (int w = 0; w < verts.Count; w += 3)
            {

                //sign of dist will determine which side of the wall we are on
                float dist = Vector3.Dot(norms[w / 3], (e.Position - verts[w]));



                if (Math.Abs(dist) < e.boundingSphere.Radius
                      && PointInTriangle(e.Position, verts[w], verts[w + 1], verts[w + 2]))  //collision
                                                                                             //e.Vel = new Vector3(0, 1, 0);
                {

                    e.TerrainY = e.Position.Y;  //hopefully the height of the point of collision
                    e.OnGround = true;

                    if (dist < e.boundingSphere.Radius)
                    {
                        e.Position = e.Position + (norms[w / 3] * (e.boundingSphere.Radius - dist));
                    }
                    else if (dist < -e.boundingSphere.Radius)
                    {
                        e.Position = e.Position + (-norms[w / 3] * (e.boundingSphere.Radius + dist));
                    }
                    collision = true;
                }/*
                Vector3 V = e.Vel;
                V.Normalize();
                V =  (Vector3.Dot(-V, norms[w / 3]))
                         * norms[w / 3] + V;
                e.Vel = e.Vel.Length() * V;
                */


            }
            return collision;
        }

        public bool checkCollisionAndResponse(ref Vector3 pos, float radius, ref Vector3 vel)
        {
            //A quad is defined by 6 vertices (3 per triangle).  Since we assume both triangles are on the
            // same plane, it is not required to check the collision with the other triangle, so skip by 6.

            bool collision = false;
            for (int w = 0; w < verts.Count; w += 3)
            {
                //sign of dist will determine which side of the wall we are on
                float dist = Vector3.Dot(norms[w / 3], (pos - verts[w]));

                if (Math.Abs(dist) < radius
                      && PointInTriangle(pos, verts[w], verts[w + 1], verts[w + 2]))  //collision
                {

                    //    if (dist < radius)
                    {
                        pos = pos + (norms[w / 3] * (radius - dist));
                    }
                    //   else if (dist < -radius)
                    {
                        //        pos = pos + (-norms[w / 3] * (radius + dist));
                    }

                    Vector3 V = vel;
                    V.Normalize();
                    V = 2 * (Vector3.Dot(-V, norms[w / 3]))
                             * norms[w / 3] + V;
                    vel = vel.Length() * V;

                    collision = true;

                }
            }
            return collision;
        }
    }
}        