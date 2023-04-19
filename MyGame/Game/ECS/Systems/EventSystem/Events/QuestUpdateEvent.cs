using MyGame.Game.ECS.Systems.QuestSystem.Quests;

namespace MyGame.Game.ECS.Systems.EventSystem.Events
{
    internal class QuestUpdateEvent : EventBase
    {
        public override EventGroup EventGroup => EventGroup.GameEvent;
        
        public QuestBase Quest { get; }

        public QuestUpdateEvent(GameTime gameTime, QuestBase quest) : base(gameTime)
        {
            Quest = quest;
        }
    }
}
