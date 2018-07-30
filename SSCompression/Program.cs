using ServiceStack;
using ServiceStack.IO;
using System;
using System.Reflection;
using System.Threading;

namespace SSCompression
{
    /// AppHost Implementation
    public sealed class AppHost : AppSelfHostBase
    {
        public AppHost() : base("Compression Bug", Assembly.GetExecutingAssembly()) { }

        /// Overriding Configure to enable compression
        public override void Configure(Funq.Container container)
        {
            var hostConfig = new HostConfig
            {
                DebugMode = false,
                CompressFilesWithExtensions = { "js", "css", "html" },
                CompressFilesLargerThanBytes = 10 * 1024,
                WebHostPhysicalPath = @"..\..\".MapServerPath(),
            };

            base.SetConfig(hostConfig);
        }

        /// Just compress everything. Commenting this override does not change behaviour
        public override bool ShouldCompressFile(IVirtualFile file)
        {
            return true;
        }
    }


    /// Entry Point
    class Program
    {
        /// Entry point dell'applicazione
        static void Main(string[] args)
        {
            try
            {
                new AppHost().Init().Start("http://localhost/");
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                throw;
            }

            // Handle CTRL+C
            var exitEvent = new ManualResetEvent(false);

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                Console.WriteLine("Stopping the server...");
                eventArgs.Cancel = true;
                exitEvent.Set();
            };

            Console.WriteLine("Use CTRL+C to close this server...");
            exitEvent.WaitOne();
        }
    }
}
