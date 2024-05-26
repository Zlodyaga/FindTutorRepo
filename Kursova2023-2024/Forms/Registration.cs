using Kursova2023_2024.Classes.Tool_classes;
using MySql.Data.MySqlClient;

namespace Kursova2023_2024.Forms
{
    public partial class Registration : Form
    {
        private TextBox nameTextBox = new TextBox();
        private TextBox surnameTextBox = new TextBox();
        private TextBox usernameTextBox = new TextBox();
        private TextBox emailTextBox = new TextBox();
        private TextBox phoneTextBox = new TextBox();
        private TextBox PasswordTextBox = new TextBox();
        private TextBox PasswordAgainTextBox = new TextBox();
        private DateTimePicker datePicker = new DateTimePicker();
        private RadioButton teacherRadioButton = new RadioButton();
        private RadioButton studentRadioButton = new RadioButton();
        private ComboBox sexComboBox = new ComboBox();

        public Registration()
        {
            InitializeComponent();
            ControlPropertiesClass.SetToDefaultForm(this);
            ControlPropertiesClass.AddBackIcon(this, StartForm.Instance);

            Label questionLabel = new Label();
            questionLabel.Location = new System.Drawing.Point(380, 30);
            questionLabel.AutoSize = true;
            ControlPropertiesClass.SetToDefaultHeaderLabelFont(questionLabel, "Who are you?");
            this.Controls.Add(questionLabel);

            teacherRadioButton.Size = new System.Drawing.Size(190, 20);
            teacherRadioButton.Text = "I'm teacher";
            teacherRadioButton.Location = new System.Drawing.Point(500, 80);
            teacherRadioButton.Checked = true;

            studentRadioButton.Size = new System.Drawing.Size(190, 20);
            studentRadioButton.Text = "I'm student/Parent";
            studentRadioButton.Location = new System.Drawing.Point(500, 110);

            this.Controls.Add(teacherRadioButton);
            this.Controls.Add(studentRadioButton);

            ControlPropertiesClass.SetToDefaultComboBox(sexComboBox);
            sexComboBox.Location = new System.Drawing.Point(250, 100);
            sexComboBox.Size = new System.Drawing.Size(190, 20); 
            sexComboBox.Items.Add("Чоловік");
            sexComboBox.Items.Add("Жінка");
            sexComboBox.SelectedIndex = 0;

            this.Controls.Add(sexComboBox);

            // Установка свойств nameTextBox
            nameTextBox.Location = new System.Drawing.Point(250, 150);
            nameTextBox.Size = new System.Drawing.Size(190, 20);
            ControlPropertiesClass.SetToDefaultTextBoxNames(nameTextBox, "Name");

            // Установка свойств surnameTextBox
            surnameTextBox.Location = new System.Drawing.Point(480, 150);
            surnameTextBox.Size = new System.Drawing.Size(190, 20);
            ControlPropertiesClass.SetToDefaultTextBoxNames(surnameTextBox, "Surname");

            // Установка свойств usernameTextBox
            usernameTextBox.Location = new System.Drawing.Point(250, 200);
            usernameTextBox.Size = new System.Drawing.Size(190, 20);
            ControlPropertiesClass.SetToDefaultTextBoxClassic(usernameTextBox, "Username");

            // Установка свойств emailTextBox
            emailTextBox.Location = new System.Drawing.Point(480, 200);
            emailTextBox.Size = new System.Drawing.Size(190, 20);
            ControlPropertiesClass.SetToDefaultTextBoxEmail(emailTextBox, "e-mail");

            // Установка свойств PasswordTextBox
            PasswordTextBox.Location = new System.Drawing.Point(250, 250);
            PasswordTextBox.Size = new System.Drawing.Size(190, 20);
            ControlPropertiesClass.SetToDefaultTextBoxPassword(PasswordTextBox, "Password");

            // Установка свойств PasswordAgainTextBox
            PasswordAgainTextBox.Location = new System.Drawing.Point(480, 250);
            PasswordAgainTextBox.Size = new System.Drawing.Size(190, 20);
            ControlPropertiesClass.SetToDefaultTextBoxPassword(PasswordAgainTextBox, "Enter password again");

            // Установка свойств phoneTextBox
            phoneTextBox.Location = new System.Drawing.Point(250, 300);
            phoneTextBox.Size = new System.Drawing.Size(190, 20);
            ControlPropertiesClass.SetToDefaultTextBoxDigits(phoneTextBox, "Phone number");

            this.Controls.Add(nameTextBox);
            this.Controls.Add(surnameTextBox);
            this.Controls.Add(usernameTextBox);
            this.Controls.Add(emailTextBox);
            this.Controls.Add(phoneTextBox);
            this.Controls.Add(PasswordTextBox);
            this.Controls.Add(PasswordAgainTextBox);

            
            ControlPropertiesClass.SetToDefaultDateTimePicker(datePicker);
            datePicker.MaxDate = DateTime.Today; 
            datePicker.Location = new System.Drawing.Point(480, 300);
            datePicker.Size = new System.Drawing.Size(190, 20);
            // Добавьте элемент управления на форму
            this.Controls.Add(datePicker);

            Button buttonRegister = new Button();
            ControlPropertiesClass.SetToDefaultButton(buttonRegister, "Register");
            buttonRegister.Size = new Size(175, 40);
            buttonRegister.Location = new Point(365, 350);
            buttonRegister.Click += ButtonRegister_Click;

            this.Controls.Add(buttonRegister);
        }

        private bool checkForText()
        {
            return ((nameTextBox.Text == "Name"
                || surnameTextBox.Text == "Surname"
                || usernameTextBox.Text == "Username"
                || emailTextBox.Text == "e-mail"
                || PasswordTextBox.Text == "Password"
                || PasswordAgainTextBox.Text == "Enter password again"
                || phoneTextBox.Text == "Phone number"));
        }
        private void ButtonRegister_Click(object sender, EventArgs e)
        {
            if (checkForText()) {
                MessageBox.Show("Please, enter all fields", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; 
            }

            string username = usernameTextBox.Text;

            if (DataBaseConnectionClass.GetUsersAll().Any(user => user.Username == username)) 
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
            string passwordAgain = PasswordAgainTextBox.Text;
            DateTime dateOfBirth = datePicker.Value.Date;
            
            // Определение роли пользователя в системе
            string roleInSystem = (teacherRadioButton.Checked) ? "teacher" : "student";
            
            // Проверка, что пароль введен правильно дважды
            if (password != passwordAgain)
            {
                MessageBox.Show("Passwords doesn't much each other.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Формирование SQL-запроса для добавления пользователя
            string query = "INSERT INTO login (username, password) VALUES (@username, @password);";

            // Создание и выполнение команды с параметрами
            using (MySqlCommand command = new MySqlCommand(query, DataBaseConnectionClass.GetConnection()))
            {
                // Добавление параметров к команде
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                // Открытие соединения с базой данных
                DataBaseConnectionClass.OpenConnection();

                // Выполнение команды
                int rowsAffected = command.ExecuteNonQuery();

                // Закрытие соединения с базой данных
                DataBaseConnectionClass.CloseConnection();

                // Проверка успешности выполнения запроса
                if (rowsAffected <= 0)
                {
                    MessageBox.Show("Error occured. Please, try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Формирование SQL-запроса для добавления пользователя
            query = "INSERT INTO users (username, first_name, last_name, sex, email, phone, date_of_birth, role_in_system) " +
                           "VALUES (@username, @firstName, @lastName, @sex, @email, @phone, @dateOfBirth, @roleInSystem);";

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
                    MessageBox.Show("User successfully registered.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Очистка полей ввода после успешного добавления пользователя
                    ClearInputFields();
                }
                else
                {
                    MessageBox.Show("Error occured. Please, try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Метод для очистки полей ввода на форме
        private void ClearInputFields()
        {
            nameTextBox.Text = "Name";
            nameTextBox.ForeColor = Color.Gray;
            surnameTextBox.Text = "Surname";
            surnameTextBox.ForeColor = Color.Gray;
            usernameTextBox.Text = "Username";
            usernameTextBox.ForeColor = Color.Gray;
            emailTextBox.Text = "e-mail";
            emailTextBox.ForeColor = Color.Gray;
            phoneTextBox.Text = "Phone number";
            phoneTextBox.ForeColor = Color.Gray;
            PasswordTextBox.Text = "Password";
            PasswordTextBox.ForeColor = Color.Gray;
            PasswordTextBox.PasswordChar = '\0';
            PasswordAgainTextBox.Text = "Enter password again";
            PasswordAgainTextBox.ForeColor = Color.Gray;
            PasswordAgainTextBox.PasswordChar = '\0';
            datePicker.Value = DateTime.Today;
        }
    }
}
