using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arkanoid.Entities
{
    class BlueBrick : Brick
    {


        public BlueBrick(int positionOnGridX, int positionOnGridY, Vector3 worldPosition, BrickColor brickColor, Model model)
        {

            brickColor = BrickColor.Blue;
            health = 1;
            this.model = model;
            this.positionOnGridX = positionOnGridX;
            this.positionOnGridY = positionOnGridY;

            position = worldPosition;
            worldMatrix = Matrix.CreateTranslation(position);

        }
    }
}
