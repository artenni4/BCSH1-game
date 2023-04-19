using Microsoft.Xna.Framework.Content;
using MyGame.Game.Constants.Enums;
using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Components.HUD;
using MyGame.Game.ECS.Systems;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.ECS.Systems.QuestSystem;

namespace MyGame.Game.ECS.Entities
{
    internal class ScoreEntity : EcsEntity
    {
        private readonly IQuestSystem _questSystem;

        public Transform Transform { get; }
        public HUDText ScoreText { get; }

        public ScoreEntity(IEventSystem eventSystem, IQuestSystem questSystem)
        {
            _questSystem = questSystem;

            Transform = AddComponent<Transform>();
            Transform.ZIndex = ZIndex.Background;

            ScoreText = AddComponent<HUDText>();
        }

        public override void Update(GameTime gameTime)
        {
            ScoreText.Text = _questSystem.ActiveQuest.GetDescription();
        }

        public override void LoadContent(ContentManager contentManager)
        {
            ScoreText.Font = contentManager.Load<SpriteFont>("game-font");
        }
    }
}
