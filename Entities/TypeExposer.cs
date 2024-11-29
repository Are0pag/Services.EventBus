using System;
using System.Collections.Generic;
using System.Linq;

namespace Scripts.Services.EventBus
{
    static public class TypeExposer<TBaseModuleType>
    {
        static private readonly Dictionary<Type, List<Type>> _cashedSubscriberTypes = new();
        
        /// <summary>
        /// Find all types, that derived from TBaseModuleType
        /// </summary>
        static public List<Type> GetSubscriberTypes(TBaseModuleType globalSubscriber) {
            var type = globalSubscriber.GetType();
            if (_cashedSubscriberTypes.TryGetValue(type, out var types)) 
                return types;

            var subscriberTypes = type
                .GetInterfaces()
                .Where(@interface => @interface.GetInterfaces().Contains(typeof(TBaseModuleType)))
                .ToList();

            _cashedSubscriberTypes[type] = subscriberTypes;
            return subscriberTypes;
        }
    }
}