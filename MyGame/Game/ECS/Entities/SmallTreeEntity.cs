using Microsoft.Xna.Framework.Content;
using MyGame.Game.Constants.Enums;
using MyGame.Game.ECS.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Entities
{
    internal class SmallTreeEntity : EcsEntity
    {
        public Transform Transform { get; }
        public Image Image { get; }
        public BoxCollider BoxCollider { get; }

        public SmallTreeEntity()
        {
            Transform = AddComponent<Transform>();
            Transform.ZIndex = ZIndex.Middleground;

            Image = AddComponent<Image>();
            Image.SourceRectangle = new Rectangle(129, 97, 30, 46);

            BoxCollider = AddComponent<BoxCollider>();
            BoxCollider.Box = new Rectangle(8, 34, 15, 13);
            BoxCollider.IsStatic = true;
        }

        public override void LoadContent(ContentManager contentManager)
        {
            Image.Texture2D = contentManager.Load<Texture2D>("sprites/objects/objects");
        }
    }
}
