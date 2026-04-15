---
name: workspace-instructions
description: Workspace-level guidance for the Soc Ops Blazor app, including build/test commands, architecture conventions, and repo documentation links.
---

Before submitting changes, complete this checklist:
- `cd SocOps && dotnet build`
- `cd SocOps && dotnet run` and verify the app loads
- Confirm linting and formatting for new/changed files

Quick facts:
- Blazor WebAssembly app in `SocOps/` targeting .NET 10
- Main project: `SocOps/SocOps.csproj`
- Entry point: `SocOps/Program.cs`
- Pages: `SocOps/Pages/`
- Components: `SocOps/Components/`
- Shared data/models: `SocOps/Data/`, `SocOps/Models/`
- Services: `SocOps/Services/`
- Docs/site content: `docs/`, `workshop/`

Build and run:
- `cd SocOps && dotnet build`
- `cd SocOps && dotnet run`

Deployment notes:
- GitHub Actions deploys from `.github/workflows/deploy.yml`
- Output is published to `./_site/app/`
- `docs/` is copied to site root and `index.html` base href is fixed for `/agent-lab-dotnet/app/`

Design and conventions:
- Preserve the existing Blazor structure and static Pages deployment model
- Use `.github/instructions/frontend-design.instructions.md` and `.github/instructions/css-utilities.instructions.md` for UI guidance
- Keep component logic in `SocOps/Components/` and page routing in `SocOps/Pages/`

Design guide:
- Favor immersive themes and distinctive visual character over generic UI layouts
- Prefer CSS-driven motion and atmosphere instead of adding external image assets when possible
- Keep color, typography, and spacing consistent using the project’s utility classes and the shared `app.css`
- When redesigning pages, ensure the interface remains accessible and the core Blazor navigation flow stays intact
