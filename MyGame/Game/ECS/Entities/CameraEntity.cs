using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.Scenes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MyGame.Game.ECS.Entities
{
    internal class CameraEntity : EcsEntity
    {
        private readonly IEntityCollection _entityCollection;

        public Transform Transform { get; }
        public TopDownCamera TopDownCamera { get; }
        public AudioSourceComponent AudioSource { get; }

        public CameraEntity(IEntityCollection entityCollection, IEventSystem eventSystem)
        {
            _entityCollection = entityCollection;

            Transform = AddComponent<Transform>();
            TopDownCamera = AddComponent<TopDownCamera>();
            TopDownCamera.Zoom = 1f;

            AudioSource = AddComponent<AudioSourceComponent>();

            eventSystem.Subscribe<MouseEvent>(OnCameraZoom);
            eventSystem.Subscribe<KeyboardEvent>(OnMusicToggle);
        }

        private bool OnCameraZoom(object sender, MouseEvent mouseEvent)
        {
            TopDownCamera.Zoom = Math.Clamp(TopDownCamera.Zoom + mouseEvent.ScrollDelta / (10 * 120f), 0.3f, 5);
            return true;
        }

        private bool OnMusicToggle(object sender, KeyboardEvent keyboardEvent)
        {
            if (keyboardEvent.PressedKeys.Contains(Keys.M))
            {
                MediaPlayer.IsMuted = !MediaPlayer.IsMuted;
                return true;
            }
            return false;
        }

        public override void LoadContent(ContentManager contentManager)
        {
            AudioSource.Song = contentManager.Load<Song>("Music/The Field Of Dreams");
            MediaPlayer.Volume = 0.3f;
            MediaPlayer.Play(AudioSource.Song);
        }

        public override void Update(GameTime gameTime)
        {
            // follow player
            Transform.Position = _entityCollection.Entities.FirstOrDefault(e => e is PlayerEntity).GetEntityCenter();
        }
    }
}
