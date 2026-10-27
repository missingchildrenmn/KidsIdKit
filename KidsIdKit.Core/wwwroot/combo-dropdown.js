// Drives the scroll-direction indicators for the custom combobox dropdown.
//
// A native <datalist> popup is rendered/owned by the browser and cannot be
// styled or measured, so EditCombo uses its own dropdown. This module watches
// the scrollable options list and toggles the `is-visible` class on the up /
// down arrow strips whenever there is more content above or below the visible
// area (via scroll, element-resize and content-change observers).
//
// It also installs a keydown guard on the input while the list is open so the
// Arrow keys don't move the text caret and Enter doesn't implicitly submit the
// surrounding <EditForm>; the actual navigation/selection is handled in Blazor.
//
// Returns a handle exposing update() and dispose().
export function attach(scrollEl, upEl, downEl, inputEl) {
    if (!scrollEl) return null;

    const update = () => {
        // A 1px tolerance avoids sub-pixel rounding leaving an arrow stuck on.
        const canScrollUp = scrollEl.scrollTop > 0;
        const canScrollDown =
            Math.ceil(scrollEl.scrollTop + scrollEl.clientHeight) < scrollEl.scrollHeight;
        if (upEl) upEl.classList.toggle('is-visible', canScrollUp);
        if (downEl) downEl.classList.toggle('is-visible', canScrollDown);
    };

    const onKeyDown = (e) => {
        if (e.key === 'ArrowUp' || e.key === 'ArrowDown' || e.key === 'Enter') {
            e.preventDefault();
        }
    };

    scrollEl.addEventListener('scroll', update, { passive: true });
    if (inputEl) inputEl.addEventListener('keydown', onKeyDown);

    // Recompute when the list is resized or its contents (filtered items) change.
    const resizeObserver = new ResizeObserver(update);
    resizeObserver.observe(scrollEl);
    const mutationObserver = new MutationObserver(update);
    mutationObserver.observe(scrollEl, { childList: true, subtree: true });

    update();

    return {
        update,
        dispose() {
            scrollEl.removeEventListener('scroll', update);
            if (inputEl) inputEl.removeEventListener('keydown', onKeyDown);
            resizeObserver.disconnect();
            mutationObserver.disconnect();
        }
    };
}

// Scrolls the option at the given index into the visible area of the list so
// keyboard navigation keeps the highlighted item on screen.
export function scrollToIndex(scrollEl, index) {
    if (!scrollEl) return;
    const child = scrollEl.children[index];
    if (child) child.scrollIntoView({ block: 'nearest' });
}
