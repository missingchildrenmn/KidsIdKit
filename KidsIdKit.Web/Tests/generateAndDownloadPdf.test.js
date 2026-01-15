// tests/generateAndDownloadPdf.test.js

import { generateAndDownloadPdf } from '../../KidsIdKit.Core/wwwroot/PdfGenerator/HtmlToPdf';

describe('generateAndDownloadPdf', () => {
    let originalWindow;
    let mockDocInstance;

    beforeAll(() => {
        // Save the original window object
        originalWindow = { ...window };
    });

    beforeEach(() => {
        // Mock DOMPurify.sanitize
        window.DOMPurify = {
            sanitize: jest.fn((input) => `sanitized: ${input}`),
        };

        // Create a mock docInstance with html and save methods
        mockDocInstance = {
            html: jest.fn((html, options) => {
                options.callback(mockDocInstance);
            }),
            save: jest.fn(),
        };

        // Mock jsPDF to return the mock docInstance
        window.jspdf = {
            jsPDF: jest.fn(() => mockDocInstance),
        };
    });

    afterEach(() => {
        jest.clearAllMocks(); // Clear mocks after each test
    });

    afterAll(() => {
        // Restore the original window object
        Object.assign(window, originalWindow);
    });

    it('should sanitize the input HTML before rendering', async () => {
        const inputHtml = '<div onclick="alert(\'xss\')">Unsafe</div>';
        const filename = 'test.pdf';

        await generateAndDownloadPdf(inputHtml, filename);

        expect(window.DOMPurify.sanitize).toHaveBeenCalledWith(inputHtml);
    });

    it('should call doc.html with sanitized HTML', async () => {
        const inputHtml = '<p>Hello World</p>';
        const filename = 'hello.pdf';

        await generateAndDownloadPdf(inputHtml, filename);

        expect(mockDocInstance.html).toHaveBeenCalledWith(
            'sanitized: <p>Hello World</p>',
            expect.objectContaining({ x: 10, y: 10 })
        );
    });

    it('should save the PDF with the given filename', async () => {
        const inputHtml = '<p>Content</p>';
        const filename = 'myfile.pdf';

        await generateAndDownloadPdf(inputHtml, filename);

        expect(mockDocInstance.save).toHaveBeenCalledWith('myfile.pdf');
    });
});
