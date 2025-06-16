Create Database SIMS
go
use SIMS;


CREATE TABLE Roles(
	RoleID INT PRIMARY KEY IDENTITY,
	RoleName NVARCHAR(50),
);
CREATE TABLE Students (
    StudentID INT PRIMARY KEY IDENTITY,
	UserName NVARCHAR(100),
	Password NVARCHAR(100),
    FullName NVARCHAR(100),
    Email NVARCHAR(100),
    DateOfBirth DATE,
    Address NVARCHAR(255),
	RoleID INT FOREIGN KEY REFERENCES Roles(RoleID)
);
CREATE TABLE Courses (
    CourseID INT PRIMARY KEY IDENTITY,
    CourseName NVARCHAR(100),
    Description NVARCHAR(255),
);
CREATE TABLE Classes (
    ClassID INT PRIMARY KEY IDENTITY,
    ClassName NVARCHAR(100),
    CourseID INT FOREIGN KEY REFERENCES Courses(CourseID),
);
CREATE TABLE Enrollments (
    EnrollmentID INT PRIMARY KEY IDENTITY,
    StudentID INT FOREIGN KEY REFERENCES Students(StudentID),
    ClassID INT FOREIGN KEY REFERENCES Classes(ClassID),
);
CREATE TABLE Scores (
    ScoreID INT PRIMARY KEY IDENTITY,
    EnrollmentID INT FOREIGN KEY REFERENCES Enrollments(EnrollmentID),
    Score DECIMAL(5, 2),
);
CREATE TABLE Admins (
    AdminID INT PRIMARY KEY IDENTITY,
	UserName NVARCHAR(100),
	Password NVARCHAR(100),
	RoleID INT FOREIGN KEY REFERENCES Roles(RoleID)
);

INSERT INTO Roles(RoleName) VALUES
('Student'), ('Admin')

INSERT INTO Students (FullName, Email, DateOfBirth, Address,UserName,Password,RoleID) VALUES
('John Doe', 'john.doe@example.com', '2000-01-01', '123 Main St', 'student1','123',1),
('Jane Smith', 'jane.smith@example.com', '2001-02-02', '456 Elm St', 'student2','123',1),
('Robert Brown', 'robert.brown@example.com', '2002-03-03', '789 Oak St', 'student3','123',1),
('Alice Green', 'alice.green@example.com', '2000-04-04', '101 Maple St', 'student4','123',1),
('Charlie White', 'charlie.white@example.com', '2001-05-05', '202 Pine St', 'student5','123',1);
INSERT INTO Courses (CourseName, Description) VALUES
('Mathematics', 'Basic mathematics course'),
('Physics', 'Introduction to Physics'),
('Chemistry', 'Basic Chemistry principles'),
('Biology', 'Introduction to Biology'),
('Computer Science', 'Basic programming concepts');
INSERT INTO Classes (ClassName, CourseID) VALUES
('Math 101', 1),
('Physics 101', 2),
('Chemistry 101', 3),
('Biology 101', 4),
('CS 101', 5);
INSERT INTO Enrollments (StudentID, ClassID) VALUES
(1, 1),
(2, 2),
(3, 3),
(4, 4),
(5, 5);
INSERT INTO Scores (EnrollmentID, Score) VALUES
(1, 95.5),
(2, 88.0),
(3, 92.3),
(4, 85.7),
(5, 90.1);
INSERT INTO Admins (UserName,Password,RoleID) VALUES
('admin','123',2),
('admin1','123',2),
('admin2','123',2),
('admin3','123',2),
('admin4','123',2);




