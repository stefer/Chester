using Chess.Messages.Commands;
using Chess.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Chess.Messages;
using Chess.Messages.Events;

namespace Chess
{
    class Program
    {
        //static void Main(string[] args)
        //{

        //    var game= new Game();

        //    Console.WriteLine(game);

        //    while(true)
        //    {
        //        var evaluation = game.Search();

        //        if (Math.Abs(evaluation.Value) >= Evaluation.CheckMate) break;

        //        Console.WriteLine($"{game.NextToMove} made move {evaluation.Move} with value {evaluation.Value}");

        //        game.MakeMove(evaluation.Move);

        //        Console.WriteLine(game);
        //        Console.WriteLine();

        //        Console.ReadKey();
        //    }

        //    Console.Beep();
        //}

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
                .ConfigureServices((_, services) => {
                    services
                        .AddSingleton<GameChanger>()
                        .AddSingleton<UciStdInOut>()
                        .AddSingleton<IUciInterpretator, UciInterpreter>()
                        .AddSingleton<IMessageBus, MessageBus>();

                    services.AddSingleton<ICommandHandler<SendUciMessage>>(services => services.GetService<UciStdInOut>());
                    services.AddSingleton<ICommandHandler<StartUci>>(services => services.GetService<UciStdInOut>());
                    services.AddSingleton<ICommandHandler<StopUci>>(services => services.GetService<UciStdInOut>());
                    services.AddSingleton<ICommandHandler<UciCommStarted>>(services => services.GetService<GameChanger>());
                });
        }
    }

    public class StdErrConsoleLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state) => default;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Console.Error.WriteLine($"[{eventId.Id,2}: {logLevel,-12}] {formatter(state, exception)}");
        }
    }

    public sealed class StdErrConsoleLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new StdErrConsoleLogger();
        }

        public void Dispose()
        {
        }
    }

    public static class StdErrConsoleLoggerExtensions
    {
        public static ILoggingBuilder AddStdErrConsoleLogger(this ILoggingBuilder builder)
        {
            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILoggerProvider, StdErrConsoleLoggerProvider>());
            return builder;
        }
    }
}
