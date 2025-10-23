# User Stories

## Epic: First-Time Setup Middleware

### Overview

This document contains user stories for the cpike.Setup.Middleware project, organized by user persona and feature area.

## Personas

### P1: Solo Developer (Alex)

Full-stack developer building self-hosted Blazor applications for personal or small client projects.

### P2: Small Team Lead (Jordan)

Tech lead managing a team of 3-5 developers building internal business applications.

### P3: Application Administrator (Sam)

System administrator or first-time user responsible for configuring a newly deployed application.

---

## Feature Area: Integration

### US-001: Quick Integration

**As a** solo developer (P1)
**I want to** add setup functionality to my Blazor app with minimal code
**So that** I can focus on building features instead of infrastructure

**Acceptance Criteria:**

- Integration requires < 5 lines of code in Program.cs
- Clear error messages if configuration is invalid
- Works without breaking existing application functionality
- Installation via NuGet package

**Priority:** P0
**Estimate:** 3 points

---

### US-002: Service Registration

**As a** developer (P1, P2)
**I want to** register setup services using a fluent API
**So that** the integration feels natural and follows .NET conventions

**Acceptance Criteria:**

- Uses builder pattern (e.g., `AddSetupWizard(setup => { ... })`)
- IntelliSense provides helpful suggestions
- Registration validates configuration at startup
- All services properly registered in DI container

**Priority:** P0
**Estimate:** 5 points

---

### US-003: Configuration Options

**As a** team lead (P2)
**I want to** configure setup behavior via appsettings.json
**So that** settings can differ between development, staging, and production

**Acceptance Criteria:**

- Setup path is configurable
- Marker file location is configurable
- Excluded paths can be specified
- Setup password requirement can be enabled/disabled
- Configuration values validated on startup

**Priority:** P1
**Estimate:** 3 points

---

## Feature Area: Security

### US-004: Setup Access Protection

**As a** developer (P1, P2)
**I want to** protect the setup wizard with a generated password
**So that** unauthorized users cannot access setup in publicly exposed deployments

**Acceptance Criteria:**

- Secure random password generated on first access
- Password logged to console and/or file
- Password must be entered before accessing setup wizard
- Password validation prevents brute force (rate limiting)
- Option to disable password protection for private environments
- Default: password protection enabled

**Priority:** P0
**Estimate:** 5 points

**Notes:**

Example console output:

```
========================================
SETUP WIZARD ACCESS PASSWORD
========================================
Your setup wizard is protected.
Enter this password to begin setup:

    XpK7-mN94-Qr2L-vB8j

This password is valid for this session only.
For security, it has also been written to:
    /app_data/setup-password.txt
========================================
```

---

### US-005: Request Interception

**As a** developer (P1, P2)
**I want** all requests automatically redirected to setup wizard
**So that** users cannot access the application until it's configured

**Acceptance Criteria:**

- All HTTP requests redirect to `/setup` when setup incomplete
- Static files (CSS, JS, images) are accessible during setup
- Health check endpoints can be excluded
- Setup routes are accessible during setup process
- Middleware has minimal performance impact (<1ms when complete)

**Priority:** P0
**Estimate:** 5 points

---

### US-006: Setup Completion Persistence

**As a** developer (P1, P2)
**I want** setup completion status persisted reliably
**So that** the wizard only runs once and can't be easily bypassed

**Acceptance Criteria:**

- Completion marker file created on successful setup
- File stored outside web root
- File contains timestamp and version information
- File cannot be easily forged or bypassed
- Clear error if file cannot be created

**Priority:** P0
**Estimate:** 3 points

---

## Feature Area: Wizard Framework

### US-007: Custom Setup Steps

**As a** developer (P1, P2)
**I want to** create custom setup steps by implementing an interface
**So that** I can add application-specific configuration logic

**Acceptance Criteria:**

- `ISetupStep` interface clearly defines contract
- `SetupStepBase` abstract class provides defaults
- Steps can access dependency injection
- Steps can validate user input
- Steps can execute business logic
- Steps can share data via state manager
- Clear documentation on creating custom steps

**Priority:** P0
**Estimate:** 8 points

---

### US-008: Step Ordering

**As a** developer (P1, P2)
**I want to** control the order in which steps execute
**So that** the wizard flows logically

**Acceptance Criteria:**

- Steps have `Order` property
- Lower numbers execute first
- Steps with same order execute in registration sequence
- Order validated at startup
- Clear error message for ordering conflicts

**Priority:** P1
**Estimate:** 3 points

---

### US-009: Step Validation

**As a** developer (P1, P2)
**I want** steps to validate user input before proceeding
**So that** invalid data doesn't get saved or cause errors

**Acceptance Criteria:**

- `ValidateAsync()` method on step interface
- Validation executes before "Next" navigation
- Validation errors prevent navigation
- Errors display clearly to user
- Can return multiple validation errors
- Field-level and step-level validation supported

**Priority:** P0
**Estimate:** 5 points

---

### US-010: State Management Between Steps

**As a** developer (P1, P2)
**I want to** share data between steps (e.g., email from step 1 shown in review step 5)
**So that** I can build cohesive wizard experiences

**Acceptance Criteria:**

- `ISetupStateManager` service available via DI
- Type-safe get/set methods
- State persists across steps in same session
- State cleared after setup completion
- State scoped to wizard session (not global)

**Priority:** P0
**Estimate:** 3 points

---

### US-011: Step Lifecycle Hooks

**As a** developer (P1, P2)
**I want** hooks that execute when navigating to/from steps
**So that** I can initialize data or cleanup resources

**Acceptance Criteria:**

- `OnNavigatingToAsync()` called before step displays
- `OnNavigatingFromAsync()` called when leaving step
- Hooks can be async
- Hooks have access to state manager
- Clear documentation on hook execution order

**Priority:** P1
**Estimate:** 3 points

---

## Feature Area: User Experience

### US-012: Wizard Navigation

**As an** administrator (P3)
**I want to** navigate forward and backward through setup steps
**So that** I can review and correct information as needed

**Acceptance Criteria:**

- "Next" button advances to next step
- "Previous" button returns to previous step
- "Previous" hidden on first step
- "Next" validates before proceeding
- "Next" shows "Complete Setup" on final step
- Navigation disabled during async operations

**Priority:** P0
**Estimate:** 5 points

---

### US-013: Progress Indicator

**As an** administrator (P3)
**I want to** see my progress through the wizard
**So that** I know how many steps remain

**Acceptance Criteria:**

- Progress bar shows percentage complete
- Step circles show completed/current/upcoming
- Current step highlighted
- Completed steps marked with checkmark
- Smooth animations between steps
- Responsive design (hide labels on mobile)

**Priority:** P0
**Estimate:** 5 points

---

### US-014: Password Access Screen

**As an** administrator (P3)
**I want** a clear screen prompting for the setup password
**So that** I know I need to find the password before proceeding

**Acceptance Criteria:**

- Password prompt shown before wizard
- Clear instructions on where to find password
- Password field masked
- Validation feedback on incorrect password
- Professional, clean design
- Shows number of attempts remaining (if rate limited)

**Priority:** P0
**Estimate:** 5 points

---

### US-015: Validation Feedback

**As an** administrator (P3)
**I want** clear validation feedback when I enter invalid data
**So that** I can correct errors before proceeding

**Acceptance Criteria:**

- Validation errors display inline below fields
- Invalid fields highlighted (red border)
- Error summary at top of step
- Specific, actionable error messages
- Errors clear when field is corrected
- Prevents form submission when invalid

**Priority:** P0
**Estimate:** 5 points

---

### US-016: Review & Confirmation

**As an** administrator (P3)
**I want to** review all my settings before finalizing setup
**So that** I can ensure everything is correct before committing

**Acceptance Criteria:**

- Review step shows all configured values
- Sensitive values (passwords) are masked
- Grouped by logical sections
- "Previous" button allows editing
- Warning that setup is one-time operation
- "Complete Setup" button requires explicit confirmation

**Priority:** P1
**Estimate:** 5 points

---

### US-017: Success Confirmation

**As an** administrator (P3)
**I want** confirmation that setup completed successfully
**So that** I know the application is ready to use

**Acceptance Criteria:**

- Success screen with checkmark/icon
- Confirmation message displayed
- Generated credentials/codes shown (if any)
- Copy-to-clipboard buttons
- Warning to save credentials
- "Go to Application" button to proceed

**Priority:** P1
**Estimate:** 3 points

---

### US-018: Responsive Design

**As an** administrator (P3)
**I want** the setup wizard to work on my phone or tablet
**So that** I'm not limited to desktop-only access

**Acceptance Criteria:**

- Wizard responsive on mobile and desktop
- Touch-friendly buttons and inputs
- Layout adjusts for small screens
- Works in portrait and landscape
- Tested on iOS and Android
- No horizontal scrolling

**Priority:** P1
**Estimate:** 5 points

---

### US-019: Accessibility

**As an** administrator with accessibility needs (P3)
**I want** the wizard to be accessible via keyboard and screen reader
**So that** I can complete setup regardless of my abilities

**Acceptance Criteria:**

- Full keyboard navigation support
- ARIA labels on all interactive elements
- Focus indicators visible
- Screen reader announces step changes
- Error messages read by screen reader
- Meets WCAG 2.1 Level AA guidelines

**Priority:** P1
**Estimate:** 5 points

---

## Feature Area: UI Components

### US-020: Pre-Built Form Components

**As a** developer (P1, P2)
**I want** pre-built, styled form components
**So that** I don't have to create and style them from scratch

**Acceptance Criteria:**

- Text input component
- Password input component
- Email input component
- Checkbox component with description
- Toggle switch component
- Components support validation
- Consistent styling across components
- Components are documented with examples

**Priority:** P0
**Estimate:** 8 points

---

### US-021: Alert/Notification Component

**As a** developer (P1, P2)
**I want** alert components for showing info, warnings, and success messages
**So that** I can communicate important information to administrators

**Acceptance Criteria:**

- Info, warning, success, and error variants
- Consistent styling with color coding
- Optional icon support
- Dismissible or persistent
- Accessible (ARIA roles)

**Priority:** P1
**Estimate:** 3 points

---

### US-022: Button Components

**As a** developer (P1, P2)
**I want** styled button components
**So that** navigation and actions look consistent

**Acceptance Criteria:**

- Primary, secondary, and outline variants
- Disabled state
- Loading state (spinner)
- Size variants (small, medium, large)
- Hover and focus states
- Accessible (keyboard, screen reader)

**Priority:** P0
**Estimate:** 3 points

---

## Feature Area: Extensibility

### US-023: Third-Party Step Registration

**As a** third-party library author (P2)
**I want to** bundle setup steps with my library
**So that** users can configure my feature during initial setup

**Acceptance Criteria:**

- Steps can be registered from any assembly
- Steps integrate seamlessly with existing wizard
- Step order can be controlled
- No conflicts with other libraries
- Clear documentation on bundling steps

**Priority:** P1
**Estimate:** 5 points

---

### US-024: Custom Completion Storage

**As a** developer (P1, P2)
**I want to** replace file-based storage with database storage
**So that** setup completion works in environments without file system access

**Acceptance Criteria:**

- `ISetupCompletionService` interface
- Can implement custom storage provider
- Can register custom provider in DI
- Built-in file-based provider works by default
- Documentation on creating custom providers

**Priority:** P2
**Estimate:** 5 points

---

### US-025: CSS Customization

**As a** developer (P1, P2)
**I want to** override CSS styles to match my application's branding
**So that** the wizard feels integrated with my app

**Acceptance Criteria:**

- CSS uses CSS custom properties (variables)
- All colors definable via variables
- Documentation lists all customizable properties
- Can override specific components
- Can replace entire stylesheet
- Example custom themes provided

**Priority:** P1
**Estimate:** 3 points

---

## Feature Area: Error Handling

### US-026: Graceful Error Handling

**As an** administrator (P3)
**When** an error occurs during setup
**I want** clear information about what went wrong
**So that** I can fix the issue and continue

**Acceptance Criteria:**

- User-friendly error messages (not stack traces)
- Actionable guidance on fixing issues
- Ability to retry failed operations
- Ability to navigate to previous step
- Errors logged for debugging
- Error boundary catches unhandled exceptions

**Priority:** P1
**Estimate:** 5 points

---

### US-027: Configuration Validation

**As a** developer (P1, P2)
**When** I misconfigure the setup wizard
**I want** clear error messages at startup
**So that** I can fix configuration before deploying

**Acceptance Criteria:**

- Configuration validated on startup
- Missing required settings cause clear errors
- Invalid paths/values cause clear errors
- Errors include suggestion for fix
- Application fails fast (doesn't start)

**Priority:** P1
**Estimate:** 3 points

---

## Feature Area: Documentation & Examples

### US-028: Quick Start Guide

**As a** developer (P1, P2)
**I want** a quick start guide that gets me running in minutes
**So that** I can evaluate the library quickly

**Acceptance Criteria:**

- Guide shows installation
- Guide shows basic integration
- Guide shows creating first custom step
- Guide uses realistic example
- Can complete guide in < 30 minutes
- Code samples work when copied

**Priority:** P0
**Estimate:** 3 points

---

### US-029: Developer Guide for Custom Steps

**As a** developer (P1, P2)
**I want** comprehensive documentation on creating custom steps
**So that** I can implement complex setup logic

**Acceptance Criteria:**

- Explains `ISetupStep` interface
- Explains `SetupStepBase` class
- Shows validation examples
- Shows state management examples
- Shows dependency injection usage
- Includes multiple realistic examples
- Explains lifecycle hooks

**Priority:** P0
**Estimate:** 5 points

---

### US-030: Example Application

**As a** developer (P1, P2)
**I want** a working example application
**So that** I can see the library in action and reference implementations

**Acceptance Criteria:**

- Example includes common setup scenarios:
  - Admin account creation
  - Database connection configuration
  - Feature toggles
  - Email/SMTP setup
  - Review and confirmation
- Example is well-commented
- Example demonstrates best practices
- Example can be run locally

**Priority:** P0
**Estimate:** 8 points

---

## Epic Summary

### Story Points by Priority

| Priority | Stories | Total Points |
|----------|---------|--------------|
| P0 (Must Have) | 17 | 89 |
| P1 (Should Have) | 12 | 52 |
| P2 (Could Have) | 1 | 5 |
| **Total** | **30** | **146** |

### Story Points by Feature Area

| Feature Area | Stories | Points |
|--------------|---------|--------|
| Integration | 3 | 11 |
| Security | 3 | 13 |
| Wizard Framework | 5 | 22 |
| User Experience | 8 | 46 |
| UI Components | 3 | 14 |
| Extensibility | 3 | 13 |
| Error Handling | 2 | 8 |
| Documentation | 3 | 16 |

### MVP Scope

For the MVP release, focus on **P0 (Must Have)** stories totaling **89 story points**.

### Sprint Planning Suggestion

Assuming a 2-week sprint with capacity for 20-25 points:

**Sprint 1: Foundation (25 points)**

- US-001, US-002, US-005, US-006, US-007, US-009, US-010

**Sprint 2: Security & Navigation (23 points)**

- US-004, US-012, US-013, US-020, US-022

**Sprint 3: Validation & UI (23 points)**

- US-014, US-015, US-020 (completion)

**Sprint 4: Documentation & Polish (18 points)**

- US-028, US-029, US-030

## Notes

- Story points use Fibonacci sequence (1, 2, 3, 5, 8, 13)
- Estimates are relative to team capacity
- P0 stories required for MVP
- P1 stories highly desired for initial release
- P2 stories deferred to future releases
