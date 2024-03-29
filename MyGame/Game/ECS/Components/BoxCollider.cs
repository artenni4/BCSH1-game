﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Components
{
    internal class BoxCollider : EcsComponent
    {
        public bool IsStatic { get; set; }

        /// <summary>
        /// Whether object can push or be pushed
        /// </summary>
        public bool IsKinematic { get; set; } = true;
        public Rectangle Box { get; set; }
    }
}
