using System;
using System.Collections.Generic;
using System.Linq;

namespace Scripts.Services.EventBus
{
    static public class TypeExposer
    {
        // где Type - тип подписчика
        // а List<Type> - типы интерфейсов, унаследованных от IGlobalSubscriber
        static private readonly Dictionary<Type, List<Type>> _cashedSubscriberTypes = new();

        // Осуществляет отбор тех типов интерфейсов, реализующих полученным экземпляром класса, которые необходимы
        static public List<Type> GetSubscriberTypes(IGlobalSubscriber globalSubscriber) {
            var type = globalSubscriber.GetType();
            if (_cashedSubscriberTypes.ContainsKey(type)) 
                return _cashedSubscriberTypes[type];

            var subscriberTypes = type
                // Так как в функцию отправлена ссылка на экземпляр класса, реализующего заданный в параметрах интерфейс, необходимо получить информацию об этом интерфейсе
                // В локальную переменную присваивается массив всех интерфейсов, реализующихся данным экземпляром
                .GetInterfaces()
                // Перебор полученных типов интерфейсов для поиска необходимого, который в данном случае выспутает тип интерфейса, унаследованного от IGlobalSubscriber
                .Where(@interface => @interface.GetInterfaces().Contains(typeof(IGlobalSubscriber)))
                .ToList();

            _cashedSubscriberTypes[type] = subscriberTypes;
            return subscriberTypes;
        }

        static public void Reset() {
            _cashedSubscriberTypes.Clear();
        }
    }
}