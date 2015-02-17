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
        private ContentManager contentManager;


        public BrickController(ContentManager contentManager)
        {
            this.contentManager = contentManager;
            bricks = new List<Brick>();
        }

        public void AddBrick(int positionOnGridX, int positionOnGridY, BrickColor brickColor)
        {
            Vector3 realBrickPosition = new Vector3(positionOnGridX * 2 - 12, 0, positionOnGridY);
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

        #region Getters & Setters

        public int BrickCount
        {
            get { return bricks.Count; }
        }

        public List<Brick> Bricks
        {
            get { return bricks; }
        }
       

        #endregion


    }
}
