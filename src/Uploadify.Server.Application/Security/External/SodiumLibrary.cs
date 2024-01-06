using System.Runtime.InteropServices;

namespace Uploadify.Server.Application.Security.External;

/// <summary>
///     Sodium is a modern, easy-to-use software library for encryption, decryption, signatures, password hashing, and more.
///     Source documentation for the Sodium library is available at https://doc.libsodium.org/.
/// </summary>
public class SodiumLibrary
{
    private const string Name = "libsodium";

    static SodiumLibrary() => sodium_init();

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void sodium_init();

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void randombytes_buf(byte[] buffer, int size);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int crypto_pwhash(byte[] buffer, long bufferLen, byte[] password, long passwordLen, byte[] salt, long opsLimit, int memLimit, int alg);
}

public enum Argon2Type
{
    Default = 0,
    Argon2i = 1,
    Argon2id = 2
}
