using System;
using System.Windows;
using Npgsql;

namespace LanguageSchoolApp
{
    public partial class AddServiceWindow : Window
    {
        public bool IsServiceAdded { get; private set; } = false;

        public AddServiceWindow()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();
            string priceText = PriceTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || !decimal.TryParse(priceText, out decimal price))
            {
                MessageBox.Show("Пожалуйста, введите корректные данные.");
                return;
            }

            string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=123;Database=LanguageSchoolDB";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                string query = "INSERT INTO \"Service\" (name, price) VALUES (@name, @price)";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.Parameters.AddWithValue("price", price);
                    cmd.ExecuteNonQuery();
                }
            }

            IsServiceAdded = true;
            MessageBox.Show("Услуга успешно добавлена.");
            Close();
        }
    }
}
