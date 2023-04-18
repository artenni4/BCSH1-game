using MyGame.Game.ECS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Components
{
    [Serializable]
    internal class SerializableComponent
    {
        public string ComponentType { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }
}
