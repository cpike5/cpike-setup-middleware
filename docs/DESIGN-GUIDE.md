# Design Guide

## Overview

This document defines the visual design system for the cpike.Setup.Middleware wizard UI. It is extracted from the HTML/CSS prototype and provides a reference for implementing the Blazor components while maintaining visual consistency.

## Design Principles

1. **Simplicity**: Clean, uncluttered interface that doesn't overwhelm
2. **Clarity**: Clear visual hierarchy and obvious next actions
3. **Professionalism**: Polished, modern appearance suitable for business applications
4. **Accessibility**: Keyboard navigable, screen reader friendly, sufficient contrast
5. **Responsiveness**: Works seamlessly on mobile and desktop devices

## Color Palette

### Primary Colors

```css
--primary-gradient-start: #2c3e50;  /* Deep blue-grey */
--primary-gradient-end: #3498db;    /* Bright blue */
--primary-blue: #3498db;            /* Primary action color */
```

**Usage:**

- Logo gradient
- Primary buttons
- Step progress indicators
- Active states

### Neutral Colors

```css
--background: #fafafa;              /* Page background */
--surface: #ffffff;                 /* Card/container background */
--border: #eee;                     /* Default borders */
--border-light: #e0e0e0;            /* Light borders */
--border-subtle: #e8f4f8;           /* Very subtle borders */
```

**Usage:**

- Page and component backgrounds
- Dividers and borders
- Input borders

### Text Colors

```css
--text-primary: #2c3e50;            /* Headings, important text */
--text-secondary: #555;             /* Body text */
--text-muted: #666;                 /* Helper text */
--text-disabled: #999;              /* Disabled state */
```

**Usage:**

- Headings, labels, body text
- Secondary information
- Disabled elements

### Semantic Colors

```css
--success: #28a745;                 /* Success states */
--warning: #ffc107;                 /* Warning states */
--error: #dc3545;                   /* Error states */
--info: #17a2b8;                    /* Informational states */

--success-bg: #d4edda;
--success-text: #155724;

--warning-bg: #fff3cd;
--warning-text: #856404;

--error-bg: #f8d7da;
--error-text: #721c24;

--info-bg: #d1ecf1;
--info-text: #0c5460;
```

**Usage:**

- Alert components
- Validation feedback
- Status indicators
- Password strength meter

## Typography

### Font Stack

```css
font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
```

**Reasoning:** Native system fonts for fast loading and familiar appearance across platforms.

### Type Scale

| Element | Size | Weight | Line Height | Usage |
|---------|------|--------|-------------|-------|
| Logo | 2rem (32px) | 400 | 1.2 | Application logo |
| Wizard Title | 1.5rem (24px) | 600 | 1.3 | Main wizard title |
| Step Title | 1.25rem (20px) | 600 | 1.3 | Individual step titles |
| Section Title | 1rem (16px) | 600 | 1.4 | Section headings |
| Body | 1rem (16px) | 400 | 1.6 | Body text, inputs |
| Small | 0.95rem (15px) | 400 | 1.6 | Descriptions |
| Caption | 0.875rem (14px) | 400 | 1.5 | Helper text |
| Label | 0.9rem (14.4px) | 500 | 1.4 | Form labels |
| Tiny | 0.75rem (12px) | 400 | 1.5 | Step labels, small hints |

### Font Variants

**Logo**: Small-caps for distinctive branding

```css
.logo {
    font-variant: small-caps;
    letter-spacing: 0.1em;
}
```

## Spacing System

### Base Unit: 0.5rem (8px)

All spacing uses multiples of the base unit for consistency.

| Variable | Value | Usage |
|----------|-------|-------|
| `--space-xs` | 0.25rem (4px) | Tight spacing |
| `--space-sm` | 0.5rem (8px) | Small gaps |
| `--space-md` | 1rem (16px) | Default spacing |
| `--space-lg` | 1.5rem (24px) | Section spacing |
| `--space-xl` | 2rem (32px) | Large spacing |

### Component Spacing

**Padding:**

- Wizard header: `2rem 2rem 1rem 2rem`
- Wizard body: `2rem`
- Wizard footer: `1.5rem 2rem`
- Form inputs: `0.75rem`
- Buttons: `0.6rem 1.5rem`

**Margins:**

- Form groups: `1.5rem` bottom
- Section spacing: `1.5rem` bottom
- Alert spacing: `1rem` bottom

## Layout

### Wizard Container

```css
max-width: 700px;
width: 100%;
background: white;
border-radius: 8px;
box-shadow: 0 2px 20px rgba(0, 0, 0, 0.05);
```

**Positioning:**

- Centered horizontally and vertically on page
- Page background: `#fafafa`
- Minimum padding: `2rem` around container

### Grid System

No formal grid system. Uses flexbox for layouts:

- **Progress steps**: `display: flex; justify-content: space-between;`
- **Form groups**: Vertical stacking with bottom margin
- **Button groups**: `display: flex; gap: 1rem;`

## Components

### 1. Logo

**Visual:**

- Small-caps text with gradient color
- Letter spacing: `0.1em`
- Size: `2rem`

**Gradient:**

```css
background: linear-gradient(135deg, #2c3e50, #3498db);
-webkit-background-clip: text;
-webkit-text-fill-color: transparent;
```

---

### 2. Wizard Header

**Structure:**

- Logo
- Title (1.5rem, weight 600)
- Subtitle (0.95rem, color #666)

**Style:**

- Padding: `2rem 2rem 1rem 2rem`
- Text align: center
- Border bottom: `1px solid #eee`

---

### 3. Progress Indicator

**Components:**

1. **Progress Line** (background bar)
   - Height: `2px`
   - Color: `#e0e0e0`
   - Position: Absolute, behind circles

2. **Progress Fill** (active portion)
   - Height: `2px`
   - Gradient: `linear-gradient(135deg, #2c3e50, #3498db)`
   - Width: Percentage based on progress
   - Transition: `width 0.3s ease`

3. **Step Circles**
   - Size: `32px` diameter
   - Border: `2px solid`
   - Background: `white`
   - Font size: `0.875rem`, weight 600

**States:**

| State | Border | Background | Text |
|-------|--------|------------|------|
| Upcoming | `#e0e0e0` | `white` | `#999` |
| Active | `#3498db` | `#3498db` | `white` |
| Active (glow) | `#3498db` | `#3498db` | `white` + `box-shadow: 0 0 0 4px rgba(52, 152, 219, 0.1)` |
| Completed | `#3498db` | `#3498db` | `white` (shows checkmark ✓) |

**Step Labels:**

- Font size: `0.75rem`
- Margin top: `0.5rem`
- Color: `#999` (upcoming), `#3498db` (active), `#666` (completed)
- Hidden on mobile

---

### 4. Form Inputs

**Text/Email/Password Inputs:**

```css
width: 100%;
padding: 0.75rem;
border: 1px solid #ddd;
border-radius: 4px;
font-size: 1rem;
transition: border-color 0.3s ease;
```

**Focus State:**

```css
border-color: #3498db;
box-shadow: 0 0 0 2px rgba(52, 152, 219, 0.1);
outline: none;
```

**Label:**

```css
display: block;
margin-bottom: 0.5rem;
color: #555;
font-size: 0.9rem;
font-weight: 500;
```

**Hint Text:**

```css
font-size: 0.875rem;
color: #999;
margin-top: 0.25rem;
```

---

### 5. Password Strength Indicator

**Structure:**

- Background bar (height: 4px, color: `#e0e0e0`)
- Foreground bar (height: 4px, width: %, transition: 0.3s)
- Text label (font-size: 0.75rem, color: `#666`)

**States:**

| Strength | Width | Color |
|----------|-------|-------|
| Weak | 33% | `#dc3545` |
| Medium | 66% | `#ffc107` |
| Strong | 100% | `#28a745` |

---

### 6. Checkbox

**Container:**

```css
display: flex;
align-items: flex-start;
padding: 0.75rem;
border: 1px solid #e8f4f8;
border-radius: 4px;
margin-bottom: 0.5rem;
transition: all 0.3s ease;
```

**Hover:**

```css
border-color: #3498db;
background: rgba(52, 152, 219, 0.02);
```

**Checkbox Input:**

- Size: `18px`
- Margin right: `0.75rem`

**Label:**

- Font weight: 500
- Color: `#2c3e50`

**Description:**

- Font size: `0.875rem`
- Color: `#666`

---

### 7. Toggle Switch

**Container:**

```css
display: flex;
align-items: center;
justify-content: space-between;
padding: 1rem;
border: 1px solid #e8f4f8;
border-radius: 4px;
```

**Switch:**

- Width: `48px`
- Height: `24px`
- Border radius: `24px`
- Background: `#ccc` (off), `#3498db` (on)
- Transition: `0.3s`

**Switch Handle:**

- Size: `18px`
- Position: `3px` from edge
- Background: `white`
- Transform: `translateX(0)` (off), `translateX(24px)` (on)

---

### 8. Buttons

**Primary Button:**

```css
background: #3498db;
color: white;
border: none;
border-radius: 4px;
padding: 0.6rem 1.5rem;
font-weight: 500;
font-size: 1rem;
cursor: pointer;
transition: all 0.3s ease;
```

**Primary Hover:**

```css
transform: translateY(-1px);
box-shadow: 0 4px 15px rgba(52, 152, 219, 0.3);
```

**Secondary Button:**

```css
background: #f8f9fa;
color: #555;
border: 1px solid #ddd;
```

**Secondary Hover:**

```css
background: #e9ecef;
border-color: #ccc;
transform: translateY(-1px);
box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
```

**Outline Button:**

```css
background: transparent;
color: #3498db;
border: 1px solid #3498db;
```

**Outline Hover:**

```css
background: #3498db;
color: white;
transform: translateY(-1px);
```

**Large Button:**

```css
padding: 0.8rem 2rem;
font-size: 1.125rem;
```

**Disabled State:**

```css
opacity: 0.5;
cursor: not-allowed;
transform: none !important; /* Override hover */
```

---

### 9. Alert Component

**Base:**

```css
padding: 1rem;
border-radius: 4px;
margin-bottom: 1rem;
border-left: 4px solid;
```

**Variants:**

| Variant | Background | Text | Border |
|---------|------------|------|--------|
| Info | `#d1ecf1` | `#0c5460` | `#17a2b8` |
| Success | `#d4edda` | `#155724` | `#28a745` |
| Warning | `#fff3cd` | `#856404` | `#ffc107` |
| Error | `#f8d7da` | `#721c24` | `#dc3545` |

---

### 10. Summary Section (Review)

**Section:**

- Margin bottom: `1.5rem`

**Section Title:**

- Font weight: 600
- Color: `#2c3e50`
- Margin bottom: `0.75rem`
- Font size: `1rem`

**Summary Item:**

```css
display: flex;
justify-content: space-between;
padding: 0.5rem 0;
border-bottom: 1px solid #eee;
```

**Summary Label:**

- Color: `#666`
- Font size: `0.9rem`

**Summary Value:**

- Color: `#2c3e50`
- Font weight: 500
- Font size: `0.9rem`

---

### 11. Success Display

**Icon:**

```css
width: 80px;
height: 80px;
background: linear-gradient(135deg, #2c3e50, #3498db);
border-radius: 50%;
display: flex;
align-items: center;
justify-content: center;
margin: 0 auto 1.5rem auto;
color: white;
font-size: 3rem;
```

**Code Display (for credentials):**

```css
background: #f8f9fa;
border: 1px solid #e0e0e0;
border-radius: 4px;
padding: 1rem;
margin: 1rem 0;
font-family: 'Courier New', monospace;
font-size: 0.95rem;
position: relative;
```

**Code Value:**

```css
color: #3498db;
user-select: all;
word-break: break-all;
```

**Copy Button:**

```css
position: absolute;
top: 0.5rem;
right: 0.5rem;
background: white;
border: 1px solid #ddd;
border-radius: 4px;
padding: 0.25rem 0.5rem;
font-size: 0.75rem;
cursor: pointer;
transition: all 0.3s ease;
```

**Copy Button Hover:**

```css
background: #3498db;
color: white;
border-color: #3498db;
```

---

### 12. Feature List

**Feature Item:**

```css
display: flex;
align-items: start;
margin-bottom: 1rem;
padding: 0.75rem;
background: #f8f9fa;
border-radius: 4px;
```

**Feature Icon:**

```css
width: 24px;
height: 24px;
background: #3498db;
color: white;
border-radius: 50%;
display: flex;
align-items: center;
justify-content: center;
margin-right: 0.75rem;
flex-shrink: 0;
font-size: 0.875rem;
```

**Feature Text:**

```css
color: #555;
font-size: 0.95rem;
```

## Animations

### Fade In (Step Transition)

```css
@keyframes fadeIn {
    from {
        opacity: 0;
        transform: translateY(10px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}
```

**Usage:** Apply to active wizard step

```css
animation: fadeIn 0.3s ease;
```

### Transitions

| Element | Property | Duration | Easing |
|---------|----------|----------|--------|
| Buttons | all | 0.3s | ease |
| Inputs | border-color | 0.3s | ease |
| Progress fill | width | 0.3s | ease |
| Step circles | all | 0.3s | ease |
| Toggle switch | all | 0.3s | ease |
| Password strength | width, background | 0.3s | ease |

## Responsive Design

### Breakpoints

**Mobile:** `max-width: 768px`

### Mobile Adjustments

**Wizard Container:**

```css
margin: 1rem; /* Reduced from 2rem */
```

**Wizard Body:**

```css
padding: 1.5rem; /* Reduced from 2rem */
```

**Wizard Header:**

```css
padding: 1.5rem; /* Reduced from 2rem 2rem 1rem */
```

**Progress Steps:**

```css
flex-wrap: wrap; /* Allow wrapping if needed */
```

**Step Labels:**

```css
display: none; /* Hide to save space */
```

**Wizard Footer:**

```css
flex-direction: column;
gap: 1rem;
```

**Wizard Actions:**

```css
width: 100%;
flex-direction: column;
```

**Buttons:**

```css
width: 100%;
```

## Shadows

**Wizard Container:**

```css
box-shadow: 0 2px 20px rgba(0, 0, 0, 0.05);
```

**Button Hover:**

```css
/* Primary */
box-shadow: 0 4px 15px rgba(52, 152, 219, 0.3);

/* Secondary */
box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
```

**Active Step Glow:**

```css
box-shadow: 0 0 0 4px rgba(52, 152, 219, 0.1);
```

## Accessibility

### Focus Indicators

**All interactive elements must have visible focus states:**

```css
button:focus,
input:focus,
a:focus {
    outline: 2px solid #3498db;
    outline-offset: 2px;
}
```

### Contrast Ratios

All text meets WCAG 2.1 Level AA requirements:

| Text | Background | Ratio | Standard |
|------|------------|-------|----------|
| `#2c3e50` | `#ffffff` | 12.6:1 | AAA |
| `#555` | `#ffffff` | 8.6:1 | AAA |
| `#666` | `#ffffff` | 7.0:1 | AAA |
| `#999` | `#ffffff` | 3.2:1 | AA (large text) |

### ARIA Labels

All components should include:

- `aria-label` or `aria-labelledby`
- `role` attributes where applicable
- `aria-live` regions for dynamic content
- `aria-invalid` for validation errors
- `aria-describedby` for helper text

### Keyboard Navigation

- All interactive elements reachable via Tab
- Enter/Space activates buttons
- Escape closes modals/dialogs
- Arrow keys navigate grouped controls

## Implementation Notes

### CSS Custom Properties

For easy customization, implement using CSS custom properties:

```css
:root {
    /* Colors */
    --primary-gradient-start: #2c3e50;
    --primary-gradient-end: #3498db;
    --primary-blue: #3498db;

    /* Spacing */
    --space-xs: 0.25rem;
    --space-sm: 0.5rem;
    --space-md: 1rem;
    --space-lg: 1.5rem;
    --space-xl: 2rem;

    /* Typography */
    --font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
    --font-size-base: 1rem;

    /* Borders */
    --border-radius: 4px;
    --border-color: #ddd;
}
```

### Component CSS Classes

Use BEM naming convention:

```
.setup-wizard {}
.setup-wizard__header {}
.setup-wizard__body {}
.setup-wizard__footer {}

.progress-indicator {}
.progress-indicator__line {}
.progress-indicator__fill {}
.progress-indicator__step {}
.progress-indicator__circle {}
.progress-indicator__label {}

.setup-input {}
.setup-input__label {}
.setup-input__field {}
.setup-input__hint {}
.setup-input__error {}

.setup-button {}
.setup-button--primary {}
.setup-button--secondary {}
.setup-button--outline {}
.setup-button--large {}
```

### Dark Mode Considerations (Future)

While not in MVP scope, design allows for dark mode by swapping CSS custom properties:

```css
@media (prefers-color-scheme: dark) {
    :root {
        --background: #1a1a1a;
        --surface: #2d2d2d;
        --text-primary: #ffffff;
        --text-secondary: #cccccc;
        --border: #444444;
        /* etc. */
    }
}
```

## Design Assets

### Logo

Text-based logo using gradient. No image assets required.

**Text:** "cpike.ca" (or application name)
**Style:** Small-caps, gradient fill

### Icons

Use Unicode characters or minimal SVG icons:

- Checkmark: ✓ or `<svg>...</svg>`
- Arrow right: → or `<svg>...</svg>`
- Arrow left: ← or `<svg>...</svg>`

For production, consider icon library like:

- Heroicons
- Feather Icons
- Bootstrap Icons

## Browser Support

**Target:** Modern browsers (last 2 versions)

- Chrome 100+
- Firefox 100+
- Safari 15+
- Edge 100+

**Graceful degradation for:**

- No CSS Grid fallback (uses Flexbox)
- No CSS custom properties fallback (uses hardcoded values)
- No backdrop-filter fallback

## Performance

### CSS Optimization

- Use GPU-accelerated properties for animations (`transform`, `opacity`)
- Minimize reflows/repaints
- Use `will-change` sparingly for critical animations

### Loading Strategy

- Inline critical CSS for first paint
- Defer non-critical styles
- Use font-display: swap for web fonts (if any)

## References

- Prototype: [first-time-setup-wizard.html](../docs/reference/first-time-setup-wizard.html)
- WCAG 2.1 Guidelines: https://www.w3.org/WAI/WCAG21/quickref/
- Material Design: https://material.io/design (inspiration, not strict adherence)
