# Creating Custom Setup Steps

## Overview

This guide explains how to create custom setup steps for the cpike.Setup.Middleware library. Setup steps are the individual screens in your wizard that collect configuration data from the administrator.

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
[Inject] protected NavigationManager Navigation { get; set; }
```

You can inject additional services:

```csharp
@inject IConfiguration Configuration
@inject ILogger<MyCustomStep> Logger
@inject UserManager<ApplicationUser> UserManager
```

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
public override async Task<ValidationResult> ValidateAsync()
{
    var errors = new List<string>();

    if (string.IsNullOrWhiteSpace(Email))
    {
        errors.Add("Email is required");
    }
    else if (!IsValidEmail(Email))
    {
        errors.Add("Email format is invalid");
    }

    if (string.IsNullOrWhiteSpace(Password))
    {
        errors.Add("Password is required");
    }
    else if (Password.Length < 8)
    {
        errors.Add("Password must be at least 8 characters");
    }

    if (Password != ConfirmPassword)
    {
        errors.Add("Passwords do not match");
    }

    if (errors.Any())
    {
        return ValidationResult.Failure(errors);
    }

    return ValidationResult.Success;
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
@page "/setup/admin"
@inherits SetupStepBase
@inject UserManager<ApplicationUser> UserManager
@inject ILogger<AdminAccountStep> Logger

<div class="step-header">
    <h2 class="step-title">@Title</h2>
    <p class="step-description">@Description</p>
</div>

<EditForm Model="@Model" OnValidSubmit="OnFormSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary />

    <div class="form-group">
        <label>Email Address</label>
        <InputText @bind-Value="Model.Email"
                   type="email"
                   class="form-input"
                   placeholder="admin@example.com" />
        <ValidationMessage For="@(() => Model.Email)" />
        <div class="form-hint">You'll use this email to sign in</div>
    </div>

    <div class="form-group">
        <label>Password</label>
        <InputText @bind-Value="Model.Password"
                   type="password"
                   class="form-input" />
        <ValidationMessage For="@(() => Model.Password)" />

        @if (!string.IsNullOrEmpty(Model.Password))
        {
            <PasswordStrength Password="@Model.Password" />
        }

        <div class="form-hint">
            Minimum 8 characters with uppercase, lowercase, number, and symbol
        </div>
    </div>

    <div class="form-group">
        <label>Confirm Password</label>
        <InputText @bind-Value="Model.ConfirmPassword"
                   type="password"
                   class="form-input" />
        <ValidationMessage For="@(() => Model.ConfirmPassword)" />
    </div>
</EditForm>

@if (!string.IsNullOrEmpty(ErrorMessage))
{
    <SetupAlert Type="AlertType.Error">
        @ErrorMessage
    </SetupAlert>
}

@code {
    public override string Title => "Create Administrator Account";
    public override string Description => "This account will have full access to all system features and settings.";
    public override int Order => 10;

    private AdminModel Model { get; set; } = new();
    private string ErrorMessage { get; set; }

    public class AdminModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]",
            ErrorMessage = "Password must contain uppercase, lowercase, number, and symbol")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password confirmation is required")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }

    public override async Task<ValidationResult> ValidateAsync()
    {
        // Check if email already exists
        var existingUser = await UserManager.FindByEmailAsync(Model.Email);
        if (existingUser != null)
        {
            return ValidationResult.Failure("An account with this email already exists");
        }

        return ValidationResult.Success;
    }

    public override async Task ExecuteAsync()
    {
        try
        {
            // Create admin user
            var user = new ApplicationUser
            {
                UserName = Model.Email,
                Email = Model.Email,
                EmailConfirmed = true
            };

            var result = await UserManager.CreateAsync(user, Model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create admin user: {errors}");
            }

            // Add to administrator role
            await UserManager.AddToRoleAsync(user, "Administrator");

            // Store for later steps
            StateManager.Set("AdminEmail", Model.Email);
            StateManager.Set("AdminUserId", user.Id);

            Logger.LogInformation("Created administrator account: {Email}", Model.Email);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating administrator account");
            ErrorMessage = "An error occurred while creating the account. Please try again.";
            throw;
        }
    }

    public override async Task OnNavigatingToAsync()
    {
        // Restore previously entered data if navigating back
        if (StateManager.TryGet<string>("AdminEmail", out var savedEmail))
        {
            Model.Email = savedEmail;
        }
    }

    private void OnFormSubmit()
    {
        // Form is valid, EditForm validation passed
        // Actual execution happens in ExecuteAsync
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
