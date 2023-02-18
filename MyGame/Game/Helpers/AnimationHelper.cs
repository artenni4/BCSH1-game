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
            Rectangle[] bounds = new Rectangle[framesCount];
            for (int i = 0; i < framesCount; i++)
            {
                bounds[i] = new Rectangle(originX + i * (rectWidth + xGap), originY, rectWidth, rectHeight);
            }

            return bounds.ToArray();
        }

        public static Rectangle[][] GenerateBoundsForAnimationAtlas(int xOrigin, int yOrigin, int rectWidth, int rectHeight, int framesCount, int animationsCount)
        {
            Rectangle[][] bounds = new Rectangle[animationsCount][];

            for (int i = 0; i < animationsCount; i++)
            {
                bounds[i] = new Rectangle[framesCount];
                for (int j = 0; j < framesCount; j++)
                {
                    bounds[i][j] = new Rectangle(xOrigin + j * rectWidth, yOrigin + i * rectHeight, rectWidth, rectHeight);
                }
            }
            return bounds;
        }
    }
}
