using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SnakeServerAPI.Service
{
    public class Routine<T>
    {
        public TimeSpan Delay { get; set; }
        public Func<T, Task> Task { get; set; }
        public Routine(TimeSpan delay, Func<T, Task> task)
        {
            this.Delay = delay;
            this.Task = task;
        }
    }
    public static class RoutinesHandlerExtension
    {
        public static void AddRoutinesHandler(this IServiceCollection serviceCollection, Action<RoutinesHandlerService> routinesBuilder = null)
        {
            var service = new RoutinesHandlerService(routinesBuilder, serviceCollection.BuildServiceProvider());
            serviceCollection.AddSingleton(service);
        }
    }
    public class RoutinesHandlerService
    {
        private Dictionary<string, Task> tasks;
        IServiceProvider serviceProvider;
        public RoutinesHandlerService(Action<RoutinesHandlerService> routinesBuilder, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            tasks = new Dictionary<string, Task>();
            if (routinesBuilder != null)
            {
                routinesBuilder.Invoke(this);
            }
        }
        public Task Add<T>(string key, Routine<T> routine)
        {
            var task = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    using (var scope = this.serviceProvider.CreateScope())
                    {
                        try
                        {
                            routine.Task.Invoke(scope.ServiceProvider.GetRequiredService<T>()).GetAwaiter().GetResult();
                        }
                        catch (Exception e)
                        {
                            scope.ServiceProvider.GetService<ILogger<RoutinesHandlerService>>().LogError(e.ToString());
                        }
                    }
                    Thread.Sleep((int)routine.Delay.TotalMilliseconds);
                }
            });
            tasks.Add(key, task);
            return task;
        }
    }

}
