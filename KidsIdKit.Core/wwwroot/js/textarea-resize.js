// Vertical resize for an ion-textarea, driven by touch + pointer events.
//
// Mobile WebViews (iOS WKWebView, Android System WebView inside
// BlazorWebView) are inconsistent about Pointer Events — especially when
// the user taps-holds-then-drags, which the OS interprets as the start of
// a text-selection / context-menu gesture and then cancels the pointer
// stream. To work reliably on phones we listen to raw Touch Events as the
// primary path and use Pointer Events only for mouse / pen on desktop.
//
// We resize the HOST <ion-textarea> element (not the shadow <textarea>)
// because Ionic recomputes the inner textarea's height on its own
// lifecycle and will overwrite anything we set there. Sizing the host is
// what Ionic actually honors.
//
// Returns a disposer that removes all listeners.
export function attach(handle, ionTextarea) {
    if (!handle || !ionTextarea) return null;

    let startY = 0;
    let startH = 0;
    let dragging = false;
    let activePointerId = null;
    let activeTouchId = null;

    const setActiveClass = (on) => {
        if (on) handle.classList.add('is-dragging');
        else handle.classList.remove('is-dragging');
    };

    const beginDrag = (clientY) => {
        startY = clientY;
        startH = ionTextarea.getBoundingClientRect().height;
        if (startH <= 0) return false;
        dragging = true;
        setActiveClass(true);
        return true;
    };

    const applyDrag = async (clientY) => {
        if (!dragging) return;
        const dy = clientY - startY;
        const next = Math.max(48, startH + dy);
        
        ionTextarea.style.height = next + 'px';
        const native = await ionTextarea.getInputElement();
        if (native) native.style.height = next + 'px';
    };

    const endDrag = () => {
        if (!dragging) return;
        dragging = false;
        setActiveClass(false);
    };

    // ---------- Touch Events (primary path on mobile) ----------

    const findTouch = (touchList, id) => {
        for (let i = 0; i < touchList.length; i++) {
            if (touchList[i].identifier === id) return touchList[i];
        }
        return null;
    };

    const onTouchStart = (e) => {
        if (dragging) return;
        const t = e.changedTouches[0];
        if (!t) return;
        if (!beginDrag(t.clientY)) return;
        activeTouchId = t.identifier;
        if (e.cancelable) e.preventDefault();
    };

    const onTouchMove = (e) => {
        if (!dragging || activeTouchId === null) return;
        const t = findTouch(e.changedTouches, activeTouchId)
            || findTouch(e.touches, activeTouchId);
        if (!t) return;
        if (e.cancelable) e.preventDefault();
        applyDrag(t.clientY);
    };

    const onTouchEnd = (e) => {
        if (!dragging || activeTouchId === null) return;
        if (e && e.changedTouches && !findTouch(e.changedTouches, activeTouchId)) return;
        activeTouchId = null;
        endDrag();
    };

    // ---------- Pointer Events (desktop mouse / pen) ----------

    const onPointerMove = (e) => {
        if (activeTouchId !== null) return;        // touch path owns it
        if (activePointerId === null) return;
        if (e.pointerId !== activePointerId) return;
        if (e.cancelable) e.preventDefault();
        applyDrag(e.clientY);
    };

    const onPointerUp = (e) => {
        if (activePointerId === null) return;
        if (e.pointerId !== activePointerId) return;
        try { handle.releasePointerCapture(activePointerId); } catch { }
        activePointerId = null;
        if (activeTouchId === null) endDrag();
    };

    const onPointerDown = (e) => {
        // Skip touch pointers — Touch Events handle those.
        if (e.pointerType === 'touch') return;
        if (e.button !== undefined && e.button !== 0) return;
        if (!beginDrag(e.clientY)) return;
        e.preventDefault();
        activePointerId = e.pointerId;
        try { handle.setPointerCapture(e.pointerId); } catch { }
    };

    const onContextMenu = (e) => { e.preventDefault(); };

    // Non-passive touch listeners are REQUIRED for preventDefault() to work
    // and to stop the OS taking the gesture for selection on tap-and-hold.
    handle.addEventListener('touchstart', onTouchStart, { passive: false });
    handle.addEventListener('touchmove', onTouchMove, { passive: false });
    handle.addEventListener('touchend', onTouchEnd, { passive: false });
    handle.addEventListener('touchcancel', onTouchEnd, { passive: false });
    // Document-level fallbacks: if the OS routes subsequent touchmove events
    // away from the original target (happens on some WebViews after a hold),
    // we still see them here. Must also be non-passive to preventDefault.
    document.addEventListener('touchmove', onTouchMove, { passive: false });
    document.addEventListener('touchend', onTouchEnd, { passive: false });
    document.addEventListener('touchcancel', onTouchEnd, { passive: false });

    handle.addEventListener('pointerdown', onPointerDown);
    handle.addEventListener('pointermove', onPointerMove);
    handle.addEventListener('pointerup', onPointerUp);
    handle.addEventListener('pointercancel', onPointerUp);
    window.addEventListener('pointermove', onPointerMove);
    window.addEventListener('pointerup', onPointerUp);
    window.addEventListener('pointercancel', onPointerUp);

    handle.addEventListener('contextmenu', onContextMenu);

    return () => {
        handle.removeEventListener('touchstart', onTouchStart);
        handle.removeEventListener('touchmove', onTouchMove);
        handle.removeEventListener('touchend', onTouchEnd);
        handle.removeEventListener('touchcancel', onTouchEnd);
        document.removeEventListener('touchmove', onTouchMove);
        document.removeEventListener('touchend', onTouchEnd);
        document.removeEventListener('touchcancel', onTouchEnd);
        handle.removeEventListener('pointerdown', onPointerDown);
        handle.removeEventListener('pointermove', onPointerMove);
        handle.removeEventListener('pointerup', onPointerUp);
        handle.removeEventListener('pointercancel', onPointerUp);
        window.removeEventListener('pointermove', onPointerMove);
        window.removeEventListener('pointerup', onPointerUp);
        window.removeEventListener('pointercancel', onPointerUp);
        handle.removeEventListener('contextmenu', onContextMenu);
    };
}
