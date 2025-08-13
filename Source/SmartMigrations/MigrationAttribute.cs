namespace SmartMigrations;

/// <summary>
/// Marks a class as a database migration with version information and execution preferences.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class MigrationAttribute : Attribute
{
    /// <summary>
    /// Gets the list of versions this migration can migrate from.
    /// </summary>
    public List<int> FromVersions { get; }

    /// <summary>
    /// Gets a value whether the list of versions represents a from-to range.
    /// </summary>
    public bool IsRange { get; }

    /// <summary>
    /// Gets the version this migration migrates to.
    /// </summary>
    public int ToVersion { get; }

    /// <summary>
    /// Gets a value indicating whether this migration should be avoided if alternative paths exist.
    /// Migrations marked as should avoid are used only when no other path is available.
    /// </summary>
    public bool ShouldAvoid { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MigrationAttribute"/> class for setting up the database schema from scratch.
    /// This constructor creates a migration that can be applied when no version exists in the database (initial setup migration).
    /// </summary>
    /// <param name="to">The version this migration migrates to.</param>
    /// <param name="shouldAvoid">Whether this migration should be avoided if alternatives exist. Default is false.</param>
    public MigrationAttribute(int to, bool shouldAvoid = false)
    {
        FromVersions = [];
        IsRange = false;
        ToVersion = to;
        ShouldAvoid = shouldAvoid;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MigrationAttribute"/> class with a single source version.
    /// </summary>
    /// <param name="from">The version this migration can migrate from.</param>
    /// <param name="to">The version this migration migrates to.</param>
    /// <param name="shouldAvoid">Whether this migration should be avoided if alternatives exist. Default is false.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="from"/> and <paramref name="to"/> are the same value.</exception>
    public MigrationAttribute(int from, int to, bool shouldAvoid = false)
    {
        if (from == to) throw new ArgumentException("From version cannot be the same as to version.", nameof(from));
        FromVersions = [from];
        IsRange = false;
        ToVersion = to;
        ShouldAvoid = shouldAvoid;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MigrationAttribute"/> class with multiple source versions.
    /// </summary>
    /// <param name="fromList">The array of versions this migration can migrate from. Cannot be null.</param>
    /// <param name="to">The version this migration migrates to.</param>
    /// <param name="shouldAvoid">Whether this migration should be avoided if alternatives exist. Default is false.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="fromList"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="fromList"/> contains the same version as <paramref name="to"/>.</exception>
    public MigrationAttribute(int[] fromList, int to, bool shouldAvoid = false)
    {
        ArgumentNullException.ThrowIfNull(fromList);
        FromVersions = fromList
            .Distinct()
            .ToList();

        if (FromVersions.Contains(to)) throw new ArgumentException("From version list cannot contain the same version as the to version.", nameof(fromList));
        IsRange = false;
        ToVersion = to;
        ShouldAvoid = shouldAvoid;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MigrationAttribute"/> class with a range of source versions.
    /// </summary>
    /// <param name="fromRangeStart">The start of the inclusive range of versions this migration can migrate from.</param>
    /// <param name="fromRangeEnd">The end of the inclusive range of versions this migration can migrate from.</param>
    /// <param name="to">The version this migration migrates to.</param>
    /// <param name="shouldAvoid">Whether this migration should be avoided if alternatives exist. Default is false.</param>
    /// <exception cref="ArgumentException">Thrown when range start is greater than the range end, or when <paramref name="to"/> is within the range.</exception>
    public MigrationAttribute(int fromRangeStart, int fromRangeEnd, int to, bool shouldAvoid = false)
    {
        if (fromRangeStart > fromRangeEnd)
            throw new ArgumentException("Range start cannot be greater than range end.", nameof(fromRangeStart));
        if (fromRangeStart <= to && to <= fromRangeEnd)
            throw new ArgumentException("To version cannot be within the from version range.", nameof(to));
        FromVersions = [fromRangeStart, fromRangeEnd];
        IsRange = true;
        ToVersion = to;
        ShouldAvoid = shouldAvoid;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MigrationAttribute"/> class with string-based version specification.
    /// Supports single version ("12"), comma-separated list ("3, 6, 12, 7"), or range ("5..10").
    /// </summary>
    /// <param name="from">The version specification this migration can migrate from. Can be a single version, comma-separated list, or range. Use null to indicate this migration can be applied from any version.</param>
    /// <param name="to">The version this migration migrates to as a string.</param>
    /// <param name="shouldAvoid">Whether this migration should be avoided if alternatives exist. Default is false.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="to"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="from"/> or <paramref name="to"/> contains an invalid version format, is empty/whitespace, when from and to versions are the same, when to version is within a from range, or when a from list contains the to version.</exception>
    public MigrationAttribute(string? from, string to, bool shouldAvoid = false)
    {
        ArgumentNullException.ThrowIfNull(to);

        if (string.IsNullOrWhiteSpace(to))
            throw new ArgumentException("To version cannot be empty or whitespace.", nameof(to));

        if (!int.TryParse(to.Trim(), out var toVersion))
            throw new ArgumentException("To version must be an integer.", nameof(to));

        ToVersion = toVersion;
        ShouldAvoid = shouldAvoid;
        IsRange = false;

        // Handle null "from"
        if (from == null)
        {
            FromVersions = [];
            return;
        }

        from = from.Trim();
        if (string.IsNullOrWhiteSpace(from))
            throw new ArgumentException("From version cannot be empty or whitespace.", nameof(from));

        // Check if it's a range (contains dash)
        if (from.Contains(".."))
        {
            var parts = from.Split("..");
            if (parts.Length != 2)
                // When input is ".." or "1..5..10"
                throw new ArgumentException("Invalid range format. Expected 'start-end'.", nameof(from));

            if (!int.TryParse(parts[0].Trim(), out var start))
                throw new ArgumentException("Range start must be an integer.", nameof(from));
            if (!int.TryParse(parts[1].Trim(), out var end))
                throw new ArgumentException("Range end must be an integer.", nameof(from));
            if (start > end)
                throw new ArgumentException("Range start cannot be greater than range end.", nameof(from));
            if (start <= ToVersion && ToVersion <= end)
                throw new ArgumentException("To version cannot be within the from version range.", nameof(from));

            FromVersions = [start, end];
            IsRange = true;
        }
        // Check if it's a comma-separated list
        else if (from.Contains(','))
        {
            var parts = from.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                // When input is ","
                throw new ArgumentException("Version list cannot be empty.", nameof(from));

            FromVersions = parts
                .Select(p => p.Trim())
                .Select(part =>
                {
                    if (!int.TryParse(part, out var version))
                        throw new ArgumentException("All versions in the list must be an integers.",
                            nameof(from));
                    return version;
                })
                .Distinct()
                .ToList();

            if (FromVersions.Contains(ToVersion))
                throw new ArgumentException("From version list cannot contain the same version as the to version.", nameof(from));
        }
        // Single version
        else
        {
            if (!int.TryParse(from, out var version))
                throw new ArgumentException("From version must be an integer.", nameof(from));
            if (version == ToVersion)
                throw new ArgumentException("From version cannot be the same as to version.", nameof(from));
            FromVersions = [version];
        }
    }
}
