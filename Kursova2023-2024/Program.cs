using Kursova2023_2024.Classes.Tool_classes;
using Kursova2023_2024.Classes;

namespace Kursova2023_2024
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Десериализация
            string settingsFilePath = "settings.json";
            if (File.Exists(settingsFilePath))
            {
                AppSettings loadedSettings = null;
                SerializationClass.DeserializationFromJson(ref loadedSettings, settingsFilePath);
                ControlPropertiesClass.themeColor = loadedSettings.themeColor;
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new StartForm());

            // Сериализация
            AppSettings settings = new AppSettings { themeColor = ControlPropertiesClass.themeColor };
            SerializationClass.SerialiazeToJson(ref settings, "settings.json");
        }
    }
}