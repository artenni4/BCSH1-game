using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems.QuestSystem.Quests
{
    internal abstract class QuestBase
    {
        public abstract bool IsComplete { get; }
        public abstract void Update(GameTime gameTime);
        public abstract string GetDescription();
        public abstract Dictionary<string, object> SaveData();
        public abstract void LoadData(Dictionary<string, object> data);
    }
}
