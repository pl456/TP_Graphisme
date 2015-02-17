using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arkanoid.Entities
{
    public class Paddle : IWorldObject
    {
        private const float z = 29; //Position en Z
        private float x;            //Position en X
        private Matrix worldMatrix;
        private Model model;
        private Vector3 position;

  
        public Paddle(ContentManager contentManager)
        {
            position = new Vector3(0, 0, z);
            worldMatrix = Matrix.CreateTranslation(position);
            model = contentManager.Load<Model>("GameObjects/Paddle");
        }

        public void Move(float moveLength)
        {
            position.X += moveLength;
            worldMatrix = Matrix.CreateTranslation(position);
        }
/*
        //Retourne la position en x
        public float X
        {
            get { return x; }
        }

        //Retourne la position en z
        public float Z
        {
            get { return z; }
        }

*/

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
            get { return 4; }
        }

        public float Depth
        {
            get { return 1; }
        }
    }
}
