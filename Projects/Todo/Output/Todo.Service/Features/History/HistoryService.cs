using System.Globalization;
using System.Text.Json;
using Najlot.Audit;
using Najlot.Map;
using Todo.Contracts;
using Todo.Service.Features.Auth;
using Todo.Service.Shared.Results;

namespace Todo.Service.Features.History;

public class HistoryService(
	IHistoryRepository repository,
	IAudit audit,
	IMap map,
	IUserIdProvider userIdProvider)
{
	public AuditSnapshot<T> CreateSnapshot<T>(T entity)
	{
		return audit.CreateSnapshot(entity);
	}

	public async Task<Result<HistoryEntry[]>> GetHistoryEntries(Guid entityId)
	{
		var entries = await repository.GetHistoryEntries(entityId).ConfigureAwait(false);
		return Result<HistoryEntry[]>.Success(map.From<HistoryModel>(entries).ToArray<HistoryEntry>());
	}

	public async Task WriteChangesAsync<T>(Guid entityId, AuditSnapshot<T> snapshot)
	{
		var changes = snapshot.GetChanges().ToArray();
		if (changes.Length == 0)
		{
			return;
		}

		await WriteEntryAsync(entityId, changes).ConfigureAwait(false);
	}

	public Task DeleteHistoryEntriesAsync(Guid entityId)
	{
		return repository.DeleteHistoryEntries(entityId);
	}

	private async Task WriteEntryAsync(Guid entityId, PropertyChange[] changes)
	{
		var historyChanges = changes.Select(static change => new HistoryChange
		{
			Path = change.Path,
			OldValue = FormatHistoryValue(change.OldValue),
			NewValue = FormatHistoryValue(change.NewValue),
		}).ToArray();

		var entry = new HistoryModel
		{
			Id = Guid.NewGuid(),
			EntityId = entityId,
			UserId = GetCurrentUserId(),
			Username = GetCurrentUsername(),
			TimeStamp = DateTime.UtcNow,
			Changes = JsonSerializer.Serialize(historyChanges)
		};

		await repository.Insert(entry).ConfigureAwait(false);
	}

	private static string FormatHistoryValue(object? value)
	{
		return value switch
		{
			null => string.Empty,
			string text => text,
			char character => character.ToString(),
			bool boolean => boolean.ToString(),
			Enum enumValue => enumValue.ToString(),
			DateTime dateTime => dateTime.ToString("O", CultureInfo.InvariantCulture),
			DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("O", CultureInfo.InvariantCulture),
			TimeSpan timeSpan => timeSpan.ToString("c", CultureInfo.InvariantCulture),
			IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture) ?? string.Empty,
			_ => JsonSerializer.Serialize(value),
		};
	}

	private Guid GetCurrentUserId()
	{
		try
		{
			return userIdProvider.GetRequiredUserId();
		}
		catch (InvalidOperationException)
		{
			return Guid.Empty;
		}
		catch (FormatException)
		{
			return Guid.Empty;
		}
	}

	private string GetCurrentUsername()
	{
		try
		{
			return userIdProvider.GetRequiredUsername();
		}
		catch (InvalidOperationException)
		{
			return string.Empty;
		}
	}
}
