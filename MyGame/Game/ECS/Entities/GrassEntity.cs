using Microsoft.Xna.Framework.Content;
using MyGame.Game.Constants.Enums;
using MyGame.Game.ECS.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Entities
{
    internal class GrassEntity : EcsEntity
    {
        public Transform Transform { get; }
        public Image Image { get; }
        public GrassComponent GrassComponent { get; }

        public GrassVariation GrassVariation
        {
            get => GrassComponent.Variation;
            set
            {
                GrassComponent.Variation = value;
                Image.SourceRectangle = new Rectangle((int)GrassComponent.Variation * 16 + 1, 1, 14, 14);
            }
        }

        public GrassEntity()
        {
            Transform = AddComponent<Transform>();
            Transform.ZIndex = ZIndex.Middleground;

            Image = AddComponent<Image>();

            GrassComponent = AddComponent<GrassComponent>();
        }

        public override void LoadContent(ContentManager contentManager)
        {
            Image.Texture2D = contentManager.Load<Texture2D>("sprites/tilesets/decor_16x16");
        }
    }
}
