using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

class Program
{
    static void Main()
    {
        Console.Write("Введите id студента: ");
        string input = Console.ReadLine();
        int studentId = int.Parse(input);
        // Исходные данные
        List<Student> students = new List<Student>
        {
            new Student { StudentCode = 1, LastName = "Пупкин", FirstName = "Василий", MiddleName = "Васильевич" },
            new Student { StudentCode = 2, LastName = "Иванов", FirstName = "Иван", MiddleName = "Иванович" },
            new Student { StudentCode = 3, LastName = "Путин", FirstName = "Владимир", MiddleName = "Владимирович" },
            // Другие студенты
        };

        List<Subject> subjects = new List<Subject>
        {
            new Subject { SubjectCode = 101, Name = "Математика", LectureHours = 30, PracticeHours = 20 },
            new Subject { SubjectCode = 102, Name = "ПКС", LectureHours = 30, PracticeHours = 30 },
            new Subject { SubjectCode = 103, Name = "БигДата", LectureHours = 30, PracticeHours = 30 },
            // Другие предметы
        };

        List<EducationPlan> educationPlan = new List<EducationPlan>
        {
            new EducationPlan { StudentCode = 1, SubjectCode = 101, Grade = "Отлично" },
            new EducationPlan { StudentCode = 1, SubjectCode = 102, Grade = "Хорошо" },
            new EducationPlan { StudentCode = 1, SubjectCode = 103, Grade = "Удовлетворительно" },
            new EducationPlan { StudentCode = 2, SubjectCode = 101, Grade = "Удовлетворительно" },
            new EducationPlan { StudentCode = 2, SubjectCode = 102, Grade = "Удовлетворительно" },
            new EducationPlan { StudentCode = 2, SubjectCode = 103, Grade = "Удовлетворительно" },
            new EducationPlan { StudentCode = 3, SubjectCode = 101, Grade = "Отлично" },
            new EducationPlan { StudentCode = 3, SubjectCode = 102, Grade = "Отлично" },
            new EducationPlan { StudentCode = 3, SubjectCode = 103, Grade = "Отлично" },
            // Другие записи в учебном плане
        };

        // Сохранение данных в JSON-файл
        SaveDataToJson("data.json", students, subjects, educationPlan);

        // Добавление новой оценки для студента
        AddNewGrade(educationPlan, 1, 404, "Хорошо");

        // Формирование списка дисциплин с оценками для необходимого студента с LINQ-запросом
        /*int studentId = 2;*/
        var studentSubjects = GetStudentSubjects(educationPlan, subjects, studentId);

        // Вывод результатов
        foreach (var subject in studentSubjects)
        {
            Console.WriteLine($"Предмет: {subject.Name}, Оценка: {subject.Grade}");
        }
    }

    static void SaveDataToJson(string fileName, List<Student> students, List<Subject> subjects, List<EducationPlan> educationPlan)
    {
        var data = new { Students = students, Subjects = subjects, EducationPlan = educationPlan };
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(fileName, json);
    }

    static void AddNewGrade(List<EducationPlan> educationPlan, int studentCode, int subjectCode, string grade)
    {
        educationPlan.Add(new EducationPlan { StudentCode = studentCode, SubjectCode = subjectCode, Grade = grade });
        SaveDataToJson("data.json", new List<Student>(), new List<Subject>(), educationPlan);
    }

    static List<StudentSubject> GetStudentSubjects(List<EducationPlan> educationPlan, List<Subject> subjects, int studentCode)
    {
        var studentSubjects = from plan in educationPlan
                              join subject in subjects on plan.SubjectCode equals subject.SubjectCode
                              where plan.StudentCode == studentCode
                              select new StudentSubject
                              {
                                  Name = subject.Name,
                                  Grade = plan.Grade
                              };

        // Вывод процента оценок
        var totalCount = studentSubjects.Count();
        var excellentCount = studentSubjects.Count(s => s.Grade == "Отлично");
        var goodCount = studentSubjects.Count(s => s.Grade == "Хорошо");
        var satisfactoryCount = studentSubjects.Count(s => s.Grade == "Удовлетворительно");

        Console.WriteLine($"Процент отличных оценок: {(double)excellentCount / totalCount * 100}%");
        Console.WriteLine($"Процент хороших оценок: {(double)goodCount / totalCount * 100}%");
        Console.WriteLine($"Процент удовлетворительных оценок: {(double)satisfactoryCount / totalCount * 100}%");

        return studentSubjects.ToList();
    }
}

class Student
{
    public int StudentCode { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
}

class Subject
{
    public int SubjectCode { get; set; }
    public string Name { get; set; }
    public int LectureHours { get; set; }
    public int PracticeHours { get; set; }
}

class EducationPlan
{
    public int StudentCode { get; set; }
    public int SubjectCode { get; set; }
    public string Grade { get; set; }
}

class StudentSubject
{
    public string Name { get; set; }
    public string Grade { get; set; }
}
