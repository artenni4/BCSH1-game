using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.ECS.Systems.QuestSystem;
using MyGame.Game.ECS.Systems.QuestSystem.Quests;
using MyGame.Game.Scenes;

namespace MyGame.Game.ECS.Systems
{
    internal class QuestProgressSystem : EcsSystem, IQuestSystem
    {
        public static IReadOnlyDictionary<string, string> LevelSequence = new Dictionary<string, string>
        {
            { "level1", "level2" },
            { "level2", "level3" }
        };

        private readonly IEventSystem _eventSystem;
        private readonly ISceneManager _sceneManager;
        private readonly QuestBase sceneQuest;

        private TimeSpan? loadNextSceneTime;
        private string nextScene;

        public QuestBase ActiveQuest => sceneQuest;

        public QuestProgressSystem(IEntityCollection entityCollection, IEventSystem eventSystem, ISceneManager sceneManager) : base(entityCollection)
        {
            _eventSystem = eventSystem;
            sceneQuest = new KillSlimesQuest(entityCollection, eventSystem);

            _eventSystem.Subscribe<QuestUpdateEvent>(OnSceneQuestCompleted);
            _sceneManager = sceneManager;
        }

        public override void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime >= loadNextSceneTime)
            {
                _sceneManager.LoadScene(nextScene);
            }

            ActiveQuest.Update(gameTime);
        }

        private bool OnSceneQuestCompleted(object sender, QuestUpdateEvent questUpdate)
        {
            if (questUpdate.Quest.IsComplete)
            {
                if (LevelSequence.TryGetValue(_sceneManager.CurrentSceneName, out nextScene))
                {
                    loadNextSceneTime = questUpdate.GameTime.TotalGameTime + TimeSpan.FromSeconds(1);
                }
                return true;
            }
            return false;
        }
    }
}
