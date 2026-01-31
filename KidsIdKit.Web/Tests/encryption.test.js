/**
 * @jest-environment jsdom
 */

// Mock the Web Crypto API for testing
const crypto = require('crypto');
const { TextEncoder, TextDecoder } = require('util');

// Add TextEncoder and TextDecoder to global scope
global.TextEncoder = TextEncoder;
global.TextDecoder = TextDecoder;

// Setup Web Crypto API mock
global.crypto = {
    getRandomValues: (array) => {
        return crypto.randomFillSync(array);
    },
    subtle: {
        importKey: async (format, keyData, algorithm, extractable, keyUsages) => {
            return { format, keyData, algorithm, extractable, keyUsages };
        },
        deriveBits: async (algorithm, keyMaterial, length) => {
            // Mock PBKDF2 derivation
            const derived = crypto.pbkdf2Sync(
                Buffer.from(keyMaterial.keyData),
                algorithm.salt,
                algorithm.iterations,
                length / 8,
                'sha256'
            );
            return derived.buffer;
        },
        encrypt: async (algorithm, key, data) => {
            // Mock AES-CBC encryption
            const cipher = crypto.createCipheriv(
                'aes-256-cbc',
                Buffer.from(key.keyData),
                Buffer.from(algorithm.iv)
            );
            const encrypted = Buffer.concat([
                cipher.update(Buffer.from(data)),
                cipher.final()
            ]);
            return encrypted.buffer;
        },
        decrypt: async (algorithm, key, data) => {
            // Mock AES-CBC decryption
            const decipher = crypto.createDecipheriv(
                'aes-256-cbc',
                Buffer.from(key.keyData),
                Buffer.from(algorithm.iv)
            );
            const decrypted = Buffer.concat([
                decipher.update(Buffer.from(data)),
                decipher.final()
            ]);
            return decrypted.buffer;
        }
    }
};

// Load the encryption module
require('../wwwroot/js/encryption.js');

describe('encryptionInterop', () => {
    describe('_arrayToBase64', () => {
        it('should convert small arrays correctly', () => {
            const smallArray = new Uint8Array([72, 101, 108, 108, 111]); // "Hello"
            const result = window.encryptionInterop._arrayToBase64(smallArray);
            expect(result).toBe(btoa('Hello'));
        });

        it('should convert medium arrays correctly', () => {
            const mediumArray = new Uint8Array(1000);
            for (let i = 0; i < 1000; i++) {
                mediumArray[i] = i % 256;
            }
            const result = window.encryptionInterop._arrayToBase64(mediumArray);
            
            // Verify by converting back
            const decoded = window.encryptionInterop._base64ToArray(result);
            expect(decoded).toEqual(mediumArray);
        });

        it('should convert large arrays (>65536 bytes) correctly using chunking', () => {
            // Create an array larger than the chunk size
            const largeArray = new Uint8Array(100000);
            for (let i = 0; i < 100000; i++) {
                largeArray[i] = i % 256;
            }
            const result = window.encryptionInterop._arrayToBase64(largeArray);
            
            // Verify by converting back
            const decoded = window.encryptionInterop._base64ToArray(result);
            expect(decoded).toEqual(largeArray);
        });

        it('should handle empty arrays', () => {
            const emptyArray = new Uint8Array(0);
            const result = window.encryptionInterop._arrayToBase64(emptyArray);
            expect(result).toBe('');
        });

        it('should handle arrays with all zero bytes', () => {
            const zeroArray = new Uint8Array(100);
            const result = window.encryptionInterop._arrayToBase64(zeroArray);
            const decoded = window.encryptionInterop._base64ToArray(result);
            expect(decoded).toEqual(zeroArray);
        });

        it('should handle arrays with all 255 bytes', () => {
            const maxArray = new Uint8Array(100).fill(255);
            const result = window.encryptionInterop._arrayToBase64(maxArray);
            const decoded = window.encryptionInterop._base64ToArray(result);
            expect(decoded).toEqual(maxArray);
        });
    });

    describe('_base64ToArray', () => {
        it('should convert base64 strings to arrays correctly', () => {
            const base64 = btoa('Hello');
            const result = window.encryptionInterop._base64ToArray(base64);
            expect(result).toEqual(new Uint8Array([72, 101, 108, 108, 111]));
        });

        it('should round-trip with _arrayToBase64', () => {
            const original = new Uint8Array([1, 2, 3, 4, 5, 255, 254, 253]);
            const base64 = window.encryptionInterop._arrayToBase64(original);
            const decoded = window.encryptionInterop._base64ToArray(base64);
            expect(decoded).toEqual(original);
        });
    });

    describe('generateKey', () => {
        it('should generate a 32-byte key', async () => {
            const keyBase64 = await window.encryptionInterop.generateKey();
            const keyArray = window.encryptionInterop._base64ToArray(keyBase64);
            expect(keyArray.length).toBe(32);
        });

        it('should generate different keys each time', async () => {
            const key1 = await window.encryptionInterop.generateKey();
            const key2 = await window.encryptionInterop.generateKey();
            expect(key1).not.toBe(key2);
        });
    });

    describe('generateSalt', () => {
        it('should generate salt of specified size', async () => {
            const saltBase64 = await window.encryptionInterop.generateSalt(16);
            const saltArray = window.encryptionInterop._base64ToArray(saltBase64);
            expect(saltArray.length).toBe(16);
        });

        it('should generate different salts each time', async () => {
            const salt1 = await window.encryptionInterop.generateSalt(16);
            const salt2 = await window.encryptionInterop.generateSalt(16);
            expect(salt1).not.toBe(salt2);
        });
    });

    describe('deriveKey', () => {
        it('should derive a key from a PIN', async () => {
            const pin = '1234';
            const salt = await window.encryptionInterop.generateSalt(16);
            const derivedKey = await window.encryptionInterop.deriveKey(pin, salt, 100000);
            
            const keyArray = window.encryptionInterop._base64ToArray(derivedKey);
            expect(keyArray.length).toBe(32); // 256 bits = 32 bytes
        });

        it('should derive same key for same PIN and salt', async () => {
            const pin = '5678';
            const salt = await window.encryptionInterop.generateSalt(16);
            
            const key1 = await window.encryptionInterop.deriveKey(pin, salt, 100000);
            const key2 = await window.encryptionInterop.deriveKey(pin, salt, 100000);
            
            expect(key1).toBe(key2);
        });

        it('should derive different keys for different PINs', async () => {
            const salt = await window.encryptionInterop.generateSalt(16);
            
            const key1 = await window.encryptionInterop.deriveKey('1111', salt, 100000);
            const key2 = await window.encryptionInterop.deriveKey('2222', salt, 100000);
            
            expect(key1).not.toBe(key2);
        });
    });

    describe('encrypt and decrypt', () => {
        it('should encrypt and decrypt text correctly', async () => {
            const plainText = 'This is a secret message';
            const key = await window.encryptionInterop.generateKey();
            
            const encrypted = await window.encryptionInterop.encrypt(plainText, key);
            const decrypted = await window.encryptionInterop.decrypt(encrypted, key);
            
            expect(decrypted).toBe(plainText);
        });

        it('should encrypt same text differently each time (due to random IV)', async () => {
            const plainText = 'Test message';
            const key = await window.encryptionInterop.generateKey();
            
            const encrypted1 = await window.encryptionInterop.encrypt(plainText, key);
            const encrypted2 = await window.encryptionInterop.encrypt(plainText, key);
            
            expect(encrypted1).not.toBe(encrypted2);
            
            // But both should decrypt to the same plaintext
            const decrypted1 = await window.encryptionInterop.decrypt(encrypted1, key);
            const decrypted2 = await window.encryptionInterop.decrypt(encrypted2, key);
            
            expect(decrypted1).toBe(plainText);
            expect(decrypted2).toBe(plainText);
        });

        it('should handle large text data', async () => {
            // Create a large text (simulate encrypted family data)
            const largeText = 'A'.repeat(100000);
            const key = await window.encryptionInterop.generateKey();
            
            const encrypted = await window.encryptionInterop.encrypt(largeText, key);
            const decrypted = await window.encryptionInterop.decrypt(encrypted, key);
            
            expect(decrypted).toBe(largeText);
        });

        it('should handle Unicode text', async () => {
            const unicodeText = 'Hello 世界 🌍 émojis';
            const key = await window.encryptionInterop.generateKey();
            
            const encrypted = await window.encryptionInterop.encrypt(unicodeText, key);
            const decrypted = await window.encryptionInterop.decrypt(encrypted, key);
            
            expect(decrypted).toBe(unicodeText);
        });

        it('should throw error when decrypting with wrong key', async () => {
            const plainText = 'Secret';
            const key1 = await window.encryptionInterop.generateKey();
            const key2 = await window.encryptionInterop.generateKey();
            
            const encrypted = await window.encryptionInterop.encrypt(plainText, key1);
            
            // Decryption with wrong key should fail
            await expect(
                window.encryptionInterop.decrypt(encrypted, key2)
            ).rejects.toThrow();
        });

        it('should throw error when encrypted data is too short', async () => {
            const key = await window.encryptionInterop.generateKey();
            const invalidData = window.encryptionInterop._arrayToBase64(new Uint8Array(10));
            
            await expect(
                window.encryptionInterop.decrypt(invalidData, key)
            ).rejects.toThrow('Encrypted data is too short');
        });
    });
});
