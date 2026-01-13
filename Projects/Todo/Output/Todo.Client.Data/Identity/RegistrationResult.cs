using System;

namespace Todo.Client.Data.Identity;

public class RegistrationResult : IEquatable<RegistrationResult>
{
	public bool IsSuccess { get; }
	public string ErrorMessage { get; }

	private RegistrationResult(bool isSuccess, string errorMessage)
	{
		IsSuccess = isSuccess;
		ErrorMessage = errorMessage;
	}

	public static RegistrationResult Success()
	{
		return new RegistrationResult(true, "");
	}

	public static RegistrationResult Failure(string errorMessage)
	{
		if (string.IsNullOrWhiteSpace(errorMessage))
		{
			throw new ArgumentException("Error message must be provided for failure result.", nameof(errorMessage));
		}

		return new RegistrationResult(false, errorMessage);
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as RegistrationResult);
	}

	public bool Equals(RegistrationResult? other)
	{
		if (other is null) return false;
		if (ReferenceEquals(this, other)) return true;
		return IsSuccess == other.IsSuccess && ErrorMessage == other.ErrorMessage;
	}

	public override int GetHashCode()
	{
		return (IsSuccess, ErrorMessage).GetHashCode();
	}

	public static bool operator ==(RegistrationResult left, RegistrationResult right)
	{
		if (left is null)
		{
			return right is null;
		}
		return left.Equals(right);
	}

	public static bool operator !=(RegistrationResult left, RegistrationResult right)
	{
		return !(left == right);
	}
}