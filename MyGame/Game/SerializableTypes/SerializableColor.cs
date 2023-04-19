using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.SerializableTypes
{
    [Serializable]
    internal record SerializableColor(byte r, byte g, byte b, byte a)
    {
        public SerializableColor(Color color)
            : this(color.R, color.G, color.B, color.A)
        {
        }

        public Color ToColor() => new Color(r, g, b, a);
    }
}
