using System;

namespace task07;

[AttributeUsage(AttributeTargets.Class)]
public class VersionAttribute : Attribute
{
    public int Major { get; }
    public int Minor { get; }

    public VersionAttribute(int major, int minor)
    {
        if (major < 0 || minor < 0)
        {
            throw new ArgumentOutOfRangeException("Компоненты версии не могут быть отрицательными.");
        }
        Major = major;
        Minor = minor;
    }
}