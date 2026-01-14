// Type definitions for custom window methods
interface Window {
    downloadFileFromStream: (fileName: string, base64String: string) => void;
    downloadFileFromText: (fileName: string, textContent: string) => void;
    canShareFiles: () => boolean;
    shareFile: (fileName: string) => Promise<void>;
}
