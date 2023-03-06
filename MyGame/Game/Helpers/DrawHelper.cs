using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.Helpers
{
    internal static class DrawHelper
    {
        static Texture2D _pixel = null;

        private static void InitPixel(SpriteBatch spriteBatch)
        {
            if (_pixel is null)
            {
                _pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _pixel.SetData(new Color[] { Color.White });
            }
        }

        public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rectangle, Color color, int thickness)
        {
            InitPixel(spriteBatch);

            spriteBatch.Draw(_pixel, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height + thickness), color);
            spriteBatch.Draw(_pixel, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + thickness, thickness), color);
            spriteBatch.Draw(_pixel, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, thickness, rectangle.Height + thickness), color);
            spriteBatch.Draw(_pixel, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + thickness, thickness), color);
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 startPos, Vector2 endPos, Color color, int thickness)
        {
            InitPixel(spriteBatch);

            // Create a texture as wide as the distance between two points and as high as
            // the desired thickness of the line.
            var distance = (int)Vector2.Distance(startPos, endPos);
            if (distance <= 0)
            {
                return;
            }

            // Rotate about the beginning middle of the line.
            Vector2 tangent = endPos - startPos;
            var rotation = (float)Math.Atan2(tangent.Y, tangent.X);
            var origin = new Vector2(0, thickness / 2);

            spriteBatch.Draw(_pixel, startPos, null, color, rotation, origin, new Vector2(tangent.Length(), thickness), SpriteEffects.None, 0.0f);
        }
    }
}
