namespace Amod.University.ServiceLocator
{
    using System;
    using System.Collections.Generic;
    using Castle.Windsor;
    using Microsoft.Practices.ServiceLocation;

    public class WindsorServiceLocator : ServiceLocatorImplBase
    {
        private readonly IWindsorContainer _container;

        public WindsorServiceLocator(IWindsorContainer container)
        {
            this._container = container;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (key != null)
                return _container.Resolve(key, serviceType);
            return _container.Resolve(serviceType);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return (object[])_container.ResolveAll(serviceType);
        }
    }
}
