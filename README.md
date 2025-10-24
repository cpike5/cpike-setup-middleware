# First Time Setup Middleware

A lightweight, extensible middleware library for .NET Blazor applications that provides a wizard-based first-time setup experience. This library makes it easy to add a professional setup wizard to your Blazor projects, ensuring administrators configure critical settings before the application becomes publicly accessible.

## Overview

This middleware intercepts all requests to your application and redirects them to a multi-step setup wizard until the setup process is completed. It's designed to be simple to integrate, easy to customize, and provides a clean, modern UI out of the box.

## Features

- **Request Interception**: Automatically redirects all requests to the setup wizard until configuration is complete
- **Password Protection**: Secure setup wizard with generated password (enabled by default)
- **Multi-Step Wizard**: Extensible step-based wizard with progress tracking
- **Builder Pattern**: Fluent API for adding custom setup steps
- **Blazor Components**: Pre-built UI components for forms, toggles, checkboxes, and more
- **File/Config Based State**: Simple file marker or configuration setting to track setup completion
- **Extensible**: Third-party features can bundle and register their own setup steps
- **Modern UI**: Clean, responsive design with professional styling (see [Interactive Prototype](docs/reference/setup-wizard-v2.html))

## Quick Start

### Installation

```bash
dotnet add package cpike.Setup.Middleware
```

### Basic Usage

**1. Register the setup wizard in `Program.cs`:**

```csharp
using cpike.Setup.Middleware.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add your normal services
builder.Services.AddRazorComponents();
builder.Services.AddServerSideBlazor();

// Add setup wizard with custom steps
builder.Services.AddSetupWizard(setup =>
{
    setup.AddStep<AdminAccountStep>();
    setup.AddStep<SystemRolesStep>();
    setup.AddStep<InviteSystemStep>();
});

var app = builder.Build();

// IMPORTANT: Add setup middleware BEFORE authentication/authorization
app.UseSetupMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
```

**2. Access your application:**

When you first run the application, you'll see the setup password in the console:

```
========================================
SETUP WIZARD ACCESS PASSWORD
========================================
Your setup wizard is protected.
Enter this password to begin setup:

    XpK7-mN94-Qr2L-vB8j

========================================
```

**3. Complete the setup wizard:**

Navigate to your application URL, enter the password, and complete the multi-step wizard. Once finished, the application will be accessible to all users.

### Creating Custom Steps

Create a Blazor component that inherits from `SetupStepBase`:

**AdminAccountStep.razor:**

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
        HelpText="Used for account recovery and notifications" />

    <SetupInput
        Id="admin-password"
        Label="Administrator Password"
        InputType="password"
        Value="@AdminPassword"
        ValueChanged="@OnPasswordChanged"
        Placeholder="Enter a strong password"
        Required="true"
        ErrorMessage="@_passwordError"
        HelpText="Must be at least 8 characters long" />
</div>

@code {
    public override string Title => "Create Administrator Account";
    public override string Description => "Set up the initial administrator account for your application.";
    public override int Order => 10;

    private string AdminEmail { get; set; } = string.Empty;
    private string AdminPassword { get; set; } = string.Empty;
    private string? _emailError;
    private string? _passwordError;

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
    }

    public override Task<ValidationResult> ValidateAsync()
    {
        // IMPORTANT: Load from StateManager because this method may be called
        // on a fresh instance that wasn't rendered (lifecycle methods not called)
        var email = StateManager.Get<string>("AdminEmail") ?? string.Empty;
        var password = StateManager.Get<string>("AdminPassword") ?? string.Empty;

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(email))
            errors.Add("Administrator email is required.");
        else if (!email.Contains("@"))
            errors.Add("Please enter a valid email address.");

        if (string.IsNullOrWhiteSpace(password))
            errors.Add("Administrator password is required.");
        else if (password.Length < 8)
            errors.Add("Password must be at least 8 characters long.");

        return errors.Any()
            ? Task.FromResult(ValidationResult.Failure(errors))
            : Task.FromResult(ValidationResult.Success);
    }

    public override Task ExecuteAsync()
    {
        // Store the admin credentials in the state manager
        StateManager.Set("AdminEmail", AdminEmail);
        StateManager.Set("AdminPassword", AdminPassword);

        Logger.LogInformation("Admin account configured: {Email}", AdminEmail);
        return Task.CompletedTask;
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

**See [Creating Custom Steps](docs/CREATING-CUSTOM-STEPS.md) for detailed documentation.**

## Project Status

**Current Phase**: MVP Development

This project is currently in early development. The initial version focuses on:

- Core middleware functionality
- Basic wizard framework
- File-based setup completion tracking
- Essential UI components

Future enhancements will include database-backed state management, additional components, and more advanced features.

## Use Case

This library is primarily built for personal Blazor projects but is open-sourced for community use and contributions. It solves the common problem of needing a one-time setup wizard for administrative configuration before opening an application to users.

**Ideal for:**

- Self-hosted Blazor applications
- Internal tools requiring initial configuration
- Multi-tenant apps needing per-tenant setup
- Applications that need admin account creation on first run

## Configuration

### Disable Password Protection (Development)

For local development, you can disable password protection:

**appsettings.Development.json:**

```json
{
  "Setup": {
    "RequirePassword": false
  }
}
```

### Customize Wizard Appearance

Override CSS custom properties to match your brand:

**wwwroot/css/site.css:**

```css
:root {
    --setup-primary-blue: #your-brand-color;
    --setup-font-family: 'Your Font', sans-serif;
}
```

**See [CSS Customization](docs/CSS-CUSTOMIZATION.md) for complete theming guide.**

## Documentation

📚 **[Complete Documentation Index](docs/INDEX.md)** - Start here to find everything

### Getting Started

- **[Creating Custom Steps](docs/CREATING-CUSTOM-STEPS.md)** - Tutorial on building setup steps
- **[Step Design & Styling Guide](docs/STEP-DESIGN-GUIDE.md)** - Best practices for creating beautiful steps
- **[Interactive Prototype](docs/reference/setup-wizard-v2.html)** - Live demo of the setup wizard
- **[Setup Password Protection](docs/SETUP-PASSWORD.md)** - Configure password security

### Customization

- **[CSS Customization](docs/CSS-CUSTOMIZATION.md)** - Theme and style the wizard
- **[Design Guide](docs/DESIGN-GUIDE.md)** - Complete design system reference

### Technical Reference

- **[Architecture Overview](docs/ARCHITECTURE.md)** - Technical design and structure
- **[Project Scope](docs/PROJECT-SCOPE.md)** - Detailed requirements and scope
- **[Product Requirements (PRD)](docs/PRD.md)** - Complete product specifications
- **[User Stories](docs/USER-STORIES.md)** - User stories and acceptance criteria

## Example Application

A complete working example is available in the `cpike.Setup.Middleware.Tester` project, demonstrating:

- Admin account creation
- System role configuration
- Invite system setup
- Review and confirmation flow

## FAQ

### How do I get the setup password?

Check the console output when your application starts, or look for the password file at `/app_data/setup-password.txt`.

### Can I run the setup wizard again?

By default, setup can only run once. Delete the `.setup-complete` marker file to re-run setup.

### How do I customize the wizard appearance?

Override CSS custom properties in your stylesheet. See [CSS Customization](docs/CSS-CUSTOMIZATION.md).

### Can I add steps from a third-party library?

Yes! Third-party libraries can register steps that integrate seamlessly. See [Creating Custom Steps](docs/CREATING-CUSTOM-STEPS.md#third-party-step-registration).

### Does this work with Blazor WebAssembly?

The MVP targets Blazor Server. WebAssembly support is planned for a future release.

## Roadmap

### MVP (v1.0) - In Progress

- ✅ Core middleware and request interception
- ✅ Password protection
- ✅ Setup wizard framework
- ✅ File-based completion tracking
- ✅ Pre-built UI components
- ✅ Comprehensive documentation
- ⏳ Implementation in progress

### Future Releases

- **v1.1**: Database-backed state, additional components, conditional steps
- **v1.2**: Blazor WebAssembly support, multi-tenant setup, localization
- **v2.0**: Plugin system, REST API, advanced validation

## Contributing

This is an open-source project built primarily for personal use but shared with the community. Contributions, issues, and feature requests are welcome!

### How to Contribute

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Setup

```bash
git clone https://github.com/cpike/cpike-setup-middleware.git
cd cpike-setup-middleware
dotnet restore
dotnet build
```

## Support

- 📖 [Documentation](docs/INDEX.md)
- 🐛 [Report Issues](https://github.com/cpike/cpike-setup-middleware/issues)
- 💬 [Discussions](https://github.com/cpike/cpike-setup-middleware/discussions)

## License

MIT License - See LICENSE file for details

---

**Built with ❤️ for the .NET community**

