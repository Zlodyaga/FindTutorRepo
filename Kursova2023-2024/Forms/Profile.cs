using Kursova2023_2024.Classes.Data_classes;
using Kursova2023_2024.Classes.Tool_classes;

namespace Kursova2023_2024.Forms
{
    public partial class Profile : Form
    {
        IUser User { get; set; }
        PictureBox pictureBox = new PictureBox();
        public Profile(IUser user)
        {
            InitializeComponent();
            ControlPropertiesClass.SetToDefaultForm(this);
            ControlPropertiesClass.AddMenuStripToForm(this);
            User = user;
            DisplayUserProfile();
        }

        private void DisplayUserProfile()
        {
            // Создание PictureBox для отображения изображения пользователя (если оно есть)
            
            pictureBox.Size = new Size(400, 400);
            pictureBox.Location = new Point(50, 50);
            pictureBox.Image = User.FindImage(); 
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox.Click += picture_Click;

            // Создание и отображение меток для остальных данных профиля
            Label nameLabel = new Label();
            ControlPropertiesClass.SetToDefaultLabelFont(nameLabel, $"{User.FirstName} {User.LastName}");
            nameLabel.AutoSize = true;
            nameLabel.Location = new Point(500, 170);

            Label usernameLabel = new Label();
            ControlPropertiesClass.SetToDefaultLabelFont(usernameLabel, $"Username: {User.Username}");
            usernameLabel.AutoSize = true;
            usernameLabel.Location = new Point(500, 200);

            Label emailLabel = new Label();
            ControlPropertiesClass.SetToDefaultLabelFont(emailLabel, $"Email: {User.Email}");
            emailLabel.AutoSize = true;
            emailLabel.Location = new Point(500, 230);

            Label phoneLabel = new Label();
            ControlPropertiesClass.SetToDefaultLabelFont(phoneLabel, $"Phone: {User.Phone}");
            phoneLabel.AutoSize = true;
            phoneLabel.Location = new Point(500, 260);

            Label dateLabel = new Label();
            ControlPropertiesClass.SetToDefaultLabelFont(dateLabel, $"Date of birth: {User.DateOfBirth.ToShortDateString()}");
            dateLabel.AutoSize = true;
            dateLabel.Location = new Point(500, 290);

            Label sexLabel = new Label();
            ControlPropertiesClass.SetToDefaultLabelFont(sexLabel, $"Sex: {User.sex}");
            sexLabel.AutoSize = true;
            sexLabel.Location = new Point(500, 320);

            Label roleLabel = new Label();
            ControlPropertiesClass.SetToDefaultHeaderLabelFont(roleLabel, $"Role: {User.GetType().Name}");
            roleLabel.AutoSize = true;
            roleLabel.Location = new Point(500, 400);

            // Добавление элементов на форму
            Controls.Add(pictureBox);
            Controls.Add(nameLabel);
            Controls.Add(usernameLabel);
            Controls.Add(emailLabel);
            Controls.Add(phoneLabel);
            Controls.Add(dateLabel);
            Controls.Add(sexLabel);
            Controls.Add(roleLabel);
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Освобождаем ресурсы всех PictureBox при замене изображений
            pictureBox.Dispose();
            GC.Collect();
        }

        private void picture_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Устанавливаем фильтр для выбора только изображений .png, .jpg и .jpeg
            openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All Files (*.*)|*.*";
            openFileDialog.Title = "Choose image";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Получаем выбранный файл
                string selectedImagePath = openFileDialog.FileName;

                // Создаем изображение из выбранного файла
                Image selectedImage = Image.FromFile(selectedImagePath);
                pictureBox.Image.Dispose();
                pictureBox.Image = selectedImage;
                User.SetImage(selectedImage);
            }
        }
    }
}
