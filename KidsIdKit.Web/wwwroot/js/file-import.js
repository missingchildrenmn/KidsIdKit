// File import utilities for Blazor WebAssembly

window.fileImportInterop = {
    selectFile: async function() {
        return new Promise((resolve) => {
            const input = document.createElement('input');
            input.type = 'file';
            input.accept = '*/*';

            input.onchange = async function(event) {
                const file = event.target.files[0];
                if (file) {
                    try {
                        // Read the file content
                        // Note: In browsers, we cannot access the full file system path for security reasons.
                        // Instead, we read the file content and return it as a JSON object with filename and content.
                        const fileContent = await file.text();
                        const result = {
                            fileName: file.name,
                            content: fileContent
                        };
                        resolve(JSON.stringify(result));
                    } catch (error) {
                        console.error('Error reading file:', error);
                        resolve(null);
                    }
                } else {
                    resolve(null);
                }
            };

            input.oncancel = function() {
                resolve(null);
            };

            // Trigger the file picker dialog
            input.click();
        });
    }
};
