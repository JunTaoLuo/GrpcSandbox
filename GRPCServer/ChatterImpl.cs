﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using HelloChat;
using Microsoft.Extensions.Logging;

namespace GRPCServer
{
    public class ChatterImpl : Chatter.ChatterBase
    {
        private ILogger _logger;

        public ChatterImpl(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ChatterImpl>();
        }

        private static HashSet<IServerStreamWriter<ChatMessage>> _subscribers = new HashSet<IServerStreamWriter<ChatMessage>>();

        public override async Task Chat(IAsyncStreamReader<ChatMessage> requestStream, IServerStreamWriter<ChatMessage> responseStream, ServerCallContext context)
        {
            if (!await requestStream.MoveNext(CancellationToken.None))
            {
                // No messages so don't register and just exit.
                return;
            }

            var user = requestStream.Current.Name;

            // Warning, the following is very racy but good enough for a proof of concept
            // Register subscriber
            _logger.LogInformation($"{user} connected");
            _subscribers.Add(responseStream);

            do
            {
                await BroadcastMessageAsync(requestStream.Current, _logger);
            } while (await requestStream.MoveNext(CancellationToken.None));

            _subscribers.Remove(responseStream);
            _logger.LogInformation($"{user} disconnected");
        }

        private static async Task BroadcastMessageAsync(ChatMessage message, ILogger logger)
        {
            foreach (var subscriber in _subscribers)
            {
                logger.LogInformation($"Broadcasting: {message.Name} - {message.Message}");
                await subscriber.WriteAsync(message);
            }
        }
    }
}
