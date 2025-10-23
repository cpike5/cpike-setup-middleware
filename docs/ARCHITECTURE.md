# Architecture Overview

## High-Level Architecture

The First-Time Setup Middleware follows a layered architecture pattern with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────┐
│                   Blazor Application                    │
│                      (Program.cs)                       │
└────────────────────┬───────────────────────────────────┘
                     │
                     │ 1. Services Registration
                     │    builder.Services.AddSetupWizard()
                     │
┌────────────────────▼───────────────────────────────────┐
│              Service Collection Extensions              │
│            (Setup Builder Configuration)                │
└────────────────────┬───────────────────────────────────┘
                     │
                     │ 2. Middleware Registration
                     │    app.UseSetupMiddleware()
                     │
┌────────────────────▼───────────────────────────────────┐
│                  Setup Middleware                       │
│            (Request Interception Layer)                 │
│  ┌──────────────────────────────────────────────────┐  │
│  │  Check Setup Complete?                           │  │
│  │    ├─ Yes → Continue to next middleware          │  │
│  │    └─ No  → Redirect to /setup                   │  │
│  └──────────────────────────────────────────────────┘  │
└────────────────────┬───────────────────────────────────┘
                     │
                     │ 3. Setup Route Requests
                     │
┌────────────────────▼───────────────────────────────────┐
│                  Setup Wizard Page                      │
│                  (Blazor Component)                     │
│  ┌──────────────────────────────────────────────────┐  │
│  │  - Progress Indicator                            │  │
│  │  - Current Step Renderer                         │  │
│  │  - Navigation Controls                           │  │
│  └──────────────────────────────────────────────────┘  │
└────────────────────┬───────────────────────────────────┘
                     │
                     │ 4. Step Execution
                     │
┌────────────────────▼───────────────────────────────────┐
│               Setup Step Pipeline                       │
│  ┌──────────────────────────────────────────────────┐  │
│  │  Step 1  →  Step 2  →  Step 3  →  Complete      │  │
│  │    ↓          ↓          ↓           ↓           │  │
│  │  Validate  Validate  Validate   Mark Done        │  │
│  │  Execute   Execute   Execute    Create Marker    │  │
│  └──────────────────────────────────────────────────┘  │
└────────────────────┬───────────────────────────────────┘
                     │
                     │ 5. State Persistence
                     │
┌────────────────────▼───────────────────────────────────┐
│            Setup Completion Service                     │
│  ┌──────────────────────────────────────────────────┐  │
│  │  File-based marker:                              │  │
│  │  - Check existence: .setup-complete              │  │
│  │  - Create marker on completion                   │  │
│  │  - Configuration options for path                │  │
│  └──────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
```

## Core Components

### 1. Middleware Layer

**`SetupMiddleware.cs`**

**Responsibilities:**

- Intercept all HTTP requests
- Check if setup is complete
- Redirect to setup wizard if not complete
- Allow setup routes through when not complete
- Pass through all requests when setup is complete

**Key Methods:**

```csharp
public class SetupMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ISetupCompletionService _completionService;
    private readonly SetupOptions _options;

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if setup is complete
        if (await _completionService.IsSetupCompleteAsync())
        {
            // Setup done, continue pipeline
            await _next(context);
            return;
        }

        // Check if current path is setup wizard
        if (context.Request.Path.StartsWithSegments(_options.SetupPath))
        {
            // Allow access to setup wizard
            await _next(context);
            return;
        }

        // Redirect to setup wizard
        context.Response.Redirect(_options.SetupPath);
    }
}
```

### 2. Service Registration

**`ServiceCollectionExtensions.cs`**

**Responsibilities:**

- Register setup services with DI container
- Configure setup options
- Register wizard steps
- Provide builder pattern API

**Key Methods:**

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSetupWizard(
        this IServiceCollection services,
        Action<SetupBuilder> configure)
    {
        // Register core services
        services.AddScoped<ISetupWizardService, SetupWizardService>();
        services.AddSingleton<ISetupCompletionService, FileBasedSetupCompletionService>();
        services.AddScoped<ISetupStateManager, SetupStateManager>();

        // Configure setup builder
        var builder = new SetupBuilder(services);
        configure(builder);

        return services;
    }
}
```

### 3. Setup Builder

**`SetupBuilder.cs`**

**Responsibilities:**

- Fluent API for step registration
- Step ordering and validation
- Configuration binding

**Key Methods:**

```csharp
public class SetupBuilder
{
    private readonly IServiceCollection _services;
    private readonly List<StepRegistration> _steps = new();

    public SetupBuilder AddStep<TStep>() where TStep : class, ISetupStep
    {
        _services.AddScoped<TStep>();
        _steps.Add(new StepRegistration(typeof(TStep)));
        return this;
    }

    public SetupBuilder AddStep<TStep>(int order) where TStep : class, ISetupStep
    {
        _services.AddScoped<TStep>();
        _steps.Add(new StepRegistration(typeof(TStep), order));
        return this;
    }

    public SetupBuilder WithOptions(Action<SetupOptions> configure)
    {
        _services.Configure(configure);
        return this;
    }
}
```

### 4. Step Infrastructure

**`ISetupStep.cs`** (Interface)

```csharp
public interface ISetupStep
{
    string Title { get; }
    string Description { get; }
    int Order { get; }

    Task<ValidationResult> ValidateAsync();
    Task ExecuteAsync();
    Task OnNavigatingFromAsync();
    Task OnNavigatingToAsync();
}
```

**`SetupStepBase.cs`** (Abstract Base Class)

```csharp
public abstract class SetupStepBase : ComponentBase, ISetupStep
{
    [Inject] protected ISetupStateManager StateManager { get; set; }
    [Inject] protected NavigationManager Navigation { get; set; }

    public abstract string Title { get; }
    public abstract string Description { get; }
    public virtual int Order => 0;

    public virtual Task<ValidationResult> ValidateAsync()
        => Task.FromResult(ValidationResult.Success);

    public abstract Task ExecuteAsync();

    public virtual Task OnNavigatingFromAsync()
        => Task.CompletedTask;

    public virtual Task OnNavigatingToAsync()
        => Task.CompletedTask;
}
```

### 5. Wizard Service

**`ISetupWizardService.cs`** / **`SetupWizardService.cs`**

**Responsibilities:**

- Manage wizard state (current step, progress)
- Execute step pipeline
- Handle navigation between steps
- Track completion status

**Key Methods:**

```csharp
public interface ISetupWizardService
{
    int CurrentStepIndex { get; }
    int TotalSteps { get; }
    bool CanNavigateNext { get; }
    bool CanNavigatePrevious { get; }

    Task<ISetupStep> GetCurrentStepAsync();
    Task<IEnumerable<ISetupStep>> GetAllStepsAsync();
    Task<bool> NextStepAsync();
    Task<bool> PreviousStepAsync();
    Task CompleteSetupAsync();
}
```

### 6. State Management

**`ISetupStateManager.cs`** / **`SetupStateManager.cs`**

**Responsibilities:**

- Store wizard state during execution
- Share data between steps
- Maintain step completion status

**Implementation:**

```csharp
public class SetupStateManager : ISetupStateManager
{
    private readonly Dictionary<string, object> _state = new();

    public void Set<T>(string key, T value)
        => _state[key] = value;

    public T Get<T>(string key)
        => _state.ContainsKey(key) ? (T)_state[key] : default;

    public bool TryGet<T>(string key, out T value)
    {
        if (_state.ContainsKey(key))
        {
            value = (T)_state[key];
            return true;
        }
        value = default;
        return false;
    }
}
```

### 7. Completion Service

**`ISetupCompletionService.cs`** / **`FileBasedSetupCompletionService.cs`**

**Responsibilities:**

- Check if setup is complete
- Mark setup as complete
- Persist completion state

**File-Based Implementation:**

```csharp
public class FileBasedSetupCompletionService : ISetupCompletionService
{
    private readonly SetupOptions _options;

    public async Task<bool> IsSetupCompleteAsync()
    {
        var markerPath = GetMarkerFilePath();
        return File.Exists(markerPath);
    }

    public async Task MarkSetupCompleteAsync()
    {
        var markerPath = GetMarkerFilePath();
        await File.WriteAllTextAsync(markerPath,
            JsonSerializer.Serialize(new SetupCompletionMarker
            {
                CompletedAt = DateTime.UtcNow,
                Version = _options.Version
            }));
    }

    private string GetMarkerFilePath()
        => Path.Combine(_options.MarkerDirectory, ".setup-complete");
}
```

## UI Component Architecture

### Wizard Container

**`SetupWizard.razor`**

```
┌─────────────────────────────────────────────────────┐
│                  Wizard Header                      │
│  - Application Logo/Name                            │
│  - Wizard Title                                     │
│  - Description                                      │
├─────────────────────────────────────────────────────┤
│               Progress Indicator                    │
│  ● ─── ○ ─── ○ ─── ○ ─── ○                        │
│  Step 1  Step 2  Step 3  Step 4  Complete          │
├─────────────────────────────────────────────────────┤
│                                                     │
│                 Current Step                        │
│              (Dynamic Component)                    │
│                                                     │
│  [Step-specific content rendered here]              │
│                                                     │
├─────────────────────────────────────────────────────┤
│                 Wizard Footer                       │
│  [← Previous]          [Skip] [Next →]              │
└─────────────────────────────────────────────────────┘
```

### Core UI Components

1. **`SetupWizard.razor`** - Main wizard container
2. **`ProgressIndicator.razor`** - Step progress display
3. **`SetupInput.razor`** - Form input with validation
4. **`SetupCheckbox.razor`** - Checkbox with label and description
5. **`SetupToggle.razor`** - Toggle switch component
6. **`SetupAlert.razor`** - Alert/notification component
7. **`SetupButton.razor`** - Styled button component
8. **`SummarySection.razor`** - Review summary display
9. **`SuccessDisplay.razor`** - Completion success page

## Data Flow

### Setup Wizard Flow

```
1. Application Starts
   └─> Middleware checks completion status
       ├─> Complete: Pass through
       └─> Not Complete: Redirect to /setup

2. Setup Wizard Loads
   └─> SetupWizardService.Initialize()
       ├─> Load all registered steps
       ├─> Sort by Order property
       └─> Set CurrentStepIndex = 0

3. Step Display
   └─> GetCurrentStepAsync()
       ├─> Retrieve step from DI
       ├─> Call OnNavigatingToAsync()
       └─> Render step component

4. User Fills Form
   └─> Step component manages local state
       └─> Optional: Store in StateManager for sharing

5. User Clicks "Next"
   └─> NextStepAsync()
       ├─> Call step.ValidateAsync()
       │   ├─> Invalid: Show errors, stay on step
       │   └─> Valid: Continue
       ├─> Call step.ExecuteAsync()
       │   └─> Perform step actions (save data, configure, etc.)
       ├─> Call step.OnNavigatingFromAsync()
       ├─> Increment CurrentStepIndex
       └─> Load next step

6. Final Step Complete
   └─> CompleteSetupAsync()
       ├─> Execute final step
       ├─> Call CompletionService.MarkSetupCompleteAsync()
       │   └─> Create .setup-complete marker file
       └─> Redirect to application home
```

## Configuration

### SetupOptions

```csharp
public class SetupOptions
{
    public string SetupPath { get; set; } = "/setup";
    public string MarkerDirectory { get; set; } = "./App_Data";
    public string MarkerFileName { get; set; } = ".setup-complete";
    public string Version { get; set; } = "1.0.0";
    public bool AllowSetupRerun { get; set; } = false;
    public List<string> ExcludedPaths { get; set; } = new();
}
```

### Configuration in appsettings.json

```json
{
  "Setup": {
    "SetupPath": "/setup",
    "MarkerDirectory": "./App_Data",
    "Version": "1.0.0",
    "ExcludedPaths": [
      "/health",
      "/api/status"
    ]
  }
}
```

## Dependency Injection Scope

| Service | Lifetime | Reason |
|---------|----------|--------|
| `ISetupCompletionService` | Singleton | Shared state, file I/O, performance |
| `ISetupWizardService` | Scoped | Per-request wizard state |
| `ISetupStateManager` | Scoped | State shared across steps in same session |
| `ISetupStep` implementations | Scoped | Per-request, may have injected scoped dependencies |

## File Structure

```
cpike.Setup.Middleware/
├── Configuration/
│   └── SetupOptions.cs
├── Extensions/
│   ├── ApplicationBuilderExtensions.cs
│   └── ServiceCollectionExtensions.cs
├── Middleware/
│   └── SetupMiddleware.cs
├── Services/
│   ├── ISetupCompletionService.cs
│   ├── FileBasedSetupCompletionService.cs
│   ├── ISetupWizardService.cs
│   ├── SetupWizardService.cs
│   ├── ISetupStateManager.cs
│   └── SetupStateManager.cs
├── Steps/
│   ├── ISetupStep.cs
│   ├── SetupStepBase.cs
│   └── SetupBuilder.cs
├── Components/
│   ├── SetupWizard.razor
│   ├── ProgressIndicator.razor
│   ├── SetupInput.razor
│   ├── SetupCheckbox.razor
│   ├── SetupToggle.razor
│   ├── SetupAlert.razor
│   ├── SetupButton.razor
│   ├── SummarySection.razor
│   └── SuccessDisplay.razor
├── Models/
│   ├── StepRegistration.cs
│   ├── ValidationResult.cs
│   └── SetupCompletionMarker.cs
└── wwwroot/
    └── css/
        └── setup-wizard.css
```

## Security Considerations

### 1. Setup Completion Bypass Prevention

- Marker file stored outside web root
- File permissions set to read-only after creation
- Integrity check on marker file (timestamp, version)

### 2. Step Execution Security

- All step actions run server-side
- Input validation on all form fields
- CSRF protection via Blazor's built-in support
- XSS protection via Blazor's automatic encoding

### 3. Sensitive Data Handling

- No sensitive data stored in wizard state
- Password fields use proper input types
- Credentials hashed before storage
- Clear state after completion

## Performance Considerations

### Middleware Performance

- Early exit when setup complete (single file existence check)
- No database queries in request pipeline
- Minimal allocations

### Wizard Performance

- Steps loaded on-demand
- State manager uses in-memory dictionary (scoped lifetime)
- Progress indicator uses minimal DOM updates
- CSS animations use GPU acceleration

## Extensibility Points

### Custom Step Types

Developers can create custom steps by:

1. Implementing `ISetupStep` interface, OR
2. Inheriting from `SetupStepBase` class

### Custom Completion Services

Developers can replace file-based storage by:

1. Implementing `ISetupCompletionService`
2. Registering custom implementation in DI container

### Custom State Management

Developers can provide custom state storage by:

1. Implementing `ISetupStateManager`
2. Registering custom implementation in DI container

### Custom UI Components

Developers can:

1. Override CSS classes for styling
2. Create custom components using provided base components
3. Replace entire wizard UI by implementing custom `SetupWizard.razor`

## Error Handling

### Middleware Errors

- File access errors: Log and allow through (fail-open)
- Configuration errors: Throw on startup with clear message

### Wizard Errors

- Validation errors: Display inline, prevent navigation
- Execution errors: Display alert, allow retry
- Navigation errors: Log and reset to safe state

### Recovery Strategies

- Invalid state: Clear and restart wizard
- File corruption: Delete marker, allow re-run
- Step failure: Show error, allow previous/retry
- Unhandled exceptions: Global error boundary with helpful message

## Testing Strategy

### Unit Tests

- Middleware logic (setup complete/incomplete paths)
- Wizard service (navigation, state management)
- Completion service (file operations)
- Step validation and execution

### Integration Tests

- Full wizard flow
- Middleware integration with Blazor app
- Step registration and execution pipeline

### Component Tests

- UI component rendering
- User interaction flows
- Validation display
- Navigation buttons

## Future Architecture Enhancements

### Database Provider Pattern

```csharp
public interface ISetupStorageProvider
{
    Task<bool> IsSetupCompleteAsync();
    Task MarkSetupCompleteAsync(SetupCompletionData data);
}

// Implementations:
// - FileBasedStorageProvider
// - SqlServerStorageProvider
// - EntityFrameworkStorageProvider
```

### Step Dependency Graph

```csharp
public interface ISetupStep
{
    IEnumerable<Type> DependsOn { get; }
    bool IsConditional { get; }
    Task<bool> ShouldExecuteAsync();
}
```

### Plugin System

```csharp
public interface ISetupPlugin
{
    string Name { get; }
    void RegisterSteps(SetupBuilder builder);
    void ConfigureServices(IServiceCollection services);
}
```
