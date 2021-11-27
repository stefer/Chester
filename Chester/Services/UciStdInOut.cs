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
        private readonly IUciInterpretator _interpretator;
        private readonly Channel<string> _channel;
        private CancellationTokenSource _tokenSource;

        public UciStdInOut(IUciInterpretator interpretator)
        {
            _interpretator = interpretator;
            _channel = Channel.CreateUnbounded<string>();
        }

        public async Task HandleAsync(SendUciMessage message) => await _channel.Writer.WriteAsync(message.Status);

        public async Task HandleAsync(StartUci message) => await Start();

        public Task HandleAsync(StopUci message)
        {
            Stop();
            return Task.CompletedTask;
        }

        public async Task HandleAsync(BestMoveEvaluated message) => await _channel.Writer.WriteAsync($"bestmove {message.Move.ToStringLong()}");

        public async Task HandleAsync(Info m)
        {
            StringBuilder sb = new("info ");

            if (m.CurrentMove != null) sb.Append(" currentmove ").Append(m.CurrentMove.ToStringLong());
            Append(m.CurrentMoveNumber, "currentmovenumber", sb);
            Append(m.Depth, "depth", sb);
            Append(m.Score, "score cp", sb);
            Append(m.TimeMs, "time", sb);
            Append(m.Nodes, "nodes", sb);
            Append(m.NodesPerSec, "nps", sb);
            if (m.Pv != null)
            {
                sb.Append(" pv ");
                foreach (var node in m.Pv) sb.Append(' ').Append(node.ToStringLong());
            }

            await _channel.Writer.WriteAsync(sb.ToString());
        }

        public static void Append<T>(T? value, string key, StringBuilder sb) where T : struct
        {
            if (value.HasValue) sb.Append(' ').Append(key).Append(' ').Append(value.Value);
        }

        public async Task Start()
        {
            _tokenSource = new CancellationTokenSource();
            var ct = _tokenSource.Token;

            var writeTask = Task.Factory.StartNew(async () => await Write(_channel.Reader, ct), ct);

            await Read(ct);
        }

        public void Stop()
        {
            if (_tokenSource != null)
                _tokenSource.Cancel();
            _tokenSource = null;
        }

        private async Task Read(CancellationToken ct)
        {
            var inputStream = Console.OpenStandardInput();
            using StreamReader reader = new(inputStream);

            while (!reader.EndOfStream)
            {
                ct.ThrowIfCancellationRequested();
                var cmdLine = await reader.ReadLineAsync();
                await _interpretator.ExecuteAsync(cmdLine);
            }
        }

        private static async Task Write(ChannelReader<string> reader, CancellationToken ct)
        {
            var stream = Console.OpenStandardOutput();
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
