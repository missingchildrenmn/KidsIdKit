// Bridges Ionic's "ionChange" custom event to Blazor. Blazor's @on* syntax
// only binds standard DOM events, so the listener is attached here and the
// picked value is forwarded to .NET via the supplied object reference.
//
// Returns a disposer that removes the listener.
export function attach(columnEl, dotNetRef) {
    if (!columnEl || !dotNetRef) return null;

    const onChange = (event) => {
        dotNetRef.invokeMethodAsync("UpdateValue", event.detail?.value ?? "");
    };

    columnEl.addEventListener("ionChange", onChange);

    return {
        dispose: () => columnEl.removeEventListener("ionChange", onChange)
    };
}

// Closes the picker's host modal. ion-modal has no declarative dismiss
// attribute (unlike ion-popover's dismiss-on-select), so the Done button
// calls the element's dismiss() method here.
export function dismiss(modalEl) {
    return modalEl?.dismiss();
}
