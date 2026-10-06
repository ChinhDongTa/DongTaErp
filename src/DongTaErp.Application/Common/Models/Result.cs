namespace DongTaErp.Application.Common.Models;

public record ErrorList(IEnumerable<string> ErrorMessages, string? CorrelationId = null);

public class Result
{
    public bool Succeeded => Status is ResultStatus.Ok or ResultStatus.NoContent or ResultStatus.Created;
    public ResultStatus Status { get; protected set; } = ResultStatus.Ok;
    public IEnumerable<string>? Errors { get; protected set; }
    public string? SuccessMessage { get; protected set; }
    public string? Location { get; protected set; }

    public Result()
    { }

    public Result(string successMessage)
    {
        SuccessMessage = successMessage;
    }

    public Result(ResultStatus status, string? successMessage = null)
    {
        Status = status;
        SuccessMessage = successMessage;
    }

    public Result(ResultStatus status, IEnumerable<string> errors, string? successMessage = null, string? location = null)
    {
        Status = status;
        Errors = errors;
        SuccessMessage = successMessage;
        Location = location;
    }

    public static Result Error(params string[] errorMessage) => new(ResultStatus.Error, errorMessage);

    public static Result Success(ResultStatus resultStatus) => new(resultStatus);

    public static Result NotFound(string message) => new(ResultStatus.NotFound, [message]);
 
}

public partial class Result<T> : Result
{
    public T? Value { get; init; }

    protected Result()
    { }

    public Result(T value) => Value = value;

    protected internal Result(T value, string successMessage) : this(value) => SuccessMessage = successMessage;

    protected Result(ResultStatus status) => Status = status;

    public Result(string successMessage) : base(successMessage)
    {
    }

    public Result(ResultStatus status, string? successMessage = null) : base(status, successMessage)
    {
    }

    public string? CorrelationId { get; protected set; }

    /// <summary>
    /// Returns the current value.
    /// </summary>
    /// <returns></returns>
    public object? GetValue() => this.Value;

    /// <summary>
    /// Represents a successful operation and accepts a values as the result of the operation
    /// </summary>
    /// <param name="value">Sets the Value property</param>
    /// <returns>A Result<typeparamref name="T"/></returns>
    public static Result<T> Success(T value) => new(value);

    /// <summary>
    /// Represents a successful operation and accepts a values as the result of the operation
    /// Sets the SuccessMessage property to the provided value
    /// </summary>
    /// <param name="value">Sets the Value property</param>
    /// <param name="successMessage">Sets the SuccessMessage property</param>
    /// <returns>A Result<typeparamref name="T"/></returns>
    public static Result<T> Success(T value, string successMessage) => new(value, successMessage);

    /// <summary>
    /// Represents a successful operation that resulted in the creation of a new resource.
    /// </summary>
    /// <typeparam name="T">The type of the resource created.</typeparam>
    /// <returns>A Result<typeparamref name="T"/> with status Created.</returns>
    public static Result<T> Created(T value) => new(ResultStatus.Created) { Value = value };

    /// <summary>
    /// Represents a successful operation that resulted in the creation of a new resource.
    /// Sets the SuccessMessage property to the provided value.
    /// </summary>
    /// <typeparam name="T">The type of the resource created.</typeparam>
    /// <param name="value">The value of the resource created.</param>
    /// <param name="location">The URL indicating where the newly created resource can be accessed.</param>
    /// <returns>A Result<typeparamref name="T"/> with status Created.</returns>
    public static Result<T> Created(T value, string location) => new(ResultStatus.Created) { Value = value, Location = location };

    /// <summary>
    /// Represents an error that occurred during the execution of the service.
    /// A single error message may be provided and will be exposed via the Errors property.
    /// </summary>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    public new static Result<T> Error(params string[] errorMessage) => new(ResultStatus.Error) { Errors = errorMessage };

    /// <summary>
    /// Represents an error that occurred during the execution of the service.
    /// Error messages may be provided and will be exposed via the Errors property.
    /// </summary>
    /// <param name="error">An optional instance of ErrorList with list of string error messages and CorrelationId.</param>
    /// <returns>A Result<typeparamref name="T"/></returns>
    public static Result<T> Error(ErrorList? error = null) => new(ResultStatus.Error)
    {
        CorrelationId = error?.CorrelationId ?? string.Empty,
        Errors = error?.ErrorMessages ?? []
    };

    /// <summary>
    /// Represents the situation where a service was unable to find a requested resource.
    /// </summary>
    /// <returns>A Result<typeparamref name="T"/></returns>
    public static Result<T> NotFound() => new(ResultStatus.NotFound);

    /// <summary>
    /// Represents the situation where a service was unable to find a requested resource.
    /// Error messages may be provided and will be exposed via the Errors property.
    /// </summary>
    /// <param name="errorMessages">A list of string error messages.</param>
    /// <returns>A Result<typeparamref name="T"/></returns>
    public static Result<T> NotFound(params string[] errorMessages) => new(ResultStatus.NotFound) { Errors = errorMessages };

    /// <summary>
    /// The parameters to the call were correct, but the user does not have permission to perform some action.
    /// See also HTTP 403 Forbidden: https://en.wikipedia.org/wiki/List_of_HTTP_statusCodes#4xxClient_errors
    /// </summary>
    /// <returns>A Result<typeparamref name="T"/></returns>
    public static Result<T> Forbidden() => new(ResultStatus.Forbidden);

    /// <summary>
    /// The parameters to the call were correct, but the user does not have permission to perform some action.
    /// See also HTTP 403 Forbidden: https://en.wikipedia.org/wiki/List_of_HTTP_statusCodes#4xxClient_errors
    /// </summary>
    /// <param name="errorMessages">A list of string error messages.</param>
    /// <returns>A Result<typeparamref name="T"/></returns>
    public static Result<T> Forbidden(params string[] errorMessages) => new(ResultStatus.Forbidden) { Errors = errorMessages };

    /// <summary>
    /// This is similar to Forbidden, but should be used when the user has not authenticated or has attempted to authenticate but failed.
    /// See also HTTP 401 Unauthorized: https://en.wikipedia.org/wiki/List_of_HTTP_statusCodes#4xxClient_errors
    /// </summary>
    /// <returns>A Result<typeparamref name="T"/></returns>
    public static Result<T> Unauthorized() => new(ResultStatus.Unauthorized);

    /// <summary>
    /// This is similar to Forbidden, but should be used when the user has not authenticated or has attempted to authenticate but failed.
    /// See also HTTP 401 Unauthorized: https://en.wikipedia.org/wiki/List_of_HTTP_statusCodes#4xxClient_errors
    /// </summary>
    /// <param name="errorMessages">A list of string error messages.</param>
    /// <returns>A Result<typeparamref name="T"/></returns>
    public static Result<T> Unauthorized(params string[] errorMessages) => new(ResultStatus.Unauthorized) { Errors = errorMessages };

    /// <summary>
    /// Represents a situation where a service is in conflict due to the current state of a resource,
    /// such as an edit conflict between multiple concurrent updates.
    /// See also HTTP 409 Conflict: https://en.wikipedia.org/wiki/List_of_HTTP_statusCodes#4xxClient_errors
    /// </summary>
    /// <returns>A Result<typeparamref name="T"/></returns>
    public static Result<T> Conflict() => new(ResultStatus.Conflict);

    /// <summary>
    /// Represents a situation where a service is in conflict due to the current state of a resource,
    /// such as an edit conflict between multiple concurrent updates.
    /// Error messages may be provided and will be exposed via the Errors property.
    /// See also HTTP 409 Conflict: https://en.wikipedia.org/wiki/List_of_HTTP_statusCodes#4xxClient_errors
    /// </summary>
    /// <param name="errorMessages">A list of string error messages.</param>
    /// <returns>A Result<typeparamref name="T"/></returns>
    public static Result<T> Conflict(params string[] errorMessages) => new(ResultStatus.Conflict) { Errors = errorMessages };

    /// <summary>
    /// Represents a critical error that occurred during the execution of the service.
    /// Everything provided by the user was valid, but the service was unable to complete due to an exception.
    /// See also HTTP 500 Internal Server Error: https://en.wikipedia.org/wiki/List_of_HTTP_statusCodes#5xx_server_errors
    /// </summary>
    /// <param name="errorMessages">A list of string error messages.</param>
    /// <returns>A Result<typeparamref name="T"/></returns>
    public static Result<T> CriticalError(params string[] errorMessages) => new(ResultStatus.CriticalError) { Errors = errorMessages };

    /// <summary>
    /// Represents a situation where a service is unavailable, such as when the underlying data store is unavailable.
    /// Errors may be transient, so the caller may wish to retry the operation.
    /// See also HTTP 503 Service Unavailable: https://en.wikipedia.org/wiki/List_of_HTTP_statusCodes#5xx_server_errors
    /// </summary>
    /// <param name="errorMessages">A list of string error messages</param>
    /// <returns></returns>
    public static Result<T> Unavailable(params string[] errorMessages) => new(ResultStatus.Unavailable) { Errors = errorMessages };

    /// <summary>
    /// Represents a situation where the server has successfully fulfilled the request, but there is no content to send back in the response body.
    /// </summary>
    /// <typeparam name="T">The type parameter representing the expected response data.</typeparam>
    /// <returns>A Result object</returns>
    public static Result<T> NoContent() => new(ResultStatus.NoContent);
}
