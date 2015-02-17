using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arkanoid.Entities
{
    public enum WallPosition
    {
        Left,
        Back,
        Right
    }

    public class Wall : IWorldObject
    {
        private float height = 1;
        private float width;
        private float depth;

        private Matrix worldMatrix;
        private Model model;
        private float x;    //Position en x
        private float z;    //Position en z
        private Vector3 position;

        
        

        public Wall(ContentManager contentManager, WallPosition wallPosition)
        {
            //Initialise le modèle
            model = contentManager.Load<Model>("GameObjects/Wall");

            //Initialise la position du mur en fonction du type de mur (gauche, fond, droit)
            switch(wallPosition)
            {
                case WallPosition.Back :
                    //x = 0;
                    //z = -1.5f;
                    position = new Vector3(0, 0, -1.5f);
                    worldMatrix = Matrix.CreateTranslation(position);
                    width = 30;
                    depth = 2;
                    break;

                case WallPosition.Left :
                   // x = -14;
                    //z = 14.5f;
                    position = new Vector3(-14, 0, 14.5f);
                    worldMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(90)) * Matrix.CreateTranslation(new Vector3(position.X, 0, 0)) *
                        Matrix.CreateTranslation(new Vector3(0, 0, position.Z));
                    width = 2;
                    depth = 30;
                    break;

                case WallPosition.Right :
                    //x = 14;
                    //z = 14.5f;
                    position = new Vector3(14, 0, 14.5f);
                    worldMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(90)) * Matrix.CreateTranslation(new Vector3(position.X, 0, 0)) *
                        Matrix.CreateTranslation(new Vector3(0, 0, position.Z));
                    width = 2;
                    depth = 30;
                    break;
            }
            
        }
        
        
        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set { worldMatrix = value; }
        }

        public Model Model
        {
            get { return model; }
            set { model = value; }
        }
        /*
        public float X
        {
            get { return x; }
        }
        */
        public Vector3 Position
        {
            get { return position; }
        }

        public float Height
        {
            get { return height; }
        }

        public float Width
        {
            get { return width; }
        }

        public float Depth
        {
            get { return depth; }
        }
    }
}
