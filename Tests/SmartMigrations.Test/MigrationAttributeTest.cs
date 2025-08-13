namespace SmartMigrations.Test;

using SmartMigrations;
using Xunit;

public class MigrationAttributeTest
{
    #region Constructor: MigrationAttribute(int to, bool shouldAvoid)

    [Theory]
    [InlineData(0, false, new int[0], 0, false, false)]
    [InlineData(0, true, new int[0], 0, false, true)]
    [InlineData(1, false, new int[0], 1, false, false)]
    [InlineData(1, true, new int[0], 1, false, true)]
    [InlineData(30, false, new int[0], 30, false, false)]
    [InlineData(30, true, new int[0], 30, false, true)]
    [InlineData(int.MaxValue, false, new int[0], int.MaxValue, false, false)]
    [InlineData(int.MaxValue, true, new int[0], int.MaxValue, false, true)]
    public void Constructor_ToOnly_Success(int to, bool shouldAvoid, int[] expectedFromVersions, int expectedTo, bool expectedIsRange, bool expectedShouldAvoid)
    {
        var attribute = new MigrationAttribute(to, shouldAvoid);
        Assert.Equal(expectedFromVersions, attribute.FromVersions);
        Assert.Equal(expectedTo, attribute.ToVersion);
        Assert.Equal(expectedIsRange, attribute.IsRange);
        Assert.Equal(expectedShouldAvoid, attribute.ShouldAvoid);
    }

    #endregion

    #region Constructor: MigrationAttribute(int from, int to, bool shouldAvoid)

    [Theory]
    [InlineData(0, 1, false, new int[] { 0 }, 1, false, false)]
    [InlineData(0, 1, true, new int[] { 0 }, 1, false, true)]
    [InlineData(8, 31, false, new int[] { 8 }, 31, false, false)]
    [InlineData(8, 31, true, new int[] { 8 }, 31, false, true)]
    [InlineData(168, 50, false, new int[] { 168 }, 50, false, false)]
    [InlineData(168, 50, true, new int[] { 168 }, 50, false, true)]
    [InlineData(0, int.MaxValue, false, new int[] { 0 }, int.MaxValue, false, false)]
    [InlineData(0, int.MaxValue, true, new int[] { 0 }, int.MaxValue, false, true)]
    public void Constructor_SingleFrom_Success(int from, int to, bool shouldAvoid, int[] expectedFromVersions, int expectedTo, bool expectedIsRange, bool expectedShouldAvoid)
    {
        var attribute = new MigrationAttribute(from, to, shouldAvoid);
        Assert.Equal(expectedFromVersions, attribute.FromVersions);
        Assert.Equal(expectedTo, attribute.ToVersion);
        Assert.Equal(expectedIsRange, attribute.IsRange);
        Assert.Equal(expectedShouldAvoid, attribute.ShouldAvoid);
    }

    [Theory]
    [InlineData(0, 0, false)]
    [InlineData(5, 5, true)]
    [InlineData(100, 100, false)]
    [InlineData(int.MaxValue, int.MaxValue, true)]
    public void Constructor_SingleFrom_ThrowsArgumentException(int from, int to, bool shouldAvoid)
    {
        Assert.Throws<ArgumentException>(() => new MigrationAttribute(from, to, shouldAvoid));
    }

    #endregion

    #region Constructor: MigrationAttribute(IEnumerable<int> fromList, int to, bool shouldAvoid)

    [Theory]
    [InlineData(new int[] { 1 }, 5, false, new int[] { 1 }, 5, false, false)]
    [InlineData(new int[] { 1, 3, 5 }, 10, true, new int[] { 1, 3, 5 }, 10, false, true)]
    [InlineData(new int[] { 0, 1, 2 }, 3, false, new int[] { 0, 1, 2 }, 3, false, false)]
    [InlineData(new int[] { 10, 20, 30, 40 }, 50, true, new int[] { 10, 20, 30, 40 }, 50, false, true)]
    [InlineData(new int[] { 100 }, 200, false, new int[] { 100 }, 200, false, false)]
    [InlineData(new int[] { 5, 1, 3 }, 10, false, new int[] { 5, 1, 3 }, 10, false, false)] // Preserves order, removes duplicates
    [InlineData(new int[] { 5, 1, 3, 1, 5 }, 10, true, new int[] { 5, 1, 3 }, 10, false, true)] // Duplicates removed, order preserved
    public void Constructor_FromList_Success(int[] fromList, int to, bool shouldAvoid, int[] expectedFromVersions, int expectedTo, bool expectedIsRange, bool expectedShouldAvoid)
    {
        var attribute = new MigrationAttribute(fromList, to, shouldAvoid);
        Assert.Equal(expectedFromVersions, attribute.FromVersions);
        Assert.Equal(expectedTo, attribute.ToVersion);
        Assert.Equal(expectedIsRange, attribute.IsRange);
        Assert.Equal(expectedShouldAvoid, attribute.ShouldAvoid);
    }

    [Theory]
    [InlineData(new int[] { 1, 5 }, 5, false)] // Contains to version
    [InlineData(new int[] { 10, 20, 30 }, 20, true)] // Contains to version in middle
    [InlineData(new int[] { 5 }, 5, false)] // Single item equals to
    public void Constructor_FromList_WithToVersionInList_ThrowsArgumentException(int[] fromList, int to, bool shouldAvoid)
    {
        Assert.Throws<ArgumentException>(() => new MigrationAttribute(fromList, to, shouldAvoid));
    }

    [Fact]
    public void Constructor_FromList_WithNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new MigrationAttribute(null!, 5));
    }

    #endregion

    #region Constructor: MigrationAttribute((int start, int end) fromRange, int to, bool shouldAvoid)

    [Theory]
    [InlineData(1, 3, 10, false, new int[] { 1, 3 }, 10, true, false)] // Note: Range stores [start, end] not full sequence
    [InlineData(5, 5, 8, true, new int[] { 5, 5 }, 8, true, true)] // Single value range
    [InlineData(0, 2, 5, false, new int[] { 0, 2 }, 5, true, false)]
    [InlineData(10, 15, 20, true, new int[] { 10, 15 }, 20, true, true)]
    [InlineData(0, 0, 1, false, new int[] { 0, 0 }, 1, true, false)]
    [InlineData(100, 200, 300, false, new int[] { 100, 200 }, 300, true, false)]
    public void Constructor_Range_Success(int start, int end, int to, bool shouldAvoid, int[] expectedFromVersions, int expectedTo, bool expectedIsRange, bool expectedShouldAvoid)
    {
        var attribute = new MigrationAttribute((start, end), to, shouldAvoid);
        Assert.Equal(expectedFromVersions, attribute.FromVersions);
        Assert.Equal(expectedTo, attribute.ToVersion);
        Assert.Equal(expectedIsRange, attribute.IsRange);
        Assert.Equal(expectedShouldAvoid, attribute.ShouldAvoid);
    }

    [Theory]
    [InlineData(5, 3, 10, false)]
    [InlineData(10, 5, 15, true)]
    [InlineData(100, 50, 200, false)]
    public void Constructor_Range_WithInvalidRange_ThrowsArgumentException(int start, int end, int to, bool shouldAvoid)
    {
        Assert.Throws<ArgumentException>(() => new MigrationAttribute((start, end), to, shouldAvoid));
    }

    [Theory]
    [InlineData(5, 10, 7, false)]
    [InlineData(1, 5, 3, true)]
    [InlineData(10, 20, 15, false)]
    [InlineData(5, 10, 5, true)]
    [InlineData(5, 10, 10, false)]
    public void Constructor_Range_WithToVersionInRange_ThrowsArgumentException(int start, int end, int to, bool shouldAvoid)
    {
        Assert.Throws<ArgumentException>(() => new MigrationAttribute((start, end), to, shouldAvoid));
    }

    #endregion

    #region Constructor: MigrationAttribute(string? from, string to, bool shouldAvoid)

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
    public void Constructor_String_Success(string? from, string to, bool shouldAvoid, int[] expectedFromVersions, int expectedTo, bool expectedIsRange, bool expectedShouldAvoid)
    {
        var attribute = new MigrationAttribute(from, to, shouldAvoid);
        Assert.Equal(expectedFromVersions, attribute.FromVersions);
        Assert.Equal(expectedTo, attribute.ToVersion);
        Assert.Equal(expectedIsRange, attribute.IsRange);
        Assert.Equal(expectedShouldAvoid, attribute.ShouldAvoid);
    }

    [Theory]
    [InlineData("5", null)] // Null to
    [InlineData("5", "")] // Empty to
    [InlineData("5", "   ")] // Whitespace to
    [InlineData("", "10")] // Empty from
    [InlineData("   ", "10")] // Whitespace from
    [InlineData("abc", "10")] // Invalid from format
    [InlineData("5", "abc")] // Invalid to format
    [InlineData("..", "10")] // Invalid range format
    [InlineData("..8", "10")] // Invalid range format
    [InlineData("8..", "10")] // Invalid range format
    [InlineData("1.5", "10")] // Invalid range format
    [InlineData("8..10..", "30")] // Invalid range format
    [InlineData("8..10..12", "30")] // Invalid range format
    [InlineData("..8..10", "30")] // Invalid range format
    [InlineData("3...5", "10")] // Invalid range format
    [InlineData("5..3", "10")] // Reversed range
    [InlineData("-12..-17", "10")] // Reversed range
    [InlineData("1..-117", "10")] // Reversed range
    [InlineData(",", "10")] // Empty comma list
    [InlineData("1,2,-3", "2")] // From list contains to version
    [InlineData("5", "5")] // Single from equals to
    [InlineData("3..7", "5")] // To within range
    [InlineData("10..20", "15")] // To within range
    [InlineData("-30..20", "-25")] // To within range
    [InlineData("-30..-20", "-21")] // To within range
    [InlineData("5..10", "5")] // To equals range start
    [InlineData("-5..10", "-5")] // To equals range start
    [InlineData("5..10", "10")] // To equals range end
    [InlineData("-25..-10", "-10")] // To equals range end
    public void Constructor_String_ThrowsException(string? from, string to)
    {
        Assert.ThrowsAny<Exception>(() => new MigrationAttribute(from, to, false));
    }

    #endregion
}
