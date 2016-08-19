using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DashMvvm.Ioc
{
    internal class IocRegistration
    {
        public bool IsSingleton { get; set; }

        public Dictionary<Type, object> ResolutionExpression { get; set; }

        public object NewestInstance { get; set; }
        


    }
}