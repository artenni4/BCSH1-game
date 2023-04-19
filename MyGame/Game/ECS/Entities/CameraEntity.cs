using Microsoft.Xna.Framework.Content;
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

        public CameraEntity(IEntityCollection entityCollection, IEventSystem eventSystem)
        {
            _entityCollection = entityCollection;

            Transform = AddComponent<Transform>();
            TopDownCamera = AddComponent<TopDownCamera>();
            TopDownCamera.Zoom = 1f;

            eventSystem.Subscribe<MouseEvent>(OnCameraZoom);
        }

        private bool OnCameraZoom(object sender, MouseEvent mouseEvent)
        {
            TopDownCamera.Zoom = Math.Clamp(TopDownCamera.Zoom + mouseEvent.ScrollDelta / (10 * 120f), 0.3f, 5);
            return true;
        }

        public override void Update(GameTime gameTime)
        {
            // follow player
            Transform.Position = _entityCollection.Entities.FirstOrDefault(e => e is PlayerEntity).GetEntityCenter();
        }
    }
}
