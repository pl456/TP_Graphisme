using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arkanoid.Entities
{
    public class BrickController
    {
        

        //Dictionnaire de briques (position, brique)
        private List<Brick> bricks;

        //Content manager de XNA
        private ContentManager contentManager;


        /*
         * Variables pour animation
         */ 

        //Y de départ des briques avant l'animation qui les ramènent au sol
        private const int Y_OFFSET_FOR_ANIMATION = 20;
        private const float Y_VELOCITY_FOR_ANIMATION = 0.5f;
        //private List<Brick> currentBricksInAnimation = null;
        //private int currentBrickRowNumberInAnimation = 0;
        //private int currentBrickIndice = 0;
        //Dit si l'animation des briques est terminée ou non
        private Boolean isAnimationCompleted = false;

        

        public BrickController(ContentManager contentManager)
        {
            this.contentManager = contentManager;
            bricks = new List<Brick>();
        }

        public void AddBrick(int positionOnGridX, int positionOnGridY, BrickColor brickColor)
        {
            Vector3 realBrickPosition = new Vector3(positionOnGridX * 2 - 12, Y_OFFSET_FOR_ANIMATION, positionOnGridY);
            switch(brickColor)
            {
                case BrickColor.Blue :

                    bricks.Add(new BlueBrick(positionOnGridX, positionOnGridY, realBrickPosition, brickColor, contentManager.Load<Model>("GameObjects/BlueBrick")));
                    break;

                case BrickColor.Red :
                    bricks.Add(new BlueBrick(positionOnGridX, positionOnGridY, realBrickPosition, brickColor, contentManager.Load<Model>("GameObjects/RedBrick")));
                    break;
            }
        }

        public void ClearList()
        {
            bricks.Clear();
        }

        public Brick GetBrick(int index)
        {
            return bricks[index];
        }

        public void DeleteBrick(Brick brick)
        {
            bricks.Remove(brick);
        }

        //Fait l'animation des briques
        public void Animate()
        {
            //Si l'animation n'est pas complétée
            if(!isAnimationCompleted)
            {
                //Descent les briques de la séquences
                foreach (Brick brick in bricks)
                    brick.TranslateInY(0.2f);

                
                //Si la liste des briques est terminé, on stoppe l'animation
                if (bricks[Bricks.Count - 1].Position.Y == 0)
                {
                    isAnimationCompleted = true;
                }
            }
        }
/*
        //Charge en mémoire une séquence de briques pour l'animation
        private void LoadBricksSequenceForAnimation()
        {

            //Vide la liste de briques
            currentBricksInAnimation.Clear();

            //Boucle la liste de brique à la recherche des briques à
            //animer
            for (; currentBrickIndice < bricks.Count; ++currentBrickIndice)
            {
                Brick brick = bricks[currentBrickIndice];
                if (currentBrickRowNumberInAnimation < brick.PositionOnGridY)
                {
                    currentBrickIndice--;
                    break;
                }

                currentBricksInAnimation.Add(brick);
            }

            currentBrickRowNumberInAnimation++;
        }
*/
        #region Getters & Setters

        public int BrickCount
        {
            get { return bricks.Count; }
        }

        public List<Brick> Bricks
        {
            get { return bricks; }
        }

        public Boolean IsAnimationCompleted
        {
            get { return isAnimationCompleted; }
            set { isAnimationCompleted = value; }
        }

        #endregion


    }
}
