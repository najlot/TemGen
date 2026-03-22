using System;

namespace <# Project.Namespace#>.Service.Services;

public enum ResultErrorCode
{
	None = 0,
	Validation = 1,
	NotFound = 2,
	Conflict = 3,
	Forbidden = 4,
	Unauthorized = 5,
	Unexpected = 6
}

public readonly struct Result
{
	private readonly string? _error;
	private readonly ResultErrorCode _errorCode;

	public bool IsSuccess { get; }
	public bool IsFailure => !IsSuccess;
	public ResultErrorCode ErrorCode => IsFailure
		? _errorCode
		: throw new InvalidOperationException("No error code for success.");

	public string Error => IsFailure
		? _error!
		: throw new InvalidOperationException("No error for success.");

	private Result(bool isSuccess, string? error, ResultErrorCode errorCode)
	{
		IsSuccess = isSuccess;
		_error = error;
		_errorCode = errorCode;
	}

	public static Result Success() => new(true, null, ResultErrorCode.None);
	public static Result Validation(string error) => Failure(ResultErrorCode.Validation, error);
	public static Result NotFound(string error) => Failure(ResultErrorCode.NotFound, error);
	public static Result Conflict(string error) => Failure(ResultErrorCode.Conflict, error);
	public static Result Forbidden(string error) => Failure(ResultErrorCode.Forbidden, error);
	public static Result Unauthorized(string error) => Failure(ResultErrorCode.Unauthorized, error);
	public static Result Unexpected(string error) => Failure(ResultErrorCode.Unexpected, error);

	private static Result Failure(ResultErrorCode errorCode, string error)
	{
		if (errorCode == ResultErrorCode.None)
		{
			throw new ArgumentException("A failure result requires a non-success error code.", nameof(errorCode));
		}

		if (string.IsNullOrWhiteSpace(error))
		{
			throw new ArgumentException("Error message must be provided for failure result.", nameof(error));
		}

		return new Result(false, error, errorCode);
	}
}

public readonly struct Result<T>
{
	private readonly T _value;
	private readonly string? _error;
	private readonly ResultErrorCode _errorCode;

	public bool IsSuccess { get; }
	public bool IsFailure => !IsSuccess;
	public ResultErrorCode ErrorCode => IsFailure
		? _errorCode
		: throw new InvalidOperationException("No error code for success.");

	public T Value => IsSuccess
		? _value
		: throw new InvalidOperationException("No value for failure.");

	public string Error => IsFailure
		? _error!
		: throw new InvalidOperationException("No error for success.");

	private Result(T value)
	{
		_value = value;
		_error = null;
		_errorCode = ResultErrorCode.None;
		IsSuccess = true;
	}

	private Result(ResultErrorCode errorCode, string error)
	{
		if (errorCode == ResultErrorCode.None)
		{
			throw new ArgumentException("A failure result requires a non-success error code.", nameof(errorCode));
		}

		if (string.IsNullOrWhiteSpace(error))
		{
			throw new ArgumentException("Error message must be provided for failure result.", nameof(error));
		}

		_value = default!;
		_error = error;
		_errorCode = errorCode;
		IsSuccess = false;
	}

	public static Result<T> Success(T value) => new(value);
	public static Result<T> Validation(string error) => Failure(ResultErrorCode.Validation, error);
	public static Result<T> NotFound(string error) => Failure(ResultErrorCode.NotFound, error);
	public static Result<T> Conflict(string error) => Failure(ResultErrorCode.Conflict, error);
	public static Result<T> Forbidden(string error) => Failure(ResultErrorCode.Forbidden, error);
	public static Result<T> Unauthorized(string error) => Failure(ResultErrorCode.Unauthorized, error);
	public static Result<T> Unexpected(string error) => Failure(ResultErrorCode.Unexpected, error);

	private static Result<T> Failure(ResultErrorCode errorCode, string error) => new(errorCode, error);
}<#cs SetOutputPathAndSkipOtherDefinitions()#>