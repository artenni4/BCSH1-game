using Microsoft.Extensions.Logging;
using MyGame.Game.ECS.Entities;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.Scenes;
using System.Linq;

namespace MyGame.Game.ECS.Systems.QuestSystem.Quests
{
    internal class KillSlimesQuest : QuestBase
    {
        private readonly IEntityCollection _entityCollection;
        private readonly IEventSystem _eventSystem;
        private int toKill;
        private int killed;

        public KillSlimesQuest(IEntityCollection entityCollection, IEventSystem eventSystem)
        {
            _entityCollection = entityCollection;
            _eventSystem = eventSystem;

            toKill = _entityCollection.Entities.OfType<SlimeEntity>().Count();
            eventSystem.Subscribe<EntityKilledEvent>(EntityKilled);
        }

        public override bool IsComplete => killed >= toKill;

        public override string GetDescription() => $"Slimes killed {killed}/{toKill}";

        public bool EntityKilled(object sender, EntityKilledEvent entityKilled)
        {
            if (entityKilled.Killed is SlimeEntity)
            {
                killed++;
                _eventSystem.Emit(this, new QuestUpdateEvent(entityKilled.GameTime, this));
                return true;
            }
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            toKill = _entityCollection.Entities.OfType<SlimeEntity>().Count();
        }
    }
}
