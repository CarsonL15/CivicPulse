# CivicPulse

A full-stack civic event discovery platform that connects people with local government meetings, town halls, and community events. AI generates plain-language summaries so anyone can understand what's being discussed and why it matters.

**Live Demo:** [green-flower-0c58be30f.2.azurestaticapps.net](https://green-flower-0c58be30f.2.azurestaticapps.net)

---

## What It Does

Most civic events are buried across scattered government websites with dense, jargon-heavy descriptions. CivicPulse solves this by putting events in one place and using AI to make them accessible:

- **AI Summaries** — GPT-4o-mini reads event descriptions and generates plain-language summaries with a "Why It Matters" explanation of how the topic impacts everyday people
- **Semantic Search** — Search by meaning, not just keywords. Searching "affordable housing" finds zoning meetings, rent hearings, and development town halls even if those exact words aren't in the listing
- **Image Search** — Upload an event poster and find matching events using Azure AI Vision multimodal embeddings
- **Bookmarks** — Save events you want to attend
- **Role-Based Access** — Citizens browse and save, Organizers create events, Admins moderate

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | Nuxt 3, Vue 3, Vuetify 3, TypeScript |
| Backend | ASP.NET Core 8, Entity Framework Core, C# |
| Database | Azure SQL |
| AI | OpenAI GPT-4o-mini (summaries), Azure OpenAI text-embedding-3-small (embeddings) |
| Search | Azure AI Search (hybrid: vector + full-text + semantic ranking) |
| Auth | ASP.NET Identity + JWT |
| CI/CD | GitHub Actions |
| Hosting | Azure App Service (API), Azure Static Web Apps (frontend) |

## Architecture

```
┌─────────────────────────────────────────────────┐
│                   Frontend                       │
│         Nuxt 3 + Vuetify (SPA mode)             │
│   Pages → Composables → API calls (useApi)       │
└──────────────────────┬──────────────────────────┘
                       │ REST / JWT
┌──────────────────────▼──────────────────────────┐
│                   Backend                        │
│              ASP.NET Core 8 API                  │
│   Controllers → Services → EF Core → Azure SQL   │
│                     │                            │
│         ┌───────────┼───────────┐                │
│         ▼           ▼           ▼                │
│   EmbeddingService  SummaryService  SearchService│
│   (Azure OpenAI)    (GPT-4o-mini)  (AI Search)  │
└──────────────────────────────────────────────────┘
```

**Event creation flow:** Organizer submits event → API generates AI summary via GPT-4o-mini → generates 1024-dim text embedding via text-embedding-3-small → stores event in Azure SQL → indexes document with embedding in Azure AI Search

**Search flow:** User query → generate query embedding → Azure AI Search performs hybrid retrieval (vector similarity + full-text BM25 + semantic reranking) → return ranked results

## Project Structure

```
CivicPulse/
├── backend/
│   ├── CivicPulse.Api/
│   │   ├── Controllers/       Auth, Events, Search, Bookmarks, Admin
│   │   ├── Services/          EventService, SearchService, EmbeddingService, SummaryService
│   │   ├── Models/            AppUser, Event, Bookmark, EventCategory
│   │   ├── Dtos/              Request/response models
│   │   ├── Data/              EF Core DbContext, seed data
│   │   └── Program.cs
│   └── CivicPulse.Api.Tests/  xUnit + Moq unit tests
├── frontend/
│   ├── app/
│   │   ├── pages/             Home, Login, Register, Bookmarks, Event Detail, Create, Admin
│   │   ├── components/        EventCard, SearchBar, CategoryFilter, AiSummary, EventForm
│   │   ├── composables/       useApi, useAuth, useSearch, useEvents, useBookmarks
│   │   ├── middleware/        Auth and admin route guards
│   │   └── tests/             Vitest + Vue Test Utils
│   └── nuxt.config.ts
└── .github/workflows/         CI/CD pipelines for both frontend and backend
```

## Key Features

**For Citizens**
- Browse upcoming civic events with AI-generated summaries
- Semantic search — find events by topic, not exact keywords
- Image search — upload an event poster to find matching events
- Filter by category (Education, Housing, Public Safety, Environment)
- Bookmark events to save for later

**For Organizers**
- Create events with title, description, date, time, location, and category
- AI automatically generates a plain-language summary and "Why It Matters" explanation

**For Admins**
- Dashboard with user and event management
- Approve/unapprove events, manage user roles, moderate content

## Testing

- **Backend:** xUnit with Moq and EF Core InMemory — covers event CRUD, bookmarks, authorization, search index sync
- **Frontend:** Vitest with Vue Test Utils — covers all major components (EventCard, SearchBar, CategoryFilter, EventForm, EventList, AiSummary)

```bash
# Run backend tests
cd backend && dotnet test

# Run frontend tests
cd frontend && npx vitest run
```

## Deployment

Both pipelines run on push to `main` via GitHub Actions:

- **Backend** — Build → Test → Publish to Azure App Service
- **Frontend** — `nuxt generate` → Deploy to Azure Static Web Apps

---

Built by Carson Layden — EWU CSCD379, Winter 2026
