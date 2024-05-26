using Kursova2023_2024.Classes.Tool_classes;
using MySql.Data.MySqlClient;
using System.Data;

namespace Kursova2023_2024.Forms.Teacher_forms
{
    public partial class AddNewVacancyForm : Form
    {
        private TextBox hourlyPaymentTextBox = new TextBox();
        private ComboBox subjectComboBox = new ComboBox();

        public AddNewVacancyForm()
        {
            InitializeComponent();
            ControlPropertiesClass.SetToDefaultForm(this);
            ControlPropertiesClass.AddMenuStripToForm(this);

            ControlPropertiesClass.SetToDefaultComboBox(subjectComboBox);
            subjectComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            subjectComboBox.Location = new System.Drawing.Point(250, 200);
            subjectComboBox.Size = new System.Drawing.Size(190, 20);
            foreach (var subject in GetSubjectsForCheckedListBox())
            {
                subjectComboBox.Items.Add(subject);
            }
            subjectComboBox.SelectedIndex = 0;

            this.Controls.Add(subjectComboBox);

            // Установка свойств hourlyPaymentTextBox
            hourlyPaymentTextBox.Location = new System.Drawing.Point(480, 200);
            hourlyPaymentTextBox.Size = new System.Drawing.Size(190, 20);
            ControlPropertiesClass.SetToDefaultTextBoxDigits(hourlyPaymentTextBox, "Hour payment");

            this.Controls.Add(hourlyPaymentTextBox);

            Button buttonRegisterVacancy = new Button();
            ControlPropertiesClass.SetToDefaultButton(buttonRegisterVacancy, "Register Vacancy");
            buttonRegisterVacancy.Size = new Size(175, 40);
            buttonRegisterVacancy.Location = new Point(365, 350);
            buttonRegisterVacancy.Click += ButtonRegister_Click;

            this.Controls.Add(buttonRegisterVacancy);
        }

        private bool checkForText() => hourlyPaymentTextBox.Text == "Hour payment";

        private void ButtonRegister_Click(object sender, EventArgs e)
        {
            if (checkForText())
            {
                MessageBox.Show("Please, enter all fields", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sex = subjectComboBox.Text;
            string email = hourlyPaymentTextBox.Text;

            // Формирование SQL-запроса для добавления пользователя
            string query = "INSERT INTO vacancies (id_user, hourly_rate, subject_of_studying, status_vacancy) " +
                "VALUES (@id_teacher, @hourly_rate, @subject_of_studying, \"Не розміщено\");";

            // Создание и выполнение команды с параметрами
            using (MySqlCommand command = new MySqlCommand(query, DataBaseConnectionClass.GetConnection()))
            {
                // Добавление параметров к команде
                command.Parameters.AddWithValue("@id_teacher", ControlPropertiesClass.CurrentUser.Id);
                command.Parameters.AddWithValue("@hourly_rate", hourlyPaymentTextBox.Text);
                command.Parameters.AddWithValue("@subject_of_studying", subjectComboBox.Text);

                // Открытие соединения с базой данных
                DataBaseConnectionClass.OpenConnection();

                // Выполнение команды
                int rowsAffected = command.ExecuteNonQuery();

                // Закрытие соединения с базой данных
                DataBaseConnectionClass.CloseConnection();

                // Проверка успешности выполнения запроса
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Vacancy successfully registered. Please, wait untill it will be published by administration.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Error occured. Please, try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private List<string> GetSubjectsForCheckedListBox() => DataBaseConnectionClass.GetActualVacancies().Select(vacancy => vacancy.Subject).Distinct().ToList();
    }
}
