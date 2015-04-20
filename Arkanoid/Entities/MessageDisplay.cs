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


    public class MessageDisplay
    {

        Texture2D Textureimage;
        Vector2 position;
        Timer timer = new Timer(3000);

        public MessageDisplay(ContentManager contentManager, string pathTextureImage)
        {
           position = new Vector2(330, 250);
           Textureimage = contentManager.Load<Texture2D>(pathTextureImage);
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
                spriteBatch.Draw(Textureimage, position, Color.White);
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
