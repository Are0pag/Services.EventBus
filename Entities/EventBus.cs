using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Services.EventBus
{
    static public class EventBus
    {
        // Где Type(Key) - это тип события, выраженный в интерфейсе, унаследованном от IGlobalSubscriber
        // а SubscribersList<IGlobalSubscriber> (Value) - это множество конкретных экземпляров классов, реализующих данный интерфейс (Key), а следовательно и реализующих ответ (реакцию) на событие
        static private readonly Dictionary<Type, SubscribersList<IGlobalSubscriber>> _subscribers = new();

        static public void Subscribe(IGlobalSubscriber subscriber) {
            var subscriberTypes = TypeExposer.GetSubscriberTypes(subscriber);
            foreach (var t in subscriberTypes) {
                // Если такое событие не зарегистрировано, то добавляется соответствующий ключ и структура данных для хранения ссылок экземпляры классов, реагирующих на данное событие
                if (!_subscribers.ContainsKey(t)) 
                    _subscribers[t] = new SubscribersList<IGlobalSubscriber>();
                // Добавляется ссылка на экземпляр (доступ к реализации (реакции))
                _subscribers[t].Add(subscriber);
            }
        }

        static public void Unsubscribe(IGlobalSubscriber subscriber) {
            var subscriberTypes = TypeExposer.GetSubscriberTypes(subscriber);
            foreach (var t in subscriberTypes) {
                if (_subscribers.ContainsKey(t))
                    _subscribers[t].Remove(subscriber);
            }
        }

        static public void RaiseEvent<TSubscriber>(Action<TSubscriber> action)
            where TSubscriber : class, IGlobalSubscriber 
        {
            // Получение вызванного события по ключу, которым выступает тип интерфейса
            var subscribers = _subscribers[typeof(TSubscriber)];

            subscribers.Executing = true;
            // Обращение к каждой реализации через ображение к ссылке экземпляра
            foreach (var subscriber in subscribers.List) {
                try {
                    action.Invoke(subscriber as TSubscriber);
                }
                catch (Exception e) {
#if UNITY_EDITOR
                    Debug.Log($"{e.Message}\nStackTrace: {e.StackTrace}");
#endif
                }
            }

            subscribers.Executing = false;
            subscribers.Cleanup();
        }

        static public void Reset() {
            _subscribers.Clear();
            TypeExposer.Reset();
        }
    }
}