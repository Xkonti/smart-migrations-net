namespace SmartMigrations.Test;

using SmartMigrations;
using Xunit;

public class MigrationAttributeNewSchemaTests
{
    #region Constructor: MigrationAttribute(int to, bool shouldAvoid = false)

    [Theory]
    [InlineData(0, false)]
    [InlineData(0, true)]
    [InlineData(1, false)]
    [InlineData(1, true)]
    [InlineData(30, null)]
    [InlineData(30, true)]
    [InlineData(int.MaxValue, false)]
    [InlineData(int.MaxValue, true)]
    public void Constructor_DefaultSchema_InitialSetup_Success(int to, bool? shouldAvoid)
    {
        var attribute = shouldAvoid != null
            ? new MigrationAttribute(to, shouldAvoid.Value)
            : new MigrationAttribute(to);
        Assert.Null(attribute.FromSchema);
        Assert.Empty(attribute.FromVersions);
        Assert.False(attribute.IsRange);
        Assert.Null(attribute.ToSchema);
        Assert.Equal(to, attribute.ToVersion);
        if (shouldAvoid.HasValue) Assert.Equal(shouldAvoid.Value, attribute.ShouldAvoid);
        else Assert.False(attribute.ShouldAvoid);
    }

    #endregion

    #region Constructor: MigrationAttribute(string? schema, int to, bool shouldAvoid = false)

    [Theory]
    [InlineData(null, 0, false, null)]
    [InlineData(null, 1, true, null)]
    [InlineData("free", 0, false, "free")]
    [InlineData("free", 135, true, "free")]
    [InlineData("paid", 100, false, "paid")]
    [InlineData("paid", -100, true,  "paid")]
    [InlineData("enterprise", int.MaxValue, false, "enterprise")]
    [InlineData("enterprise", int.MaxValue, true,  "enterprise")]
    [InlineData("custom_schema", int.MinValue, false, "custom_schema")]
    [InlineData("custom_schema", int.MinValue, true,  "custom_schema")]
    [InlineData(" a schema   ", -315498, false, "a schema")]
    [InlineData("\n\t\nsomeThing123   \n", 69420, true,  "someThing123")]
    public void Constructor_SpecificSchema_InitialSetup_Success(string? schema, int to, bool shouldAvoid, string? expectedSchema)
    {
        var attribute = new MigrationAttribute(schema, to, shouldAvoid);
        Assert.Equal(expectedSchema, attribute.FromSchema);
        Assert.Empty(attribute.FromVersions);
        Assert.False(attribute.IsRange);
        Assert.Equal(expectedSchema, attribute.ToSchema);
        Assert.Equal(to, attribute.ToVersion);
        Assert.Equal(shouldAvoid, attribute.ShouldAvoid);
    }

    [Theory]
    [InlineData("", 1, false)]
    [InlineData("   ", 1, true)]
    [InlineData("\t", 1, false)]
    [InlineData("\n", 1, true)]
    [InlineData("\t  \n", 1, false)]
    [InlineData("\n\n\n    \n", 1, true)]
    public void Constructor_SpecificSchema_InitialSetup_InvalidSchema_ThrowsException(string schema, int to, bool shouldAvoid)
    {
        Assert.Throws<ArgumentException>(() => new MigrationAttribute(schema, to, shouldAvoid));
    }

    #endregion
}
