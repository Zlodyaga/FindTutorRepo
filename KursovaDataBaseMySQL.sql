DROP DATABASE IF EXISTS findTeachers;
CREATE DATABASE findTeachers CHARACTER SET utf8mb4;
USE findTeachers;

-- Створення таблиці забанених користувачів
CREATE TABLE banned (
    username VARCHAR(50) PRIMARY KEY
);

-- Створення таблиці входу
CREATE TABLE login (
    username VARCHAR(50) PRIMARY KEY,
    password VARCHAR(50)
);

-- Створення таблиці користувач
CREATE TABLE users (
    id_user INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) UNIQUE,
    first_name VARCHAR(50),
    last_name VARCHAR(50),
    sex ENUM('Чоловік', 'Жінка', 'Не визначено'),
    email VARCHAR(100),
    phone VARCHAR(15),
    date_of_birth DATE,
    role_in_system VARCHAR(20)
);

-- Створення таблиці вакансій
CREATE TABLE vacancies (
    id_vacancy INT AUTO_INCREMENT PRIMARY KEY,
    id_user INT,
    hourly_rate DECIMAL(10, 2),
    subject_of_studying VARCHAR(50),
    status_vacancy ENUM('Розміщено', 'Не розміщено'),
    FOREIGN KEY (id_user) REFERENCES users(id_user)
);

-- Створення таблиці відгуків на вакансії
CREATE TABLE response_to_vacancies (
    id_vacancy INT,
    id_user INT,
    status_response ENUM('Прийняте', 'На перегляді', 'Відхилено'),
    response_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (id_vacancy) REFERENCES vacancies(id_vacancy),
    FOREIGN KEY (id_user) REFERENCES users(id_user)
);

-- Заповнення таблиці login
INSERT INTO login (username, password) VALUES
('user1', 'password1'),
('user2', 'password2'),
('user3', 'password3'),
('user4', 'password4'),
('user5', 'password5'),
('user6', 'password6'),
('user7', 'password7'),
('user8', 'password8'),
('user9', 'password9'),
('user10', 'password10'),
('user11', 'password11'),
('user12', 'password12'),
('admin1', 'password1'),
('admin2', 'password2');

-- Заповнення таблиці users
INSERT INTO users (username, first_name, last_name, sex, email, phone, date_of_birth, role_in_system) VALUES
('user1', 'John', 'Doe', 'Чоловік', 'john@example.com', '+1234567890', '1990-01-01', 'teacher'),
('user2', 'Jane', 'Smith', 'Жінка', 'jane@example.com', '+0987654321', '2006-05-15', 'student'),
('user3', 'Alice', 'Johnson', 'Жінка', 'alice@example.com', '+1122334455', '1985-08-10', 'teacher'),
('user4', 'Bob', 'Smith', 'Чоловік', 'bob@example.com', '+9988776655', '2004-03-20', 'student'),
('user5', 'Emma', 'Wilson', 'Жінка', 'emma@example.com', '+1122336655', '1988-11-30', 'teacher'),
('user6', 'Michael', 'Brown', 'Чоловік', 'michael@example.com', '+1234509876', '1993-04-05', 'teacher'),
('user7', 'Emily', 'Taylor', 'Жінка', 'emily@example.com', '+1122337788', '1999-09-20', 'student'),
('user8', 'David', 'Martinez', 'Чоловік', 'david@example.com', '+1555666777', '1995-07-12', 'teacher'),
('user9', 'Sophia', 'Anderson', 'Жінка', 'sophia@example.com', '+1999888777', '2002-02-18', 'student'),
('user10', 'James', 'Garcia', 'Чоловік', 'james@example.com', '+1888777666', '1990-11-25', 'teacher'),
('user11', 'Olivia', 'Hernandez', 'Жінка', 'olivia@example.com', '+1444666555', '2001-08-30', 'student'),
('user12', 'William', 'Young', 'Чоловік', 'william@example.com', '+1666555444', '1987-12-15', 'teacher'),
('admin1', 'Admin', 'Adminov', 'Не визначено', 'admin@example.com', '+1111111111', '1980-12-25', 'admin'),
('admin2', 'Super', 'Admin', 'Не визначено', 'superadmin@example.com', '+9876543210', '1975-06-15', 'admin');

-- Заповнення таблиці vacancies
INSERT INTO vacancies (id_user, hourly_rate, subject_of_studying, status_vacancy) VALUES
(1, 325.00, 'Programming', 'Розміщено'),
(3, 220.00, 'Design', 'Розміщено'),
(5, 130.00, 'Marketing', 'Розміщено'),
(3, 200.00, 'English Literature', 'Не розміщено'),
(6, 250.00, 'Mathematics', 'Розміщено'),
(8, 180.00, 'Physics', 'Розміщено'),
(10, 300.00, 'Chemistry', 'Розміщено'),
(12, 220.00, 'Biology', 'Розміщено');

-- Заповнення таблиці response_to_vacancies
INSERT INTO response_to_vacancies (id_vacancy, id_user, status_response, response_date) VALUES
(1, 2, 'Відхилено', '2024-02-01 14:00:00'),
(1, 4, 'Відхилено', '2024-03-02 12:00:00'),
(2, 2, 'Прийняте', '2024-04-03 08:00:00'),
(3, 7, 'На перегляді', '2024-04-10 10:00:00'),
(4, 9, 'На перегляді', '2024-04-15 15:00:00'),
(5, 11, 'На перегляді', '2024-04-20 16:00:00');

DELIMITER //
CREATE TRIGGER update_vacancy_status_for_banned AFTER INSERT ON banned
FOR EACH ROW
BEGIN
    UPDATE vacancies
    SET status_vacancy = 'Не розміщено'
    WHERE id_user = (SELECT id_user FROM users WHERE username = NEW.username);
END;
//
DELIMITER ;