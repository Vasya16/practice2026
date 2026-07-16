using System;
using System.Collections.Generic;
using Xunit;
using task10;

namespace task10tests;

public class PluginSystemTests
{
    [Fact]
    public void SortPluginsByDependencies_ShouldArrangeInCorrectOrder()
    {
        var mockPlugins = new List<PluginInfo>
        {
            new PluginInfo { Name = "PluginB", Dependencies = new[] { "PluginA" }, PluginType = typeof(object) },
            new PluginInfo { Name = "PluginA", Dependencies = Array.Empty<string>(), PluginType = typeof(object) }
        };

        var sorted = PluginManager.SortPluginsByDependencies(mockPlugins);

        Assert.Equal(2, sorted.Count);
        Assert.Equal("PluginA", sorted[0].Name);
        Assert.Equal("PluginB", sorted[1].Name);
    }

    [Fact]
    public void SortPluginsByDependencies_WithCyclicDependency_ShouldThrowException()
    {
        var cyclicPlugins = new List<PluginInfo>
        {
            new PluginInfo { Name = "PluginA", Dependencies = new[] { "PluginB" }, PluginType = typeof(object) },
            new PluginInfo { Name = "PluginB", Dependencies = new[] { "PluginA" }, PluginType = typeof(object) }
        };

        Assert.Throws<InvalidOperationException>(() => PluginManager.SortPluginsByDependencies(cyclicPlugins));
    }
}