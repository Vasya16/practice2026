using System;
using System.Collections.Generic;
using Xunit;
using task05;

namespace task05tests;

public class TestClass
{
    public int PublicField;
    private string _privateField = "";
    public int Property { get; set; }

    public void Method() { _ = _privateField; }
    public string ComplexMethod(int id, bool flag) => "test";
}

[Serializable]
public class AttributedClass { }

public class ClassAnalyzerTests
{
    [Fact]
    public void GetPublicMethods_ReturnsCorrectMethods()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var methods = analyzer.GetPublicMethods();

        Assert.Contains("Method", methods);
        Assert.Contains("ComplexMethod", methods);
    }

    [Fact]
    public void GetAllFields_IncludesPrivateFields()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var fields = analyzer.GetAllFields();

        Assert.Contains("_privateField", fields);
        Assert.Contains("PublicField", fields);
    }

    [Fact]
    public void GetProperties_ReturnsCorrectPropertyNames()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var properties = analyzer.GetProperties();

        Assert.Contains("Property", properties);
    }

    [Fact]
    public void HasAttribute_DetectsExistingAttribute()
    {
        var analyzerWithAttr = new ClassAnalyzer(typeof(AttributedClass));
        var analyzerWithoutAttr =  new ClassAnalyzer(typeof(TestClass));
        
        Assert.True(analyzerWithAttr.HasAttribute<SerializableAttribute>());
        Assert.False(analyzerWithoutAttr.HasAttribute<SerializableAttribute>());
    }

    [Fact]
    public void GetMethodParams_ReturnsFormattedParametersAndReturnType()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var info = analyzer.GetMethodParams("ComplexMethod");

        Assert.Contains("Int32 id", info);
        Assert.Contains("Boolean flag", info);
        Assert.Contains("Return: String", info);
    }
}