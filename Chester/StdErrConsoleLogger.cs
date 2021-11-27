using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;

namespace Chester
{
    public class StdErrConsoleLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state) => default;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) => Console.Error.WriteLine($"[{eventId.Id,2}: {logLevel,-12}] {formatter(state, exception)}");
    }

    public sealed class StdErrConsoleLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => new StdErrConsoleLogger();

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
