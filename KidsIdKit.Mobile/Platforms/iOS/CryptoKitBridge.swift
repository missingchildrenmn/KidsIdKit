import Foundation
import CryptoKit
import CommonCrypto

/// Bridge helpers exposing selective CryptoKit functionality to native callers.
///
/// This file provides C-callable functions used by other platforms or native
/// layers to derive symmetric keys, encrypt and decrypt data using AES-GCM,
/// and free allocated native pointers. Returned pointers are allocated on the
/// heap and must be freed by calling `freeNativePtr` from the caller.

@_cdecl("getKey")
/// Derive a 32-byte symmetric key using HKDF-SHA256 from the given PIN and salt.
///
/// - Parameters:
///   - pinPointer: Pointer to the PIN bytes.
///   - pinLength: Length of the PIN in bytes.
///   - saltPointer: Pointer to the salt bytes.
///   - saltLength: Length of the salt in bytes.
///   - lengthOut: Output pointer that will be set to the length of the returned key.
/// - Returns: An `UnsafePointer<UInt8>` to an allocated buffer containing the derived key bytes.
///
/// - Note: The returned pointer is heap-allocated. The caller is responsible for
///   freeing it by calling `freeNativePtr` to avoid memory leaks.
public func getKey(pinPointer: UnsafePointer<UInt8>, pinLength: UInt8, saltPointer: UnsafePointer<UInt8>, saltLength: UInt8, lengthOut: UnsafeMutablePointer<UInt8>) -> UnsafePointer<UInt8> {
    let pinBuffer = UnsafeBufferPointer(start: pinPointer, count: Int(pinLength))
    let pinArray = Array(pinBuffer)
    let saltBuffer = UnsafeBufferPointer(start: saltPointer, count: Int(saltLength))
    let saltArray = Array(saltBuffer)

    let symmetricKey = SymmetricKey(data: pinArray)
    
    let derivedKey = HKDF<SHA256>.deriveKey(
        inputKeyMaterial: symmetricKey,
        salt: saltArray,
        outputByteCount: 32
    )

    var keyData: [UInt8] = []
        derivedKey.withUnsafeBytes {
            keyData.append(contentsOf: $0)
        }

    lengthOut.pointee = UInt8(keyData.count)
    let pointer = UnsafeMutablePointer<UInt8>.allocate(capacity: keyData.count)
    pointer.initialize(from: keyData, count: keyData.count)
    return UnsafePointer(pointer)
}

@_cdecl("encryptData")
/// Encrypt raw bytes using AES-GCM with the provided symmetric key.
///
/// - Parameters:
///   - keyPointer: Pointer to the symmetric key bytes.
///   - keyLength: Length of the symmetric key in bytes.
///   - targetPointer: Pointer to the plaintext bytes to encrypt.
///   - targetLength: Length of the plaintext in bytes.
///   - lengthOut: Output pointer set to the length of the encrypted payload.
/// - Returns: An `UnsafePointer<UInt8>` to an allocated buffer containing the
///   combined sealed box (nonce + ciphertext + tag) on success, or an empty
///   buffer on failure. The caller must free the returned pointer via
///   `freeNativePtr` when finished.
public func encryptData(keyPointer: UnsafePointer<UInt8>, keyLength: Int32, targetPointer: UnsafePointer<UInt8>, targetLength: Int32, lengthOut: UnsafeMutablePointer<Int32>) -> UnsafePointer<UInt8> {
    let keyBuffer = UnsafeBufferPointer(start: keyPointer, count: Int(keyLength))
    let keyArray: [UInt8] = Array(keyBuffer)
    let targetBuffer = UnsafeBufferPointer(start: targetPointer, count: Int(targetLength))
    let targetArray: [UInt8] = Array(targetBuffer)

    let key = SymmetricKey(data: Data(keyArray))

    do {
        let sealedBox = try AES.GCM.seal(Data(targetArray), using: key)

        var encryptedData: [UInt8] = []
            sealedBox.combined!.withUnsafeBytes {
                encryptedData.append(contentsOf: $0)
            }

        lengthOut.pointee = Int32(encryptedData.count)
        let pointer = UnsafeMutablePointer<UInt8>.allocate(capacity: encryptedData.count)
        pointer.initialize(from: encryptedData, count: encryptedData.count)
    return UnsafePointer(pointer)
        } catch {
        lengthOut.pointee = 0
        let pointer = UnsafeMutablePointer<UInt8>.allocate(capacity: 0)
        pointer.initialize(from: [UInt8](), count: 0)
        return UnsafePointer(pointer)
    }
}

@_cdecl("decryptData")
/// Decrypt data previously produced by `encryptData` using AES-GCM.
///
/// - Parameters:
///   - keyPointer: Pointer to the symmetric key bytes.
///   - keyLength: Length of the symmetric key in bytes.
///   - encryptedPointer: Pointer to the combined sealed box bytes (nonce + ciphertext + tag).
///   - encryptedLength: Length of the encrypted payload in bytes.
///   - lengthOut: Output pointer set to the length of the decrypted plaintext.
/// - Returns: An `UnsafePointer<UInt8>` to an allocated buffer containing the
///   decrypted plaintext on success, or an empty buffer on failure. The caller
///   must free the returned pointer via `freeNativePtr` when finished.
public func decryptData(keyPointer: UnsafePointer<UInt8>, keyLength: Int32, encryptedPointer: UnsafePointer<UInt8>, encryptedLength: Int32, lengthOut: UnsafeMutablePointer<Int32>) -> UnsafePointer<UInt8> {
    let keyBuffer = UnsafeBufferPointer(start: keyPointer, count: Int(keyLength))
    let keyArray: [UInt8] = Array(keyBuffer)
    let encryptedBuffer = UnsafeBufferPointer(start: encryptedPointer, count: Int(encryptedLength))
    let encryptedArray: [UInt8] = Array(encryptedBuffer)

    let key = SymmetricKey(data: Data(keyArray))

    do {
        let sealedBox = try AES.GCM.SealedBox(combined: Data(encryptedArray))
        let decryptedData = try AES.GCM.open(sealedBox, using: key)

        var decryptedArray: [UInt8] = []
            decryptedData.withUnsafeBytes {
                decryptedArray.append(contentsOf: $0)
            }

        lengthOut.pointee = Int32(decryptedArray.count)
        let pointer = UnsafeMutablePointer<UInt8>.allocate(capacity: decryptedArray.count)
        pointer.initialize(from: decryptedArray, count: decryptedArray.count)
        return UnsafePointer(pointer)
    } catch {
        lengthOut.pointee = 0
        let pointer = UnsafeMutablePointer<UInt8>.allocate(capacity: 0)
        pointer.initialize(from: [UInt8](), count: 0)
        return UnsafePointer(pointer)
    }
}

@_cdecl("freeNativePtr")
/// Free a pointer previously returned by this bridge.
///
/// - Parameter ptr: Pointer returned by one of the functions above. If `nil`, the
///   call is a no-op. This function deallocates the memory allocated by the
///   bridge functions; callers must not access the memory after calling this.
public func freeNativePtr(ptr: UnsafePointer<UInt8>?) {
    ptr?.deallocate()
}
