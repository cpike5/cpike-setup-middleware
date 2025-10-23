# Product Requirements Document (PRD)

## Product Overview

**Product Name:** cpike.Setup.Middleware
**Version:** 1.0.0 (MVP)
**Target Release:** TBD
**Document Version:** 1.0
**Last Updated:** 2025-10-23

### Executive Summary

cpike.Setup.Middleware is a .NET library that provides a wizard-based first-time setup experience for Blazor Server applications. It enables developers to implement administrative configuration workflows that must be completed before the application becomes accessible to end users.

### Problem Statement

Many Blazor applications require initial configuration (admin account creation, database setup, API keys, etc.) before they can be used. Currently, developers must:

- Build custom setup pages from scratch
- Implement request interception logic manually
- Create state management for multi-step workflows
- Design and style UI components for forms and wizards

This is time-consuming, error-prone, and results in inconsistent user experiences across applications.

### Solution

A lightweight middleware library that provides:

- Automatic request interception until setup is complete
- Pre-built wizard framework with step management
- Professional, responsive UI components
- Simple integration via builder pattern API
- Extensible architecture for custom setup steps

## Target Users

### Primary Persona: Solo Developer

**Name:** Alex
**Role:** Full-stack developer building self-hosted applications
**Goals:**

- Quickly add setup functionality to new projects
- Professional-looking UI without design effort
- Focus on business logic, not boilerplate

**Pain Points:**

- Limited time for building infrastructure features
- Not a designer, struggles with UI/UX
- Wants consistency across multiple projects

### Secondary Persona: Small Team Lead

**Name:** Jordan
**Role:** Tech lead for a small development team
**Goals:**

- Standardize setup experience across company applications
- Enable team members to add setup steps easily
- Maintain code quality and consistency

**Pain Points:**

- Team has varying skill levels
- Need reusable components to save time
- Want to avoid reinventing the wheel

## Product Goals

### Business Goals

1. Reduce time-to-market for Blazor applications with setup requirements
2. Provide open-source alternative to custom setup implementations
3. Build reputation in .NET/Blazor community
4. Create foundation for future commercial plugins/extensions

### User Goals

1. Integrate setup wizard in < 30 minutes
2. Create custom setup steps in < 15 minutes
3. Professional UI with zero design effort
4. Reliable, secure setup process

### Technical Goals

1. Zero breaking changes to existing applications
2. Minimal performance overhead
3. Clean, maintainable codebase
4. Comprehensive documentation and examples

## Functional Requirements

### FR-1: Middleware Integration

**Priority:** P0 (Must Have)

**Description:** The middleware must intercept all HTTP requests and redirect to the setup wizard until setup is completed.

**Acceptance Criteria:**

- [ ] Middleware is registered via `app.UseSetupMiddleware()`
- [ ] All requests redirect to `/setup` when setup is incomplete
- [ ] Requests pass through normally when setup is complete
- [ ] Setup wizard routes are accessible during setup
- [ ] Static files are accessible during setup
- [ ] Performance overhead < 1ms when setup is complete

**User Story:** As a developer, I want the middleware to automatically protect my application until setup is complete, so users cannot access the app in an unconfigured state.

---

### FR-2: Service Registration

**Priority:** P0 (Must Have)

**Description:** Developers must be able to register the setup wizard and its steps via dependency injection.

**Acceptance Criteria:**

- [ ] Services are registered via `builder.Services.AddSetupWizard()`
- [ ] Builder pattern API for step registration
- [ ] Configuration binding from appsettings.json
- [ ] All services properly registered in DI container
- [ ] Clear error messages for misconfiguration

**User Story:** As a developer, I want a simple API to register setup functionality, so I can integrate it into my application with minimal code.

---

### FR-3: Setup Step Framework

**Priority:** P0 (Must Have)

**Description:** Provide a framework for creating and executing setup steps in a defined sequence.

**Acceptance Criteria:**

- [ ] `ISetupStep` interface defines step contract
- [ ] `SetupStepBase` provides base implementation
- [ ] Steps execute in defined order
- [ ] Steps can validate input before proceeding
- [ ] Steps can execute business logic (save data, configure services, etc.)
- [ ] State can be shared between steps
- [ ] Lifecycle hooks: OnNavigatingTo, OnNavigatingFrom

**User Story:** As a developer, I want to create custom setup steps by implementing an interface or base class, so I can add application-specific configuration logic.

---

### FR-4: Wizard Navigation

**Priority:** P0 (Must Have)

**Description:** Users must be able to navigate forward and backward through setup steps.

**Acceptance Criteria:**

- [ ] "Next" button advances to next step
- [ ] "Previous" button returns to previous step
- [ ] "Next" button validates current step before advancing
- [ ] "Previous" button is hidden on first step
- [ ] "Next" button shows "Complete Setup" on final step
- [ ] Validation errors prevent navigation
- [ ] Navigation state persists during wizard session

**User Story:** As an administrator, I want to navigate through setup steps at my own pace, so I can review and correct information as needed.

---

### FR-5: Progress Indicator

**Priority:** P0 (Must Have)

**Description:** Display visual progress indicator showing current step and overall progress.

**Acceptance Criteria:**

- [ ] Progress bar shows percentage complete
- [ ] Step circles indicate completed, current, and upcoming steps
- [ ] Current step is highlighted
- [ ] Completed steps are marked with checkmark or different color
- [ ] Step labels are visible on desktop, hidden on mobile
- [ ] Progress updates when navigating between steps

**User Story:** As an administrator, I want to see my progress through the setup wizard, so I know how many steps remain.

---

### FR-6: Form Input Components

**Priority:** P0 (Must Have)

**Description:** Provide pre-built form components for common input types.

**Acceptance Criteria:**

- [ ] Text input component with label and hint text
- [ ] Password input component
- [ ] Email input component
- [ ] Checkbox component with label and description
- [ ] Toggle switch component
- [ ] All components support validation
- [ ] Validation errors display inline
- [ ] Components use consistent styling

**User Story:** As a developer, I want pre-built form components, so I don't have to create and style them from scratch.

---

### FR-7: Validation Framework

**Priority:** P0 (Must Have)

**Description:** Support client-side and server-side validation with clear error display.

**Acceptance Criteria:**

- [ ] Server-side validation via `ValidateAsync()` method
- [ ] Validation errors prevent navigation to next step
- [ ] Validation errors display with clear messaging
- [ ] Field-level validation highlights invalid fields
- [ ] Summary of validation errors displayed at top of step
- [ ] Custom validation rules can be implemented

**User Story:** As an administrator, I want clear validation feedback, so I can correct errors before proceeding.

---

### FR-8: Setup Completion

**Priority:** P0 (Must Have)

**Description:** Mark setup as complete and persist completion state.

**Acceptance Criteria:**

- [ ] Completion marker file created on successful setup
- [ ] Marker file stored in configurable directory
- [ ] Marker file contains timestamp and version
- [ ] Middleware checks marker file on each request
- [ ] Setup wizard redirects to home page on completion
- [ ] Success page displays confirmation and important information

**User Story:** As an administrator, I want to see confirmation that setup completed successfully, so I know the application is ready to use.

---

### FR-9: State Management

**Priority:** P0 (Must Have)

**Description:** Manage wizard state during execution and share data between steps.

**Acceptance Criteria:**

- [ ] `ISetupStateManager` service for state storage
- [ ] State persists across steps in same session
- [ ] Type-safe get/set methods
- [ ] State is scoped to wizard session
- [ ] State cleared after completion
- [ ] State not persisted to disk (in-memory only for MVP)

**User Story:** As a developer, I want to share data between steps (e.g., admin email entered in step 1 displayed in review step), so I can build cohesive wizard experiences.

---

### FR-10: Review & Confirmation

**Priority:** P1 (Should Have)

**Description:** Provide a review step showing all configured values before final confirmation.

**Acceptance Criteria:**

- [ ] Summary component displays key/value pairs
- [ ] Sensitive values (passwords) are masked
- [ ] Review step appears before final completion
- [ ] "Previous" button allows returning to edit values
- [ ] Clear warning that setup is one-time operation
- [ ] "Complete Setup" button requires explicit confirmation

**User Story:** As an administrator, I want to review all my settings before completing setup, so I can ensure everything is correct.

---

### FR-11: Success Display

**Priority:** P1 (Should Have)

**Description:** Show success page with important information after setup completion.

**Acceptance Criteria:**

- [ ] Success checkmark/icon displayed
- [ ] Success message confirms completion
- [ ] Important credentials/codes displayed (if generated)
- [ ] Copy-to-clipboard buttons for credentials
- [ ] Warning to save credentials securely
- [ ] "Go to Application" button to proceed

**User Story:** As an administrator, I want to see and save any generated credentials, so I can access the application after setup.

---

### FR-12: Responsive Design

**Priority:** P1 (Should Have)

**Description:** Wizard UI must work on mobile and desktop devices.

**Acceptance Criteria:**

- [ ] Wizard container is responsive (max-width on desktop, full-width on mobile)
- [ ] Form inputs are touch-friendly on mobile
- [ ] Buttons are appropriately sized for touch
- [ ] Step labels hidden on small screens
- [ ] Layout adjusts for portrait and landscape
- [ ] Tested on iOS Safari, Android Chrome, desktop browsers

**User Story:** As an administrator, I want to complete setup on any device, so I'm not limited to desktop-only access.

---

### FR-13: Configuration Options

**Priority:** P1 (Should Have)

**Description:** Allow customization of setup behavior via configuration.

**Acceptance Criteria:**

- [ ] Setup path is configurable (default: `/setup`)
- [ ] Marker file directory is configurable
- [ ] Marker file name is configurable
- [ ] Excluded paths can be specified (e.g., health checks)
- [ ] Configuration via appsettings.json supported
- [ ] Configuration via code supported

**User Story:** As a developer, I want to configure setup behavior, so it fits my application's requirements.

---

### FR-14: Step Ordering

**Priority:** P1 (Should Have)

**Description:** Steps execute in a defined, customizable order.

**Acceptance Criteria:**

- [ ] Steps have `Order` property
- [ ] Steps execute in ascending order
- [ ] Steps with same order execute in registration order
- [ ] Order is validated on startup
- [ ] Clear error if order creates conflicts

**User Story:** As a developer, I want to control the order of setup steps, so the wizard flows logically.

---

### FR-15: Error Handling

**Priority:** P1 (Should Have)

**Description:** Graceful error handling with clear messaging.

**Acceptance Criteria:**

- [ ] Step execution errors display user-friendly messages
- [ ] Errors allow retry or navigation to previous step
- [ ] Critical errors (middleware, services) logged
- [ ] File I/O errors handled gracefully
- [ ] Unhandled exceptions caught by error boundary
- [ ] Error messages include actionable guidance

**User Story:** As an administrator, when an error occurs, I want clear information about what went wrong and how to fix it.

## Non-Functional Requirements

### NFR-1: Performance

**Priority:** P0 (Must Have)

| Metric | Target | Measurement |
|--------|--------|-------------|
| Middleware overhead (setup complete) | < 1ms | Average request processing time |
| Wizard page load time | < 500ms | Time to interactive |
| Step navigation | < 200ms | Click to next step render |
| File I/O operations | < 50ms | Marker file read/write |

---

### NFR-2: Security

**Priority:** P0 (Must Have)

**Requirements:**

- [ ] All input validated server-side
- [ ] XSS protection via Blazor's automatic encoding
- [ ] CSRF protection via Blazor's anti-forgery support
- [ ] Marker file stored outside web root
- [ ] No sensitive data in client-side state
- [ ] Passwords hashed before storage
- [ ] Setup completion marker not easily bypassed

---

### NFR-3: Usability

**Priority:** P0 (Must Have)

**Requirements:**

- [ ] Zero documentation required for basic usage
- [ ] Consistent UI following established patterns
- [ ] Clear error messages with actionable guidance
- [ ] Intuitive wizard flow
- [ ] Accessible via keyboard navigation
- [ ] Screen reader compatible (ARIA labels)

---

### NFR-4: Maintainability

**Priority:** P1 (Should Have)

**Requirements:**

- [ ] Clean code following C# conventions
- [ ] Comprehensive XML documentation
- [ ] Separation of concerns (middleware, UI, services)
- [ ] Unit test coverage > 80%
- [ ] Integration tests for critical paths
- [ ] Clear architecture documentation

---

### NFR-5: Compatibility

**Priority:** P0 (Must Have)

**Requirements:**

- [ ] .NET 8.0+ support
- [ ] Blazor Server support
- [ ] Windows/Linux/macOS compatibility
- [ ] Modern browser support (last 2 versions)
  - Chrome
  - Firefox
  - Edge
  - Safari

---

### NFR-6: Extensibility

**Priority:** P1 (Should Have)

**Requirements:**

- [ ] Custom steps can be created without modifying library
- [ ] Custom completion services can replace file-based implementation
- [ ] Custom state management can be provided
- [ ] UI can be customized via CSS overrides
- [ ] Third-party plugins can register steps

## Success Metrics

### Developer Metrics

| Metric | Target | Method |
|--------|--------|--------|
| Integration time | < 30 min | User testing |
| Custom step creation time | < 15 min | User testing |
| Documentation completeness | 100% public API | Review |
| GitHub stars (6 months) | 100+ | Analytics |

### Technical Metrics

| Metric | Target | Method |
|--------|--------|--------|
| Unit test coverage | > 80% | Code coverage tools |
| Build success rate | 100% | CI/CD |
| NuGet download count (6 months) | 500+ | NuGet analytics |

### Quality Metrics

| Metric | Target | Method |
|--------|--------|--------|
| Critical bugs | 0 | Issue tracking |
| Community issues resolution time | < 7 days | GitHub metrics |
| Code maintainability index | > 80 | Static analysis |

## Dependencies

### External Dependencies

| Dependency | Version | Purpose |
|------------|---------|---------|
| Microsoft.AspNetCore.Components | 8.0+ | Blazor components |
| Microsoft.Extensions.DependencyInjection | 8.0+ | Dependency injection |
| Microsoft.Extensions.Configuration | 8.0+ | Configuration binding |
| Microsoft.Extensions.Logging | 8.0+ | Logging support |

### Development Dependencies

| Dependency | Version | Purpose |
|------------|---------|---------|
| xUnit | Latest | Unit testing |
| Moq | Latest | Mocking |
| FluentAssertions | Latest | Test assertions |

## Release Criteria

### MVP Release Checklist

#### Core Functionality

- [ ] Middleware intercepts requests correctly
- [ ] Service registration works
- [ ] Steps execute in correct order
- [ ] Validation prevents invalid progression
- [ ] Setup completion persists correctly
- [ ] State management works across steps

#### UI Components

- [ ] All planned components implemented
- [ ] Responsive design works on mobile/desktop
- [ ] Styling matches prototype
- [ ] Accessibility requirements met
- [ ] Browser compatibility verified

#### Documentation

- [ ] README with quick start
- [ ] API documentation (XML comments)
- [ ] Developer guide for custom steps
- [ ] CSS customization guide
- [ ] Architecture overview
- [ ] Example application

#### Testing

- [ ] Unit tests written for core logic
- [ ] Integration tests for middleware
- [ ] Component tests for UI
- [ ] Manual testing on all supported browsers
- [ ] Example app demonstrates all features

#### Quality

- [ ] No critical bugs
- [ ] No security vulnerabilities
- [ ] Code review completed
- [ ] Static analysis passing
- [ ] Performance benchmarks met

## Future Enhancements

### Version 1.1 (Planned)

- Database-backed setup completion storage
- Additional form components (date picker, file upload, rich text)
- Conditional steps (show based on previous choices)
- Step templates (common scenarios pre-built)

### Version 1.2 (Consideration)

- Blazor WebAssembly support
- Multi-tenant setup tracking
- Setup re-run capability
- Migration wizard support
- Localization (i18n)

### Version 2.0 (Vision)

- Plugin system for third-party integrations
- Setup wizard themes
- REST API for headless setup
- Step dependency graph
- Advanced validation framework

## Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| File permission issues in production | High | Medium | Document requirements; graceful degradation |
| Browser compatibility issues | Medium | Low | Test on all major browsers; document requirements |
| Performance impact on large apps | Medium | Low | Benchmark; optimize middleware |
| API design changes needed | High | Medium | Community feedback before 1.0; semantic versioning |
| Low adoption | Medium | Medium | Marketing; examples; blog posts; conference talks |

## Appendix

### Glossary

- **Setup Wizard**: Multi-step UI for configuring the application
- **Setup Step**: Individual configuration screen in the wizard
- **Marker File**: File that indicates setup completion
- **Setup State**: Data shared between steps during wizard execution
- **Completion Service**: Service responsible for tracking setup completion

### References

- [Blazor Documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/)
- [ASP.NET Core Middleware](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/)
- [UI Prototype](../reference/first-time-setup-wizard.html)

### Changelog

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2025-10-23 | Initial PRD |
