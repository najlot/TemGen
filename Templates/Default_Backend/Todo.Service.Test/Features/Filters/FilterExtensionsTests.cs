using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using <# Project.Namespace#>.Contracts.Filters;
using <# Project.Namespace#>.Service.Features.Filters;

namespace <# Project.Namespace#>.Service.Test.Features.Filters;

public enum State
{
	None = 0,
	Active = 1,
	Pending = 2,
	Archived = 3
}

public sealed class ComprehensiveEntity
{
	public int Id { get; set; }
	public long BigInteger { get; set; }
	public short SmallInteger { get; set; }
	public byte ByteValue { get; set; }

	public double DoubleValue { get; set; }
	public float FloatValue { get; set; }
	public decimal CurrencyValue { get; set; }

	public string Title { get; set; }
	public char CategoryLetter { get; set; }
	public bool IsActive { get; set; }

	public DateTime CreatedAt { get; set; }
	public DateTimeOffset Timestamp { get; set; }
	public TimeSpan Duration { get; set; }

	public Guid UniqueIdentifier { get; set; }
	public Uri WebLink { get; set; }

	public int? OptionalCount { get; set; }
	public DateTime? DeletedAt { get; set; }

	public State State { get; set; }
}

public class FilterExtensionsTests
{
	private static readonly DateTime FirstCreatedAt = new(2024, 1, 1, 8, 0, 0);
	private static readonly DateTime SecondCreatedAt = new(2024, 1, 2, 8, 0, 0);
	private static readonly DateTime ThirdCreatedAt = new(2024, 1, 3, 8, 0, 0);
	private static readonly DateTime FourthCreatedAt = new(2024, 1, 4, 8, 0, 0);

	private static readonly DateTimeOffset FirstTimestamp = new(2024, 1, 1, 8, 0, 0, TimeSpan.Zero);
	private static readonly DateTimeOffset SecondTimestamp = new(2024, 1, 2, 8, 0, 0, TimeSpan.Zero);
	private static readonly DateTimeOffset ThirdTimestamp = new(2024, 1, 3, 8, 0, 0, TimeSpan.Zero);
	private static readonly DateTimeOffset FourthTimestamp = new(2024, 1, 4, 8, 0, 0, TimeSpan.Zero);

	private static readonly TimeSpan FirstDuration = TimeSpan.FromMinutes(30);
	private static readonly TimeSpan SecondDuration = TimeSpan.FromMinutes(60);
	private static readonly TimeSpan ThirdDuration = TimeSpan.FromMinutes(90);
	private static readonly TimeSpan FourthDuration = TimeSpan.FromMinutes(120);

	private static readonly Guid FirstGuid = Guid.Parse("11111111-1111-1111-1111-111111111111");
	private static readonly Guid SecondGuid = Guid.Parse("22222222-2222-2222-2222-222222222222");
	private static readonly Guid ThirdGuid = Guid.Parse("33333333-3333-3333-3333-333333333333");
	private static readonly Guid FourthGuid = Guid.Parse("44444444-4444-4444-4444-444444444444");

	private static readonly TestCaseData[] ValueFilterCases =
	[
		new TestCaseData(nameof(ComprehensiveEntity.Id), FilterOperator.Equals, "1", new[] { 1 })
			.SetName("ApplyFilter_should_filter_int_equals"),
		new TestCaseData(nameof(ComprehensiveEntity.Id), FilterOperator.NotEquals, "1", new[] { 2, 3, 4 })
			.SetName("ApplyFilter_should_filter_int_not_equals"),
		new TestCaseData(nameof(ComprehensiveEntity.BigInteger), FilterOperator.Equals, "2000", new[] { 2 })
			.SetName("ApplyFilter_should_filter_long_equals"),
		new TestCaseData(nameof(ComprehensiveEntity.SmallInteger), FilterOperator.NotEquals, "7", new[] { 1, 3, 4 })
			.SetName("ApplyFilter_should_filter_short_not_equals"),
		new TestCaseData(nameof(ComprehensiveEntity.ByteValue), FilterOperator.Equals, "3", new[] { 3 })
			.SetName("ApplyFilter_should_filter_byte_equals"),
		new TestCaseData(nameof(ComprehensiveEntity.DoubleValue), FilterOperator.GreaterThan, "2.0", new[] { 2, 3, 4 })
			.SetName("ApplyFilter_should_filter_double_greater_than"),
		new TestCaseData(nameof(ComprehensiveEntity.FloatValue), FilterOperator.LessThan, "3.0", new[] { 1, 2 })
			.SetName("ApplyFilter_should_filter_float_less_than"),
		new TestCaseData(nameof(ComprehensiveEntity.CurrencyValue), FilterOperator.GreaterThanOrEqual, "30.75", new[] { 3, 4 })
			.SetName("ApplyFilter_should_filter_decimal_greater_than_or_equal"),
		new TestCaseData(nameof(ComprehensiveEntity.CurrencyValue), FilterOperator.LessThanOrEqual, "20.5", new[] { 1, 2 })
			.SetName("ApplyFilter_should_filter_decimal_less_than_or_equal"),
		new TestCaseData(nameof(ComprehensiveEntity.Title), FilterOperator.Contains, "task", new[] { 1 })
			.SetName("ApplyFilter_should_filter_string_contains"),
		new TestCaseData(nameof(ComprehensiveEntity.Title), FilterOperator.DoesNotContain, "task", new[] { 2, 3, 4 })
			.SetName("ApplyFilter_should_filter_string_does_not_contain"),
		new TestCaseData(nameof(ComprehensiveEntity.Title), FilterOperator.StartsWith, "Alpha", new[] { 1 })
			.SetName("ApplyFilter_should_filter_string_starts_with"),
		new TestCaseData(nameof(ComprehensiveEntity.Title), FilterOperator.EndsWith, "item", new[] { 2 })
			.SetName("ApplyFilter_should_filter_string_ends_with"),
		new TestCaseData(nameof(ComprehensiveEntity.CategoryLetter), FilterOperator.Equals, "D", new[] { 4 })
			.SetName("ApplyFilter_should_filter_char_equals"),
		new TestCaseData(nameof(ComprehensiveEntity.IsActive), FilterOperator.Equals, bool.TrueString, new[] { 1, 3 })
			.SetName("ApplyFilter_should_filter_bool_equals"),
		new TestCaseData(nameof(ComprehensiveEntity.CreatedAt), FilterOperator.GreaterThanOrEqual, SecondCreatedAt.ToString("O", CultureInfo.InvariantCulture), new[] { 2, 3, 4 })
			.SetName("ApplyFilter_should_filter_datetime_greater_than_or_equal"),
		new TestCaseData(nameof(ComprehensiveEntity.CreatedAt), FilterOperator.LessThan, ThirdCreatedAt.ToString("O", CultureInfo.InvariantCulture), new[] { 1, 2 })
			.SetName("ApplyFilter_should_filter_datetime_less_than"),
		new TestCaseData(nameof(ComprehensiveEntity.Timestamp), FilterOperator.Equals, SecondTimestamp.ToString("O", CultureInfo.InvariantCulture), new[] { 2 })
			.SetName("ApplyFilter_should_filter_datetime_offset_equals"),
		new TestCaseData(nameof(ComprehensiveEntity.Duration), FilterOperator.Equals, ThirdDuration.ToString("c", CultureInfo.InvariantCulture), new[] { 3 })
			.SetName("ApplyFilter_should_filter_time_span_equals"),
		new TestCaseData(nameof(ComprehensiveEntity.UniqueIdentifier), FilterOperator.Equals, FourthGuid.ToString(), new[] { 4 })
			.SetName("ApplyFilter_should_filter_guid_equals"),
		new TestCaseData(nameof(ComprehensiveEntity.OptionalCount), FilterOperator.GreaterThan, "5", new[] { 3 })
			.SetName("ApplyFilter_should_filter_nullable_int_greater_than"),
		new TestCaseData(nameof(ComprehensiveEntity.State), FilterOperator.Equals, "pending", new[] { 2 })
			.SetName("ApplyFilter_should_filter_enum_equals_case_insensitive")
	];

	private static readonly TestCaseData[] EmptyFilterCases =
	[
		new TestCaseData(nameof(ComprehensiveEntity.Title), FilterOperator.IsEmpty, new[] { 3, 4 })
			.SetName("ApplyFilter_should_filter_string_is_empty"),
		new TestCaseData(nameof(ComprehensiveEntity.Title), FilterOperator.IsNotEmpty, new[] { 1, 2 })
			.SetName("ApplyFilter_should_filter_string_is_not_empty"),
		new TestCaseData(nameof(ComprehensiveEntity.OptionalCount), FilterOperator.IsEmpty, new[] { 2, 4 })
			.SetName("ApplyFilter_should_filter_nullable_int_is_empty"),
		new TestCaseData(nameof(ComprehensiveEntity.OptionalCount), FilterOperator.IsNotEmpty, new[] { 1, 3 })
			.SetName("ApplyFilter_should_filter_nullable_int_is_not_empty"),
		new TestCaseData(nameof(ComprehensiveEntity.DeletedAt), FilterOperator.IsEmpty, new[] { 1, 3, 4 })
			.SetName("ApplyFilter_should_filter_nullable_datetime_is_empty"),
		new TestCaseData(nameof(ComprehensiveEntity.DeletedAt), FilterOperator.IsNotEmpty, new[] { 2 })
			.SetName("ApplyFilter_should_filter_nullable_datetime_is_not_empty")
	];

	[TestCaseSource(nameof(ValueFilterCases))]
	public void ApplyFilter_should_handle_value_based_filters(string field, FilterOperator filterOperator, string value, int[] expectedIds)
	{
		var result = ApplyFilter(field, filterOperator, value);

		Assert.That(result, Is.EqualTo(expectedIds));
	}

	[TestCaseSource(nameof(EmptyFilterCases))]
	public void ApplyFilter_should_handle_empty_filters(string field, FilterOperator filterOperator, int[] expectedIds)
	{
		var result = ApplyFilter(field, filterOperator);

		Assert.That(result, Is.EqualTo(expectedIds));
	}

	private static int[] ApplyFilter(string field, FilterOperator filterOperator, string value = null)
	{
		var condition = new FilterCondition
		{
			Field = field,
			Operator = filterOperator,
			Value = value
		};

		return CreateQuery()
			.ApplyFilter(field, condition)
			.Select(static entity => entity.Id)
			.ToArray();
	}

	private static IQueryable<ComprehensiveEntity> CreateQuery()
		=> CreateEntities().AsQueryable();

	private static List<ComprehensiveEntity> CreateEntities()
	{
		return
		[
			new ComprehensiveEntity
			{
				Id = 1,
				BigInteger = 1_000,
				SmallInteger = 5,
				ByteValue = 1,
				DoubleValue = 1.5,
				FloatValue = 1.5f,
				CurrencyValue = 10.25m,
				Title = "Alpha task",
				CategoryLetter = 'A',
				IsActive = true,
				CreatedAt = FirstCreatedAt,
				Timestamp = FirstTimestamp,
				Duration = FirstDuration,
				UniqueIdentifier = FirstGuid,
				WebLink = new Uri("https://example.com/alpha"),
				OptionalCount = 3,
				DeletedAt = null,
				State = State.Active,
			},
			new ComprehensiveEntity
			{
				Id = 2,
				BigInteger = 2_000,
				SmallInteger = 7,
				ByteValue = 2,
				DoubleValue = 2.5,
				FloatValue = 2.5f,
				CurrencyValue = 20.50m,
				Title = "Beta item",
				CategoryLetter = 'B',
				IsActive = false,
				CreatedAt = SecondCreatedAt,
				Timestamp = SecondTimestamp,
				Duration = SecondDuration,
				UniqueIdentifier = SecondGuid,
				WebLink = new Uri("https://example.com/beta"),
				OptionalCount = null,
				DeletedAt = new DateTime(2024, 2, 1, 12, 0, 0, DateTimeKind.Utc),
				State = State.Pending,
			},
			new ComprehensiveEntity
			{
				Id = 3,
				BigInteger = 3_000,
				SmallInteger = 9,
				ByteValue = 3,
				DoubleValue = 3.5,
				FloatValue = 3.5f,
				CurrencyValue = 30.75m,
				Title = string.Empty,
				CategoryLetter = 'C',
				IsActive = true,
				CreatedAt = ThirdCreatedAt,
				Timestamp = ThirdTimestamp,
				Duration = ThirdDuration,
				UniqueIdentifier = ThirdGuid,
				WebLink = new Uri("https://example.com/archive"),
				OptionalCount = 8,
				DeletedAt = null,
				State = State.Archived,
			},
			new ComprehensiveEntity
			{
				Id = 4,
				BigInteger = 4_000,
				SmallInteger = 11,
				ByteValue = 4,
				DoubleValue = 4.5,
				FloatValue = 4.5f,
				CurrencyValue = 40.00m,
				Title = null,
				CategoryLetter = 'D',
				IsActive = false,
				CreatedAt = FourthCreatedAt,
				Timestamp = FourthTimestamp,
				Duration = FourthDuration,
				UniqueIdentifier = FourthGuid,
				WebLink = new Uri("https://example.com/empty"),
				OptionalCount = null,
				DeletedAt = null,
				State = State.None,
			}
		];
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>