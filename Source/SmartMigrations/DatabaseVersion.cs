using System.Globalization;

namespace SmartMigrations;


/// <summary>
/// Represents a database version with major, minor, patch, and optional build numbers.
/// Supports formats: "1.2.3" or "1.2.3.4" where all parts are non-negative integers.
/// </summary>
public sealed record DatabaseVersion(int Major, int? Minor, int? Patch, int? Build) : IComparable<DatabaseVersion>
{
    /// <summary>
    /// Represents an invalid database version. Used as a return value for failed TryParse operations.
    /// </summary>
    public static readonly DatabaseVersion InvalidVersion = new(-1, null, null, null);

    /// <summary>
    /// Parses a version string into a DatabaseVersion object.
    /// </summary>
    /// <param name="version">The version string to parse. Supports formats like "1.2.3.4", "1.2.3", "1.2", "1", and wildcards "1.x.x.x".</param>
    /// <returns>A DatabaseVersion object representing the parsed version.</returns>
    /// <exception cref="ArgumentException">Thrown when the version string is invalid or contains invalid values.</exception>
    /// <exception cref="FormatException">Thrown when the version string contains non-numeric values (except 'x' for wildcards).</exception>
    public static DatabaseVersion Parse(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException($"'{nameof(version)}' cannot be null or whitespace.", nameof(version));

        var parts = version.Split('.');
        if (parts.Length > 4)
            throw new ArgumentException($"'{nameof(version)}' cannot have more than 4 version segments.", nameof(version));

        // Only the major version must be present
        // version '2' which translates to '2.0.0.0')
        var major = int.Parse(parts[0], CultureInfo.InvariantCulture);

        // Minor version is optional
        // version '1.3' = '1.3.0.0'
        // version '3.x' = '3.x.x.x' (x = any)
        int? minor = parts.Length >= 2
            ? parts[1].Equals("x", StringComparison.OrdinalIgnoreCase)
                ? null
                : int.Parse(parts[1], CultureInfo.InvariantCulture)
            : 0;

        // Patch is optional, can't be present if `minor` is not present
        // version '2.0.2' = '2.0.2.0'
        // version '10.1.x' = '10.1.x.x' (x = any)
        // version '3.x.3' = ERROR
        int? patch;
        if (parts.Length >= 3)
        {
            if (parts[2].Equals("x", StringComparison.OrdinalIgnoreCase))
            {
                patch = null;
            }
            else
            {
                patch = int.Parse(parts[2], CultureInfo.InvariantCulture);
            }
        }
        else
        {
            // If minor is a wildcard, patch should also be null (wildcard)
            // Otherwise, patch defaults to 0
            patch = minor is null ? null : 0;
        }

        if (minor is null && patch is not null)
            throw new ArgumentException("Patch segment can't be set when preceding minor segment is a wildcard", nameof(version));

        // Build is optional, can't be present if `patch` is not present
        // version '18.0.4.201' = '18.0.4.201'
        // version '3.291.5.x' = '3.291.5.x' (x = any)
        // version '2.0.x.100' = ERROR
        // version '1.x.x.2' = ERROR
        int? build;
        if (parts.Length == 4)
        {
            if (parts[3].Equals("x", StringComparison.OrdinalIgnoreCase))
            {
                build = null;
            }
            else
            {
                build = int.Parse(parts[3], CultureInfo.InvariantCulture);
            }
        }
        else
        {
            // If patch is a wildcard, build should also be null (wildcard)
            // Otherwise, build defaults to 0
            build = patch is null ? null : 0;
        }

        if (patch is null && build is not null)
            throw new ArgumentException("Build segment can't be set when preceding patch segment is a wildcard", nameof(version));

        // Ensure all versions are >=0
        if (major < 0 || minor < 0 || patch < 0 || build < 0)
            throw new ArgumentException("Version can't contain negative values", nameof(version));

        return new DatabaseVersion(major, minor, patch, build);
    }

    /// <summary>
    /// Attempts to parse a version string into a DatabaseVersion object.
    /// </summary>
    /// <param name="version">The version string to parse.</param>
    /// <param name="result">When this method returns, contains the parsed DatabaseVersion if successful, or InvalidVersion if parsing failed.</param>
    /// <returns>true if the version was successfully parsed; otherwise, false.</returns>
    public static bool TryParse(string version, out DatabaseVersion result)
    {
        try
        {
            result = Parse(version);
            return true;
        }
        catch
        {
            result = InvalidVersion;
            return false;
        }
    }

    /// <summary>
    /// Determines whether this DatabaseVersion is equal to another DatabaseVersion.
    /// Null segments (wildcards) are considered to match any value.
    /// </summary>
    /// <param name="other">The DatabaseVersion to compare with this instance.</param>
    /// <returns>true if the versions are equal (considering wildcards); otherwise, false.</returns>
    public bool Equals(DatabaseVersion? other)
    {
        if (other is null) return false;

        // Major must match exactly (no wildcards allowed for Major)
        if (Major != other.Major) return false;

        // Minor: null (wildcard) matches any value
        if (Minor != null && other.Minor != null && Minor != other.Minor) return false;

        // Patch: null (wildcard) matches any value
        if (Patch != null && other.Patch != null && Patch != other.Patch) return false;

        // Build: null (wildcard) matches any value
        return Build == null || other.Build == null || Build == other.Build;
    }

    /// <summary>
    /// Returns the hash code for this DatabaseVersion.
    /// </summary>
    /// <returns>A hash code that uniquely identifies this version including wildcards.</returns>
    public override int GetHashCode() => (Major, Minor, Patch, Build).GetHashCode();

    /// <summary>
    /// Compares this DatabaseVersion with another DatabaseVersion for ordering.
    /// Null segments (wildcards) are considered equal to any value in that segment.
    /// </summary>
    /// <param name="other">The DatabaseVersion to compare with this instance.</param>
    /// <returns>A value less than zero if this instance precedes other, zero if they are equal, or greater than zero if this instance follows other.</returns>
    public int CompareTo(DatabaseVersion? other)
    {
        if (other is null) throw new ArgumentNullException(nameof(other));

        // Compare Major (required, no wildcards)
        var majorComparison = Major.CompareTo(other.Major);
        if (majorComparison != 0) return majorComparison;

        // Compare Minor - null (wildcard) is considered equal to any value
        if (Minor != null && other.Minor != null)
        {
            var minorComparison = Minor.Value.CompareTo(other.Minor.Value);
            if (minorComparison != 0) return minorComparison;
        }
        // If either is null (wildcard), consider them equal and continue

        // Compare Patch - null (wildcard) is considered equal to any value
        if (Patch != null && other.Patch != null)
        {
            var patchComparison = Patch.Value.CompareTo(other.Patch.Value);
            if (patchComparison != 0) return patchComparison;
        }
        // If either is null (wildcard), consider them equal and continue

        // Compare Build - null (wildcard) is considered equal to any value
        if (Build != null && other.Build != null)
        {
            return Build.Value.CompareTo(other.Build.Value);
        }
        // If either is null (wildcard), consider them equal
        return 0;
    }
}
