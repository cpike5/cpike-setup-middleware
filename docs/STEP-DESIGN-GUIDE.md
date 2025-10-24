# Step Design & Styling Guide

This guide provides best practices and patterns for creating well-designed, consistent setup wizard steps.

## Table of Contents

- [Design Principles](#design-principles)
- [Step Structure](#step-structure)
- [Using Built-in Components](#using-built-in-components)
- [Layout Patterns](#layout-patterns)
- [Validation & Error Display](#validation--error-display)
- [Complete Examples](#complete-examples)
- [Accessibility Guidelines](#accessibility-guidelines)
- [Do's and Don'ts](#dos-and-donts)

## Design Principles

### 1. Consistency
- Use the provided `SetupInput`, `SetupToggle`, `SetupCheckbox`, and `SetupAlert` components
- Follow the same layout structure across all steps
- Maintain consistent spacing and visual hierarchy

### 2. Clarity
- Provide clear, concise labels and descriptions
- Use help text to explain non-obvious fields
- Show contextual information with `SetupAlert` components

### 3. Progressive Disclosure
- Only show relevant fields (e.g., hide SQL auth fields when Windows Auth is selected)
- Use conditional rendering with `@if` statements
- Group related fields together

### 4. Immediate Feedback
- Clear field-level errors when user starts typing
- Use `ValueChanged` callbacks to update state in real-time
- Provide inline validation feedback

## Step Structure

Every step should follow this basic structure:

```razor
@using cpike.Setup.Middleware.Steps
@using cpike.Setup.Middleware.Models
@using cpike.Setup.Middleware.Components
@using cpike.Setup.Middleware.Services
@inherits SetupStepBase

<div class="step-content">
    <!-- 1. Alert/Information Section (Optional but recommended) -->
    <SetupAlert Severity="info" Title="Step Title">
        Brief description of what this step configures.
    </SetupAlert>

    <!-- 2. Form Fields -->
    <SetupInput ... />
    <SetupToggle ... />
    <SetupCheckbox ... />

    <!-- 3. Additional Context/Warnings (Optional) -->
    <SetupAlert Severity="warning">
        Important notes or warnings.
    </SetupAlert>
</div>

@code {
    // Step metadata
    public override string Title => "Step Title";
    public override string Description => "Brief description";
    public override int Order => 10;

    // Field values
    private string MyField { get; set; } = string.Empty;

    // Error messages
    private string? _myFieldError;

    // Dual constructors (REQUIRED)
    public MyStep() { }
    public MyStep(ISetupStateManager stateManager, ILogger<MyStep> logger)
    {
        StateManager = stateManager;
        Logger = logger;
    }

    // Lifecycle methods
    protected override void OnInitialized() { }
    public override Task<ValidationResult> ValidateAsync() { }
    public override Task ExecuteAsync() { }

    // Event handlers
    private void OnMyFieldChanged(string? value) { }
}
```

## Using Built-in Components

### SetupInput

The primary input component for text, email, password, number, and other input types.

**Properties:**
- `Id` (required): Unique identifier for the input
- `Label` (required): Display label
- `InputType`: "text", "email", "password", "number", etc.
- `Value`: Current value
- `ValueChanged`: Callback when value changes
- `Placeholder`: Placeholder text
- `Required`: Whether the field is required
- `ErrorMessage`: Validation error to display
- `HelpText`: Explanatory text below the input
- `Autocomplete`: HTML autocomplete attribute

**Example:**
```razor
<SetupInput
    Id="admin-email"
    Label="Administrator Email"
    InputType="email"
    Value="@AdminEmail"
    ValueChanged="@OnEmailChanged"
    Placeholder="admin@example.com"
    Required="true"
    ErrorMessage="@_emailError"
    HelpText="Used for account recovery and notifications"
    Autocomplete="email" />
```

**Best Practices:**
- Always provide a unique `Id`
- Use appropriate `InputType` for better mobile keyboards and validation
- Include helpful `HelpText` for non-obvious fields
- Use proper `Autocomplete` values for better UX
- Clear error when user types: `_emailError = null;` in `ValueChanged`

### SetupToggle

A toggle switch for boolean options.

**Properties:**
- `Label` (required): Display label
- `Description`: Explanatory text
- `Checked`: Current state
- `CheckedChanged`: Callback when toggled

**Example:**
```razor
<SetupToggle
    Label="Use Windows Authentication"
    Description="Use Windows Authentication instead of SQL Server authentication"
    Checked="@UseWindowsAuth"
    CheckedChanged="@OnWindowsAuthChanged" />
```

**Best Practices:**
- Use for binary choices that take immediate effect
- Provide clear `Description` explaining what the toggle does
- Consider clearing related field errors when toggling

### SetupCheckbox

A checkbox for boolean options or multi-select scenarios.

**Properties:**
- `Label` (required): Display label
- `Description`: Explanatory text
- `Checked`: Current state
- `CheckedChanged`: Callback when checked/unchecked

**Example:**
```razor
<SetupCheckbox
    Label="Enable email notifications"
    Description="Receive important updates and alerts via email"
    Checked="@EmailNotifications"
    CheckedChanged="@((value) => EmailNotifications = value)" />
```

**Best Practices:**
- Use for optional features or preferences
- Group related checkboxes together
- Use `SetupToggle` instead for primary on/off settings

### SetupAlert

Display contextual information, warnings, or errors.

**Severities:**
- `info`: Informational messages (blue)
- `success`: Success messages (green)
- `warning`: Warning messages (yellow)
- `error`: Error messages (red)

**Example:**
```razor
<SetupAlert Severity="info" Title="Database Configuration">
    Configure the database connection settings for your application.
</SetupAlert>

<SetupAlert Severity="warning">
    This setting cannot be changed after setup completes.
</SetupAlert>
```

**Best Practices:**
- Use at the top of steps to provide context
- Use `warning` severity for important caveats
- Keep messages concise and actionable

## Layout Patterns

### Simple Form

For straightforward data collection:

```razor
<div class="step-content">
    <SetupAlert Severity="info" Title="Step Title">
        What this step configures.
    </SetupAlert>

    <SetupInput ... />
    <SetupInput ... />
    <SetupToggle ... />
</div>
```

### Conditional Fields

Show/hide fields based on user choices:

```razor
<div class="step-content">
    <SetupToggle
        Label="Enable Feature"
        Checked="@FeatureEnabled"
        CheckedChanged="@((value) => FeatureEnabled = value)" />

    @if (FeatureEnabled)
    {
        <SetupInput
            Label="Feature Setting"
            Value="@FeatureSetting"
            ValueChanged="@OnFeatureSettingChanged" />
    }
</div>
```

### Grouped Fields

Group related fields with alerts:

```razor
<div class="step-content">
    <SetupAlert Severity="info" Title="Authentication">
        Configure authentication settings.
    </SetupAlert>

    <SetupInput ... />
    <SetupInput ... />

    <SetupAlert Severity="info" Title="Permissions">
        Set up default permissions.
    </SetupAlert>

    <SetupCheckbox ... />
    <SetupCheckbox ... />
</div>
```

### Review/Summary

Display collected information:

```razor
<div class="step-content">
    <SetupAlert Severity="info">
        Please review your configuration before completing setup.
    </SetupAlert>

    <SummarySection
        Title="Section Title"
        Description="Section description"
        Items="@_summaryItems" />
</div>
```

## Validation & Error Display

### Field-Level Validation

1. **Declare error fields:**
```csharp
private string? _emailError;
private string? _passwordError;
```

2. **Display errors in components:**
```razor
<SetupInput
    ErrorMessage="@_emailError"
    ... />
```

3. **Set errors in ValidateAsync:**
```csharp
public override Task<ValidationResult> ValidateAsync()
{
    var email = StateManager.Get<string>("AdminEmail") ?? string.Empty;

    ClearErrors();
    var errors = new List<string>();

    if (string.IsNullOrWhiteSpace(email))
    {
        _emailError = "Email is required";
        errors.Add("Email is required.");
    }

    return errors.Any()
        ? Task.FromResult(ValidationResult.Failure(errors))
        : Task.FromResult(ValidationResult.Success);
}
```

4. **Clear errors on input:**
```csharp
private void OnEmailChanged(string? value)
{
    AdminEmail = value ?? string.Empty;
    StateManager.Set("AdminEmail", AdminEmail);
    _emailError = null; // Clear error when user types
}
```

5. **Helper to clear all errors:**
```csharp
private void ClearErrors()
{
    _emailError = null;
    _passwordError = null;
}
```

## Complete Examples

### Example 1: Admin Account Step

**✅ Well-designed step with modern components:**

```razor
@using cpike.Setup.Middleware.Steps
@using cpike.Setup.Middleware.Models
@using cpike.Setup.Middleware.Components
@using cpike.Setup.Middleware.Services
@inherits SetupStepBase

<div class="step-content">
    <SetupAlert Severity="info" Title="Create Administrator Account">
        This account will have full access to the application. Choose a strong password and keep it secure.
    </SetupAlert>

    <SetupInput
        Id="admin-email"
        Label="Administrator Email"
        InputType="email"
        Value="@AdminEmail"
        ValueChanged="@OnEmailChanged"
        Placeholder="admin@example.com"
        Required="true"
        ErrorMessage="@_emailError"
        HelpText="Used for account recovery and notifications"
        Autocomplete="email" />

    <SetupInput
        Id="admin-password"
        Label="Administrator Password"
        InputType="password"
        Value="@AdminPassword"
        ValueChanged="@OnPasswordChanged"
        Placeholder="Enter a strong password"
        Required="true"
        ErrorMessage="@_passwordError"
        HelpText="Must be at least 8 characters long"
        Autocomplete="new-password" />

    <SetupToggle
        Label="Enable email notifications"
        Description="Receive important updates and alerts via email"
        Checked="@EmailNotifications"
        CheckedChanged="@((value) => EmailNotifications = value)" />
</div>

@code {
    public override string Title => "Create Administrator Account";
    public override string Description => "Set up the initial administrator account.";
    public override int Order => 10;

    private string AdminEmail { get; set; } = string.Empty;
    private string AdminPassword { get; set; } = string.Empty;
    private bool EmailNotifications { get; set; } = true;

    private string? _emailError;
    private string? _passwordError;

    // Dual constructors
    public AdminAccountStep() { }
    public AdminAccountStep(ISetupStateManager stateManager, ILogger<AdminAccountStep> logger)
    {
        StateManager = stateManager;
        Logger = logger;
    }

    protected override void OnInitialized()
    {
        AdminEmail = StateManager?.Get<string>("AdminEmail") ?? string.Empty;
        AdminPassword = StateManager?.Get<string>("AdminPassword") ?? string.Empty;
        EmailNotifications = StateManager?.Get<bool?>("EmailNotifications") ?? true;
    }

    public override Task<ValidationResult> ValidateAsync()
    {
        var email = StateManager.Get<string>("AdminEmail") ?? string.Empty;
        var password = StateManager.Get<string>("AdminPassword") ?? string.Empty;

        ClearErrors();
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(email))
        {
            _emailError = "Email is required";
            errors.Add("Administrator email is required.");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            _passwordError = "Password is required";
            errors.Add("Administrator password is required.");
        }

        return errors.Any()
            ? Task.FromResult(ValidationResult.Failure(errors))
            : Task.FromResult(ValidationResult.Success);
    }

    public override Task ExecuteAsync()
    {
        StateManager.Set("AdminEmail", AdminEmail);
        StateManager.Set("AdminPassword", AdminPassword);
        StateManager.Set("EmailNotifications", EmailNotifications);

        Logger.LogInformation("Admin account configured: {Email}", AdminEmail);
        return Task.CompletedTask;
    }

    private void ClearErrors()
    {
        _emailError = null;
        _passwordError = null;
    }

    private void OnEmailChanged(string? value)
    {
        AdminEmail = value ?? string.Empty;
        StateManager.Set("AdminEmail", AdminEmail);
        _emailError = null;
    }

    private void OnPasswordChanged(string? value)
    {
        AdminPassword = value ?? string.Empty;
        StateManager.Set("AdminPassword", AdminPassword);
        _passwordError = null;
    }
}
```

### Example 2: Database Configuration Step

**✅ Conditional fields with modern styling:**

```razor
@using cpike.Setup.Middleware.Steps
@using cpike.Setup.Middleware.Models
@using cpike.Setup.Middleware.Components
@using cpike.Setup.Middleware.Services
@inherits SetupStepBase

<div class="step-content">
    <SetupAlert Severity="info" Title="Database Configuration">
        Configure the database connection settings for your application.
    </SetupAlert>

    <SetupInput
        Id="db-server"
        Label="Database Server"
        InputType="text"
        Value="@DatabaseServer"
        ValueChanged="@OnServerChanged"
        Placeholder="localhost"
        Required="true"
        ErrorMessage="@_serverError"
        HelpText="The hostname or IP address of your database server"
        Autocomplete="off" />

    <SetupInput
        Id="db-name"
        Label="Database Name"
        InputType="text"
        Value="@DatabaseName"
        ValueChanged="@OnNameChanged"
        Placeholder="MyApplicationDb"
        Required="true"
        ErrorMessage="@_nameError"
        HelpText="The name of the database to use"
        Autocomplete="off" />

    <SetupToggle
        Label="Use Windows Authentication"
        Description="Use Windows Authentication instead of SQL Server authentication"
        Checked="@UseWindowsAuth"
        CheckedChanged="@OnWindowsAuthChanged" />

    @if (!UseWindowsAuth)
    {
        <SetupInput
            Id="db-user"
            Label="Database Username"
            InputType="text"
            Value="@DatabaseUser"
            ValueChanged="@OnUserChanged"
            Placeholder="sa"
            Required="true"
            ErrorMessage="@_userError"
            HelpText="SQL Server authentication username"
            Autocomplete="username" />

        <SetupInput
            Id="db-password"
            Label="Database Password"
            InputType="password"
            Value="@DatabasePassword"
            ValueChanged="@OnPasswordChanged"
            Placeholder="Enter password"
            Required="true"
            ErrorMessage="@_passwordError"
            HelpText="SQL Server authentication password"
            Autocomplete="current-password" />
    }
</div>

@code {
    public override string Title => "Configure Database Connection";
    public override string Description => "Set up the database connection for your application.";
    public override int Order => 20;

    private string DatabaseServer { get; set; } = "localhost";
    private string DatabaseName { get; set; } = string.Empty;
    private bool UseWindowsAuth { get; set; } = false;
    private string DatabaseUser { get; set; } = string.Empty;
    private string DatabasePassword { get; set; } = string.Empty;

    private string? _serverError;
    private string? _nameError;
    private string? _userError;
    private string? _passwordError;

    // Dual constructors
    public DatabaseConfigStep() { }
    public DatabaseConfigStep(ISetupStateManager stateManager, ILogger<DatabaseConfigStep> logger)
    {
        StateManager = stateManager;
        Logger = logger;
    }

    protected override void OnInitialized()
    {
        DatabaseServer = StateManager?.Get<string>("DatabaseServer") ?? "localhost";
        DatabaseName = StateManager?.Get<string>("DatabaseName") ?? string.Empty;
        UseWindowsAuth = StateManager?.Get<bool?>("UseWindowsAuth") ?? false;
        DatabaseUser = StateManager?.Get<string>("DatabaseUser") ?? string.Empty;
        DatabasePassword = StateManager?.Get<string>("DatabasePassword") ?? string.Empty;
    }

    public override Task<ValidationResult> ValidateAsync()
    {
        var dbServer = StateManager.Get<string>("DatabaseServer") ?? string.Empty;
        var dbName = StateManager.Get<string>("DatabaseName") ?? string.Empty;
        var useWinAuth = StateManager.Get<bool?>("UseWindowsAuth") ?? false;
        var dbUser = StateManager.Get<string>("DatabaseUser") ?? string.Empty;
        var dbPassword = StateManager.Get<string>("DatabasePassword") ?? string.Empty;

        ClearErrors();
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(dbServer))
        {
            _serverError = "Database server is required";
            errors.Add("Database server is required.");
        }

        if (string.IsNullOrWhiteSpace(dbName))
        {
            _nameError = "Database name is required";
            errors.Add("Database name is required.");
        }

        if (!useWinAuth)
        {
            if (string.IsNullOrWhiteSpace(dbUser))
            {
                _userError = "Username is required";
                errors.Add("Database username is required.");
            }

            if (string.IsNullOrWhiteSpace(dbPassword))
            {
                _passwordError = "Password is required";
                errors.Add("Database password is required.");
            }
        }

        return errors.Any()
            ? Task.FromResult(ValidationResult.Failure(errors))
            : Task.FromResult(ValidationResult.Success);
    }

    public override Task ExecuteAsync()
    {
        StateManager.Set("DatabaseServer", DatabaseServer);
        StateManager.Set("DatabaseName", DatabaseName);
        StateManager.Set("UseWindowsAuth", UseWindowsAuth);
        StateManager.Set("DatabaseUser", DatabaseUser);
        StateManager.Set("DatabasePassword", DatabasePassword);

        Logger.LogInformation("Database configured: Server={Server}, Database={Database}",
            DatabaseServer, DatabaseName);
        return Task.CompletedTask;
    }

    private void ClearErrors()
    {
        _serverError = null;
        _nameError = null;
        _userError = null;
        _passwordError = null;
    }

    private void OnServerChanged(string? value)
    {
        DatabaseServer = value ?? string.Empty;
        StateManager.Set("DatabaseServer", DatabaseServer);
        _serverError = null;
    }

    private void OnNameChanged(string? value)
    {
        DatabaseName = value ?? string.Empty;
        StateManager.Set("DatabaseName", DatabaseName);
        _nameError = null;
    }

    private void OnWindowsAuthChanged(bool value)
    {
        UseWindowsAuth = value;
        StateManager.Set("UseWindowsAuth", UseWindowsAuth);
        if (UseWindowsAuth)
        {
            _userError = null;
            _passwordError = null;
        }
    }

    private void OnUserChanged(string? value)
    {
        DatabaseUser = value ?? string.Empty;
        StateManager.Set("DatabaseUser", DatabaseUser);
        _userError = null;
    }

    private void OnPasswordChanged(string? value)
    {
        DatabasePassword = value ?? string.Empty;
        StateManager.Set("DatabasePassword", DatabasePassword);
        _passwordError = null;
    }
}
```

## Accessibility Guidelines

### Labels and IDs
- Always provide unique `Id` attributes
- Labels are automatically associated via the component
- Use descriptive labels that clearly indicate the field purpose

### Help Text
- Provide help text for non-obvious fields
- Keep help text concise and actionable
- Use help text to explain requirements (e.g., "At least 8 characters")

### Keyboard Navigation
- All built-in components support keyboard navigation
- Tab order follows DOM order
- Enter key submits the form (handled by wizard)

### Screen Readers
- Components include proper ARIA attributes
- Error messages are announced to screen readers
- Required fields are properly marked

### Color Contrast
- All components meet WCAG AA standards
- Don't rely solely on color to convey information
- Error states include both color and text

## Do's and Don'ts

### ✅ Do

- **Use built-in components** for consistency
- **Provide help text** for non-obvious fields
- **Clear errors on input** for better UX
- **Group related fields** logically
- **Use conditional rendering** to hide irrelevant fields
- **Store state in real-time** via `ValueChanged`
- **Validate from StateManager** in `ValidateAsync()`
- **Provide contextual alerts** at the top of steps
- **Use appropriate input types** (email, password, number, etc.)
- **Include proper autocomplete** attributes

### ❌ Don't

- **Don't use raw HTML inputs** - use `SetupInput` instead
- **Don't use raw checkboxes** - use `SetupToggle` or `SetupCheckbox`
- **Don't skip help text** on complex fields
- **Don't rely on component fields** in `ValidateAsync()` - always read from StateManager
- **Don't forget dual constructors** - they're required for validation to work
- **Don't overwhelm users** with too many fields on one step
- **Don't use generic error messages** - be specific
- **Don't forget to clear errors** when users correct input
- **Don't skip the initial SetupAlert** - it provides important context
- **Don't use inconsistent styling** - stick to the component library

### Bad Example ❌

```razor
<!-- DON'T DO THIS -->
<div class="form-group">
    <label for="email">Email</label>
    <input type="text" id="email" @bind="Email" class="form-control" />
</div>

<div class="form-group">
    <label>
        <input type="checkbox" @bind="UseAuth" />
        Use Authentication
    </label>
</div>
```

### Good Example ✅

```razor
<!-- DO THIS INSTEAD -->
<SetupInput
    Id="email"
    Label="Email Address"
    InputType="email"
    Value="@Email"
    ValueChanged="@OnEmailChanged"
    Placeholder="user@example.com"
    Required="true"
    ErrorMessage="@_emailError"
    HelpText="Used for sign-in and notifications"
    Autocomplete="email" />

<SetupToggle
    Label="Use Authentication"
    Description="Require users to authenticate before accessing the application"
    Checked="@UseAuth"
    CheckedChanged="@((value) => UseAuth = value)" />
```

## Custom Styling (Advanced)

If you need custom styling beyond the built-in components, see [CSS Customization Guide](CSS-CUSTOMIZATION.md) for theming options.

### CSS Custom Properties

The wizard uses CSS custom properties that can be overridden:

```css
:root {
    --setup-primary-blue: #4A90E2;
    --setup-text-primary: #1a202c;
    --setup-text-secondary: #4a5568;
    --setup-border-color: #e2e8f0;
    --setup-error-color: #e53e3e;
    --setup-success-color: #38a169;
}
```

### Step Content Spacing

The `.step-content` wrapper provides consistent spacing. Built-in components automatically have proper margins. If adding custom elements, maintain consistent spacing:

```css
.step-content > * {
    margin-bottom: 1.5rem; /* 24px */
}

.step-content > *:last-child {
    margin-bottom: 0;
}
```

## Summary

Creating great setup steps:

1. **Use the component library** - `SetupInput`, `SetupToggle`, `SetupAlert`, etc.
2. **Follow the structure** - Alert → Fields → Validation
3. **Implement dual constructors** - Required for proper validation
4. **Validate from StateManager** - Not from component fields
5. **Clear errors on input** - Better user experience
6. **Provide help text** - Users appreciate guidance
7. **Use conditional rendering** - Keep the UI clean and focused
8. **Test thoroughly** - Ensure validation works in all scenarios

For more details, see:
- [Creating Custom Steps](CREATING-CUSTOM-STEPS.md)
- [CSS Customization](CSS-CUSTOMIZATION.md)
- [Architecture Overview](ARCHITECTURE.md)
