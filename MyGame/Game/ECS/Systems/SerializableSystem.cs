using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems
{
    [Serializable]
    internal class SerializableSystem
    {
        public string SystemType { get; set; }
        public Dictionary<string, object> Properties { get; set; }
    }
}
