using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arkanoid.Entities
{
    enum CurrentVelocity
    {
        Slow,
        Medium,
        Fast
    }

    public class Ball : IWorldObject
    {
        private const float DEFAULT_VELOCITY = 0.20f;        //Vitesse par défaut
        private const float FAST_VELOCITY = 0.3f;           //Vitesse rapide
        private const float SLOW_VELOCITY = 0.1f;          //Vitesse lente
        private  float xOffset = 1f;                     //Décallage des X pour éviter que la balle aille toujours au mêmes places
        private Matrix worldMatrix;                         //Matrice du monde de la balle
        private Model model;                                //Model 3d de la balle
        private float angle = 45;                           //Angle de rotation de la balle
        private Vector3 position;                           //Position de la balle
        private CurrentVelocity currentVelocityType;        //Type de vélocité (lent, moyen, rapide)
        private float velocityInX;                          //Vitesse sur l'axe des x
        private float velocityInZ;                          //Vitesse sur l'axe des z
        private int directionInX = -1;                      //Direction sur l'axe des x (-1 ou 1)
        private int directionInZ = -1;                      //Direction sur l'axe des z (-1 ou 1)
       

        public Ball(ContentManager contentManager)
        {
            position = new Vector3(1, 0, 28);
            worldMatrix = Matrix.CreateTranslation(position);
            currentVelocityType = CurrentVelocity.Medium;
            model = contentManager.Load<Model>("GameObjects/Ball");

           // Console.Out.Write("directionInX | xOffset | velocityInX | directionInZ | velocityInZ | position.Z | position.X");
        }

        public void Move()
        {

           
            //Détermine la vitesse de la balle
            switch(currentVelocityType)
            {
                case CurrentVelocity.Fast :

                    velocityInX = FAST_VELOCITY * directionInX * xOffset;
                    velocityInZ = FAST_VELOCITY * directionInZ;
                    break;
                case CurrentVelocity.Slow :

                    velocityInX = SLOW_VELOCITY * directionInX * xOffset;
                    velocityInZ = SLOW_VELOCITY * directionInZ;
                    break;
                default :

                    velocityInX = DEFAULT_VELOCITY * directionInX * xOffset;
                    velocityInZ = DEFAULT_VELOCITY * directionInZ;
                    break;
            }

            position.Z += velocityInZ;
            position.X += velocityInX;
            angle += 3; //Angle de rotation au centre de gravité de la basse (elle tourne sur elle même)


            
            worldMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(angle)) * Matrix.CreateTranslation(position);

            //Console.Out.WriteLine(directionInX + " " +  xOffset + " " +  velocityInX + " " +  directionInZ + " " +  velocityInZ + " " +  position.Z + " " +  position.X);
        }

        //Change la vitesse de la balle (rapide)
        public void SetFastVelocity()
        {
            currentVelocityType = CurrentVelocity.Fast;
        }

        //Change la vitesse de la balle (Default)
        public void SetMediumVelocity()
        {
            currentVelocityType = CurrentVelocity.Medium;
        }

        //Change la vitesse de la balle (lente)
        public void SetSlowVelocity()
        {
            currentVelocityType = CurrentVelocity.Slow;
        }
        
        public void ReverseVelocityInX()
        {
            directionInX = directionInX * -1;
        }

        public void ReverseVelocityInZ()
        {
            directionInZ = directionInZ * -1;
        }

        //Ajuste l'angle de translation de la balle
        //L'angle est trouvé grace à la formule linéraire
        //1.5 |x| + 1 où x est le point de frappe de la
        //balle sur le paddle.
        public void AjustAngle(float paddlePositionX)
        {
            xOffset = 1.5f * Math.Abs(paddlePositionX - position.X) + 1;
        }
        
        //Remet les paramètres par défaut à la balle
        public void Reset()
        {
            position = new Vector3(1, 0, 28);
            worldMatrix = Matrix.CreateTranslation(position);
            currentVelocityType = CurrentVelocity.Medium;
            angle = 45;
            directionInX = -1;
            directionInZ = -1;
        }

        #region Setters & getters

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
            set { position = value; }
        }

        public float Height
        {
            get { return 1; }
        }

        public float Width
        {
            get { return 1; }
        }

        public float Depth
        {
            get { return 1; }
        }

        public int DirectionInX
        {
            get { return directionInX; }
        }

        public int DirectionInZ
        {
            get { return directionInZ; }
        }

        #endregion


    }
}
