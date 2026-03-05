# CivicPulse - Local Civic Event Discovery Platform

## Elevator Pitch

CivicPulse is a platform that connects young people with local civic events happening in their community. Event organizers post their events in one central place, and AI generates plain-language summaries so anyone can understand what's being discussed and why it matters. Users can search semantically for topics they care about, making it easy to find relevant city council meetings, school board hearings, town halls, and community events without navigating confusing government websites.

## Target Audience

- Young people who are civically curious but don't know where to start
- Event organizers (advocacy groups, neighborhood associations, local political organizations, community groups) who want to reach more attendees

## Use Cases

- As a citizen, I can search "affordable housing" and find upcoming zoning meetings, housing hearings, and town halls related to rent and development
- As a citizen, I can browse events by category (education, housing, public safety, environment) and save ones I want to attend
- As a citizen, I can read a plain-language AI summary of an event and understand what's being discussed and why it matters
- As an organizer, I can create an account and post events with a title, description, date, time, location, and category
- As an admin, I can moderate posted events and manage user accounts

## Tech Stack

**Frontend**
- Nuxt 4
- Vuetify
- TypeScript

**Backend**
- ASP.NET Core 8
- Controllers, Services, DTOs
- Entity Framework Core

**Database**
- Azure SQL

**AI & Search**
- LLM API (undecided on model) plain-language summary generation + text embeddings
- Vector search using embeddings for semantic event discovery

**Cloud & Deployment**
- Azure Static Web Apps (frontend)
- Azure App Service (backend)
- GitHub Actions CI/CD

## Technical Requirements

- User authentication with role-based access (Citizen, Organizer, Admin)
- Event CRUD operations for organizers
- AI-powered plain-language summary generation from raw event descriptions, including a "why this matters" explanation of how the topic impacts everyday people
- Vector search using text embeddings so users can find relevant events by meaning, not just keywords
- Category-based filtering and browsing
- Saved/bookmarked events for authenticated users
- Admin dashboard for user and event moderation
- Clean backend architecture using Controllers, Services, DTOs, and dependency injection
- Frontend built using Vue components and Vuetify
- Unit tests covering core services on frontend and backend
- Responsive design for mobile and desktop
- Full deployment to Azure with a working CI/CD pipeline
