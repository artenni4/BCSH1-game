using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.Scenes
{
    [Serializable]
    internal record GameSave(string CurrentSceneName, List<SerializableScene> SavedScenes);
}
