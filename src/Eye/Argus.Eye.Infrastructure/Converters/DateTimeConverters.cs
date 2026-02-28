using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Argus.Eye.Infrastructure.Converters;

public class DateOnlyConverter() : ValueConverter<DateOnly, DateTime>(
    dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
    dateTime => DateOnly.FromDateTime(dateTime));

public class TimeOnlyConverter() : ValueConverter<TimeOnly, TimeSpan>(
    timeOnly => timeOnly.ToTimeSpan(),
    timeSpan => TimeOnly.FromTimeSpan(timeSpan));
