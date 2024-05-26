using Kursova2023_2024.Classes.Data_classes;
using Kursova2023_2024.Classes.Tool_classes;
using MySql.Data.MySqlClient;

namespace Kursova2023_2024.Forms
{
    public partial class MainMenu : Form
    {
        private TrackBar MinTrackBar;
        private TrackBar MaxTrackBar;
        private Label leftValueLabel;
        private Label rightValueLabel;
        private FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
        private CheckedListBox checkedListBox = new CheckedListBox();
        private TextBox textBox = new TextBox();

        private List<Vacancy> vacanciesAllOpened;
        private List<int> vacanciesIdStudentResponded = DataBaseConnectionClass.GetRespondedVacanciesIdForStudent();
        private List<int> vacanciesIdStudentRespondedByTeacher = DataBaseConnectionClass.GetRespondedVacanciesIdForStudentByTeacher();
        private List<PictureBox> pictureBoxList = new List<PictureBox>();

        public MainMenu(List<Vacancy> vacanciesCurrent)
        {
            InitializeComponent();
            ControlPropertiesClass.SetToDefaultForm(this);
            ControlPropertiesClass.AddMenuStripToForm(this);
            vacanciesAllOpened = vacanciesCurrent;

            int maxValueTrackBar;
            int minValueTrackBar;
            if (vacanciesAllOpened.Count != 0)
            {
                maxValueTrackBar = (int)Math.Ceiling(vacanciesAllOpened.Max(vacancy => vacancy.HourlyRate));
                minValueTrackBar = (int)Math.Floor(vacanciesAllOpened.Min(vacancy => vacancy.HourlyRate));
            }
            else
            {
                maxValueTrackBar = 0; 
                minValueTrackBar = 0;
            }

            // Настраиваем FlowLayoutPanel
            flowLayoutPanel.Size = new Size(735, 510);
            flowLayoutPanel.Top = 25;
            flowLayoutPanel.Left = 220;
            flowLayoutPanel.AutoScroll = true; // Включаем автоматическую прокрутку

            // Создаем объекты для добавления в FlowLayoutPanel
            UpdateFlowLayoutPanel(vacanciesAllOpened);
            this.Controls.Add(flowLayoutPanel);

            // Установка свойств TextBox
            textBox.Location = new System.Drawing.Point(5, 70);
            textBox.Size = new System.Drawing.Size(195, 20);
            ControlPropertiesClass.SetToDefaultTextBoxNames(textBox, "Enter name/surname");
            textBox.KeyDown += TextBoxSearch_KeyDown;

            // Добавление TextBox на форму
            this.Controls.Add(textBox);

            // Создание Label для отображения значения "Цена за занятия"
            Label searchLabel = new Label();
            searchLabel.Location = new System.Drawing.Point(5, 30);
            searchLabel.AutoSize = true;
            ControlPropertiesClass.SetToDefaultHeaderLabelFont(searchLabel, "Search");
            this.Controls.Add(searchLabel);

            // Создание Label для отображения значения "Цена за занятия"
            Label priceLabel = new Label();
            priceLabel.Location = new System.Drawing.Point(5, 110);
            priceLabel.AutoSize = true;
            ControlPropertiesClass.SetToDefaultHeaderLabelFont(priceLabel, "Price for lesson");

            // Создание первого TrackBar (Min)
            MinTrackBar = new TrackBar();
            MinTrackBar.Location = new System.Drawing.Point(5, 160);
            MinTrackBar.Size = new System.Drawing.Size(195, 20);
            MinTrackBar.Minimum = minValueTrackBar;
            MinTrackBar.Maximum = maxValueTrackBar;
            MinTrackBar.Value = minValueTrackBar;
            MinTrackBar.TickFrequency = 10; // Шаг ползунка
            MinTrackBar.Scroll += TrackBar_Scroll; // Обработчик события прокрутки

            // Создание второго TrackBar (Max)
            MaxTrackBar = new TrackBar();
            MaxTrackBar.Location = new System.Drawing.Point(5, 240);
            MaxTrackBar.Size = new System.Drawing.Size(195, 20);
            MaxTrackBar.Minimum = minValueTrackBar;
            MaxTrackBar.Maximum = maxValueTrackBar;
            MaxTrackBar.Value = maxValueTrackBar; // Начальное значение
            MaxTrackBar.TickFrequency = 10; // Шаг ползунка
            MaxTrackBar.Scroll += TrackBar_Scroll; // Обработчик события прокрутки

            // Создание Label для отображения значения TrackBar (Min)
            leftValueLabel = new Label();
            leftValueLabel.Location = new System.Drawing.Point(10, 210);
            leftValueLabel.AutoSize = true;
            UpdateValueLabel(leftValueLabel, MinTrackBar.Value, MinTrackBar); // Установка начального значения

            // Создание Label для отображения значения TrackBar (Max)
            rightValueLabel = new Label();
            rightValueLabel.Location = new System.Drawing.Point(10, 290);
            rightValueLabel.AutoSize = true;
            UpdateValueLabel(rightValueLabel, MaxTrackBar.Value, MaxTrackBar); // Установка начального значения

            // Добавление элементов на форму
            this.Controls.Add(priceLabel);
            this.Controls.Add(MinTrackBar);
            this.Controls.Add(MaxTrackBar);
            this.Controls.Add(leftValueLabel);
            this.Controls.Add(rightValueLabel);

            // Настройка CheckedListBox
            ControlPropertiesClass.SetToDefaultCheckedListBox(checkedListBox);

            // Установка свойств CheckedListBox
            checkedListBox.Location = new System.Drawing.Point(10, 320); // Установка положения по X и Y
            checkedListBox.Size = new System.Drawing.Size(195, 140); // Установка ширины и высоты

            // Добавление элементов в CheckedListBox
            RefreshCheckedListBox();

            // Добавление CheckedListBox на форму
            this.Controls.Add(checkedListBox);

            Button buttonFilter = new Button();
            ControlPropertiesClass.SetToDefaultButton(buttonFilter, "Apply filter");
            buttonFilter.Size = new Size(195, 40);
            buttonFilter.Location = new Point(10, 470);
            buttonFilter.Click += ButtonFilter_Click;

            this.Controls.Add(buttonFilter);

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

        // Обработчик события прокрутки TrackBar
        private void TrackBar_Scroll(object sender, EventArgs e)
        {
            TrackBar trackBar = (TrackBar)sender;
            Label valueLabel = (trackBar == MinTrackBar) ? leftValueLabel : rightValueLabel;
            UpdateValueLabel(valueLabel, trackBar.Value, trackBar); // Обновление отображаемого значения в соответствующем Label
            if (trackBar != MaxTrackBar)
            { //MinTrackBar
                if (MaxTrackBar.Value - 1 < MinTrackBar.Value)
                {
                    MinTrackBar.Maximum = MinTrackBar.Value;
                } else {
                    if (MinTrackBar.Minimum != MinTrackBar.Maximum - 1)
                    {
                        MinTrackBar.Maximum = MaxTrackBar.Value - 1;
                        MaxTrackBar.Minimum = MinTrackBar.Value;
                    }
                }
            } else
            { //MaxTrackBar
                if (MinTrackBar.Value + 1 > MaxTrackBar.Value)
                {
                    MaxTrackBar.Minimum = MaxTrackBar.Value;
                }
                else
                {
                    if (MaxTrackBar.Minimum + 1 != MaxTrackBar.Maximum)
                    {
                        MaxTrackBar.Minimum = MinTrackBar.Value + 1;
                        MinTrackBar.Maximum = MaxTrackBar.Value;
                    }
                }
            }
        }

        private void RefreshCheckedListBox()
        {
            checkedListBox.Items.Clear();
            foreach (var subject in GetSubjectsForCheckedListBox())
            {
                checkedListBox.Items.Add(subject);
            }
        }

        private List<string> GetSubjectsForCheckedListBox() => vacanciesAllOpened.Select(vacancy => vacancy.Subject).Distinct().ToList();

        // Метод для обновления отображаемого значения в Label
        private void UpdateValueLabel(Label label, int value, TrackBar trackBar)
        {
            String text = (trackBar == MinTrackBar) ? "Minimum" : "Maximum";
            label.Text = $"{text} UAH: {value}";
        }

        private void ButtonOnPanelContact_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            Panel parentPanel = (clickedButton.Parent != null) ? (Panel)clickedButton.Parent : throw new Exception("Parent panel is null");

            int id_vacancy = (parentPanel.Tag != null) ? (int)parentPanel.Tag : -1;

            // Формирование SQL-запроса для добавления пользователя
            string query = "INSERT INTO response_to_vacancies (id_vacancy, id_user, status_response, response_date) " +
                           "VALUES (@id_vacancy, @id_user, @status_response, @response_date);";

            // Создание и выполнение команды с параметрами
            using (MySqlCommand command = new MySqlCommand(query, DataBaseConnectionClass.GetConnection()))
            {
                DateTime currentDate = DateTime.Now;
                string formattedDate = currentDate.ToString("yyyy-MM-dd HH:mm:ss");

                // Добавление параметров к команде
                command.Parameters.AddWithValue("@id_vacancy", id_vacancy);
                command.Parameters.AddWithValue("@id_user", ControlPropertiesClass.CurrentUser.Id);
                command.Parameters.AddWithValue("@status_response", "На перегляді");
                command.Parameters.AddWithValue("@response_date", currentDate);

                // Открытие соединения с базой данных
                DataBaseConnectionClass.OpenConnection();

                // Выполнение команды
                int rowsAffected = command.ExecuteNonQuery();

                // Закрытие соединения с базой данных
                DataBaseConnectionClass.CloseConnection();

                // Проверка успешности выполнения запроса
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Response successfully registered.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Error occured. Please, try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            vacanciesAllOpened.RemoveAll(v => v.Id == id_vacancy);

            UpdateFlowLayoutPanel(vacanciesAllOpened);
        }

        private void ButtonOnPanelDeleteContact_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            Panel parentPanel = (clickedButton.Parent != null) ? (Panel)clickedButton.Parent : throw new Exception("Parent panel is null");

            int id_vacancy = (parentPanel.Tag != null) ? (int)parentPanel.Tag : -1;

            // Формирование SQL-запроса для добавления пользователя
            string query = "UPDATE response_to_vacancies SET status_response = \"Відхилено\"" +
                           "WHERE id_user = @id_student AND id_vacancy = @id_vacancy" +
                           " AND (status_response = \"Прийняте\" OR status_response = \"На перегляді\");";

            // Создание и выполнение команды с параметрами
            using (MySqlCommand command = new MySqlCommand(query, DataBaseConnectionClass.GetConnection()))
            {
                // Добавление параметров к команде
                command.Parameters.AddWithValue("@id_student", ControlPropertiesClass.CurrentUser.Id);
                command.Parameters.AddWithValue("@id_vacancy", id_vacancy);

                // Открытие соединения с базой данных
                DataBaseConnectionClass.OpenConnection();

                // Выполнение команды
                int rowsAffected = command.ExecuteNonQuery();

                // Закрытие соединения с базой данных
                DataBaseConnectionClass.CloseConnection();

                // Проверка успешности выполнения запроса
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Vacancy successfully deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Очистка полей ввода после успешного добавления пользователя
                }
                else
                {
                    MessageBox.Show("Error occured. Please, try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            vacanciesAllOpened.RemoveAll(v => v.Id == id_vacancy);

            UpdateFlowLayoutPanel(vacanciesAllOpened);
        }

        private void ButtonOnPanelDeleteVacancy_Click(object sender, EventArgs e)
        {
            // Формирование текста сообщения
            string message = $"Ви впевненні, що хочете видалити цю вакансію?";

            // Показ диалогового окна с подтверждением
            DialogResult result = MessageBox.Show(message, "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Проверка результата диалога
            if (result == DialogResult.Yes)
            {
                Button clickedButton = (Button)sender;
                Panel parentPanel = (clickedButton.Parent != null) ? (Panel)clickedButton.Parent : throw new Exception("Parent panel is null");

                int id_vacancy = (parentPanel.Tag != null) ? (int)parentPanel.Tag : -1;
                DataBaseConnectionClass.OpenConnection();
                try
                {

                    // Формирование SQL-запроса для добавления пользователя
                    string query = "DELETE FROM response_to_vacancies WHERE id_vacancy = @id_vacancy;";

                    // Создание и выполнение команды с параметрами
                    using (MySqlCommand command = new MySqlCommand(query, DataBaseConnectionClass.GetConnection()))
                    {
                        // Добавление параметров к команде
                        command.Parameters.AddWithValue("@id_vacancy", id_vacancy);
                        
                        // Выполнение команды
                        int rowsAffected = command.ExecuteNonQuery();

                        // Проверка успешности выполнения запроса
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Vacancy responses successfully deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // Очистка полей ввода после успешного добавления пользователя
                        }
                    }

                    query = "DELETE FROM vacancies WHERE id_vacancy = @id_vacancy;";

                    // Создание и выполнение команды с параметрами
                    using (MySqlCommand command = new MySqlCommand(query, DataBaseConnectionClass.GetConnection()))
                    {
                        // Добавление параметров к команде
                        command.Parameters.AddWithValue("@id_vacancy", id_vacancy);

                        // Выполнение команды
                        int rowsAffected = command.ExecuteNonQuery();

                        // Проверка успешности выполнения запроса
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Vacancy successfully deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // Очистка полей ввода после успешного добавления пользователя
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("An error occurred while deleting vacancy: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }

                vacanciesAllOpened.RemoveAll(v => v.Id == id_vacancy);

                UpdateFlowLayoutPanel(vacanciesAllOpened);
            }
        }

        private void ButtonFilter_Click(object sender, EventArgs e)
        {
            // Получаем выбранные предметы из CheckedListBox
            List<string> selectedSubjects = checkedListBox.CheckedItems.Cast<string>().ToList();

            // Фильтруем вакансии по условиям
            List<Vacancy> filteredVacancies;
            if (selectedSubjects.Any())
            {
                filteredVacancies = vacanciesAllOpened
                    .Where(v => v.HourlyRate >= MinTrackBar.Value && v.HourlyRate <= MaxTrackBar.Value && selectedSubjects.Contains(v.Subject))
                    .ToList();
            }
            else
            {
                // Если ни один предмет не выбран, то возвращаем все вакансии без фильтрации по предметам
                filteredVacancies = vacanciesAllOpened
                    .Where(v => v.HourlyRate >= MinTrackBar.Value && v.HourlyRate <= MaxTrackBar.Value)
                    .ToList();
            }

            if(textBox.Text != "Enter name/surname")
                filteredVacancies = FindSearchInTextBox(filteredVacancies, (textBox.Text));

            // Обновляем FlowLayoutPanel с отфильтрованными вакансиями
            UpdateFlowLayoutPanel(filteredVacancies);
        }

        private void UpdateFlowLayoutPanel(List<Vacancy> vacancies)
        {
            // Очищаем существующие элементы в FlowLayoutPanel
            flowLayoutPanel.Controls.Clear();
            RefreshCheckedListBox();

            // Освобождаем ресурсы всех PictureBox, которые находятся в списке pictureBoxList
            foreach (PictureBox pictureBox in pictureBoxList)
            {
                pictureBox.Dispose();
            }
            // Очищаем список pictureBoxList
            pictureBoxList.Clear();

            // Создаем новые элементы на основе обновленных данных
            foreach (Vacancy vacancy in vacancies)
            {
                IUser user = FindUserById(vacancy.UserId);
                // Создаем контейнер Panel для каждого объекта
                Panel itemPanel = new Panel();
                itemPanel.Size = new Size(680, 100);
                itemPanel.BorderStyle = BorderStyle.FixedSingle;
                itemPanel.Tag = vacancy.Id; // Сохраняем индекс в свойстве Tag

                // Добавляем изображение
                PictureBox pictureBox = new PictureBox();
                pictureBox.Image = FindUserById(vacancy.UserId).FindImage();
                pictureBox.Size = new Size(100, 100);
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBoxList.Add(pictureBox);
                itemPanel.Controls.Add(pictureBox);

                // Добавляем текст и теги 
                Label textLabel = new Label();
                ControlPropertiesClass.SetToDefaultLabelFont(textLabel, $"{user.FirstName} {user.LastName}");
                textLabel.AutoSize = true;
                textLabel.Location = new Point(110, 10);
                itemPanel.Controls.Add(textLabel);

                Label tagLabel = new Label();
                ControlPropertiesClass.SetToDefaultLabelFont(tagLabel, vacancy.Subject);
                tagLabel.AutoSize = true;
                tagLabel.Location = new Point(110, 40);
                itemPanel.Controls.Add(tagLabel);

                // Добавляем текст и теги 
                Label hourlyRateLabel = new Label();
                ControlPropertiesClass.SetToDefaultLabelFont(hourlyRateLabel, $"{vacancy.HourlyRate} UAH per hour");
                hourlyRateLabel.AutoSize = true;

                switch (ControlPropertiesClass.CurrentUser)
                {
                    case Student:
                        Button buttonOnPanel = new Button();
                        buttonOnPanel.Size = new Size(150, 40);
                        buttonOnPanel.Location = new Point(500, 40);

                        if (!vacanciesIdStudentResponded.Contains(vacancy.Id))
                        {
                            ControlPropertiesClass.SetToDefaultButton(buttonOnPanel, "Contact tutor");
                            buttonOnPanel.Click += ButtonOnPanelContact_Click;
                        }
                        else
                        {
                            ControlPropertiesClass.SetToDefaultButton(buttonOnPanel, "Delete contact");
                            buttonOnPanel.Click += ButtonOnPanelDeleteContact_Click;
                            Label labelOnPanel = new Label();
                            labelOnPanel.Size = new Size(150, 40);
                            labelOnPanel.Location = new Point(305, 35);
                            string text = (vacanciesIdStudentRespondedByTeacher.Contains(vacancy.Id)) ? FindUserById(vacancy.UserId).Phone : "Waiting response";
                            ControlPropertiesClass.SetToDefaultLabelFont(labelOnPanel, text);
                            itemPanel.Controls.Add(labelOnPanel);
                        }

                        itemPanel.Controls.Add(buttonOnPanel);
                        hourlyRateLabel.Location = new Point(500, 10);
                        break;
                    case Admin:
                        pictureBox.Click += PictureOnPanel_Click;

                        // Добавляем переключатель
                        CheckBox publishedCheckBox = new CheckBox();
                        ControlPropertiesClass.SetToDefaultCheckBox(publishedCheckBox, "Published");
                        publishedCheckBox.AutoSize = true;
                        publishedCheckBox.Location = new Point(560, 40);
                        publishedCheckBox.Checked = true;
                        
                        EventHandler publishedCheckBoxCheckedChanged = null;
                        publishedCheckBoxCheckedChanged = (sender, args) =>
                        {
                            // Отключаем обработку событий временно
                            publishedCheckBox.CheckedChanged -= publishedCheckBoxCheckedChanged;

                            DialogResult result = MessageBox.Show("Are you sure to unpublish this vacancy?", "Confirm action", MessageBoxButtons.OKCancel);

                            if (result == DialogResult.OK)
                            {
                                ChangePublishVacancy(vacancy.Id);
                                if (publishedCheckBox.Checked)
                                    vacanciesAllOpened.Add(vacancy);
                                else
                                    vacanciesAllOpened.RemoveAll(v => v.Id == vacancy.Id);
                                UpdateFlowLayoutPanel(vacancies);
                            }
                            else
                            {
                                // Если пользователь отменил действие, возвращаем CheckBox в исходное состояние
                                publishedCheckBox.Checked = !publishedCheckBox.Checked;
                            }

                            // Включаем обработку событий обратно
                            publishedCheckBox.CheckedChanged += publishedCheckBoxCheckedChanged;
                        };

                        publishedCheckBox.CheckedChanged += publishedCheckBoxCheckedChanged;

                        itemPanel.Controls.Add(publishedCheckBox);
                        hourlyRateLabel.Location = new Point(400, 40);
                        break;
                    case Teacher:
                        if (vacancy.UserId == ControlPropertiesClass.CurrentUser.Id)
                        {
                            hourlyRateLabel.Location = new Point(500, 10);
                            buttonOnPanel = new Button();
                            buttonOnPanel.Size = new Size(150, 40);
                            buttonOnPanel.Location = new Point(500, 40);
                            ControlPropertiesClass.SetToDefaultButton(buttonOnPanel, "Delete vacancy");
                            buttonOnPanel.Click += ButtonOnPanelDeleteVacancy_Click;
                            itemPanel.Controls.Add(buttonOnPanel);

                            Label publishedLabel = new Label();
                            ControlPropertiesClass.SetToDefaultHeaderLabelFont(publishedLabel, $"{vacancy.Status}");
                            publishedLabel.AutoSize = true;
                            publishedLabel.Location = new Point(305, 35);
                            itemPanel.Controls.Add(publishedLabel);
                        }
                        else { hourlyRateLabel.Location = new Point(500, 40); }
                        break;
                }
                itemPanel.Controls.Add(hourlyRateLabel);

                // Добавляем созданный контейнер в FlowLayoutPanel
                flowLayoutPanel.Controls.Add(itemPanel);
            }
        }

        private void ChangePublishVacancy(int idVacancy)
        {
            // Формирование SQL-запроса для добавления пользователя
            string query = "UPDATE vacancies SET status_vacancy = CASE " +
                "WHEN status_vacancy = 'Розміщено' THEN 'Не розміщено' " +
                "WHEN status_vacancy = 'Не розміщено' THEN 'Розміщено' END " +
                "WHERE id_vacancy = @id_vacancy;";

            // Создание и выполнение команды с параметрами
            using (MySqlCommand command = new MySqlCommand(query, DataBaseConnectionClass.GetConnection()))
            {
                // Добавление параметров к команде
                command.Parameters.AddWithValue("@id_vacancy", idVacancy);

                // Открытие соединения с базой данных
                DataBaseConnectionClass.OpenConnection();

                // Выполнение команды
                int rowsAffected = command.ExecuteNonQuery();

                // Закрытие соединения с базой данных
                DataBaseConnectionClass.CloseConnection();

                // Проверка успешности выполнения запроса
                if (rowsAffected <= 0)
                {
                    MessageBox.Show("Error occured while changing vacancy. Please, try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void PictureOnPanel_Click(object sender, EventArgs e)
        {
            PictureBox pictureBox = sender as PictureBox;
            if (pictureBox != null && pictureBox.Parent != null)
            {
                int id = (pictureBox.Parent.Tag != null) ? (int)pictureBox.Parent.Tag : -1;
                if (id != -1)
                {
                    Profile profile = new Profile(FindUserById(FindVacancyById(id).UserId));
                    profile.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Parent control's Tag is null or invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void TextBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            // Проверяем, была ли нажата клавиша Enter
            if (e.KeyCode == Keys.Enter)
            {
                // Обновляем FlowLayoutPanel с отфильтрованными вакансиями
                UpdateFlowLayoutPanel(FindSearchInTextBox(vacanciesAllOpened, (textBox.Text)));
            }
        }

        private List<Vacancy> FindSearchInTextBox(List<Vacancy> searchedVacancies, string searchText) {
            // Фильтруем вакансии по имени или фамилии пользователя
            return searchedVacancies
                .Where(v =>
                {
                    IUser user = FindUserById(v.UserId);
                    return user != null && (user.FirstName.Contains(searchText, StringComparison.OrdinalIgnoreCase) || user.LastName.Contains(searchText, StringComparison.OrdinalIgnoreCase));
                })
                .ToList();
        }

        private IUser? FindUserById(int id) => DataBaseConnectionClass.GetUsersNotBanned().FirstOrDefault(user => user.Id == id);

        private Vacancy? FindVacancyById(int id) => vacanciesAllOpened.FirstOrDefault(vacancy => vacancy.Id == id);
    }
}
