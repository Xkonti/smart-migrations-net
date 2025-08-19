namespace SmartMigrations.Test;

using SmartMigrations;
using Xunit;

public class MigrationAttributeNewSchemaTests
{
    #region Constructor: MigrationAttribute(int to, bool shouldAvoid = false)

    [Theory]
    [InlineData(0, false, new int[0], 0, false, false)]
    [InlineData(0, true, new int[0], 0, false, true)]
    [InlineData(1, false, new int[0], 1, false, false)]
    [InlineData(1, true, new int[0], 1, false, true)]
    [InlineData(30, false, new int[0], 30, false, false)]
    [InlineData(30, true, new int[0], 30, false, true)]
    [InlineData(int.MaxValue, false, new int[0], int.MaxValue, false, false)]
    [InlineData(int.MaxValue, true, new int[0], int.MaxValue, false, true)]
    public void Constructor_DefaultSchema_InitialSetup_Success(int to, bool shouldAvoid, int[] expectedFromVersions, int expectedTo, bool expectedIsRange, bool expectedShouldAvoid)
    {
        var attribute = new MigrationAttribute(to, shouldAvoid);
        Assert.Equal(expectedFromVersions, attribute.FromVersions);
        Assert.Equal(expectedTo, attribute.ToVersion);
        Assert.Equal(expectedIsRange, attribute.IsRange);
        Assert.Equal(expectedShouldAvoid, attribute.ShouldAvoid);
        Assert.Null(attribute.FromSchema);
        Assert.Null(attribute.ToSchema);
    }

    #endregion

    #region Constructor: MigrationAttribute(string? schema, int to, bool shouldAvoid = false)

    [Theory]
    [InlineData(null, 0, false, new int[0], 0, false, false)]
    [InlineData(null, 1, true, new int[0], 1, false, true)]
    [InlineData("free", 0, false, new int[0], 0, false, false)]
    [InlineData("free", 1, true, new int[0], 1, false, true)]
    [InlineData("paid", 100, false, new int[0], 100, false, false)]
    [InlineData("paid", 100, true, new int[0], 100, false, true)]
    [InlineData("enterprise", int.MaxValue, false, new int[0], int.MaxValue, false, false)]
    [InlineData("enterprise", int.MaxValue, true, new int[0], int.MaxValue, false, true)]
    [InlineData("custom_schema", int.MinValue, false, new int[0], int.MinValue, false, false)]
    [InlineData("custom_schema", int.MinValue, true, new int[0], int.MinValue, false, true)]
    public void Constructor_SpecificSchema_InitialSetup_Success(string? schema, int to, bool shouldAvoid, int[] expectedFromVersions, int expectedTo, bool expectedIsRange, bool expectedShouldAvoid)
    {
        var attribute = new MigrationAttribute(schema, to, shouldAvoid);
        Assert.Equal(expectedFromVersions, attribute.FromVersions);
        Assert.Equal(expectedTo, attribute.ToVersion);
        Assert.Equal(expectedIsRange, attribute.IsRange);
        Assert.Equal(expectedShouldAvoid, attribute.ShouldAvoid);
        Assert.Equal(schema, attribute.FromSchema);
        Assert.Equal(schema, attribute.ToSchema);
    }

    [Theory]
    [InlineData("", 1, false)]
    [InlineData("   ", 1, false)]
    [InlineData("\t", 1, false)]
    [InlineData("\n", 1, false)]
    public void Constructor_SpecificSchema_InitialSetup_InvalidSchema_ThrowsException(string schema, int to, bool shouldAvoid)
    {
        Assert.Throws<ArgumentException>(() => new MigrationAttribute(schema, to, shouldAvoid));
    }

    #endregion

    #region Edge Cases for Initial Setup Migrations

    [Fact]
    public void Constructor_InitialSetup_EmptyFromVersions()
    {
        var attribute = new MigrationAttribute(10);
        Assert.Empty(attribute.FromVersions);
    }

    [Fact]
    public void Constructor_InitialSetup_IsRangeFalse()
    {
        var attribute = new MigrationAttribute(10);
        Assert.False(attribute.IsRange);
    }

    [Fact]
    public void Constructor_InitialSetup_SchemaConsistency()
    {
        var attribute = new MigrationAttribute("test", 10);
        Assert.Equal("test", attribute.FromSchema);
        Assert.Equal("test", attribute.ToSchema);
    }

    #endregion
}