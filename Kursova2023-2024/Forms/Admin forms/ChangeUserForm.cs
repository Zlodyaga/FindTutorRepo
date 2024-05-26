using Kursova2023_2024.Classes.Data_classes;
using Kursova2023_2024.Classes.Tool_classes;
using Kursova2023_2024.Forms.AdminForms;
using MySql.Data.MySqlClient;
using System.Data;

namespace Kursova2023_2024.Forms.Admin_forms
{
    public partial class ChangeUserForm : Form
    {
        private TextBox nameTextBox = new TextBox();
        private TextBox surnameTextBox = new TextBox();
        private TextBox usernameTextBox = new TextBox();
        private TextBox emailTextBox = new TextBox();
        private TextBox phoneTextBox = new TextBox();
        private TextBox PasswordTextBox = new TextBox();
        private DateTimePicker datePicker = new DateTimePicker();
        private ComboBox RoleComboBox = new ComboBox();
        private ComboBox sexComboBox = new ComboBox();
        private string roleUserNow;
        private IUser userToChange;
        private List<IUser> users;
        private string passwordUser;

        public ChangeUserForm(IUser userTemp)
        {
            this.userToChange = DataBaseConnectionClass.GetUsersAll().Single(userFromList => userFromList.Id == userTemp.Id); ;
            passwordUser = GetUserPassword(userToChange.Username);
            InitializeComponent();
            ControlPropertiesClass.SetToDefaultForm(this);
            ControlPropertiesClass.AddBackIcon(this, new ListUsers());

            ControlPropertiesClass.SetToDefaultComboBox(RoleComboBox);
            RoleComboBox.Location = new System.Drawing.Point(480, 250);
            RoleComboBox.Size = new System.Drawing.Size(190, 20);

            // Добавление элементов в CheckedListBox
            foreach (var roles in GetRolesForComboBox())
            {
                RoleComboBox.Items.Add(roles);
            }
            RoleComboBox.Text = $"{userToChange.GetType().Name}";

            this.Controls.Add(RoleComboBox);

            ControlPropertiesClass.SetToDefaultComboBox(sexComboBox);
            sexComboBox.Location = new System.Drawing.Point(250, 100);
            sexComboBox.Size = new System.Drawing.Size(190, 20);
            foreach (var sex in GetSexForComboBox())
            {
                sexComboBox.Items.Add(sex);
            }
            sexComboBox.Text = userToChange.sex;

            this.Controls.Add(sexComboBox);

            // Установка свойств nameTextBox
            nameTextBox.Location = new System.Drawing.Point(250, 150);
            nameTextBox.Size = new System.Drawing.Size(190, 20);
            ControlPropertiesClass.SetToDefaultTextBoxNames(nameTextBox, userToChange.FirstName);

            // Установка свойств surnameTextBox
            surnameTextBox.Location = new System.Drawing.Point(480, 150);
            surnameTextBox.Size = new System.Drawing.Size(190, 20);
            ControlPropertiesClass.SetToDefaultTextBoxNames(surnameTextBox, userToChange.LastName);

            // Установка свойств usernameTextBox
            usernameTextBox.Location = new System.Drawing.Point(250, 200);
            usernameTextBox.Size = new System.Drawing.Size(190, 20);
            ControlPropertiesClass.SetToDefaultTextBoxClassic(usernameTextBox, userToChange.Username);

            // Установка свойств emailTextBox
            emailTextBox.Location = new System.Drawing.Point(480, 200);
            emailTextBox.Size = new System.Drawing.Size(190, 20);
            ControlPropertiesClass.SetToDefaultTextBoxEmail(emailTextBox, userToChange.Email);

            // Установка свойств PasswordTextBox
            PasswordTextBox.Location = new System.Drawing.Point(250, 250);
            PasswordTextBox.Size = new System.Drawing.Size(190, 20);
            ControlPropertiesClass.SetToDefaultTextBoxClassic(PasswordTextBox, passwordUser);

            // Установка свойств phoneTextBox
            phoneTextBox.Location = new System.Drawing.Point(250, 300);
            phoneTextBox.Size = new System.Drawing.Size(190, 20);
            ControlPropertiesClass.SetToDefaultTextBoxDigits(phoneTextBox, userToChange.Phone);

            this.Controls.Add(nameTextBox);
            this.Controls.Add(surnameTextBox);
            this.Controls.Add(usernameTextBox);
            this.Controls.Add(emailTextBox);
            this.Controls.Add(phoneTextBox);
            this.Controls.Add(PasswordTextBox);


            ControlPropertiesClass.SetToDefaultDateTimePicker(datePicker);
            datePicker.MaxDate = DateTime.Today;
            datePicker.Location = new System.Drawing.Point(480, 300);
            datePicker.Size = new System.Drawing.Size(190, 20);
            // Добавьте элемент управления на форму
            this.Controls.Add(datePicker);

            Button buttonChangeData = new Button();
            ControlPropertiesClass.SetToDefaultButton(buttonChangeData, "Change data");
            buttonChangeData.Size = new Size(175, 40);
            buttonChangeData.Location = new Point(365, 350);
            buttonChangeData.Click += ButtonChangeData_Click;

            this.Controls.Add(buttonChangeData);
            UpdateData(userToChange);
        }

        private void UpdateData(IUser user)
        {
            this.userToChange = user;
            users = DataBaseConnectionClass.GetUsersAll().Where(user => user.Username != userToChange.Username).ToList();
            switch (userToChange)
            {
                case Admin:
                    roleUserNow = "Admin";
                    break;
                case Teacher:
                    roleUserNow = "Teacher";
                    break;
                case Student:
                    roleUserNow = "Student";
                    break;
            }
        }

        private string GetUserPassword(string username)
        {
            string query = "SELECT password FROM login WHERE username = @username";
            using (MySqlCommand command = new MySqlCommand(query, DataBaseConnectionClass.GetConnection()))
            {
                command.Parameters.AddWithValue("@username", username);
                DataBaseConnectionClass.OpenConnection();

                // Выполнить запрос и получить результат
                string password = command.ExecuteScalar() as string;

                // Закрыть соединение с базой данных
                DataBaseConnectionClass.CloseConnection();

                // Проверяем, был ли найден пользователь
                if (password != null)
                {
                    return password;
                }
                else
                {
                    MessageBox.Show("Error while taking password user", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
        }

        private List<string> GetRolesForComboBox()
        {
            List<string> roles = new List<string>();

            DataBaseConnectionClass.OpenConnection();

            string query = "SELECT DISTINCT role_in_system FROM users;";
            MySqlCommand command = new MySqlCommand(query, DataBaseConnectionClass.connection);
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                roles.Add(reader.GetString("role_in_system"));
            }

            reader.Close();
            DataBaseConnectionClass.CloseConnection();

            return roles;
        }

        private List<string> GetSexForComboBox()
        {
            List<string> sex = new List<string>();

            DataBaseConnectionClass.OpenConnection();

            string query = "SELECT DISTINCT sex FROM users;";
            MySqlCommand command = new MySqlCommand(query, DataBaseConnectionClass.connection);
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                sex.Add(reader.GetString("sex"));
            }

            reader.Close();
            DataBaseConnectionClass.CloseConnection();

            return sex;
        }

        private bool checkForText()
        {
            return ((nameTextBox.Text == userToChange.FirstName
                && surnameTextBox.Text == userToChange.LastName
                && emailTextBox.Text == userToChange.Email
                && sexComboBox.Text == userToChange.sex
                && RoleComboBox.Text == roleUserNow.ToLower()
                && phoneTextBox.Text == userToChange.Phone));
        }

        private bool checkPasswordOrLoginChanges() => (PasswordTextBox.Text == passwordUser && usernameTextBox.Text == userToChange.Username);

        private void ButtonChangeData_Click(object sender, EventArgs e)
        {
            if (checkForText() && checkPasswordOrLoginChanges())
            {
                MessageBox.Show("You didn't make any changes", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string username = usernameTextBox.Text;

            if (users.Any(user => user.Username == username))
            {
                MessageBox.Show("User already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sex = sexComboBox.Text;
            string firstName = nameTextBox.Text;
            string lastName = surnameTextBox.Text;
            string email = emailTextBox.Text;
            string phone = phoneTextBox.Text;
            string password = PasswordTextBox.Text;
            string roleInSystem = RoleComboBox.Text;
            DateTime dateOfBirth = datePicker.Value.Date;

            ChangeTableUsersWithoutUsername(userToChange.Username, firstName, lastName, sex, email, phone, dateOfBirth, roleInSystem);
            if (!checkPasswordOrLoginChanges())
            {
                ChangeLoginAndPasswordInDatabase(username, password);
            }
        }

        private void ChangeTableUsersWithoutUsername(string username, string firstName, string lastName, string sex, string email, string phone, DateTime dateOfBirth, string roleInSystem)
        {
            // Формирование SQL-запроса для добавления пользователя
            string query = "UPDATE users SET first_name = @firstName, last_name = @lastName, sex = @sex, email = @email, " +
                           "phone = @phone, date_of_birth = @dateOfBirth, role_in_system = @roleInSystem WHERE username = @username;";

            // Создание и выполнение команды с параметрами
            using (MySqlCommand command = new MySqlCommand(query, DataBaseConnectionClass.GetConnection()))
            {
                // Добавление параметров к команде
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@firstName", firstName);
                command.Parameters.AddWithValue("@lastName", lastName);
                command.Parameters.AddWithValue("@sex", sex);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@phone", phone);
                command.Parameters.AddWithValue("@dateOfBirth", dateOfBirth);
                command.Parameters.AddWithValue("@roleInSystem", roleInSystem);

                // Открытие соединения с базой данных
                DataBaseConnectionClass.OpenConnection();

                // Выполнение команды
                int rowsAffected = command.ExecuteNonQuery();

                // Закрытие соединения с базой данных
                DataBaseConnectionClass.CloseConnection();

                // Проверка успешности выполнения запроса
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Data successfully changed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Очистка полей ввода после успешного добавления пользователя
                }
                else
                {
                    MessageBox.Show("Error occured. Please, try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ChangeLoginAndPasswordInDatabase(string newUsername, string newPassword)
        {
            // Формируем запрос для обновления имени пользователя
            List<string> tables = (checkForBan(userToChange.Username) == "banned") ? new List<string> { "users", "login", "banned"} : new List<string> { "users", "login" };
            string query;
            foreach (string table in tables)
            {
                query = $"UPDATE {table} SET username = @new_username WHERE username = @old_username;";

                using (MySqlCommand command = new MySqlCommand(query, DataBaseConnectionClass.GetConnection()))
                {
                    // Добавляем параметры
                    command.Parameters.AddWithValue("@old_username", userToChange.Username);
                    command.Parameters.AddWithValue("@new_username", newUsername);

                    DataBaseConnectionClass.OpenConnection();
                    // Выполняем запрос
                    int rowsAffected = command.ExecuteNonQuery();

                    DataBaseConnectionClass.CloseConnection();
                    // Выводим сообщение об успешном обновлении
                    if (rowsAffected <= 0)
                    {
                        MessageBox.Show($"Error occurred while updating password in {table}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

                // Формируем запрос для обновления пароля
                query = "UPDATE login SET password = @newPassword WHERE username = @username;";

                using (MySqlCommand passwordUpdateCommand = new MySqlCommand(query, DataBaseConnectionClass.GetConnection()))
                {
                DataBaseConnectionClass.OpenConnection();

                // Добавляем параметры
                passwordUpdateCommand.Parameters.AddWithValue("@username", newUsername);
                passwordUpdateCommand.Parameters.AddWithValue("@newPassword", newPassword);

                // Выполняем запрос
                int rowsAffected = passwordUpdateCommand.ExecuteNonQuery();

                DataBaseConnectionClass.CloseConnection();

                if (rowsAffected <= 0)
                {
                    MessageBox.Show("Error occurred while updating password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string? checkForBan(string login)
        {
            switch (DataBaseConnectionClass.GetBannedLogins().Contains(login))
            {
                case true:
                    return "banned";
                default: return null;
            }
        }
    }
}
