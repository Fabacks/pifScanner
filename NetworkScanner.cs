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
            // Throw if cancellation is requested
            ct.ThrowIfCancellationRequested();

            using (var ping = new Ping())
            {
                var reply = ping.SendPingAsync(ip, 1000).GetAwaiter().GetResult();

                // Throw if cancellation is requested
                ct.ThrowIfCancellationRequested();

                if (reply.Status == IPStatus.Success)
                {
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

                    var macAddress = GetMacAddress(ip);

                    var device = new NetworkDevice
                    {
                        Ip = ip,
                        Hostname = hostName,
                        MacAddress = macAddress,
                        ResponseTime = reply.RoundtripTime.ToString()
                    };

                    // Appelle l'action fournie pour chaque appareil trouvé
                    onDeviceFound?.Invoke(device);
                }
            }
        }

        private string GetMacAddress(string ipAddress)
        {
            // Ping the IP to force refresh the ARP table
            using (var ping = new Ping())
            {
                var reply = ping.Send(ipAddress);

                if (reply.Status != IPStatus.Success)
                {
                    return "MAC Not Found";
                }
            }

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
                if (line.Contains(ipAddress))
                {
                    var parts = line.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2)
                    {
                        return parts[1];
                    }
                }
            }

            return "MAC Not Found";
        }
    }

}
