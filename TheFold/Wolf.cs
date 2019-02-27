using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TheFold
{
    class Wolf
    {
        private Vector3 position, vel, tgtPosition, wolfDir, runDir, oppVel;
        private float radius, terrainY, velResTimer, runAwayTime;
        private bool onGround, tgtAcquired, runAway;
        private String tgtName;




        public Wolf(Vector3 pos, float r)
        {
            position = pos;
            radius = r;
            onGround = false;
            velResTimer = 500;
            tgtAcquired = false;
            //tgtPosition = pos;
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
        public Vector3 TgtPosition
        {
            get { return tgtPosition; }
            set { tgtPosition = value; }
        }
        public Vector3 Vel
        {
            get { return vel; }
            set { vel = value; }
        }
        public Vector3 RunDir
        {
            get { return runDir; }
            set { runDir = value; }
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
        public bool OnGround
        {
            get { return onGround; }
            set { onGround = value; }
        }
        public bool TgtAcquired
        {
            get { return tgtAcquired; }
            set { tgtAcquired = value; }
        }
        public string TgtName
        {
            get { return tgtName; }
            set { tgtName = value; }
        }
        public Boolean RunAway
        {
            get { return runAway; }
            set { runAway = value; }
        }


        public void Update(GameTime gameTime)
        {
            Vector3 gravity = new Vector3(0, -10, 0);
            vel += (gravity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            runAwayTime -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;


            if (tgtAcquired == true)
            {
                wolfDir = (tgtPosition - position);// *.2f;
                vel = Vector3.Normalize(wolfDir)*200f;
                vel += (gravity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            else
            {
                vel = Vector3.Zero;
                vel += (gravity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }


            if (runAway == true)    //this resets the timer below
            {
                runAwayTime = 500;
                runAway = false;
               // vel = -vel;
                vel = runDir;
                // vel += (gravity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            if (runAwayTime > 0)    //this timer is for the duration of a sheep runnin away
            {
                vel += runDir;
                vel += (gravity * (float)gameTime.ElapsedGameTime.TotalSeconds);
                position += vel * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }



            vel += (gravity * (float)gameTime.ElapsedGameTime.TotalSeconds);

            position += vel * (float)gameTime.ElapsedGameTime.TotalSeconds;

        }

    }

}
