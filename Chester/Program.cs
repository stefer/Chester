using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Chester.Services;
using Chester.Messages.Events;
using Chester.Messages.Commands;
using Chester.Messages;
using Chester.Search;

namespace Chester
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            IServiceScope serviceScope = host.Services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;
            var messageBus = (MessageBus)provider.GetRequiredService<IMessageBus>();

            var hostTask = host.RunAsync();

            await messageBus.SendAsync(new StartUci());
            await hostTask;
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddStdErrConsoleLogger();
                })
                .ConfigureServices((_, services) =>
                {
                    services
                        .AddSingleton<GameChanger>()
                        .AddSingleton<UciStdInOut>()
                        .AddSingleton<IUciInterpretator, UciInterpreter>()
                        .AddSingleton<IMessageBus, MessageBus>()
                        .AddSingleton<ISearchReporter, SearchReporter>();

                    services
                        .AddMessageHandler<SendUciMessage, UciStdInOut>()
                        .AddMessageHandler<StartUci, UciStdInOut>()
                        .AddMessageHandler<StopUci, UciStdInOut>()
                        .AddMessageHandler<BestMoveEvaluated, UciStdInOut>()
                        .AddMessageHandler<Info, UciStdInOut>()

                        .AddMessageHandler<UciCommStarted, GameChanger>()
                        .AddMessageHandler<UciReadyRequested, GameChanger>()
                        .AddMessageHandler<StartNewGame, GameChanger>()
                        .AddMessageHandler<SetPosition, GameChanger>()
                        .AddMessageHandler<Go, GameChanger>();
                });
        }
    }

    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessageHandler<TMessage, THandler>(this IServiceCollection services)
            where TMessage : Message
            where THandler : ICommandHandler<TMessage>
        {
            return services.AddSingleton<ICommandHandler<TMessage>>(s => s.GetService<THandler>());
        }
    }
}
