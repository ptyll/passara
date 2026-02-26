namespace Passara.Core.Cryptography;

/// <summary>
/// Options for key derivation function operations.
/// </summary>
public sealed record KdfOptions
{
    /// <summary>
    /// Gets the number of iterations (operations limit).
    /// </summary>
    public int Iterations { get; }

    /// <summary>
    /// Gets the memory limit in KiB.
    /// </summary>
    public int MemoryKib { get; }

    /// <summary>
    /// Gets the parallelism factor.
    /// </summary>
    public int Parallelism { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="KdfOptions"/> class.
    /// </summary>
    /// <param name="iterations">The number of iterations.</param>
    /// <param name="memoryKib">The memory limit in KiB.</param>
    /// <param name="parallelism">The parallelism factor.</param>
    public KdfOptions(int iterations, int memoryKib, int parallelism)
    {
        Iterations = iterations;
        MemoryKib = memoryKib;
        Parallelism = parallelism;
    }

    /// <summary>
    /// Gets interactive KDF options for fast key derivation (mobile devices).
    /// </summary>
    public static KdfOptions Interactive => new(
        KdfParameters.Argon2InteractiveIterations,
        KdfParameters.Argon2InteractiveMemoryKib,
        KdfParameters.Argon2InteractiveParallelism);

    /// <summary>
    /// Gets moderate KDF options for balanced security and performance (default).
    /// </summary>
    public static KdfOptions Moderate => new(
        KdfParameters.Argon2ModerateIterations,
        KdfParameters.Argon2ModerateMemoryKib,
        KdfParameters.Argon2ModerateParallelism);

    /// <summary>
    /// Gets sensitive KDF options for maximum security (slower).
    /// </summary>
    public static KdfOptions Sensitive => new(
        KdfParameters.Argon2SensitiveIterations,
        KdfParameters.Argon2SensitiveMemoryKib,
        KdfParameters.Argon2SensitiveParallelism);
}
