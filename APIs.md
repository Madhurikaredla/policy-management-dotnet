# API Documentation

## AuthController
- **POST /api/auth/register**: Register a new user
- **POST /api/auth/login**: Login and receive JWT token

## UserController
- **GET /api/users**: Get all users
- **GET /api/users/{id}**: Get user by ID
- **GET /api/users/by-email?email=**: Get user by email
- **PUT /api/users/{id}**: Update user
- **DELETE /api/users/{id}**: Mark user as inactive

## PolicyController
- **GET /api/policies**: Get all active policies
- **GET /api/policies/{id}**: Get policy by ID
- **POST /api/policies**: Create a new policy (Admin)
- **PUT /api/policies/{id}**: Update a policy (Admin)
- **DELETE /api/policies/{id}**: Delete a policy (Admin)
- **GET /api/policies/search?minAmount=&maxAmount=**: Search policies by premium amount
- **GET /api/policies/status?isActive=**: Get policies by status

## EnrollmentController
- **POST /api/enrollments/policy/{policyId}/enroll**: User requests to enroll in a policy
- **GET /api/enrollments/user/{userId}**: User views their own enrollments
- **GET /api/enrollments/admin**: Admin views all enrollments or filter by status
- **POST /api/enrollments/admin/{id}/approve**: Admin approves an enrollment
- **POST /api/enrollments/admin/{id}/reject**: Admin rejects an enrollment

## Common
- All endpoints return a consistent response format: `{ message, data, error }`
- Use JWT Bearer token for protected endpoints (see Swagger UI for details)

---
For request/response models and more details, see the DTOs and Entities folders.
