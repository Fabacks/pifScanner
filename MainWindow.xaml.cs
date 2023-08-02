using System;
using System.Collections.Generic;
using System.ComponentModel;
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

                btnSearch.Content = "Lancer le scan";

                return;
            }

            // Create a new cancellation token source
            cts = new CancellationTokenSource();

            // Change the button text to "Stop Scan"
            btnSearch.Content = "Arrêter le scan";

            // Clear the previous scan results
            devices.Clear();

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
                        btnSearch.Content = "Lancer le scan";
                    });
                }
            });
        }
    }
}
