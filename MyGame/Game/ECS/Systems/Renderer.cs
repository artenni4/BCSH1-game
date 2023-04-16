using MyGame.Game.Configuration;
using MyGame.Game.Constants;
using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Components.Animation;
using MyGame.Game.ECS.Entities;
using MyGame.Game.Scenes;
using System.Data;
using System.Linq;

namespace MyGame.Game.ECS.Systems
{
    /// <summary>
    /// Renderer system, used for rendering entities
    /// </summary>
    internal class Renderer : EcsSystem
    {
        private readonly IConfiguration _configuration;
        private readonly IEntityCollection _entityCollection;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;

        private const float virtualWidth = 384f;
        private const float virtualHeight = 216f;

        public Renderer(GraphicsDevice graphicsDevice, IEntityCollection entityCollection, IConfiguration configuration)
            : base(entityCollection)
        {
            _configuration = configuration;
            _entityCollection = entityCollection;
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(_graphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            _graphicsDevice.Clear(new Color(80, 155, 102));
            
            // find camera
            CameraEntity cameraEntity = GetEntityOfType<CameraEntity>();

            if (cameraEntity is null)
            {
                _graphicsDevice.Clear(Color.Black);
                return;
            }

            // render entities
            var camera = cameraEntity.GetComponent<TopDownCamera>();
            var cameraTransform = cameraEntity.GetComponent<Transform>();
            var viewport = _graphicsDevice.Viewport;

            // create transform matrix for a camera, basically how camera sees the world
            var cameraMatrix = GetTransformMatrix(viewport, cameraTransform, camera);

            // NOTE: SamplerState.PointClamp - pixelates textures on scale
            _spriteBatch.Begin(sortMode: SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp, transformMatrix: cameraMatrix);
            foreach (var entity in _entityCollection.Entities.Where(e => e.HasComponent<Transform>()))
            {
                var transform = entity.GetComponent<Transform>();
                // TODO fix Y axis invertion
                var position = new Vector2(transform.Position.X, -transform.Position.Y); // invert Y pos for camera

                if (entity.TryGetComponent<Image>(out var image))
                {
                    _spriteBatch.Draw(image.Texture2D, position, image.SourceRectangle, Color.White, transform.Rotation,
                        Vector2.Zero, transform.Scale, SpriteEffects.None, transform.ZIndex);
                }

                if (entity.TryGetComponent<Animation>(out var animation))
                {
                    animation.StateMachine.Update(gameTime.ElapsedGameTime);
                    var animationData = animation.GetAnimationData();

                    _spriteBatch.Draw(animation.Texture2D, position, animationData.Bounds, Color.White, transform.Rotation,
                        animationData.Origin, transform.Scale, animationData.SpriteEffects, transform.ZIndex);
                }

                DrawDebug(position, transform.Scale, entity);
            }
            _spriteBatch.End();
        }

        private void DrawDebug(Vector2 position, float scale, EcsEntity entity)
        {
            // TODO extract debug to separate renderer
            // debug box collider
            if (_configuration.GetValue<bool>(ConfigurationConstants.ShowBoxColliders) && entity.TryGetComponent<BoxCollider>(out var boxCollider))
            {
                _spriteBatch.DrawRectangle(
                    position.X + boxCollider.Box.X,
                    position.Y + boxCollider.Box.Y,
                    boxCollider.Box.Width * scale,
                    boxCollider.Box.Height * scale,
                    Color.LightGreen, 1);
            }

            // debug ai intention
            if (_configuration.GetValue<bool>(ConfigurationConstants.ShowAiDebug) && entity.TryGetComponent<PlayerDetector>(out var _))
            {
                var player = GetEntityOfType<PlayerEntity>();
                if (player is null)
                {
                    return;
                }

                var entityPos = entity.GetEntityCenter();
                var playerPos = player.GetEntityCenter();

                _spriteBatch.DrawLine(new Vector2(entityPos.X, -entityPos.Y), new Vector2(playerPos.X, -playerPos.Y), Color.Red, 1);
            }
        }

        private static Matrix GetTransformMatrix(Viewport viewport, Transform cameraTransform, TopDownCamera camera)
        {
            float scaleX = viewport.Width / virtualWidth;
            float scaleY = viewport.Height / virtualHeight;

            float minScale = MathHelper.Min(scaleX, scaleY);

            // that Y axis invertion needs to be fixed...
            var cameraPosition = new Vector2(-cameraTransform.Position.X, cameraTransform.Position.Y);
            return Matrix.CreateTranslation(new Vector3(cameraPosition, 0)) *
                Matrix.CreateRotationZ(cameraTransform.Rotation) *
                Matrix.CreateScale(camera.Zoom, camera.Zoom, 1) *
                // below translation could be virtualWidth / 2 and virtualHeight / 2
                Matrix.CreateTranslation(viewport.Width / minScale / 2f, viewport.Height / minScale / 2f, 0) *
                Matrix.CreateScale(minScale, minScale, 1f);
        }
    }
}
