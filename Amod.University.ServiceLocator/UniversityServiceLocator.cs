using System;
using Castle.Core.Resource;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using MSServiceLocator = Microsoft.Practices.ServiceLocation;
using Amod.University.ServiceLocator;

namespace Amod.University.ServiceLocator
{
    public sealed class UniversityServiceLocator
    {
        #region Instance Members

        private static readonly Lazy<UniversityServiceLocator> _instance = new Lazy<UniversityServiceLocator>(() => new UniversityServiceLocator(), true);

        #endregion


        #region Ctor

        static UniversityServiceLocator()
        {
            try
            {
                MSServiceLocator.IServiceLocator injector =
                    new WindsorServiceLocator(
                        new WindsorContainer(
                            new XmlInterpreter(
                                new ConfigResource("DI.Components"))));

                Microsoft.Practices.ServiceLocation.ServiceLocator.SetLocatorProvider(() => injector);
            }
            catch (MSServiceLocator.ActivationException ieException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw;
            }

        }

        #endregion

        #region Instance/Ctor

        public static UniversityServiceLocator Instance
        {
            get
            {
                return _instance.Value;
            }

        }

        #endregion

        #region Public Methods

        public T GetInstance<T>(string identifier = null)
        {
            try
            {
                if (string.IsNullOrEmpty(identifier))
                    return MSServiceLocator.ServiceLocator.Current.GetInstance<T>();
                return MSServiceLocator.ServiceLocator.Current.GetInstance<T>(identifier);
            }
            catch (MSServiceLocator.ActivationException wException)
            {
                throw;
            }

        }

        #endregion
    }
}
