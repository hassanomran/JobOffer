Job Offer Management System

A full-stack application for managing job offers, built with:

    Backend: .NET 8 Web API, Entity Framework Core (Code-First), SQL Server

    Frontend: Angular 12+, Bootstrap

✨ Features

    Job offer CRUD operations

    PDF generation

    DocuSign simulation

    File preview

    Email sending

🚀 Technologies
Backend

    .NET 8 Web API

    Entity Framework Core (Code-First)

    SQL Server

    CORS enabled for Angular

    Logging with Microsoft.Extensions.Logging

Frontend

    Angular 12+

    Bootstrap

    HTML5 for PDF preview (<embed>)

    Angular Reactive Forms

📂 Project Structure

JobOfferManagement/
│
├── backend/                  # .NET 8 Web API
│   ├── Controllers/           # API controllers
│   ├── Models/                # EF Core entities
│   ├── Data/                  # DbContext
│   ├── Services/              # Business logic
│   ├── Migrations/            # EF Core migrations
│   └── Program.cs
│
└── frontend/                  # Angular 12+ project
    ├── src/app/modules/job-offer/   # Job offer module
    ├── src/app/core/                # Shared services, interceptors
    └── src/environments/            # API environment configs

