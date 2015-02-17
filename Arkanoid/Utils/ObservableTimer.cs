using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arkanoid.Utils
{
    //Cette classe sert de l,objet observable dans le pattern observateur. Elle détient le timer du jeu
    //et notifie tous les CustomeTimer qui ont besoin
    public class ObservableTimer
    {
        private List<CustomTimer> observers;

        public ObservableTimer()
        {
            observers = new List<CustomTimer>();
        }

        public void RegisterObserver(CustomTimer timer)
        {
            observers.Add(timer);
        }

        public void UnRegisterObserver(CustomTimer timer)
        {
            observers.Remove(timer);
        }

        public void NotifyObservers(GameTime gameTime)
        {
            foreach(CustomTimer c in observers)
            {
                c.Notify(gameTime);
            }
        }
    }
}
