# Setup Wizard Prototypes

## Available Prototypes

### Version 2 (Recommended) - `setup-wizard-v2.html`

**Status:** ✅ Current - Fully implements documented design system

**Features:**
- ✅ Complete CSS custom properties (design tokens)
- ✅ BEM naming convention for CSS classes
- ✅ Password access screen (Step 0)
- ✅ All documented components implemented
- ✅ Fully responsive design
- ✅ Semantic HTML structure
- ✅ Clean, maintainable code
- ✅ Follows DESIGN-GUIDE.md specifications

**Components Included:**
- Password access screen with lock icon
- Welcome screen with feature list
- Admin account creation with password strength
- System roles with checkboxes
- Review/confirmation screen
- Success screen with credentials display
- Progress indicator with 6 steps
- All form components (input, checkbox, toggle)
- Alert components (info, warning, success, error)
- Button variants (primary, secondary, large)

**New in V2:**
- CSS custom properties for easy theming
- Password protection step
- BEM class naming for better maintainability
- Improved semantic structure
- Better accessibility (ARIA-ready class names)
- Matches documented design system exactly

**File:** [setup-wizard-v2.html](setup-wizard-v2.html)

---

### Version 1 (Original) - `first-time-setup-wizard.html`

**Status:** 📦 Original concept prototype

**Features:**
- ✅ Original design concept
- ✅ Basic functionality demonstration
- ✅ 5-step wizard (no password step)
- ✅ Inline styles

**File:** [first-time-setup-wizard.html](first-time-setup-wizard.html)

**Note:** This was the initial prototype used to document the design system. V2 is the refined implementation.

---

## Key Differences

| Feature | V1 (Original) | V2 (Current) |
|---------|---------------|--------------|
| CSS Variables | ❌ No | ✅ Yes (complete design tokens) |
| BEM Naming | ❌ No | ✅ Yes |
| Password Step | ❌ No | ✅ Yes |
| Design Guide Match | ⚠️ Partial | ✅ Exact |
| Maintainability | ⚠️ Medium | ✅ High |
| Themability | ❌ Hard | ✅ Easy |
| Production Ready | ❌ No | ✅ Yes |

## Usage

### For Development Reference

Use **V2** (`setup-wizard-v2.html`) as the reference implementation when building the Blazor components.

**Why V2?**
- Matches the documented design system exactly
- Uses CSS custom properties for theming
- BEM naming makes component extraction easier
- Includes all documented features (including password protection)
- Production-ready code quality

### For Design Preview

Both versions can be used to preview the wizard, but V2 is recommended for:
- Stakeholder demonstrations
- Design reviews
- User testing
- Documentation screenshots

### Opening the Prototype

```bash
# Open in browser (from project root)
# Windows
start docs/reference/setup-wizard-v2.html

# Mac
open docs/reference/setup-wizard-v2.html

# Linux
xdg-open docs/reference/setup-wizard-v2.html
```

Or simply drag the file into your browser.

## Implementation Notes

### Converting to Blazor Components

When implementing the Blazor components, use V2 as your reference:

**1. Extract CSS to `setup-wizard.css`:**
```css
/* Copy CSS custom properties from V2 */
:root {
    --setup-primary-blue: #3498db;
    /* ... etc */
}

/* Copy component styles with BEM naming */
.setup-wizard { }
.setup-wizard__header { }
.setup-wizard__body { }
```

**2. Create Blazor components with matching class names:**
```razor
<!-- SetupWizard.razor -->
<div class="setup-wizard">
    <div class="setup-wizard__header">
        <!-- ... -->
    </div>
</div>
```

**3. Use the documented component structure:**

See [CREATING-CUSTOM-STEPS.md](../CREATING-CUSTOM-STEPS.md) for detailed implementation guidance.

## Testing the Prototype

### Password Access

The V2 prototype includes password protection. Use this password:

```
XpK7-mN94-Qr2L-vB8j
```

### Responsive Testing

Test at these breakpoints:
- Mobile: 375px (iPhone SE)
- Tablet: 768px (iPad)
- Desktop: 1024px+

The prototype is fully responsive and will adjust layout automatically.

### Browser Testing

Tested and working in:
- ✅ Chrome 120+
- ✅ Firefox 120+
- ✅ Safari 17+
- ✅ Edge 120+

## Customization Examples

### Change Primary Color

Edit the CSS custom property in V2:

```css
:root {
    --setup-primary-blue: #9c27b0; /* Purple instead of blue */
}
```

All components will automatically update.

### Change Font

```css
:root {
    --setup-font-family: 'Inter', sans-serif;
}
```

### Dark Mode

Add a dark mode class:

```css
body.dark-mode {
    --setup-background: #1a1a1a;
    --setup-surface: #2d2d2d;
    --setup-text-primary: #ffffff;
    /* ... etc */
}
```

Toggle with JavaScript:
```javascript
document.body.classList.toggle('dark-mode');
```

## Feedback & Iteration

### Reporting Issues

If you find issues with the prototype:
1. Note which version (V1 or V2)
2. Describe the issue
3. Include browser/device info
4. Provide screenshot if visual

### Suggesting Improvements

Design improvements should reference:
- [DESIGN-GUIDE.md](../DESIGN-GUIDE.md) - For design system changes
- [CSS-CUSTOMIZATION.md](../CSS-CUSTOMIZATION.md) - For theming options
- [PROJECT-SCOPE.md](../PROJECT-SCOPE.md) - For feature additions

## Changelog

### V2 (2025-10-23)
- ✅ Implemented complete design system from DESIGN-GUIDE.md
- ✅ Added CSS custom properties for theming
- ✅ Implemented BEM naming convention
- ✅ Added password access step
- ✅ Improved semantic HTML structure
- ✅ Enhanced accessibility class names
- ✅ Production-ready code quality

### V1 (Initial)
- ✅ Original concept prototype
- ✅ 5-step wizard demonstration
- ✅ Basic component examples

## Next Steps

1. ✅ V2 prototype complete
2. ⏳ Extract CSS to separate file
3. ⏳ Implement Blazor components
4. ⏳ Add C# backend logic
5. ⏳ Implement state management
6. ⏳ Add validation
7. ⏳ Write tests

---

**Current Recommendation:** Use **V2** (`setup-wizard-v2.html`) for all development work.
