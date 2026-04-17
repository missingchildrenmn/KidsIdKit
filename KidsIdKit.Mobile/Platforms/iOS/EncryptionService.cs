using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using KidsIdKit.Core.Services;

namespace KidsIdKit.Mobile.Services;

/// <summary>
/// Encryption service using System.Security.Cryptography for MAUI.
/// </summary>
public class EncryptionService : IEncryptionService
{
    private const int KeySize = 256;
    private const int IvSize = 16;

    [DllImport("__Internal", EntryPoint = "getKey")]
    public extern static IntPtr GetKey(IntPtr pinPointer, int pinLength, IntPtr saltPointer, int saltLength, out int length);
    [DllImport("__Internal", EntryPoint = "freeNativePtr")]
    private static extern void FreeNativePtr(IntPtr ptr);
    [DllImport("__Internal", EntryPoint = "encryptData")]
    private static extern IntPtr EncryptData(IntPtr keyPointer, Int32 keyLength, IntPtr targetPointer, Int32 targetLength, out Int32 length);

    [DllImport("__Internal", EntryPoint = "decryptData")]
    private static extern IntPtr DecryptData(IntPtr keyPointer, Int32 keyLength, IntPtr encryptedPointer, Int32 encryptedLength, out Int32 length);

    public Task<byte[]> GenerateKeyAsync()
    {
        using var aes = Aes.Create();
        aes.KeySize = KeySize;
        aes.GenerateKey();
        return Task.FromResult(aes.Key);
    }

    public Task<byte[]> GenerateSaltAsync(int size = 32)
    {
        var salt = new byte[size];
        RandomNumberGenerator.Fill(salt);
        return Task.FromResult(salt);
    }

    private byte[] GetKey(string pin, byte[] salt)
    {
        IntPtr pinPointer = Marshal.AllocHGlobal(pin.Length);
        IntPtr saltPointer = Marshal.AllocHGlobal(salt.Length);
        IntPtr keyPointer = IntPtr.Zero;
        try
        {
            byte[] pinBytes = Encoding.UTF8.GetBytes(pin);
            Marshal.Copy(pinBytes, 0, pinPointer, pinBytes.Length);
            Marshal.Copy(salt, 0, saltPointer, salt.Length);

            keyPointer = GetKey(pinPointer, pinBytes.Length, saltPointer, salt.Length, out int length);
            byte[] managedData = new byte[length];
            Marshal.Copy(keyPointer, managedData, 0, length);
            
            return managedData;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetKey: {ex}");
            throw;
        }
        finally
        {
            if (keyPointer != IntPtr.Zero)
            {
                FreeNativePtr(keyPointer);
            }
            Marshal.FreeHGlobal(saltPointer);
        }
    }

    private byte[] EncryptData(byte[] keyBytes, string plainText)
    {
        byte[] targetBytes = Encoding.UTF8.GetBytes(plainText);
        IntPtr keyPointer = IntPtr.Zero;
        IntPtr targetPointer = IntPtr.Zero;
        IntPtr encryptedPointer = IntPtr.Zero;
        try
        {
            keyPointer = CopyBytesToPointer(keyBytes);
            targetPointer = CopyBytesToPointer(targetBytes);

            encryptedPointer = EncryptData(keyPointer, keyBytes.Length, targetPointer, targetBytes.Length, out int length);
            byte[] encryptedData = CopyPointerToBytes(encryptedPointer, length);

            return encryptedData;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in EncryptData: {ex}");
            throw;
        }
        finally
        {
            if (encryptedPointer != IntPtr.Zero)
            {
                FreeNativePtr(encryptedPointer);
            }
            Marshal.FreeHGlobal(keyPointer);
            Marshal.FreeHGlobal(targetPointer);
        }
    }

    private string DecryptData(byte[] key, byte[] encryptedBytes)
    {
        IntPtr keyPointer = IntPtr.Zero;
        IntPtr encryptedPointer = IntPtr.Zero;
        IntPtr decryptedPointer = IntPtr.Zero;
        try
        {
            keyPointer = CopyBytesToPointer(key);
            encryptedPointer = CopyBytesToPointer(encryptedBytes);

            decryptedPointer = DecryptData(keyPointer, key.Length, encryptedPointer, encryptedBytes.Length, out int length);
            byte[] decryptedData = CopyPointerToBytes(decryptedPointer, length);
            
            return Encoding.UTF8.GetString(decryptedData);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DecryptData: {ex}");
            throw;
        }
        finally
        {
            if (decryptedPointer != IntPtr.Zero)
            {
                FreeNativePtr(decryptedPointer);
            }
            Marshal.FreeHGlobal(keyPointer);
            Marshal.FreeHGlobal(encryptedPointer);
        }
    }

    private IntPtr CopyBytesToPointer(byte[] data)
    {
        IntPtr pointer = Marshal.AllocHGlobal(data.Length);
        Marshal.Copy(data, 0, pointer, data.Length);
        return pointer;
    }

    private byte[] CopyPointerToBytes(IntPtr pointer, int length)
    {
        byte[] data = new byte[length];
        Marshal.Copy(pointer, data, 0, length);
        return data;
    }

    public Task<byte[]> DeriveKeyAsync(string pin, byte[] salt, int iterations = 100_000)
    {
        var derivedKey = GetKey(pin, salt);
            
        return Task.FromResult(derivedKey);
    }

    public Task<string> EncryptAsync(string plainText, byte[] key)
    {
        ArgumentNullException.ThrowIfNull(plainText);
        ArgumentNullException.ThrowIfNull(key);

        if (key.Length != KeySize / 8)
            throw new ArgumentException($"Key must be {KeySize / 8} bytes for AES-256.", nameof(key));

        var result = EncryptData(key, plainText);

        return Task.FromResult(Convert.ToBase64String(result));
    }

    public Task<string> DecryptAsync(string encryptedText, byte[] key)
    {
        ArgumentNullException.ThrowIfNull(encryptedText);
        ArgumentNullException.ThrowIfNull(key);

        if (key.Length != KeySize / 8)
            throw new ArgumentException($"Key must be {KeySize / 8} bytes for AES-256.", nameof(key));

        var encryptedBytes = Convert.FromBase64String(encryptedText);

        if (encryptedBytes.Length < IvSize)
            throw new ArgumentException("Encrypted data is too short.", nameof(encryptedText));

        var decryptedText = DecryptData(key, encryptedBytes);

        return Task.FromResult(decryptedText);
    }
}
