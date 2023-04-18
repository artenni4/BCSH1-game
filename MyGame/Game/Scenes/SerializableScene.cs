using MyGame.Game.ECS.Entities;
using MyGame.Game.ECS.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.Scenes
{
    [Serializable]
    internal class SerializableScene
    {
        public string Name { get; set; }
        public List<SerializableEntity> Entities { get; set; }
        public List<SerializableSystem> Systems { get; set; }
    }
}
