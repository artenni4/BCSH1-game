using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.Helpers
{
    internal static class DrawHelper
    {
        static Texture2D _pointTexture = null;
        public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rectangle, Color color, int thickness)
        {
            if (_pointTexture == null)
            {
                _pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _pointTexture.SetData(new Color[] { Color.White });
            }

            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height + thickness), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + thickness, thickness), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, thickness, rectangle.Height + thickness), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + thickness, thickness), color);
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 startPos, Vector2 endPos, Color color, int thickness)
        {
            // Create a texture as wide as the distance between two points and as high as
            // the desired thickness of the line.
            var distance = (int)Vector2.Distance(startPos, endPos);
            if (distance <= 0)
            {
                return;
            }

            var texture = new Texture2D(spriteBatch.GraphicsDevice, distance, thickness);

            // Fill texture with given color.
            var data = new Color[distance * thickness];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = color;
            }
            texture.SetData(data);

            // Rotate about the beginning middle of the line.
            var rotation = (float)Math.Atan2(endPos.Y - startPos.Y, endPos.X - startPos.X);
            var origin = new Vector2(0, thickness / 2);

            spriteBatch.Draw(texture, startPos, null, Color.White, rotation, origin, 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}
