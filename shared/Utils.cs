using System;
using System.IO;
using Grpc.Core;

namespace Common
{
    public static class Utils
    {
        public static string CertDir = Path.Combine(GetSolutionDirectory(), "certs");
        public static string ServerPFXPath = Path.Combine(CertDir, "server.pfx");
        public static SslCredentials ClientSslCredentials
            = new SslCredentials(
                File.ReadAllText(Path.Combine(CertDir, "ca.crt")),
                new KeyCertificatePair(
                    File.ReadAllText(Path.Combine(CertDir, "client.crt")),
                    File.ReadAllText(Path.Combine(CertDir, "client.key"))));

        private static string GetSolutionDirectory()
        {
            var applicationBasePath = AppContext.BaseDirectory;

            var directoryInfo = new DirectoryInfo(applicationBasePath);
            do
            {
                var solutionFileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, "GRPCSample.sln"));
                if (solutionFileInfo.Exists)
                {
                    return directoryInfo.FullName;
                }

                directoryInfo = directoryInfo.Parent;
            } while (directoryInfo.Parent != null);

            throw new InvalidOperationException($"Solution directory could not be found for {applicationBasePath}.");
        }
    }
}