using MyGame.Game.ECS.Systems.EventSystem.Events;

namespace MyGame.Game.ECS.Systems.EventSystem
{
    internal delegate bool EcsEventHandler<TEvent>(object sender, TEvent @event) where TEvent : EventBase;

    internal interface IEventSystem
    {
        /// <summary>
        /// Represents operation on the stack of event handlers.
        /// Every event should be propagated throug the stack until it's handled by one of handlers
        /// </summary>
        void Subscribe<TEvent>(EcsEventHandler<TEvent> handler) where TEvent : EventBase;

        /// <summary>
        /// Represents operation on the stack of event handlers.
        /// Every event should be propagated throug the stack until it's handled by one of handlers
        /// </summary>
        void Unsubscribe<TEvent>(EcsEventHandler<TEvent> handler) where TEvent : EventBase;

        /// <summary>
        /// Sends event to event handlers that are currently registered
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        void Emit<TEvent>(object sender, TEvent @event) where TEvent : EventBase;
    }
}
