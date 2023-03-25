using Microsoft.Xna.Framework.Content;
using MyGame.Game.Animators;
using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Components.Animation;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace MyGame.Game.ECS.Entities
{
    internal class PlayerEntity : EcsEntity
    {
        public float Speed { get; set; } = 100f;

        public Transform Transform { get; }
        public BoxCollider BoxCollider { get; }

        public Animation Animation { get; }

        public PlayerEntity()
        {
            Transform = AddComponent<Transform>();

            BoxCollider = AddComponent<BoxCollider>();
            BoxCollider.Box = new Rectangle(19, 23, 12, 17);

            Animation = AddComponent<Animation>();
            Animation.Animator = new PlayerAnimator();
        }

        public override void LoadContent(ContentManager contentManager)
        {
            Animation.Texture2D = contentManager.Load<Texture2D>("sprites/characters/player");
        }
    }
}
