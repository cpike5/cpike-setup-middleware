# Documentation Index

Welcome to the cpike.Setup.Middleware documentation! This index will help you find the right documentation for your needs.

## Getting Started

New to the project? Start here:

1. **[README](../README.md)** - Project overview, quick start guide, and basic usage
2. **[UI Prototype](reference/first-time-setup-wizard.html)** - Visual reference showing what the wizard looks like
3. **[Creating Custom Steps](CREATING-CUSTOM-STEPS.md)** - Tutorial on building your first setup step

## For Developers

### Integration & Usage

- **[Creating Custom Steps](CREATING-CUSTOM-STEPS.md)**
  - How to implement custom setup steps
  - Validation and execution
  - State management between steps
  - Complete examples

- **[Step Design & Styling Guide](STEP-DESIGN-GUIDE.md)**
  - Best practices for creating well-designed steps
  - Using built-in components (SetupInput, SetupToggle, etc.)
  - Layout patterns and validation
  - Complete examples with modern styling

- **[CSS Customization](CSS-CUSTOMIZATION.md)**
  - How to customize the wizard's appearance
  - CSS custom properties reference
  - Theming examples
  - Dark mode support

- **[Setup Password Protection](SETUP-PASSWORD.md)**
  - Configuring password protection
  - Security best practices
  - Troubleshooting password issues

### Technical Reference

- **[Architecture Overview](ARCHITECTURE.md)**
  - System architecture and components
  - Data flow and lifecycle
  - Service registration and DI
  - File structure

- **[Design Guide](DESIGN-GUIDE.md)**
  - Complete design system
  - Color palette and typography
  - Component specifications
  - Animation and transitions

## For Project Managers

### Planning Documents

- **[Project Scope](PROJECT-SCOPE.md)**
  - Project vision and goals
  - What's in scope and out of scope
  - Success criteria
  - Risks and mitigations

- **[Product Requirements (PRD)](PRD.md)**
  - Detailed functional requirements
  - Non-functional requirements
  - Success metrics
  - Release criteria

- **[User Stories](USER-STORIES.md)**
  - User personas
  - Detailed user stories with acceptance criteria
  - Story points and priorities
  - Sprint planning suggestions

## Quick Reference

### Common Tasks

| Task | Documentation |
|------|--------------|
| Install and integrate the middleware | [README - Quick Start](../README.md#quick-start) |
| Create a new setup step | [Creating Custom Steps](CREATING-CUSTOM-STEPS.md#quick-start) |
| Design a well-styled step | [Step Design & Styling Guide](STEP-DESIGN-GUIDE.md) |
| Change wizard colors | [CSS Customization - CSS Variables](CSS-CUSTOMIZATION.md#css-custom-properties) |
| Configure password protection | [Setup Password Protection](SETUP-PASSWORD.md#configuration) |
| Understand the architecture | [Architecture Overview](ARCHITECTURE.md#high-level-architecture) |
| See visual examples | [UI Prototype](reference/first-time-setup-wizard.html) |

### API Reference

| Component | Documentation |
|-----------|--------------|
| `ISetupStep` interface | [Creating Custom Steps - Step Interface](CREATING-CUSTOM-STEPS.md#step-interface) |
| `SetupStepBase` class | [Creating Custom Steps - Using SetupStepBase](CREATING-CUSTOM-STEPS.md#using-setupstepbase) |
| `ISetupStateManager` | [Creating Custom Steps - Sharing Data](CREATING-CUSTOM-STEPS.md#sharing-data-between-steps) |
| `SetupOptions` | [Setup Password - API Reference](SETUP-PASSWORD.md#api-reference) |
| CSS Classes | [Design Guide - Components](DESIGN-GUIDE.md#components) |
| CSS Variables | [CSS Customization - Complete List](CSS-CUSTOMIZATION.md#complete-list-of-variables) |

## By Role

### Solo Developer

**Goal:** Get setup wizard working quickly

1. [README - Quick Start](../README.md#quick-start)
2. [Creating Custom Steps - Quick Start](CREATING-CUSTOM-STEPS.md#quick-start)
3. [UI Prototype](reference/first-time-setup-wizard.html)

### Team Lead

**Goal:** Understand project scope and architecture

1. [Project Scope](PROJECT-SCOPE.md)
2. [Architecture Overview](ARCHITECTURE.md)
3. [Product Requirements](PRD.md)

### Designer

**Goal:** Customize appearance and branding

1. [Design Guide](DESIGN-GUIDE.md)
2. [CSS Customization](CSS-CUSTOMIZATION.md)
3. [UI Prototype](reference/first-time-setup-wizard.html)

### DevOps Engineer

**Goal:** Deploy securely in production

1. [Setup Password Protection](SETUP-PASSWORD.md)
2. [Setup Password - Production Deployments](SETUP-PASSWORD.md#production-deployments)
3. [Project Scope - Non-Functional Requirements](PROJECT-SCOPE.md#non-functional-requirements)

## Documentation by Type

### Tutorials

Step-by-step guides:

- [Creating Custom Steps](CREATING-CUSTOM-STEPS.md)
- [CSS Customization](CSS-CUSTOMIZATION.md)
- [README - Quick Start](../README.md#quick-start)

### How-To Guides

Task-oriented guides:

- [Setup Password Protection - Configuration](SETUP-PASSWORD.md#configuration)
- [CSS Customization - Overriding Components](CSS-CUSTOMIZATION.md#overriding-specific-components)
- [Creating Custom Steps - Validation](CREATING-CUSTOM-STEPS.md#validation)

### Reference

Technical specifications:

- [Architecture Overview](ARCHITECTURE.md)
- [Design Guide](DESIGN-GUIDE.md)
- [Product Requirements](PRD.md)
- [Setup Password - API Reference](SETUP-PASSWORD.md#api-reference)

### Explanation

Conceptual documentation:

- [Project Scope](PROJECT-SCOPE.md)
- [User Stories](USER-STORIES.md)
- [Architecture - Design Decisions](ARCHITECTURE.md)

## Examples

### Code Examples

- [Creating Custom Steps - Complete Example](CREATING-CUSTOM-STEPS.md#complete-example-admin-account-step)
- [README - Basic Usage](../README.md#basic-usage)
- [README - Creating Custom Steps](../README.md#creating-custom-steps)

### Visual Examples

- [UI Prototype](reference/first-time-setup-wizard.html)
- [Design Guide - Components](DESIGN-GUIDE.md#components)

### Configuration Examples

- [Setup Password - Use Cases](SETUP-PASSWORD.md#use-cases)
- [CSS Customization - Examples](CSS-CUSTOMIZATION.md#examples)

## Contributing to Documentation

Found an error or want to improve the docs?

1. Documentation source: `/docs` directory
2. Markdown format with GitHub Flavored Markdown
3. Follow existing structure and style
4. Test all code examples
5. Include screenshots for UI changes

## Documentation Standards

Our documentation follows these principles:

- **Clarity**: Write for developers of all skill levels
- **Completeness**: Cover all features and edge cases
- **Currency**: Keep docs up-to-date with code changes
- **Examples**: Include working code examples
- **Navigation**: Link related topics

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2025-10-23 | Initial documentation suite |

## Need Help?

- **Can't find what you need?** Check the [README](../README.md)
- **Found a bug?** [Report it on GitHub](https://github.com/cpike/cpike-setup-middleware/issues)
- **Have a question?** [Start a discussion](https://github.com/cpike/cpike-setup-middleware/discussions)

## Documentation Checklist

Before your first setup:

- [ ] Read [README - Quick Start](../README.md#quick-start)
- [ ] Review [UI Prototype](reference/first-time-setup-wizard.html)
- [ ] Understand [Creating Custom Steps](CREATING-CUSTOM-STEPS.md)
- [ ] Configure [Setup Password](SETUP-PASSWORD.md) if needed

Before customizing:

- [ ] Review [Design Guide](DESIGN-GUIDE.md)
- [ ] Check [CSS Customization](CSS-CUSTOMIZATION.md)
- [ ] Browse [CSS Variables](CSS-CUSTOMIZATION.md#css-custom-properties)

Before production deployment:

- [ ] Review [Setup Password - Security](SETUP-PASSWORD.md#security-considerations)
- [ ] Check [Project Scope - NFRs](PROJECT-SCOPE.md#non-functional-requirements)
- [ ] Verify [Architecture - Security](ARCHITECTURE.md#security-considerations)

## Glossary

- **Setup Step**: Individual screen in the wizard that collects configuration data
- **Setup Wizard**: Multi-step UI for configuring the application on first run
- **State Manager**: Service for sharing data between setup steps
- **Completion Marker**: File that indicates setup has been completed
- **Setup Password**: Generated password protecting access to the setup wizard

## Related Resources

- [.NET 8 Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [Blazor Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
- [ASP.NET Core Middleware](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/)

---

**Last Updated:** 2025-10-23
**Documentation Version:** 1.0
