# VgcCollege.Web

This repo contains a Razor Pages + Identity + EF Core sample for Vgc College.

Setup
1. Configure `DefaultConnection` in `VgcCollege/appsettings.json` to point to a SQL Server or SQLite database.
2. From the `VgcCollege` project directory run:
   - `dotnet ef database update` (ensure tools installed)
3. Run the app: `dotnet run --project VgcCollege` or via Visual Studio.

Seeded demo accounts
- Admin: `admin@vgc.local` / `Admin123!` (Administrator)
- Faculty: `faculty@vgc.local` / `Faculty123!` (Faculty)
- Student: `student@vgc.local` / `Student123!` (Student)
- Student2: `student2@vgc.local` / `Student123!` (Student)

Tests
- Tests are in `tests/VgcCollege.Tests`. Run `dotnet test` from repo root.


