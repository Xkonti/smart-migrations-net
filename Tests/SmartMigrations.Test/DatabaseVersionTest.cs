namespace SmartMigrations.Test;

using SmartMigrations;
using Xunit;

public class DatabaseVersionTest
{
    [Theory]
    [InlineData("1.2.3.4", 1, 2, 3, 4)]
    [InlineData("0.0.0.0", 0, 0, 0, 0)]
    [InlineData("0.0.0.1", 0, 0, 0, 1)]
    [InlineData("0.3.0.2", 0, 3, 0, 2)]
    [InlineData("0.120.3.45123", 0, 120, 3, 45123)]
    [InlineData("2147483647.2147483646.2147483645.2147483644", 2147483647, 2147483646, 2147483645, 2147483644)]
    public void Parse_WithValidFullVersionString_ReturnsCorrectVersion(
        string versionString, int major, int minor, int patch, int build)
    {
        var version = DatabaseVersion.Parse(versionString);
        Assert.NotNull(version);
        Assert.Equal(major, version.Major);
        Assert.Equal(minor, version.Minor);
        Assert.Equal(patch, version.Patch);
        Assert.Equal(build, version.Build);
        var tryResult = DatabaseVersion.TryParse(versionString, out var tryVersion);
        Assert.True(tryResult);
        Assert.NotNull(tryVersion);
        Assert.Equal(major, tryVersion.Major);
        Assert.Equal(minor, tryVersion.Minor);
        Assert.Equal(patch, tryVersion.Patch);
        Assert.Equal(build, tryVersion.Build);
    }

    [Theory]
    [InlineData("1.2.3", 1, 2, 3, 0)]
    [InlineData("4.5", 4, 5, 0, 0)]
    [InlineData("6", 6, 0, 0, 0)]
    [InlineData("7.0.0", 7, 0, 0, 0)]
    [InlineData("132574.1213546", 132574, 1213546, 0, 0)]
    public void Parse_WithValidPartialVersionString_SetsRestToZero(
        string versionString, int major, int minor, int patch, int build)
    {
        var version = DatabaseVersion.Parse(versionString);
        Assert.NotNull(version);
        Assert.Equal(major, version.Major);
        Assert.Equal(minor, version.Minor);
        Assert.Equal(patch, version.Patch);
        Assert.Equal(build, version.Build);
        var tryResult = DatabaseVersion.TryParse(versionString, out var tryVersion);
        Assert.True(tryResult);
        Assert.NotNull(tryVersion);
        Assert.Equal(major, tryVersion.Major);
        Assert.Equal(minor, tryVersion.Minor);
        Assert.Equal(patch, tryVersion.Patch);
        Assert.Equal(build, tryVersion.Build);
    }

    [Theory]
    [InlineData("9.9.1.x", 9, 9, 1, null)]
    [InlineData("3.1.x.x", 3, 1, null, null)]
    [InlineData("0.x.x.x", 0, null, null, null)]
    [InlineData("10.915.x", 10, 915, null, null)]
    [InlineData("6.x.x", 6, null, null, null)]
    [InlineData("5153.x", 5153, null, null, null)]
    public void Parse_WithValidWildcardVersionString_SetsToNull(
        string versionString, int major, int? minor, int? patch, int? build)
    {
        var version = DatabaseVersion.Parse(versionString);
        Assert.NotNull(version);
        Assert.Equal(major, version.Major);
        Assert.Equal(minor, version.Minor);
        Assert.Equal(patch, version.Patch);
        Assert.Equal(build, version.Build);
        var tryResult = DatabaseVersion.TryParse(versionString, out var tryVersion);
        Assert.True(tryResult);
        Assert.NotNull(tryVersion);
        Assert.Equal(major, tryVersion.Major);
        Assert.Equal(minor, tryVersion.Minor);
        Assert.Equal(patch, tryVersion.Patch);
        Assert.Equal(build, tryVersion.Build);
    }

    [Theory]
    [InlineData("0.1.2.-3")]
    [InlineData("0.1.-2.1")]
    [InlineData("0.-11.2.0")]
    [InlineData("-8.5.2.6")]
    [InlineData("1.1.-6")]
    [InlineData("6.-12.2")]
    [InlineData("-3.500.1")]
    [InlineData("6.-12")]
    [InlineData("-3.500")]
    [InlineData("-3")]
    [InlineData("0.1.-2.x")]
    [InlineData("0.-11.2.x")]
    [InlineData("-8.5.2.x")]
    [InlineData("6.-12.x")]
    [InlineData("-3.500.x")]
    [InlineData("6.-10.x.x")]
    [InlineData("-3.x.x")]
    public void Parse_WithNegativeValues_ThrowsArgumentException(string versionString)
    {
        _ = Assert.Throws<ArgumentException>(() => DatabaseVersion.Parse(versionString));
        var tryResult = DatabaseVersion.TryParse(versionString, out var tryVersion);
        Assert.False(tryResult);
        Assert.Equal(DatabaseVersion.InvalidVersion, tryVersion);
    }

    [Theory]
    [InlineData("1.2.3.4-alpha")]
    [InlineData("1.2.3-beta")]
    [InlineData("2.0.0+build")]
    [InlineData("hello")]
    [InlineData("world.test")]
    [InlineData("1.alpha.3")]
    [InlineData("beta.2.3")]
    [InlineData("1.2.beta")]
    [InlineData("1.2.3.alpha")]
    [InlineData("a.b.c.d")]
    [InlineData("1.2.3.4-")]
    [InlineData("1.2.3-")]
    [InlineData("1.2-")]
    [InlineData("1-")]
    public void Parse_WithInvalidCharacters_ThrowsFormatException(string versionString)
    {
        _ = Assert.Throws<FormatException>(() => DatabaseVersion.Parse(versionString));
        var tryResult = DatabaseVersion.TryParse(versionString, out var tryVersion);
        Assert.False(tryResult);
        Assert.Equal(DatabaseVersion.InvalidVersion, tryVersion);
    }

    [Theory]
    [InlineData("1..5.4")]
    [InlineData("1...5")]
    [InlineData(".1.2.3")]
    [InlineData("1.2.3.")]
    [InlineData("1.2..4")]
    [InlineData("1..3.4")]
    [InlineData("..2.3")]
    [InlineData("1.2..")]
    [InlineData(".")]
    [InlineData("..")]
    [InlineData("...")]
    [InlineData("1.")]
    [InlineData(".2")]
    [InlineData("1.2.")]
    [InlineData(".2.3")]
    public void Parse_WithSkippedSegments_ThrowsFormatException(string versionString)
    {
        _ = Assert.Throws<FormatException>(() => DatabaseVersion.Parse(versionString));
        var tryResult = DatabaseVersion.TryParse(versionString, out var tryVersion);
        Assert.False(tryResult);
        Assert.Equal(DatabaseVersion.InvalidVersion, tryVersion);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("\r\n")]
    [InlineData("   \t  ")]
    public void Parse_WithEmptyOrWhitespaceString_ThrowsArgumentException(string versionString)
    {
        _ = Assert.Throws<ArgumentException>(() => DatabaseVersion.Parse(versionString));
        var tryResult = DatabaseVersion.TryParse(versionString, out var tryVersion);
        Assert.False(tryResult);
        Assert.Equal(DatabaseVersion.InvalidVersion, tryVersion);
    }

    [Theory]
    [InlineData("1.2.3.4.5")]
    [InlineData("1.2.3.4.5.6")]
    [InlineData("0.1.2.3.4")]
    [InlineData("2.4.61.24.x")]
    [InlineData("1.2.3.4.x")]
    [InlineData("1.x.x.x.x")]
    [InlineData("1.2.3.4.5.6.7.8.9")]
    [InlineData("5.4.3.2.1.0")]
    [InlineData("....")]
    public void Parse_WithMoreThanFourSegments_ThrowsArgumentException(string versionString)
    {
        _ = Assert.Throws<ArgumentException>(() => DatabaseVersion.Parse(versionString));
        var tryResult = DatabaseVersion.TryParse(versionString, out var tryVersion);
        Assert.False(tryResult);
        Assert.Equal(DatabaseVersion.InvalidVersion, tryVersion);
    }

    [Theory]
    [InlineData("1.2.3.4", "1.2.3.4", true)]
    [InlineData("1.2.3.0", "1.2.3.0", true)]
    [InlineData("5.0.0.0", "5.0.0.0", true)]
    [InlineData("1.2.3.4", "1.2.3.5", false)]
    [InlineData("1.2.3.4", "1.2.4.4", false)]
    [InlineData("1.2.3.4", "1.3.3.4", false)]
    [InlineData("1.2.3.4", "2.2.3.4", false)]
    public void Equals_WithNoWildcards_ComparesAllSegments(string version1, string version2, bool expected)
    {
        var v1 = DatabaseVersion.Parse(version1);
        var v2 = DatabaseVersion.Parse(version2);

        Assert.Equal(expected, v1.Equals(v2));
        Assert.Equal(expected, v2.Equals(v1));
        Assert.Equal(expected, v1 == v2);
        Assert.Equal(!expected, v1 != v2);
    }

    [Theory]
    [InlineData("1.3.x.x", "1.3.5.123", true)]
    [InlineData("1.3.5.123", "1.3.x.x", true)]
    [InlineData("2.x.x.x", "2.0.0.0", true)]
    [InlineData("2.x.x.x", "2.999.888.777", true)]
    [InlineData("3.5.x", "3.5.0", true)]
    [InlineData("3.5.x", "3.5.999", true)]
    [InlineData("4.2.1.x", "4.2.1.0", true)]
    [InlineData("4.2.1.x", "4.2.1.9999", true)]
    public void Equals_WithWildcards_MatchesAnyValue(string version1, string version2, bool expected)
    {
        var v1 = DatabaseVersion.Parse(version1);
        var v2 = DatabaseVersion.Parse(version2);

        Assert.Equal(expected, v1.Equals(v2));
        Assert.Equal(expected, v2.Equals(v1));
        Assert.Equal(expected, v1 == v2);
        Assert.Equal(!expected, v1 != v2);
    }

    [Theory]
    [InlineData("1.x.x.x", "2.x.x.x", false)]
    [InlineData("1.x.x.x", "2.0.0.0", false)]
    [InlineData("3.5.x", "3.6.x", false)]
    [InlineData("3.5.x", "3.6.0", false)]
    [InlineData("4.2.1.x", "4.2.2.x", false)]
    [InlineData("4.2.1.x", "4.2.2.0", false)]
    [InlineData("5.3.x.x", "5.4.x.x", false)]
    [InlineData("5.3.x.x", "5.4.0.0", false)]
    public void Equals_WithWildcards_FailsWhenNonWildcardsDiffer(string version1, string version2, bool expected)
    {
        var v1 = DatabaseVersion.Parse(version1);
        var v2 = DatabaseVersion.Parse(version2);

        Assert.Equal(expected, v1.Equals(v2));
        Assert.Equal(expected, v2.Equals(v1));
        Assert.Equal(expected, v1 == v2);
        Assert.Equal(!expected, v1 != v2);
    }

    [Theory]
    [InlineData("1.x.x.x", "1.x.x.x", true)]
    [InlineData("2.3.x.x", "2.3.x.x", true)]
    [InlineData("4.5.6.x", "4.5.6.x", true)]
    [InlineData("1.x.x.x", "1.3.x.x", true)]
    [InlineData("1.3.x.x", "1.x.x.x", true)]
    [InlineData("5.x.x.x", "5.2.x.x", true)]
    [InlineData("5.2.x.x", "5.x.x.x", true)]
    [InlineData("7.x.x.x", "7.8.9.x", true)]
    [InlineData("7.8.9.x", "7.x.x.x", true)]
    public void Equals_WithBothWildcards_MatchesCorrectly(string version1, string version2, bool expected)
    {
        var v1 = DatabaseVersion.Parse(version1);
        var v2 = DatabaseVersion.Parse(version2);

        Assert.Equal(expected, v1.Equals(v2));
        Assert.Equal(expected, v2.Equals(v1));
        Assert.Equal(expected, v1 == v2);
        Assert.Equal(!expected, v1 != v2);
    }

    [Fact]
    public void Equals_WithNullParameter_ReturnsFalse()
    {
        var version = DatabaseVersion.Parse("1.2.3.4");
        Assert.False(version.Equals(null));
    }

    [Fact]
    public void Equals_WithSameReference_ReturnsTrue()
    {
        var version = DatabaseVersion.Parse("1.2.3.4");
        Assert.True(version.Equals(version));
    }

    [Fact]
    public void GetHashCode_GeneratesUniqueHashForDifferentVersions()
    {
        var v1 = DatabaseVersion.Parse("1.2.3.4");
        var v2 = DatabaseVersion.Parse("1.5.6.7");
        var v3 = DatabaseVersion.Parse("1.x.x.x");
        var v4 = DatabaseVersion.Parse("2.2.3.4");

        // Different versions should have different hash codes
        Assert.NotEqual(v1.GetHashCode(), v2.GetHashCode());
        Assert.NotEqual(v1.GetHashCode(), v3.GetHashCode());
        Assert.NotEqual(v1.GetHashCode(), v4.GetHashCode());

        // Same version should have same hash code
        var v1Copy = DatabaseVersion.Parse("1.2.3.4");
        Assert.Equal(v1.GetHashCode(), v1Copy.GetHashCode());
    }

    [Theory]
    [InlineData("1.2.3.4", "1.2.3.5", -1)]
    [InlineData("1.2.3.5", "1.2.3.4", 1)]
    [InlineData("1.2.3.4", "1.2.3.4", 0)]
    [InlineData("1.2.3.4", "1.2.4.4", -1)]
    [InlineData("1.2.4.4", "1.2.3.4", 1)]
    [InlineData("1.2.3.4", "1.3.3.4", -1)]
    [InlineData("1.3.3.4", "1.2.3.4", 1)]
    [InlineData("1.2.3.4", "2.2.3.4", -1)]
    [InlineData("2.2.3.4", "1.2.3.4", 1)]
    [InlineData("10.5.2.1", "10.5.2.100", -1)]
    [InlineData("10.5.2.100", "10.5.2.1", 1)]
    public void CompareTo_WithNoWildcards_ComparesCorrectly(string version1, string version2, int expectedSign)
    {
        var v1 = DatabaseVersion.Parse(version1);
        var v2 = DatabaseVersion.Parse(version2);

        var result = v1.CompareTo(v2);
        Assert.Equal(expectedSign, Math.Sign(result));

        // Verify inverse comparison
        var inverseResult = v2.CompareTo(v1);
        Assert.Equal(-expectedSign, Math.Sign(inverseResult));
    }

    [Theory]
    [InlineData("1.2.x.x", "1.2.3.4", 0)]
    [InlineData("1.2.3.4", "1.2.x.x", 0)]
    [InlineData("1.x.x.x", "1.5.6.7", 0)]
    [InlineData("1.5.6.7", "1.x.x.x", 0)]
    [InlineData("2.3.4.x", "2.3.4.100", 0)]
    [InlineData("2.3.4.100", "2.3.4.x", 0)]
    [InlineData("3.5.x", "3.5.99", 0)]
    [InlineData("3.5.99", "3.5.x", 0)]
    public void CompareTo_WithWildcards_TreatsAsEqual(string version1, string version2, int expectedSign)
    {
        var v1 = DatabaseVersion.Parse(version1);
        var v2 = DatabaseVersion.Parse(version2);

        var result = v1.CompareTo(v2);
        Assert.Equal(expectedSign, Math.Sign(result));

        // Verify inverse comparison
        var inverseResult = v2.CompareTo(v1);
        Assert.Equal(expectedSign, Math.Sign(inverseResult));
    }

    [Theory]
    [InlineData("1.x.x.x", "2.x.x.x", -1)]
    [InlineData("2.x.x.x", "1.x.x.x", 1)]
    [InlineData("1.2.x.x", "1.3.x.x", -1)]
    [InlineData("1.3.x.x", "1.2.x.x", 1)]
    [InlineData("5.6.7.x", "5.6.8.x", -1)]
    [InlineData("5.6.8.x", "5.6.7.x", 1)]
    [InlineData("3.x.x.x", "3.5.x.x", 0)]  // Both have wildcard at minor, so equal
    [InlineData("3.5.x.x", "3.x.x.x", 0)]  // Both have wildcard (one at minor), so equal
    public void CompareTo_WithPartialWildcards_ComparesNonWildcardParts(string version1, string version2, int expectedSign)
    {
        var v1 = DatabaseVersion.Parse(version1);
        var v2 = DatabaseVersion.Parse(version2);

        var result = v1.CompareTo(v2);
        Assert.Equal(expectedSign, Math.Sign(result));

        // Verify inverse comparison
        var inverseResult = v2.CompareTo(v1);
        Assert.Equal(-expectedSign, Math.Sign(inverseResult));
    }

    [Theory]
    [InlineData("1.2", "1.x", 0)]  // 1.2.0.0 vs 1.x.x.x - wildcard at minor
    [InlineData("1.x", "1.2", 0)]  // 1.x.x.x vs 1.2.0.0 - wildcard at minor
    [InlineData("1.2.3", "1.2.x", 0)]  // 1.2.3.0 vs 1.2.x.x - wildcard at patch
    [InlineData("1.2.x", "1.2.3", 0)]  // 1.2.x.x vs 1.2.3.0 - wildcard at patch
    public void CompareTo_WithMixedFormats_HandlesCorrectly(string version1, string version2, int expectedSign)
    {
        var v1 = DatabaseVersion.Parse(version1);
        var v2 = DatabaseVersion.Parse(version2);

        var result = v1.CompareTo(v2);
        Assert.Equal(expectedSign, Math.Sign(result));

        // Verify inverse comparison
        var inverseResult = v2.CompareTo(v1);
        Assert.Equal(-expectedSign, Math.Sign(inverseResult));
    }

    [Fact]
    public void CompareTo_WithNull_ThrowsArgumentNullException()
    {
        var version = DatabaseVersion.Parse("1.2.3.4");
        Assert.Throws<ArgumentNullException>(() => version.CompareTo(null));
    }

    [Fact]
    public void CompareTo_WithSameReference_ReturnsZero()
    {
        var version = DatabaseVersion.Parse("1.2.3.4");
        Assert.Equal(0, version.CompareTo(version));
    }

    [Theory]
    [InlineData("1.2.3.4", "1.2.3.4")]
    [InlineData("5.x.x.x", "5.x.x.x")]
    [InlineData("3.4.x", "3.4.x")]
    [InlineData("2.1.5.x", "2.1.5.x")]
    public void CompareTo_WithEqualVersions_ReturnsZero(string version1, string version2)
    {
        var v1 = DatabaseVersion.Parse(version1);
        var v2 = DatabaseVersion.Parse(version2);

        Assert.Equal(0, v1.CompareTo(v2));
        Assert.Equal(0, v2.CompareTo(v1));
    }

    [Theory]
    [InlineData(new[] { "2.1.0.0", "1.5.0.0", "1.2.0.0", "2.0.0.0", "1.2.1.0", "1.2.0.1" }, new[] { "1.2.0.0", "1.2.0.1", "1.2.1.0", "1.5.0.0", "2.0.0.0", "2.1.0.0" })]
    [InlineData(new[] { "3.0.0.0", "1.0.0.0", "2.0.0.0" }, new[] { "1.0.0.0", "2.0.0.0", "3.0.0.0" })]
    [InlineData(new[] { "1.3.0.0", "1.1.0.0", "1.2.0.0" }, new[] { "1.1.0.0", "1.2.0.0", "1.3.0.0" })]
    public void CompareTo_ImplementsIComparable(string[] input, string[] expectedSorted)
    {
        var versions = input.Select(DatabaseVersion.Parse).ToList();
        versions.Sort();

        for (int i = 0; i < expectedSorted.Length; i++)
        {
            var expected = DatabaseVersion.Parse(expectedSorted[i]);
            Assert.Equal(expected.Major, versions[i].Major);
            Assert.Equal(expected.Minor, versions[i].Minor);
            Assert.Equal(expected.Patch, versions[i].Patch);
            Assert.Equal(expected.Build, versions[i].Build);
        }
    }
}
