using Kursova2023_2024.Classes.Data_classes;
using Kursova2023_2024.Classes.Tool_classes;
using Kursova2023_2024.Forms;
using MySql.Data.MySqlClient;

namespace Kursova2023_2024
{
    public partial class StartForm : Form
    {
        // Статическое поле для хранения ссылки на экземпляр StartForm
        public static StartForm Instance { get; private set; }

        public StartForm()
        {
            InitializeComponent();
            ControlPropertiesClass.ApplyMouseProperties(ForgetLabel);
            ControlPropertiesClass.ApplyMouseProperties(RegistrationLabel);
            ControlPropertiesClass.SetToDefaultButton(LogInButton, "Log in");
            ControlPropertiesClass.SetToDefaultTextBoxClassic(LoginTextBox, "Enter login");
            ControlPropertiesClass.SetToDefaultTextBoxPassword(PasswordTextBox, "Enter password");

            // Присваиваем текущий экземпляр StartForm статическому полю Instance
            Instance = this;
        }

        private void LogInButton_Click(object sender, EventArgs e)
        {
            DataBaseConnectionClass.OpenConnection();
            string query = "SELECT * FROM login WHERE username = @login AND password = @password;";
            MySqlCommand command = new MySqlCommand(query, DataBaseConnectionClass.connection);
            command.Parameters.AddWithValue("@login", LoginTextBox.Text);
            command.Parameters.AddWithValue("@password", PasswordTextBox.Text);
            string username = command.ExecuteScalar() as string ?? "not find";

            DataBaseConnectionClass.CloseConnection();

            var user = DataBaseConnectionClass.GetUsersNotBanned().FirstOrDefault(user => user.Username == username);

            if (user != null)
            {
                switch (user)
                {
                    case Teacher teacher:
                        ControlPropertiesClass.CurrentUser = teacher;
                        break;
                    case Admin admin:
                        ControlPropertiesClass.CurrentUser = admin;
                        break;
                    case Student student:
                        ControlPropertiesClass.CurrentUser = student;
                        break;
                }
            }
            else
            {
                MessageBox.Show("Користувача не знайдено.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            PasswordTextBox.Text = "Enter password";
            PasswordTextBox.ForeColor = Color.Gray;
            PasswordTextBox.PasswordChar = '\0';

            MainMenu menu;
            if (user is Student)
            {
                menu = new MainMenu(DataBaseConnectionClass.GetActualVacancies().Where(v => !DataBaseConnectionClass.GetRespondedVacanciesIdForStudent().Any(id => id == v.Id)).ToList());
            }
            else { menu = new MainMenu(DataBaseConnectionClass.GetActualVacancies()); }
            menu.Show();
            this.Hide();
        }

        private void ForgetLabel_Click(object sender, EventArgs e)
        {
            string query = "SELECT password FROM login WHERE username = @username";
            using (MySqlCommand command = new MySqlCommand(query, DataBaseConnectionClass.GetConnection()))
            {
                command.Parameters.AddWithValue("@username", LoginTextBox.Text);
                DataBaseConnectionClass.OpenConnection();

                // Выполнить запрос и получить результат
                string password = command.ExecuteScalar() as string;

                // Закрыть соединение с базой данных
                DataBaseConnectionClass.CloseConnection();

                // Проверяем, был ли найден пользователь
                if (password != null)
                {
                    PasswordTextBox.PasswordChar = '*';
                    PasswordTextBox.ForeColor = Color.Black;
                    PasswordTextBox.Text = password;
                }
                else
                {
                    MessageBox.Show("Користувача не знайдено.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RegistrationLabel_Click(object sender, EventArgs e)
        {
            Registration registration = new Registration();
            registration.Show();
            this.Hide();
        }
    }
}
