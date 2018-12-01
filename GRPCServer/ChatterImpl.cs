using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using HelloChat;

namespace GRPCServer
{
    public class ChatterImpl : Chatter.ChatterBase
    {
        private static HashSet<IServerStreamWriter<ChatMessage>> _subscribers = new HashSet<IServerStreamWriter<ChatMessage>>();

        public override async Task Chat(IAsyncStreamReader<ChatMessage> requestStream, IServerStreamWriter<ChatMessage> responseStream, ServerCallContext context)
        {
            if (!await requestStream.MoveNext(CancellationToken.None))
            {
                // No messages so don't register and just exit.
                return;
            }

            // Warning, the following is very racy but good enough for a proof of concept
            // Register subscriber
            _subscribers.Add(responseStream);

            do
            {
                await BroadcastMessageAsync(requestStream.Current);
            } while (await requestStream.MoveNext(CancellationToken.None));

            _subscribers.Remove(responseStream);
        }

        private static async Task BroadcastMessageAsync(ChatMessage message)
        {
            foreach (var subscriber in _subscribers)
            {
                await subscriber.WriteAsync(message);
            }
        }
    }

    public class MessageReceivedArgs : EventArgs
    {
        public ChatMessage ChatMessage { get; set; }
        public IServerStreamWriter<ChatMessage> Writer { get; set; }
    }
}
