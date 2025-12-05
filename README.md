# WeTrust API - Minimal skeleton

## Quick start (local)

1. Install .NET 8 SDK: https://dotnet.microsoft.com/download
2. From repo root:
   cd WeTrust.Api
   dotnet restore
   dotnet build
   dotnet run
3. Set environment variable DATABASE_URL to your Supabase connection.
4. Set JWT_SECRET env var.

## Push to GitHub
git init
git add .
git commit -m "Initial commit"
# create repo on GitHub and push
git remote add origin <repo-url>
git push -u origin main

## Deploy to Render
- Connect GitHub repo on Render.
- Create a Web Service, point to repo.
- Set env vars: DATABASE_URL, JWT_SECRET
- Use Dockerfile (Render will build it).

