namespace SmartMigrations.Test;

using SmartMigrations;
using Xunit;

public class MigrationAttributeTest
{
    [Fact]
    public void Constructor_WithValidParameters_SetsPropertiesCorrectly()
    {
        const string from = "1.0.0";
        const string to = "1.1.0";
        const bool shouldAvoid = true;

        var attribute = new MigrationAttribute(from, to, shouldAvoid);

        Assert.NotNull(attribute.FromVersion);
        Assert.Equal(1, attribute.FromVersion!.Major);
        Assert.Equal(0, attribute.FromVersion!.Minor);
        Assert.Equal(0, attribute.FromVersion!.Patch);
        Assert.Equal(0, attribute.FromVersion!.Build);
        
        Assert.NotNull(attribute.ToVersion);
        Assert.Equal(1, attribute.ToVersion.Major);
        Assert.Equal(1, attribute.ToVersion.Minor);
        Assert.Equal(0, attribute.ToVersion.Patch);
        Assert.Equal(0, attribute.ToVersion.Build);
        
        Assert.Equal(shouldAvoid, attribute.ShouldAvoid);
    }

    [Fact]
    public void Constructor_WithNullFrom_SetsFromVersionToNull()
    {
        const string to = "1.1.0";

        var attribute = new MigrationAttribute(null, to);

        Assert.Null(attribute.FromVersion);
        Assert.NotNull(attribute.ToVersion);
        Assert.Equal(1, attribute.ToVersion.Major);
        Assert.Equal(1, attribute.ToVersion.Minor);
        Assert.Equal(0, attribute.ToVersion.Patch);
        Assert.Equal(0, attribute.ToVersion.Build);
    }

    [Fact]
    public void Constructor_WithDefaultParameters_SetsDefaultValues()
    {
        const string from = "1.0.0";
        const string to = "1.1.0";

        var attribute = new MigrationAttribute(from, to);

        Assert.NotNull(attribute.FromVersion);
        Assert.Equal(1, attribute.FromVersion!.Major);
        Assert.Equal(0, attribute.FromVersion!.Minor);
        Assert.Equal(0, attribute.FromVersion!.Patch);
        Assert.Equal(0, attribute.FromVersion!.Build);
        
        Assert.NotNull(attribute.ToVersion);
        Assert.Equal(1, attribute.ToVersion.Major);
        Assert.Equal(1, attribute.ToVersion.Minor);
        Assert.Equal(0, attribute.ToVersion.Patch);
        Assert.Equal(0, attribute.ToVersion.Build);
        
        Assert.False(attribute.ShouldAvoid);
    }

    [Fact]
    public void Constructor_WithNullTo_ThrowsArgumentNullException()
    {
        const string from = "1.0.0";

        Assert.Throws<ArgumentNullException>(() => new MigrationAttribute(from, null!));
    }

    [Fact]
    public void Attribute_CanBeAppliedToClass()
    {
        var attributeUsage = typeof(MigrationAttribute).GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .Cast<AttributeUsageAttribute>()
            .FirstOrDefault();

        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Class, attributeUsage.ValidOn);
    }

    [Migration("1.0.0", "1.1.0", shouldAvoid: true)]
    private sealed class TestMigrationClass
    {
    }

    [Fact]
    public void Attribute_WhenAppliedToClass_CanBeRetrieved()
    {
        var attribute = typeof(TestMigrationClass).GetCustomAttributes(typeof(MigrationAttribute), false)
            .Cast<MigrationAttribute>()
            .FirstOrDefault();

        Assert.NotNull(attribute);
        Assert.NotNull(attribute.FromVersion);
        Assert.Equal(1, attribute.FromVersion!.Major);
        Assert.Equal(0, attribute.FromVersion!.Minor);
        Assert.Equal(0, attribute.FromVersion!.Patch);
        Assert.Equal(0, attribute.FromVersion!.Build);
        
        Assert.NotNull(attribute.ToVersion);
        Assert.Equal(1, attribute.ToVersion.Major);
        Assert.Equal(1, attribute.ToVersion.Minor);
        Assert.Equal(0, attribute.ToVersion.Patch);
        Assert.Equal(0, attribute.ToVersion.Build);
        
        Assert.True(attribute.ShouldAvoid);
    }

    [Theory]
    [InlineData("1.0.0")]
    [InlineData("0.1.0")]
    [InlineData("10.20.30")]
    [InlineData("1.2.3.4")]
    [InlineData("0.0.1")]
    [InlineData("5.10.15.20")]
    public void Constructor_WithValidDatabaseVersions_DoesNotThrow(string version)
    {
        var exception = Record.Exception(() => new MigrationAttribute(version, "2.0.0"));

        Assert.Null(exception);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid")]
    [InlineData("1.0.0-alpha")]
    [InlineData("1.0.0+build")]
    [InlineData("1.2.3.4.5")]
    [InlineData("-1.0.0")]
    [InlineData("1.-1.0")]
    [InlineData("1.0.-1")]
    [InlineData("1.0.0.-1")]
    public void Constructor_WithInvalidFromVersion_ThrowsException(string invalidVersion)
    {
        Assert.ThrowsAny<Exception>(() => new MigrationAttribute(invalidVersion, "2.0.0"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid")]
    [InlineData("1.0.0-alpha")]
    [InlineData("1.0.0+build")]
    [InlineData("1.2.3.4.5")]
    [InlineData("-1.0.0")]
    [InlineData("1.-1.0")]
    [InlineData("1.0.-1")]
    [InlineData("1.0.0.-1")]
    public void Constructor_WithInvalidToVersion_ThrowsException(string invalidVersion)
    {
        Assert.ThrowsAny<Exception>(() => new MigrationAttribute("1.0.0", invalidVersion));
    }

    [Fact]
    public void Constructor_SupportsVersionReversal_FromHigherToLower()
    {
        var attribute = new MigrationAttribute("2.0.0", "1.3.3");

        Assert.NotNull(attribute.FromVersion);
        Assert.Equal(2, attribute.FromVersion!.Major);
        Assert.Equal(0, attribute.FromVersion!.Minor);
        Assert.Equal(0, attribute.FromVersion!.Patch);
        Assert.Equal(0, attribute.FromVersion!.Build);
        
        Assert.NotNull(attribute.ToVersion);
        Assert.Equal(1, attribute.ToVersion.Major);
        Assert.Equal(3, attribute.ToVersion.Minor);
        Assert.Equal(3, attribute.ToVersion.Patch);
        Assert.Equal(0, attribute.ToVersion.Build);
    }
}
