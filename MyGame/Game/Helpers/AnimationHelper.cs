using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.Game.Helpers
{
    internal static class AnimationHelper
    {
        /// <summary>
        /// Used to decide what to do with last rectangle if it's not in correct form
        /// </summary>
        public enum RemainderAction
        {
            /// <summary>
            /// Appends remainder to previous bounds
            /// </summary>
            AppendToLast, 

            /// <summary>
            /// Removes remainder completely
            /// </summary>
            CutOff, 

            /// <summary>
            /// Separates remainder to new bound that will be the last one
            /// </summary>
            Separate,
        }

        public static Rectangle[] GenerateBoundsForAtlasHorizontal(int originX, int originY, int rectHeight, int rectWidth, int xGap, int atlasWidth, 
            RemainderAction remainderAction = RemainderAction.CutOff)
        {
            int currWidth = 0;
            LinkedList<Rectangle> bounds = new();
            while (currWidth + rectWidth <= atlasWidth)
            {
                bounds.AddLast(new Rectangle(originX + currWidth, originY, rectWidth, rectHeight));
                currWidth += rectWidth + xGap;
            }

            if (currWidth + rectWidth > atlasWidth)
            {
                int remainderWidth = atlasWidth - currWidth;
                switch (remainderAction)
                {
                    case RemainderAction.AppendToLast:
                        bounds.RemoveLast();
                        bounds.AddLast(new Rectangle(atlasWidth - remainderWidth - rectWidth, originY, rectWidth + remainderWidth, rectHeight));
                        break;
                    case RemainderAction.CutOff:
                        break;
                    case RemainderAction.Separate:
                        bounds.AddLast(new Rectangle(atlasWidth - remainderWidth, originY, remainderWidth, rectHeight));
                        break;
                }
            }

            return bounds.ToArray();
        }
    }
}
