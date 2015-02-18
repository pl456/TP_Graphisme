using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arkanoid.Entities
{
    public enum BrickColor
    {
        Blue,
        Red
    }

    public abstract class Brick : IWorldObject
    {
        

        protected BrickColor brickColor;
        protected int health;
        protected Model model;
        protected Matrix worldMatrix;

        //Position X, Y sur la grille (pas sur le plan 3D)
        protected int positionOnGridX;
        protected int positionOnGridY;

        //Position réelle dans le plan 3D
        protected Vector3 position;

        //Fait une translation en y. Ne peut pas dépasser 0
        public void TranslateInY(float yDisplacement)
        {
            float newY = position.Y - yDisplacement;

            if (newY < 0)
                newY = 0;

            position.Y = newY;

            worldMatrix = Matrix.CreateTranslation(position);
        }

        #region Getters & Setters
        public BrickColor BrickColor
        {
            get { return brickColor; }
            set { brickColor = value; }
        }

        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        public Model Model
        {
            get { return model; }
        }

        public int PositionOnGridX
        {
            get { return positionOnGridX; }
            set { positionOnGridX = value; }
        }

        public int PositionOnGridY
        {
            get { return positionOnGridY; }
            set { positionOnGridY = value; }
        }

        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set { worldMatrix = value; }
        }

        public Vector3 Position
        {
            get { return position; }

        }

        public float Height
        {
            get { return 1; }
        }

        public float Width
        {
            get { return 2; }
        }

        public float Depth
        {
            get { return 1; }
        }
        #endregion
    }
}