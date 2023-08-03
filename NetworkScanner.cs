using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace pifScanner
{
    public class NetworkScanner
    {
        public async Task Scan(string startIp, string endIp, Action<NetworkDevice> onDeviceFound, CancellationToken ct)
        {
            var startParts = startIp.Split('.').Select(int.Parse).ToArray();
            var endParts = endIp.Split('.').Select(int.Parse).ToArray();

            // Mono threading
            //for (var i = startParts[3]; i <= endParts[3]; i++)
            //{
            //    var ip = $"{startParts[0]}.{startParts[1]}.{startParts[2]}.{i}";
            //    getIpAddress(ip, onDeviceFound, ct);
            //}

            // Multi threading
            var ipRange = Enumerable.Range(startParts[3], endParts[3] - startParts[3] + 1).ToList();
            await Task.Run(() => Parallel.ForEach(ipRange, (i) =>
            {
                var ip = $"{startParts[0]}.{startParts[1]}.{startParts[2]}.{i}";
                getIpAddress(ip, onDeviceFound, ct);
            }), ct);
        }


        private void getIpAddress(string ip, Action<NetworkDevice> onDeviceFound, CancellationToken ct)
        {
            using (var ping = new Ping())
            {
                // Throw if cancellation is requested
                ct.ThrowIfCancellationRequested();
                
                var reply = ping.SendPingAsync(ip, 3000).GetAwaiter().GetResult();
                if (reply.Status != IPStatus.Success)
                    return;

                string hostName = "Unknown";
                try
                {
                    var hostEntry = Dns.GetHostEntryAsync(ip).GetAwaiter().GetResult();
                    hostName = hostEntry.HostName;
                }
                catch (SocketException)
                {
                    // L'hôte est inconnu, continuez l'analyse.
                }

                var device = new NetworkDevice
                {
                    Ip = ip,
                    Hostname = hostName,
                    MacAddress = GetMacAddress(ip),
                    ResponseTime = reply.RoundtripTime.ToString()
                };

                // Appelle l'action fournie pour chaque appareil trouvé
                onDeviceFound?.Invoke(device);
            }
        }

        private string GetMacAddress(string ipAddress)
        {
            var arpRequest = new ProcessStartInfo
            {
                FileName = "arp",
                Arguments = "-a " + ipAddress,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            var arpProcess = Process.Start(arpRequest);

            string output = arpProcess.StandardOutput.ReadToEnd();

            // Extract MAC Address from 'arp -a' Output
            var lines = output.Split('\n');
            foreach (var line in lines)
            {
                if ( !line.Contains(ipAddress) )
                    continue;

                var parts = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    // Check if part is a MAC address
                    if (System.Text.RegularExpressions.Regex.IsMatch(part, @"([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})"))
                    {
                        return part;
                    }
                }
            }

            return "MAC Not Found";
        }
    }

}
