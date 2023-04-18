using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.SerializableTypes
{
    [Serializable]
    internal record SerializableVector(float X, float Y)
    {
        public SerializableVector(Vector2 vector2)
            : this(vector2.X, vector2.Y)
        {

        }
        
        public Vector2 ToVector2() => new(X, Y);
    }
}
