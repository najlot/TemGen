using Microsoft.AspNetCore.Mvc;
using <# Project.Namespace#>.Service.Shared.Results;

namespace <# Project.Namespace#>.Service.Shared.Results;

internal static class ResultControllerExtensions
{
	public static ActionResult ToActionResult(this ControllerBase controller, Result result)
	{
		if (result.IsSuccess)
		{
			return controller.Ok();
		}

		return controller.ToFailureActionResult(result);
	}

	public static ActionResult<T> ToActionResult<T>(this ControllerBase controller, Result<T> result)
	{
		if (result.IsSuccess)
		{
			return controller.Ok(result.Value);
		}

		return controller.ToFailureActionResult(result);
	}

	private static ActionResult ToFailureActionResult(this ControllerBase controller, Result result)
	{
		return result.ErrorCode switch
		{
			ResultErrorCode.NotFound => controller.NotFound(result.Error),
			ResultErrorCode.Conflict => controller.Conflict(result.Error),
			ResultErrorCode.Forbidden => controller.Forbid(),
			ResultErrorCode.Unauthorized => controller.Unauthorized(result.Error),
			_ => controller.BadRequest(result.Error)
		};
	}

	private static ActionResult<T> ToFailureActionResult<T>(this ControllerBase controller, Result<T> result)
	{
		return result.ErrorCode switch
		{
			ResultErrorCode.NotFound => controller.NotFound(result.Error),
			ResultErrorCode.Conflict => controller.Conflict(result.Error),
			ResultErrorCode.Forbidden => controller.Forbid(),
			ResultErrorCode.Unauthorized => controller.Unauthorized(result.Error),
			_ => controller.BadRequest(result.Error)
		};
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>