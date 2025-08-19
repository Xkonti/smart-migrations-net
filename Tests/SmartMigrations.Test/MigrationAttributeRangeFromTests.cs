namespace SmartMigrations.Test;

using SmartMigrations;
using Xunit;

public class MigrationAttributeRangeFromTests
{
    #region Constructor: MigrationAttribute(int fromRangeStart, int fromRangeEnd, int to, bool shouldAvoid = false)

    [Theory]
    [InlineData(1, 3, 10, false, new int[] { 1, 3 }, 10, true, false)]
    [InlineData(5, 5, 8, true, new int[] { 5, 5 }, 8, true, true)]
    [InlineData(0, 2, 5, false, new int[] { 0, 2 }, 5, true, false)]
    [InlineData(-10, 15, 20, true, new int[] { -10, 15 }, 20, true, true)]
    [InlineData(0, 0, 1, false, new int[] { 0, 0 }, 1, true, false)]
    [InlineData(100, 200, 300, false, new int[] { 100, 200 }, 300, true, false)]
    public void Constructor_DefaultSchema_RangeFrom_Success(int fromRangeStart, int fromRangeEnd, int to, bool shouldAvoid, int[] expectedFromVersions, int expectedTo, bool expectedIsRange, bool expectedShouldAvoid)
    {
        var attribute = new MigrationAttribute(fromRangeStart, fromRangeEnd, to, shouldAvoid);
        Assert.Equal(expectedFromVersions, attribute.FromVersions);
        Assert.Equal(expectedTo, attribute.ToVersion);
        Assert.Equal(expectedIsRange, attribute.IsRange);
        Assert.Equal(expectedShouldAvoid, attribute.ShouldAvoid);
        Assert.Null(attribute.FromSchema);
        Assert.Null(attribute.ToSchema);
    }

    [Theory]
    [InlineData(5, 3, 10, false)]
    [InlineData(10, 5, 15, true)]
    [InlineData(-10, -55, 15, true)]
    [InlineData(100, 50, 200, false)]
    public void Constructor_DefaultSchema_RangeFrom_InvalidRange_ThrowsException(int fromRangeStart, int fromRangeEnd, int to, bool shouldAvoid)
    {
        Assert.Throws<ArgumentException>(() => new MigrationAttribute(fromRangeStart, fromRangeEnd, to, shouldAvoid));
    }

    [Theory]
    [InlineData(5, 10, 7, false)]
    [InlineData(1, 5, 3, true)]
    [InlineData(-10, 20, 15, false)]
    [InlineData(5, 10, 5, true)]
    [InlineData(5, 10, 10, false)]
    public void Constructor_DefaultSchema_RangeFrom_ToInRange_ThrowsException(int fromRangeStart, int fromRangeEnd, int to, bool shouldAvoid)
    {
        Assert.Throws<ArgumentException>(() => new MigrationAttribute(fromRangeStart, fromRangeEnd, to, shouldAvoid));
    }

    #endregion

    #region Constructor: MigrationAttribute(string? schema, int fromRangeStart, int fromRangeEnd, int to, bool shouldAvoid = false)

    [Theory]
    [InlineData(null, 1, 5, 10, false, new int[] { 1, 5 }, 10, true, false)]
    [InlineData(null, 1, 5, 10, true, new int[] { 1, 5 }, 10, true, true)]
    [InlineData("free", 0, 10, 20, false, new int[] { 0, 10 }, 20, true, false)]
    [InlineData("free", 0, 10, 20, true, new int[] { 0, 10 }, 20, true, true)]
    [InlineData("paid", -10, 10, 20, false, new int[] { -10, 10 }, 20, true, false)]
    [InlineData("paid", -10, 10, 20, true, new int[] { -10, 10 }, 20, true, true)]
    [InlineData("enterprise", -100, -50, -25, false, new int[] { -100, -50 }, -25, true, false)]
    [InlineData("enterprise", -100, -50, -25, true, new int[] { -100, -50 }, -25, true, true)]
    public void Constructor_SpecificSchema_RangeFrom_Success(string? schema, int fromRangeStart, int fromRangeEnd, int to, bool shouldAvoid, int[] expectedFromVersions, int expectedTo, bool expectedIsRange, bool expectedShouldAvoid)
    {
        var attribute = new MigrationAttribute(schema, fromRangeStart, fromRangeEnd, to, shouldAvoid);
        Assert.Equal(expectedFromVersions, attribute.FromVersions);
        Assert.Equal(expectedTo, attribute.ToVersion);
        Assert.Equal(expectedIsRange, attribute.IsRange);
        Assert.Equal(expectedShouldAvoid, attribute.ShouldAvoid);
        Assert.Equal(schema, attribute.FromSchema);
        Assert.Equal(schema, attribute.ToSchema);
    }

    [Theory]
    [InlineData("", 1, 5, 10, false)]
    [InlineData("   ", 1, 5, 10, false)]
    [InlineData("\t", 1, 5, 10, false)]
    [InlineData("\n", 1, 5, 10, false)]
    public void Constructor_SpecificSchema_RangeFrom_InvalidSchema_ThrowsException(string schema, int fromRangeStart, int fromRangeEnd, int to, bool shouldAvoid)
    {
        Assert.Throws<ArgumentException>(() => new MigrationAttribute(schema, fromRangeStart, fromRangeEnd, to, shouldAvoid));
    }

    #endregion

    #region Constructor: MigrationAttribute(string? fromSchema, int fromRangeStart, int fromRangeEnd, string? toSchema, int to, bool shouldAvoid = false)

    [Theory]
    [InlineData(null, 1, 5, null, 10, false, new int[] { 1, 5 }, 10, true, false)]
    [InlineData(null, 1, 5, null, 10, true, new int[] { 1, 5 }, 10, true, true)]
    [InlineData("free", 0, 10, "free", 20, false, new int[] { 0, 10 }, 20, true, false)]
    [InlineData("free", 0, 10, "free", 20, true, new int[] { 0, 10 }, 20, true, true)]
    [InlineData("free", 5, 15, "paid", 1, false, new int[] { 5, 15 }, 1, true, false)]
    [InlineData("free", 5, 15, "paid", 1, true, new int[] { 5, 15 }, 1, true, true)]
    [InlineData("paid", 10, 20, "enterprise", 1, false, new int[] { 10, 20 }, 1, true, false)]
    [InlineData("paid", 10, 20, "enterprise", 1, true, new int[] { 10, 20 }, 1, true, true)]
    [InlineData(null, 5, 10, "premium", 1, false, new int[] { 5, 10 }, 1, true, false)]
    [InlineData("basic", 5, 10, null, 15, false, new int[] { 5, 10 }, 15, true, false)]
    public void Constructor_CrossSchema_RangeFrom_Success(string? fromSchema, int fromRangeStart, int fromRangeEnd, string? toSchema, int to, bool shouldAvoid, int[] expectedFromVersions, int expectedTo, bool expectedIsRange, bool expectedShouldAvoid)
    {
        var attribute = new MigrationAttribute(fromSchema, fromRangeStart, fromRangeEnd, toSchema, to, shouldAvoid);
        Assert.Equal(expectedFromVersions, attribute.FromVersions);
        Assert.Equal(expectedTo, attribute.ToVersion);
        Assert.Equal(expectedIsRange, attribute.IsRange);
        Assert.Equal(expectedShouldAvoid, attribute.ShouldAvoid);
        Assert.Equal(fromSchema, attribute.FromSchema);
        Assert.Equal(toSchema, attribute.ToSchema);
    }

    [Theory]
    [InlineData("", 1, 5, "paid", 10, false)]
    [InlineData("   ", 1, 5, "paid", 10, false)]
    [InlineData("free", 1, 5, "", 10, false)]
    [InlineData("free", 1, 5, "   ", 10, false)]
    [InlineData("\t", 1, 5, "paid", 10, false)]
    [InlineData("free", 1, 5, "\n", 10, false)]
    public void Constructor_CrossSchema_RangeFrom_InvalidSchema_ThrowsException(string fromSchema, int fromRangeStart, int fromRangeEnd, string toSchema, int to, bool shouldAvoid)
    {
        Assert.Throws<ArgumentException>(() => new MigrationAttribute(fromSchema, fromRangeStart, fromRangeEnd, toSchema, to, shouldAvoid));
    }

    #endregion

    #region Edge Cases for Range From Migrations

    [Fact]
    public void Constructor_RangeFrom_IsRangeTrue()
    {
        var attribute = new MigrationAttribute(1, 10, 20);
        Assert.True(attribute.IsRange);
    }

    [Fact]
    public void Constructor_RangeFrom_FromVersionsContainsTwoElements()
    {
        var attribute = new MigrationAttribute(5, 15, 20);
        Assert.Equal(2, attribute.FromVersions.Length);
        Assert.Equal(5, attribute.FromVersions[0]);
        Assert.Equal(15, attribute.FromVersions[1]);
    }

    [Fact]
    public void Constructor_RangeFrom_SingleValueRange()
    {
        var attribute = new MigrationAttribute(5, 5, 10);
        Assert.Equal(new int[] { 5, 5 }, attribute.FromVersions);
        Assert.True(attribute.IsRange);
    }

    [Fact]
    public void Constructor_RangeFrom_LargeRange()
    {
        var attribute = new MigrationAttribute(1, 10000, 10001);
        Assert.Equal(new int[] { 1, 10000 }, attribute.FromVersions);
        Assert.Equal(10001, attribute.ToVersion);
    }

    [Fact]
    public void Constructor_RangeFrom_NegativeRange()
    {
        var attribute = new MigrationAttribute(-1000, -500, -250);
        Assert.Equal(new int[] { -1000, -500 }, attribute.FromVersions);
        Assert.Equal(-250, attribute.ToVersion);
    }

    [Fact]
    public void Constructor_RangeFrom_CrossingZeroRange()
    {
        var attribute = new MigrationAttribute(-100, 100, 200);
        Assert.Equal(new int[] { -100, 100 }, attribute.FromVersions);
        Assert.Equal(200, attribute.ToVersion);
    }

    #endregion
}