# Concert Ticket Management System

A .NET 9 Web API for managing concert events and tickets, with full test coverage and readiness for containerized deployment.

---

## Table of Contents

1. [Features](#features)  
2. [Project Structure](#project-structure)  
3. [Prerequisites](#prerequisites)  
4. [Getting Started](#getting-started)  
   - [Clone & Restore](#clone--restore)  
   - [Database Setup](#database-setup)  
   - [Run the API](#run-the-api)  
5. [API Usage](#api-usage)  
   - [Swagger UI](#swagger-ui)  
   - [Authentication](#authentication)  
6. [Testing](#testing)  
   - [Unit Tests](#unit-tests)  
   - [Integration Tests](#integration-tests)  
7. [Containerization & Deployment](#containerization--deployment)  
8. [Next Steps](#next-steps)  
9. [License](#license)  

---

## Features

- **Event Management**  
  - Create, update, list, retrieve events  
  - Each event has date, venue, description, capacity  

- **Ticket Reservations & Sales**  
  - Define ticket types with pricing and quantity  
  - Reserve tickets, complete purchase, cancel reservations  
  - View real-time availability  

- **Security & Validation**  
  - JWT-based authentication & role-based authorization  
  - Request validation via FluentValidation  
  - Centralized error handling returning RFC-compliant ProblemDetails  

- **Clean Architecture**  
  - Domain, Infrastructure, API, Service layer separation  
  - DTOs with AutoMapper mappings  
  - Thin controllers delegating business logic to services  

- **Testing**  
  - Unit tests for services using EF Core InMemory  
  - Integration tests against an in-memory host and real JWT flow  

---

## Project Structure
