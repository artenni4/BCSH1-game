using Microsoft.Xna.Framework.Graphics;
using MyGame.Game.ECS.Components;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MyGame.Game.ECS.Systems
{
    /// <summary>
    /// Renderer system, used for rendering entities
    /// </summary>
    internal class Renderer : EcsSystem
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;

        private const float virtualWidth = 1366f;
        private const float virtualHeight = 768f;

        public Renderer(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(_graphicsDevice);
        }

        public override void Draw(GameTime gameTime, ICollection<EcsEntity> entities)
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

            // NOTE: SamplerState.PointClamp - pixelates textures on scale
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: cameraMatrix);
            foreach (var entity in entities.Where(e => e.ContainsComponents<Transform>()))
            {
                var transform = entity.GetComponent<Transform>();
                var position = new Vector2(transform.Position.X, -transform.Position.Y); // invert Y pos for camera


                if (entity.TryGetComponent<Image>(out var image))
                {
                    _spriteBatch.Draw(image.Texture2D, position, null, Color.White, transform.Rotation,
                        new Vector2(image.Texture2D.Width / 2, image.Texture2D.Height / 2), transform.Scale, SpriteEffects.None, transform.ZIndex);
                }

                if (entity.TryGetComponent<Animation>(out var animation))
                {
                    if (animation.IsPlaying)
                    {
                        animation.TimePlayed += gameTime.ElapsedGameTime;
                    }
                    else
                    {
                        animation.TimePlayed = TimeSpan.Zero;
                    }

                    // calculate frame
                    ulong rectIndex = Convert.ToUInt64(animation.TimePlayed.TotalSeconds * animation.Speed);

                    // if animation played one cycle
                    ulong framesCount = Convert.ToUInt64(animation.StateFrames.Length);
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

                    var bounds = animation.StateFrames[rectIndex];
                    var origin = new Vector2(bounds.Width / 2f, bounds.Height / 2f); 
                    var spriteEffect = animation.FlipHorizontally ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                    _spriteBatch.Draw(animation.Texture2D, position, bounds, Color.White, transform.Rotation,
                        origin, transform.Scale, spriteEffect, transform.ZIndex);
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
