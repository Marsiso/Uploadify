using Uploadify.Server.Application.Security.External;

namespace Uploadify.Server.Application.Security.Services;

public class ArgonPasswordHasherOptions
{
    public const string Delimiter = ".";

    public string? Pepper { get; set; }

    /// <summary>
    ///     Defaults to 16 B.
    /// </summary>
    public int SaltSize { get; set; } = 16;

    /// <summary>
    ///     Defaults to 32 B.
    /// </summary>
    public int KeySize { get; set; } = 32;

    public Argon2Type Algorithm { get; set; } = Argon2Type.Argon2id;

    /// <summary>
    ///     Represents the maximum amount of computations to perform. Raising this number will make the function require more CPU cycles to compute a key.
    ///     This number must be between crypto_pwhash_OPSLIMIT_MIN and crypto_pwhash_OPSLIMIT_MAX.
    /// </summary>
    public long OperationsLimit { get; set; } = 4;

    /// <summary>
    ///     Memory limit is the maximum amount of RAM in bytes that the function will use.
    ///     This number must be between crypto_pwhash_MEMLIMIT_MIN and crypto_pwhash_MEMLIMIT_MAX.
    ///     Defaults to 128 MiB.
    /// </summary>
    public int MemoryLimit { get; set; } = 268_435_456;
}
