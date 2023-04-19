using Microsoft.Extensions.DependencyInjection;
using MyGame.Game.Constants;
using MyGame.Game.Constants.Enums;
using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Components.HUD;
using MyGame.Game.ECS.Entities;
using MyGame.Game.ECS.Systems;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.Scenes;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Linq;

namespace MyGame.Game.Factories
{
    internal class SceneFactory
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SceneFactory(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public SceneBase CreateSceneFromSave(SerializableScene serializableScene)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            var scene = serviceProvider.GetService<SceneBase>();
            scene.Name = serializableScene.Name;

            var systems = serializableScene.Systems.Select(ss =>
            {
                var system = (EcsSystem)serviceProvider.GetRequiredService(Type.GetType(ss.SystemType));
                system.InitializeDeserialized(ss);
                return system;
            }).ToArray();
            scene.AddSystems(systems);
            
            var entities = serializableScene.Entities.Select(se =>
            {
                var entity = (EcsEntity)serviceProvider.GetRequiredService(Type.GetType(se.EntityType));
                entity.InitializeDeserialized(se);
                return entity;
            }).ToArray();
            scene.AddEntities(entities);

            return scene;

        }


        /// <summary>
        /// Creates scene from xml map definition
        /// </summary>
        /// <param name="sceneName">name of scene without extensions</param>
        /// <returns>scene instance</returns>
        public SceneBase CreateSceneFromXml(string sceneName)
        {
            var filepath = Path.Combine(PersistenceConstants.MapsFolder, Path.ChangeExtension(sceneName, PersistenceConstants.MapExtension));
            var document = XDocument.Load(filepath);

            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            var scene = serviceProvider.GetRequiredService<SceneBase>();
            scene.Name = sceneName;
            var systems = document.Root.Elements("Systems").First().Elements()
                .Select(s => (EcsSystem)serviceProvider.GetRequiredService(FindType(s.Name.NamespaceName, s.Name.LocalName))).ToArray();
            scene.AddSystems(systems);

            var entities = document.Root.Elements("Entities").First().Elements().Select(xEntity =>
            {
                var entity = (EcsEntity)serviceProvider.GetRequiredService(FindType(xEntity.Name.NamespaceName, xEntity.Name.LocalName));
                var entityProperties = xEntity.Elements();
                SetupEntity(entity, entityProperties);
                return entity;
            }).ToArray();
            scene.AddEntities(entities);

            AddGrass(scene, serviceProvider); // TODO maybe consider other way of adding decor
            return scene;
        }

        private void AddGrass(SceneBase scene, IServiceProvider serviceProvider)
        {
            var rand = new Random(scene.Name.GetHashCode()); // init random with scene name
            int grassCount = rand.Next(60, 100);
            for (int i = 0; i < grassCount; i++)
            {
                var grass = serviceProvider.GetRequiredService<GrassEntity>();
                grass.GrassVariation = (GrassVariation)rand.Next((int)GrassVariation.Fourth + 1);
                grass.Transform.Position = new Vector2(rand.NextSingle() * 600 - 300, rand.NextSingle() * 600 - 300);
                grass.Transform.ZIndex = ZIndex.Background;
                scene.AddEntities(grass);
            }
        }

        private static void SetupEntity(EcsEntity entity, IEnumerable<XElement> properties)
        {
            foreach (var property in properties)
            {
                if (property.Name.LocalName == "Position")
                {
                    var x = float.Parse(property.Attribute("X").Value);
                    var y = float.Parse(property.Attribute("Y").Value);
                    entity.GetComponent<Transform>().Position = new Vector2(x, y);
                }
                else if (property.Name.LocalName == "ScreenPosition")
                {
                    var x = float.Parse(property.Attribute("X").Value);
                    var y = float.Parse(property.Attribute("Y").Value);
                    entity.GetComponent<HUDElement>().ScreenPosition = new Vector2(x, y);
                }
                else if (entity is SlimeEntity slime && property.Name.LocalName == "Strength")
                {
                    var strength = Enum.Parse<SlimeStrength>(property.Value);
                    slime.Strength = strength;
                }
            }
        }

        private static Type FindType(string namespaceUri, string localName)
        {
            var assembly = typeof(EcsSystem).Assembly;
            var namespaceMapping = new Dictionary<string, string>
            {
                { "Systems", "MyGame.Game.ECS.Systems" },
                { "Entities", "MyGame.Game.ECS.Entities" }
            };

            return assembly.GetType($"{namespaceMapping[namespaceUri]}.{localName}");
        }
    }
}
