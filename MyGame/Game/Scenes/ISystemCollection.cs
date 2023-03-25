using MyGame.Game.ECS.Systems;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.Scenes
{
    internal interface ISystemCollection
    {
        ICollection<EcsSystem> Systems { get; }
    }
}
