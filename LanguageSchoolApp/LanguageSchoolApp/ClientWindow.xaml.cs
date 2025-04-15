using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Npgsql;

namespace LanguageSchoolApp
{
    public partial class ClientWindow : Window
    {
        private int serviceId;
        private string serviceName;
        private string connectionString = "Host=localhost;Username=postgres;Password=123;Database=LanguageSchoolDB";

        public ClientWindow(int serviceId, string serviceName)
        {
            InitializeComponent();
            this.serviceId = serviceId;
            this.serviceName = serviceName;

            Title = $"Клиенты услуги: {serviceName}";
            LoadClients();
        }

        private void LoadClients()
        {
            List<Client> clients = new List<Client>();

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("SELECT id, full_name, email, phone, photo FROM \"Client\" WHERE service_id = @serviceId", conn);
                cmd.Parameters.AddWithValue("serviceId", serviceId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        byte[] photoBytes = reader["photo"] as byte[];

                        clients.Add(new Client
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            FullName = reader["full_name"].ToString(),
                            Email = reader["email"].ToString(),
                            Phone = reader["phone"].ToString(),
                            Photo = photoBytes,
                            PhotoImage = ConvertToImage(photoBytes)
                        });
                    }
                }
            }

            ClientListView.ItemsSource = clients;
        }

        private BitmapImage ConvertToImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;

            using (var ms = new MemoryStream(imageData))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }

        private void AddClientButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddClientWindow(serviceId);
            addWindow.ShowDialog();
            LoadClients(); // обновим список после добавления
        }
        private void DeleteClientButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedClient = ClientListView.SelectedItem as Client;

            if (selectedClient == null)
            {
                MessageBox.Show("Выберите клиента для удаления.");
                return;
            }

            var result = MessageBox.Show($"Удалить клиента {selectedClient.FullName}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand("DELETE FROM \"Client\" WHERE id = @id", conn);
                    cmd.Parameters.AddWithValue("id", selectedClient.Id);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Клиент удален.");
                LoadClients(); // Обновим список
            }
        }
    }

    public class Client
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public byte[] Photo { get; set; }
        public BitmapImage PhotoImage { get; set; }
    }

}
