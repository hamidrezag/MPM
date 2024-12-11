using Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Extensions
{
    public static class ServiceRegistrationWithAttribute
    {
        public static IServiceCollection AddInfrastructureServiceWithAttribute(this IServiceCollection services)
        {
            Type transientAttribute = typeof(TransientAttribute);
            Type scopedAttribute = typeof(ScopedAttribute);
            Type singletonAttribute = typeof(SingletonAttribute);
            //var ass2 = Assembly.GetEntryAssembly().GetTypes();
            var ass = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes()
                .Where(a => (a.IsDefined(transientAttribute) || a.IsDefined(scopedAttribute) || a.IsDefined(singletonAttribute)) && !a.IsInterface && !a.IsAbstract))
                  .Select(a => new { assignedType = a, serviceTypes = a.GetInterfaces().ToList() })
                     .ToList();


            //      var ass = Assembly.GetEntryAssembly()
            //.GetTypes()
            //.Where(a => (a.IsDefined(transientAttribute) || a.IsDefined(scopedAttribute) || a.IsDefined(singletonAttribute)) && !a.IsInterface && !a.IsAbstract)
            //.Select(a => new { assignedType = a, serviceTypes = a.GetInterfaces().ToList() })
            //.ToList();

            List<string> lll = new List<string>();

            foreach (var type in ass)
            {
                if (type.assignedType.IsDefined(transientAttribute, false))
                {
                    foreach (var itemno in type.serviceTypes)
                    {

                        services.AddTransient(itemno, type.assignedType);
                    }
                }

                if (type.assignedType.IsDefined(scopedAttribute, false))
                {
                    foreach (var itemno in type.serviceTypes)
                    {

                        services.AddScoped(itemno, type.assignedType);
                        lll.Add(type.assignedType.Name);
                    }
                }

                if (type.assignedType.IsDefined(singletonAttribute, false))
                {
                    foreach (var itemno in type.serviceTypes)
                    {

                        services.AddSingleton(itemno, type.assignedType);
                    }
                }


            }
            return services;
        }
    }


}
