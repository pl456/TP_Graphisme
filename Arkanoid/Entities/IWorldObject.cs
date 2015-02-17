using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arkanoid.Entities
{
    //Cette interface sert à établir les méthodes qui sont dans chacuns des élément visibles du jeux.
    //Elle sert à s'assurer que les classes ont les bonne méthiodes pour le traitement des collisions.
    public interface IWorldObject
    {
        // Property declaration:
        Vector3 Position
        {
            get;
        }

        Model Model
        {
            get;
        }

        float Height
        {
            get;
        }

        float Width
        {
            get;
        }

        float Depth
        {
            get;
        }
    }
}
