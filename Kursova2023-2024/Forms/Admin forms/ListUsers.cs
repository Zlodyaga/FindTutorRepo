using Kursova2023_2024.Classes.Data_classes;
using Kursova2023_2024.Classes.Tool_classes;
using Kursova2023_2024.Forms.Admin_forms;
using Microsoft.VisualBasic.ApplicationServices;
using MySql.Data.MySqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Kursova2023_2024.Forms.AdminForms
{
    public partial class ListUsers : Form
    {
        private List<IUser> users = DataBaseConnectionClass.GetUsersAll();
        private FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
        private CheckedListBox rolesCheckedListBox = new CheckedListBox();
        private CheckedListBox BansCheckedListBox = new CheckedListBox();
        private TextBox textBox = new TextBox();
        private List<PictureBox> pictureBoxList = new List<PictureBox>();

        public ListUsers()
        {
            InitializeComponent();
            ControlPropertiesClass.SetToDefaultForm(this);
            ControlPropertiesClass.AddMenuStripToForm(this);

            // Создаем FlowLayoutPanel
            flowLayoutPanel.Size = new Size(735, 510);
            flowLayoutPanel.Top = 25;
            flowLayoutPanel.Left = 220;
            flowLayoutPanel.AutoScroll = true; // Включаем автоматическую прокрутку

            // Создаем объекты для добавления в FlowLayoutPanel
            UpdateUsersInFlowLayoutPanel(users);

            // Добавляем FlowLayoutPanel на форму
            this.Controls.Add(flowLayoutPanel);

            // Установка свойств TextBox
            textBox.Location = new System.Drawing.Point(10, 260);
            textBox.Size = new System.Drawing.Size(175, 20);
            ControlPropertiesClass.SetToDefaultTextBoxNames(textBox, "Enter name/surname");
            textBox.KeyDown += TextBoxSearch_KeyDown;

            // Добавление TextBox на форму
            this.Controls.Add(textBox);

            // Создание Label для отображения значения "Цена за занятия"
            Label searchLabel = new Label();
            searchLabel.Location = new System.Drawing.Point(10, 220);
            searchLabel.AutoSize = true;
            ControlPropertiesClass.SetToDefaultHeaderLabelFont(searchLabel, "Search");
            this.Controls.Add(searchLabel);


            // Настройка нового экземпляра CheckedListBox
            ControlPropertiesClass.SetToDefaultCheckedListBox(rolesCheckedListBox);
            ControlPropertiesClass.SetToDefaultCheckedListBox(BansCheckedListBox);

            // Установка свойств CheckedListBox
            rolesCheckedListBox.Location = new System.Drawing.Point(10, 320); // Установка положения по X и Y
            rolesCheckedListBox.Size = new System.Drawing.Size(175, 90); // Установка ширины и высоты

            // Добавление элементов в CheckedListBox
            foreach (var roles in GetRolesForCheckedListBox())
            {
                rolesCheckedListBox.Items.Add($"{char.ToUpper(roles[0])}{roles.Substring(1).ToLower()}");
            }

            // Добавление CheckedListBox на форму
            this.Controls.Add(rolesCheckedListBox);
            
            BansCheckedListBox.Location = new System.Drawing.Point(10, 110); // Установка положения по X и Y
            BansCheckedListBox.Size = new System.Drawing.Size(175, 90); // Установка ширины и высоты

            BansCheckedListBox.Items.Add("Banned");
            BansCheckedListBox.Items.Add("Not banned");

            this.Controls.Add(BansCheckedListBox);

            Button buttonFilter = new Button();
            ControlPropertiesClass.SetToDefaultButton(buttonFilter, "Apply filter");
            buttonFilter.Size = new Size(175, 40);
            buttonFilter.Location = new Point(10, 430);
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

        private void ChangePictureOnPanel_Click(object sender, EventArgs e)
        {
            PictureBox clickedPicture = (PictureBox)sender;
            Panel parentPanel = (clickedPicture.Parent != null) ? (Panel)clickedPicture.Parent : throw new Exception("Parent panel is null");

            int id = (parentPanel.Tag != null) ? (int)parentPanel.Tag : -1;

            // Теперь у вас есть индекс объекта, на который был совершен клик
            ChangeUserForm changeUserForm = new ChangeUserForm(FindUserById(id));
            changeUserForm.Show();
            this.Close();
        }

        private void ButtonFilter_Click(object sender, EventArgs e)
        {
            // Получаем выбранные предметы из CheckedListBox
            List<string> selectedRoles = rolesCheckedListBox.CheckedItems.Cast<string>().ToList();
            List<string> selectedBans = BansCheckedListBox.CheckedItems.Cast<string>().ToList();

            // Фильтруем вакансии по условиям
            List<IUser> filteredUsers = users;
            if (selectedRoles.Any())
            {
                filteredUsers = users
                    .Where(user => selectedRoles.Contains(user.GetType().Name))
                    .ToList();
            }

            if (selectedBans.Count == 1)
            {
                switch (selectedBans[0])
                {
                    case "Banned":
                        // Отфильтровать пользователей, у которых логин находится в списке забаненных
                        filteredUsers = filteredUsers.Where(user => checkForBan(user.Username) != null).ToList();
                        break;
                    case "Not banned":
                        // Отфильтровать пользователей, у которых логин не находится в списке забаненных
                        filteredUsers = filteredUsers.Where(user => checkForBan(user.Username) == null).ToList();
                        break;
                }
            }
            if (textBox.Text != "Enter name/surname")
                filteredUsers = FindSearchInTextBox(filteredUsers, (textBox.Text));

            
            UpdateUsersInFlowLayoutPanel(filteredUsers);
        }

        private void TextBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            // Проверяем, была ли нажата клавиша Enter
            if (e.KeyCode == Keys.Enter)
            {
                UpdateUsersInFlowLayoutPanel(FindSearchInTextBox(users, textBox.Text));
            }
        }

        private List<IUser> FindSearchInTextBox(List<IUser> searchedUsers, string searchText)
        {
            // Фильтруем пользователей по имени или фамилии
            return searchedUsers
                .Where(user => user.FirstName.Contains(searchText, StringComparison.OrdinalIgnoreCase) || user.LastName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private void UpdateUsersInFlowLayoutPanel(List<IUser> userList) {
            // Очищаем существующие элементы в FlowLayoutPanel
            flowLayoutPanel.Controls.Clear();

            foreach (IUser user in userList)
            {
                // Создаем контейнер Panel для каждого объекта
                Panel itemPanel = new Panel();
                itemPanel.Size = new Size(680, 100);
                itemPanel.BorderStyle = BorderStyle.FixedSingle;
                itemPanel.Tag = user.Id; // Сохраняем индекс в свойстве Tag

                // Добавляем изображение
                PictureBox pictureBox = new PictureBox();
                pictureBox.Image = user.FindImage();
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

                Label usernameLabel = new Label();
                ControlPropertiesClass.SetToDefaultLabelFont(usernameLabel, $"Id - {user.Id} {checkForBan(user.Username)}");
                usernameLabel.AutoSize = true;
                usernameLabel.Location = new Point(110, 40);
                itemPanel.Controls.Add(usernameLabel);

                Label roleLabel = new Label();
                ControlPropertiesClass.SetToDefaultHeaderLabelFont(roleLabel, $"{user.GetType().Name}");
                roleLabel.AutoSize = true;
                roleLabel.Location = new Point(305, 35);
                itemPanel.Controls.Add(roleLabel);

                // Добавляем изображение
                PictureBox profilePictureBox = new PictureBox();
                profilePictureBox.Image = FindImageByName("Profile"); // Замените yourImage на ваше изображение
                profilePictureBox.Size = new Size(25, 25);
                profilePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                profilePictureBox.Location = new Point(540, 35);
                ControlPropertiesClass.SetToDefaultClickablePicture(profilePictureBox);
                profilePictureBox.Click += ProfilePictureOnPanel_Click;
                itemPanel.Controls.Add(profilePictureBox);

                // Добавляем изображение
                PictureBox changePictureBox = new PictureBox();
                changePictureBox.Image = FindImageByName("Change"); // Замените yourImage на ваше изображение
                changePictureBox.Size = new Size(25, 25);
                changePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                changePictureBox.Location = new Point(580, 35);
                ControlPropertiesClass.SetToDefaultClickablePicture(changePictureBox);
                changePictureBox.Click += ChangePictureOnPanel_Click;
                itemPanel.Controls.Add(changePictureBox);

                // Добавляем изображение
                PictureBox deletePictureBox = new PictureBox();
                deletePictureBox.Image = FindImageByName("Delete"); // Замените yourImage на ваше изображение
                deletePictureBox.Size = new Size(25, 25);
                deletePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                deletePictureBox.Location = new Point(620, 35);
                ControlPropertiesClass.SetToDefaultClickablePicture(deletePictureBox);
                deletePictureBox.Click += (sender, args) =>
                {
                    BanOrUnbanUser(user);
                    UpdateUsersInFlowLayoutPanel(users);
                };

                itemPanel.Controls.Add(deletePictureBox);

                // Добавляем созданный контейнер в FlowLayoutPanel
                flowLayoutPanel.Controls.Add(itemPanel);
            }
        }

        private string? checkForBan(string login)
        {
            switch(DataBaseConnectionClass.GetBannedLogins().Contains(login))
            {
                case true:
                    return "banned";
                default: return null;
            }
        }

        private void BanOrUnbanUser(IUser user)
        {
            // Проверяем, забанен ли пользователь
            string banStatus = checkForBan(user.Username);

            // Если пользователь забанен, удаляем его из таблицы banned
            if (banStatus != null && banStatus == "banned")
            {
                // Формирование текста сообщения
                string message = $"Ви впевненні, що хочете розбанити цього користувача? (Username - {user.Username}, Name - {user.FirstName} {user.LastName})";

                // Показ диалогового окна с подтверждением
                DialogResult result = MessageBox.Show(message, "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // Проверка результата диалога
                if (result == DialogResult.Yes)
                {
                    string query = "DELETE FROM banned WHERE username = @username";
                    using (MySqlCommand command = new MySqlCommand(query, DataBaseConnectionClass.GetConnection()))
                    {
                        DataBaseConnectionClass.OpenConnection();
                        command.Parameters.AddWithValue("@username", user.Username);
                        command.ExecuteNonQuery();
                        DataBaseConnectionClass.CloseConnection();
                    }
                }
            }
            // Если пользователь не забанен, баним его
            else
            {
                // Формирование текста сообщения
                string message = $"Ви впевненні, що хочете забанити цього користувача? (Username - {user.Username}, Name - {user.FirstName} {user.LastName})";

                // Показ диалогового окна с подтверждением
                DialogResult result = MessageBox.Show(message, "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // Проверка результата диалога
                if (result == DialogResult.Yes)
                {
                    string query = "INSERT INTO banned (username) VALUES (@username)";
                    using (MySqlCommand command = new MySqlCommand(query, DataBaseConnectionClass.GetConnection()))
                    {
                        DataBaseConnectionClass.OpenConnection();
                        command.Parameters.AddWithValue("@username", user.Username);
                        command.ExecuteNonQuery();
                        DataBaseConnectionClass.CloseConnection();
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

        private List<string> GetRolesForCheckedListBox()
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

        private IUser FindUserById(int id) => users.FirstOrDefault(user => user.Id == id);
    }
}
