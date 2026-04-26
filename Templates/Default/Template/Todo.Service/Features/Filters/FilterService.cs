using Najlot.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using <# Project.Namespace#>.Contracts.Filters;
using <# Project.Namespace#>.Contracts.Shared;
using <# Project.Namespace#>.Service.Features.Auth;
using <# Project.Namespace#>.Service.Shared.Results;

namespace <# Project.Namespace#>.Service.Features.Filters;

public sealed class FilterService(
	IFilterRepository filterRepository,
	IUserIdProvider userIdProvider,
	IMap map)
{
	public IAsyncEnumerable<Filter> GetItemsAsync(ItemType targetType)
	{
		var userId = userIdProvider.GetRequiredUserId();
		var query = filterRepository.GetAllQueryable()
			.Where(item => item.UserId == userId && item.TargetType == targetType)
			.OrderByDescending(item => item.IsDefault)
			.ThenBy(item => item.Name);

		return map.From<FilterModel>(query).To<Filter>().ToAsyncEnumerable();
	}

	public async Task<Result> CreateFilterAsync(CreateFilter command)
	{
		var validation = Validate(command.Name, command.IsDefault, command.Conditions);
		if (validation.IsFailure)
		{
			return validation;
		}

		command.Name = command.Name.Trim();

		var userId = userIdProvider.GetRequiredUserId();
		if (!command.IsDefault && HasDuplicateName(userId, command.TargetType, command.Name, excludeId: null))
		{
			return Result.Conflict("A filter with the same name already exists.");
		}

		if (command.IsDefault)
		{
			var hasDefaultFilter = filterRepository
				.GetAllQueryable()
				.Any(item => item.UserId == userId && item.TargetType == command.TargetType && item.IsDefault);

			if (hasDefaultFilter)
			{
				return Result.Conflict("A default filter already exists for this item type.");
			}
		}

		var model = map.From(command).To<FilterModel>();
		model.UserId = userId;
		await filterRepository.Insert(model).ConfigureAwait(false);
		return Result.Success();
	}

	public async Task<Result> UpdateFilterAsync(UpdateFilter command)
	{
		if (command.Id == Guid.Empty)
		{
			return Result.Validation("Filter id is required.");
		}

		var validation = Validate(command.Name, command.IsDefault, command.Conditions);
		if (validation.IsFailure)
		{
			return validation;
		}

		command.Name = command.Name.Trim();

		var userId = userIdProvider.GetRequiredUserId();
		var model = await filterRepository.Get(command.Id).ConfigureAwait(false);
		if (model is null)
		{
			return Result.NotFound("Filter not found.");
		}

		if (model.UserId != userId)
		{
			return Result.Forbidden("You must not modify other users' filters.");
		}

		if (!command.IsDefault && HasDuplicateName(userId, command.TargetType, command.Name, model.Id))
		{
			return Result.Conflict("A filter with the same name already exists.");
		}

		if (command.IsDefault)
		{
			var otherDefaults = filterRepository
				.GetAllQueryable()
				.Where(item => item.UserId == userId && item.TargetType == command.TargetType && item.IsDefault && item.Id != command.Id)
				.ToArray();

			foreach (var otherDefault in otherDefaults)
			{
				if (string.IsNullOrWhiteSpace(otherDefault.Name))
				{
					await filterRepository.Delete(otherDefault.Id).ConfigureAwait(false);
					continue;
				}

				otherDefault.IsDefault = false;
				await filterRepository.Update(otherDefault).ConfigureAwait(false);
			}
		}

		map.From(command).To(model);
		await filterRepository.Update(model).ConfigureAwait(false);
		return Result.Success();
	}

	public async Task<Result> DeleteItemAsync(Guid id)
	{
		var userId = userIdProvider.GetRequiredUserId();
		var model = await filterRepository.Get(id).ConfigureAwait(false);
		if (model is null)
		{
			return Result.NotFound("Filter not found.");
		}

		if (model.UserId != userId)
		{
			return Result.Forbidden("You must not delete other users' filters.");
		}

		await filterRepository.Delete(id).ConfigureAwait(false);
		return Result.Success();
	}

	private bool HasDuplicateName(Guid userId, ItemType targetType, string name, Guid? excludeId)
	{
		name = name.Trim();
		var query = filterRepository
			.GetAllQueryable()
			.Where(item => item.UserId == userId
						&& item.TargetType == targetType
						&& !item.IsDefault
						&& string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));

		if (excludeId.HasValue)
		{
			query = query.Where(item => item.Id != excludeId.Value);
		}

		return query.Any();
	}

	private static Result Validate(string name, bool isDefault, IReadOnlyCollection<FilterCondition> conditions)
	{
		if (conditions.Count == 0)
		{
			return Result.Validation("At least one filter condition is required.");
		}

		if (!isDefault && string.IsNullOrWhiteSpace(name))
		{
			return Result.Validation("Filter name is required.");
		}

		return Result.Success();
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>