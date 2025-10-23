# CSS Customization Guide

## Overview

This guide explains how to customize the visual appearance of the setup wizard to match your application's branding and design system.

## Table of Contents

- [Quick Start](#quick-start)
- [CSS Custom Properties](#css-custom-properties)
- [Overriding Specific Components](#overriding-specific-components)
- [Complete Theme Replacement](#complete-theme-replacement)
- [Dark Mode](#dark-mode)
- [Advanced Customization](#advanced-customization)

## Quick Start

### Method 1: CSS Custom Properties (Recommended)

The easiest way to customize the wizard is to override CSS custom properties (CSS variables):

**wwwroot/css/site.css:**

```css
:root {
    /* Override primary colors */
    --setup-primary-color: #your-brand-color;
    --setup-primary-gradient-start: #your-color-1;
    --setup-primary-gradient-end: #your-color-2;

    /* Override fonts */
    --setup-font-family: 'Your Font', sans-serif;

    /* Override spacing */
    --setup-border-radius: 8px;
}
```

### Method 2: Override Specific CSS Classes

Create a custom stylesheet that overrides specific component styles:

**wwwroot/css/setup-custom.css:**

```css
/* Override wizard container */
.setup-wizard {
    max-width: 900px;
    box-shadow: 0 4px 30px rgba(0, 0, 0, 0.1);
}

/* Override primary button color */
.setup-button--primary {
    background: #your-brand-color;
}
```

**_Host.cshtml or App.razor:**

```html
<link rel="stylesheet" href="css/setup-custom.css" />
```

## CSS Custom Properties

### Complete List of Variables

The wizard uses CSS custom properties for all major styling decisions. Override any of these in your stylesheet:

#### Colors

```css
:root {
    /* Primary Colors */
    --setup-primary-gradient-start: #2c3e50;
    --setup-primary-gradient-end: #3498db;
    --setup-primary-blue: #3498db;
    --setup-primary-hover: #2980b9;

    /* Backgrounds */
    --setup-background: #fafafa;
    --setup-surface: #ffffff;
    --setup-surface-alt: #f8f9fa;

    /* Borders */
    --setup-border: #eee;
    --setup-border-light: #e0e0e0;
    --setup-border-subtle: #e8f4f8;

    /* Text */
    --setup-text-primary: #2c3e50;
    --setup-text-secondary: #555;
    --setup-text-muted: #666;
    --setup-text-disabled: #999;

    /* Semantic Colors */
    --setup-success: #28a745;
    --setup-success-bg: #d4edda;
    --setup-success-text: #155724;

    --setup-warning: #ffc107;
    --setup-warning-bg: #fff3cd;
    --setup-warning-text: #856404;

    --setup-error: #dc3545;
    --setup-error-bg: #f8d7da;
    --setup-error-text: #721c24;

    --setup-info: #17a2b8;
    --setup-info-bg: #d1ecf1;
    --setup-info-text: #0c5460;
}
```

#### Typography

```css
:root {
    --setup-font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
    --setup-font-size-base: 1rem;
    --setup-font-size-sm: 0.875rem;
    --setup-font-size-lg: 1.125rem;
    --setup-font-size-xl: 1.5rem;

    --setup-font-weight-normal: 400;
    --setup-font-weight-medium: 500;
    --setup-font-weight-semibold: 600;

    --setup-line-height-base: 1.6;
    --setup-line-height-heading: 1.3;
}
```

#### Spacing

```css
:root {
    --setup-space-xs: 0.25rem;
    --setup-space-sm: 0.5rem;
    --setup-space-md: 1rem;
    --setup-space-lg: 1.5rem;
    --setup-space-xl: 2rem;
}
```

#### Borders & Shadows

```css
:root {
    --setup-border-radius: 4px;
    --setup-border-radius-lg: 8px;
    --setup-border-radius-pill: 24px;

    --setup-shadow-sm: 0 2px 8px rgba(0, 0, 0, 0.05);
    --setup-shadow-md: 0 2px 20px rgba(0, 0, 0, 0.05);
    --setup-shadow-lg: 0 4px 30px rgba(0, 0, 0, 0.1);

    --setup-shadow-button-hover: 0 4px 15px rgba(52, 152, 219, 0.3);
}
```

#### Transitions

```css
:root {
    --setup-transition-fast: 0.15s ease;
    --setup-transition-base: 0.3s ease;
    --setup-transition-slow: 0.5s ease;
}
```

### Example: Brand Color Customization

```css
/* Override primary color throughout the wizard */
:root {
    --setup-primary-blue: #9c27b0;  /* Purple */
    --setup-primary-gradient-start: #7b1fa2;
    --setup-primary-gradient-end: #ba68c8;
}
```

This will update:

- Primary buttons
- Progress indicators
- Active step circles
- Links and interactive elements
- Logo gradient

## Overriding Specific Components

### Component CSS Class Reference

All components use BEM (Block Element Modifier) naming:

```
.setup-wizard
.setup-wizard__header
.setup-wizard__body
.setup-wizard__footer

.progress-indicator
.progress-indicator__line
.progress-indicator__fill
.progress-indicator__step
.progress-indicator__circle
.progress-indicator__label

.setup-button
.setup-button--primary
.setup-button--secondary
.setup-button--outline
.setup-button--large

.setup-input
.setup-input__label
.setup-input__field
.setup-input__hint
.setup-input__error

.setup-checkbox
.setup-checkbox__input
.setup-checkbox__label
.setup-checkbox__description

.setup-toggle
.setup-toggle__switch
.setup-toggle__slider

.setup-alert
.setup-alert--info
.setup-alert--success
.setup-alert--warning
.setup-alert--error
```

### Example: Customize Wizard Container

```css
/* Make wizard larger */
.setup-wizard {
    max-width: 900px;
}

/* Add more dramatic shadow */
.setup-wizard {
    box-shadow: 0 10px 50px rgba(0, 0, 0, 0.15);
}

/* Rounded corners */
.setup-wizard {
    border-radius: 16px;
}

/* Add border */
.setup-wizard {
    border: 2px solid var(--setup-primary-blue);
}
```

### Example: Customize Buttons

```css
/* Square buttons instead of rounded */
.setup-button {
    border-radius: 0;
}

/* Larger buttons */
.setup-button {
    padding: 1rem 2rem;
    font-size: 1.125rem;
}

/* Different hover effect */
.setup-button--primary:hover {
    transform: scale(1.05);
    box-shadow: 0 6px 20px rgba(52, 152, 219, 0.4);
}

/* Outline primary button */
.setup-button--primary {
    background: transparent;
    color: var(--setup-primary-blue);
    border: 2px solid var(--setup-primary-blue);
}

.setup-button--primary:hover {
    background: var(--setup-primary-blue);
    color: white;
}
```

### Example: Customize Progress Indicator

```css
/* Larger step circles */
.progress-indicator__circle {
    width: 40px;
    height: 40px;
    font-size: 1rem;
}

/* Different progress bar color */
.progress-indicator__fill {
    background: linear-gradient(90deg, #ff6b6b, #ff8e53);
}

/* Always show step labels (even on mobile) */
.progress-indicator__label {
    display: block !important;
}
```

### Example: Customize Form Inputs

```css
/* Larger input fields */
.setup-input__field {
    padding: 1rem;
    font-size: 1.125rem;
}

/* Different focus color */
.setup-input__field:focus {
    border-color: #9c27b0;
    box-shadow: 0 0 0 3px rgba(156, 39, 176, 0.1);
}

/* Rounded inputs */
.setup-input__field {
    border-radius: 24px;
}
```

### Example: Customize Alerts

```css
/* Remove left border accent */
.setup-alert {
    border-left: none;
    border: 1px solid;
}

/* Rounded alerts */
.setup-alert {
    border-radius: 8px;
}

/* Add icon space */
.setup-alert::before {
    content: "ℹ️";
    margin-right: 0.5rem;
}

.setup-alert--success::before {
    content: "✅";
}

.setup-alert--warning::before {
    content: "⚠️";
}

.setup-alert--error::before {
    content: "❌";
}
```

## Complete Theme Replacement

If you want complete control, you can replace the entire stylesheet:

### Step 1: Disable Default Styles

In your `Program.cs` or setup configuration:

```csharp
builder.Services.AddSetupWizard(setup =>
{
    setup.UseDefaultStyles = false; // Disable built-in styles
    setup.AddStep<MyStep>();
});
```

### Step 2: Create Your Own Stylesheet

Create `wwwroot/css/setup-wizard-custom.css` with your complete styles.

You can use the [Design Guide](DESIGN-GUIDE.md) as a reference for required classes and structure.

### Step 3: Include Your Stylesheet

**_Host.cshtml or App.razor:**

```html
<link rel="stylesheet" href="css/setup-wizard-custom.css" />
```

## Dark Mode

### Automatic Dark Mode

Add dark mode support using `prefers-color-scheme`:

```css
@media (prefers-color-scheme: dark) {
    :root {
        --setup-background: #1a1a1a;
        --setup-surface: #2d2d2d;
        --setup-surface-alt: #3a3a3a;

        --setup-border: #444444;
        --setup-border-light: #555555;

        --setup-text-primary: #ffffff;
        --setup-text-secondary: #cccccc;
        --setup-text-muted: #999999;

        --setup-primary-blue: #64b5f6;

        /* Adjust semantic colors for dark backgrounds */
        --setup-success-bg: #1b5e20;
        --setup-success-text: #a5d6a7;

        --setup-warning-bg: #f57f17;
        --setup-warning-text: #fff9c4;

        --setup-error-bg: #c62828;
        --setup-error-text: #ffcdd2;

        --setup-info-bg: #01579b;
        --setup-info-text: #b3e5fc;
    }

    /* Adjust wizard shadow for dark mode */
    .setup-wizard {
        box-shadow: 0 2px 20px rgba(0, 0, 0, 0.5);
    }

    /* Adjust input styling for dark mode */
    .setup-input__field {
        background: var(--setup-surface-alt);
        color: var(--setup-text-primary);
        border-color: var(--setup-border);
    }
}
```

### Manual Dark Mode Toggle

```css
/* Default: light mode */
:root {
    /* ... light mode variables ... */
}

/* Dark mode: apply when .dark-mode class on body */
body.dark-mode {
    --setup-background: #1a1a1a;
    --setup-surface: #2d2d2d;
    /* ... other dark mode variables ... */
}
```

**Toggle with JavaScript:**

```javascript
// Toggle dark mode
document.body.classList.toggle('dark-mode');
```

## Advanced Customization

### Custom Fonts

#### Using Google Fonts

**_Host.cshtml:**

```html
<link rel="preconnect" href="https://fonts.googleapis.com">
<link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600&display=swap" rel="stylesheet">
```

**CSS:**

```css
:root {
    --setup-font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
}
```

#### Using Self-Hosted Fonts

```css
@font-face {
    font-family: 'Your Font';
    src: url('/fonts/your-font.woff2') format('woff2');
    font-weight: 400;
    font-display: swap;
}

:root {
    --setup-font-family: 'Your Font', sans-serif;
}
```

### Custom Logo

Replace the text logo with an image:

```css
.setup-wizard__logo-text {
    display: none;
}

.setup-wizard__logo::before {
    content: "";
    display: block;
    width: 200px;
    height: 60px;
    background: url('/images/your-logo.svg') no-repeat center;
    background-size: contain;
    margin: 0 auto 1rem;
}
```

### Custom Animations

```css
/* Custom fade-in animation for steps */
@keyframes slideInRight {
    from {
        opacity: 0;
        transform: translateX(20px);
    }
    to {
        opacity: 1;
        transform: translateX(0);
    }
}

.wizard-step.active {
    animation: slideInRight 0.4s ease;
}

/* Custom button hover animation */
.setup-button--primary {
    position: relative;
    overflow: hidden;
}

.setup-button--primary::before {
    content: "";
    position: absolute;
    top: 50%;
    left: 50%;
    width: 0;
    height: 0;
    border-radius: 50%;
    background: rgba(255, 255, 255, 0.3);
    transform: translate(-50%, -50%);
    transition: width 0.6s, height 0.6s;
}

.setup-button--primary:hover::before {
    width: 300px;
    height: 300px;
}
```

### Responsive Customization

```css
/* Different max-width on large screens */
@media (min-width: 1200px) {
    .setup-wizard {
        max-width: 1000px;
    }
}

/* Different button styles on mobile */
@media (max-width: 768px) {
    .setup-button {
        padding: 0.75rem 1rem;
        font-size: 0.95rem;
    }
}

/* Always show step labels on larger mobile devices */
@media (min-width: 640px) and (max-width: 768px) {
    .progress-indicator__label {
        display: block !important;
        font-size: 0.65rem;
    }
}
```

### Integration with CSS Frameworks

#### Tailwind CSS

If using Tailwind, you can mix utility classes with component classes:

```razor
<div class="setup-wizard tw-shadow-2xl tw-rounded-xl">
    <!-- wizard content -->
</div>
```

Or override with Tailwind variables:

```css
:root {
    --setup-primary-blue: theme('colors.blue.500');
    --setup-success: theme('colors.green.500');
    --setup-border-radius: theme('borderRadius.lg');
}
```

#### Bootstrap

If using Bootstrap, ensure wizard styles don't conflict:

```css
/* Scope wizard styles to avoid Bootstrap conflicts */
.setup-wizard .btn {
    /* Reset Bootstrap button styles */
    all: unset;
}

/* Or namespace all wizard styles */
.cpike-setup .setup-wizard {
    /* Wizard styles here */
}
```

### RTL (Right-to-Left) Support

```css
[dir="rtl"] .setup-wizard__footer {
    flex-direction: row-reverse;
}

[dir="rtl"] .progress-indicator__circle {
    /* Adjust for RTL */
}

[dir="rtl"] .setup-checkbox {
    flex-direction: row-reverse;
}

[dir="rtl"] .setup-checkbox__input {
    margin-right: 0;
    margin-left: 0.75rem;
}
```

## Best Practices

### 1. Use CSS Custom Properties

Prefer CSS variables over hardcoded values for easy theming:

```css
/* ✅ Good */
.my-custom-element {
    color: var(--setup-primary-blue);
    padding: var(--setup-space-md);
}

/* ❌ Avoid */
.my-custom-element {
    color: #3498db;
    padding: 1rem;
}
```

### 2. Maintain Specificity

Avoid overly specific selectors:

```css
/* ✅ Good */
.setup-button--primary {
    background: var(--my-brand-color);
}

/* ❌ Avoid */
.setup-wizard .setup-wizard__footer .setup-button.setup-button--primary {
    background: var(--my-brand-color);
}
```

### 3. Test Responsiveness

Always test customizations on mobile and desktop:

```css
/* Test at these common breakpoints */
@media (max-width: 640px)  { /* Mobile */ }
@media (max-width: 768px)  { /* Tablet */ }
@media (max-width: 1024px) { /* Small laptop */ }
@media (min-width: 1280px) { /* Desktop */ }
```

### 4. Preserve Accessibility

When customizing, maintain accessibility:

```css
/* ✅ Maintain sufficient contrast */
.setup-button--primary {
    background: #1a73e8;
    color: #ffffff;  /* Contrast ratio: 4.5:1+ */
}

/* ✅ Keep focus indicators visible */
.setup-input__field:focus {
    outline: 2px solid var(--setup-primary-blue);
    outline-offset: 2px;
}

/* ❌ Don't remove focus indicators */
*:focus {
    outline: none; /* Bad for accessibility */
}
```

### 5. Document Customizations

Add comments explaining non-obvious customizations:

```css
/* Override: Using brand colors from design system */
:root {
    --setup-primary-blue: #1a73e8; /* Google Blue 500 */
    --setup-success: #34a853;      /* Google Green 500 */
}

/* Override: Larger wizard for dashboard context */
.setup-wizard {
    max-width: 900px; /* Wider than default 700px */
}
```

## Examples

### Example 1: Corporate Theme

```css
:root {
    /* Corporate blue/grey theme */
    --setup-primary-blue: #005eb8;
    --setup-primary-gradient-start: #003d7a;
    --setup-primary-gradient-end: #005eb8;

    --setup-surface: #ffffff;
    --setup-background: #f5f7fa;

    --setup-font-family: 'Arial', sans-serif;
    --setup-border-radius: 2px; /* Sharp corners */
}

.setup-wizard {
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1); /* Subtle shadow */
}

.setup-button {
    text-transform: uppercase; /* UPPERCASE BUTTONS */
    letter-spacing: 0.05em;
    font-weight: 600;
}
```

### Example 2: Playful/Modern Theme

```css
:root {
    --setup-primary-blue: #7c3aed;
    --setup-primary-gradient-start: #ec4899;
    --setup-primary-gradient-end: #8b5cf6;

    --setup-font-family: 'Comic Sans MS', 'Poppins', sans-serif;
    --setup-border-radius: 16px; /* Very rounded */

    --setup-shadow-md: 0 10px 40px rgba(124, 58, 237, 0.2);
}

.setup-wizard {
    background: linear-gradient(135deg, #fef3ff, #ede9fe);
}

.setup-button--primary {
    background: linear-gradient(135deg, #ec4899, #8b5cf6);
    border-radius: 24px;
}

.progress-indicator__circle {
    width: 40px;
    height: 40px;
}
```

### Example 3: Minimal Theme

```css
:root {
    --setup-primary-blue: #000000;
    --setup-text-primary: #000000;
    --setup-text-secondary: #666666;
    --setup-border: #e0e0e0;

    --setup-space-md: 0.75rem;
    --setup-space-lg: 1.25rem;
}

.setup-wizard {
    box-shadow: none;
    border: 1px solid var(--setup-border);
    border-radius: 0;
}

.setup-button {
    border-radius: 0;
}

.setup-button--primary {
    background: #000000;
    color: #ffffff;
}

.progress-indicator__fill {
    background: #000000;
}

.progress-indicator__circle {
    border-radius: 0; /* Square circles */
}
```

## Troubleshooting

### Styles Not Applying

1. **Check specificity**: Your selector might not be specific enough
   ```css
   /* Instead of */
   .setup-button { }

   /* Try */
   .setup-wizard .setup-button { }
   ```

2. **Check load order**: Ensure your custom CSS loads after the default styles
   ```html
   <link rel="stylesheet" href="_content/cpike.Setup.Middleware/css/setup.css" />
   <link rel="stylesheet" href="css/setup-custom.css" />
   ```

3. **Use `!important` sparingly**: Only for overriding third-party styles
   ```css
   .setup-button--primary {
       background: var(--my-brand-color) !important;
   }
   ```

### CSS Variables Not Working

Check browser support. CSS custom properties work in all modern browsers but not IE11.

**Fallback for older browsers:**

```css
.setup-button--primary {
    background: #3498db; /* Fallback */
    background: var(--setup-primary-blue); /* Modern browsers */
}
```

### Responsive Styles Not Working

Check media query syntax and order:

```css
/* Base styles */
.setup-wizard {
    max-width: 700px;
}

/* Mobile styles (should come after base) */
@media (max-width: 768px) {
    .setup-wizard {
        max-width: 100%;
    }
}
```

## Resources

- [Design Guide](DESIGN-GUIDE.md) - Complete design system reference
- [MDN: CSS Custom Properties](https://developer.mozilla.org/en-US/docs/Web/CSS/Using_CSS_custom_properties)
- [MDN: CSS Specificity](https://developer.mozilla.org/en-US/docs/Web/CSS/Specificity)
- [WCAG Color Contrast Checker](https://webaim.org/resources/contrastchecker/)
