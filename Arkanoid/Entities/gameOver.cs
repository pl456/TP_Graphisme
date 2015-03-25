using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Arkanoid.Entities
{


    public class gameOver
    {

        Texture2D gameOverImage;
        Vector2 position;
        Timer timer = new Timer(2000);

        public gameOver(ContentManager contentManager)
        {
           position = new Vector2(250, 250);
           gameOverImage = contentManager.Load<Texture2D>("GameObjects/gameOver");
           timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

        }

        public void show()
        {
            timer.Start();
             
        }

        public void draw(SpriteBatch spriteBatch)
        {         
            if (timer.Enabled) 
            {
                spriteBatch.Begin();
                spriteBatch.Draw(gameOverImage, position, Color.White);
                spriteBatch.End();
            }      
        }

        public Boolean isShowing()
        {
            return timer.Enabled;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            timer.Enabled = false;
        }

    }
}
