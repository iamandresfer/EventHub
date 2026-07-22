# Auth SVG Icon Sizing Fix

## Problem

SVG icons inside input fields on the Login and Register pages render too large because they lack explicit `width`/`height` attributes. The eye toggle icons already have `width="20" height="20"`, but the input icons (email, lock, user, badge, phone) do not.

## Approach

**Approach B:** Add `width="20" height="20"` to every inline SVG icon, plus CSS for proper `.input-icon` positioning.

## Files to Change

### 1. `Views/Auth/Login.cshtml`
- Add `width="20" height="20"` to email and lock SVGs (lines 16, 31)
- Remove inline `style="..."` / `padding-left` from input elements

### 2. `Views/Auth/Register.cshtml`
- Add `width="20" height="20"` to user, badge, email, phone, password, and confirm password SVGs
- Remove inline `style="padding-left: 42px;"` / `style="padding-left: 42px; padding-right: 48px;"` from all inputs

### 3. `Views/Shared/_AuthLayout.cshtml`
- Add `.input-wrapper` positioning and `.input-icon` sizing CSS:
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

## No Breaking Changes

- All existing inputs already have class `input-wrapper` parent
- All existing icons already wrapped in `span.input-icon`
- The `toggle-password` button position remains unaffected (it uses its own positioning)

## Verification

- Login page: email and lock icons render at 20x20px, aligned left inside the input
- Register page: all 6 input icons render at 20x20px
- Both pages work in light and dark mode
- Password toggle eye icons remain unchanged (already had explicit sizing)
