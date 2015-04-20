using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Arkanoid.Entities
{
    public class LevelLayout
    {
        private const String LEVELS_TEXT_FILE_NAME = "..\\..\\..\\Ressources\\Levels.txt";
        private BrickController brickController;
        private List<Level> levels = new List<Level>();         //Contient les levels
        private int actualLevelNumber = 1;                          //Numéro du niveau actuel

        public LevelLayout(BrickController brickController)
        {
            this.brickController = brickController;

            //Charge les niveaux à partir du fichier
            LoadLevels();

        }

        private void LoadLevels()
        {
            System.IO.StreamReader file = new System.IO.StreamReader(LEVELS_TEXT_FILE_NAME);

            string lineContent;
            while ((lineContent = file.ReadLine()) != null)
            {
                
                //Bouble les caractères de la ligne pour déterminer si la ligne est un commentaire ou non
                for (int i = 0; i < lineContent.Count(); i++)
                {
                    //Si le caractère lu n'est pas un whitespace (espace, tab, etc.)
                    if(!Char.IsWhiteSpace(lineContent[i]))
                    {
                        //Si la ligne n'est pas un commentaire
                        if (lineContent[i] != '#')
                        {
                            //Crée le niveau en mémoire
                            AddLevelToList(lineContent);
                            
                        }

                        break;
                    }
                }
            }

            file.Close();
        }

        //Ajoute un level à la liste (en mémoire, n'affiche pas le level)
        private void AddLevelToList(String levelString)
        {
            

            //Boucle la chaîne de level et sépare le nom du level de sa chaîne de layout
            for(int i=0; i<levelString.Count(); i++)
            {
                //Atteinte de la première virgule (délimite le nom et le layout)
                if(levelString[i] == ',')
                {
                    Level level = new Level();
                    level.Name = levelString.Substring(0, i);
                    level.Layout = levelString.Substring(i + 1, levelString.Count()-(i+1));
                    levels.Add(level);
                    break;
                }
            }
        }

        public void CreateLevel(int levelNumber)
        {
            string layoutString = "";

            /*
            switch(levelNumber)
            {
                case 0 :
                    layoutString = "0,6,B;1,6,B;2,6,B;3,6,B;4,6,B;5,6,B;6,6,B;7,6,B;8,6,B;9,6,B;10,6,B;11,6,B;12,6,B";
                    break;
                case 1 :
                    layoutString = "";
                    break;
                case 2 :
                    layoutString = "";
                    break;
            }
            */

            if (levelNumber > levels.Count() || levelNumber < 1)
                throw new Exception("Chargement du niveau impossible, le numéro de niveau est invalide");

            //Garde en mémoire le numéro du niveau en cours
            actualLevelNumber = levelNumber;

            //Crée le niveau
            //NB les numéro de niveau vont de 1 à x, alors que les indices de niveau dans la liste 
            //vont de 0 à x. C,est pourquoi nous appellons l'indice avant le numéro de niveau.
            CreateLevelFromString(levels[levelNumber-1]);
        }

        //Crée le niveau en fonction de la chaine (indique la coordonnée (x,y) et la couleur de la brique (B,R, ...)
        //Met les briques dans le contrôleur
        private void CreateLevelFromString(Level level)
        {
            
            String[] str = level.Layout.Split(';');
            String[] elements;

            //Vide la liste de briques du contrôleur
            brickController.ClearList();

            int positionX;
            int positionY;
            BrickColor brickColor = BrickColor.Blue;    //Initialisation (la vrai couleur est établie plus bas (non-nullable)

            if (level.Layout != "")
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

        //Retourne le nombre total de niveau disponible
        public int NumberOfLevels
        {
            get { return levels.Count; }
           
        }

        //Retourne numéro du niveau actuel
        public int ActualLevelNumber
        {
            get { return actualLevelNumber; }

        }

        //Retourne nom du niveau actuel
        public string ActualLevelName
        {
            get { return levels[actualLevelNumber-1].Name; }

        }

        public List<string> LevelNameList
        {
            get
            {
                List<string> list = new List<string>();
                foreach (Level level in levels)
                    list.Add(level.Name);


                return list;
            }
        }
        
    }
}
