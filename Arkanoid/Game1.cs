#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Arkanoid.Entities;
using Arkanoid.Utils;

#endregion

namespace Arkanoid
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {

        private const float paddleVelocity = 0.7f;
        GraphicsDeviceManager graphics;
        

        SpriteBatch spriteBatch;
 
        private BrickController brickController;    //Contient la liste des briques et leurs positions
        private LevelLayout levelLayout;            //Contient la disposition des briques dans le level. Il emplit le BrickController avec les bonnes briques au bon endroit

        //Input devices
        KeyboardState currentKeys;

        //private Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private Matrix view = Matrix.CreateLookAt(new Vector3(0, 30, 40), new Vector3(0, 0, 0), Vector3.UnitY);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 100f);

        //Murs qui définissent la zone de jeu
        Wall backWall;
        Wall leftWall;
        Wall rightWall;
        
        //Palette controlée par le joueur
        Paddle paddle;

        //Balle du jeu
        Ball ball;

       
        //Obersvable qui prend le gametime et le dispatch à tous les timer custom qui sont en écoute
        ObservableTimer observableTimer = new ObservableTimer();

        //Timer qui empèche la détection de collision entre la balle et le paddle. Ceci est utilisé pour empècher que la balle entre en collision 2
        //fois avec le paddle très rapidement. Cela arrive notamment quand la balle frappe le coté du paddle et que le joueur bouge le paddle en même temps.
        CustomTimer paddleStopHitDetectionTimer = new CustomTimer();

        //Timers qui empèche la détection de collision entre la balle et les murs gauche et droit. Ceci est utilisé pour empècher que la balle entre reste collé
        //sur le mur. Cela est dû à une détection de collision trop rapide.
        CustomTimer leftWallStopHitDetectionTimer = new CustomTimer();
        CustomTimer rightWallStopHitDetectionTimer = new CustomTimer(); 

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            
            /*
            System.Drawing.Rectangle resolution = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            graphics.PreferredBackBufferWidth = resolution.Width;
            graphics.PreferredBackBufferHeight = resolution.Height;
            graphics.ApplyChanges();
            */
            
            graphics.PreferredBackBufferWidth = 900;
            graphics.PreferredBackBufferHeight = 700;
            graphics.ApplyChanges();


            //Ajuste l'angle de caméra pour bien voir le jeu
            view *= Matrix.CreateRotationX(MathHelper.ToRadians(20));


            brickController = new BrickController(Content);
            levelLayout = new LevelLayout(brickController);

            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Initialise les murs
            backWall = new Wall(Content, WallPosition.Back);
            leftWall = new Wall(Content, WallPosition.Left);
            rightWall = new Wall(Content, WallPosition.Right);

            //Initialise le paddle
            paddle = new Paddle(Content);

            //Initialise la balle
            ball = new Ball(Content);


            //Mets les briques aux bonnes position dans le contrôleur de briques
            levelLayout.CreateLevel(1);

            //Enregistre les timers dans l'observable
            observableTimer.RegisterObserver(paddleStopHitDetectionTimer);
            observableTimer.RegisterObserver(leftWallStopHitDetectionTimer);
            observableTimer.RegisterObserver(rightWallStopHitDetectionTimer);
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            //Notifie les timers customer que le gamtime a changé
            observableTimer.NotifyObservers(gameTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            currentKeys = Keyboard.GetState();

            //Press Esc To Exit
            if (currentKeys.IsKeyDown(Keys.Escape))
                this.Exit();

            /*
            //Press Directional Keys to rotate cube
            if (currentKeys.IsKeyDown(Keys.Up))
                world *= Matrix.CreateRotationX(-0.05f);
            if (currentKeys.IsKeyDown(Keys.Down))
                world *= Matrix.CreateRotationX(0.05f);
            if (currentKeys.IsKeyDown(Keys.Left))
                world *= Matrix.CreateRotationY(-0.05f);
            if (currentKeys.IsKeyDown(Keys.Right))
                world *= Matrix.CreateRotationY(0.05f);
            */

            //Haut/bas + Gauche/Droite
            if (currentKeys.IsKeyDown(Keys.Up))
                view *= Matrix.CreateTranslation(0, -0.3f, 0) * Matrix.CreateRotationX(MathHelper.ToRadians(1));
            if (currentKeys.IsKeyDown(Keys.Down))
                view *= Matrix.CreateTranslation(0, 0.3f, 0) * Matrix.CreateRotationX(MathHelper.ToRadians(-1));
            if (currentKeys.IsKeyDown(Keys.Left))
                view *= Matrix.CreateTranslation(0.3f, 0, 0);
            if (currentKeys.IsKeyDown(Keys.Right))
                view *= Matrix.CreateTranslation(-0.3f, 0, 0);

            //Z
            if (currentKeys.IsKeyDown(Keys.S))
                view *= Matrix.CreateTranslation(0, 0, -0.6f);
            if (currentKeys.IsKeyDown(Keys.W))
                view *= Matrix.CreateTranslation(0, 0, 0.6f);


            //Mouvement du paddle
            if (currentKeys.IsKeyDown(Keys.A))
                if (paddle.Position.X > -10.5)
                    paddle.Move(-paddleVelocity);
            if (currentKeys.IsKeyDown(Keys.D))
                if (paddle.Position.X < 10.5)
                    paddle.Move(paddleVelocity);


            //TODO : supprimer (test de vélocité)
            if (currentKeys.IsKeyDown(Keys.NumPad1))
                ball.SetSlowVelocity();
            if (currentKeys.IsKeyDown(Keys.NumPad2))
                ball.SetMediumVelocity();
            if (currentKeys.IsKeyDown(Keys.NumPad3))
                ball.SetFastVelocity();

            //Mouvement de la balle
            ball.Move();


            //Vérifie les collisions
            CheckForCollisions();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
           

            GraphicsDevice.Clear(Color.Black);

            //Affiche les briques du controleurs
            foreach (Brick brick in brickController.Bricks)
            {
                DrawModel(brick.Model, brick.WorldMatrix, view, projection);
            }

            //Affiche les murs
            DrawModel(backWall.Model, backWall.WorldMatrix, view, projection);
            DrawModel(leftWall.Model, leftWall.WorldMatrix, view, projection);
            DrawModel(rightWall.Model, rightWall.WorldMatrix, view, projection);

            //Affiche le paddle 
            DrawModel(paddle.Model, paddle.WorldMatrix, view, projection);

            //Affiche la balle
            DrawModel(ball.Model, ball.WorldMatrix, view, projection);

            base.Draw(gameTime);
        }

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
           

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }

            
        }

        //Vérifie s'il y a des collisons
        private void CheckForCollisions()
        {
            
            /*
             * Vérifie la collision entre la balle et le mur gauche
             * 
             */
           
            if (CheckCollisionHitBox(ball, leftWall))
            {
                //Vérifie le temps de déactivation des collisions sur le mur gauche (voir variable de classe "leftWallStopHitDetectionTimer")
                if (leftWallStopHitDetectionTimer.IsTimerStarted == false || leftWallStopHitDetectionTimer.isTimeUp(500))
                    ball.ReverseVelocityInX();
            }

            /*
             * Vérifie la collision entre la balle et le mur du fond
             * 
             */

            if (CheckCollisionHitBox(ball, backWall))
            {
                ball.ReverseVelocityInZ();
            }

            /*
             * Vérifie la collision entre la balle et le mur de droite
             * 
             */

            if (CheckCollisionHitBox(ball, rightWall))
            {
                //Vérifie le temps de déactivation des collisions sur le mur gauche (voir variable de classe "rightWallStopHitDetectionTimer")
                if (rightWallStopHitDetectionTimer.IsTimerStarted == false || rightWallStopHitDetectionTimer.isTimeUp(500))
                    ball.ReverseVelocityInX();
            }

            /*
             * Vérifie la collision entre la balle et le paddle
             * 
             */

            if (CheckCollisionHitBox(ball, paddle))
            {
                //Vérifie le temps de déactivation des collisions sur le paddle (voir variable de classe "paddleStopHitDetectionTimer")
                if (paddleStopHitDetectionTimer.IsTimerStarted == false || paddleStopHitDetectionTimer.isTimeUp(500))
                {
                    paddleStopHitDetectionTimer.Start();
                    //Selon la position de frappe de la balle sur le paddle, on inverse ou non la direction est-ouest de la balle
                    if ((ball.DirectionInX < 1 && ball.Position.X > paddle.Position.X) || (ball.DirectionInX > -1 && ball.Position.X < paddle.Position.X))
                    {
                        //Renverse le sens de la direction de la balle en x
                        //Si la balle arrive de l'ouest et frappe la moitié ouest
                        //du paddle, la balle retournera à l'ouest.
                        ball.ReverseVelocityInX();

                        //Ajuste l'angle de direction de la balle en fonction de l'endroit où elle touche le paddle
                        ball.AjustAngle(paddle.Position.X);
                    }
                   
                    //Change la direction en z
                    //Si la balle va vers le nord et qu'elle frappe un objet,
                    //est ira vers le sud
                    ball.ReverseVelocityInZ();
                }
            }

            /*
             * Vérifie la collision entre la balle et une brique
             * 
             */

            //Boucle chacune des briques pour voir la collision
            foreach(Brick brick in brickController.Bricks)
            {
                if(CheckCollisionHitBox(ball, brick))
                {
                    brickController.DeleteBrick(brick);

                    //Si la balle frappe le coté (est-ouest) d'une brique
                    if ((int)Decimal.Ceiling((decimal)(ball.Position.Z - ball.Height)) != 0)
                    {
                        ball.ReverseVelocityInX();
                        ball.ReverseVelocityInZ();
                    }

                    //Si la balle frappe le haut ou le bas d'une brique (nord-sud)
                    else
                    {
                        //Change la direction en z
                        //Si la balle va vers le nord et qu'elle frappe un objet,
                        //est ira vers le sud
                        ball.ReverseVelocityInZ();
                    }
                    
                    break;
                }
            }


        }

        //Vérifie s'il y a collision entre 2 objets
        private Boolean CheckCollisionHitBox(IWorldObject worldObject1, IWorldObject worldObject2)
        {

            //Boite de collision pour l'objet1
            BoundingBox BoundingBoxObj1 = new BoundingBox(worldObject1.Position - new Vector3(worldObject1.Width / 2, worldObject1.Height / 2, worldObject1.Depth / 2),
                 worldObject1.Position + new Vector3(worldObject1.Width / 2, worldObject1.Height / 2, worldObject1.Depth / 2));

            //Boite de collision pour l'objet2
            BoundingBox BoundingBoxObj2 = new BoundingBox(worldObject2.Position - new Vector3(worldObject2.Width / 2, worldObject2.Height / 2, worldObject2.Depth / 2), 
                worldObject2.Position + new Vector3(worldObject2.Width / 2, worldObject2.Height / 2, worldObject2.Depth / 2));

            //Vérifie si les objets entre en collision
            if (BoundingBoxObj1.Intersects(BoundingBoxObj2))
                return true;
                
            return false;
        }
    }
}
