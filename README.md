# Project - Book Sharing Platform

A full-stack project, designed to apply modern .NET development practices, cloud services, and frontend integration. The project is built with Clean Architecture, SOLID and Clean code principles, secure authentication, and cloud storage integration.

---

##  Tech Stack

###  Backend (.NET / C#)
- ASP.NET Core Web API (.NET 8)
- Clean Architecture (Domain, Application, Infrastructure, API)
- Entity Framework Core + SQL Server
- AutoMapper/Mapperly
- FluentValidation
- xUnit + Moq for testing
- JWT Authentication with refresh tokens
- File Storage: AWS S3 or Azure Storage

###  Frontend
- HTML, CSS, JavaScript (Typescript preferably)
- Bootstrap/Tailwind for styling
- Token persistence with `localStorage`
- Optionally Angular if time/skill allows

---

##  Functional Requirements

###  User Management
- Admin page: Admin can see the list of all users, admin can see the list of all books, admin can block/delete users, admin can add/edit/delete books
- Register, Login, Logout
- JWT-based authentication
- Role-based authorization: `User`, `Admin`
- Only authenticated users can create/edit books
- Only Admins can delete books by other users

###  Book Management
- Create Book:
  - Title (required)
  - Author (required)
  - Description (optional)
  - Tags (comma-separated or selectable)
  - Cover Image (file upload)
- View all books
- View book details
- Edit & delete own books
- Admins can delete any book

###  Search, Filter, Pagination
- Search by title or author
- Filter by tags or uploader
- Sort by newest / alphabetical
- Pagination on book listings

### Book Cover Upload
- Upload image to Azure Blob or S3
- Store URL in database
- Display cover image on UI

###  API & Validation
- RESTful API
- FluentValidation for input validation
- Proper HTTP status codes

---

## Non-Functional Requirements

- All endpoints protected and role-checked
- Exception handling middleware
- Async/await best practices
- Proper solution architecture with Clean Architecture layers
- Use SOLID and Clean code principles
- Secure credentials via environment/config
- Use i18n internationalization (https://learn.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-9.0)
- Deployment-ready app

---

## Tasks

---

###  Week 1 – Setup & Clean Architecture
- Create GitHub repo
- Setup solution with:
  - Domain
  - Application
  - Infrastructure
  - API
- Configure DI, AutoMapper/Mapperly, FluentValidation
- Define initial domain entities: User, Book, Tag, etc.

##### Useful links:
- https://docs.github.com/en/get-started/start-your-journey/hello-world
- https://blog.codacy.com/what-is-clean-code
- https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures

###  Week 2 – Authentication + EF Core
- Add EF Core + Migrations
- Implement login and registration endpoints
- Store hashed passwords and users in DB
- Frontend: login/register form
- Unit test for AuthValidator

##### Useful links:
- https://learn.microsoft.com/en-us/ef/core/
- https://www.entityframeworktutorial.net/efcore/entity-framework-core.aspx
- https://learn.microsoft.com/en-us/aspnet/core/security/authentication/configure-jwt-bearer-authentication?view=aspnetcore-9.0
- https://medium.com/@solomongetachew112/jwt-authentication-in-net-8-a-complete-guide-for-secure-and-scalable-applications-6281e5e8667c
- https://medium.com/@michaszala/1-authentication-guide-net-6-0-simple-login-and-register-user-api-5c3c275311a6

###  Week 3 – Book CRUD API
- Add CRUD for books:
  - `GET /books`, `GET /books/{id}`
  - `POST`, `PUT`, `DELETE`
- Protect routes with JWT
- Add validation, DTOs, Mapping
- Add filtering: `author`, `tag`, `search`
- Pagination support

###  Week 4 – File Upload + Cloud Integration
- Implement `IFileStorageService`
  - Azure Blob or S3 backend
- Store image Id in database
- Validate image format and size
- Frontend form + preview for cover image

##### Useful links:
- https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/csharp_s3_code_examples.html
- https://useme-alehosaini.medium.com/s3-management-using-aws-sdk-for-net-c-8ac014217315

###  Week 5 – Role-Based Access Control
- Add `User` / `Admin` roles
- Admins can delete any book
- Only book owners can edit/delete
- Refresh token logic (optional)
- Frontend: show role-specific UI
- Store token in localStorage

##### Useful links:
- https://learn.microsoft.com/en-us/aspnet/core/security/authorization/roles?view=aspnetcore-9.0
- https://medium.com/@solomongetachew112/implementing-role-based-authorization-in-asp-net-c259f919fb5d


###  Week 6 – UI Polish + Search, Sort, Filter
- Add search bar, filters (tags, uploader)
- Add sort dropdown: alphabetical, newest
- Book details page with full info


###  Week 7 – Testing & Polish
- Add unit tests
- Improve error messages and response codes
- Cleanup architecture/code

### Optional tasks
- Deploy backend (Azure. Render, Scaleway, Heroku, etc.)
- Deploy frontend (Vercel / GitHub Pages)
- Implement commenting, new comments should be displayed in real-time
- Add dark theme
- Add 2 language to the app



