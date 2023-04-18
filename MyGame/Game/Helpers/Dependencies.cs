using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyGame.Game.Configuration;
using MyGame.Game.ECS.Entities;
using MyGame.Game.ECS.Systems;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.Scenes;
using System.Linq;

namespace MyGame.Game.Helpers
{
    internal static class Dependencies
    {
        public static IServiceCollection AddGameServices(this IServiceCollection services)
        {
            // add other dependecies
            services.AddSingleton<IConfiguration, ConfigurationStorage>();
            services.AddLogging(config =>
            {
                config.SetMinimumLevel(LogLevel.Trace);
                config.AddDebug();
            });

            services.AddScoped<IEventSystem, EventSystem>();
            services.AddScoped<SceneBase>();
            services.AddScoped<IEntityCollection>(sp => sp.GetRequiredService<SceneBase>());
            services.AddScoped<ISystemCollection>(sp => sp.GetRequiredService<SceneBase>());

            var assemblyTypes = typeof(MyGame).Assembly.GetTypes();
            // add systems
            var systems = assemblyTypes.Where(t => t.IsSubclassOf(typeof(EcsSystem)) && !t.IsAbstract);
            foreach (var system in systems)
            {
                services.AddScoped(system);
            }

            // add entities
            var entities = assemblyTypes.Where(t => t.IsSubclassOf(typeof(EcsEntity)) && !t.IsAbstract);
            foreach (var entity in entities)
            {
                services.AddTransient(entity);
            }
            return services;
        }
    }
}
