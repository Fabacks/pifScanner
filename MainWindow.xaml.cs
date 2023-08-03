using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace pifScanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Liste des devices trouvé
        private List<NetworkDevice> devices = new List<NetworkDevice>();
        
        // Event d'anulation du bouton rechercher
        private CancellationTokenSource cts;

        // Time search
        private Stopwatch searchTime = new Stopwatch();

        public MainWindow()
        {
            InitializeComponent();

            // Initialize the DataGrid
            dataGridDevice.ItemsSource = devices;

            // Valeur par défaut
            txtIpStart.Text = "192.168.1.1";
            txtIpEnd.Text = "192.168.1.255";
        }

        private void AddResultToList(NetworkDevice device)
        {
            Dispatcher.Invoke(() =>
            {
                devices.Add(device);

                dataGridDevice.Items.Refresh();

                // Update device count
                lblDeviceCount.Content = $"Device Count: {devices.Count}";
            });
        }


        private void MenuQuit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (cts != null)
            {
                // Cancel the previous scan if there is one
                cts.Cancel();
                cts = null;

                btnSearch.Content = "Lunch scan";

                return;
            }

            // Start search time stopwatch
            searchTime.Reset();
            searchTime.Start();

            // Create a new cancellation token source
            cts = new CancellationTokenSource();

            // Change the button text to "Stop Scan"
            btnSearch.Content = "Stop scan";

            // Reset device list
            devices.Clear();
            lblDeviceCount.Content = "Device Count: 0";

            // Refresh the DataGrid
            dataGridDevice.Items.Refresh();

            // Plage de la recherche
            var IpStart = txtIpStart.Text;
            var IpEnd = txtIpEnd.Text;

            Task.Run(async () =>
            {
                var scanner = new NetworkScanner();
                try
                {
                    await scanner.Scan(IpStart, IpEnd, AddResultToList, cts.Token);
                }
                catch (OperationCanceledException)
                {
                    // Handle the cancellation
                }
                finally
                {
                    // After scan is completed or canceled, revert the button text back to "Start Scan" on the main UI thread.
                    Dispatcher.Invoke(() =>
                    {
                        btnSearch.Content = "Lunch scan";

                        // Stop search time stopwatch and update label
                        searchTime.Stop();
                        lblSearchTime.Content = $"Search Time: {searchTime.Elapsed.TotalSeconds}s";
                    });
                }
            });
        }
    }
}
