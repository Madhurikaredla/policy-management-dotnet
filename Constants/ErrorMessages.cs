namespace PolicyManagement.Constants;

public static class ErrorMessages
{
    public const string InvalidEmailOrPassword = "Invalid email or password";
    public const string InvalidPhoneOrPassword = "Invalid country code, phone number, or password";
    public const string UserNotFound = "User not found";
    public const string UserInactive = "User is inactive";
    public const string RegistrationFailed = "User creation failed";
    public const string UserAlreadyExists = "User already exists";

    public const string EmailAlreadyExists = "User with this email already exists";

    public const string PhoneAlreadyExists = "User with this phone number already exists";

    public const string ValidationFailed = "Validation failed";
    public const string Unauthorized = "Unauthorized";
    public const string InternalError = "An internal error occurred";
    public const string Success = "Request processed successfully";
}
