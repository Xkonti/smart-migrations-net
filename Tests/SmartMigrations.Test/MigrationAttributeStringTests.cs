namespace SmartMigrations.Test;

using SmartMigrations;
using Xunit;

public class MigrationAttributeStringTests
{
    #region Constructor: MigrationAttribute(string? from, string to, bool shouldAvoid = false)

    [Theory]
    [InlineData(null, "5", false, new int[0], 5, false, false)]
    [InlineData(null, "-15", false, new int[0], -15, false, false)]
    [InlineData("3", "10", true, new int[] { 3 }, 10, false, true)]
    [InlineData("3", "-11", false, new int[] { 3 }, -11, false, false)]
    [InlineData("-9", "-8", true, new int[] { -9 }, -8, false, true)]
    [InlineData("-9", "0", true, new int[] { -9 }, 0, false, true)]
    [InlineData("-46", "158", true, new int[] { -46 }, 158, false, true)]
    [InlineData("1,3,5", "8", false, new int[] { 1, 3, 5 }, 8, false, false)]
    [InlineData("2..5", "10", true, new int[] { 2, 5 }, 10, true, true)]
    [InlineData("0", "1", false, new int[] { 0 }, 1, false, false)]
    [InlineData(" 1 , 2 , 3 ", " 10 ", false, new int[] { 1, 2, 3 }, 10, false, false)]
    [InlineData("1,2,2, 3,1", "15", true, new int[] { 1, 2, 3 }, 15, false, true)]
    [InlineData("-1, 2,16,  -94,-1", "15", true, new int[] { -1, 2, 16, -94 }, 15, false, true)]
    [InlineData("0..0", "5", false, new int[] { 0, 0 }, 5, true, false)]
    [InlineData("-670..12", "50", true, new int[] { -670, 12 }, 50, true, true)]
    [InlineData("-21..-2", "-1", true, new int[] { -21, -2 }, -1, true, true)]
    [InlineData("-10000..-9800", "-1358", false, new int[] { -10000, -9800 }, -1358, true, false)]
    [InlineData("100,50,75", "200", true, new int[] { 100, 50, 75 }, 200, false, true)]
    public void Constructor_DefaultSchema_StringFrom_Success(string? from, string to, bool shouldAvoid, int[] expectedFromVersions, int expectedTo, bool expectedIsRange, bool expectedShouldAvoid)
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
    [InlineData("5", "")]
    [InlineData("5", "   ")]
    [InlineData("", "10")]
    [InlineData("   ", "10")]
    [InlineData("abc", "10")]
    [InlineData("5", "abc")]
    [InlineData("..", "10")]
    [InlineData("..8", "10")]
    [InlineData("8..", "10")]
    [InlineData("1.5", "10")]
    [InlineData("8..10..", "30")]
    [InlineData("8..10..12", "30")]
    [InlineData("..8..10", "30")]
    [InlineData("3...5", "10")]
    [InlineData("5..3", "10")]
    [InlineData("-12..-17", "10")]
    [InlineData("1..-117", "10")]
    [InlineData(",", "10")]
    [InlineData("1,2,-3", "2")]
    [InlineData("5", "5")]
    [InlineData("3..7", "5")]
    [InlineData("10..20", "15")]
    [InlineData("-30..20", "-25")]
    [InlineData("-30..-20", "-21")]
    [InlineData("5..10", "5")]
    [InlineData("-5..10", "-5")]
    [InlineData("5..10", "10")]
    [InlineData("-25..-10", "-10")]
    public void Constructor_DefaultSchema_StringFrom_InvalidFormat_ThrowsException(string from, string to)
    {
        Assert.ThrowsAny<Exception>(() => new MigrationAttribute(from, to, false));
    }

    [Fact]
    public void Constructor_DefaultSchema_StringFrom_NullTo_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new MigrationAttribute("5", null!, false));
    }

    #endregion

    #region Constructor: MigrationAttribute(string? schema, string? from, string to, bool shouldAvoid = false)

    [Theory]
    [InlineData(null, null, "10", false, new int[0], 10, false, false)]
    [InlineData(null, null, "10", true, new int[0], 10, false, true)]
    [InlineData("free", "5", "10", false, new int[] { 5 }, 10, false, false)]
    [InlineData("free", "5", "10", true, new int[] { 5 }, 10, false, true)]
    [InlineData("paid", "1,2,3", "10", false, new int[] { 1, 2, 3 }, 10, false, false)]
    [InlineData("paid", "1,2,3", "10", true, new int[] { 1, 2, 3 }, 10, false, true)]
    [InlineData("enterprise", "5..10", "15", false, new int[] { 5, 10 }, 15, true, false)]
    [InlineData("enterprise", "5..10", "15", true, new int[] { 5, 10 }, 15, true, true)]
    public void Constructor_SpecificSchema_StringFrom_Success(string? schema, string? from, string to, bool shouldAvoid, int[] expectedFromVersions, int expectedTo, bool expectedIsRange, bool expectedShouldAvoid)
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
    [InlineData("", "5", "10", false)]
    [InlineData("   ", "5", "10", false)]
    [InlineData("\t", "5", "10", false)]
    [InlineData("\n", "5", "10", false)]
    public void Constructor_SpecificSchema_StringFrom_InvalidSchema_ThrowsException(string schema, string from, string to, bool shouldAvoid)
    {
        Assert.Throws<ArgumentException>(() => new MigrationAttribute(schema, from, to, shouldAvoid));
    }

    #endregion

    #region Constructor: MigrationAttribute(string? fromSchema, string? from, string? toSchema, string to, bool shouldAvoid = false)

    [Theory]
    [InlineData(null, null, null, "10", false, new int[0], 10, false, false)]
    [InlineData(null, null, null, "10", true, new int[0], 10, false, true)]
    [InlineData("free", "5", "free", "10", false, new int[] { 5 }, 10, false, false)]
    [InlineData("free", "5", "free", "10", true, new int[] { 5 }, 10, false, true)]
    [InlineData("free", "10,15,20", "paid", "1", false, new int[] { 10, 15, 20 }, 1, false, false)]
    [InlineData("free", "10,15,20", "paid", "1", true, new int[] { 10, 15, 20 }, 1, false, true)]
    [InlineData("paid", "5..15", "enterprise", "1", false, new int[] { 5, 15 }, 1, true, false)]
    [InlineData("paid", "5..15", "enterprise", "1", true, new int[] { 5, 15 }, 1, true, true)]
    [InlineData(null, "5", "premium", "1", false, new int[] { 5 }, 1, false, false)]
    [InlineData("basic", "5", null, "10", false, new int[] { 5 }, 10, false, false)]
    public void Constructor_CrossSchema_StringFrom_Success(string? fromSchema, string? from, string? toSchema, string to, bool shouldAvoid, int[] expectedFromVersions, int expectedTo, bool expectedIsRange, bool expectedShouldAvoid)
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
    [InlineData("", "5", "paid", "10", false)]
    [InlineData("   ", "5", "paid", "10", false)]
    [InlineData("free", "5", "", "10", false)]
    [InlineData("free", "5", "   ", "10", false)]
    [InlineData("\t", "5", "paid", "10", false)]
    [InlineData("free", "5", "\n", "10", false)]
    public void Constructor_CrossSchema_StringFrom_InvalidSchema_ThrowsException(string fromSchema, string from, string toSchema, string to, bool shouldAvoid)
    {
        Assert.Throws<ArgumentException>(() => new MigrationAttribute(fromSchema, from, toSchema, to, shouldAvoid));
    }

    #endregion

    #region Edge Cases for String Constructors

    [Fact]
    public void Constructor_String_SingleVersionParsing()
    {
        var attribute = new MigrationAttribute("5", "10");
        Assert.Equal(new int[] { 5 }, attribute.FromVersions);
        Assert.Equal(10, attribute.ToVersion);
        Assert.False(attribute.IsRange);
    }

    [Fact]
    public void Constructor_String_ListVersionParsing()
    {
        var attribute = new MigrationAttribute("1,2,3", "10");
        Assert.Equal(new int[] { 1, 2, 3 }, attribute.FromVersions);
        Assert.Equal(10, attribute.ToVersion);
        Assert.False(attribute.IsRange);
    }

    [Fact]
    public void Constructor_String_RangeVersionParsing()
    {
        var attribute = new MigrationAttribute("5..10", "15");
        Assert.Equal(new int[] { 5, 10 }, attribute.FromVersions);
        Assert.Equal(15, attribute.ToVersion);
        Assert.True(attribute.IsRange);
    }

    [Fact]
    public void Constructor_String_WhitespaceHandling()
    {
        var attribute = new MigrationAttribute(" 5 ", " 10 ");
        Assert.Equal(new int[] { 5 }, attribute.FromVersions);
        Assert.Equal(10, attribute.ToVersion);
    }

    [Fact]
    public void Constructor_String_ListWithWhitespace()
    {
        var attribute = new MigrationAttribute("1, 2, 3", "10");
        Assert.Equal(new int[] { 1, 2, 3 }, attribute.FromVersions);
        Assert.Equal(10, attribute.ToVersion);
    }

    [Fact]
    public void Constructor_String_ListDuplicatesRemoved()
    {
        var attribute = new MigrationAttribute("1,2,2,3,1", "10");
        Assert.Equal(new int[] { 1, 2, 3 }, attribute.FromVersions);
        Assert.Equal(10, attribute.ToVersion);
    }

    [Fact]
    public void Constructor_String_NegativeVersions()
    {
        var attribute = new MigrationAttribute("-5", "-1");
        Assert.Equal(new int[] { -5 }, attribute.FromVersions);
        Assert.Equal(-1, attribute.ToVersion);
    }

    [Fact]
    public void Constructor_String_NegativeList()
    {
        var attribute = new MigrationAttribute("-5,-3,-1", "0");
        Assert.Equal(new int[] { -5, -3, -1 }, attribute.FromVersions);
        Assert.Equal(0, attribute.ToVersion);
    }

    [Fact]
    public void Constructor_String_NegativeRange()
    {
        var attribute = new MigrationAttribute("-10..-5", "0");
        Assert.Equal(new int[] { -10, -5 }, attribute.FromVersions);
        Assert.Equal(0, attribute.ToVersion);
        Assert.True(attribute.IsRange);
    }

    [Fact]
    public void Constructor_String_LargeNumbers()
    {
        var attribute = new MigrationAttribute("1000000", "2000000");
        Assert.Equal(new int[] { 1000000 }, attribute.FromVersions);
        Assert.Equal(2000000, attribute.ToVersion);
    }

    #endregion
}