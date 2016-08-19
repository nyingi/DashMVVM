using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DashMvvm.Ioc
{
    public static class DashResolver
    {
        
        private static List<IocRegistration> _registrations = new List<IocRegistration>(); 
        public static TInterface Resolve<TInterface>()
        {
            var registration =
                _registrations
                    .FirstOrDefault(a => a.ResolutionExpression.Keys.FirstOrDefault() == typeof (TInterface));
            if (registration == null)
            {
                return default(TInterface);
            }
            if (registration.IsSingleton && registration.NewestInstance != null)
            {
                return (TInterface) registration.NewestInstance;
            }
            var expression = registration.ResolutionExpression[typeof (TInterface)] as Expression<Func<TInterface>>;
            if (expression != null)
            {
                registration.NewestInstance = expression.Compile().Invoke();
            }
            else
            {
                return default(TInterface);
            }
            return (TInterface) registration.NewestInstance;
        }

        public static void Register<TInterface>(
            Expression<Func<TInterface>> registrationExpression,bool isSingleton)
        {
            _registrations =
                _registrations.Where(a => a.ResolutionExpression.Keys.FirstOrDefault() != typeof (TInterface)).ToList();

            var reg = new IocRegistration
            {
                ResolutionExpression = new Dictionary<Type, object>()
                {
                    {typeof (TInterface), registrationExpression}
                },
                IsSingleton = isSingleton
            };
            _registrations.Add(reg);
        }
    }
}