using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using Npgsql;
using LanguageSchoolApp.Models;
using System.Linq;
using System.Windows.Input;

namespace LanguageSchoolApp
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Service> services = new ObservableCollection<Service>();

        public MainWindow()
        {
            InitializeComponent();
            LoadServicesFromDatabase();
            ServiceListView.ItemsSource = services;
        }

        private void LoadServicesFromDatabase()
        {
            services.Clear();

            string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=123;Database=LanguageSchoolDB";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT id, name, price FROM \"Service\" ORDER BY id";

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        services.Add(new Service
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Price = reader.GetDecimal(2)
                        });
                    }
                }
            }
        }

        private void AddService_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddServiceWindow();
            addWindow.ShowDialog();

            if (addWindow.IsServiceAdded)
            {
                LoadServicesFromDatabase();
            }
        }

        private void DeleteService_Click(object sender, RoutedEventArgs e)
        {
            if (ServiceListView.SelectedItem is Service selectedService)
            {
                string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=123;Database=LanguageSchoolDB";

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "DELETE FROM \"Service\" WHERE id = @id";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("id", selectedService.Id);
                        cmd.ExecuteNonQuery();
                    }
                }

                services.Remove(selectedService);
            }
        }

        private void SortAsc_Click(object sender, RoutedEventArgs e)
        {
            var sorted = services.OrderBy(s => s.Name).ToList();
            services.Clear();
            foreach (var service in sorted)
                services.Add(service);
        }

        private void SortDesc_Click(object sender, RoutedEventArgs e)
        {
            var sorted = services.OrderByDescending(s => s.Name).ToList();
            services.Clear();
            foreach (var service in sorted)
                services.Add(service);
        }
        private void ServiceListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ServiceListView.SelectedItem is Service selectedService)
            {
                var clientWindow = new ClientWindow(selectedService.Id, selectedService.Name);
                clientWindow.ShowDialog();
                LoadServicesFromDatabase(); // если хочешь обновлять услуги после изменений
            }
        }
    }
}
