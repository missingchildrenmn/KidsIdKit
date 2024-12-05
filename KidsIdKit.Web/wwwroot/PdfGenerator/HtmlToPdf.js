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

        // Option 2: Using html2canvas and adding image to jsPDF
        // Uncomment below if you prefer this method
        /*
        const element = typeof htmlOrElement === 'string' ? document.createElement('div') : htmlOrElement;
        if (typeof htmlOrElement === 'string') {
            element.innerHTML = sanitizedHtml;
            document.body.appendChild(element); // Temporarily add to DOM
        }

        html2canvas(element).then(canvas => {
            const imgData = canvas.toDataURL('image/png');
            doc.addImage(imgData, 'PNG', 10, 10);
            doc.save(filename);
            if (typeof htmlOrElement === 'string') {
                document.body.removeChild(element); // Clean up
            }
            resolve();
        }).catch(error => {
            if (typeof htmlOrElement === 'string') {
                document.body.removeChild(element); // Clean up
            }
            reject(error);
        });
        */
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
        doc.html(sanitizedHtml, {
            callback: (docInstance) => {
                const output = docInstance.output("arraybuffer");
                resolve(new Uint8Array(output));
            },
            x: 10,
            y: 10
        });

        // Option 2: Using html2canvas and adding image to jsPDF
        // Uncomment below if you prefer this method
        /*
        const element = typeof htmlOrElement === 'string' ? document.createElement('div') : htmlOrElement;
        if (typeof htmlOrElement === 'string') {
            element.innerHTML = sanitizedHtml;
            document.body.appendChild(element); // Temporarily add to DOM
        }

        html2canvas(element).then(canvas => {
            const imgData = canvas.toDataURL('image/png');
            doc.addImage(imgData, 'PNG', 10, 10);
            const output = doc.output("arraybuffer");
            resolve(new Uint8Array(output));
            if (typeof htmlOrElement === 'string') {
                document.body.removeChild(element); // Clean up
            }
        }).catch(error => {
            if (typeof htmlOrElement === 'string') {
                document.body.removeChild(element); // Clean up
            }
            reject(error);
        });
        */
    });
}
