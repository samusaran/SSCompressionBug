using ServiceStack;
using ServiceStack.IO;
using ServiceStack.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

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
                WebHostPhysicalPath = ".".MapServerPath(),
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
            var ips = GetAllLocalIPv4(NetworkInterfaceType.Ethernet);

            new AppHost().Init().Start(ips.Select(x => $"http://{x}/"));

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

        private static string[] GetAllLocalIPv4(NetworkInterfaceType interface_type)
        {
            List<String> ipAddrList = new List<String>();
            ipAddrList.Add("localhost");

            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == interface_type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ipAddrList.Add(ip.Address.ToString());
                        }
                    }
                }
            }

            return ipAddrList.ToArray();
        }
    }
}
