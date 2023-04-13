using Microsoft.Xna.Framework.Content;
using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Components.Collider;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Entities
{
    internal class BushEntity : EcsEntity
    {
        public Transform Transform { get; }
        public Image Image { get; }
        public BoxCollider BoxCollider { get; }

        public BushEntity()
        {
            Transform = AddComponent<Transform>();

            Image = AddComponent<Image>();
            Image.SourceRectangle = new Rectangle(96, 112, 32, 32);

            BoxCollider = AddComponent<BoxCollider>();
            BoxCollider.Box = new Rectangle(4, 4, 24, 24);
            BoxCollider.IsStatic = true;
        }

        public override void LoadContent(ContentManager contentManager)
        {
            Image.Texture2D = contentManager.Load<Texture2D>("sprites/objects/objects");
        }

        public override void Update(GameTime gameTime)
        {

        }
    }
}
