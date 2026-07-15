// Helpers for the ion-picker modal. The wheel's selection is read directly
// (getValue) when Done is tapped instead of listening for Ionic's
// "ionChange" event: the event only fires after the wheel's settle
// animation, so a quick tap on an option followed by Done could otherwise
// close the modal before the selection was reported.

// Rewinds the wheel to the committed value (the Blazor-rendered value
// attribute) whenever the picker reopens, so a canceled presentation (e.g.,
// backdrop dismiss) leaves no trace. Returns a disposer that removes the
// listener.
export function attach(columnEl, modalEl) {
    if (!columnEl || !modalEl) return null;

    const onPresent = () => {
        columnEl.value = columnEl.getAttribute("value");
    };
    modalEl.addEventListener("ionModalWillPresent", onPresent);

    return {
        dispose: () => modalEl.removeEventListener("ionModalWillPresent", onPresent)
    };
}

// The wheel's currently selected value, or null when nothing is selected.
export function getValue(columnEl) {
    return columnEl?.value ?? null;
}

// Closes the picker's host modal. ion-modal has no declarative dismiss
// attribute (unlike ion-popover's dismiss-on-select), so the Done button
// calls the element's dismiss() method here.
export function dismiss(modalEl) {
    return modalEl?.dismiss();
}
