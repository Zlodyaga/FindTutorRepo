using Kursova2023_2024.Classes.Data_classes;
using Kursova2023_2024.Classes.Tool_classes;
using MySql.Data.MySqlClient;

namespace Kursova2023_2024.Forms.Teacher_forms
{
    public partial class ResponsesFromStudentsForm : Form
    {
        private List<Pair<int, int>> vacanciesAllWithStudents;
        private List<Pair<int, int>> vacanciesApplyedWithStudents;
        private List<IUser> users;
        private List<Vacancy> vacancies;

        private FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
        private List<PictureBox> pictureBoxList = new List<PictureBox>();

        public ResponsesFromStudentsForm()
        {
            InitializeComponent();
            ControlPropertiesClass.SetToDefaultForm(this);
            ControlPropertiesClass.AddMenuStripToForm(this);
            RefreshLists();

            // Создаем FlowLayoutPanel
            flowLayoutPanel.Size = new Size(935, 540);
            flowLayoutPanel.Top = 25;
            flowLayoutPanel.Left = 25;
            flowLayoutPanel.AutoScroll = true; // Включаем автоматическую прокрутку

            // Создаем объекты для добавления в FlowLayoutPanel
            UpdateUsersInFlowLayoutPanel(vacanciesAllWithStudents);

            // Добавляем FlowLayoutPanel на форму
            this.Controls.Add(flowLayoutPanel);

            this.FormClosing += MainForm_FormClosing;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Освобождаем ресурсы всех PictureBox при замене изображений
            foreach (PictureBox pictureBox in pictureBoxList)
            {
                pictureBox.Dispose();
            }
            // Очищаем список pictureBoxList
            pictureBoxList.Clear();
            GC.Collect();
        }

        private void ProfilePictureOnPanel_Click(object sender, EventArgs e)
        {
            PictureBox clickedPicture = (PictureBox)sender;
            Panel parentPanel = (clickedPicture.Parent != null) ? (Panel)clickedPicture.Parent : throw new Exception("Parent panel is null");

            int id = (parentPanel.Tag != null) ? (int)parentPanel.Tag : -1;

            // Теперь у вас есть индекс объекта, на который был совершен клик
            Profile profile = new Profile(FindUserById(id));
            profile.Show();
            this.Close();
        }

        private void UpdateUsersInFlowLayoutPanel(List<Pair<int, int>> usersWithVacanciesList)
        {
            // Очищаем существующие элементы в FlowLayoutPanel
            flowLayoutPanel.Controls.Clear();

            foreach (Pair<int, int> userWithVacancy in usersWithVacanciesList)
            {
                IUser user = FindUserById(userWithVacancy.First);
                Vacancy vacancy = FindVacancyById(userWithVacancy.Second);

                // Создаем контейнер Panel для каждого объекта
                Panel itemPanel = new Panel();
                itemPanel.Size = new Size(900, 100);
                itemPanel.BorderStyle = BorderStyle.FixedSingle;
                itemPanel.Tag = user.Id; // Сохраняем индекс в свойстве Tag

                // Добавляем изображение
                PictureBox pictureBox = new PictureBox();
                pictureBox.Image = user.FindImage();
                pictureBox.Size = new Size(100, 100);
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox.Click += ProfilePictureOnPanel_Click;
                pictureBoxList.Add(pictureBox);
                itemPanel.Controls.Add(pictureBox);

                // Добавляем текст и теги 
                Label textLabel = new Label();
                ControlPropertiesClass.SetToDefaultLabelFont(textLabel, $"{user.FirstName} {user.LastName}");
                textLabel.AutoSize = true;
                textLabel.Location = new Point(110, 10);
                itemPanel.Controls.Add(textLabel);

                Label usernameLabel = new Label();
                string text = CheckForApply(userWithVacancy) ? "Прийняте" : "На перегляді";
                ControlPropertiesClass.SetToDefaultLabelFont(usernameLabel, text);
                usernameLabel.AutoSize = true;
                usernameLabel.Location = new Point(110, 40);
                itemPanel.Controls.Add(usernameLabel);

                Label subjectLabel = new Label();
                ControlPropertiesClass.SetToDefaultHeaderLabelFont(subjectLabel, $"{vacancy.Subject} {vacancy.HourlyRate} UAH per hour");
                subjectLabel.AutoSize = true;
                subjectLabel.Location = new Point(305, 35);
                itemPanel.Controls.Add(subjectLabel);
                if (!CheckForApply(userWithVacancy))
                {
                    Button buttonApplyStudent = new Button();
                    ControlPropertiesClass.SetToDefaultButton(buttonApplyStudent, "Apply");
                    buttonApplyStudent.Size = new Size(100, 40);
                    buttonApplyStudent.Location = new Point(680, 30);
                    buttonApplyStudent.Tag = vacancy.Id;
                    buttonApplyStudent.Click += ButtonApplyStudent_Click;

                    itemPanel.Controls.Add(buttonApplyStudent);
                }

                // Добавляем изображение
                PictureBox deletePictureBox = new PictureBox();
                deletePictureBox.Image = FindImageByName("Delete"); // Замените yourImage на ваше изображение
                deletePictureBox.Size = new Size(30, 30);
                deletePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                deletePictureBox.Location = new Point(820, 35);
                deletePictureBox.Tag = vacancy.Id;
                ControlPropertiesClass.SetToDefaultClickablePicture(deletePictureBox);
                deletePictureBox.Click += (sender, args) =>
                {
                    DeletePictureOnPanel_Click(sender, args);
                    RefreshLists();
                    UpdateUsersInFlowLayoutPanel(vacanciesAllWithStudents);
                };

                itemPanel.Controls.Add(deletePictureBox);

                // Добавляем созданный контейнер в FlowLayoutPanel
                flowLayoutPanel.Controls.Add(itemPanel);
            }
        }

        private bool CheckForApply(Pair<int, int> userWithVacancy) => vacanciesApplyedWithStudents.Any(v =>
        v.First == userWithVacancy.First &&
        v.Second == userWithVacancy.Second &&
        v.time == userWithVacancy.time);

        private void RefreshLists()
        {
            vacanciesAllWithStudents = DataBaseConnectionClass.GetAppliedAndRespondedVacanciesIdForTeacher();
            vacanciesApplyedWithStudents = DataBaseConnectionClass.GetAppliedVacanciesIdForTeacher();
            vacancies = DataBaseConnectionClass.GetActualVacancies();

            users = DataBaseConnectionClass
                .GetUsersNotBanned()
                .Where(user => vacanciesAllWithStudents.Any(pair => pair.First == user.Id))
                .ToList();
            if (users.Count == 0)
            {
                Label usernameLabel = new Label();
                ControlPropertiesClass.SetToDefaultHeaderLabelFont(usernameLabel, "Sorry, you don't have any responses from students");
                usernameLabel.AutoSize = true;
                usernameLabel.Location = new Point(230, 258); //430, 258
                this.Controls.Add(usernameLabel);
            }
        }

        private void ButtonApplyStudent_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            Panel parentPanel = (clickedButton.Parent != null) ? (Panel)clickedButton.Parent : throw new Exception("Parent panel is null");

            int id_user = (parentPanel.Tag != null) ? (int)parentPanel.Tag : -1;
            int id_vacancy = (clickedButton.Tag != null) ? (int)clickedButton.Tag : -1;
            string date = FindVacancyTime(id_user, id_vacancy).time.ToString("yyyy-MM-dd HH:mm:ss");

            // Формирование SQL-запроса для добавления пользователя
            string query = "UPDATE response_to_vacancies SET status_response = \"Прийняте\"" +
                "WHERE id_vacancy = @id_vacancy AND id_user = @id_user AND response_date = @response_date;";

            // Создание и выполнение команды с параметрами
            using (MySqlCommand command = new MySqlCommand(query, DataBaseConnectionClass.GetConnection()))
            {
                // Добавление параметров к команде
                command.Parameters.AddWithValue("@id_user", id_user);
                command.Parameters.AddWithValue("@id_vacancy", id_vacancy);
                command.Parameters.AddWithValue("@response_date", date);

                // Открытие соединения с базой данных
                DataBaseConnectionClass.OpenConnection();

                // Выполнение команды
                int rowsAffected = command.ExecuteNonQuery();

                // Закрытие соединения с базой данных
                DataBaseConnectionClass.CloseConnection();

                // Проверка успешности выполнения запроса
                if (rowsAffected > 0)
                {
                    MessageBox.Show("User successfully applyed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Очистка полей ввода после успешного добавления пользователя
                }
                else
                {
                    MessageBox.Show("Error occured. Please, try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            RefreshLists();
            UpdateUsersInFlowLayoutPanel(vacanciesAllWithStudents);
        }

        private void DeletePictureOnPanel_Click(object sender, EventArgs e)
        {
            // Формирование текста сообщения
            string message = $"Ви впевненні, що хочете видалити цей контакт?";

            // Показ диалогового окна с подтверждением
            DialogResult result = MessageBox.Show(message, "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Проверка результата диалога
            if (result == DialogResult.Yes)
            {
                PictureBox clickedPicture = (PictureBox)sender;
                Panel parentPanel = (clickedPicture.Parent != null) ? (Panel)clickedPicture.Parent : throw new Exception("Parent panel is null");

                int id_user = (parentPanel.Tag != null) ? (int)parentPanel.Tag : -1;
                int id_vacancy = (clickedPicture.Tag != null) ? (int)clickedPicture.Tag : -1;

                // Формирование SQL-запроса для добавления пользователя
                string query = "UPDATE response_to_vacancies SET status_response = \"Відхилено\"" +
                    "WHERE id_vacancy = @id_vacancy AND id_user = @id_user AND response_date = @response_date;";

                // Создание и выполнение команды с параметрами
                using (MySqlCommand command = new MySqlCommand(query, DataBaseConnectionClass.GetConnection()))
                {
                    // Добавление параметров к команде
                    command.Parameters.AddWithValue("@id_user", id_user);
                    command.Parameters.AddWithValue("@id_vacancy", id_vacancy);
                    command.Parameters.AddWithValue("@response_date", FindVacancyTime(id_user, id_vacancy).time.ToString("yyyy-MM-dd HH:mm:ss"));

                    // Открытие соединения с базой данных
                    DataBaseConnectionClass.OpenConnection();

                    // Выполнение команды
                    int rowsAffected = command.ExecuteNonQuery();

                    // Закрытие соединения с базой данных
                    DataBaseConnectionClass.CloseConnection();

                    // Проверка успешности выполнения запроса
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("User successfully rejected.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // Очистка полей ввода после успешного добавления пользователя
                    }
                    else
                    {
                        MessageBox.Show("Error occured. Please, try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private Image FindImageByName(string name)
        {
            // Определяем путь к изображению относительно текущего расположения приложения
            string imagePath = ControlPropertiesClass.ImagePath + $"{name}_Icon.png";

            // Получаем абсолютный путь к изображению
            string absoluteImagePath = Path.Combine(Application.StartupPath, imagePath);

            // Создаем изображение из файла
            Image image = Image.FromFile(absoluteImagePath);

            return image;
        }

        private IUser FindUserById(int id) => users.FirstOrDefault(user => user.Id == id);

        private Vacancy? FindVacancyById(int id) => vacancies.FirstOrDefault(vacancy => vacancy.Id == id);

        private Pair<int, int> FindVacancyTime(int id_user, int id_vacancy) => vacanciesAllWithStudents.FirstOrDefault(obj => obj.First == id_user && obj.Second == id_vacancy);
    }
}
