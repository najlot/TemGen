using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Todo.Contracts.Filters;

namespace Todo.Service.Features.Filters;

public static class FilterExtensions
{
	public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, string propertyName, FilterCondition condition)
	{
		if (string.IsNullOrWhiteSpace(propertyName))
		{
			return query;
		}

		var parameter = Expression.Parameter(typeof(T), "e");

		MemberExpression property;
		try
		{
			property = Expression.Property(parameter, propertyName);
		}
		catch (ArgumentException)
		{
			return query;
		}

		if (condition.Operator is FilterOperator.IsEmpty or FilterOperator.IsNotEmpty)
		{
			var emptyCheck = BuildEmptyCheck(property);
			if (emptyCheck is null)
			{
				return query;
			}

			var body = condition.Operator == FilterOperator.IsNotEmpty
				? Expression.Not(emptyCheck)
				: emptyCheck;

			return query.Where(Expression.Lambda<Func<T, bool>>(body, parameter));
		}

		if (string.IsNullOrWhiteSpace(condition.Value))
		{
			return query;
		}

		if (!TryParseValue(condition.Value, property.Type, out var parsedValue) || parsedValue is null)
		{
			return query;
		}

		var valueExpression = BuildValueExpression(parsedValue, property.Type);
		var filterExpression = BuildFilterExpression(property, valueExpression, condition.Operator);
		if (filterExpression is null)
		{
			return query;
		}

		return query.Where(Expression.Lambda<Func<T, bool>>(filterExpression, parameter));
	}

	private static Expression? BuildEmptyCheck(MemberExpression property)
	{
		if (property.Type == typeof(string)
			&& typeof(string).GetMethod(nameof(string.IsNullOrEmpty), [typeof(string)]) is { } isNullOrEmptyMethod)
		{
			return Expression.Call(isNullOrEmptyMethod, property);
		}

		if (!CanBeNull(property.Type))
		{
			return null;
		}

		return Expression.Equal(property, Expression.Constant(null, property.Type));
	}

	private static Expression BuildValueExpression(object value, Type propertyType)
	{
		var underlyingType = Nullable.GetUnderlyingType(propertyType);
		if (underlyingType is null)
		{
			return Expression.Constant(value, propertyType);
		}

		return Expression.Convert(Expression.Constant(value, underlyingType), propertyType);
	}

	private static Expression? BuildFilterExpression(MemberExpression property, Expression valueExpression, FilterOperator filterOperator)
	{
		return filterOperator switch
		{
			FilterOperator.Equals => Expression.Equal(property, valueExpression),
			FilterOperator.NotEquals => Expression.NotEqual(property, valueExpression),
			FilterOperator.GreaterThan when SupportsOrderedComparison(property.Type) => Expression.GreaterThan(property, valueExpression),
			FilterOperator.GreaterThanOrEqual when SupportsOrderedComparison(property.Type) => Expression.GreaterThanOrEqual(property, valueExpression),
			FilterOperator.LessThan when SupportsOrderedComparison(property.Type) => Expression.LessThan(property, valueExpression),
			FilterOperator.LessThanOrEqual when SupportsOrderedComparison(property.Type) => Expression.LessThanOrEqual(property, valueExpression),
			FilterOperator.Contains => BuildStringMethodExpression(property, valueExpression, nameof(string.Contains)),
			FilterOperator.DoesNotContain => BuildNegatedStringMethodExpression(property, valueExpression, nameof(string.Contains)),
			FilterOperator.StartsWith => BuildStringMethodExpression(property, valueExpression, nameof(string.StartsWith)),
			FilterOperator.EndsWith => BuildStringMethodExpression(property, valueExpression, nameof(string.EndsWith)),
			_ => null,
		};
	}

	private static Expression? BuildNegatedStringMethodExpression(MemberExpression property, Expression valueExpression, string methodName)
	{
		var methodExpression = BuildStringMethodExpression(property, valueExpression, methodName);
		return methodExpression is null ? null : Expression.Not(methodExpression);
	}

	private static Expression? BuildStringMethodExpression(MemberExpression property, Expression valueExpression, string methodName)
	{
		if (property.Type != typeof(string) || valueExpression.Type != typeof(string))
		{
			return null;
		}

		var method = typeof(string).GetMethod(methodName, [typeof(string)]);
		if (method is null)
		{
			return null;
		}

		var methodCall = Expression.Call(property, method, valueExpression);
		var nullCheck = Expression.NotEqual(property, Expression.Constant(null, typeof(string)));
		return Expression.AndAlso(nullCheck, methodCall);
	}

	private static bool TryParseValue(string value, Type propertyType, out object? parsedValue)
	{
		var targetType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

		if (targetType == typeof(string))
		{
			parsedValue = value;
			return true;
		}

		if (targetType.IsEnum)
		{
			if (Enum.TryParse(targetType, value, ignoreCase: true, out var enumValue))
			{
				parsedValue = enumValue;
				return true;
			}

			parsedValue = null;
			return false;
		}

		var converter = TypeDescriptor.GetConverter(targetType);
		if (converter.CanConvertFrom(typeof(string)))
		{
			try
			{
				parsedValue = converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
				return parsedValue is not null;
			}
			catch
			{
			}

			try
			{
				parsedValue = converter.ConvertFrom(null, CultureInfo.CurrentCulture, value);
				return parsedValue is not null;
			}
			catch
			{
			}
		}

		parsedValue = null;
		return false;
	}

	private static bool SupportsOrderedComparison(Type propertyType)
	{
		var targetType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
		return targetType == typeof(DateTime)
			|| targetType == typeof(long)
			|| targetType == typeof(short)
			|| targetType == typeof(int)
			|| targetType == typeof(ulong)
			|| targetType == typeof(ushort)
			|| targetType == typeof(uint)
			|| targetType == typeof(decimal)
			|| targetType == typeof(double)
			|| targetType == typeof(float);
	}

	private static bool CanBeNull(Type propertyType)
		=> !propertyType.IsValueType || Nullable.GetUnderlyingType(propertyType) is not null;
}
