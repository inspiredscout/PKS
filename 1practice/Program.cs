using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

class Program
{
    static void Main()
    {
        Console.Write("Введите id студента для добавления оценки: ");
        string input1 = Console.ReadLine();
        int rStudentId = int.Parse(input1);
        Console.Write("Введите id предмета для добавления оценки( 0 если не будете добавлять): ");
        string input2 = Console.ReadLine();
        int subjectId = int.Parse(input2);
        Console.Write("Введите оценку (Отлично, Хорошо, Удовлетворительно): ");
        string mark = Console.ReadLine();
        Console.Write("Введите id студента: ");
        string input3 = Console.ReadLine();
        int studentId = int.Parse(input3);
        // Чтение данных из JSON-файла
        string json = File.ReadAllText("data.json");

        // Десериализация JSON-строки в объект
        var data = JsonSerializer.Deserialize<DataModel>(json);

        // Использование данных из объекта с использованием LINQ
        List<Student> students = data.Students;
        List<Subject> subjects = data.Subjects;
        List<EducationPlan> educationPlan = data.EducationPlan;

        // Добавление новой оценки для студента
        if (subjectId != 0)
        {
            AddNewGrade(educationPlan, rStudentId, subjectId, mark);
        }

        // Формирование списка дисциплин с оценками для необходимого студента с LINQ-запросом
        /*int studentId = 1;*/
        var studentSubjects = GetStudentSubjects(educationPlan, subjects, studentId);

        // Вывод результатов
        Console.WriteLine($"Дисциплины с оценками для студента {studentId}:");
        foreach (var subject in studentSubjects)
        {
            Console.WriteLine($"Предмет: {subject.Name}, Оценка: {subject.Grade}");
        }
    }

    // Добавление новой оценки для студента
    static void AddNewGrade(List<EducationPlan> educationPlan, int studentCode, int subjectCode, string grade)
    {
        educationPlan.Add(new EducationPlan { StudentCode = studentCode, SubjectCode = subjectCode, Grade = grade });
        SaveDataToJson("data.json", educationPlan);
    }

    // Сохранение данных в файл JSON без дублирования
    static void SaveDataToJson(string fileName, List<EducationPlan> newEducationPlan)
    {
        // Чтение существующих данных из файла
        string existingData = File.ReadAllText(fileName);

        // Десериализация JSON-строки в объект
        var existingDataObject = JsonSerializer.Deserialize<DataModel>(existingData);

        // Удаление существующих данных, соответствующих новым данным
        existingDataObject.EducationPlan.RemoveAll(ep => newEducationPlan.Any(newEp =>
            ep.StudentCode == newEp.StudentCode &&
            ep.SubjectCode == newEp.SubjectCode &&
            ep.Grade == newEp.Grade));

        // Объединение существующих данных и новых данных
        existingDataObject.EducationPlan.AddRange(newEducationPlan);

        // Сериализация объединенных данных
        string jsonData = JsonSerializer.Serialize(existingDataObject, new JsonSerializerOptions { WriteIndented = true });

        // Запись данных обратно в файл
        File.WriteAllText(fileName, jsonData);
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

    // Классы для модели данных
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

    class DataModel
    {
        public List<Student> Students { get; set; }
        public List<Subject> Subjects { get; set; }
        public List<EducationPlan> EducationPlan { get; set; }
    }
}
