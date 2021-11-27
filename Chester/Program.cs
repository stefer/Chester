using Chester.Messages;
using Chester.Messages.Commands;
using Chester.Messages.Events;
using Chester.Search;
using Chester.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Chester
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();

            var serviceScope = host.Services.CreateScope();
            var provider = serviceScope.ServiceProvider;
            var messageBus = (MessageBus)provider.GetRequiredService<IMessageBus>();

            var hostTask = host.RunAsync();

            await messageBus.SendAsync(new StartUci());
            await hostTask;
        }

        private static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging => {
                    logging.ClearProviders();
                    logging.AddStdErrConsoleLogger();
                })
                .ConfigureServices((_, services) => {
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

    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessageHandler<TMessage, THandler>(this IServiceCollection services)
            where TMessage : Message
            where THandler : ICommandHandler<TMessage> => services.AddSingleton<ICommandHandler<TMessage>>(s => s.GetService<THandler>());
    }
}
