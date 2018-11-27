﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Http;

namespace GRPCSample.Internal
{
    internal interface IServerCallHandler
    {
        Task HandleCallAsync(HttpContext httpContext);
    }

    internal class UnaryServerCallHandler<TRequest, TResponse> : IServerCallHandler
        where TRequest : class
        where TResponse : class
    {
        readonly Method<TRequest, TResponse> method;
        readonly UnaryServerMethod<TRequest, TResponse> handler;

        public UnaryServerCallHandler(Method<TRequest, TResponse> method, UnaryServerMethod<TRequest, TResponse> handler)
        {
            this.method = method;
            this.handler = handler;
        }

        public async Task HandleCallAsync(HttpContext httpContext)
        {
            httpContext.Response.ContentType = "application/grpc";
            httpContext.Response.Headers.Append("grpc-encoding", "identity");

            var requestPayload = await StreamUtils.ReadMessageAsync(httpContext.Request.Body);
            // TODO: make sure the payload is not null
            var request = method.RequestMarshaller.Deserializer(requestPayload);

            // TODO: make sure there are no more request messages.

            var response = await handler(request, null);

            // TODO: make sure the response is not null
            var responsePayload = method.ResponseMarshaller.Serializer(response);

            await StreamUtils.WriteMessageAsync(httpContext.Response.Body, responsePayload, 0, responsePayload.Length);

            httpContext.Response.AppendTrailer("grpc-status", ((int)StatusCode.OK).ToString());
        }
    }

    // utilities for parsing gRPC messages from request/response streams
    // NOTE: implementations are not efficient and are very GC heavy
    internal class StreamUtils
    {
        const int MessageDelimiterSize = 4;  // how many bytes it takes to encode "Message-Length"

        // reads a grpc message
        // returns null if there are no more messages.
        public static async Task<byte[]> ReadMessageAsync(Stream stream)
        {
            // read Compressed-Flag and Message-Length
            // as described in https://github.com/grpc/grpc/blob/master/doc/PROTOCOL-HTTP2.md
            var delimiterBuffer = new byte[1 + MessageDelimiterSize];
            if (!await ReadExactlyBytesOrNothing(stream, delimiterBuffer, 0, delimiterBuffer.Length))
            {
                return null;
            }

            var compressionFlag = delimiterBuffer[0];
            var messageLength = DecodeMessageLength(new ReadOnlySpan<byte>(delimiterBuffer, 1, MessageDelimiterSize));

            if (compressionFlag != 0)
            {
                // TODO(jtattermusch): support compressed messages
                throw new IOException("Compressed messages are not yet supported.");
            }

            var msgBuffer = new byte[messageLength];
            if (!await ReadExactlyBytesOrNothing(stream, msgBuffer, 0, msgBuffer.Length))
            {
                throw new IOException("Unexpected end of stream.");
            }
            return msgBuffer;
        }

        public static async Task WriteMessageAsync(Stream stream, byte[] buffer, int offset, int count)
        {
            var delimiterBuffer = new byte[1 + MessageDelimiterSize];
            delimiterBuffer[0] = 0; // = non-compressed
            EncodeMessageLength(count, new Span<byte>(delimiterBuffer, 1, MessageDelimiterSize));
            await stream.WriteAsync(delimiterBuffer, 0, delimiterBuffer.Length);

            await stream.WriteAsync(buffer, offset, count);
        }

        // reads exactly the number of requested bytes (returns true if successfully read).
        // returns false if we reach end of stream before successfully reading anything.
        // throws if stream ends in the middle of reading.
        private static async Task<bool> ReadExactlyBytesOrNothing(Stream stream, byte[] buffer, int offset, int count)
        {
            bool noBytesRead = true;
            while (count > 0)
            {
                int bytesRead = await stream.ReadAsync(buffer, offset, count);
                if (bytesRead == 0)
                {
                    if (noBytesRead)
                    {
                        return false;
                    }
                    throw new IOException("Unexpected end of stream.");
                }
                noBytesRead = false;
                offset += bytesRead;
                count -= bytesRead;
            }
            return true;
        }

        private static void EncodeMessageLength(int messageLength, Span<byte> dest)
        {
            if (dest.Length < MessageDelimiterSize)
            {
                throw new ArgumentException("Buffer too small to decode message length.");
            }

            ulong unsignedValue = (ulong)messageLength;
            for (int i = MessageDelimiterSize - 1; i >= 0; i--)
            {
                // msg length stored in big endian
                dest[i] = (byte)(unsignedValue & 0xff);
                unsignedValue >>= 8;
            }
        }

        private static int DecodeMessageLength(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length < MessageDelimiterSize)
            {
                throw new ArgumentException("Buffer too small to decode message length.");
            }

            ulong result = 0;
            for (int i = 0; i < MessageDelimiterSize; i++)
            {
                // msg length stored in big endian
                result = (result << 8) + buffer[i];
            }

            if (result > int.MaxValue)
            {
                throw new IOException("Message too large: " + result);
            }
            return (int)result;
        }

    }
}
