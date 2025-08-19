namespace SmartMigrations.Test;

using SmartMigrations;
using Xunit;

public class MigrationAttributeSingleFromTests
{
    #region Constructor: MigrationAttribute(int from, int to, bool shouldAvoid = false)

    [Theory]
    [InlineData(0, 1, false, new int[] { 0 }, 1, false, false)]
    [InlineData(0, 1, true, new int[] { 0 }, 1, false, true)]
    [InlineData(8, 31, false, new int[] { 8 }, 31, false, false)]
    [InlineData(8, 31, true, new int[] { 8 }, 31, false, true)]
    [InlineData(168, 50, false, new int[] { 168 }, 50, false, false)]
    [InlineData(168, 50, true, new int[] { 168 }, 50, false, true)]
    [InlineData(0, int.MaxValue, false, new int[] { 0 }, int.MaxValue, false, false)]
    [InlineData(0, int.MaxValue, true, new int[] { 0 }, int.MaxValue, false, true)]
    public void Constructor_DefaultSchema_SingleFrom_Success(int from, int to, bool shouldAvoid, int[] expectedFromVersions, int expectedTo, bool expectedIsRange, bool expectedShouldAvoid)
    {
        var attribute = new MigrationAttribute(from, to, shouldAvoid);
        Assert.Equal(expectedFromVersions, attribute.FromVersions);
        Assert.Equal(expectedTo, attribute.ToVersion);
        Assert.Equal(expectedIsRange, attribute.IsRange);
        Assert.Equal(expectedShouldAvoid, attribute.ShouldAvoid);
        Assert.Null(attribute.FromSchema);
        Assert.Null(attribute.ToSchema);
    }

    [Theory]
    [InlineData(0, 0, false)]
    [InlineData(5, 5, true)]
    [InlineData(-42, -42, true)]
    [InlineData(100, 100, false)]
    [InlineData(int.MaxValue, int.MaxValue, true)]
    public void Constructor_DefaultSchema_SingleFrom_SameFromTo_ThrowsException(int from, int to, bool shouldAvoid)
    {
        Assert.Throws<ArgumentException>(() => new MigrationAttribute(from, to, shouldAvoid));
    }

    #endregion

    #region Constructor: MigrationAttribute(string? schema, int from, int to, bool shouldAvoid = false)

    [Theory]
    [InlineData(null, 0, 1, false, new int[] { 0 }, 1, false, false)]
    [InlineData(null, 0, 1, true, new int[] { 0 }, 1, false, true)]
    [InlineData("free", 1, 2, false, new int[] { 1 }, 2, false, false)]
    [InlineData("free", 1, 2, true, new int[] { 1 }, 2, false, true)]
    [InlineData("paid", 10, 20, false, new int[] { 10 }, 20, false, false)]
    [InlineData("paid", 10, 20, true, new int[] { 10 }, 20, false, true)]
    [InlineData("enterprise", 100, 50, false, new int[] { 100 }, 50, false, false)]
    [InlineData("enterprise", 100, 50, true, new int[] { 100 }, 50, false, true)]
    public void Constructor_SpecificSchema_SingleFrom_Success(string? schema, int from, int to, bool shouldAvoid, int[] expectedFromVersions, int expectedTo, bool expectedIsRange, bool expectedShouldAvoid)
    {
        var attribute = new MigrationAttribute(schema, from, to, shouldAvoid);
        Assert.Equal(expectedFromVersions, attribute.FromVersions);
        Assert.Equal(expectedTo, attribute.ToVersion);
        Assert.Equal(expectedIsRange, attribute.IsRange);
        Assert.Equal(expectedShouldAvoid, attribute.ShouldAvoid);
        Assert.Equal(schema, attribute.FromSchema);
        Assert.Equal(schema, attribute.ToSchema);
    }

    [Theory]
    [InlineData("", 1, 2, false)]
    [InlineData("   ", 1, 2, false)]
    [InlineData("\t", 1, 2, false)]
    [InlineData("\n", 1, 2, false)]
    public void Constructor_SpecificSchema_SingleFrom_InvalidSchema_ThrowsException(string schema, int from, int to, bool shouldAvoid)
    {
        Assert.Throws<ArgumentException>(() => new MigrationAttribute(schema, from, to, shouldAvoid));
    }

    #endregion

    #region Constructor: MigrationAttribute(string? fromSchema, int from, string? toSchema, int to, bool shouldAvoid = false)

    [Theory]
    [InlineData(null, 1, null, 2, false, new int[] { 1 }, 2, false, false)]
    [InlineData(null, 1, null, 2, true, new int[] { 1 }, 2, false, true)]
    [InlineData("free", 1, "free", 2, false, new int[] { 1 }, 2, false, false)]
    [InlineData("free", 1, "free", 2, true, new int[] { 1 }, 2, false, true)]
    [InlineData("free", 10, "paid", 1, false, new int[] { 10 }, 1, false, false)]
    [InlineData("free", 10, "paid", 1, true, new int[] { 10 }, 1, false, true)]
    [InlineData("paid", 5, "enterprise", 1, false, new int[] { 5 }, 1, false, false)]
    [InlineData("paid", 5, "enterprise", 1, true, new int[] { 5 }, 1, false, true)]
    [InlineData(null, 5, "premium", 1, false, new int[] { 5 }, 1, false, false)]
    [InlineData("basic", 5, null, 10, false, new int[] { 5 }, 10, false, false)]
    public void Constructor_CrossSchema_SingleFrom_Success(string? fromSchema, int from, string? toSchema, int to, bool shouldAvoid, int[] expectedFromVersions, int expectedTo, bool expectedIsRange, bool expectedShouldAvoid)
    {
        var attribute = new MigrationAttribute(fromSchema, from, toSchema, to, shouldAvoid);
        Assert.Equal(expectedFromVersions, attribute.FromVersions);
        Assert.Equal(expectedTo, attribute.ToVersion);
        Assert.Equal(expectedIsRange, attribute.IsRange);
        Assert.Equal(expectedShouldAvoid, attribute.ShouldAvoid);
        Assert.Equal(fromSchema, attribute.FromSchema);
        Assert.Equal(toSchema, attribute.ToSchema);
    }

    [Theory]
    [InlineData("", 1, "paid", 2, false)]
    [InlineData("   ", 1, "paid", 2, false)]
    [InlineData("free", 1, "", 2, false)]
    [InlineData("free", 1, "   ", 2, false)]
    [InlineData("\t", 1, "paid", 2, false)]
    [InlineData("free", 1, "\n", 2, false)]
    public void Constructor_CrossSchema_SingleFrom_InvalidSchema_ThrowsException(string fromSchema, int from, string toSchema, int to, bool shouldAvoid)
    {
        Assert.Throws<ArgumentException>(() => new MigrationAttribute(fromSchema, from, toSchema, to, shouldAvoid));
    }

    [Theory]
    [InlineData("free", 5, "paid", 5, false)]
    [InlineData("free", 5, "paid", 5, true)]
    [InlineData(null, 10, null, 10, false)]
    [InlineData("enterprise", -1, "enterprise", -1, true)]
    public void Constructor_CrossSchema_SingleFrom_SameFromTo_ThrowsException(string? fromSchema, int from, string? toSchema, int to, bool shouldAvoid)
    {
        Assert.Throws<ArgumentException>(() => new MigrationAttribute(fromSchema, from, toSchema, to, shouldAvoid));
    }

    #endregion

    #region Edge Cases for Single From Migrations

    [Fact]
    public void Constructor_SingleFrom_FromVersionsLength()
    {
        var attribute = new MigrationAttribute(5, 10);
        Assert.Single(attribute.FromVersions);
        Assert.Equal(5, attribute.FromVersions[0]);
    }

    [Fact]
    public void Constructor_SingleFrom_IsRangeFalse()
    {
        var attribute = new MigrationAttribute(5, 10);
        Assert.False(attribute.IsRange);
    }

    [Fact]
    public void Constructor_SingleFrom_NegativeVersions()
    {
        var attribute = new MigrationAttribute(-100, -50);
        Assert.Equal(new int[] { -100 }, attribute.FromVersions);
        Assert.Equal(-50, attribute.ToVersion);
    }

    #endregion
}