using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TheFold
{
    public class Sheep
    {

       static Random rnd;
       private Vector3 position, sheepDir, runDir, hrdGDir, vel, lastTouch;
       private String name;
       private Boolean isMeander, runAway, herdGrav, onGround, isStraggle, notFound;
       private float meander, runAwayTime, terrainY, radius, velResTimer, notFoundTimer, 
            turnShpX, soundingFirstX, soundingSecondZ, soundingFirstZ, soundingSecondX; //useless

        private Quaternion orient, shpOrientZ, shpOrientX;


        public BoundingSphere boundingSphere
        {
            get { return new BoundingSphere(position, radius); }
        }

        public Sheep(Vector3 pos, String s, Vector3 ShD, float m, float r)//using this method to create sheep
        {
            position = pos;
            name = s;
            isMeander = false;
            runAway = false;
            meander = m;
            sheepDir = ShD;//initial starting direction
            velResTimer = 500;
            onGround = false;
            terrainY = 0;

            notFound = true;
            isStraggle = false;
            radius = r;
            soundingFirstX = pos.X;
            soundingFirstZ = pos.Z;
            soundingSecondX = 0;
            soundingSecondZ = 0;

            notFoundTimer = 20000f;
            


            if (rnd == null)
                rnd = new Random();
        }
        //------------------------accessor classes-------------------------
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Vector3 LastTouch
        {
            get { return lastTouch; }
            set { lastTouch = value; }
        }
        public Vector3 HrdGDir
        {
            get { return hrdGDir; }
            set { hrdGDir = value; }
        }
        public Vector3 RunDir
        {
            get { return runDir; }
            set { runDir = value; }
        }
        public Boolean RunAway
        {
            get { return runAway; }
            set { runAway = value; }
        }
        public Boolean HerdGrav
        {
            get { return herdGrav; }
            set { herdGrav = value; }
        }
        public Boolean IsStraggle
        {
            get { return isStraggle; }
            set { isStraggle = value; }
        }
        public Boolean NotFound
        {
            get { return notFound; }
            set { notFound = value; }
        }
        public bool OnGround
        {
            get { return onGround; }
            set { onGround = value; }
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
        public Vector3 Vel
        {
            get { return vel; }
            set { vel = value; }
        }
        
        public Quaternion ShpOrientX
        {
            get { return shpOrientX; }
            set { shpOrientX = value; }
        }
        public Quaternion ShpOrientZ
        {
            get { return shpOrientZ; }
            set { shpOrientZ = value; }
        }
        public String Name
        {
            get { return name; }
            set { name = value; }
        }
        public Vector3 SheepDir
        {
            get { return sheepDir; }

            set { sheepDir = value; }
        }
        public float Meander
        {
            get { return meander; }
            set { meander = value; }
        }
        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }
        public float SoundingFirstX
        {
            get { return soundingFirstX; }
            set { soundingFirstX = value; }
        }
        public float SoundingFirstZ
        {
            get { return soundingFirstZ; }
            set { soundingFirstZ = value; }
        }
        public float SoundingSecondZ
        {
            get { return soundingSecondZ; }
            set { soundingSecondZ = value; }
        }
        public float SoundingSecondX
        {
            get { return soundingSecondX; }
            set { soundingSecondX = value; }
        }


        //----------------------- end accessors---------------------
        public void findOrient(float x1, float z1, float x2, float z2)
        {
            float orientX = x1 - x2;
            float orientZ = z1 - z2;
            //Console.Out.WriteLine("OrientX = "+orientX+" , OrientZ = "+orientZ);
            shpOrientX = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), orientX);
            shpOrientZ = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), orientZ);
        }

        public void Update(GameTime gameTime)
        {

            soundingFirstX = position.X;
            soundingFirstZ = position.Z;

            Vector3 gravity = new Vector3(0, -1, 0);
            vel += (gravity * (float)gameTime.ElapsedGameTime.TotalSeconds);

            meander -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            runAwayTime -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            notFoundTimer -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (notFoundTimer < 0)
            {
                notFound = true;
                notFoundTimer = 20000;
            }


            //position += vel * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (meander < 0)    //this is the countdown until a sheeps direction changes. also resets itself
            {
                vel = Vector3.Zero;
                isMeander = !isMeander;
                 meander = 8000 - rnd.Next(8000);
                //sheepDir = Vector3.Normalize(new Vector3(rnd.Next(360) - rnd.Next(360), 0, rnd.Next(360) - rnd.Next(360))) * 50f;
                sheepDir = new Vector3(rnd.Next(360) - rnd.Next(360), 0, rnd.Next(360) - rnd.Next(360)) * .1f;
                vel += sheepDir;
                turnShpX = sheepDir.X;

               // soundingSecondX = sheepDir.X;
              //  soundingSecondZ = sheepDir.Z;

              //  if(isMeander == true)   //this is just my bumbling attempt to make the sheep turn the direction I want
                 //   findOrient(soundingFirstX, soundingFirstZ, soundingSecondX, soundingSecondZ);


                //shpOrientX = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), turnShpX);

            }

            if (isMeander == true)  //sheeps independent movement while true, the false time should be still unless herded. like grazing.
            {
                //vel = sheepDir * 20f;
                vel += (gravity * (float)gameTime.ElapsedGameTime.TotalSeconds);
                position += vel * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if(isMeander == false)
            {
                vel = Vector3.Zero;
                gravity = new Vector3(0, -1000, 0);
                vel += (gravity * (float)gameTime.ElapsedGameTime.TotalSeconds);
                position += vel * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //this if statement was created in attemmpt to prevent the sheep floating while not meandering
            }

            if (runAway == true)    //this resets the timer below
            {
                runAwayTime = 500;
                runAway = false;
                vel = runDir;
               // vel += (gravity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            if (runAwayTime > 0)    //this timer is for the duration of a sheep runnin away
            {
                //vel += runDir;
                vel += (gravity * (float)gameTime.ElapsedGameTime.TotalSeconds);
                position += vel * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            

            if (Vector3.Distance(new Vector3(0, lastTouch.Y, 0), new Vector3(0, position.Y, 0)) > 2)
                vel.Y += -1;//this if statement is my attempt to prevent upward bouncing, free floating

            if (herdGrav == true)
                vel += hrdGDir;
            /*
            checkCollision(new Plane(Vector3.Up, 0f));
            checkCollision(new Plane(Vector3.Right, -35f));
            checkCollision(new Plane(Vector3.Left, -35f));
            checkCollision(new Plane(Vector3.Backward, -35f));
            checkCollision(new Plane(Vector3.Forward, -35f));
            */
        }

        public void checkCollision(Plane other)
        {
            if (PlaneIntersectionType.Intersecting ==
                 new BoundingSphere(this.position, this.radius).Intersects(other))
            {

                //this.pos.Y = this.radius;

                float dist = Vector3.Dot(other.Normal, this.position - (other.Normal * other.D));
                if (dist < this.radius)
                {
                    position = position + (other.Normal * (radius - dist));
                   // Console.Out.WriteLine("sheep "+this.name+" hit");
                }
                //  else if (dist < -this.radius)
                {
                    //    pos = pos + (-other.Normal * (radius + dist));
                }

                //make sure the sphere is moving before we fix its direction
                if (this.vel.Length() > 0.001f)
                {
                    Vector3 V = this.vel;
                    V.Normalize();
                    V =(Vector3.Dot(-V, other.Normal))
                             * other.Normal + V;
                    this.vel = this.vel.Length() * V;
                }


                //this.vel = 2 * Vector3.Dot(-this.vel, other.Normal) * other.Normal + this.vel;
                //this.vel = Vector3.Zero;
            }
        }

        public void checkCollision(Sheep other)
        {
            if (new BoundingSphere(this.position, this.radius).Intersects(new BoundingSphere(other.position, other.radius)))
            {
                Vector3 axis = other.Position - this.position;
                float dist = other.radius + this.radius;
                float move = (dist - axis.Length()) / 2f;
                axis.Normalize();
                Vector3 U1x = axis * Vector3.Dot(axis, this.vel);
                Vector3 U1y = this.vel - U1x;

                Vector3 U2x = -axis * Vector3.Dot(-axis, other.vel);
                Vector3 U2y = other.vel - U2x;

                Vector3 V1x = U2x;
                Vector3 V2x = U1x;

                this.vel = (V1x + U1y);// *damping[j];
                other.vel = (V2x + U2y);// *damping[i];
                                        // sphereVel[i] *= 0.99f;
                                        // sphereVel[j] *= 0.99f;

                other.position += axis * move;
                this.position -= axis * move;
            }
        }
        //tree collision
       
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

                Vector3 U2x = -axis * Vector3.Dot(-axis, new Vector3(0,0,0));//i replaced the trees velocity with vector (0,0,0)
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

