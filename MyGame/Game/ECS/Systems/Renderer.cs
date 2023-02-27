﻿using Microsoft.Xna.Framework.Graphics;
using MyGame.Game.Configuration;
using MyGame.Game.Constants;
using MyGame.Game.ECS.Components;
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
        private readonly IConfiguration _configuration;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;

        private const float virtualWidth = 384f;
        private const float virtualHeight = 216f;

        public Renderer(GraphicsDevice graphicsDevice, IConfiguration configuration)
        {
            _configuration = configuration;
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
                cameraEntity = entities.SingleOrDefault(e => e.ContainsComponent<TopDownCamera>());
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
            _spriteBatch.Begin(sortMode: SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp, transformMatrix: cameraMatrix);
            foreach (var entity in entities.Where(e => e.ContainsComponent<Transform>()))
            {
                var transform = entity.GetComponent<Transform>();
                // TODO fix Y axis invertion
                var position = new Vector2(transform.Position.X, -transform.Position.Y); // invert Y pos for camera

                // debug box collider
                if (_configuration.GetValue<bool>(ConfigurationConstants.ShowBoxColliders) && entity.TryGetComponent<BoxCollider>(out var boxCollider))
                {
                    _spriteBatch.DrawRectangle(new Rectangle(position.ToPoint() + boxCollider.Box.Location, boxCollider.Box.Size), Color.LightGreen, 1);
                }

                // debug ai intention
                if (_configuration.GetValue<bool>(ConfigurationConstants.ShowAiDebug) && entity.TryGetComponent<PlayerDetector>(out var playerDetector))
                {
                    var entityPos = entity.GetEntityCenter();
                    var playerPos = playerDetector.Player.GetEntityCenter();

                    _spriteBatch.DrawLine(new Vector2(entityPos.X, -entityPos.Y), new Vector2(playerPos.X, -playerPos.Y), Color.Red, 1);
                }

                if (entity.TryGetComponent<Image>(out var image))
                {
                    _spriteBatch.Draw(image.Texture2D, position, null, Color.White, transform.Rotation,
                        Vector2.Zero, transform.Scale, SpriteEffects.None, transform.ZIndex);
                }

                if (entity.TryGetComponent<Animation>(out var animation))
                {
                    if (animation.IsPlaying)
                    {
                        animation.TimePlayed += gameTime.ElapsedGameTime;
                        var duration = animation.GetStateDuration(animation.State);
                        if (animation.TimePlayed >= duration)
                        {
                            animation.TimePlayed -= duration;
                        }
                    }
                    else
                    {
                        animation.TimePlayed = TimeSpan.Zero;
                    }

                    var bounds = animation.GetCurrentBound();
                    var origin = Vector2.Zero; // new Vector2(bounds.Width / 2f, bounds.Height / 2f); 
                    var spriteEffect = animation.State.GetSpriteEffect();

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
