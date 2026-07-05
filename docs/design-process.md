### Design and Development Process

The main decisions I made during the design and development process were the following:

1. Defined a business use case: a basic billiards tournament registration system. This served as a lightweight Product Requirements Document (PRD) that established the functional scope and objectives.
2. Based on the requirements and my experience, I designed the overall solution architecture using **.NET Clean Architecture** for the backend and a basic **Angular SPA (Single Page Application)** for the frontend.
3. Created the architecture boilerplate to establish a solid project foundation and a reusable application skeleton.
4. Designed the data model using **Entity Framework Code First**.
5. Defined and implemented the API contracts, starting from the data layer and progressing through the upper application layers.
6. Identified and designed the required unit and integration tests to validate the application's behavior.
7. Implemented the different backend layers, including repositories, application services, and API controllers.
8. Once the backend reached a stable state, I started building the frontend application structure.
9. Implemented a service wrapper to centralize communication with the backend APIs.
10. Implemented Angular interceptors and route guards to support basic authentication and request handling.
11. Developed the frontend components, including user login, tournament creation, and participant registration.
12. Performed smoke testing, followed by a verification and optimization phase to ensure the solution was stable, maintainable, and ready for use.
