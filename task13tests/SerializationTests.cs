using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using task13;

namespace task13tests;

public class SerializationTests
{
    [Fact]
    public void Serialize_ShouldIgnoreNullGradesAndFormatDate()
    {
        var student = new Student
        {
            FirstName = "Иван",
            LastName = "Иванов",
            BirthDate = new DateTime(2005, 5, 12),
            Grades = null
        };

        string json = StudentSerializer.Serialize(student);

        Assert.Contains("\"BirthDate\": \"2005-05-12\"", json);
        Assert.DoesNotContain("\"Grades\"", json);
    }

    [Fact]
    public void Deserialize_WithInvalidData_ShouldThrowArgumentException()
    {
        string invalidJson = "{\"FirstName\":\"\",\"LastName\":\"Петров\",\"BirthDate\":\"2004-01-01\"}";
        Assert.Throws<ArgumentException>(() => StudentSerializer.DeserializeWithValidation(invalidJson));
    }

    [Fact]
    public void SaveAndLoadFromFile_ShouldPreserveData()
    {
        string tempFile = Path.GetTempFileName();
        var student = new Student
        {
            FirstName = "Ольга",
            LastName = "Плахина",
            BirthDate = new DateTime(2007, 01, 01),
            Grades = new List<Subject> { new Subject { Name = "ООП", Grade = 5 } }
        };

        try
        {
            StudentSerializer.SaveToFile(tempFile, student);
            var loadedStudent = StudentSerializer.LoadFromFile(tempFile);

            Assert.Equal(student.FirstName, loadedStudent.FirstName);
            Assert.Equal(student.LastName, loadedStudent.LastName);
            Assert.Equal(student.BirthDate, loadedStudent.BirthDate);
            Assert.Single(loadedStudent.Grades!);
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    [Fact]
    public void Console_Demonstration_Output()
    {
        var student = new Student
        {
            FirstName = "Иван",
            LastName = "Иванов",
            BirthDate = new DateTime(2005, 5, 12),
            Grades = new List<Subject> 
            { 
                new Subject { Name = "Математика", Grade = 5 },
                new Subject { Name = "Физика", Grade = 4 }
            }
        };
        string json = StudentSerializer.Serialize(student);
        
        Assert.NotNull(json);
        Assert.Contains("2005-05-12", json);
    }
}