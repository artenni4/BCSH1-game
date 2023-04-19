using MyGame.Game.ECS.Systems.QuestSystem.Quests;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems.QuestSystem
{
    internal interface IQuestSystem
    {
        public QuestBase ActiveQuest { get; }
    }
}
