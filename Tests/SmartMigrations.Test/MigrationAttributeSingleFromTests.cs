namespace SmartMigrations.Test;

using SmartMigrations;
using Xunit;

public class MigrationAttributeSingleFromTests
{
    #region Constructor: MigrationAttribute(int from, int to, bool shouldAvoid = false)

    [Theory]
    [InlineData(0, 1, false, new[] { 0 })]
    [InlineData(5, 1, true, new[] { 5 })]
    [InlineData(8, 31, null, new[] { 8 })]
    [InlineData(8, -105631, true, new[] { 8 })]
    [InlineData(168, -50, false, new[] { 168 })]
    [InlineData(-168, 50, true, new[] { -168 })]
    [InlineData(0, int.MaxValue, false, new[] { 0 })]
    [InlineData(int.MaxValue, int.MinValue, true, new[] { int.MaxValue })]
    public void Constructor_DefaultSchema_SingleFrom_Success(int from, int to, bool? shouldAvoid, int[] expectedFromVersions)
    {
        var attribute = shouldAvoid.HasValue
            ? new MigrationAttribute(from, to, shouldAvoid.Value)
            : new MigrationAttribute(from, to);
        Assert.Null(attribute.FromSchema);
        Assert.Equal(expectedFromVersions, attribute.FromVersions);
        Assert.False(attribute.IsRange);
        Assert.Null(attribute.ToSchema);
        Assert.Equal(to, attribute.ToVersion);
        if (shouldAvoid.HasValue) Assert.Equal(shouldAvoid.Value, attribute.ShouldAvoid);
        else Assert.False(attribute.ShouldAvoid);
    }

    [Theory]
    [InlineData(0, 0, false)]
    [InlineData(5, 5, true)]
    [InlineData(-42, -42, true)]
    [InlineData(100, 100, false)]
    [InlineData(int.MaxValue, int.MaxValue, true)]
    [InlineData(int.MinValue, int.MinValue, true)]
    public void Constructor_DefaultSchema_SingleFrom_SameFromTo_ThrowsException(int from, int to, bool shouldAvoid)
    {
        Assert.Throws<ArgumentException>(() => new MigrationAttribute(from, to, shouldAvoid));
    }

    #endregion

    #region Constructor: MigrationAttribute(string? schema, int from, int to, bool shouldAvoid = false)

    [Theory]
    [InlineData(null, 0, 1, false, new[] { 0 }, null)]
    [InlineData(null, 0, -1, true, new[] { 0 }, null)]
    [InlineData("free", 1, 2, false, new[] { 1 }, "free")]
    [InlineData("fr ee", -6, 2, true, new[] { -6 }, "fr ee")]
    [InlineData("paid ", 10, 20, false, new[] { 10 }, "paid")]
    [InlineData("  \npaid", int.MaxValue, 20, true, new[] { int.MaxValue }, "paid")]
    [InlineData("enterprise", 100, 50, false, new[] { 100 }, "enterprise")]
    [InlineData("\n enterprise \t\t", int.MinValue, int.MinValue + 1, true, new[] { int.MinValue }, "enterprise")]
    [InlineData(" 123 How Are You?\n\r", 100, -50, true, new[] { 100 }, "123 How Are You?")]
    public void Constructor_SpecificSchema_SingleFrom_Success(string? schema, int from, int to, bool shouldAvoid, int[] expectedFromVersions, string? expectedSchema)
    {
        var attribute = new MigrationAttribute(schema, from, to, shouldAvoid);
        Assert.Equal(expectedSchema, attribute.FromSchema);
        Assert.Equal(expectedFromVersions, attribute.FromVersions);
        Assert.False(attribute.IsRange);
        Assert.Equal(expectedSchema, attribute.ToSchema);
        Assert.Equal(to, attribute.ToVersion);
        Assert.Equal(shouldAvoid, attribute.ShouldAvoid);
    }

    [Theory]
    [InlineData("", 1, 2)]
    [InlineData("   ", 1, 2)]
    [InlineData("\t", 1, 2)]
    [InlineData("\n", 1, 2)]
    public void Constructor_SpecificSchema_SingleFrom_InvalidSchema_ThrowsException(string schema, int from, int to)
    {
        Assert.Throws<ArgumentException>(() => new MigrationAttribute(schema, from, to));
    }

    #endregion

    #region Constructor: MigrationAttribute(string? fromSchema, int from, string? toSchema, int to, bool shouldAvoid = false)

    [Theory]
    [InlineData(null, 1, null, 2, true, new[] { 1 }, null, null)]
    [InlineData(null, -11, null, 2, false, new[] { -11 }, null, null)]
    [InlineData("free", 1, "free", 200, null, new[] { 1 }, "free", "free")]
    [InlineData("free", -101, "free", -200, true, new[] { -101 }, "free", "free")]
    [InlineData("free", 10, "paid", 1, false, new[] { 10 }, "free", "paid")]
    [InlineData("free ", 10, " paid", 1, null, new[] { 10 }, "free", "paid")]
    [InlineData("paid\n", 5, "enterprise", 1, true, new[] { 5 }, "paid", "enterprise")]
    [InlineData("paid", 5, "enterprise", 1, false, new[] { 5 }, "paid", "enterprise")]
    [InlineData(null, 5, "premium\n\n", 1, null, new[] { 5 }, null, "premium")]
    [InlineData("basic", 5, null, 10, true, new[] { 5 }, "basic", null)]
    public void Constructor_CrossSchema_SingleFrom_Success(string? fromSchema, int from, string? toSchema, int to, bool? shouldAvoid, int[] expectedFromVersions, string? expectedFromSchema, string? expectedToSchema)
    {
        var attribute = shouldAvoid.HasValue
            ? new MigrationAttribute(fromSchema, from, toSchema, to, shouldAvoid.Value)
            : new MigrationAttribute(fromSchema, from, toSchema, to);
        Assert.Equal(expectedFromSchema, attribute.FromSchema);
        Assert.Equal(expectedFromVersions, attribute.FromVersions);
        Assert.False(attribute.IsRange);
        Assert.Equal(expectedToSchema, attribute.ToSchema);
        Assert.Equal(to, attribute.ToVersion);
        if (shouldAvoid.HasValue) Assert.Equal(shouldAvoid.Value, attribute.ShouldAvoid);
        else Assert.False(attribute.ShouldAvoid);
    }

    [Theory]
    [InlineData("", 1, "paid", 2)]
    [InlineData("   ", 1, "paid", 2)]
    [InlineData("free", 1, "", 2)]
    [InlineData("free", 1, "   ", 2)]
    [InlineData("\t", 1, "paid", 2)]
    [InlineData("free", 1, "\n", 2)]
    [InlineData("  ", 1, "\n", 2)]
    public void Constructor_CrossSchema_SingleFrom_InvalidSchema_ThrowsException(string fromSchema, int from, string toSchema, int to)
    {
        Assert.Throws<ArgumentException>(() => new MigrationAttribute(fromSchema, from, toSchema, to));
    }

    [Theory]
    [InlineData("free", 5, "paid", 5, false)]
    [InlineData("free", 5, "paid ", 5, true)]
    [InlineData(null, 10, null, 10, false)]
    [InlineData("\n\nenterprise", -1, "  enterprise \t\r\n  ", -1, true)]
    public void Constructor_CrossSchema_SingleFrom_SameFromTo_ThrowsException(string? fromSchema, int from, string? toSchema, int to, bool shouldAvoid)
    {
        Assert.Throws<ArgumentException>(() => new MigrationAttribute(fromSchema, from, toSchema, to, shouldAvoid));
    }

    #endregion
}
