# Project Scope

## Project Name

**cpike.Setup.Middleware** - First-Time Setup Wizard for .NET Blazor Applications

## Project Vision

Create a lightweight, extensible middleware library that provides a professional wizard-based first-time setup experience for .NET Blazor applications, enabling developers to easily implement administrative configuration workflows before their applications go live.

## Target Audience

### Primary

- Personal Blazor projects (self-hosted applications)
- Solo developers and small teams
- Internal tooling developers

### Secondary

- Open-source community (contributors and users)
- Enterprise developers building admin-first applications
- Multi-tenant application developers

## Goals

### Primary Goals (MVP)

1. **Request Interception**: Intercept all HTTP requests and redirect to setup wizard until completion
2. **Multi-Step Wizard Framework**: Provide extensible step-based wizard infrastructure
3. **Builder Pattern API**: Fluent, developer-friendly API for registering setup steps
4. **File-Based State**: Simple file marker system to track setup completion
5. **Core UI Components**: Essential Blazor components for building setup forms
6. **Example Implementation**: Working demo app showing typical setup scenarios

### Secondary Goals (Future)

1. **Database-Backed State**: Option to store setup state in database
2. **Advanced Components**: Additional UI components (file uploads, rich editors, etc.)
3. **Validation Framework**: Built-in validation with custom rule support
4. **Multi-Tenant Support**: Per-tenant setup tracking
5. **Setup Re-run**: Admin ability to re-trigger setup wizard
6. **Migration Wizards**: Support for upgrade/migration workflows between versions

## In Scope (MVP)

### Core Middleware

- [x] Basic middleware structure
- [ ] Request interception logic
- [ ] Setup completion detection (file-based)
- [ ] Redirect to setup wizard route
- [ ] Completion marker creation
- [ ] Configuration options (setup path, marker file location)

### Wizard Framework

- [ ] Step registration system (builder pattern)
- [ ] Step execution pipeline
- [ ] Step validation framework
- [ ] Step ordering/sequencing
- [ ] Progress tracking
- [ ] State management between steps
- [ ] Navigation (next, previous)
- [ ] Step completion status

### Service Collection Extensions

- [ ] `AddSetupWizard()` extension method
- [ ] Step registration API
- [ ] Configuration binding
- [ ] Dependency injection integration

### UI Components

- [ ] Wizard container component
- [ ] Progress indicator component
- [ ] Step base component
- [ ] Form input component
- [ ] Checkbox component
- [ ] Toggle switch component
- [ ] Button components
- [ ] Alert/notification component
- [ ] Summary/review component
- [ ] Success completion component

### Styling & Design

- [ ] Base CSS framework (extracted from prototype)
- [ ] Responsive design (mobile-friendly)
- [ ] Consistent color scheme and branding
- [ ] Animation/transitions
- [ ] Accessibility considerations (ARIA labels, keyboard navigation)

### Documentation

- [x] README.md with quick start
- [x] Project scope document
- [ ] Architecture overview
- [ ] User stories
- [ ] Product requirements document (PRD)
- [ ] Design guide
- [ ] API documentation
- [ ] Code examples
- [ ] Integration guide

### Testing

- [ ] Example Blazor application (Tester project)
- [ ] Basic unit tests for core logic
- [ ] Integration test for middleware

## Out of Scope (MVP)

### Features Explicitly Excluded

1. **Database State Management**: Only file-based state in MVP
2. **User Authentication**: Middleware assumes authentication is handled separately
3. **Authorization/Permissions**: No built-in role checking for setup access
4. **Multi-Language Support**: English only for MVP
5. **Advanced Validation**: Only basic validation; no complex rule engine
6. **Step Skipping**: All steps must be completed in sequence
7. **Draft/Save Progress**: No partial completion - wizard must finish in one session
8. **Template System**: No pre-built setup templates
9. **Import/Export**: No configuration import/export
10. **Setup History/Audit Log**: No tracking of who ran setup or when
11. **Email Notifications**: No built-in email functionality
12. **External Service Integration**: No third-party service connectors
13. **Backup/Restore**: No configuration backup functionality
14. **Version Migration**: No automatic upgrade wizards

### Technical Exclusions

1. **Multi-Framework Support**: Blazor Server only (not WASM or MAUI initially)
2. **Custom Routing**: Uses standard Blazor routing
3. **State Persistence Beyond File**: No Redis, session, or cookie-based state
4. **Advanced Security**: No encryption of setup data at rest
5. **Performance Optimization**: No caching or advanced performance features
6. **Logging Framework**: Uses standard .NET logging
7. **Telemetry/Analytics**: No built-in usage tracking

## Success Criteria

The MVP will be considered successful when:

1. **Integration Time < 30 minutes**: A developer can integrate the middleware into an existing Blazor app in under 30 minutes
2. **Custom Step Creation < 15 minutes**: Creating a new custom setup step takes less than 15 minutes
3. **Zero Breaking Changes**: Can be added to existing projects without breaking existing functionality
4. **Professional Appearance**: UI matches or exceeds the quality of the HTML prototype
5. **Working Example**: The Tester application demonstrates all core features
6. **Documentation Complete**: All MVP documentation is written and accurate

## Constraints

### Technical Constraints

- **Framework**: .NET 8.0+
- **Platform**: Blazor Server (initially)
- **State Storage**: File system access required
- **Browser Support**: Modern browsers (Chrome, Firefox, Edge, Safari - last 2 versions)

### Project Constraints

- **Timeline**: MVP development (no hard deadline, but target is reasonable timeframe)
- **Resources**: Solo developer (primary author)
- **Budget**: Open source, no budget constraints
- **Dependencies**: Minimize external package dependencies

### Design Constraints

- **Simplicity First**: Favor simple, understandable code over clever abstractions
- **Convention Over Configuration**: Sensible defaults, minimal required configuration
- **Extensibility**: Design for extension without modifying core library
- **Backwards Compatibility**: Once released, maintain API stability

## Key Scenarios

### Scenario 1: New Application Setup

A developer creates a new Blazor application and wants to add a first-time setup wizard for:

- Creating the initial admin account
- Configuring database connection
- Setting up basic application settings

### Scenario 2: Third-Party Feature Integration

A developer installs a third-party library (e.g., blog engine, user management) that includes its own setup steps. These steps should integrate seamlessly into the existing setup wizard.

### Scenario 3: Multi-Step Configuration

A developer needs to collect configuration across multiple related steps:

- Admin account creation
- System role configuration
- Invite system setup
- Review and confirmation
- Success with generated credentials

### Scenario 4: Production Deployment

An admin deploys the application to production for the first time. On first access:

- All traffic is redirected to setup wizard
- Setup wizard completes all configuration
- Setup completion is persisted
- Subsequent requests bypass the wizard
- Application is now accessible to end users

## Non-Functional Requirements

### Performance

- Middleware overhead: < 1ms when setup is complete
- Wizard page load: < 500ms on modern hardware
- File I/O for state checking: Minimal impact on request pipeline

### Security

- No sensitive data stored in plaintext (delegate to implementing application)
- Protected against common web vulnerabilities (XSS, CSRF via Blazor defaults)
- Setup completion marker not easily bypassed

### Usability

- Intuitive wizard flow requiring no documentation
- Clear error messages and validation feedback
- Responsive design works on mobile and desktop
- Accessible to screen readers and keyboard navigation

### Maintainability

- Clean, well-documented code
- Separation of concerns (middleware, UI, state management)
- Easy to extend without modifying core
- Comprehensive inline code documentation

### Reliability

- Graceful degradation if file system access fails
- Clear error messages for misconfiguration
- No silent failures
- Idempotent setup execution

## Future Considerations

While not in the MVP scope, these features are candidates for future releases:

1. **Database Provider System**: Pluggable state providers (file, SQL, NoSQL)
2. **Step Templates**: Pre-built common steps (admin creation, SMTP config, etc.)
3. **Conditional Steps**: Steps that appear based on previous choices
4. **Parallel Steps**: Steps that can be completed in any order
5. **Setup Analytics**: Track which steps take longest, where users struggle
6. **Localization**: Multi-language support
7. **Theming System**: Easy customization of colors and styles
8. **Step Dependency Graph**: Complex dependencies between steps
9. **API-Driven Setup**: Headless setup via REST API
10. **Setup Plugins**: NuGet packages that auto-register setup steps

## Dependencies

### Framework Dependencies

- `Microsoft.AspNetCore.Components` - Core Blazor functionality
- `Microsoft.Extensions.DependencyInjection` - DI support
- `Microsoft.Extensions.Configuration` - Configuration binding
- `Microsoft.Extensions.Logging` - Logging support

### Development Dependencies

- `xUnit` or `NUnit` - Unit testing
- `Moq` - Mocking framework for tests

### No External UI Dependencies

The library will not depend on Bootstrap, Tailwind, or other CSS frameworks to keep it lightweight and avoid conflicts.

## Risks and Mitigations

| Risk | Impact | Likelihood | Mitigation |
|------|--------|------------|------------|
| File permission issues in production | High | Medium | Clear documentation on required permissions; graceful error handling |
| Conflicts with existing routing | Medium | Low | Use unique route prefix; document integration order |
| State corruption during setup | High | Low | Atomic file operations; validation before marking complete |
| Third-party step registration conflicts | Medium | Low | Step ordering system; clear documentation on step lifecycle |
| Browser compatibility issues | Medium | Low | Target modern browsers only; document requirements |
| Performance impact on request pipeline | Low | Low | Minimal logic in middleware; early exit when setup complete |

## Open Questions

1. **Configuration File Format**: JSON, YAML, or code-based configuration?
   - *Current thinking: Code-based for type safety, with optional JSON binding*

2. **Step State Sharing**: How do steps share data during wizard execution?
   - *Current thinking: Scoped service for wizard state*

3. **Validation Timing**: Client-side, server-side, or both?
   - *Current thinking: Server-side required, client-side optional for UX*

4. **Error Recovery**: What happens if a step fails midway?
   - *Current thinking: Show error, allow retry or previous step navigation*

5. **Setup Re-triggering**: Should there be a way to re-run setup?
   - *Current thinking: Not in MVP; delete marker file manually if needed*
