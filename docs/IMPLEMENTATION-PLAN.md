# Implementation Plan

**Project:** cpike.Setup.Middleware - First-Time Setup Wizard for .NET Blazor Applications
**Version:** 1.0.0 (MVP)
**Last Updated:** 2025-10-23

## Overview

This document provides a phased implementation plan for building the cpike.Setup.Middleware library, based on the comprehensive requirements documented in the PRD, Architecture, and User Stories documents.

---

## Phase 1: Core Foundation (Priority P0)

### 1.1 Middleware Layer
**Goal:** Implement request interception and setup completion detection

**Tasks:**
- Enhance `SetupMiddleware.cs` with completion checking logic
- Implement path exclusion logic (static files, health checks)
- Add performance optimization (early exit when complete)
- Create `ISetupCompletionService` interface
- Build `FileBasedSetupCompletionService` implementation
- Implement marker file creation/validation with timestamp and version

**Files to Create/Modify:**
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/SetupMiddleware.cs` (enhance)
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Services/ISetupCompletionService.cs` (create)
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Services/FileBasedSetupCompletionService.cs` (create)
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Models/SetupCompletionMarker.cs` (create)

**Acceptance Criteria:**
- Middleware overhead < 1ms when setup complete
- All requests redirect to `/setup` when incomplete
- Static files accessible during setup
- Marker file stored outside web root
- File contains timestamp and version information

**User Stories:** US-005, US-006

---

### 1.2 Service Registration & Configuration
**Goal:** Fluent builder pattern API for easy integration

**Tasks:**
- Enhance `ServiceCollectionExtensions.cs` with `AddSetupWizard()` method
- Create `SetupBuilder` class with fluent step registration
- Implement configuration binding from appsettings.json
- Enhance `SetupOptions.cs` with all required options
- Add startup validation for misconfiguration

**Files to Create/Modify:**
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Extensions/ServiceCollectionExtensions.cs` (enhance)
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Configuration/SetupOptions.cs` (enhance)
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Steps/SetupBuilder.cs` (create)
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Models/StepRegistration.cs` (create)

**Expected API:**
```csharp
builder.Services.AddSetupWizard(setup =>
{
    setup.AddStep<AdminAccountStep>();
    setup.AddStep<DatabaseStep>(order: 20);
    setup.WithOptions(o =>
    {
        o.SetupPath = "/setup";
        o.MarkerDirectory = "./App_Data";
    });
});
```

**Acceptance Criteria:**
- Integration requires < 5 lines of code
- IntelliSense provides helpful suggestions
- Configuration validates at startup
- All services properly registered in DI container

**User Stories:** US-001, US-002, US-003

---

### 1.3 Step Framework
**Goal:** Extensible step-based architecture

**Tasks:**
- Create `ISetupStep` interface (Title, Description, Order, lifecycle methods)
- Build `SetupStepBase` abstract class with common functionality
- Implement `ISetupStateManager` for cross-step data sharing
- Create `ValidationResult` model
- Build step registration and ordering system
- Implement step lifecycle hooks (OnNavigatingTo, OnNavigatingFrom)

**Files to Create:**
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Steps/ISetupStep.cs`
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Steps/SetupStepBase.cs`
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Services/ISetupStateManager.cs`
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Services/SetupStateManager.cs`
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Models/ValidationResult.cs`

**ISetupStep Interface:**
```csharp
public interface ISetupStep
{
    string Title { get; }
    string Description { get; }
    int Order { get; }

    Task<ValidationResult> ValidateAsync();
    Task ExecuteAsync();
    Task OnNavigatingToAsync();
    Task OnNavigatingFromAsync();
}
```

**Acceptance Criteria:**
- Steps can validate user input
- Steps can execute business logic
- Steps can share data via state manager
- Lifecycle hooks execute at correct times
- Steps execute in defined order

**User Stories:** US-007, US-008, US-009, US-010, US-011

---

## Phase 2: Wizard Service & Navigation (Priority P0)

### 2.1 Wizard Management Service
**Goal:** Orchestrate wizard flow and state

**Tasks:**
- Create `ISetupWizardService` interface
- Implement `SetupWizardService` with navigation logic
- Add current step tracking
- Implement progress calculation
- Build validation execution pipeline
- Create completion handling

**Files to Create:**
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Services/ISetupWizardService.cs`
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Services/SetupWizardService.cs`

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

**Acceptance Criteria:**
- Navigation validates before proceeding
- State persists across steps
- Progress calculated correctly
- Completion triggers marker creation

**User Stories:** US-012

---

### 2.2 Blazor Wizard Component
**Goal:** Main wizard UI container

**Tasks:**
- Create `SetupWizard.razor` main container
- Implement wizard routing
- Build navigation buttons with state management
- Add loading states during async operations
- Implement error boundary for graceful failures

**Files to Create:**
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Components/SetupWizard.razor`
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Components/SetupWizard.razor.cs`
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Pages/Setup.razor`

**UI Structure:**
```
SetupWizard.razor
├── WizardHeader (logo, title)
├── ProgressIndicator (steps, percentage)
├── DynamicComponent (current step)
└── WizardFooter (Previous/Next/Complete buttons)
```

**Acceptance Criteria:**
- Wizard displays current step dynamically
- Navigation buttons work correctly
- Loading states prevent double-submission
- Error boundary catches failures

**User Stories:** US-012

---

## Phase 3: Security & Password Protection (Priority P0)

### 3.1 Password Generation & Validation
**Goal:** Secure setup wizard access

**Tasks:**
- Create `ISetupPasswordService` interface
- Implement secure random password generation
- Build password validation with rate limiting
- Add console output formatting
- Create password file persistence
- Implement session-based verification

**Files to Create:**
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Services/ISetupPasswordService.cs`
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Services/SetupPasswordService.cs`
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Models/PasswordValidationResult.cs`

**Password Format:** `XpK7-mN94-Qr2L-vB8j` (4 segments, 4 chars each, alphanumeric)

**Security Features:**
- Rate limiting (max 5 attempts, exponential backoff)
- Cryptographic RNG for generation
- Session-based authentication
- Optional requirement toggle

**Acceptance Criteria:**
- Password generated on first access
- Password logged to console and file
- Validation prevents brute force
- Session persists password verification

**User Stories:** US-004

---

### 3.2 Password Entry UI
**Goal:** Professional password prompt screen

**Tasks:**
- Create `PasswordPrompt.razor` component
- Build password validation UI
- Display helpful instructions
- Show attempt counter
- Add smooth transition on success

**Files to Create:**
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Components/PasswordPrompt.razor`
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Components/PasswordPrompt.razor.cs`

**Acceptance Criteria:**
- Clear instructions on finding password
- Validation feedback on errors
- Shows remaining attempts
- Professional design

**User Stories:** US-014

---

## Phase 4: UI Components Library (Priority P0)

### 4.1 Form Input Components
**Goal:** Pre-built, styled form controls

**Components to Create:**
- `SetupInput.razor` - Text/email/password input
- `SetupCheckbox.razor` - Checkbox with description
- `SetupToggle.razor` - Toggle switch
- `SetupButton.razor` - Button variants
- `SetupAlert.razor` - Alert messages

**Files to Create:**
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Components/SetupInput.razor`
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Components/SetupCheckbox.razor`
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Components/SetupToggle.razor`
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Components/SetupButton.razor`
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Components/SetupAlert.razor`

**Features:**
- Server-side validation support
- Inline error display
- Consistent styling
- Accessibility (ARIA labels)

**Acceptance Criteria:**
- All components support validation
- Consistent styling across components
- Touch-friendly on mobile
- Documented with examples

**User Stories:** US-020, US-021, US-022

---

### 4.2 Progress & Summary Components
**Goal:** Visual feedback and review functionality

**Components to Create:**
- `ProgressIndicator.razor` - Steps visualization
- `SummarySection.razor` - Configuration review
- `SuccessDisplay.razor` - Completion page

**Files to Create:**
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Components/ProgressIndicator.razor`
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Components/SummarySection.razor`
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Components/SuccessDisplay.razor`

**Acceptance Criteria:**
- Progress shows percentage and step status
- Summary masks sensitive values
- Success page displays credentials
- Copy-to-clipboard functionality

**User Stories:** US-013, US-016, US-017

---

## Phase 5: Styling & Responsive Design (Priority P0)

### 5.1 CSS Framework
**Goal:** Professional, mobile-responsive styling

**Tasks:**
- Extract CSS from HTML prototype
- Implement CSS custom properties
- Build responsive breakpoints
- Add animations and transitions
- Ensure accessibility compliance

**Files to Create:**
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/wwwroot/css/setup-wizard.css`
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/wwwroot/css/setup-wizard-components.css`

**CSS Variables:**
```css
:root {
    --setup-primary-blue: #4A90E2;
    --setup-success-green: #7ED321;
    --setup-warning-orange: #F5A623;
    --setup-error-red: #D0021B;
    --setup-font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto;
    --setup-border-radius: 8px;
    --setup-spacing-unit: 16px;
}
```

**Breakpoints:**
- Mobile: < 768px
- Tablet: 768px - 1024px
- Desktop: > 1024px

**Acceptance Criteria:**
- Works on mobile and desktop
- Touch-friendly buttons
- Smooth animations
- High contrast for accessibility

**User Stories:** US-018, US-019

---

### 5.2 Accessibility Implementation
**Goal:** WCAG 2.1 Level AA compliance

**Tasks:**
- Add ARIA labels to all elements
- Implement keyboard navigation
- Ensure screen reader compatibility
- Add focus indicators
- Test with screen readers

**Acceptance Criteria:**
- Full keyboard navigation
- Screen reader announces changes
- Focus indicators visible
- Meets WCAG 2.1 Level AA

**User Stories:** US-019

---

## Phase 6: Documentation & Examples (Priority P0)

### 6.1 Code Documentation
**Goal:** Comprehensive inline and API docs

**Tasks:**
- Add XML documentation to all public APIs
- Document lifecycle and execution order
- Create inline code examples
- Build API reference

**Acceptance Criteria:**
- 100% XML documentation coverage
- Examples in XML comments
- Clear parameter descriptions
- Exception documentation

**User Stories:** US-028, US-029

---

### 6.2 Example Application
**Goal:** Working reference implementation

**Tasks:**
- Build complete example in Tester project
- Implement realistic steps
- Add comprehensive comments
- Create example README

**Example Steps:**
- `AdminAccountStep.razor` - Admin creation with Identity
- `DatabaseConfigStep.razor` - Connection string configuration
- `SystemRolesStep.razor` - Role setup
- `EmailConfigStep.razor` - SMTP configuration
- `ReviewStep.razor` - Summary and confirmation

**Files to Create:**
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware.Tester/Steps/AdminAccountStep.razor`
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware.Tester/Steps/DatabaseConfigStep.razor`
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware.Tester/Steps/SystemRolesStep.razor`
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware.Tester/Steps/EmailConfigStep.razor`
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware.Tester/Steps/ReviewStep.razor`
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware.Tester/README.md`

**Acceptance Criteria:**
- Example runs without errors
- Demonstrates all features
- Well-commented code
- Realistic use cases

**User Stories:** US-030

---

## Phase 7: Testing (Priority P0)

### 7.1 Unit Tests
**Goal:** Core logic validation

**Test Coverage:**
- Middleware request interception
- Setup completion service
- Wizard service navigation
- State manager
- Step validation
- Password service

**Test Project:**
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware.Tests/cpike.Setup.Middleware.Tests.csproj`

**Test Classes to Create:**
- `SetupMiddlewareTests.cs`
- `FileBasedSetupCompletionServiceTests.cs`
- `SetupWizardServiceTests.cs`
- `SetupStateManagerTests.cs`
- `SetupPasswordServiceTests.cs`

**Framework:** xUnit + Moq + FluentAssertions

**Target:** > 80% code coverage

---

### 7.2 Integration Tests
**Goal:** End-to-end workflow validation

**Test Scenarios:**
- Full wizard completion flow
- Middleware integration
- Step execution pipeline
- Password protection workflow
- Error handling

**Test Class:**
- `IntegrationTests/SetupWizardIntegrationTests.cs`

---

## Phase 8: Polish & Release Preparation (Priority P1)

### 8.1 Error Handling
**Tasks:**
- Implement global error boundary
- Add user-friendly error messages
- Build retry mechanisms
- Add comprehensive logging
- Test error scenarios

**Files to Create:**
- `src/cpike.Setup.Middleware/cpike.Setup.Middleware/Components/ErrorBoundary.razor`

**User Stories:** US-026, US-027

---

### 8.2 Performance Optimization
**Tasks:**
- Benchmark middleware overhead
- Optimize file I/O operations
- Minimize memory allocations
- Profile rendering performance
- Add caching where appropriate

**Target Metrics:**
- Middleware overhead: < 1ms (setup complete)
- Wizard page load: < 500ms
- Step navigation: < 200ms
- File I/O: < 50ms

---

### 8.3 Configuration & Extensibility
**Tasks:**
- Document all configuration options
- Create custom completion service example
- Show third-party step registration
- Build sample theme overrides

**Documentation Updates:**
- Configuration guide
- Extensibility guide
- Theme customization examples

**User Stories:** US-023, US-024, US-025

---

## Phase 9: Future Enhancements (Priority P2)

### 9.1 Database Provider Pattern
- Abstract storage with `ISetupStorageProvider`
- Implement SQL Server provider
- Entity Framework provider

### 9.2 Advanced Features
- Conditional step execution
- Step dependency graph
- Draft/resume functionality
- Setup wizard themes
- Localization support
- Blazor WebAssembly support

---

## Implementation Timeline

### Sprint 1 (3-4 weeks): Foundation
**Deliverables:**
- Phase 1.1: Middleware Layer ✓
- Phase 1.2: Service Registration ✓
- Phase 1.3: Step Framework ✓
- Basic middleware functional

**Milestone:** Request interception works, steps can be registered

---

### Sprint 2 (2-3 weeks): Wizard & Security
**Deliverables:**
- Phase 2.1: Wizard Service ✓
- Phase 2.2: Wizard Component ✓
- Phase 3.1: Password Service ✓
- Phase 3.2: Password UI ✓

**Milestone:** Working wizard with password protection

---

### Sprint 3 (2 weeks): UI & Example
**Deliverables:**
- Phase 4.1: Form Components ✓
- Phase 4.2: Progress Components ✓
- Phase 5.1: CSS Framework ✓
- Phase 6.2: Example Application ✓

**Milestone:** Professional UI with working example

---

### Sprint 4 (1-2 weeks): Documentation & Testing
**Deliverables:**
- Phase 6.1: Code Documentation ✓
- Phase 7.1: Unit Tests ✓
- Phase 7.2: Integration Tests ✓
- Phase 8: Polish & Optimization ✓

**Milestone:** MVP ready for release

---

**Total MVP Timeline:** 8-12 weeks

---

## Critical Path

```
Middleware Layer
    ↓
Completion Service
    ↓
Step Framework
    ↓
Wizard Service
    ↓
Wizard Component
    ↓
UI Components
    ↓
Example Application
    ↓
Testing & Documentation
```

---

## Key Dependencies

**External:**
- Microsoft.AspNetCore.Components (8.0+)
- Microsoft.Extensions.DependencyInjection (8.0+)
- Microsoft.Extensions.Configuration (8.0+)
- Microsoft.Extensions.Logging (8.0+)

**Development:**
- xUnit (latest)
- Moq (latest)
- FluentAssertions (latest)

---

## Risks & Mitigations

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| File permission issues in production | High | Medium | Clear documentation; graceful degradation |
| Browser compatibility issues | Medium | Low | Test on all major browsers; document requirements |
| Performance impact on large apps | Medium | Low | Benchmark early; optimize middleware |
| API design changes needed | High | Medium | Community feedback before 1.0; semantic versioning |
| Step ordering conflicts | Medium | Medium | Clear error messages; validation at startup |

---

## Success Metrics

**MVP Complete When:**
- ✅ Integration takes < 30 minutes
- ✅ Custom step creation < 15 minutes
- ✅ Zero breaking changes to existing apps
- ✅ Professional UI matches prototype
- ✅ Working example demonstrates all features
- ✅ All P0 documentation complete
- ✅ Unit test coverage > 80%
- ✅ No critical bugs
- ✅ Performance benchmarks met

---

## Reference Documents

- [README.md](../README.md) - Project overview
- [PROJECT-SCOPE.md](PROJECT-SCOPE.md) - Scope and goals
- [ARCHITECTURE.md](ARCHITECTURE.md) - Technical architecture
- [PRD.md](PRD.md) - Product requirements
- [USER-STORIES.md](USER-STORIES.md) - User stories and acceptance criteria
- [DESIGN-GUIDE.md](DESIGN-GUIDE.md) - Design system
- [CREATING-CUSTOM-STEPS.md](CREATING-CUSTOM-STEPS.md) - Developer guide

---

**Last Updated:** 2025-10-23
**Status:** Ready for Implementation
