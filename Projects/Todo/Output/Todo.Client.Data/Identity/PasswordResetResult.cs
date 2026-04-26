using System;

namespace Todo.Client.Data.Identity;

public class PasswordResetResult : IEquatable<PasswordResetResult>
{
	public bool IsSuccess { get; }
	public string ErrorMessage { get; }

	private PasswordResetResult(bool isSuccess, string errorMessage)
	{
		IsSuccess = isSuccess;
		ErrorMessage = errorMessage;
	}

	public static PasswordResetResult Success()
	{
		return new PasswordResetResult(true, string.Empty);
	}

	public static PasswordResetResult Failure(string errorMessage)
	{
		if (string.IsNullOrWhiteSpace(errorMessage))
		{
			throw new ArgumentException("Error message must be provided for failure result.", nameof(errorMessage));
		}

		return new PasswordResetResult(false, errorMessage);
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as PasswordResetResult);
	}

	public bool Equals(PasswordResetResult? other)
	{
		if (other is null) return false;
		if (ReferenceEquals(this, other)) return true;
		return IsSuccess == other.IsSuccess && ErrorMessage == other.ErrorMessage;
	}

	public override int GetHashCode() => HashCode.Combine(IsSuccess, ErrorMessage);

	public static bool operator ==(PasswordResetResult left, PasswordResetResult right)
	{
		if (left is null)
		{
			return right is null;
		}

		return left.Equals(right);
	}

	public static bool operator !=(PasswordResetResult left, PasswordResetResult right)
	{
		return !(left == right);
	}
}