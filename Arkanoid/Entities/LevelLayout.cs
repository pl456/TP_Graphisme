using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arkanoid.Entities
{
    public class LevelLayout
    {
        private BrickController brickController;


        public LevelLayout(BrickController brickController)
        {
            this.brickController = brickController;
        }

        public void CreateLevel(int levelNumber)
        {
            string layoutString = "";

            switch(levelNumber)
            {
                case 0 :
                    layoutString = "0,6,B;1,6,B;2,6,B;3,6,B;4,6,B;5,6,B;6,6,B;7,6,B;8,6,B;9,6,B;10,6,B;11,6,B;12,6,B";
                    break;
                case 1 :
                    layoutString = "3,3,B;9,3,B;3,4,B;9,4,B;4,5,B;8,5,B;4,6,B;8,6,B;3,7,B;4,7,B;5,7,B;6,7,B;7,7,B;8,7,B;9,7,B;3,8,B;4,8,B;5,8,B;6,8,B;7,8,B;8,8,B;9,8,B;2,9,B;3,9,B;4,9,R;5,9,B;6,9,B;7,9,B;8,9,R;9,9,B;10,9,B;2,10,B;3,10,B;4,10,R;5,10,B;6,10,B;7,10,B;8,10,R;9,10,B;10,10,B;1,11,B;2,11,B;3,11,B;4,11,B;5,11,B;6,11,B;7,11,B;8,11,B;9,11,B;10,11,B;11,11,B;1,12,B;2,12,B;3,12,B;4,12,B;5,12,B;6,12,B;7,12,B;8,12,B;9,12,B;10,12,B;11,12,B;1,13,B;2,13,B;3,13,B;4,13,B;5,13,B;6,13,B;7,13,B;8,13,B;9,13,B;10,13,B;11,13,B;1,14,B;2,14,B;3,14,B;4,14,B;5,14,B;6,14,B;7,14,B;8,14,B;9,14,B;10,14,B;11,14,B;1,15,B;2,15,B;3,15,B;9,15,B;10,15,B;11,15,B;1,16,B;3,16,B;9,16,B;11,16,B;1,17,B;3,17,B;9,17,B;11,17,B;1,18,B;3,18,B;9,18,B;11,18,B;4,19,B;5,19,B;7,19,B;8,19,B;4,20,B;5,20,B;7,20,B;8,20,B";
                    break;
                case 2 :
                    layoutString = "6,0,B;6,1,B;6,2,B;6,3,B;6,4,B;3,5,B;4,5,B;5,5,B;6,5,B;7,5,B;8,5,B;9,5,B;2,6,B;3,6,B;4,6,B;5,6,R;6,6,R;7,6,R;8,6,B;9,6,B;10,6,B;2,7,B;3,7,B;4,7,R;5,7,R;6,7,R;7,7,R;8,7,R;9,7,B;10,7,B;0,8,B;1,8,B;2,8,B;3,8,B;4,8,R;5,8,R;6,8,R;7,8,R;8,8,R;9,8,B;10,8,B;11,8,B;12,8,B;0,9,B;1,9,B;2,9,B;3,9,B;4,9,R;5,9,R;6,9,R;7,9,R;8,9,R;9,9,B;10,9,B;11,9,B;12,9,B;0,10,B;1,10,B;2,10,B;3,10,B;4,10,R;5,10,R;6,10,R;7,10,R;8,10,R;9,10,B;10,10,B;11,10,B;12,10,B;0,11,B;1,11,B;4,11,B;6,11,B;8,11,B;11,11,B;12,11,B;0,12,B;6,12,B;12,12,B;0,13,B;6,13,B;12,13,B;0,14,B;6,14,B;12,14,B;0,15,B;4,15,R;6,15,R;12,15,B;0,16,B;4,16,R;5,16,R;6,16,R;12,16,B;0,17,B;5,17,R;6,17,R;12,17,B;0,18,B;6,18,R;12,18,B;0,19,B;6,19,R;12,19,B;0,20,B;6,20,R;12,20,B;0,21,B;6,21,R;12,21,B;0,22,B;6,22,R;12,22,B;0,23,B;6,23,R;12,23,B;0,24,B;6,24,R;12,24,B;0,25,B;6,25,R;12,25,B;0,26,B;6,26,R;12,26,B";
                    break;
            }

            CreateLevelFromString(layoutString);
        }

        //Crée le niveau en fonction de la chaine (indique la coordonnée (x,y) et la couleur de la brique (B,R, ...)
        //Met les briques dans le contrôleur
        public void CreateLevelFromString(string layoutString)
        {
            String[] str = layoutString.Split(';');
            String[] elements;

            //Vide la liste de briques du contrôleur
            brickController.ClearList();

            int positionX;
            int positionY;
            BrickColor brickColor = BrickColor.Blue;    //Initialisation (la vrai couleur est établie plus bas (non-nullable)

            if (layoutString != "")
            { 
                //Boucle chacunes des séquences de caractères du genre "1,2,B". => (x=1, y=2, couleur=bleue)
                foreach(String strBrick in str)
                {
                    elements = strBrick.Split(',');

                    //Position x.y dans la chaîne
                    positionX = Convert.ToInt32(elements[0]);
                    positionY = Convert.ToInt32(elements[1]);


                    //Couleur dans la chaîne
                    switch (Convert.ToChar(elements[2]))
                    {
                        case 'B' :
                            brickColor = BrickColor.Blue;
                            break;

                        case 'R':
                            brickColor = BrickColor.Red;
                            break;


                    }
                    //Crée une brique dans le contrôleur 
                    brickController.AddBrick(positionX, positionY, brickColor);
                }
            }
        }


        public BrickController BrickController
        {
            get { return brickController; }
            set { brickController = value; }
        }
        
    }
}
