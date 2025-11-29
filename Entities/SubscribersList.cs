using System;
using System.Collections.Generic;

namespace Scripts.Services.EventBus
{
    public class SubscribersList<TSubscriber>
        where TSubscriber : class
    {
        public readonly List<TSubscriber> List = new();
        public Type InvokerType { get; set; }
        private bool _needsCleanUp;

        public bool Executing;

        public void SetInvoker(Type invokerType) {
            if (invokerType != null) 
                InvokerType ??= invokerType;
        }

        public void Add(TSubscriber subscriber) {
            List.Add(subscriber);
        }

        public void Remove(TSubscriber subscriber) {
            if (Executing) {
                var subscriberIndex = List.IndexOf(subscriber);
                
                if (subscriberIndex < 0) 
                    return;
                
                _needsCleanUp = true;
                List[subscriberIndex] = null;
            }
            else
                List.Remove(subscriber);
        }

        public void Cleanup() {
            if (!_needsCleanUp) 
                return;

            List.RemoveAll(s => s == null);
            _needsCleanUp = false;
        }
    }
}