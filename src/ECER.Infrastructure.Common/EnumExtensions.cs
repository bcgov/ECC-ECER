namespace ECER.Infrastructure.Common;

public static class EnumExtensions
{
  public static TEnumTo Convert<TEnumFrom, TEnumTo>(this TEnumFrom value)
    where TEnumFrom : struct
    where TEnumTo : struct
    => Enum.Parse<TEnumTo>(value.ToString()!);

  public static IEnumerable<TEnumTo> Convert<TEnumFrom, TEnumTo>(this IEnumerable<TEnumFrom> values)
  where TEnumFrom : struct
  where TEnumTo : struct
    => values.Select(Convert<TEnumFrom, TEnumTo>);
}
