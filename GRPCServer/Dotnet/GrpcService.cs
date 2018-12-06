using System;

namespace GRPCServer.Dotnet
{
    public class GrpcService<TImplementation> : IGrpcService
    {
        public Type ImplementationType => typeof(TImplementation);
    }
}
