namespace SmartMigrations.Test;

using SmartMigrations;
using Xunit;

public class MigrationAttributeListFromTests
{
    #region Constructor: MigrationAttribute(int[] fromList, int to, bool shouldAvoid = false)

    [Theory]
    [InlineData(new int[] { 1 }, 5, false, new int[] { 1 }, 5, false, false)]
    [InlineData(new int[] { 1, 3, 5 }, 10, true, new int[] { 1, 3, 5 }, 10, false, true)]
    [InlineData(new int[] { 0, 1, 2 }, 3, false, new int[] { 0, 1, 2 }, 3, false, false)]
    [InlineData(new int[] { 10, 20, 30, 40 }, 50, true, new int[] { 10, 20, 30, 40 }, 50, false, true)]
    [InlineData(new int[] { 100 }, 200, false, new int[] { 100 }, 200, false, false)]
    [InlineData(new int[] { 5, 1, 3 }, 10, false, new int[] { 5, 1, 3 }, 10, false, false)]
    [InlineData(new int[] { 5, 1, 3, 1, 5 }, 10, true, new int[] { 5, 1, 3 }, 10, false, true)]
    public void Constructor_DefaultSchema_ListFrom_Success(int[] fromList, int to, bool shouldAvoid, int[] expectedFromVersions, int expectedTo, bool expectedIsRange, bool expectedShouldAvoid)
    {
        var attribute = new MigrationAttribute(fromList, to, shouldAvoid);
        Assert.Equal(expectedFromVersions, attribute.FromVersions);
        Assert.Equal(expectedTo, attribute.ToVersion);
        Assert.Equal(expectedIsRange, attribute.IsRange);
        Assert.Equal(expectedShouldAvoid, attribute.ShouldAvoid);
        Assert.Null(attribute.FromSchema);
        Assert.Null(attribute.ToSchema);
    }

    [Theory]
    [InlineData(new int[] { 1, 5 }, 5, false)]
    [InlineData(new int[] { 7, -12 }, 7, false)]
    [InlineData(new int[] { 7, -12 }, -12, true)]
    [InlineData(new int[] { 10, 20, 30 }, 20, true)]
    [InlineData(new int[] { 5 }, 5, false)]
    public void Constructor_DefaultSchema_ListFrom_ContainsTo_ThrowsException(int[] fromList, int to, bool shouldAvoid)
    {
        Assert.Throws<ArgumentException>(() => new MigrationAttribute(fromList, to, shouldAvoid));
    }

    [Fact]
    public void Constructor_DefaultSchema_ListFrom_NullList_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new MigrationAttribute((int[])null!, 5));
    }

    #endregion

    #region Constructor: MigrationAttribute(string? schema, int[] fromList, int to, bool shouldAvoid = false)

    [Theory]
    [InlineData(null, new int[] { 1 }, 5, false, new int[] { 1 }, 5, false, false)]
    [InlineData(null, new int[] { 1 }, 5, true, new int[] { 1 }, 5, false, true)]
    [InlineData("free", new int[] { 1, 2, 3 }, 10, false, new int[] { 1, 2, 3 }, 10, false, false)]
    [InlineData("free", new int[] { 1, 2, 3 }, 10, true, new int[] { 1, 2, 3 }, 10, false, true)]
    [InlineData("paid", new int[] { 5, 10, 15, 20 }, 25, false, new int[] { 5, 10, 15, 20 }, 25, false, false)]
    [InlineData("paid", new int[] { 5, 10, 15, 20 }, 25, true, new int[] { 5, 10, 15, 20 }, 25, false, true)]
    [InlineData("enterprise", new int[] { -10, -5, 0, 5, 10 }, 15, false, new int[] { -10, -5, 0, 5, 10 }, 15, false, false)]
    [InlineData("enterprise", new int[] { -10, -5, 0, 5, 10 }, 15, true, new int[] { -10, -5, 0, 5, 10 }, 15, false, true)]
    public void Constructor_SpecificSchema_ListFrom_Success(string? schema, int[] fromList, int to, bool shouldAvoid, int[] expectedFromVersions, int expectedTo, bool expectedIsRange, bool expectedShouldAvoid)
    {
        var attribute = new MigrationAttribute(schema, fromList, to, shouldAvoid);
        Assert.Equal(expectedFromVersions, attribute.FromVersions);
        Assert.Equal(expectedTo, attribute.ToVersion);
        Assert.Equal(expectedIsRange, attribute.IsRange);
        Assert.Equal(expectedShouldAvoid, attribute.ShouldAvoid);
        Assert.Equal(schema, attribute.FromSchema);
        Assert.Equal(schema, attribute.ToSchema);
    }

    [Theory]
    [InlineData("", new int[] { 1 }, 5, false)]
    [InlineData("   ", new int[] { 1 }, 5, false)]
    [InlineData("\t", new int[] { 1 }, 5, false)]
    [InlineData("\n", new int[] { 1 }, 5, false)]
    public void Constructor_SpecificSchema_ListFrom_InvalidSchema_ThrowsException(string schema, int[] fromList, int to, bool shouldAvoid)
    {
        Assert.Throws<ArgumentException>(() => new MigrationAttribute(schema, fromList, to, shouldAvoid));
    }

    #endregion

    #region Constructor: MigrationAttribute(string? fromSchema, int[] fromList, string? toSchema, int to, bool shouldAvoid = false)

    [Theory]
    [InlineData(null, new int[] { 1 }, null, 5, false, new int[] { 1 }, 5, false, false)]
    [InlineData(null, new int[] { 1 }, null, 5, true, new int[] { 1 }, 5, false, true)]
    [InlineData("free", new int[] { 1, 2, 3 }, "free", 10, false, new int[] { 1, 2, 3 }, 10, false, false)]
    [InlineData("free", new int[] { 1, 2, 3 }, "free", 10, true, new int[] { 1, 2, 3 }, 10, false, true)]
    [InlineData("free", new int[] { 5, 10, 15 }, "paid", 1, false, new int[] { 5, 10, 15 }, 1, false, false)]
    [InlineData("free", new int[] { 5, 10, 15 }, "paid", 1, true, new int[] { 5, 10, 15 }, 1, false, true)]
    [InlineData("paid", new int[] { 10, 20 }, "enterprise", 1, false, new int[] { 10, 20 }, 1, false, false)]
    [InlineData("paid", new int[] { 10, 20 }, "enterprise", 1, true, new int[] { 10, 20 }, 1, false, true)]
    [InlineData(null, new int[] { 5, 10 }, "premium", 1, false, new int[] { 5, 10 }, 1, false, false)]
    [InlineData("basic", new int[] { 5, 10 }, null, 15, false, new int[] { 5, 10 }, 15, false, false)]
    public void Constructor_CrossSchema_ListFrom_Success(string? fromSchema, int[] fromList, string? toSchema, int to, bool shouldAvoid, int[] expectedFromVersions, int expectedTo, bool expectedIsRange, bool expectedShouldAvoid)
    {
        var attribute = new MigrationAttribute(fromSchema, fromList, toSchema, to, shouldAvoid);
        Assert.Equal(expectedFromVersions, attribute.FromVersions);
        Assert.Equal(expectedTo, attribute.ToVersion);
        Assert.Equal(expectedIsRange, attribute.IsRange);
        Assert.Equal(expectedShouldAvoid, attribute.ShouldAvoid);
        Assert.Equal(fromSchema, attribute.FromSchema);
        Assert.Equal(toSchema, attribute.ToSchema);
    }

    [Theory]
    [InlineData("", new int[] { 1 }, "paid", 5, false)]
    [InlineData("   ", new int[] { 1 }, "paid", 5, false)]
    [InlineData("free", new int[] { 1 }, "", 5, false)]
    [InlineData("free", new int[] { 1 }, "   ", 5, false)]
    [InlineData("\t", new int[] { 1 }, "paid", 5, false)]
    [InlineData("free", new int[] { 1 }, "\n", 5, false)]
    public void Constructor_CrossSchema_ListFrom_InvalidSchema_ThrowsException(string fromSchema, int[] fromList, string toSchema, int to, bool shouldAvoid)
    {
        Assert.Throws<ArgumentException>(() => new MigrationAttribute(fromSchema, fromList, toSchema, to, shouldAvoid));
    }

    #endregion

    #region Edge Cases for List From Migrations

    [Fact]
    public void Constructor_ListFrom_DuplicatesRemoved()
    {
        var attribute = new MigrationAttribute(new int[] { 1, 2, 2, 3, 1, 3 }, 10);
        Assert.Equal(new int[] { 1, 2, 3 }, attribute.FromVersions);
    }

    [Fact]
    public void Constructor_ListFrom_EmptyList()
    {
        var attribute = new MigrationAttribute(new int[] { }, 10);
        Assert.Empty(attribute.FromVersions);
        Assert.Equal(10, attribute.ToVersion);
    }

    [Fact]
    public void Constructor_ListFrom_IsRangeFalse()
    {
        var attribute = new MigrationAttribute(new int[] { 1, 2, 3 }, 10);
        Assert.False(attribute.IsRange);
    }

    [Fact]
    public void Constructor_ListFrom_OrderPreserved()
    {
        var attribute = new MigrationAttribute(new int[] { 5, 1, 3, 2 }, 10);
        Assert.Equal(new int[] { 5, 1, 3, 2 }, attribute.FromVersions);
    }

    [Fact]
    public void Constructor_ListFrom_LargeList()
    {
        var largeList = Enumerable.Range(1, 100).ToArray();
        var attribute = new MigrationAttribute(largeList, 200);
        Assert.Equal(largeList, attribute.FromVersions);
        Assert.Equal(200, attribute.ToVersion);
    }

    #endregion
}