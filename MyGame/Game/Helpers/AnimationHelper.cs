using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.Game.Helpers
{
    internal static class AnimationHelper
    {
        public static Rectangle[] GenerateBoundsForAtlasHorizontal(int originX, int originY, int rectHeight, int rectWidth, int framesCount, int xGap = 0)
        {
            LinkedList<Rectangle> bounds = new();
            for (int i = 0; i < framesCount; i++)
            {
                bounds.AddLast(new Rectangle(originX + i * (rectWidth + xGap), originY, rectWidth, rectHeight));
            }

            return bounds.ToArray();
        }
    }
}
