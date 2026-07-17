using System;
using System.IO;
using System.Text.Json;

namespace task13;

public static class StudentSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    public static string Serialize(Student student)
    {
        return JsonSerializer.Serialize(student, Options);
    }

    public static Student DeserializeWithValidation(string json)
    {
        var student = JsonSerializer.Deserialize<Student>(json, Options);

        if (student == null)
            throw new ArgumentException("JSON-строка пуста.");
        if (string.IsNullOrWhiteSpace(student.FirstName) || string.IsNullOrWhiteSpace(student.LastName))
            throw new ArgumentException("Имя и фамилия студента не могут быть пустыми.");
        if (student.BirthDate > DateTime.Now || student.BirthDate < new DateTime(1900, 1, 1))
            throw new ArgumentException("Некорректная дата рождения.");

        return student;
    }

    public static void SaveToFile(string filePath, Student student)
    {
        string json = Serialize(student);
        File.WriteAllText(filePath, json);
    }

    public static Student LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Файл JSON не найден.");

        string json = File.ReadAllText(filePath);
        return DeserializeWithValidation(json);
    }
}
