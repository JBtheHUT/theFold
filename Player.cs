using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TheFold
{
    class Player
    {
        private Vector3 position, vel;
        private float radius, terrainY, velResTimer;
        private bool onGround;
        //terrainY is the horrible name for the position of the terrain mesh where the player or the sheep is touching
        //-------------------constructer
        public Player(Vector3 pos, float r)
        {
            position = pos;
            radius = r;
            onGround = false;
            velResTimer = 500;
        }

        public BoundingSphere boundingSphere
        {
            get { return new BoundingSphere(position, radius); }
        }
        //----------------------accessor
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
        public float TerrainY
        {
            get { return terrainY; }
            set { terrainY = value; }
        }
        public float VelResTimer
        {
            get { return velResTimer; }
            set { velResTimer = value; }
        }


        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public bool OnGround
        {
            get { return onGround; }
            set { onGround = value; }
        }
        //----------------------other
        public void Update(GameTime gameTime)
        {
         //   Console.Out.WriteLine("Player position is: "+position);

            Vector3 gravity = new Vector3(0, -1, 0);
            vel += (gravity * (float)gameTime.ElapsedGameTime.TotalSeconds);

            position += vel * (float)gameTime.ElapsedGameTime.TotalSeconds;

            velResTimer -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            
            if (velResTimer < 0)
            {
                vel = Vector3.Zero;
                gravity = new Vector3(0, -500, 0);
                vel += (gravity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            
            gravity = new Vector3(0, -1, 0);
            vel += (gravity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            position += vel * (float)gameTime.ElapsedGameTime.TotalSeconds;
            /*
            if (onGround == true)
            {
                vel.Y = terrainY;
            }
            */

        }

        public void checkCollision(Vector3 pos, float r)
        {
            if (new BoundingSphere(this.position, this.radius).Intersects(new BoundingSphere(pos, r)))
            {
                Vector3 axis = pos - this.position;
                float dist = r + this.radius;
                float move = (dist - axis.Length()) / 2f;
                axis.Normalize();
                Vector3 U1x = axis * Vector3.Dot(axis, this.vel);
                U1x.Y = 0;
                Vector3 U1y = this.vel - U1x;
                U1y.Y = 0;

                Vector3 U2x = -axis * Vector3.Dot(-axis, new Vector3(0, 0, 0));//i replaced the trees velocity with vector (0,0,0)
                //Vector3 U2y = other.Vel - U2x;

                Vector3 V1x = U2x;
                Vector3 V2x = U1x;

                this.vel = (V1x + U1y);// *damping[j];
                                       //other.Vel = (V2x + U2y);// *damping[i];
                                       // sphereVel[i] *= 0.99f;
                                       // sphereVel[j] *= 0.99f;

                axis.Y = 0;

                //other.Position += axis * move;
                this.position -= axis * move;
            }
        }
    }
}
