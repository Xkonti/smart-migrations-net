namespace SmartMigrations;

/// <summary>
/// Marks a class as a database migration with version information and execution preferences.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class MigrationAttribute : Attribute
{
    /// <summary>
    /// Gets the version this migration can migrate from. Null for initial migrations that set up the database from scratch.
    /// </summary>
    public DatabaseVersion? FromVersion { get; }

    /// <summary>
    /// Gets the version this migration migrates to.
    /// </summary>
    public DatabaseVersion ToVersion { get; }

    /// <summary>
    /// Gets a value indicating whether this migration should be avoided if alternative paths exist.
    /// Migrations marked as should avoid are used only when no other path is available.
    /// </summary>
    public bool ShouldAvoid { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MigrationAttribute"/> class.
    /// </summary>
    /// <param name="from">The version this migration can migrate from. Use null for initial migrations that set up the database from scratch.</param>
    /// <param name="to">The version this migration migrates to.</param>
    /// <param name="shouldAvoid">Whether this migration should be avoided if alternatives exist. Default is false.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="to"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="from"/> or <paramref name="to"/> is not a valid database version.</exception>
    public MigrationAttribute(string? from, string to, bool shouldAvoid = false)
    {
        ArgumentNullException.ThrowIfNull(to);

        FromVersion = from != null ? DatabaseVersion.Parse(from) : null;
        ToVersion = DatabaseVersion.Parse(to);
        ShouldAvoid = shouldAvoid;
    }
}
