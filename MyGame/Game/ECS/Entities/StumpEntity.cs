using Microsoft.Xna.Framework.Content;
using MyGame.Game.Constants.Enums;
using MyGame.Game.ECS.Components;

namespace MyGame.Game.ECS.Entities
{
    internal class StumpEntity : EcsEntity
    {
        public Transform Transform { get; }
        public Image Image { get; }
        public BoxCollider BoxCollider { get; }

        public StumpEntity()
        {
            Transform = AddComponent<Transform>();
            Transform.ZIndex = ZIndex.Middleground;

            Image = AddComponent<Image>();
            Image.SourceRectangle = new Rectangle(161, 113, 30, 30);

            BoxCollider = AddComponent<BoxCollider>();
            BoxCollider.Box = new Rectangle(2, 8, 26, 18);
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
