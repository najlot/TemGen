#nullable enable

using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Contracts.Auth;
using Todo.Service.Features.Auth;
using Todo.Service.Features.Users;

namespace Todo.Service.Test.Features.Auth;

public class PasswordResetServiceTests
{
	[Test]
	public async Task RequestPasswordReset_must_store_hashed_code_and_send_mail_for_existing_user()
	{
		var user = new UserModel
		{
			Id = Guid.NewGuid(),
			Username = "alice",
			EMail = "alice@example.com",
			PasswordHash = PasswordHasher.HashPassword("old-password")
		};
		var userRepository = new InMemoryUserRepository([user]);
		var codeSender = new FakePasswordResetCodeSender();
		var service = new PasswordResetService(userRepository, codeSender, NullLogger<PasswordResetService>.Instance);

		var result = await service.RequestPasswordReset(new RequestPasswordReset { EMail = "alice@example.com", Culture = "de-DE" });

		Assert.Multiple(() =>
		{
			Assert.That(result.IsSuccess, Is.True);
			Assert.That(user.PasswordResetCodeHash, Is.Not.Null.And.Length.GreaterThan(0));
			Assert.That(user.PasswordResetCodeExpiresAt, Is.Not.Null);
			Assert.That(codeSender.Messages, Has.Count.EqualTo(1));
			Assert.That(codeSender.Messages[0].Email, Is.EqualTo("alice@example.com"));
			Assert.That(codeSender.Messages[0].Culture, Is.EqualTo("de-DE"));
			Assert.That(PasswordHasher.VerifyPassword(codeSender.Messages[0].Code, user.PasswordResetCodeHash), Is.True);
		});
	}

	[Test]
	public async Task RequestPasswordReset_must_succeed_without_sending_for_unknown_email()
	{
		var userRepository = new InMemoryUserRepository([]);
		var codeSender = new FakePasswordResetCodeSender();
		var service = new PasswordResetService(userRepository, codeSender, NullLogger<PasswordResetService>.Instance);

		var result = await service.RequestPasswordReset(new RequestPasswordReset { EMail = "missing@example.com" });

		Assert.Multiple(() =>
		{
			Assert.That(result.IsSuccess, Is.True);
			Assert.That(codeSender.Messages, Is.Empty);
		});
	}

	[Test]
	public async Task ResetPassword_must_replace_password_hash_and_clear_reset_code_for_valid_code()
	{
		var user = new UserModel
		{
			Id = Guid.NewGuid(),
			Username = "alice",
			EMail = "alice@example.com",
			PasswordHash = PasswordHasher.HashPassword("old-password"),
			PasswordResetCodeHash = PasswordHasher.HashPassword("123456"),
			PasswordResetCodeExpiresAt = DateTime.UtcNow.AddMinutes(5)
		};
		var userRepository = new InMemoryUserRepository([user]);
		var service = new PasswordResetService(userRepository, new FakePasswordResetCodeSender(), NullLogger<PasswordResetService>.Instance);

		var result = await service.ResetPassword(new ResetPassword
		{
			EMail = "alice@example.com",
			Code = "123456",
			Password = "new-password"
		});

		Assert.Multiple(() =>
		{
			Assert.That(result.IsSuccess, Is.True);
			Assert.That(PasswordHasher.VerifyPassword("new-password", user.PasswordHash), Is.True);
			Assert.That(PasswordHasher.VerifyPassword("old-password", user.PasswordHash), Is.False);
			Assert.That(user.PasswordResetCodeHash, Is.Null);
			Assert.That(user.PasswordResetCodeExpiresAt, Is.Null);
		});
	}

	[Test]
	public async Task ResetPassword_must_reject_invalid_or_expired_code()
	{
		var user = new UserModel
		{
			Id = Guid.NewGuid(),
			Username = "alice",
			EMail = "alice@example.com",
			PasswordHash = PasswordHasher.HashPassword("old-password"),
			PasswordResetCodeHash = PasswordHasher.HashPassword("123456"),
			PasswordResetCodeExpiresAt = DateTime.UtcNow.AddMinutes(-1)
		};
		var userRepository = new InMemoryUserRepository([user]);
		var service = new PasswordResetService(userRepository, new FakePasswordResetCodeSender(), NullLogger<PasswordResetService>.Instance);

		var result = await service.ResetPassword(new ResetPassword
		{
			EMail = "alice@example.com",
			Code = "123456",
			Password = "new-password"
		});

		Assert.Multiple(() =>
		{
			Assert.That(result.IsFailure, Is.True);
			Assert.That(result.Error, Is.EqualTo("Invalid or expired password reset code."));
			Assert.That(PasswordHasher.VerifyPassword("old-password", user.PasswordHash), Is.True);
		});
	}

	private sealed class InMemoryUserRepository(IEnumerable<UserModel> initialUsers) : IUserRepository
	{
		private readonly List<UserModel> _users = initialUsers.ToList();

		public IQueryable<UserModel> GetAllQueryable()
		{
			return _users.AsQueryable();
		}

		public Task<UserModel?> Get(Guid id)
		{
			return Task.FromResult<UserModel?>(_users.FirstOrDefault(user => user.Id == id));
		}

		public Task<UserModel?> Get(string username)
		{
			return Task.FromResult<UserModel?>(_users.FirstOrDefault(user => user.DeletedAt == null && user.Username == username));
		}

		public Task<UserModel?> GetByEmail(string email)
		{
			return Task.FromResult<UserModel?>(_users.FirstOrDefault(user =>
				user.DeletedAt == null && string.Equals(user.EMail, email, StringComparison.OrdinalIgnoreCase)));
		}

		public Task Insert(UserModel model)
		{
			_users.Add(model);
			return Task.CompletedTask;
		}

		public Task Update(UserModel model)
		{
			var index = _users.FindIndex(user => user.Id == model.Id);
			if (index >= 0)
			{
				_users[index] = model;
			}

			return Task.CompletedTask;
		}

		public Task Delete(Guid id)
		{
			_users.RemoveAll(user => user.Id == id);
			return Task.CompletedTask;
		}
	}

	private sealed class FakePasswordResetCodeSender : IPasswordResetCodeSender
	{
		public bool CanSend { get; set; } = true;

		public List<(string Email, string Username, string Code, TimeSpan ExpiresIn, string Culture)> Messages { get; } = [];

		public Task SendAsync(string email, string username, string code, TimeSpan expiresIn, string culture)
		{
			Messages.Add((email, username, code, expiresIn, culture));
			return Task.CompletedTask;
		}
	}
}
