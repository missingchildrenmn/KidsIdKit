export function generateAndDownloadPdf(htmlOrElement, filename) {
    // Access jsPDF and DOMPurify from the global window object
    const { jsPDF } = window.jspdf;
    const DOMPurify = window.DOMPurify;
    const canvg = window.canvg; // Access canvg
    const html2canvas = window.html2canvas; // Access html2canvas

    // Sanitize the HTML content to prevent XSS
    const sanitizedHtml = DOMPurify.sanitize(htmlOrElement);

    const doc = new jsPDF({
        orientation: 'p',
        unit: 'pt',
        format: 'a4'
    });

    return new Promise((resolve, reject) => {
        // Option 1: Using jsPDF's html method directly
        doc.html(sanitizedHtml, {
            callback: (docInstance) => {
                docInstance.save(filename);
                resolve();
            },
            x: 10,
            y: 10
        });
    });
}

export function generatePdf(htmlOrElement) {
    // Access jsPDF and DOMPurify from the global window object
    const { jsPDF } = window.jspdf;
    const DOMPurify = window.DOMPurify;
    const canvg = window.canvg; // Access canvg
    const html2canvas = window.html2canvas; // Access html2canvas

    // Sanitize the HTML content to prevent XSS
    const sanitizedHtml = DOMPurify.sanitize(htmlOrElement);

    const doc = new jsPDF();
    return new Promise((resolve, reject) => {
        // Option 1: Using jsPDF's html method directly
        doc.fromHTML(sanitizedHtml, {
            callback: (docInstance) => {
                const output = docInstance.output("arraybuffer");
                resolve(new Uint8Array(output));
            },
            x: 10,
            y: 10
        });
    });
}
