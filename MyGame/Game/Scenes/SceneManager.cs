using Microsoft.Extensions.DependencyInjection;
using MyGame.Game.Factories;
using System.Linq;

namespace MyGame.Game.Scenes
{
    internal class SceneManager : ISceneManager
    {
        private readonly SceneFactory sceneFactory;
        private readonly MyGame game;

        public string CurrentSceneName { get; private set; }
        public Dictionary<string, SceneBase> SavedScenes { get; private set; }

        public SceneBase CurrentScene
        {
            get
            {
                if (SavedScenes.TryGetValue(CurrentSceneName, out var scene))
                {
                    return scene;
                }
                return null;
            }
        }

        public SceneManager(MyGame game)
        {
            this.game = game;

            var serviceProvider = new ServiceCollection()
                .AddSingleton<ISceneManager>(this)
                .AddSingleton(game.GraphicsDevice)
                .AddSingleton(sp => new SpriteBatch(sp.GetRequiredService<GraphicsDevice>()))
                .AddGameServices().BuildServiceProvider();
            sceneFactory = new SceneFactory(serviceProvider.GetService<IServiceScopeFactory>());
            SavedScenes = new Dictionary<string, SceneBase>();
        }

        public void LoadScene(string sceneName)
        {
            CurrentSceneName = sceneName;
            if (CurrentScene is null)
            {
                SavedScenes[sceneName] = sceneFactory.CreateSceneFromXml(sceneName);
            }
            game.Content.Unload();
            CurrentScene.LoadContent(game.Content);
        }

        public void SaveGame(string saveName)
        {
            var serializableScenes = SavedScenes.Select(ss => ss.Value.ToSerializableScene()).ToList();
            SaveHelper.SaveGame(saveName, new GameSave(CurrentSceneName, serializableScenes));
        }

        public void LoadGame(string saveName)
        {
            var save = SaveHelper.LoadGame(saveName);
            SavedScenes = save.SavedScenes.Select(ss => sceneFactory.CreateSceneFromSave(ss)).ToDictionary(ss => ss.Name);
            LoadScene(save.CurrentSceneName);
        }

        public void Draw(GameTime gameTime)
        {
            CurrentScene.Draw(gameTime);
        }

        public void Update(GameTime gameTime)
        {
            CurrentScene.Update(gameTime);
        }
    }
}
