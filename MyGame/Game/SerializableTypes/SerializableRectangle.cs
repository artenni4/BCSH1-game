using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.SerializableTypes
{
    [Serializable]
    internal record SerializableRectangle(int X, int Y, int Width, int Height)
    {
        public SerializableRectangle(Rectangle rectangle)
            : this(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height)
        {
        }

        public Rectangle ToRectangle() => new(X, Y, Width, Height);
    }
}
