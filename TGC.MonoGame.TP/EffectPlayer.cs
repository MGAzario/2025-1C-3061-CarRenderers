using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Content.Collisions;


namespace TGC.MonoGame.TP
{

    public static class UI
    {

        public static SpriteBatch spriteBatch;
        public static SpriteFont font;
        public static GraphicsDevice graphicsDevice;
        

        public static void Initialize(SpriteFont aux, GraphicsDevice gd)
        {
            graphicsDevice = gd;
            spriteBatch = new SpriteBatch(graphicsDevice);
            font = aux;
        }
        
        public static void DrawCenterTextY(string msg, float Y, float escala)
        {
            var W = graphicsDevice.Viewport.Width;
            var H = graphicsDevice.Viewport.Height;
            var size = font.MeasureString(msg) * escala;
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null,
                Matrix.CreateScale(escala) * Matrix.CreateTranslation((W - size.X) / 2, Y, 0));
            spriteBatch.DrawString(font, msg, new Vector2(0, 0), Color.YellowGreen);
            spriteBatch.End();
        }

    }
    
}