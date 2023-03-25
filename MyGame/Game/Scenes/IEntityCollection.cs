using MyGame.Game.ECS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.Scenes
{
    internal interface IEntityCollection
    {
        ICollection<EcsEntity> Entities { get; }
    }
}
