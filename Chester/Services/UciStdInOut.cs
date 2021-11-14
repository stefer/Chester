using Chester.Messages;
using Chester.Messages.Commands;
using Chester.Messages.Events;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Chester.Services
{
    public interface IUciInterpretator
    {
        public Task ExecuteAsync(string commandLine);
    }

    internal class UciStdInOut :
        ICommandHandler<SendUciMessage>,
        ICommandHandler<StartUci>,
        ICommandHandler<StopUci>,
        ICommandHandler<BestMoveEvaluated>,
        ICommandHandler<Info>
    {
        private readonly IUciInterpretator interpretator;
        readonly Channel<string> channel;
        CancellationTokenSource tokenSource;

        public UciStdInOut(IUciInterpretator interpretator)
        {
            this.interpretator = interpretator;
            channel = Channel.CreateUnbounded<string>();
        }

        public async Task HandleAsync(SendUciMessage message)
        {
            await channel.Writer.WriteAsync(message.Status);
        }

        public async Task HandleAsync(StartUci message)
        {
            await Start();
        }

        public Task HandleAsync(StopUci message)
        {
            Stop();
            return Task.CompletedTask;
        }

        public async Task HandleAsync(BestMoveEvaluated message)
        {
            await channel.Writer.WriteAsync($"bestmove {message.Move.ToStringLong()}");
        }

        public async Task HandleAsync(Info message)
        {
            StringBuilder sb = new();

            if (message.CurrentMove != null) sb.Append("currentmove ").Append(message.CurrentMove.ToStringLong());
            if (message.CurrentMoveNumber.HasValue) sb.Append(" currentmovenumber ").Append(message.CurrentMoveNumber.Value);
            if (message.Score.HasValue) sb.Append(" score cp ").Append(message.Score.Value);

            await channel.Writer.WriteAsync(sb.ToString());
        }

        public async Task Start()
        {
            tokenSource = new CancellationTokenSource();
            CancellationToken ct = tokenSource.Token;

            var writeTask = Task.Factory.StartNew(async () => await Write(channel.Reader, ct), ct);

            await Read(ct);
        }

        public void Stop()
        {
            if (tokenSource != null)
                tokenSource.Cancel();
            tokenSource = null;
        }

        private async Task Read(CancellationToken ct)
        {
            Stream inputStream = Console.OpenStandardInput();
            using StreamReader reader = new(inputStream);

            while (!reader.EndOfStream)
            {
                ct.ThrowIfCancellationRequested();
                var cmdLine = await reader.ReadLineAsync();
                await interpretator.ExecuteAsync(cmdLine);
            }
        }

        private async Task Write(ChannelReader<string> reader, CancellationToken ct)
        {
            Stream stream = Console.OpenStandardOutput();
            using StreamWriter writer = new(stream);
            writer.AutoFlush = true;

            while (true)
            {
                ct.ThrowIfCancellationRequested();
                var output = await reader.ReadAsync(ct);
                await writer.WriteLineAsync(output);
            }
        }
    }
}
