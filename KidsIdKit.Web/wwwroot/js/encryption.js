// Web Crypto API encryption utilities for Blazor WebAssembly

window.encryptionInterop = {
    // Generate a random 32-byte key
    generateKey: async function () {
        const key = new Uint8Array(32);
        crypto.getRandomValues(key);
        return this._arrayToBase64(key);
    },

    // Generate random salt
    generateSalt: async function (size) {
        const salt = new Uint8Array(size);
        crypto.getRandomValues(salt);
        return this._arrayToBase64(salt);
    },

    // Derive key from PIN using PBKDF2
    deriveKey: async function (pin, saltBase64, iterations) {
        const encoder = new TextEncoder();
        const pinData = encoder.encode(pin);
        const salt = this._base64ToArray(saltBase64);

        // Import PIN as key material
        const keyMaterial = await crypto.subtle.importKey(
            'raw',
            pinData,
            'PBKDF2',
            false,
            ['deriveBits']
        );

        // Derive 256 bits (32 bytes)
        const derivedBits = await crypto.subtle.deriveBits(
            {
                name: 'PBKDF2',
                salt: salt,
                iterations: iterations,
                hash: 'SHA-256'
            },
            keyMaterial,
            256
        );

        return this._arrayToBase64(new Uint8Array(derivedBits));
    },

    // Encrypt using AES-CBC
    encrypt: async function (plainText, keyBase64) {
        const encoder = new TextEncoder();
        const plainData = encoder.encode(plainText);
        const keyData = this._base64ToArray(keyBase64);

        // Generate random IV
        const iv = new Uint8Array(16);
        crypto.getRandomValues(iv);

        // Import key
        const key = await crypto.subtle.importKey(
            'raw',
            keyData,
            { name: 'AES-CBC' },
            false,
            ['encrypt']
        );

        // Encrypt
        const encrypted = await crypto.subtle.encrypt(
            { name: 'AES-CBC', iv: iv },
            key,
            plainData
        );

        // Prepend IV to encrypted data
        const result = new Uint8Array(iv.length + encrypted.byteLength);
        result.set(iv, 0);
        result.set(new Uint8Array(encrypted), iv.length);

        return this._arrayToBase64(result);
    },

    // Decrypt using AES-CBC
    decrypt: async function (encryptedBase64, keyBase64) {
        const encryptedData = this._base64ToArray(encryptedBase64);
        const keyData = this._base64ToArray(keyBase64);

        if (encryptedData.length < 16) {
            throw new Error('Encrypted data is too short');
        }

        // Extract IV (first 16 bytes)
        const iv = encryptedData.slice(0, 16);
        const cipherData = encryptedData.slice(16);

        // Import key
        const key = await crypto.subtle.importKey(
            'raw',
            keyData,
            { name: 'AES-CBC' },
            false,
            ['decrypt']
        );

        // Decrypt
        const decrypted = await crypto.subtle.decrypt(
            { name: 'AES-CBC', iv: iv },
            key,
            cipherData
        );

        const decoder = new TextDecoder();
        return decoder.decode(decrypted);
    },

    // Helper: Convert Uint8Array to Base64
    _arrayToBase64: function (array) {
        let binary = '';
        for (let i = 0; i < array.length; i++) {
            binary += String.fromCharCode(array[i]);
        }
        return btoa(binary);
    },

    // Helper: Convert Base64 to Uint8Array
    _base64ToArray: function (base64) {
        const binary = atob(base64);
        const array = new Uint8Array(binary.length);
        for (let i = 0; i < binary.length; i++) {
            array[i] = binary.charCodeAt(i);
        }
        return array;
    }
};
