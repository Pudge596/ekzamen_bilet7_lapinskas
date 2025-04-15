using Microsoft.Win32;
using Npgsql;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LanguageSchoolApp
{
    public partial class AddClientWindow : Window
    {
        private int serviceId;
        private byte[] photoBytes;
        private string connectionString = "Host=localhost;Username=postgres;Password=123;Database=LanguageSchoolDB";

        public AddClientWindow(int serviceId)
        {
            InitializeComponent();
            this.serviceId = serviceId;
        }

        private void ChoosePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";

            if (dialog.ShowDialog() == true)
            {
                photoBytes = File.ReadAllBytes(dialog.FileName);
                BitmapImage image = new BitmapImage(new Uri(dialog.FileName));
                PhotoPreview.Source = image;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullNameTextBox.Text;
            string email = EmailTextBox.Text;
            string phone = PhoneTextBox.Text;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("INSERT INTO \"Client\" (full_name, email, phone, photo, service_id) VALUES (@name, @mail, @phone, @photo, @sid)", conn);
                cmd.Parameters.AddWithValue("name", fullName);
                cmd.Parameters.AddWithValue("mail", email);
                cmd.Parameters.AddWithValue("phone", phone);
                cmd.Parameters.AddWithValue("photo", (object)photoBytes ?? DBNull.Value);
                cmd.Parameters.AddWithValue("sid", serviceId);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Клиент добавлен");
            this.Close();
        }
    }
}
