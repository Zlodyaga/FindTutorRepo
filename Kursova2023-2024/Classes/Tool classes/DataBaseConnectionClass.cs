using Kursova2023_2024.Classes.Data_classes;
using MySql.Data.MySqlClient;

namespace Kursova2023_2024.Classes.Tool_classes
{
    public static class DataBaseConnectionClass
    {
        public static MySqlConnection connection = new MySqlConnection("server=localhost;port=3307;username=root;password=root;database=findteachers");

        public static void OpenConnection()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
        }

        public static void CloseConnection()
        {
            if (connection.State == System.Data.ConnectionState.Open)
                connection.Close();
        }

        public static MySqlConnection GetConnection()
        {
            return connection;
        }

        public static List<IUser> GetUsersNotBanned()
        {
            List<IUser> users = new List<IUser>();

            OpenConnection();

            string query = "SELECT u.* FROM users u LEFT JOIN banned b ON u.username = b.username WHERE b.username IS NULL;";
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                int id = reader.GetInt32("id_user");
                string username = reader.GetString("username");
                string lastName = reader.GetString("last_name");
                string firstName = reader.GetString("first_name");
                string email = reader.GetString("email");
                string phone = reader.GetString("phone");
                string sex = reader.GetString("sex");
                DateTime dateOfBirth = reader.GetDateTime("date_of_birth");

                switch (reader.GetString("role_in_system"))
                {
                    case "admin":
                        users.Add(new Admin(id, username, lastName, firstName, email, phone, dateOfBirth, sex));
                        break;
                    case "teacher":
                        users.Add(new Teacher(id, username, lastName, firstName, email, phone, dateOfBirth, sex));
                        break;
                    case "student":
                        users.Add(new Student(id, username, lastName, firstName, email, phone, dateOfBirth, sex));
                        break;
                    default:
                        break;
                }
            }

            reader.Close();
            CloseConnection();

            return users;
        }

        public static List<string> GetBannedLogins()
        {
            List<string> logins = new List<string>();

            OpenConnection();

            string query = "SELECT * FROM banned;";
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string username = reader.GetString("username");
                logins.Add(username);
            }

            reader.Close();
            CloseConnection();

            return logins;
        }

        public static List<IUser> GetUsersAll()
        {
            List<IUser> users = new List<IUser>();

            OpenConnection();

            string query = "SELECT * FROM users;";
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                int id = reader.GetInt32("id_user");
                string username = reader.GetString("username");
                string lastName = reader.GetString("last_name");
                string firstName = reader.GetString("first_name");
                string email = reader.GetString("email");
                string phone = reader.GetString("phone");
                string sex = reader.GetString("sex");
                DateTime dateOfBirth = reader.GetDateTime("date_of_birth");

                switch (reader.GetString("role_in_system"))
                {
                    case "admin":
                        users.Add(new Admin(id, username, lastName, firstName, email, phone, dateOfBirth, sex));
                        break;
                    case "teacher":
                        users.Add(new Teacher(id, username, lastName, firstName, email, phone, dateOfBirth, sex));
                        break;
                    case "student":
                        users.Add(new Student(id, username, lastName, firstName, email, phone, dateOfBirth, sex));
                        break;
                    default:
                        break;
                }
            }

            reader.Close();
            CloseConnection();

            return users;
        }

        public static List<Vacancy> GetVacancies()
        {
            List<Vacancy> vacancies = new List<Vacancy>();

            OpenConnection();

            string query = "SELECT * FROM vacancies;";
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                int id_vacancy = reader.GetInt32("id_vacancy");
                int id_user = reader.GetInt32("id_user");
                double hourly_rate = reader.GetDouble("hourly_rate");
                string subject = reader.GetString("subject_of_studying");
                string status = reader.GetString("status_vacancy");

                vacancies.Add(new Vacancy(id_vacancy, id_user, hourly_rate, subject, status));
            }

            reader.Close();
            CloseConnection();

            return vacancies;
        }

        public static List<Vacancy> GetActualVacancies() => GetVacancies().Where(v => v.Status == "Розміщено").ToList();

        public static List<int> GetRespondedVacanciesIdForStudent()
        {
            List<int> respondedVacancies = new List<int>();
            try
            {
                string query = "SELECT id_vacancy FROM response_to_vacancies " +
                               "WHERE id_user = @id_user AND status_response != \"Відхилено\";";

                OpenConnection();

                using (MySqlCommand command = new MySqlCommand(query, GetConnection()))
                {
                    command.Parameters.AddWithValue("@id_user", ControlPropertiesClass.CurrentUser.Id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            respondedVacancies.Add(reader.GetInt32("id_vacancy"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while taking FROM response_to_vacancies: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection();
            }

            return respondedVacancies;
        }

        public static List<int> GetRespondedVacanciesIdForStudentByTeacher()
        {
            List<int> respondedVacancies = new List<int>();
            try
            {
                string query = "SELECT id_vacancy FROM response_to_vacancies " +
                               "WHERE id_user = @id_user AND status_response = \"Прийняте\";";

                DataBaseConnectionClass.OpenConnection();

                using (MySqlCommand command = new MySqlCommand(query, DataBaseConnectionClass.GetConnection()))
                {
                    command.Parameters.AddWithValue("@id_user", ControlPropertiesClass.CurrentUser.Id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            respondedVacancies.Add(reader.GetInt32("id_vacancy"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while taking FROM response_to_vacancies: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                DataBaseConnectionClass.CloseConnection();
            }

            return respondedVacancies;
        }

        public static List<Pair<int, int>> GetAppliedAndRespondedVacanciesIdForTeacher()
        {
            List<Pair<int, int>> respondedVacancies = new List<Pair<int, int>>();
            try
            {
                string query = "SELECT response_to_vacancies.id_user, response_to_vacancies.id_vacancy, response_date FROM findteachers.response_to_vacancies " +
                    "JOIN vacancies USING(id_vacancy) " +
                    "WHERE vacancies.id_user = @id_teacher AND response_to_vacancies.status_response != \"Відхилено\";";

                OpenConnection();

                using (MySqlCommand command = new MySqlCommand(query, GetConnection()))
                {
                    command.Parameters.AddWithValue("@id_teacher", ControlPropertiesClass.CurrentUser.Id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int userId = reader.GetInt32("id_user");
                            int vacancyId = reader.GetInt32("id_vacancy");
                            DateTime time = reader.GetDateTime("response_date");
                            respondedVacancies.Add(new Pair<int, int>(userId, vacancyId, time));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while fetching responded vacancies for teacher: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection();
            }

            return respondedVacancies;
        }

        public static List<Pair<int, int>> GetAppliedVacanciesIdForTeacher()
        {
            List<Pair<int, int>> respondedVacancies = new List<Pair<int, int>>();
            try
            {
                string query = "SELECT response_to_vacancies.id_user, response_to_vacancies.id_vacancy, response_date FROM findteachers.response_to_vacancies " +
                    "JOIN vacancies USING(id_vacancy) " +
                    "WHERE vacancies.id_user = @id_teacher AND response_to_vacancies.status_response = \"Прийняте\";";

                OpenConnection();

                using (MySqlCommand command = new MySqlCommand(query, GetConnection()))
                {
                    command.Parameters.AddWithValue("@id_teacher", ControlPropertiesClass.CurrentUser.Id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int userId = reader.GetInt32("id_user");
                            int vacancyId = reader.GetInt32("id_vacancy");
                            DateTime time = reader.GetDateTime("response_date");
                            respondedVacancies.Add(new Pair<int, int>(userId, vacancyId, time));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while fetching responded vacancies for teacher: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection();
            }

            return respondedVacancies;
        }
    }
}
