// Pointer-driven vertical resize for an ion-textarea.
// Attaches pointer listeners to `handle` and adjusts the height of the
// native <textarea> rendered inside the ion-textarea's shadow DOM.
//
// Returns a function that removes the listeners (for disposal).
export function attach(handle, ionTextarea) {
    if (!handle || !ionTextarea) return null;

    const getNative = () => ionTextarea.shadowRoot && ionTextarea.shadowRoot.querySelector('textarea');

    let startY = 0;
    let startH = 0;
    let native = null;
    let activePointerId = null;

    const onMove = (e) => {
        if (native === null) return;
        const dy = e.clientY - startY;
        const next = Math.max(48, startH + dy);
        native.style.height = next + 'px';
    };

    const onUp = (e) => {
        if (activePointerId !== null) {
            try { handle.releasePointerCapture(activePointerId); } catch { }
            activePointerId = null;
        }
        window.removeEventListener('pointermove', onMove);
        window.removeEventListener('pointerup', onUp);
        window.removeEventListener('pointercancel', onUp);
        native = null;
    };

    const onDown = (e) => {
        native = getNative();
        if (!native) return;
        e.preventDefault();
        startY = e.clientY;
        startH = native.getBoundingClientRect().height;
        activePointerId = e.pointerId;
        try { handle.setPointerCapture(e.pointerId); } catch { }
        window.addEventListener('pointermove', onMove);
        window.addEventListener('pointerup', onUp);
        window.addEventListener('pointercancel', onUp);
    };

    handle.addEventListener('pointerdown', onDown);

    return () => {
        handle.removeEventListener('pointerdown', onDown);
        window.removeEventListener('pointermove', onMove);
        window.removeEventListener('pointerup', onUp);
        window.removeEventListener('pointercancel', onUp);
    };
}
