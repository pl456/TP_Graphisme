using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arkanoid.Utils
{
    public class CustomTimer
    {
        private double startTime = 0;
        private GameTime gameTime;
        private Boolean isTimerStarted = false;

        
        public CustomTimer()
        {
            this.gameTime = null;   //Le gametime est updaté par ObservableTimer
        }

        //Utilisé quand l'observable ObservableTimer notifie les observeurs
        public void Notify(GameTime gameTime)
        {
            this.gameTime = gameTime;
        }

        public void Start()
        {
            startTime = gameTime.TotalGameTime.TotalMilliseconds;
            isTimerStarted = true;
        }

        public void updateTime(GameTime gameTime)
        {
            this.gameTime = gameTime;
        }

        //Détemine si un nombre x de milisecondes se sont écoulé depuis
        //le démarrage du timer (méthode Start())
        public Boolean isTimeUp(long elapsedTimeInMilis)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds >= startTime + elapsedTimeInMilis)
                return true;

            return false;
        }

        public Boolean IsTimerStarted
        {
            get { return isTimerStarted; }
        }
    }
}
