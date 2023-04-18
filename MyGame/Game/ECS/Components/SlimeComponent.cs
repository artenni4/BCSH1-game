using MyGame.Game.Constants.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Components
{
    internal class SlimeComponent : EcsComponent
    {
        public SlimeStrength SlimeStrength { get; set; }
        public TimeSpan JumpInterval { get; set; }
        public float AttackDistance { get; set; }
        public float Speed { get; set; }
    }
}
