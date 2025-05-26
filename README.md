# Learning Management Dashboard

A mini Learning Management System (LMS) dashboard showcasing end‑to‑end full‑stack development using .NET Core, in‑memory data storage, RESTful APIs, and a vanilla JavaScript frontend with a focus on clean architecture and separation of concerns.

---

<img width="1000"  src="https://github.com/user-attachments/assets/e5dc9d56-cda6-45e8-a418-2e0e94628baa">


## 🛠️ Setup Instructions

1. **Clone the repository**

   ```bash
   git clone https://github.com/yiratpeleg/LearningManagementDashboard.git
   cd LearningManagementDashboard
   ```

2. **Bring up LocalStack (S3 mock)**

   We use LocalStack to simulate AWS S3 locally.

   ```bash
   docker compose up -d localstack
   ```

   This exposes S3 on `http://localhost:4566`.

3. **Backend**

   * Navigate to the backend project:

     ```bash
     cd backend/src/LearningManagementDashboard
     ```
   * Restore dependencies and run:

     ```bash
     dotnet restore
     dotnet run
     ```
   * The API will be available at `https://localhost:5001/api`
   * On startup the S3 bucket `learning-dashboard` is created in LocalStack.

4. **Frontend**

   * From the `frontend` folder, install no dependencies (vanilla JS):

     ```bash
     cd frontend/src
     ```
   * Start a simple HTTP server (e.g., using VS Code Live Server or `npx serve`):

     ```bash
     npx serve . -l 5500
     ```
   * Open `http://localhost:5500/index.html` in your browser.

5. **Environment Configuration**

   * Update `launchSettings.json` (backend) to match desired ports.
   * Ensure CORS (if any) is configured to allow the frontend origin.

---

## 📦 AWS / LocalStack Configuration

We leverage the AWS SDK for .NET against LocalStack’s S3 endpoint:

* **Docker Compose**
   * Our `docker-compose.yml` includes:

  ```yaml
  services:
    localstack:
      image: localstack/localstack:latest
      ports:
        - '4566:4566'
      environment:
        - SERVICES=s3
        - DEFAULT_REGION=us-east-1
  ```

* **appsettings.json**
   * Point the SDK at LocalStack:

  ```jsonc
  {
    "AWS": {
      "ServiceURL": "http://localhost:4566",
      "UseHttp": true
    }
  }
  ```

* **Bucket auto-creation**
   * On startup, the bucket `learning-dashboard` is created in LocalStack if missing.

* **Verifying via AWS CLI**
   * Run ```aws configure``` with the following params:
   * `AWS_ACCESS_KEY_ID=mock AWS_SECRET_ACCESS_KEY=mock`

  ```bash
  aws --endpoint-url http://localhost:4566 s3 ls s3://learning-dashboard/courses/
  aws --endpoint-url http://localhost:4566 s3 cp s3://learning-dashboard/courses/<id>.json -
  ```

---

## 🏛️ Architecture & Design Decisions

### Backend (.NET Core)

* **Clean layered structure**: Controllers → Services → Models → Exceptions → Mapping Profiles
* **Dependency Injection**: `ICourseService`, `IStudentService`, `IEnrolmentService` registered as singletons in `Program.cs` to ensure a single shared in-memory data store for the app’s lifetime and avoid recreating service instances on each request.
* **In‑Memory Storage**: `Dictionary<Guid, T>` to simulate persistence with unique constraints and quick lookups.
* **AutoMapper**: Centralized mapping profiles to separate internal domain entities from external request/response models.
* **Robust logging & error handling**: Structured `ILogger<T>` usage, global exception handler, `ApiError` patterns.
* **Async/Await**: All service operations return `Task<T>` for future scalability to real databases.

### Frontend (HTML5, CSS3, Vanilla JS)

* **Modular O-O JavaScript**: `App` orchestrator + Controllers + Services + Templates + DOM utils.
* **Separation of Concerns**:

  * **ApiClient**: HTTP verbs abstraction (`get`, `post`, `put`, `delete`).
  * **CourseService / StudentService / EnrolmentService**: Business API wrappers.
  * **Controllers**: UI logic (binding, rendering) per feature, extending `BaseController`.
  * **Templates**: Pure functions for HTML generation, enabling easy unit testing.
  * **DOM Utilities**: `fetchText`, `showSection`, `escapeHtml` to simplify view rendering.
* **Lazy Loading & Caching**:

  * Sections loaded on demand with `loaded` flags.
  * Event delegation for table actions to minimize listeners.

---

## 💡 Trade-offs & Areas for Improvement

* **In‑Memory Data**: Simplifies setup but not persistent. Future work: integrate EF Core with DB.
* **Validation**: Frontend relies on browser HTML5 + minimal error alerts. Improvement: build a reactive form library or integrate a UI framework for richer UX.
* **Global State**: Managed via simple flags and `App` class. Scaling may benefit from state management patterns (Redux‑like, Observables).
* **Routing**: URL hash preserves tabs. For a larger app, consider a client‑side router (e.g., Navigo) or SPA framework.
* **Testing**: Backend has unit tests for part of the services but lacks integration tests - consider adding xUnit or NUnit integration tests. Frontend has no unit tests and automated tests - add Jest unit tests and end-to-end tests for full coverage.

---

*Developed with attention to clean code principles, SOLID design, and pragmatic trade‑offs for a concise yet extensible mini‑LMS dashboard.*
