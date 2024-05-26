namespace Kursova2023_2024.Classes.Data_classes
{
    public class Vacancy
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public double HourlyRate { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }

        public Vacancy(int id, int userId, double hourlyRate, string subject, string status)
        {
            Id = id;
            UserId = userId;
            HourlyRate = hourlyRate;
            Subject = subject;
            Status = status;
        }
    }
}
