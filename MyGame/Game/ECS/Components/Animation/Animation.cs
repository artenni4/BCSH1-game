using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Components.Animation
{
    internal class Animation : EcsComponent
    {
        /// <summary>
        /// Sprite Atlas
        /// </summary>
        public Texture2D Texture2D { get; set; }

        public IAnimator Animator { get; set; }
    }
}
