using MyGame.Game.ECS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem.Events
{
    internal class PlayerDetectionEvent : EventBase
    {
        public override EventGroup EventGroup => EventGroup.GameEvent;

        private readonly bool isDetected;

        public bool IsDetected => isDetected;
        public bool IsLost => !isDetected;

        public EcsEntity Detector { get; }

        public PlayerDetectionEvent(GameTime gameTime, EcsEntity detector, bool isDetected) : base(gameTime)
        {
            Detector = detector;
            this.isDetected = isDetected;
        }
    }
}
