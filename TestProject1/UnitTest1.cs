using Kursova2023_2024.Classes.Data_classes;
using System;
using System.Drawing;
using System.IO;

namespace TestProject1
{
    [TestClass]
    public class UserTests
    {
        private string ImagePath = "C:\\Users\\dimon\\source\\repos\\Kursova2023-2024\\TestProject1\\bin\\Debug\\net8.0-windows8.0\\Pictures\\";

        // ���������� ������ SetImage
        [TestMethod]
        public void SetImage_Saves_Image_Successfully()
        {
            // Arrange
            IUser user = CreateAdmin();
            Image imageToSave = new Bitmap(10, 10);

            // Act
            user.SetImage(imageToSave);

            // Assert
            // ����������, �� ���� ���������� ������
            Assert.IsTrue(File.Exists($"{ImagePath}profile-image-id-{user.Id}.png"));
        }

        // ���������� ������ FindImage
        [TestMethod]
        public void FindImage_Returns_Default_Image_When_File_Not_Found()
        {
            // Arrange
            IUser user = CreateAdminNoImage();

            // Act
            Image result = user.FindImage();

            // Assert
            Assert.IsNotNull(result);
            // ����������, �� ��������� �������� - �� �������� �� �������������
            Assert.AreEqual($"{ImagePath}no-profile-image-icon.png", result.Tag);
        }

        [TestMethod]
        public void FindImage_Returns_Existing_Image_When_File_Found()
        {
            // Arrange
            IUser user = CreateAdmin();
            // ��������� ���������� ���� ��� ����������
            using (var image = new Bitmap(10, 10))
            {
                image.Save($"{ImagePath}profile-image-id-{user.Id}.png");
            }

            // Act
            Image result = user.FindImage();

            // Assert
            Assert.IsNotNull(result);
            // ����������, �� ��������� �������� - �� ���������� �����������
            Assert.AreEqual($"{ImagePath}profile-image-id-{user.Id}.png", result.Tag);
        }

        // ��������� ����������� ��'���� IUser ��� ����������
        private IUser CreateAdmin()
        {
            return new Admin(999, "admin", "Admin", "Admin", "admin@example.com", "123456789", DateTime.Now, "male");
        }
        private IUser CreateAdminNoImage()
        {
            return new Admin(9999, "admin", "Admin", "Admin", "admin@example.com", "123456789", DateTime.Now, "male");
        }
    }
}
