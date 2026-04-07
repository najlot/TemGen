using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Najlot.Audit;
using Najlot.Map;
using <# Project.Namespace#>.Contracts;
using <# Project.Namespace#>.Service.Features.Auth;
using <# Project.Namespace#>.Service.Features.History;
using <# Project.Namespace#>.Service.Features.TodoItems;

namespace <# Project.Namespace#>.Service.Test.Mappings;

public class MapHistoryTests
{
	[Test]
	public async Task Write_history_changes_as_invariant_strings()
	{
		var previousCulture = CultureInfo.CurrentCulture;
		var previousUiCulture = CultureInfo.CurrentUICulture;

		try
		{
			CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
			CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("de-DE");

			var repository = new InMemoryHistoryRepository();
			var audit = new Audit();
			audit.Register<TestAuditEntity>(static entity =>
			[
				new PropertyValue(nameof(TestAuditEntity.Amount), entity.Amount),
				new PropertyValue(nameof(TestAuditEntity.Enabled), entity.Enabled),
				new PropertyValue(nameof(TestAuditEntity.CreatedAt), entity.CreatedAt),
			]);

			var service = new HistoryService(
				repository,
				audit,
				new Map().Register<#cs Write(Project.Namespace.Replace(".", ""))#>ServiceMappings(),
				new FakeUserIdProvider());

			var entity = new TestAuditEntity
			{
				Amount = 1234.5m,
				Enabled = false,
				CreatedAt = new DateTime(2026, 4, 1, 12, 34, 56, DateTimeKind.Utc),
			};

			var snapshot = service.CreateSnapshot(entity);
			entity.Amount = 6789.01m;
			entity.Enabled = true;
			entity.CreatedAt = entity.CreatedAt.AddHours(1);

			await service.WriteChangesAsync(Guid.NewGuid(), snapshot);

			var entry = repository.Inserted.Single();
			var result = JsonSerializer.Deserialize<HistoryChange[]>(entry.Changes);

			Assert.That(entry.Username, Is.EqualTo("tester"));
			Assert.That(result, Has.Length.EqualTo(3));
			Assert.That(result![0].OldValue, Is.EqualTo("1234.5"));
			Assert.That(result[0].NewValue, Is.EqualTo("6789.01"));
			Assert.That(result[1].OldValue, Is.EqualTo("False"));
			Assert.That(result[1].NewValue, Is.EqualTo("True"));
			Assert.That(result[2].OldValue, Is.EqualTo("2026-04-01T12:34:56.0000000Z"));
			Assert.That(result[2].NewValue, Is.EqualTo("2026-04-01T13:34:56.0000000Z"));
		}
		finally
		{
			CultureInfo.CurrentCulture = previousCulture;
			CultureInfo.CurrentUICulture = previousUiCulture;
		}
	}

	[Test]
	public async Task Write_history_changes_for_multiple_new_array_children()
	{
		var repository = new InMemoryHistoryRepository();
		var service = new HistoryService(
			repository,
			new Audit().Register<#cs Write(Project.Namespace.Replace(".", ""))#>ServiceAuditProviders(),
			new Map().Register<#cs Write(Project.Namespace.Replace(".", ""))#>ServiceMappings(),
			new FakeUserIdProvider());

		var entity = new TodoItemModel
		{
			Id = Guid.NewGuid(),
			Title = "Before",
			Content = "Before",
			CreatedAt = new DateTime(2026, 4, 1, 12, 34, 56, DateTimeKind.Utc),
			CreatedBy = "tester",
			ChangedAt = new DateTime(2026, 4, 1, 12, 34, 56, DateTimeKind.Utc),
			ChangedBy = "tester",
			Priority = "Normal",
			Checklist =
			[
				new ChecklistTask
				{
					Id = Guid.NewGuid(),
					Description = "Existing",
					IsDone = false,
				},
			],
		};

		var snapshot = service.CreateSnapshot(entity);
		entity.Checklist.AddRange(
			[
				new ChecklistTask
				{
					Id = Guid.NewGuid(),
					Description = "Added 1",
					IsDone = false,
				},
				new ChecklistTask
				{
					Id = Guid.NewGuid(),
					Description = "Added 2",
					IsDone = true,
				},
			]);

		await service.WriteChangesAsync(entity.Id, snapshot);

		Assert.That(repository.Inserted, Has.Count.EqualTo(1));
	}

	private sealed class TestAuditEntity
	{
		public decimal Amount { get; set; }
		public bool Enabled { get; set; }
		public DateTime CreatedAt { get; set; }
	}

	private sealed class FakeUserIdProvider : IUserIdProvider
	{
		public Guid GetRequiredUserId() => Guid.Parse("11111111-1111-1111-1111-111111111111");

		public string GetRequiredUsername() => "tester";
	}

	private sealed class InMemoryHistoryRepository : IHistoryRepository
	{
		public List<HistoryModel> Inserted { get; } = [];

		public IQueryable<HistoryModel> GetAllQueryable() => Inserted.AsQueryable();

		public Task<HistoryModel?> Get(Guid id) => Task.FromResult(Inserted.FirstOrDefault(item => item.Id == id));

		public Task Insert(HistoryModel model)
		{
			Inserted.Add(model);
			return Task.CompletedTask;
		}

		public Task Update(HistoryModel model) => Task.CompletedTask;

		public Task Delete(Guid id) => Task.CompletedTask;

		public Task<HistoryModel[]> GetHistoryEntries(Guid entityId)
			=> Task.FromResult(Inserted.Where(item => item.EntityId == entityId).ToArray());

		public Task DeleteHistoryEntries(Guid entityId)
		{
			Inserted.RemoveAll(item => item.EntityId == entityId);
			return Task.CompletedTask;
		}
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>