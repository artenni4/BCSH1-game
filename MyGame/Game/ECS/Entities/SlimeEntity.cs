﻿using MyGame.Game.Animators;
using MyGame.Game.Constants.Enums;
using MyGame.Game.Constants;
using MyGame.Game.ECS.Components.Animation;
using MyGame.Game.ECS.Components;
using MyGame.Game.StateMachine;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace MyGame.Game.ECS.Entities
{
    internal class SlimeEntity : EcsEntity
    {
        public float Speed { get; set; } = 40f;

        public float healthPoints = 100f;
        public float HealthPoints
        {
            get => healthPoints;
            set => healthPoints = Math.Clamp(value, 0f, 100f);
        }

        public bool IsDead => HealthPoints <= 0f;

        public StateMachine<AiState> StateMachine { get; }

        public Transform Transform { get; }

        public BoxCollider BoxCollider { get; }

        public Animation Animation { get; }

        public PlayerDetector PlayerDetector { get; }

        public SlimeEntity()
        {
            Transform = AddComponent<Transform>();
            Transform.ZIndex = 1f;

            BoxCollider = AddComponent<BoxCollider>();
            BoxCollider.Box = new Rectangle(10, 13, 13, 10);

            Animation = AddComponent<Animation>();
            Animation.Animator = new SlimeAnimator();

            PlayerDetector = AddComponent<PlayerDetector>();
            PlayerDetector.DetectionRadius = 100f;

            StateMachine = new StateMachineBuilder<AiState>()
                .State(AiState.WalkAround)
                    .TransitionTo(AiState.ChasePlayer).OnTrigger(AiTriggers.PlayerDetected)
                .State(AiState.ChasePlayer)
                    .TransitionTo(AiState.WalkAround).OnTrigger(AiTriggers.PlayerLost)
                .BuildStateMachine(AiState.WalkAround);
        }

        public override void LoadContent(ContentManager contentManager)
        {
            GetComponent<Animation>().Texture2D = contentManager.Load<Texture2D>("sprites/characters/slime1");
        }
    }
}
