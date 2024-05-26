using Kursova2023_2024.Classes.Data_classes;
using Kursova2023_2024.Forms;
using Kursova2023_2024.Forms.Admin_forms;
using Kursova2023_2024.Forms.AdminForms;
using Kursova2023_2024.Forms.Teacher_forms;

namespace Kursova2023_2024.Classes.Tool_classes
{
    public static class ControlPropertiesClass
    {
        public static IUser? CurrentUser { get; set; }
        public static string ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pictures\\");

        private static Color colorFormWhite = Color.White;
        private static Color colorFormDark = Color.LightGray;
        private static Color textBoxFontColor = Color.Black;
        private static Color originalColor;
        private static Color mouseEnterColor = Color.Silver;
        private static Color mouseDownColor = Color.FromArgb(224, 224, 224);
        private static Font fontForHeaderLabels = new Font("Franklin Gothic Medium", 18f);
        private static Font fontForLabels = new Font("Segoe UI", 12f);
        private static Font fontForButtons = new Font("Segoe UI", 14f);
        public static string themeColor = "Dark";

        public static void ApplyMouseProperties(Control control)
        {
            originalColor = control.BackColor;

            control.MouseEnter += (sender, e) =>
            {
                control.BackColor = mouseEnterColor;
            };

            control.MouseDown += (sender, e) =>
            {
                control.BackColor = mouseDownColor;
            };

            control.MouseUp += (sender, e) =>
            {
                control.BackColor = mouseEnterColor;
            };

            control.MouseLeave += (sender, e) =>
            {
                control.BackColor = originalColor;
            };
        }

        public static void AddMenuStripToForm(Form form)
        {
            MenuStrip menuStrip = InitializeMenuStrip(CurrentUser, form);
            form.Controls.Add(menuStrip);
            menuStrip.Dock = DockStyle.Top;
        }

        public static void AddBackIcon(Form formNow, Form formGoBack)
        {
            string absoluteImagePath = Path.Combine(Application.StartupPath, (ImagePath + "Back_Icon.jpg"));
            Image image = Image.FromFile(absoluteImagePath);

            // Добавляем изображение
            PictureBox pictureBox = new PictureBox();
            pictureBox.Image = image;
            pictureBox.Size = new Size(50, 50);
            pictureBox.Cursor = Cursors.Hand;
            pictureBox.Top = 5;
            pictureBox.Left = 5;
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;

            pictureBox.Click += (sender, args) =>
            {
                formGoBack.Show();
                formNow.Close();
            };
            formNow.Controls.Add(pictureBox);
        }

        public static void SetToDefaultForm(Form form)
        {
            if(themeColor == "Dark") {
                form.BackColor = colorFormDark;
            } else { 
                form.BackColor = colorFormWhite; 
            }
            
            form.StartPosition = FormStartPosition.CenterScreen;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Size = new Size(960, 540);
        }

        public static void SetToDefaultCheckBox(CheckBox checkBox, string text)
        {
            checkBox.ForeColor = textBoxFontColor;
            checkBox.Font = fontForLabels;
            checkBox.Text = text;
            checkBox.ForeColor = Color.Black;
        }

        public static void SetToDefaultDateTimePicker(DateTimePicker datePicker)
        {
            datePicker.Format = DateTimePickerFormat.Custom;
            datePicker.CustomFormat = "yyyy-MM-dd";
            datePicker.Value = DateTime.Today;
            datePicker.Font = fontForLabels;
        }

        public static void SetToDefaultHeaderLabelFont(Label label, string text)
        {
            label.Font = fontForHeaderLabels;
            label.Text = text;
        }

        public static void SetToDefaultLabelFont(Label label, string text)
        {
            label.Font = fontForLabels;
            label.Text = text;
        }

        private static void SetToDefaultTextBox(TextBox textBox, string text)
        {
            textBox.ForeColor = textBoxFontColor;
            textBox.Font = fontForLabels;
            textBox.Text = text;
            textBox.ForeColor = Color.Gray;

            textBox.Enter += (sender, args) => {
                if (textBox.Text == text)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                }
            };

            textBox.Leave += (sender, args) => {
                if (textBox.Text == "")
                {
                    textBox.Text = text;
                    textBox.ForeColor = Color.Gray;
                }
            };
        }

        public static void SetToDefaultTextBoxClassic(TextBox textBox, string text)
        {
            SetToDefaultTextBox(textBox, text);

            textBox.KeyPress += (sender, args) =>
            {
                if (!char.IsLetterOrDigit(args.KeyChar) && args.KeyChar != (char)Keys.Back)
                {
                    // Отменяем ввод символа
                    args.Handled = true;
                }
            };
        }

        public static void SetToDefaultTextBoxEmail(TextBox textBox, string text)
        {
            SetToDefaultTextBox(textBox, text);

            textBox.KeyPress += (sender, e) =>
            {
                if (!char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != '@' && e.KeyChar != '.' && e.KeyChar != (char)Keys.Back)
                {
                    // Отменяем ввод символа
                    e.Handled = true;
                }
            };
        }

        public static void SetToDefaultTextBoxDigits(TextBox textBox, string text)
        {
            SetToDefaultTextBox(textBox, text);

            textBox.KeyPress += (sender, e) =>
            {
                if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                {
                    // Отменяем ввод символа
                    e.Handled = true;
                }
            };
        }

        public static void SetToDefaultTextBoxNames(TextBox textBox, string text)
        {
            SetToDefaultTextBox(textBox, text);

            textBox.KeyPress += (sender, args) =>
            {
                if (!char.IsLetter(args.KeyChar) && args.KeyChar != (char)Keys.Back)
                {
                    // Отменяем ввод символа
                    args.Handled = true;
                }
            };
        }

        public static void SetToDefaultTextBoxPassword(TextBox textBox, string text) 
        {
            textBox.ForeColor = textBoxFontColor;
            textBox.Font = fontForLabels;
            textBox.Text = text;
            textBox.ForeColor = Color.Gray;

            textBox.Enter += (sender, args) => {
                if (textBox.Text == text)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                    textBox.PasswordChar = '*';
                }
            };

            textBox.Leave += (sender, args) => {
                if (textBox.Text == "")
                {
                    textBox.Text = text;
                    textBox.ForeColor = Color.Gray;
                    textBox.PasswordChar = '\0';
                }
            };

            textBox.KeyPress += (sender, args) =>
            {
                if (!char.IsLetterOrDigit(args.KeyChar) && args.KeyChar != (char)Keys.Back)
                {
                    // Отменяем ввод символа
                    args.Handled = true;
                }
            };
        }

        public static void SetToDefaultComboBox(ComboBox comboBox) 
        {
            comboBox.Size = new System.Drawing.Size(175, 35);
            comboBox.Font = fontForLabels;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public static void SetToDefaultButton(Button button, string text)
        {
            button.BackColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(224, 224, 224);
            button.FlatAppearance.MouseOverBackColor = Color.Silver;
            button.Font = fontForButtons;
            button.Text = text;
            button.Cursor = Cursors.Hand;
        }

        public static void SetToDefaultCheckedListBox(CheckedListBox checkedListBox) 
        {
            checkedListBox.Font = fontForLabels;
            checkedListBox.CheckOnClick = true;
        }

        public static void SetToDefaultClickablePicture(PictureBox pictureBox)
        {
            pictureBox.Cursor = Cursors.Hand;
        }

        // Статический метод для инициализации MenuStrip
        private static MenuStrip InitializeMenuStrip(IUser user, Form form)
        {
            MenuStrip menuStrip = new MenuStrip();
            menuStrip.BackColor = Color.Gainsboro;
            ToolStripMenuItem homeMenuItem = new ToolStripMenuItem("Homepage");
            homeMenuItem.Click += (sender, args) => {
                MainMenu menu;
                if (CurrentUser is Student)
                {
                    menu = new MainMenu(DataBaseConnectionClass.GetActualVacancies().Where(v => !DataBaseConnectionClass.GetRespondedVacanciesIdForStudent().Any(id => id == v.Id)).ToList());
                }
                else { menu = new MainMenu(DataBaseConnectionClass.GetActualVacancies()); }
                menu.Show();
                form.Close();
            };

            ToolStripMenuItem profileMenuItem = new ToolStripMenuItem("Profile");
            profileMenuItem.Click += (sender, args) => {
                Profile profile = new Profile(CurrentUser);
                profile.Show();
                form.Close();
            };
            menuStrip.Items.Add(homeMenuItem);
            menuStrip.Items.Add(profileMenuItem);

            switch (user)
            {
                case Admin:
                    ToolStripMenuItem adminMenuItem = new ToolStripMenuItem("Admin menu");
                    ToolStripMenuItem listOfUsersMenuItem = new ToolStripMenuItem("List Of users");
                    ToolStripMenuItem vacanciesOfTeachersMenuItem = new ToolStripMenuItem("Vacancies of teachers");

                    listOfUsersMenuItem.Click += (sender, args) => {
                        ListUsers listUsers = new ListUsers();
                        listUsers.Show();
                        form.Close();
                    };

                    vacanciesOfTeachersMenuItem.Click += (sender, args) => {
                        NewVacanciesForm newVacanciesForm = new NewVacanciesForm();
                        newVacanciesForm.Show();
                        form.Close();
                    };

                    adminMenuItem.DropDownItems.Add(listOfUsersMenuItem);
                    adminMenuItem.DropDownItems.Add(vacanciesOfTeachersMenuItem);


                    menuStrip.Items.Add(adminMenuItem);
                    break;
                case Teacher:
                    ToolStripMenuItem TeacherMenuItem = new ToolStripMenuItem("Teacher menu");
                    ToolStripMenuItem myVacanciesTeacherMenuItem = new ToolStripMenuItem("My vacancies");
                    ToolStripMenuItem addVacancyTeacherMenuItem = new ToolStripMenuItem("Add vacancy");
                    ToolStripMenuItem responsesFromStudentsItem = new ToolStripMenuItem("Responses to vacancy");

                    myVacanciesTeacherMenuItem.Click += (sender, args) => { 
                        MainMenu menu = new MainMenu(DataBaseConnectionClass.GetVacancies().Where(v => v.UserId == CurrentUser.Id).ToList());
                        menu.Show();
                        form.Close();
                    };

                    addVacancyTeacherMenuItem.Click += (sender, args) => {
                        AddNewVacancyForm addNewVacancyForm = new AddNewVacancyForm();
                        addNewVacancyForm.Show();
                        form.Close();
                    };

                    responsesFromStudentsItem.Click += (sender, args) => {
                        ResponsesFromStudentsForm responsesFromStudentsForm = new ResponsesFromStudentsForm();
                        responsesFromStudentsForm.Show();
                        form.Close();
                    };

                    TeacherMenuItem.DropDownItems.Add(myVacanciesTeacherMenuItem);
                    TeacherMenuItem.DropDownItems.Add(addVacancyTeacherMenuItem);
                    TeacherMenuItem.DropDownItems.Add(responsesFromStudentsItem);
                    menuStrip.Items.Add(TeacherMenuItem);
                    break;
                case Student:
                    ToolStripMenuItem myRequestsMenuItem = new ToolStripMenuItem("Requests on contact");

                    myRequestsMenuItem.Click += (sender, args) => { 
                        List<Vacancy> vacanciesRespondedByStudent = DataBaseConnectionClass.GetVacancies().Where(v => DataBaseConnectionClass.GetRespondedVacanciesIdForStudent().Any(id => id == v.Id)).ToList();
                        MainMenu menu = new MainMenu(vacanciesRespondedByStudent.Where(v => v.Status == "Розміщено").ToList());
                        menu.Show();
                        form.Close();
                    };

                    menuStrip.Items.Add(myRequestsMenuItem);
                    break;
            }

            // Создаем элемент меню для размещения справа
            ToolStripMenuItem exitMenuItem = new ToolStripMenuItem("Exit App");
            ToolStripMenuItem logOutMenuItem = new ToolStripMenuItem("Log out");
            string text = (themeColor == "Dark") ? "White" : "Dark";
            ToolStripMenuItem SettingsMenuItem = new ToolStripMenuItem($"Change to {text}");
            SettingsMenuItem.Click += (sender, args) => { 
                SettingsMenuItem.Text = $"Change to {changeTheme(form)}";
            };
            exitMenuItem.Click += (sender, args) => Application.Exit();
            logOutMenuItem.Click += (sender, args) => {
                CurrentUser = null;
                StartForm.Instance.Show();
                form.Close();
            };

            // Рассчитываем ширину MenuStrip
            int menuStripWidth = 0;
            foreach (ToolStripItem item in menuStrip.Items)
            {
                menuStripWidth += item.Width;
            }

            // Рассчитываем отступ для элемента справа
            int rightItemsLeftMargin = (CurrentUser is Student) ? 770 - menuStripWidth * 2 : 810 - menuStripWidth * 2;

            // Устанавливаем отступ для элемента справа
            exitMenuItem.Margin = new Padding(rightItemsLeftMargin, 0, 0, 0);

            menuStrip.Items.Add(exitMenuItem);
            menuStrip.Items.Add(logOutMenuItem);
            menuStrip.Items.Add(SettingsMenuItem);

            return menuStrip;
        }

        private static string changeTheme(Form form)
        {
            switch(themeColor)
            {
                case "Dark":
                    themeColor = "White";
                    form.BackColor = colorFormWhite;
                    return "Dark";
                case "White":
                    themeColor = "Dark";
                    form.BackColor = colorFormDark;
                    return "White";
                default: 
                    return themeColor;
            }
        }
    }
}
