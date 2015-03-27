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
        private Wall backWall;
        private Wall leftWall;
        private Wall rightWall;
        private MessageDisplay gameOver;
        private MessageDisplay win;
        
        //Palette controlée par le joueur
        private Paddle paddle;

        //Balle du jeu
        private Ball ball;

       
        //Obersvable qui prend le gametime et le dispatch à tous les timer custom qui sont en écoute
        private ObservableTimer observableTimer = new ObservableTimer();

        //Timer qui empèche la détection de collision entre la balle et le paddle. Ceci est utilisé pour empècher que la balle entre en collision 2
        //fois avec le paddle très rapidement. Cela arrive notamment quand la balle frappe le coté du paddle et que le joueur bouge le paddle en même temps.
        private CustomTimer paddleStopHitDetectionTimer = new CustomTimer();

        //Timers qui empèche la détection de collision entre la balle et les murs gauche et droit. Ceci est utilisé pour empècher que la balle entre reste collé
        //sur le mur. Cela est dû à une détection de collision trop rapide.
        private CustomTimer leftWallStopHitDetectionTimer = new CustomTimer();
        private CustomTimer rightWallStopHitDetectionTimer = new CustomTimer();
        private CustomTimer bricksAnimationTimer = new CustomTimer(); 

        //Nombre de vie au début
        private static int startnumberOfLives = 3;
        //Nombre de vies du joueur
        private int numberOfLives = startnumberOfLives;

        //Numéro du level
        int currentLevelNumber = 1;
        enum GameState
        {
            MainMenu,
            Playing,
            GameOver,
            Win
        }

        GameState currentGameState = GameState.MainMenu;
        button btnLevel1;
        button btnLevel2;
        button btnExit;
        button LevelDisplay;

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

            //Initialise le GameOver image
            gameOver = new MessageDisplay(Content, "GameObjects/gameOver");
            win = new MessageDisplay(Content, "GameObjects/Winner");
            btnLevel1 = new button(Content, "GameObjects/Level1", graphics.GraphicsDevice);
            btnLevel1.setPosition(new Vector2(200, 150));

            btnLevel2 = new button(Content, "GameObjects/Level2", graphics.GraphicsDevice);
            btnLevel2.setPosition(new Vector2(200, 250));

            btnExit = new button(Content, "GameObjects/Exit", graphics.GraphicsDevice);
            btnExit.setPosition(new Vector2(200, 350));

            LevelDisplay = new button(Content, "GameObjects/PaddleTexture", graphics.GraphicsDevice);
            LevelDisplay.setPosition(new Vector2(10, 0));

            //Rendre visilbe la souris
            IsMouseVisible = true;

            //Mets les briques aux bonnes position dans le contrôleur de briques
            levelLayout.CreateLevel(currentLevelNumber);

            //Enregistre les timers dans l'observable
            observableTimer.RegisterObserver(paddleStopHitDetectionTimer);
            observableTimer.RegisterObserver(leftWallStopHitDetectionTimer);
            observableTimer.RegisterObserver(rightWallStopHitDetectionTimer);
            observableTimer.RegisterObserver(bricksAnimationTimer);
            
            
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


            //Si le bouton escape est enfoncé, on quitte le jeu
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                GameOver();

            //selon le status de la partie on execute le bon update
            switch (currentGameState)
            {
                case GameState.MainMenu:
                    MainMenuUpdate(gameTime);
                    break;
                case GameState.Playing:
                    PlayingUpdate(gameTime);
                    break;
                case GameState.GameOver:
                    GameOverUpdate(gameTime);
                    break;
                case GameState.Win:
                    WinUpdate(gameTime);
                    break;
            }
         }

        protected void MainMenuUpdate(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();

            //Action du bouton level1
            if (btnLevel1.isClicked == true)   
            {
                currentLevelNumber = 1;
                levelLayout.CreateLevel(1);
                currentGameState = GameState.Playing;                
            }

            //Action du bouton level2
            if (btnLevel2.isClicked == true)
            {
                currentLevelNumber = 2;
                levelLayout.CreateLevel(2);
                currentGameState = GameState.Playing;
            }

            //Action du bouton Exit on Sort du jeu
            if (btnExit.isClicked == true)
            {
                Exit();
            }

            //Update l'affichage du bouton selon la souris
            btnLevel1.Update(mouse);
            btnLevel2.Update(mouse);
            btnExit.Update(mouse);

        }

        protected void PlayingUpdate(GameTime gameTime)
        {
            //Notifie les timers customer que le gamtime a changé
            observableTimer.NotifyObservers(gameTime);

            currentKeys = Keyboard.GetState();

            //Vérifie si l'animation du début de jeu est complétée
            if (!brickController.IsAnimationCompleted)
            {
                //if (bricksAnimationTimer.IsTimerStarted == false || bricksAnimationTimer.isTimeUp(50))
                //{
                //bricksAnimationTimer.Start();
                brickController.Animate();
                //}
            }

            else
            {
                //Vérifie si le joueur à cassé toute les briques du niveau
                if (brickController.BrickCount == 0)
                {
                    //S'il ne reste pas de niveau à chargé (le joueur à gagné)
                    if (levelLayout.NumberOfLevels == currentLevelNumber)
                    {
                        PlayerHasWon();
                    }

                    //S'il reste des niveau, on charge le niveau suivant
                    else
                    {
                        levelLayout.CreateLevel(++currentLevelNumber);
                        brickController.IsAnimationCompleted = false;
                        ResetBoard();
                    }
                }

                    //Les briques ne sont pas toutes cassé, le jeu se poursuit
                else
                {
                    //Vérifie si la balle est sortie de la zone de jeu
                    if (ball.Position.Z > 30)
                    {
                        if (--numberOfLives == 0)    //Enlève une vie au joueur et vérifie s'il est gameover
                        {
                            GameOver();
                        }

                        ResetBoard();       //Remet la balle et le paddle à leur position respective
                    }

                    //Mouvement de la balle
                    ball.Move();

                    //Vérifie les collisions
                    CheckForCollisions();
                }
            }

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
            base.Update(gameTime);

        }

        protected void GameOverUpdate(GameTime gameTime)
        {
            //si le temps est fini on revient au menu;
            if (!gameOver.isShowing() & currentGameState == GameState.GameOver)
            {
                btnLevel1.ResetClick();
                btnLevel2.ResetClick();
                btnExit.ResetClick();
                currentGameState = GameState.MainMenu;
                return;
            }
        }

        protected void WinUpdate(GameTime gameTime)
        {
            //si le temps est fini on revient au menu;
            if (!win.isShowing() & currentGameState == GameState.Win)
            {
                btnLevel1.ResetClick();
                btnLevel2.ResetClick();
                btnExit.ResetClick();
                currentGameState = GameState.MainMenu;
                return;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //affiche la bonne écran selon le statut.
            switch(currentGameState)
            {
                case GameState.MainMenu:
                    MainMenuDraw(gameTime);
                    break;    
                case GameState.Playing:
                    PlayingDraw(gameTime);
                    break;
                case GameState.GameOver:
                    GameOverDraw(gameTime);
                    break;
                case GameState.Win:
                    WinDraw(gameTime);
                    break;
            }      

            base.Draw(gameTime);
        }

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
           

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;

                    effect.LightingEnabled = true; // turn on the lighting subsystem.
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0.2f, 0.2f, 0.2f); 
                    effect.DirectionalLight0.Direction = new Vector3(-1, 0.25f, -1);  // coming along the x-axis
                    effect.DirectionalLight0.SpecularColor = new Vector3(0.1f, 0.1f, 0.1f); // with green highlights

                   effect.AmbientLightColor = new Vector3(0.4f, 0.4f, 0.4f);
                   effect.EmissiveColor = new Vector3(0.2f, 0.2f, 0.2f);

                   effect.World = mesh.ParentBone.Transform *  world;
                   effect.View = view;
                   effect.Projection = projection;

  
                }

                mesh.Draw();
            }

            
        }

        protected void GameOverDraw(GameTime gametime)
        {
            //affiche l'image de gameOver
            gameOver.draw(spriteBatch);
        }

        protected void WinDraw(GameTime gametime)
        {
            //affiche l'image de Win
            win.draw(spriteBatch);
        }

        protected void MainMenuDraw(GameTime gametime)
        {
            //affiche le menu
            spriteBatch.Begin();
            spriteBatch.Draw(Content.Load<Texture2D>("GameObjects/PaddleTexture"), new Rectangle(0, 0, 900, 700), Color.White);

            //affiche les boutons
            btnLevel1.draw(spriteBatch);
            btnLevel2.draw(spriteBatch);          
            btnExit.draw(spriteBatch);
            
            spriteBatch.End();
        }

        protected void PlayingDraw(GameTime gametime)
        {

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

            LevelDisplay.drawString(spriteBatch, "Nombre de vie: " + numberOfLives.ToString() + "/" + startnumberOfLives.ToString());
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

        //Méthode qui permet de remettre le jeu au départ. Notez-bien, cette méthode remet uniquement la balle
        //et le paddle au position de base. Les briques sont laissées telle-quelle. On utilise cette méthode 
        //pour reprendre le jeu après la perte d'une vie.
        private void ResetBoard()
        {
            System.Threading.Thread.Sleep(400);  
            ball.Reset();
            paddle.Reset();
        }

        private void PlayerHasWon()
        {
            win.show();
            ResetBoard();
            brickController.IsAnimationCompleted = false;
            currentGameState = GameState.Win;
            numberOfLives = startnumberOfLives;
        }
        
        //Methode appelée quand le joueur a perdu
        private void GameOver()
        {          
            gameOver.show();
            ResetBoard();
            brickController.IsAnimationCompleted = false;
            currentGameState = GameState.GameOver;
            numberOfLives = startnumberOfLives;
        }
    }
}
