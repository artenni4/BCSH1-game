using MyGame.Game.ECS.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Entities
{
    [Serializable]
    internal class SerializableEntity
    {
        public string EntityType { get; set; }
        public Guid Id { get; set; }
        public List<SerializableComponent> Components { get; set; }
    }
}
