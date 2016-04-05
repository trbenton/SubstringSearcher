using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SubstringFramework.Util
{
    public static class InterfaceRegistrar
    {
        public static IEnumerable<T> RegisterInterfaces<T>() where T : class
        {
            var serverInstances = from t in Assembly.GetCallingAssembly().GetTypes()
                                  where
                                      t.GetInterfaces().Contains(typeof(T)) &&
                                      t.GetConstructor(Type.EmptyTypes) != null
                                  select Activator.CreateInstance(t) as T;
            return serverInstances;
        }
    }
}
