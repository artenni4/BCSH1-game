using MyGame.Game.ECS.Entities;
using MyGame.Game.SerializableTypes;
using MyGame.Game.StateMachine;
using SharpDX.WIC;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace MyGame.Game.ECS.Components
{
    internal abstract class EcsComponent
    {
        protected EcsComponent()
        {
        }
        
        protected EcsComponent(EcsEntity entity)
        {
            Entity = entity;
        }
        /// <summary>
        /// Owner entity of the component
        /// </summary>
        public EcsEntity Entity { get; init; }


        public virtual SerializableComponent ToSerializableComponent() =>
            new()
            {
                ComponentType = GetType().FullName,
                Data = SaveData()
            };

        public void InitializeDeserialized(SerializableComponent serializableComponent)
        {
            if (serializableComponent.Data is not null)
            {
                LoadData(serializableComponent.Data);
            }
        }

        private protected virtual void LoadData(Dictionary<string, object> data)
        {
            var componentType = GetType();
            foreach (var (propName, value) in data)
            {
                componentType.GetProperty(propName).SetValue(this, DeserializableValue(value));
            }
        }

        private protected virtual Dictionary<string, object> SaveData()
        {
            var componentType = GetType();
            var properties = componentType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead && p.CanWrite);
            var data = new Dictionary<string, object>();
            foreach (var property in properties.Where(p => p.Name != nameof(Entity)))
            {
                if (TrySerializableValue(property, out var value))
                {
                    data[property.Name] = value;
                }
            }

            return data;
        }

        private protected virtual bool TrySerializableValue(PropertyInfo propertyInfo, out object value)
        {
            if (propertyInfo.PropertyType == typeof(Vector2?) || propertyInfo.PropertyType == typeof(Vector2))
            {
                var vec = (Vector2?)propertyInfo.GetValue(this, null);
                value = vec.HasValue ? new SerializableVector(vec.Value) : (object)null;
            }
            else if (propertyInfo.PropertyType == typeof(Rectangle?) || propertyInfo.PropertyType == typeof(Rectangle))
            {
                var rect = (Rectangle?)propertyInfo.GetValue(this, null);
                value = rect.HasValue ? new SerializableRectangle(rect.Value) : (object)null;
            }
            else if (propertyInfo.PropertyType == typeof(Texture2D))
            {
                value = null;
                return false;
            }
            else
            {
                value = propertyInfo.GetValue(this, null);
            }
            return true;
        }

        private protected virtual object DeserializableValue(object value)
        {
            if (value is SerializableVector vector)
            {
                return vector.ToVector2();
            }
            else if (value is SerializableRectangle rectangle)
            {
                return rectangle.ToRectangle();
            }
            else
            {
                return value;
            }
        }
    }
}
