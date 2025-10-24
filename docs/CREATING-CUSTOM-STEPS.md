# Creating Custom Setup Steps

## Overview

This guide explains how to create custom setup steps for the cpike.Setup.Middleware library. Setup steps are the individual screens in your wizard that collect configuration data from the administrator.

> 💡 **Looking for design and styling guidance?** Check out the **[Step Design & Styling Guide](STEP-DESIGN-GUIDE.md)** for best practices on creating beautiful, modern setup steps using the built-in component library.

## Quick Start

### Minimal Step Implementation

The simplest way to create a step is to inherit from `SetupStepBase`:

```csharp
@inherits SetupStepBase

<h2>@Title</h2>
<p>@Description</p>

<div class="form-group">
    <label>Your Setting</label>
    <input type="text" @bind="MySetting" class="form-input" />
</div>

@code {
    public override string Title => "My Custom Step";
    public override string Description => "Configure my custom feature";
    public override int Order => 10;

    private string MySetting { get; set; }

    public override async Task ExecuteAsync()
    {
        // Save your settings here
        // e.g., save to database, write to config file, etc.
        await MyService.SaveSettingAsync(MySetting);
    }
}
```

### Register the Step

In your `Program.cs`:

```csharp
builder.Services.AddSetupWizard(setup =>
{
    setup.AddStep<MyCustomStep>();
});
```

That's it! Your step is now part of the setup wizard.

## Step Interface

All steps must implement the `ISetupStep` interface:

```csharp
public interface ISetupStep
{
    // Display properties
    string Title { get; }
    string Description { get; }
    int Order { get; }

    // Lifecycle methods
    Task<ValidationResult> ValidateAsync();
    Task ExecuteAsync();
    Task OnNavigatingFromAsync();
    Task OnNavigatingToAsync();
}
```

## Using SetupStepBase

Most steps should inherit from `SetupStepBase`, which provides:

- Base implementation of `ISetupStep`
- Access to dependency injection
- Helper methods and properties
- Default implementations of lifecycle methods

```csharp
@inherits SetupStepBase
@inject IMyService MyService

<div class="step-content">
    <!-- Your UI here -->
</div>

@code {
    public override string Title => "My Step Title";
    public override string Description => "What this step does";
    public override int Order => 10;

    // Your component code
}
```

### Available Properties

`SetupStepBase` provides these injected properties:

```csharp
[Inject] protected ISetupStateManager StateManager { get; set; }
[Inject] protected ILogger Logger { get; set; }
```

You can inject additional services:

```csharp
@inject IConfiguration Configuration
@inject UserManager<ApplicationUser> UserManager
```

### Dual Constructor Pattern (IMPORTANT)

Steps need to support two different instantiation scenarios:

1. **Blazor rendering**: Requires a parameterless constructor and uses `[Inject]` attributes
2. **DI container instantiation**: Used by the wizard service for validation on non-rendered instances

**Use this pattern:**

```csharp
@code {
    // Parameterless constructor for Blazor rendering
    public MyCustomStep()
    {
    }

    // Constructor injection for when instance is created via DI (not rendered)
    public MyCustomStep(ISetupStateManager stateManager, ILogger<MyCustomStep> logger)
    {
        StateManager = stateManager;
        Logger = logger;
    }

    protected override void OnInitialized()
    {
        // Load state from StateManager when rendered
        MyValue = StateManager?.Get<string>("MyValue") ?? string.Empty;
    }

    public override Task<ValidationResult> ValidateAsync()
    {
        // IMPORTANT: Always load from StateManager, not local fields
        // This method may be called on a fresh instance created via DI
        var myValue = StateManager.Get<string>("MyValue") ?? string.Empty;

        if (string.IsNullOrWhiteSpace(myValue))
            return Task.FromResult(ValidationResult.Failure("Value is required"));

        return Task.FromResult(ValidationResult.Success);
    }
}
```

**Why this matters:**
- When navigating between steps, the wizard service creates a new instance via DI to call `ValidateAsync()`
- This instance is NOT rendered, so Blazor lifecycle methods (`OnInitialized`, etc.) are NOT called
- Therefore, `ValidateAsync()` must read directly from `StateManager` rather than relying on component fields

## Step Ordering

Steps execute in ascending order based on the `Order` property:

```csharp
public override int Order => 10;
```

**Guidelines:**

- Use increments of 10 (10, 20, 30, ...) to allow insertion of steps later
- Lower numbers execute first
- Steps with the same order execute in registration order
- Welcome/intro steps: 0-10
- Configuration steps: 10-80
- Review step: 90
- Completion step: 100

**Example:**

```csharp
public class WelcomeStep : SetupStepBase
{
    public override int Order => 0;
}

public class AdminAccountStep : SetupStepBase
{
    public override int Order => 10;
}

public class DatabaseConfigStep : SetupStepBase
{
    public override int Order => 20;
}

public class ReviewStep : SetupStepBase
{
    public override int Order => 90;
}
```

## Validation

Implement `ValidateAsync()` to validate user input before proceeding to the next step:

```csharp
public override Task<ValidationResult> ValidateAsync()
{
    // IMPORTANT: Always load from StateManager, not component fields
    // This method may be called on a fresh DI instance that wasn't rendered
    var email = StateManager.Get<string>("AdminEmail") ?? string.Empty;
    var password = StateManager.Get<string>("AdminPassword") ?? string.Empty;
    var confirmPassword = StateManager.Get<string>("AdminPasswordConfirm") ?? string.Empty;

    var errors = new List<string>();

    if (string.IsNullOrWhiteSpace(email))
    {
        errors.Add("Email is required");
    }
    else if (!email.Contains("@"))
    {
        errors.Add("Email format is invalid");
    }

    if (string.IsNullOrWhiteSpace(password))
    {
        errors.Add("Password is required");
    }
    else if (password.Length < 8)
    {
        errors.Add("Password must be at least 8 characters");
    }

    if (password != confirmPassword)
    {
        errors.Add("Passwords do not match");
    }

    if (errors.Any())
    {
        return Task.FromResult(ValidationResult.Failure(errors));
    }

    return Task.FromResult(ValidationResult.Success);
}
```

**Validation Result:**

```csharp
// Success
return ValidationResult.Success;

// Single error
return ValidationResult.Failure("Email is required");

// Multiple errors
return ValidationResult.Failure(new[] {
    "Email is required",
    "Password is too short"
});
```

### Field-Level Validation

For field-level validation, use Blazor's `EditForm` and `DataAnnotations`:

```csharp
@using System.ComponentModel.DataAnnotations

<EditForm Model="@Model" OnValidSubmit="HandleValidSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary />

    <div class="form-group">
        <label>Email</label>
        <InputText @bind-Value="Model.Email" class="form-input" />
        <ValidationMessage For="@(() => Model.Email)" />
    </div>

    <div class="form-group">
        <label>Password</label>
        <InputText type="password" @bind-Value="Model.Password" class="form-input" />
        <ValidationMessage For="@(() => Model.Password)" />
    </div>
</EditForm>

@code {
    private AdminModel Model { get; set; } = new();

    public class AdminModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; set; }
    }

    public override async Task<ValidationResult> ValidateAsync()
    {
        // Additional validation beyond data annotations
        if (await UserManager.FindByEmailAsync(Model.Email) != null)
        {
            return ValidationResult.Failure("Email already exists");
        }

        return ValidationResult.Success;
    }
}
```

## Executing Step Logic

The `ExecuteAsync()` method is called after successful validation when the user clicks "Next":

```csharp
public override async Task ExecuteAsync()
{
    // Save configuration
    await Configuration.UpdateAsync("AdminEmail", Email);

    // Create database records
    var user = new ApplicationUser
    {
        Email = Email,
        UserName = Email,
        EmailConfirmed = true
    };

    var result = await UserManager.CreateAsync(user, Password);

    if (!result.Succeeded)
    {
        throw new InvalidOperationException(
            $"Failed to create user: {string.Join(", ", result.Errors)}");
    }

    // Add to role
    await UserManager.AddToRoleAsync(user, "Administrator");

    // Store data for later steps
    StateManager.Set("AdminUserId", user.Id);
    StateManager.Set("AdminEmail", Email);

    Logger.LogInformation("Created administrator account: {Email}", Email);
}
```

**Best Practices:**

1. **Be Idempotent**: Executing multiple times should be safe
2. **Handle Errors**: Throw meaningful exceptions
3. **Log Actions**: Use ILogger for audit trail
4. **Store State**: Use StateManager to share data with other steps

## Lifecycle Hooks

### OnNavigatingToAsync

Called when the user navigates TO this step:

```csharp
public override async Task OnNavigatingToAsync()
{
    // Load default values
    Email = Configuration["DefaultAdminEmail"];

    // Pre-populate from state
    if (StateManager.TryGet<string>("AdminEmail", out var savedEmail))
    {
        Email = savedEmail;
    }

    // Fetch data from database
    AvailableRoles = await RoleManager.Roles.ToListAsync();

    Logger.LogInformation("Navigated to admin account step");
}
```

**Use Cases:**

- Load default values
- Restore previously entered data
- Fetch data from database
- Initialize component state

### OnNavigatingFromAsync

Called when the user navigates AWAY from this step:

```csharp
public override async Task OnNavigatingFromAsync()
{
    // Cleanup temporary resources
    TempDataService.Clear();

    // Save draft (if implementing draft functionality)
    await DraftService.SaveAsync(new
    {
        Email,
        SelectedRoles
    });

    Logger.LogInformation("Navigated away from admin account step");
}
```

**Use Cases:**

- Cleanup resources
- Save draft data
- Update analytics
- Dispose temporary objects

## Sharing Data Between Steps

Use `ISetupStateManager` to share data across steps:

### Storing Data

```csharp
// In Step 1: Admin Account
public override async Task ExecuteAsync()
{
    await CreateAdminUser();

    // Store for later steps
    StateManager.Set("AdminEmail", Email);
    StateManager.Set("AdminUserId", userId);
    StateManager.Set("SelectedRoles", SelectedRoles);
}
```

### Retrieving Data

```csharp
// In Step 5: Review
public override async Task OnNavigatingToAsync()
{
    // Get required value (throws if not found)
    AdminEmail = StateManager.Get<string>("AdminEmail");

    // Try get (safe)
    if (StateManager.TryGet<string[]>("SelectedRoles", out var roles))
    {
        SelectedRoles = roles;
    }
    else
    {
        SelectedRoles = Array.Empty<string>();
    }
}
```

### Type Safety

```csharp
// Store complex objects
StateManager.Set("AdminUser", new AdminUserData
{
    Email = Email,
    Roles = SelectedRoles,
    CreatedAt = DateTime.UtcNow
});

// Retrieve
var adminUser = StateManager.Get<AdminUserData>("AdminUser");
```

## Using Pre-Built Components

The library provides pre-built components you can use in your steps:

### SetupInput

```razor
<SetupInput
    Label="Email Address"
    @bind-Value="Email"
    Type="email"
    Placeholder="admin@example.com"
    HintText="You'll use this email to sign in"
    Required="true" />
```

### SetupCheckbox

```razor
<SetupCheckbox
    Label="Enable Feature"
    Description="This enables the advanced feature set"
    @bind-Value="EnableFeature" />
```

### SetupToggle

```razor
<SetupToggle
    Label="Require Email Confirmation"
    Description="Users must confirm their email before accessing the system"
    @bind-Value="RequireEmailConfirmation" />
```

### SetupAlert

```razor
<SetupAlert Type="AlertType.Info">
    <strong>Note:</strong> This setting cannot be changed after setup completes.
</SetupAlert>

<SetupAlert Type="AlertType.Warning">
    Changing this may impact existing users.
</SetupAlert>

<SetupAlert Type="AlertType.Success">
    Configuration saved successfully!
</SetupAlert>
```

### SetupButton

```razor
<SetupButton
    Type="ButtonType.Primary"
    OnClick="TestConnection">
    Test Connection
</SetupButton>

<SetupButton
    Type="ButtonType.Secondary"
    OnClick="LoadDefaults">
    Load Defaults
</SetupButton>
```

## Complete Example: Admin Account Step

```razor
@using cpike.Setup.Middleware.Steps
@using cpike.Setup.Middleware.Models
@using cpike.Setup.Middleware.Components
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

    <SetupInput
        Id="admin-password-confirm"
        Label="Confirm Password"
        InputType="password"
        Value="@AdminPasswordConfirm"
        ValueChanged="@OnConfirmPasswordChanged"
        Placeholder="Re-enter password"
        Required="true"
        ErrorMessage="@_confirmPasswordError"
        Autocomplete="new-password" />
</div>

@code {
    public override string Title => "Create Administrator Account";
    public override string Description => "Set up the initial administrator account for your application.";
    public override int Order => 10;

    private string AdminEmail { get; set; } = string.Empty;
    private string AdminPassword { get; set; } = string.Empty;
    private string AdminPasswordConfirm { get; set; } = string.Empty;

    private string? _emailError;
    private string? _passwordError;
    private string? _confirmPasswordError;

    // Parameterless constructor for Blazor rendering
    public AdminAccountStep()
    {
    }

    // Constructor injection for when instance is created via DI (not rendered)
    public AdminAccountStep(ISetupStateManager stateManager, ILogger<AdminAccountStep> logger)
    {
        StateManager = stateManager;
        Logger = logger;
    }

    protected override void OnInitialized()
    {
        // Restore values from state manager if they exist
        AdminEmail = StateManager?.Get<string>("AdminEmail") ?? string.Empty;
        AdminPassword = StateManager?.Get<string>("AdminPassword") ?? string.Empty;
        AdminPasswordConfirm = StateManager?.Get<string>("AdminPasswordConfirm") ?? string.Empty;

        Logger?.LogInformation("AdminAccountStep.OnInitialized - Email: '{Email}', PasswordLength: {Length}",
            AdminEmail, AdminPassword?.Length ?? 0);
    }

    public override Task<ValidationResult> ValidateAsync()
    {
        // IMPORTANT: Load from StateManager because this method may be called
        // on a fresh instance that wasn't rendered (lifecycle methods not called)
        var email = StateManager.Get<string>("AdminEmail") ?? string.Empty;
        var password = StateManager.Get<string>("AdminPassword") ?? string.Empty;
        var confirmPassword = StateManager.Get<string>("AdminPasswordConfirm") ?? string.Empty;

        ClearErrors();
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(email))
        {
            _emailError = "Administrator email is required";
            errors.Add("Administrator email is required.");
        }
        else if (!email.Contains("@"))
        {
            _emailError = "Please enter a valid email address";
            errors.Add("Please enter a valid email address.");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            _passwordError = "Administrator password is required";
            errors.Add("Administrator password is required.");
        }
        else if (password.Length < 8)
        {
            _passwordError = "Password must be at least 8 characters long";
            errors.Add("Password must be at least 8 characters long.");
        }

        if (password != confirmPassword)
        {
            _confirmPasswordError = "Passwords do not match";
            errors.Add("Passwords do not match.");
        }

        if (errors.Any())
        {
            return Task.FromResult(ValidationResult.Failure(errors));
        }

        return Task.FromResult(ValidationResult.Success);
    }

    public override Task ExecuteAsync()
    {
        // Store the admin credentials in the state manager
        StateManager.Set("AdminEmail", AdminEmail);
        StateManager.Set("AdminPassword", AdminPassword);

        Logger.LogInformation("Admin account configured: {Email}", AdminEmail);
        return Task.CompletedTask;
    }

    private void ClearErrors()
    {
        _emailError = null;
        _passwordError = null;
        _confirmPasswordError = null;
    }

    private void OnEmailChanged(string? value)
    {
        AdminEmail = value ?? string.Empty;
        StateManager.Set("AdminEmail", AdminEmail);
        _emailError = null; // Clear error when user types
    }

    private void OnPasswordChanged(string? value)
    {
        AdminPassword = value ?? string.Empty;
        StateManager.Set("AdminPassword", AdminPassword);
        _passwordError = null; // Clear error when user types
    }

    private void OnConfirmPasswordChanged(string? value)
    {
        AdminPasswordConfirm = value ?? string.Empty;
        StateManager.Set("AdminPasswordConfirm", AdminPasswordConfirm);
        _confirmPasswordError = null; // Clear error when user types
    }
}
```

## Advanced Topics

### Conditional Steps

Steps that only appear based on previous choices:

```csharp
public override async Task<bool> ShouldExecuteAsync()
{
    // Only show if email was enabled in previous step
    var emailEnabled = StateManager.Get<bool>("EmailEnabled");
    return emailEnabled;
}
```

*Note: `ShouldExecuteAsync()` is a planned feature for future releases.*

### Async Initialization

For expensive initialization, use `OnInitializedAsync`:

```csharp
protected override async Task OnInitializedAsync()
{
    await base.OnInitializedAsync();

    // Load data from database
    AvailableRoles = await RoleManager.Roles.ToListAsync();
    AvailableFeatures = await FeatureService.GetAllAsync();
}
```

### Custom Validation with Services

```csharp
@inject IEmailValidator EmailValidator

public override async Task<ValidationResult> ValidateAsync()
{
    // Use injected service for validation
    var emailValid = await EmailValidator.IsValidAsync(Model.Email);
    if (!emailValid)
    {
        return ValidationResult.Failure("Email domain is not allowed");
    }

    return ValidationResult.Success;
}
```

### Error Handling

```csharp
public override async Task ExecuteAsync()
{
    try
    {
        await SaveConfigurationAsync();
    }
    catch (DbException ex)
    {
        Logger.LogError(ex, "Database error during setup");
        throw new InvalidOperationException(
            "Unable to save configuration. Please check database connection.", ex);
    }
    catch (Exception ex)
    {
        Logger.LogError(ex, "Unexpected error during setup");
        throw new InvalidOperationException(
            "An unexpected error occurred. Please try again or contact support.", ex);
    }
}
```

## Testing Custom Steps

### Unit Testing

```csharp
public class AdminAccountStepTests
{
    [Fact]
    public async Task ValidateAsync_EmptyEmail_ReturnsFailure()
    {
        // Arrange
        var step = new AdminAccountStep
        {
            Model = new() { Email = "" }
        };

        // Act
        var result = await step.ValidateAsync();

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Email is required", result.Errors);
    }

    [Fact]
    public async Task ExecuteAsync_ValidData_CreatesUser()
    {
        // Arrange
        var userManager = CreateMockUserManager();
        var step = new AdminAccountStep(userManager)
        {
            Model = new()
            {
                Email = "admin@test.com",
                Password = "Test123!@#"
            }
        };

        // Act
        await step.ExecuteAsync();

        // Assert
        userManager.Verify(x => x.CreateAsync(
            It.IsAny<ApplicationUser>(),
            "Test123!@#"), Times.Once);
    }
}
```

## Best Practices

1. **Keep Steps Focused**: Each step should configure one logical area
2. **Validate Thoroughly**: Don't rely only on client-side validation
3. **Provide Clear Feedback**: Show progress, errors, and success messages
4. **Handle Errors Gracefully**: Catch exceptions, log them, show user-friendly messages
5. **Use State Manager**: Share data between steps via `StateManager`
6. **Log Important Actions**: Use `ILogger` for audit trail
7. **Test Thoroughly**: Unit test validation and execution logic
8. **Document Complex Steps**: Add comments explaining non-obvious logic
9. **Follow UI Guidelines**: Use pre-built components for consistency
10. **Consider Accessibility**: Add ARIA labels, support keyboard navigation

## Troubleshooting

### Step Not Appearing

Check:

- Step is registered in `Program.cs`
- Step order is correct
- No exceptions in constructor or `OnInitializedAsync`

### Validation Not Working

Check:

- `ValidateAsync()` returns correct `ValidationResult`
- Validation errors are being displayed in UI
- Form model has proper data annotations

### State Not Persisting

Check:

- `StateManager` is being used (not local storage or cookies)
- Keys are consistent between steps
- Data is being set before navigation

### Dependency Injection Errors

Check:

- Services are registered in `Program.cs`
- Service lifetime is compatible (scoped/transient)
- Constructor or property injection is used correctly

## Next Steps

- Review the [CSS Customization Guide](CSS-CUSTOMIZATION.md) to style your steps
- Check the [Architecture Overview](ARCHITECTURE.md) to understand how steps fit in
- Explore the example app for real-world step implementations
