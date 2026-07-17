using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13;

public class Subject
{
    public string Name { get; set; } = null!;
    public int Grade { get; set; }
}

public class Student
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    [JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime BirthDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Subject>? Grades { get; set; }
}

public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    private const string Format = "yyyy-MM-dd";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.ParseExact(reader.GetString()!, Format, null);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format));
    }
}
