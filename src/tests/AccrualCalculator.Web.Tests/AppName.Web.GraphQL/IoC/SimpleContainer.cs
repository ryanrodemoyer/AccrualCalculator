using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AppTests.AppName.Web.GraphQL
{
    public class SimpleContainer : ISimpleContainer
    {
        private readonly Dictionary<Type, Func<object>> _registrations = new Dictionary<Type, Func<object>>();

        public void Register<TService>()
        {
            Register<TService, TService>();
        }

        public void Register<TService, TImpl>() where TImpl : TService
        {
            _registrations.Add(typeof(TService),
                () =>
                {
                    var implType = typeof(TImpl);
                    return typeof(TService) == implType
                        ? CreateInstance(implType)
                        : Get(implType);
                });
        }

        public void Register<TService>(Func<TService> instanceCreator)
        {
            _registrations.Add(typeof(TService), () => instanceCreator());
        }

        public void Singleton<TService>(TService instance)
        {
            _registrations.Add(typeof(TService), () => instance);
        }

        public void Singleton<TService>(Func<TService> instanceCreator)
        {
            var lazy = new Lazy<TService>(instanceCreator);
            Register(() => lazy.Value);
        }
        
        public void SwapSingleton<TService>(TService instance)
        {
            _registrations[typeof(TService)] = () => instance;
        }

        public void SwapSingleton<TService>(Func<TService> instanceCreator)
        {
            var lazy = new Lazy<TService>(instanceCreator);
            _registrations[typeof(TService)] = () => lazy.Value;
        }

        public T Get<T>()
        {
            return (T) Get(typeof(T));
        }

        public object Get(Type serviceType)
        {
            if (_registrations.TryGetValue(serviceType, out var creator))
            {
                return creator();
            }

            if (!serviceType.GetTypeInfo().IsAbstract)
            {
                return CreateInstance(serviceType);
            }

            throw new InvalidOperationException("No registration for " + serviceType);
        }

        public void Dispose()
        {
            _registrations.Clear();
        }

        private object CreateInstance(Type implementationType)
        {
            var ctor = implementationType.GetConstructors().OrderByDescending(x => x.GetParameters().Length).First();
            var parameterTypes = ctor.GetParameters().Select(p => p.ParameterType);
            var dependencies = parameterTypes.Select(Get).ToArray();
            return Activator.CreateInstance(implementationType, dependencies);
        }
    }
}