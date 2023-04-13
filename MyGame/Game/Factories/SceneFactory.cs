using Microsoft.Extensions.DependencyInjection;
using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Entities;
using MyGame.Game.ECS.Systems;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.Scenes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        // TODO create scene from save

        public SceneBase CreateScene(XDocument document)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            var scene = serviceProvider.GetRequiredService<SceneBase>();
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

            return scene;
        }

        private void SetupEntity(EcsEntity entity, IEnumerable<XElement> properties)
        {
            foreach (var property in properties)
            {
                if (property.Name.LocalName == "Position")
                {
                    var x = float.Parse(property.Attribute("X").Value);
                    var y = float.Parse(property.Attribute("Y").Value);
                    entity.GetComponent<Transform>().Position = new Vector2(x, y);
                }
                else if (entity is SlimeEntity slime && property.Name.LocalName == "Strength")
                {
                    var strength = Enum.Parse<SlimeEntity.Strength>(property.Value);
                    slime.SlimeStrength = strength;
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
