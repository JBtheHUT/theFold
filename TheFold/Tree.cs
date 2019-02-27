using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TheFold
{
    class Tree
    {
        private Vector3 position, vel;
        private float radius;

        public Tree(Vector3 pos, float r)
        {
            position = pos;
            radius = r;
            vel = new Vector3(0, 0, 0);
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Vector3 Vel
        {
            get { return vel; }
            set { vel = value; }
        }
        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }
        public BoundingSphere boundingSphere
        {
            get { return new BoundingSphere(position, radius); }
        }


        //i want to do tree collision
        /*
        public void checkCollision(Tree other, Sheep shp)
        {
            if (new BoundingSphere(shp.Position, shp.Radius).Intersects(new BoundingSphere(other.Position, other.Radius)))
            {
                Vector3 axis = other.Position - shp.Position;
                float dist = other.Radius + shp.Radius;
                float move = (dist - axis.Length()) / 2f;
                axis.Normalize();
                Vector3 U1x = axis * Vector3.Dot(axis, shp.Vel);
                Vector3 U1y = shp.Vel - U1x;

                Vector3 U2x = -axis * Vector3.Dot(-axis, other.Vel);
                Vector3 U2y = other.Vel - U2x;

                Vector3 V1x = U2x;
                Vector3 V2x = U1x;

                shp.Vel = (V1x + U1y);// *damping[j];
                other.Vel = (V2x + U2y);// *damping[i];
                                        // sphereVel[i] *= 0.99f;
                                        // sphereVel[j] *= 0.99f;

                other.Position += axis * move;
                shp.Position -= axis * move;
            }
        }
        */
    }
}
