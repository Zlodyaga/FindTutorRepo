using Kursova2023_2024.Classes.Data_classes;
using Kursova2023_2024.Classes.Tool_classes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kursova2023_2024.Forms.Admin_forms
{
    public partial class NewVacanciesForm : Form
    {
        private TrackBar MinTrackBar;
        private TrackBar MaxTrackBar;
        private Label leftValueLabel;
        private Label rightValueLabel;
        private FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
        private CheckedListBox checkedListBox = new CheckedListBox();
        private TextBox textBox = new TextBox();

        private List<IUser> users = DataBaseConnectionClass.GetUsersAll();
        private List<Vacancy> vacanciesAll = DataBaseConnectionClass.GetVacancies();
        private List<Vacancy> vacanciesNew;
        private List<PictureBox> pictureBoxList = new List<PictureBox>();

        public NewVacanciesForm()
        {
            InitializeComponent();
            ControlPropertiesClass.SetToDefaultForm(this);
            ControlPropertiesClass.AddMenuStripToForm(this);

            vacanciesNew = vacanciesAll.Where(v => v.Status == "Не розміщено").ToList();

            // Настраиваем FlowLayoutPanel
            flowLayoutPanel.Size = new Size(735, 510);
            flowLayoutPanel.Top = 25;
            flowLayoutPanel.Left = 220;
            flowLayoutPanel.AutoScroll = true; // Включаем автоматическую прокрутку

            // Создаем объекты для добавления в FlowLayoutPanel
            UpdateFlowLayoutPanel(vacanciesNew);
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

            int maxValueTrackBar = (int)Math.Ceiling(vacanciesNew.Max(vacancy => vacancy.HourlyRate));
            int minValueTrackBar = (int)Math.Floor(vacanciesNew.Min(vacancy => vacancy.HourlyRate));

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
                }
                else
                {
                    if (MinTrackBar.Minimum != MinTrackBar.Maximum - 1)
                    {
                        MinTrackBar.Maximum = MaxTrackBar.Value - 1;
                        MaxTrackBar.Minimum = MinTrackBar.Value;
                    }
                }
            }
            else
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

        private List<string> GetSubjectsForCheckedListBox() => vacanciesNew.Select(vacancy => vacancy.Subject).Distinct().ToList();

        // Метод для обновления отображаемого значения в Label
        private void UpdateValueLabel(Label label, int value, TrackBar trackBar)
        {
            String text = (trackBar == MinTrackBar) ? "Minimum" : "Maximum";
            label.Text = $"{text} UAH: {value}";
        }

        //TODO добавить действие к клику на карточку
        private void ButtonOnPanel_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            Panel parentPanel = (clickedButton.Parent != null) ? (Panel)clickedButton.Parent : throw new Exception("Parent panel is null");

            int id = (parentPanel.Tag != null) ? (int)parentPanel.Tag : -1;

            // Теперь у вас есть индекс объекта, на который был совершен клик
            MessageBox.Show("Clicked on panel with id vacancy: " + id);
        }

        private void ButtonFilterNewVacancies_Click(object sender, EventArgs e)
        {
            UpdateFlowLayoutPanel(vacanciesNew);
        }

        private void ButtonFilter_Click(object sender, EventArgs e)
        {
            // Получаем выбранные предметы из CheckedListBox
            List<string> selectedSubjects = checkedListBox.CheckedItems.Cast<string>().ToList();

            // Фильтруем вакансии по условиям
            List<Vacancy> filteredVacancies = vacanciesNew;
            if (selectedSubjects.Any())
            {
                filteredVacancies = filteredVacancies
                    .Where(v => v.HourlyRate >= MinTrackBar.Value && v.HourlyRate <= MaxTrackBar.Value && selectedSubjects.Contains(v.Subject))
                    .ToList();
            }
            else
            {
                // Если ни один предмет не выбран, то возвращаем все вакансии без фильтрации по предметам
                filteredVacancies = filteredVacancies
                    .Where(v => v.HourlyRate >= MinTrackBar.Value && v.HourlyRate <= MaxTrackBar.Value)
                    .ToList();
            }

            if (textBox.Text != "Enter name/surname")
                filteredVacancies = FindSearchInTextBox(filteredVacancies, (textBox.Text));

            // Обновляем FlowLayoutPanel с отфильтрованными вакансиями
            UpdateFlowLayoutPanel(filteredVacancies);
        }

        private void UpdateFlowLayoutPanel(List<Vacancy> vacancies)
        {

            // Очищаем существующие элементы в FlowLayoutPanel
            flowLayoutPanel.Controls.Clear();
            RefreshCheckedListBox();

            // Создаем новые элементы на основе обновленных данных
            foreach (Vacancy vacancy in vacancies)
            {
                IUser user = FindUserById(vacancy.UserId);
                if (checkForBan(user.Username) == "banned") { continue; }
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
                pictureBox.Click += PictureOnPanel_Click;
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
                hourlyRateLabel.Location = new Point(400, 40);

                // Добавляем переключатель
                CheckBox publishedCheckBox = new CheckBox();
                ControlPropertiesClass.SetToDefaultCheckBox(publishedCheckBox, "Published");
                publishedCheckBox.AutoSize = true;
                publishedCheckBox.Location = new Point(560, 40);
                publishedCheckBox.Checked = false;
                EventHandler publishedCheckBoxCheckedChanged = null;
                publishedCheckBoxCheckedChanged = (sender, args) =>
                {
                    // Отключаем обработку событий временно
                    publishedCheckBox.CheckedChanged -= publishedCheckBoxCheckedChanged;

                    DialogResult result = MessageBox.Show("Are you sure to publish this vacancy?", "Confirm action", MessageBoxButtons.OKCancel);

                    if (result == DialogResult.OK)
                    {
                        ChangePublishVacancy(vacancy.Id);
                        if (!publishedCheckBox.Checked)
                            vacanciesNew.Add(vacancy);
                        else
                            vacanciesNew.RemoveAll(v => v.Id == vacancy.Id);
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
                
                itemPanel.Controls.Add(hourlyRateLabel);

                // Добавляем созданный контейнер в FlowLayoutPanel
                flowLayoutPanel.Controls.Add(itemPanel);
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
                    Profile profile = new Profile(FindUserById(FindVacancyById(id, vacanciesAll).UserId));
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
                UpdateFlowLayoutPanel(FindSearchInTextBox(vacanciesNew, (textBox.Text)));
            }
        }

        private List<Vacancy> FindSearchInTextBox(List<Vacancy> searchedVacancies, string searchText)
        {
            // Фильтруем вакансии по имени или фамилии пользователя
            return searchedVacancies
                .Where(v =>
                {
                    IUser user = FindUserById(v.UserId);
                    return user != null && (user.FirstName.Contains(searchText, StringComparison.OrdinalIgnoreCase) || user.LastName.Contains(searchText, StringComparison.OrdinalIgnoreCase));
                })
                .ToList();
        }

        private IUser? FindUserById(int id) => users.FirstOrDefault(user => user.Id == id);
        private Vacancy? FindVacancyById(int id, List<Vacancy> vacancyList) => vacancyList.FirstOrDefault(vacancy => vacancy.Id == id);
    }
}
