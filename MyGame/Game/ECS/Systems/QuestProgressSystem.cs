using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.ECS.Systems.QuestSystem;
using MyGame.Game.ECS.Systems.QuestSystem.Quests;
using MyGame.Game.Scenes;

namespace MyGame.Game.ECS.Systems
{
    internal class QuestProgressSystem : EcsSystem, IQuestSystem
    {
        private readonly IEventSystem _eventSystem;
        private readonly QuestBase sceneQuest;

        public QuestBase ActiveQuest => sceneQuest;

        public QuestProgressSystem(IEntityCollection entityCollection, IEventSystem eventSystem) : base(entityCollection)
        {
            _eventSystem = eventSystem;
            sceneQuest = new KillSlimesQuest(entityCollection, eventSystem);
        }

        public override void Update(GameTime gameTime)
        {
            ActiveQuest.Update(gameTime);
        }
    }
}
