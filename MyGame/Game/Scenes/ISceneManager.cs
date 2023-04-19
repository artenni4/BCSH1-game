using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.Scenes
{
    internal interface ISceneManager
    {
        public string CurrentSceneName { get; }
        public void LoadScene(string sceneName);
        public void SaveGame(string saveName);
        public void LoadGame(string saveName);

        public void Update(GameTime gameTime);
        public void Draw(GameTime gameTime);
    }
}
