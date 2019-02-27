using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace TheFold
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont ariel;

        string gameState, deviceName;
        int score, armUpCnt, armDwnCnt;
        Model Sky, ground, shepherd, sheep, dish, wolf, fence, tree, river, deadSheep, shepArmUp, shepArmDown,
            title, pause, tut_A, tut_B, tut_C, tut_D, tut_E, tut_F, tut_G, tut_H, tut_i;
        Vector3 cameraOffset, playerPos, playerForward;
        bool playerDir, boutTaHit, timeTaHit, chkGameState;
        Random rnd;
        //List<Vector3> sheepPos;
        Player shepPlayer;
        List<Sheep> sheepList;
        List<Wolf> wolfList;
        List<Tree> treeList;
        List<Vector3> deadSheepList;
        MeshCollider worldMesh;
        Quaternion orient;//, shpOrientZ, shpOrientX;
        float turnOrient, hitLag, titleTimer;//, turnShpX, turnShpZ;

        SoundEffect deathBleat;
        
        public void clearGameData()
        {
            //foreach (Wolf wlf in wolfList)
            
            wolfList.Clear();
            sheepList.Clear();
            deadSheepList.Clear();
            shepPlayer = null;
            
        }
        public void resetGame()
        {
            for (int i = 0; i < 80; i++)
            {
                sheepList.Add(new Sheep(new Vector3(rnd.Next(50) - rnd.Next(50), 5, rnd.Next(50) - rnd.Next(50)),
                    "BoPeep_" + itoa(i).ToString(),
                    FindMeander(),
                    (5000 - rnd.Next(5000)),
                    sheep.Meshes[0].BoundingSphere.Radius));//Landon's code in sphere uses "sphere.Meshes[0].BoundingSphere.Radius"
                //Console.Out.WriteLine("sheepList[" + i + "] : " + sheepList[i].Name + "starts at " + sheepList[i].Position);
                Console.Out.WriteLine("sheepList[" + i + "] : " + sheepList[i].Name + "sheep dir is" + sheepList[i].SheepDir);
            }
            wolfList.Add(new Wolf(new Vector3(1736, -81, -651),
                wolf.Meshes[0].BoundingSphere.Radius));
            wolfList.Add(new Wolf(new Vector3(2235, -152, -515),
                wolf.Meshes[0].BoundingSphere.Radius));
            wolfList.Add(new Wolf(new Vector3(1941, -84, -52),
                wolf.Meshes[0].BoundingSphere.Radius));
            wolfList.Add(new Wolf(new Vector3(1190, -74, 172),
                wolf.Meshes[0].BoundingSphere.Radius));
            wolfList.Add(new Wolf(new Vector3(870, -22, -463),
                wolf.Meshes[0].BoundingSphere.Radius));
            wolfList.Add(new Wolf(new Vector3(1236, -131, -373),
                wolf.Meshes[0].BoundingSphere.Radius));
            wolfList.Add(new Wolf(new Vector3(3000, -116, -525),
                wolf.Meshes[0].BoundingSphere.Radius));
            wolfList.Add(new Wolf(new Vector3(3261, -85, 172),
                wolf.Meshes[0].BoundingSphere.Radius));



            gameState = "title";
            titleTimer = 150;
            playerPos = new Vector3(0, 0, 0);
            score = 0;
            boutTaHit = false;
            timeTaHit = false;
            shepPlayer = new Player(playerPos, (shepherd.Meshes[0].BoundingSphere.Radius) - 3);

        }
        public Char itoa(int i)
        {
            return (Char)(Convert.ToUInt16('A') + i);
        }
        public Vector3 FindMeander()    //this just gives a randomized normalized direction for the sheep to wander toward, called in load
        {
            Vector3 meanderDir = new Vector3(rnd.Next(360) - rnd.Next(360),
                0, rnd.Next(360) - rnd.Next(360));
            meanderDir.Normalize();
            return meanderDir*50;
        }
        public Vector3 FindTarget(String tgtName)
        {
            Vector3 target = new Vector3(0,0,0);

            foreach(Sheep shp in sheepList)
            {
                if (shp.Name == tgtName)
                {
                    target = shp.Position;
                    target += new Vector3(0, -10, 0);
                }
            }

            return target;
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
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.Window.AllowUserResizing = true;
            this.Window.EndScreenDeviceChange(this.Window.ScreenDeviceName, 1500, 1500);
        }

        protected override void Initialize()
        {
            turnOrient = 0;
            playerPos = new Vector3(0,0,0);
            playerForward = Vector3.Right;
            cameraOffset = new Vector3(0, 50, -150);
            playerDir = true;
            rnd = new Random();
            score = 0;
            boutTaHit = false;
            timeTaHit = false;
            chkGameState = false;
            armUpCnt = 0;
            armDwnCnt = 0;
            hitLag = 0;
            gameState = "title";
            titleTimer = 150;
//            gameState = "game";
            //deadSheepList = null;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            treeList = new List<Tree>();
            sheepList = new List<Sheep>();
            wolfList = new List<Wolf>();
            deadSheepList = new List<Vector3>();
            spriteBatch = new SpriteBatch(GraphicsDevice);

            deathBleat = Content.Load<SoundEffect>("deathBleat");

            ariel = Content.Load<SpriteFont>("File");
            shepArmUp = Content.Load<Model>("shepArmUp");
            shepArmDown = Content.Load<Model>("shepArmDown");
            river = Content.Load<Model>("river");
            Sky = Content.Load<Model>("skysphere");
            ground = Content.Load<Model>("theFoldTerrain");
            fence = Content.Load<Model>("theFoldFence");
            tree = Content.Load<Model>("theFoldTreesSpread");
            sheep = Content.Load<Model>("finalSheep4");
            title = Content.Load<Model>("newTitle_Screen");
            wolf = Content.Load<Model>("wolfModel");
            dish = Content.Load<Model>("dish");
            shepherd = Content.Load<Model>("shepherdC10");
            deadSheep = Content.Load<Model>("deadSheep");
            pause = Content.Load<Model>("newTitle_Screen");
            tut_A = Content.Load<Model>("winCon_A");
            tut_B = Content.Load<Model>("winCon_B");
            tut_C = Content.Load<Model>("herdSafety");
            tut_D = Content.Load<Model>("single_Kill");
            tut_E = Content.Load<Model>("herd_screen");
            tut_F = Content.Load<Model>("meander_screen");
            tut_G = Content.Load<Model>("hit_A");
            tut_H = Content.Load<Model>("hit_B");
            tut_i = Content.Load<Model>("controlScreen");

            worldMesh = new MeshCollider(ground, Matrix.CreateScale(8f) * Matrix.CreateTranslation(0, -5, 0));

            for (int i = 0; i < 80; i++)
            {
                sheepList.Add(new Sheep(new Vector3(rnd.Next(50) - rnd.Next(50), 5, rnd.Next(50) - rnd.Next(50)),
                    "BoPeep_" + itoa(i).ToString(),
                    FindMeander(),
                    (5000 - rnd.Next(5000)),
                    sheep.Meshes[0].BoundingSphere.Radius));//Landon's code in sphere uses "sphere.Meshes[0].BoundingSphere.Radius"
                //Console.Out.WriteLine("sheepList[" + i + "] : " + sheepList[i].Name + "starts at " + sheepList[i].Position);
                Console.Out.WriteLine("sheepList[" + i + "] : " + sheepList[i].Name + "sheep dir is" + sheepList[i].SheepDir);
            }

            Console.Out.WriteLine("Sheep remaining" + sheepList.Count);

            //loading wolves and trees  like a noob
            #region
            
            wolfList.Add(new Wolf(new Vector3(1736, -81, -651),
                wolf.Meshes[0].BoundingSphere.Radius));
            wolfList.Add(new Wolf(new Vector3(2235, -152, -515),
                wolf.Meshes[0].BoundingSphere.Radius));
            wolfList.Add(new Wolf(new Vector3(1941, -84, -52),
                wolf.Meshes[0].BoundingSphere.Radius));
            wolfList.Add(new Wolf(new Vector3(1190, -74, 172),
                wolf.Meshes[0].BoundingSphere.Radius));
            wolfList.Add(new Wolf(new Vector3(870, -22, -463),
                wolf.Meshes[0].BoundingSphere.Radius));
            wolfList.Add(new Wolf(new Vector3(1236, -131, -373),
                wolf.Meshes[0].BoundingSphere.Radius));
            wolfList.Add(new Wolf(new Vector3(3000, -116, -525),
                wolf.Meshes[0].BoundingSphere.Radius));
            wolfList.Add(new Wolf(new Vector3(3261, -85, 172),
                wolf.Meshes[0].BoundingSphere.Radius));
                
            treeList.Add(new Tree(new Vector3(535, -40, -250),
                tree.Meshes[0].BoundingSphere.Radius+10));
            treeList.Add(new Tree(new Vector3(373, -14, -242),
                tree.Meshes[0].BoundingSphere.Radius+10));
            treeList.Add(new Tree(new Vector3(676, -55, -29),
                tree.Meshes[0].BoundingSphere.Radius+10));
            treeList.Add(new Tree(new Vector3(841, -69, -125),
                tree.Meshes[0].BoundingSphere.Radius+10));
            treeList.Add(new Tree(new Vector3(957, -80, 294),
                tree.Meshes[0].BoundingSphere.Radius+10));

            treeList.Add(new Tree(new Vector3(1186, -107, -375),
                tree.Meshes[0].BoundingSphere.Radius+10));
            treeList.Add(new Tree(new Vector3(1371, -121, -448),
                tree.Meshes[0].BoundingSphere.Radius+10));
            treeList.Add(new Tree(new Vector3(1306, -118, -283),
                tree.Meshes[0].BoundingSphere.Radius+10));
            treeList.Add(new Tree(new Vector3(1379, -122, -160),
                tree.Meshes[0].BoundingSphere.Radius+10));
            treeList.Add(new Tree(new Vector3(1537, -127, -29),
                tree.Meshes[0].BoundingSphere.Radius+10));

            treeList.Add(new Tree(new Vector3(1702, -125, -74),
                tree.Meshes[0].BoundingSphere.Radius+10));
            treeList.Add(new Tree(new Vector3(1863, -128, -378),
                tree.Meshes[0].BoundingSphere.Radius+10));
            treeList.Add(new Tree(new Vector3(2236, -133, -458),
                tree.Meshes[0].BoundingSphere.Radius+10));
            treeList.Add(new Tree(new Vector3(2482, -133, -338),
                tree.Meshes[0].BoundingSphere.Radius+10));
            treeList.Add(new Tree(new Vector3(2369, -130, -138),
                tree.Meshes[0].BoundingSphere.Radius+10));

            treeList.Add(new Tree(new Vector3(2436, -106, 199),
                tree.Meshes[0].BoundingSphere.Radius+10));
            treeList.Add(new Tree(new Vector3(2678, -104, 430),
                tree.Meshes[0].BoundingSphere.Radius+10));
            treeList.Add(new Tree(new Vector3(3895, -31, -400),
                tree.Meshes[0].BoundingSphere.Radius+10));

            #endregion


            //this will give wolves random positions but they float. needs to find the nearest intersecting y value.
            /*
            for (int i = 0; i < 6; i++)
            {
                wolfList.Add(new Wolf(new Vector3(1500, -10, rnd.Next(100) - rnd.Next(100)), 
                    wolf.Meshes[0].BoundingSphere.Radius));
            }
            */
            shepPlayer = new Player(playerPos, (shepherd.Meshes[0].BoundingSphere.Radius)-3);//reducing the radius on the bounding sphere didnt seem to affect the floating bug
            //shepherd model is floating above the ground a little
            //in order to fix I will manually shrink the bounding sphere by reducing the radius here by 1
            //didnt work

        }
         
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            titleTimer -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            

            if (titleTimer < 0)//this timer is to avoid the lightning fast checks that makes pausing very flickery
            {
                KeyboardState kb = Keyboard.GetState();
                if (kb.IsKeyDown(Keys.Enter))
                {

                    switch (gameState)
                    {
                        case "title":
                            gameState = "tutorial_A";
                            titleTimer = 150;
                            break;
                        case "tutorial_A":
                            gameState = "tutorial_B";
                            titleTimer = 150;
                            break;
                        case "tutorial_B":
                            gameState = "tutorial_C";
                            titleTimer = 150;
                            break;
                        case "tutorial_C":
                            gameState = "tutorial_D";
                            titleTimer = 150;
                            break;
                        case "tutorial_D":
                            gameState = "tutorial_E";
                            titleTimer = 150;
                            break;
                        case "tutorial_E":
                            gameState = "tutorial_F";
                            titleTimer = 150;
                            break;
                        case "tutorial_F":
                            gameState = "tutorial_G";
                            titleTimer = 150;
                            break;
                        case "tutorial_G":
                            gameState = "tutorial_H";
                            titleTimer = 150;
                            break;
                        case "tutorial_H":
                            gameState = "tutorial_I";
                            titleTimer = 150;
                            break;
                        case "tutorial_I":
                            gameState = "game";
                            titleTimer = 150;
                            break;
                    }
                        //gamestate and tutorial if/else
                    #region
                    #endregion
                }
            }
            //--------------------------------------------------game state---------------------------------------------
            if (gameState == "game")
            {

                orient = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), turnOrient);

                KeyboardState kb = Keyboard.GetState();
                if ((shepPlayer.Position.Z > 242) && (shepPlayer.Position.Z < 480) && (shepPlayer.Position.X > 3832) && (shepPlayer.Position.X < 3984))
                     gameState = "endtitle";
    
                hitLag -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;//hit animation workaround

                    //keyboard controls
                #region
                    if (kb.IsKeyDown(Keys.E))
                    {
                        //shepPlayer.Vel = Vector3.TransformNormal(playerForward, Matrix.CreateRotationY(-0.05f));
                        playerForward = Vector3.TransformNormal(playerForward, Matrix.CreateRotationY(-0.05f));
                        turnOrient += (-0.05f);
                        playerDir = true;
                        shepPlayer.VelResTimer = 500;
                    }

                    if (kb.IsKeyDown(Keys.Q))
                    {
                        //shepPlayer.Vel = Vector3.TransformNormal(playerForward, Matrix.CreateRotationY(0.05f));
                        playerForward = Vector3.TransformNormal(playerForward, Matrix.CreateRotationY(0.05f));
                        turnOrient += (0.05f);
                        playerDir = false;
                        shepPlayer.VelResTimer = 500;
                    }

                    if (kb.IsKeyDown(Keys.W))
                    {
                        shepPlayer.Vel = playerForward * 100.0f;
                        shepPlayer.Vel += new Vector3(0, -50, 0);
                        playerDir = false;
                        shepPlayer.VelResTimer = 10;
                    }

                    if (kb.IsKeyDown(Keys.S))
                    {
                        shepPlayer.Vel = playerForward * -100.0f;
                        shepPlayer.Vel += new Vector3(0, -50, 0);
                        // playerPos -= playerForward * 3.0f;
                        playerDir = false;
                        shepPlayer.VelResTimer = 10;
                    }

                    if (kb.IsKeyDown(Keys.A))   //sidestep left 
                    {
                        shepPlayer.Vel = Vector3.Cross(Vector3.Up, playerForward) * 100.0f;
                        shepPlayer.Vel += new Vector3(0, -50, 0);
                        //playerPos += Vector3.Cross(Vector3.Up,playerForward) * 3.0f;
                        playerDir = false;
                        shepPlayer.VelResTimer = 10;
                    }

                    if (kb.IsKeyDown(Keys.D))   //sidestep right
                    {
                        shepPlayer.Vel = Vector3.Cross(playerForward, Vector3.Up) * 100.0f;
                        shepPlayer.Vel += new Vector3(0, -50, 0);
                        //playerPos += Vector3.Cross(playerForward, Vector3.Up) * 3.0f;
                        playerDir = false;
                        shepPlayer.VelResTimer = 10;
                    }

                    if (kb.IsKeyDown(Keys.Space))   //jump
                    {
                        shepPlayer.Vel += new Vector3(0, 1, 0);
                        //playerPos += Vector3.Cross(playerForward, Vector3.Up) * 3.0f;
                        playerDir = false;
                        shepPlayer.VelResTimer = 10;
                    }



                    if (kb.IsKeyDown(Keys.Enter))
                    {

                        if (titleTimer < 0)//this timer is to avoid the lightning fast checks that makes pausing very flickery
                    {
                        gameState = "title";
                    }

                        Console.Out.WriteLine("playerForward: " + playerForward);
                        Console.Out.WriteLine("playerPos: " + shepPlayer.Position);
                        titleTimer = 50;

                    }
                    #endregion

                    //sheep stuff
                #region
                    foreach (Sheep shp in sheepList)
                    {
                        if ((shp.Position.Z > 202) && (shp.Position.Z < 530) && (shp.Position.X > 3752) && (shp.Position.X < 4074))
                        {//this if will remove sheep and count them in 'score' as it enters the fold
                            sheepList.Remove(shp);
                            Console.Out.WriteLine("Sheep remaining" + sheepList.Count);
                            score++;
                            break;
                        }


                        foreach (Tree tre in treeList)
                        {
                            shp.checkCollision(tre.Position, tre.Radius);
                        }

                        shp.Update(gameTime);
                        int count = 0;

                        foreach (Sheep shpB in sheepList)
                        {
                            if (shp != shpB)
                            {
                                shp.checkCollision(shpB);//sheep on sheep collision
                            }
                            //-------------begin checking and sending relevent herding--------
                            if (Vector3.Distance(shp.Position, shpB.Position) < 100)
                            {
                                shp.HerdGrav = true;
                                shp.HrdGDir = (shpB.Position - shp.Position) * .005f;

                                //s Vector3.Normalize(shp.HrdGDir);
                                //shp.HrdGDir;
                            }
                            else
                                shp.HerdGrav = false;

                            if (Vector3.Distance(shp.Position, shpB.Position) < 10)
                                shp.HerdGrav = false;
                            //-------------end checking and sending relevent herding--------
                            if (Vector3.Distance(shp.Position, shpB.Position) < 100)//if there are 5 or more sheep within 500 of shp then it is safe from wolves
                                count++;
                        }

                        if (count < 7)
                            shp.IsStraggle = true;//if it is true that a sheep is straggling then the wolves will move in on it
                        else
                            shp.IsStraggle = false;


                        if (Vector3.Distance(shp.Position, shepPlayer.Position) < 50) //shepherd herding logic
                        {
                            float runAwaySpeed = 120 - (Vector3.Distance(shp.Position, shepPlayer.Position) * 2);
                            shp.RunAway = !shp.RunAway; //runAway is checked in sheep and will trigger a scooting away
                            shp.RunDir = Vector3.Normalize(playerForward) * runAwaySpeed;
                            shp.RunDir += new Vector3(0, -10, 0);
                        }




                        worldMesh.checkCollisionAndResponse(shp);
                        //shp.checkPlayerCollision(shepPlayer);   //this remotely calls the collision class in sheep.  Hopefully shepherd can no longer walk though sheep
                    }
                    #endregion

                    //shepPlayer.Position = playerPos;
                worldMesh.checkCollisionAndResponse(shepPlayer);
                shepPlayer.Update(gameTime);
                foreach (Tree tre in treeList)
                {
                    shepPlayer.checkCollision(tre.Position, tre.Radius);
                }

                    //wolf stuff
                #region
                    foreach (Wolf wlf in wolfList)
                    {
                        //it the wolf doesnt have a target the next four lines will find a straggler
                        if (wlf.TgtAcquired == false)
                        {
                            foreach (Sheep shp in sheepList)
                                if (shp.IsStraggle == true && shp.NotFound == true)
                                {
                                    wlf.TgtName = shp.Name;
                                    wlf.TgtAcquired = true;
                                    shp.NotFound = false;
                                }
                        }
                        else
                            wlf.TgtPosition = FindTarget(wlf.TgtName);//the wolf does have a target then FindTarget will pass the position to the Wolf Class


                        foreach (Sheep shp in sheepList)//if a sheep is within 5 of a wolf they should be removed
                            if (Vector3.Distance(wlf.Position, shp.Position) < 5)
                            {
                                deathBleat.Play();
                                deadSheepList.Add(shp.Position);
                                sheepList.Remove(shp);
                                wlf.TgtAcquired = false;
                                Console.Out.WriteLine("Sheep remaining" + sheepList.Count);
                                break;
                            }

                        worldMesh.checkCollisionAndResponse(wlf);
                        wlf.Update(gameTime);

                        if (Vector3.Distance(wlf.Position, shepPlayer.Position) < 50) //arm up animation workaround
                        {
                            boutTaHit = true;
                            armUpCnt++;
                        }
                        if (Vector3.Distance(wlf.Position, shepPlayer.Position) < 25) //to scare off wolves
                        {
                            // float runAwaySpeed = 120 - (Vector3.Distance(wlf.Position, shepPlayer.Position) * 2);
                            wlf.RunAway = !wlf.RunAway; //runAway is checked in wolf and will trigger a scooting away
                            wlf.TgtAcquired = false;


                        /*
                        wolfDir = (tgtPosition - position);// *.2f;
                        vel = Vector3.Normalize(wolfDir) * 200f;
                        vel += (gravity * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        */
                        wlf.RunDir = Vector3.Normalize((wlf.Position - wlf.TgtPosition)) * 200; 
                       // wlf.RunDir = Vector3.Normalize(playerForward) * 200;
                            wlf.RunDir += new Vector3(0, -10, 0);

                            timeTaHit = true;//arm down animation workaround
                            armDwnCnt++;
                            hitLag = 1000;
                        }


                    }


                    #endregion

                    //animation workaround below
                    //if (armDwnCnt == 0)
                if (hitLag > 0)
                    if (armUpCnt == 0)
                    {
                       boutTaHit = false;
                       timeTaHit = false;
                    }

                armDwnCnt = 0;
                armUpCnt = 0;
            }
            //----------------------------------------------------game state end-----------------------------------------------------
            else if (gameState == "title")
            {
            
                KeyboardState kb = Keyboard.GetState();
                if (kb.IsKeyDown(Keys.Enter))
                {
                    if (titleTimer < 0)//this timer is to avoid the lightning fast checks that makes pausing very flickery
                    {
                        gameState = "game";
                    }
                    titleTimer = 50;

                }
            }
            else if (gameState == "endtitle")
            {
            KeyboardState kb = Keyboard.GetState();
            if (kb.IsKeyDown(Keys.Enter))
                {
                    clearGameData();
                    resetGame();
                }

             }
            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            if (gameState == "game")
            {
                //gamestate draw
                #region
                GraphicsDevice.Clear(Color.CornflowerBlue);            //GraphicsDevice.Clear(new Color(150, 150, 150));
                graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

                GraphicsDevice.BlendState = BlendState.AlphaBlend;

                Matrix world = Matrix.CreateTranslation(1, 0, 1);
                world = Matrix.Identity;
                world.Backward = playerForward;
                world.Up = Vector3.Up;
                world.Left = Vector3.Cross(playerForward, Vector3.Up);

                Matrix view = Matrix.CreateLookAt(shepPlayer.Position + Vector3.Transform(cameraOffset, world),

                                                    shepPlayer.Position, //what you look at
                                                    Vector3.Up);
                /*
                 *             proj = Matrix.CreatePerspectiveFieldOfView
                 *             (MathHelper.PiOver4,
                 *              GraphicsDevice.Viewport.Width / GraphicsDevice.Viewport.Height,
                 *              0.01f,
                 *              20000.0f);
                Matrix world;
                */
                Matrix proj = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(60), 1, 1f, 15000f);
                graphics.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                //------------------------------game camaera is set up------------------------

                world = Matrix.Identity;
                world.Backward = playerForward;
                world.Up = Vector3.Up;  //assuming up is always (0,1,0)
                world.Left = Vector3.Cross(playerForward, Vector3.Up);
                world *= Matrix.CreateScale(0.1f);

                world *= Matrix.CreateScale(20000f);
                world.Translation = playerPos;// Vector3.Zero;

                Sky.Draw(world, view, proj);

                graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                world = Matrix.CreateScale(8f) * Matrix.CreateTranslation(0, -5, 0);

                foreach (ModelMesh mesh in ground.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = world;
                        effect.View = view;
                        effect.Projection = proj;
                        effect.EnableDefaultLighting();
                    }
                    mesh.Draw();

                    foreach (Tree tre in treeList)
                    {

                        world = Matrix.Identity;
                        world.Up = Vector3.Up;  //assuming up is always (0,1,0)
                        world *= Matrix.CreateScale(8f) * Matrix.CreateTranslation(tre.Position);
                        tree.Draw(world, view, proj);
                    }

                }
                // ground.Draw(world, view, proj);
                //--------------------------------------- ground and sky is drawn--------------------------------

                world = Matrix.Identity;
                world.Up = Vector3.Up;  //assuming up is always (0,1,0)
                world *= Matrix.CreateScale(8f) * Matrix.CreateTranslation(0, -5, 0);
                fence.Draw(world, view, proj);

                world = Matrix.Identity;
                world.Up = Vector3.Up;  //assuming up is always (0,1,0)
                world *= Matrix.CreateScale(8f) * Matrix.CreateTranslation(0, -5, 0);
                river.Draw(world, view, proj);

                world = Matrix.Identity;
                world.Up = Vector3.Up;  //assuming up is always (0,1,0)
                world *= Matrix.CreateScale(.05f) * Matrix.CreateFromQuaternion(orient)//* Matrix.CreateRotationY(playerForward.X)// * Matrix.CreateRotationX(1.55f)
                    * Matrix.CreateTranslation(shepPlayer.Position);

                if (boutTaHit == true)
                    shepArmUp.Draw(world, view, proj);
                else if (timeTaHit == true)
                    shepArmDown.Draw(world, view, proj);
                else
                    shepherd.Draw(world, view, proj);

                if (deadSheepList != null)//drawing dead sheep
                    foreach (Vector3 pos in deadSheepList)
                    {
                        world = Matrix.Identity;
                        // world.Forward = pos; 
                        // world.Right = Vector3.Cross(world.Forward, Vector3.Up);
                        world.Up = Vector3.Up;  //assuming up is always (0,1,0)
                        world *= Matrix.CreateScale(.1f) * Matrix.CreateTranslation(pos);
                        deadSheep.Draw(world, view, proj);
                    }

                foreach (Sheep shp in sheepList)
                {

                    world = Matrix.Identity;
                    world.Forward = Vector3.Normalize(shp.Vel);
                    world.Right = Vector3.Cross(world.Forward, Vector3.Up);
                    world.Up = Vector3.Up;  //assuming up is always (0,1,0)
                    world *= Matrix.CreateScale(.1f) //* Matrix.CreateRotationX(1.55f)
                                                     //* Matrix.CreateFromQuaternion(shp.ShpOrientX)
                                                     //                  * Matrix.CreateFromQuaternion(shp.ShpOrientZ)
                                                     //* Matrix.CreateFromQuaternion(shp.ShpOrientZ)
                        * Matrix.CreateRotationY(MathHelper.PiOver2)
                        * Matrix.CreateTranslation(shp.Position);
                    sheep.Draw(world, view, proj);
                }

                foreach (Wolf wlf in wolfList)
                {
                    world = Matrix.Identity;
                    world.Forward = Vector3.Normalize(wlf.TgtPosition - wlf.Position);
                    world.Right = Vector3.Cross(world.Forward, Vector3.Up);
                    world.Up = Vector3.Up;  //assuming up is always (0,1,0)
                    world *= Matrix.CreateScale(.1f)
                        * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(wlf.Position);
                    wolf.Draw(world, view, proj);
                }

                spriteBatch.Begin();
                spriteBatch.DrawString(ariel, "Sheep remaining: " + sheepList.Count, new Vector2(0, 0), Color.White);
                spriteBatch.End();

                #endregion
            }
            else if (gameState == "tutorial_A")
            {
                //titlestate draw
                #region
                GraphicsDevice.Clear(Color.CornflowerBlue);            //GraphicsDevice.Clear(new Color(150, 150, 150));
                graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

                GraphicsDevice.BlendState = BlendState.AlphaBlend;

                Matrix world = Matrix.CreateTranslation(1, 0, 1);
                world = Matrix.Identity;
                world.Backward = new Vector3(1, 0, 0);
                world.Up = Vector3.Up;
                world.Left = Vector3.Cross(new Vector3(1, 0, 0), Vector3.Up);

                Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 0) + Vector3.Transform(new Vector3(0, 0, -215), world),

                                                    new Vector3(1, 0, 0), //what you look at
                                                    Vector3.Up);
                /*
                 *             proj = Matrix.CreatePerspectiveFieldOfView
                 *             (MathHelper.PiOver4,
                 *              GraphicsDevice.Viewport.Width / GraphicsDevice.Viewport.Height,
                 *              0.01f,
                 *              20000.0f);
                Matrix world;
                */
                Matrix proj = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(60), 1, 1f, 15000f);
                graphics.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                //------------------------------game camaera is set up------------------------

                world = Matrix.Identity;
                world.Up = Vector3.Up;  //assuming up is always (0,1,0)
                world *= Matrix.CreateScale(.25f)
                    //  * Matrix.CreateRotationX(MathHelper.ToRadians(90))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(270))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(90))
                    * Matrix.CreateTranslation(0, 0, 0);
                tut_A.Draw(world, view, proj);
                #endregion
            }
            else if (gameState == "tutorial_B")
            {
                //titlestate draw
                #region
                GraphicsDevice.Clear(Color.CornflowerBlue);            //GraphicsDevice.Clear(new Color(150, 150, 150));
                graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

                GraphicsDevice.BlendState = BlendState.AlphaBlend;

                Matrix world = Matrix.CreateTranslation(1, 0, 1);
                world = Matrix.Identity;
                world.Backward = new Vector3(1, 0, 0);
                world.Up = Vector3.Up;
                world.Left = Vector3.Cross(new Vector3(1, 0, 0), Vector3.Up);

                Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 0) + Vector3.Transform(new Vector3(0, 0, -215), world),

                                                    new Vector3(1, 0, 0), //what you look at
                                                    Vector3.Up);
                /*
                 *             proj = Matrix.CreatePerspectiveFieldOfView
                 *             (MathHelper.PiOver4,
                 *              GraphicsDevice.Viewport.Width / GraphicsDevice.Viewport.Height,
                 *              0.01f,
                 *              20000.0f);
                Matrix world;
                */
                Matrix proj = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(60), 1, 1f, 15000f);
                graphics.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                //------------------------------game camaera is set up------------------------

                world = Matrix.Identity;
                world.Up = Vector3.Up;  //assuming up is always (0,1,0)
                world *= Matrix.CreateScale(.25f)
                    //  * Matrix.CreateRotationX(MathHelper.ToRadians(90))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(270))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(90))
                    * Matrix.CreateTranslation(0, 0, 0);
                tut_B.Draw(world, view, proj);
                #endregion
            }
            else if (gameState == "tutorial_C")
            {
                //titlestate draw
                #region
                GraphicsDevice.Clear(Color.CornflowerBlue);            //GraphicsDevice.Clear(new Color(150, 150, 150));
                graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

                GraphicsDevice.BlendState = BlendState.AlphaBlend;

                Matrix world = Matrix.CreateTranslation(1, 0, 1);
                world = Matrix.Identity;
                world.Backward = new Vector3(1, 0, 0);
                world.Up = Vector3.Up;
                world.Left = Vector3.Cross(new Vector3(1, 0, 0), Vector3.Up);

                Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 0) + Vector3.Transform(new Vector3(0, 0, -215), world),

                                                    new Vector3(1, 0, 0), //what you look at
                                                    Vector3.Up);
                /*
                 *             proj = Matrix.CreatePerspectiveFieldOfView
                 *             (MathHelper.PiOver4,
                 *              GraphicsDevice.Viewport.Width / GraphicsDevice.Viewport.Height,
                 *              0.01f,
                 *              20000.0f);
                Matrix world;
                */
                Matrix proj = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(60), 1, 1f, 15000f);
                graphics.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                //------------------------------game camaera is set up------------------------

                world = Matrix.Identity;
                world.Up = Vector3.Up;  //assuming up is always (0,1,0)
                world *= Matrix.CreateScale(.25f)
                    //  * Matrix.CreateRotationX(MathHelper.ToRadians(90))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(270))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(90))
                    * Matrix.CreateTranslation(0, 0, 0);
                tut_C.Draw(world, view, proj);
                #endregion
            }
            else if (gameState == "tutorial_D")
            {
                //titlestate draw
                #region
                GraphicsDevice.Clear(Color.CornflowerBlue);            //GraphicsDevice.Clear(new Color(150, 150, 150));
                graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

                GraphicsDevice.BlendState = BlendState.AlphaBlend;

                Matrix world = Matrix.CreateTranslation(1, 0, 1);
                world = Matrix.Identity;
                world.Backward = new Vector3(1, 0, 0);
                world.Up = Vector3.Up;
                world.Left = Vector3.Cross(new Vector3(1, 0, 0), Vector3.Up);

                Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 0) + Vector3.Transform(new Vector3(0, 0, -215), world),

                                                    new Vector3(1, 0, 0), //what you look at
                                                    Vector3.Up);
                /*
                 *             proj = Matrix.CreatePerspectiveFieldOfView
                 *             (MathHelper.PiOver4,
                 *              GraphicsDevice.Viewport.Width / GraphicsDevice.Viewport.Height,
                 *              0.01f,
                 *              20000.0f);
                Matrix world;
                */
                Matrix proj = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(60), 1, 1f, 15000f);
                graphics.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                //------------------------------game camaera is set up------------------------

                world = Matrix.Identity;
                world.Up = Vector3.Up;  //assuming up is always (0,1,0)
                world *= Matrix.CreateScale(.25f)
                    //  * Matrix.CreateRotationX(MathHelper.ToRadians(90))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(270))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(90))
                    * Matrix.CreateTranslation(0, 0, 0);
                tut_D.Draw(world, view, proj);
                #endregion
            }
            else if (gameState == "tutorial_E")
            {
                //titlestate draw
                #region
                GraphicsDevice.Clear(Color.CornflowerBlue);            //GraphicsDevice.Clear(new Color(150, 150, 150));
                graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

                GraphicsDevice.BlendState = BlendState.AlphaBlend;

                Matrix world = Matrix.CreateTranslation(1, 0, 1);
                world = Matrix.Identity;
                world.Backward = new Vector3(1, 0, 0);
                world.Up = Vector3.Up;
                world.Left = Vector3.Cross(new Vector3(1, 0, 0), Vector3.Up);

                Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 0) + Vector3.Transform(new Vector3(0, 0, -215), world),

                                                    new Vector3(1, 0, 0), //what you look at
                                                    Vector3.Up);
                /*
                 *             proj = Matrix.CreatePerspectiveFieldOfView
                 *             (MathHelper.PiOver4,
                 *              GraphicsDevice.Viewport.Width / GraphicsDevice.Viewport.Height,
                 *              0.01f,
                 *              20000.0f);
                Matrix world;
                */
                Matrix proj = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(60), 1, 1f, 15000f);
                graphics.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                //------------------------------game camaera is set up------------------------

                world = Matrix.Identity;
                world.Up = Vector3.Up;  //assuming up is always (0,1,0)
                world *= Matrix.CreateScale(.25f)
                    //  * Matrix.CreateRotationX(MathHelper.ToRadians(90))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(270))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(90))
                    * Matrix.CreateTranslation(0, 0, 0);
                tut_E.Draw(world, view, proj);
                #endregion
            }
            else if (gameState == "tutorial_F")
            {
                //titlestate draw
                #region
                GraphicsDevice.Clear(Color.CornflowerBlue);            //GraphicsDevice.Clear(new Color(150, 150, 150));
                graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

                GraphicsDevice.BlendState = BlendState.AlphaBlend;

                Matrix world = Matrix.CreateTranslation(1, 0, 1);
                world = Matrix.Identity;
                world.Backward = new Vector3(1, 0, 0);
                world.Up = Vector3.Up;
                world.Left = Vector3.Cross(new Vector3(1, 0, 0), Vector3.Up);

                Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 0) + Vector3.Transform(new Vector3(0, 0, -215), world),

                                                    new Vector3(1, 0, 0), //what you look at
                                                    Vector3.Up);
                /*
                 *             proj = Matrix.CreatePerspectiveFieldOfView
                 *             (MathHelper.PiOver4,
                 *              GraphicsDevice.Viewport.Width / GraphicsDevice.Viewport.Height,
                 *              0.01f,
                 *              20000.0f);
                Matrix world;
                */
                Matrix proj = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(60), 1, 1f, 15000f);
                graphics.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                //------------------------------game camaera is set up------------------------

                world = Matrix.Identity;
                world.Up = Vector3.Up;  //assuming up is always (0,1,0)
                world *= Matrix.CreateScale(.25f)
                    //  * Matrix.CreateRotationX(MathHelper.ToRadians(90))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(270))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(90))
                    * Matrix.CreateTranslation(0, 0, 0);
                tut_F.Draw(world, view, proj);
                #endregion
            }
            else if (gameState == "tutorial_G")
            {
                //titlestate draw
                #region
                GraphicsDevice.Clear(Color.CornflowerBlue);            //GraphicsDevice.Clear(new Color(150, 150, 150));
                graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

                GraphicsDevice.BlendState = BlendState.AlphaBlend;

                Matrix world = Matrix.CreateTranslation(1, 0, 1);
                world = Matrix.Identity;
                world.Backward = new Vector3(1, 0, 0);
                world.Up = Vector3.Up;
                world.Left = Vector3.Cross(new Vector3(1, 0, 0), Vector3.Up);

                Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 0) + Vector3.Transform(new Vector3(0, 0, -215), world),

                                                    new Vector3(1, 0, 0), //what you look at
                                                    Vector3.Up);
                /*
                 *             proj = Matrix.CreatePerspectiveFieldOfView
                 *             (MathHelper.PiOver4,
                 *              GraphicsDevice.Viewport.Width / GraphicsDevice.Viewport.Height,
                 *              0.01f,
                 *              20000.0f);
                Matrix world;
                */
                Matrix proj = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(60), 1, 1f, 15000f);
                graphics.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                //------------------------------game camaera is set up------------------------

                world = Matrix.Identity;
                world.Up = Vector3.Up;  //assuming up is always (0,1,0)
                world *= Matrix.CreateScale(.25f)
                    //  * Matrix.CreateRotationX(MathHelper.ToRadians(90))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(270))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(90))
                    * Matrix.CreateTranslation(0, 0, 0);
                tut_G.Draw(world, view, proj);
                #endregion
            }
            else if (gameState == "tutorial_H")
            {
                //titlestate draw
                #region
                GraphicsDevice.Clear(Color.CornflowerBlue);            //GraphicsDevice.Clear(new Color(150, 150, 150));
                graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

                GraphicsDevice.BlendState = BlendState.AlphaBlend;

                Matrix world = Matrix.CreateTranslation(1, 0, 1);
                world = Matrix.Identity;
                world.Backward = new Vector3(1, 0, 0);
                world.Up = Vector3.Up;
                world.Left = Vector3.Cross(new Vector3(1, 0, 0), Vector3.Up);

                Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 0) + Vector3.Transform(new Vector3(0, 0, -215), world),

                                                    new Vector3(1, 0, 0), //what you look at
                                                    Vector3.Up);
                /*
                 *             proj = Matrix.CreatePerspectiveFieldOfView
                 *             (MathHelper.PiOver4,
                 *              GraphicsDevice.Viewport.Width / GraphicsDevice.Viewport.Height,
                 *              0.01f,
                 *              20000.0f);
                Matrix world;
                */
                Matrix proj = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(60), 1, 1f, 15000f);
                graphics.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                //------------------------------game camaera is set up------------------------

                world = Matrix.Identity;
                world.Up = Vector3.Up;  //assuming up is always (0,1,0)
                world *= Matrix.CreateScale(.25f)
                    //  * Matrix.CreateRotationX(MathHelper.ToRadians(90))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(270))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(90))
                    * Matrix.CreateTranslation(0, 0, 0);
                tut_H.Draw(world, view, proj);
                #endregion
            }
            else if (gameState == "tutorial_I")
            {
                //titlestate draw
                #region
                GraphicsDevice.Clear(Color.CornflowerBlue);            //GraphicsDevice.Clear(new Color(150, 150, 150));
                graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

                GraphicsDevice.BlendState = BlendState.AlphaBlend;

                Matrix world = Matrix.CreateTranslation(1, 0, 1);
                world = Matrix.Identity;
                world.Backward = new Vector3(1, 0, 0);
                world.Up = Vector3.Up;
                world.Left = Vector3.Cross(new Vector3(1, 0, 0), Vector3.Up);

                Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 0) + Vector3.Transform(new Vector3(0, 0, -215), world),

                                                    new Vector3(1, 0, 0), //what you look at
                                                    Vector3.Up);
                /*
                 *             proj = Matrix.CreatePerspectiveFieldOfView
                 *             (MathHelper.PiOver4,
                 *              GraphicsDevice.Viewport.Width / GraphicsDevice.Viewport.Height,
                 *              0.01f,
                 *              20000.0f);
                Matrix world;
                */
                Matrix proj = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(60), 1, 1f, 15000f);
                graphics.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                //------------------------------game camaera is set up------------------------

                world = Matrix.Identity;
                world.Up = Vector3.Up;  //assuming up is always (0,1,0)
                world *= Matrix.CreateScale(.25f)
                    //  * Matrix.CreateRotationX(MathHelper.ToRadians(90))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(270))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(90))
                    * Matrix.CreateTranslation(0, 0, 0);
                tut_i.Draw(world, view, proj);
                #endregion
            }





            else if (gameState == "title")
            {
                //titlestate draw
                #region
                GraphicsDevice.Clear(Color.CornflowerBlue);            //GraphicsDevice.Clear(new Color(150, 150, 150));
                graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

                GraphicsDevice.BlendState = BlendState.AlphaBlend;

                Matrix world = Matrix.CreateTranslation(1, 0, 1);
                world = Matrix.Identity;
                world.Backward = new Vector3(1, 0, 0);
                world.Up = Vector3.Up;
                world.Left = Vector3.Cross(new Vector3(1, 0, 0), Vector3.Up);

                Matrix view = Matrix.CreateLookAt(new Vector3(0,0,0)+ Vector3.Transform(new Vector3(0, 0, -215), world),

                                                    new Vector3(1, 0, 0), //what you look at
                                                    Vector3.Up);
                /*
                 *             proj = Matrix.CreatePerspectiveFieldOfView
                 *             (MathHelper.PiOver4,
                 *              GraphicsDevice.Viewport.Width / GraphicsDevice.Viewport.Height,
                 *              0.01f,
                 *              20000.0f);
                Matrix world;
                */
                Matrix proj = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(60), 1, 1f, 15000f);
                graphics.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                //------------------------------game camaera is set up------------------------

                world = Matrix.Identity;
                world.Up = Vector3.Up;  //assuming up is always (0,1,0)
                world *= Matrix.CreateScale(.25f)
                    //  * Matrix.CreateRotationX(MathHelper.ToRadians(90))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(270))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(90))
                    * Matrix.CreateTranslation(0, 0, 0);
                pause.Draw(world, view, proj);
                #endregion
            }
            else if (gameState == "endtitle")
            {
                //end titlestate draw
                #region
                GraphicsDevice.Clear(Color.CornflowerBlue);            //GraphicsDevice.Clear(new Color(150, 150, 150));
                graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

                GraphicsDevice.BlendState = BlendState.AlphaBlend;

                Matrix world = Matrix.CreateTranslation(1, 0, 1);
                world = Matrix.Identity;
                world.Backward = new Vector3(1, 0, 0);
                world.Up = Vector3.Up;
                world.Left = Vector3.Cross(new Vector3(1, 0, 0), Vector3.Up);

                Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 0) + Vector3.Transform(new Vector3(0, 0, -215), world),

                                                    new Vector3(1, 0, 0), //what you look at
                                                    Vector3.Up);
                /*
                 *             proj = Matrix.CreatePerspectiveFieldOfView
                 *             (MathHelper.PiOver4,
                 *              GraphicsDevice.Viewport.Width / GraphicsDevice.Viewport.Height,
                 *              0.01f,
                 *              20000.0f);
                Matrix world;
                */
                Matrix proj = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(60), 1, 1f, 15000f);
                graphics.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                //------------------------------game camaera is set up------------------------

                world = Matrix.Identity;
                world.Up = Vector3.Up;  //assuming up is always (0,1,0)
                world *= Matrix.CreateScale(.25f)
                    //  * Matrix.CreateRotationX(MathHelper.ToRadians(90))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(270))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(90))
                    * Matrix.CreateTranslation(0, 0, 0);
                title.Draw(world, view, proj);


                spriteBatch.Begin();
                spriteBatch.DrawString(ariel, "You saved " + score+" sheep", new Vector2((GraphicsDevice.Viewport.Y)/2, (GraphicsDevice.Viewport.X) / 2), Color.White);
                spriteBatch.End();
                #endregion
            }

            base.Draw(gameTime);
        }
    }
}
