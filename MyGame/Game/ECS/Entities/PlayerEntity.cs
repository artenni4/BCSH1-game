using Microsoft.Xna.Framework.Content;
using MyGame.Game.Constants;
using MyGame.Game.Constants.Enums;
using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Components.Animation;
using MyGame.Game.ECS.Components.Attack;
using MyGame.Game.ECS.Systems;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.StateMachine;
using System.Linq;

namespace MyGame.Game.ECS.Entities
{
    internal class PlayerEntity : EcsEntity
    {
        public Transform Transform { get; }
        public BoxCollider BoxCollider { get; }
        public PlayerAnimation Animation { get; }
        public EntityHealth EntityHealth { get; }
        public MeleeAttackComponent MeleeAttackComponent { get; }
        public PlayerComponent PlayerComponent { get; }

        private readonly IEventSystem _eventSystem;
        private KeyboardState _lastKeyboardState;

        public PlayerEntity(IEventSystem eventSystem)
        {
            Transform = AddComponent<Transform>();
            Transform.ZIndex = ZIndex.Middleground;

            BoxCollider = AddComponent<BoxCollider>();
            BoxCollider.Box = new Rectangle(17, 22, 15, 19);

            Animation = AddComponent<PlayerAnimation>();

            EntityHealth = AddComponent<EntityHealth>();
            EntityHealth.DamageCooldown = TimeSpan.FromSeconds(1.5f);
            EntityHealth.MaxHealth = 100f;
            EntityHealth.HealthPoints = 100f;

            MeleeAttackComponent = AddComponent<MeleeAttackComponent>();
            MeleeAttackComponent.AttackDuration = PlayerAnimation.AttackLeftNode.Duration;
            MeleeAttackComponent.Faction = AttackFactions.PlayerFaction;
            MeleeAttackComponent.Cooldown = TimeSpan.FromSeconds(1);
            MeleeAttackComponent.DamageDealtThreshhold = 0.25f;
            MeleeAttackComponent.Range = 30f;
            MeleeAttackComponent.DamageAmount = 20f;

            PlayerComponent = AddComponent<PlayerComponent>();
            PlayerComponent.Speed = 100f;

            _eventSystem = eventSystem;
            _eventSystem.Subscribe<DamageEvent>(OnPlayerDamaged);
            _eventSystem.Subscribe<KeyboardEvent>(OnKeyboardEvent);
            _eventSystem.Subscribe<MouseEvent>(OnMouseEvent);

            Animation.StateMachine.StateChanged += StateMachine_StateChanged;
        }

        private void StateMachine_StateChanged(object sender, TransitionEventArgs<AnimationNode> e)
        {
            if (IsAttackAnimation())
            {
                MeleeAttackComponent.AttackDirection = GetAttackDirection();
            }
        }

        public override void LoadContent(ContentManager contentManager)
        {
            Animation.Texture2D = contentManager.Load<Texture2D>("sprites/characters/player");
        }

        private bool OnPlayerDamaged(object sender, DamageEvent damageEvent)
        {
            if (damageEvent.Target != this)
            {
                return false;
            }

            if (EntityHealth.IsDead)
            {
                BoxCollider.IsKinematic = false;
                _eventSystem.Emit(this, new PlayerDeathEvent(damageEvent.GameTime, this));
            }
            return false;
        }

        private bool OnKeyboardEvent(object sender, KeyboardEvent keyboardEvent)
        {
            _lastKeyboardState = keyboardEvent.KeyboardState;

            var input = InputHelper.GetInputAxisNormalized(_lastKeyboardState);
            var handled = input != Vector2.Zero; // if player moves
            Animation.StateMachine.SetDirectionVector(input);

            return handled;
        }

        private bool OnMouseEvent(object sender, MouseEvent mouseEvent)
        {
            if (mouseEvent.MouseState.LeftButton == ButtonState.Pressed)
            {
                _eventSystem.Emit(this, new AttackInitiationEvent(mouseEvent.GameTime, this));
                return true;
            }
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            if (IsMovable())
            {
                var input = InputHelper.GetInputAxisNormalized(_lastKeyboardState);
                Transform.Position += input * PlayerComponent.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        private bool IsMovable()
        {
            AnimationNode state = Animation.StateMachine.State;
            return state == PlayerAnimation.WalkDownNode ||
                state == PlayerAnimation.WalkUpNode ||
                state == PlayerAnimation.WalkRightNode ||
                state == PlayerAnimation.WalkLeftNode;
        }

        public bool IsAttackAnimation()
        {
            AnimationNode state = Animation.StateMachine.State;
            return state == PlayerAnimation.AttackDownNode ||
                state == PlayerAnimation.AttackUpNode ||
                state == PlayerAnimation.AttackRightNode ||
                state == PlayerAnimation.AttackLeftNode;
        }

        private MeleeAttackDirection GetAttackDirection()
        {
            AnimationNode state = Animation.StateMachine.State;
            if (state == PlayerAnimation.AttackDownNode)
            {
                return MeleeAttackDirection.Down;
            }
            else if (state == PlayerAnimation.AttackUpNode)
            {
                return MeleeAttackDirection.Up;
            }
            else if (state == PlayerAnimation.AttackLeftNode)
            {
                return MeleeAttackDirection.Left;
            }
            else if (state == PlayerAnimation.AttackRightNode)
            {
                return MeleeAttackDirection.Right;
            }
            throw new ArgumentException($"{nameof(state)} is not attack animation state");
        }
    }
}
