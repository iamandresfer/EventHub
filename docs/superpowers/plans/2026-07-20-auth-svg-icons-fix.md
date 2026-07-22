# Auth SVG Icons Fix Implementation Plan

> **For agentic workers:** Single task plan. TDD not applicable here (pure view/CSS changes).

**Goal:** Fix oversized SVG input icons on Login and Register pages by adding explicit `width="20" height="20"` attributes and proper CSS positioning.

**Architecture:** Add CSS positioning for `.input-icon` and `.input-wrapper` in the shared auth layout. Add `width="20" height="20"` to all input SVGs in Login.cshtml and Register.cshtml. Remove inline `style` padding overrides from inputs in Register.cshtml — padding will be handled by CSS.

**Tech Stack:** ASP.NET MVC 5, Razor views, CSS custom properties

## Global Constraints
- All input-icon SVGs must render at 20x20px
- Inputs must have 42px left padding for icon clearance
- Password inputs must have 48px right padding for toggle button clearance
- Must work in both light and dark mode
- Zero breaking changes to existing layout

---

### Task 1: Add CSS and fix SVGs on all auth pages

**Files:**
- Modify: `EventHub.01.Web/Views/Shared/_AuthLayout.cshtml` — add `.input-wrapper` and `.input-icon` CSS
- Modify: `EventHub.01.Web/Views/Auth/Login.cshtml` — add `width="20" height="20"` to email + lock SVGs
- Modify: `EventHub.01.Web/Views/Auth/Register.cshtml` — add `width="20" height="20"` to all 6 input SVGs, remove inline `style` padding

**Step 1: Add `.input-icon` positioning CSS to `_AuthLayout.cshtml`**

Add after line 145 (after `.form-group input:focus` rules):

```css
.input-wrapper { position: relative; }
.input-icon {
    position: absolute;
    left: 12px;
    top: 50%;
    transform: translateY(-50%);
    color: var(--text-muted);
    display: flex;
    align-items: center;
    pointer-events: none;
}
.input-icon svg { width: 20px; height: 20px; }
.input-wrapper input { padding-left: 42px; }
```

**Step 2: Fix Login.cshtml SVGs**

Add `width="20" height="20"` to the email icon (line 16):
```html
<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
```

Add `width="20" height="20"` to the lock icon (line 31):
```html
<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
```

**Step 3: Fix Register.cshtml SVGs**

Add `width="20" height="20"` to the user SVG (line 17):
```html
<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
```

Add `width="20" height="20"` to the badge SVG (line 32):
```html
<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
```

Add `width="20" height="20"` to the email SVG (line 49):
```html
<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
```

Add `width="20" height="20"` to the phone SVG (line 64):
```html
<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
```

Add `width="20" height="20"` to the password lock SVG (line 79):
```html
<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
```

Add `width="20" height="20"` to the confirm password lock SVG (line 111):
```html
<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
```

**Step 4: Remove inline padding from Register.cshtml inputs**

Remove `style="padding-left: 42px;"` from Nombre input (line 22), Alias input (line 38), Email input (line 54), Telefono input (line 69).

Remove `style="padding-left: 42px; padding-right: 48px;"` from Password input (line 84) and ConfirmPassword input (line 116) — the CSS handles left padding and right padding is already covered by existing `.password-wrapper input { padding-right: 48px; }`.
