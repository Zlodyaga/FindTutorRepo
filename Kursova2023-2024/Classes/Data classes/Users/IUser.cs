using Kursova2023_2024.Classes.Tool_classes;

namespace Kursova2023_2024.Classes.Data_classes
{
    public interface IUser
    {
        int Id { get; set; } // Ідентифікатор користувача
        string Username { get; set; } // Логін користувача
        string LastName { get; set; } // Прізвище користувача
        string FirstName { get; set; } // Ім'я користувача
        string Email { get; set; } // Електронна пошта користувача
        string Phone { get; set; } // Номер телефону користувача
        string sex { get; set; } // Стать користувача
        DateTime DateOfBirth { get; set; } // Дата народження користувача

        public Image FindImage()
        {
            // Формуємо шлях до зображення з урахуванням ідентифікатора
            string imagePath = ControlPropertiesClass.ImagePath + $"profile-image-id-{Id}.png";

            // Отримуємо абсолютний шлях до зображення
            string absoluteImagePath = Path.Combine(Application.StartupPath, imagePath);

            Image image;

            // Перевіряємо, чи існує файл за вказаним шляхом
            if (File.Exists(absoluteImagePath))
            {
                // Якщо існує, завантажуємо зображення
                image = Image.FromFile(absoluteImagePath);
                image.Tag = absoluteImagePath;
            }
            else
            {
                // Якщо зображення не знайдено, використовуємо резервний шлях
                string defaultImagePath = ControlPropertiesClass.ImagePath + "no-profile-image-icon.png";
                string absoluteDefaultImagePath = Path.Combine(Application.StartupPath, defaultImagePath);
                image = Image.FromFile(absoluteDefaultImagePath);
                image.Tag = absoluteDefaultImagePath;
            }

            return image;
        }

        public void SetImage(Image image)
        {
            try
            {
                // Створюємо копію зображення
                using (Image imageToSave = new Bitmap(image))
                {
                    // Формуємо шлях до зображення
                    string imagePath = ControlPropertiesClass.ImagePath + $"profile-image-id-{Id}.png";

                    // Отримуємо абсолютний шлях до зображення
                    string absoluteImagePath = Path.Combine(Application.StartupPath, imagePath);

                    // Зберігаємо зображення за вказаним шляхом
                    imageToSave.Save(absoluteImagePath);
                }
            }
            catch (Exception ex)
            {
                // Обробка винятків
                MessageBox.Show("An error occurred while saving the image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
