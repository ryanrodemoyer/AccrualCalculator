using System;

namespace AppTests.AppName.Web.GraphQL
{
    public interface ISimpleContainer : IDisposable
    {
        object Get(Type serviceType);
        T Get<T>();
        void Register<TService>();
        void Register<TService>(Func<TService> instanceCreator);
        void Register<TService, TImpl>() where TImpl : TService;
        void Singleton<TService>(TService instance);
        void Singleton<TService>(Func<TService> instanceCreator);
        void SwapSingleton<TService>(TService instance);
        void SwapSingleton<TService>(Func<TService> instanceCreator);
    }
}