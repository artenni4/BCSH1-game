using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.SerializableTypes
{
    [Serializable]
    internal record SerializableTexture(string Name)
    {
        public SerializableTexture(Texture2D texture2D)
            : this(texture2D.Name)
        {
        }
    }
}
