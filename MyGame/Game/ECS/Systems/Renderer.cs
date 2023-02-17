using MyGame.Game.ECS.Components;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MyGame.Game.ECS.Systems
{
    /// <summary>
    /// Renderer system, used for rendering entities
    /// </summary>
    internal class Renderer : EcsSystem
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;

        private const float virtualWidth = 1366f;
        private const float virtualHeight = 768f;

        public Renderer(GraphicsDeviceManager graphics)
        {
            _graphics = graphics;
            _graphicsDevice = _graphics.GraphicsDevice;
            _spriteBatch = new SpriteBatch(_graphicsDevice);
        }

        public override void Draw(GameTime gameTime, IList<EcsEntity> entities)
        {
            _graphicsDevice.Clear(Color.Wheat);
            
            // find camera
            EcsEntity cameraEntity = null;
            try
            {
                cameraEntity = entities.SingleOrDefault(e => e.ContainsComponents<TopDownCamera>());
            }
            catch (InvalidOperationException) 
            {
                throw new ArgumentException("Cannot render while 2 or more cameras exist");
            }

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

            _spriteBatch.Begin(transformMatrix: cameraMatrix);
            foreach (var entity in entities.Where(e => e.ContainsComponents<Transform>()))
            {
                var transform = entity.GetComponent<Transform>();
                var position = new Vector2(transform.Position.X, -transform.Position.Y); // invert Y pos for camera


                if (entity.GetComponent<Image>() is Image image)
                {
                    _spriteBatch.Draw(image.Texture2D, position, null, Color.White, transform.Rotation,
                        new Vector2(image.Texture2D.Width / 2, image.Texture2D.Height / 2), transform.Scale, SpriteEffects.None, transform.ZIndex);
                }

                if (entity.GetComponent<Animation>() is Animation animation)
                {
                    // calculate frame
                    ulong rectIndex;
                    if (animation.PreviousStart is null)
                    {
                        rectIndex = 0;
                    }
                    else
                    {
                        rectIndex = Convert.ToUInt64((gameTime.TotalGameTime - animation.PreviousStart.Value).TotalSeconds * animation.Speed);
                    }

                    // if animation played one cycle
                    ulong framesCount = Convert.ToUInt64(animation.Frames.Length);
                    if (rectIndex >= framesCount)
                    {
                        if (animation.IsCycled)
                        {
                            rectIndex %= framesCount;
                        }
                        else
                        {
                            rectIndex = framesCount - 1;
                        }
                    }
                    
                    _spriteBatch.Draw(animation.Texture2D, position, animation.Frames[rectIndex], Color.White, transform.Rotation,
                        Vector2.Zero, transform.Scale, SpriteEffects.None, transform.ZIndex);
                }
            }
            _spriteBatch.End();
        }

        private static Matrix GetTransformMatrix(Viewport viewport, Transform cameraTransform, TopDownCamera camera)
        {
            float scaleX = viewport.Width / virtualWidth;
            float scaleY = viewport.Height / virtualHeight;

            float minScale = MathHelper.Min(scaleX, scaleY);

            return Matrix.CreateTranslation(new Vector3(-cameraTransform.Position, 0)) *
                Matrix.CreateRotationZ(cameraTransform.Rotation) *
                Matrix.CreateScale(camera.Zoom, camera.Zoom, 1) *
                // below translation could be virtualWidth / 2 and virtualHeight / 2
                Matrix.CreateTranslation(viewport.Width / minScale / 2f, viewport.Height / minScale / 2f, 0) *
                Matrix.CreateScale(minScale, minScale, 1f);
        }
    }
}
