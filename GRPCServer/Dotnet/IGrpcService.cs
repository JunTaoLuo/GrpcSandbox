using System;

namespace GRPCServer.Dotnet
{
    public interface IGrpcService
    {
        Type ImplementationType { get; }
    }
}
