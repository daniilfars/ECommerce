using Shared.Domain;

namespace Modules.Catalog.Domain;

public sealed class ProductId : ValueObject
{
    public int Value { get; }

    private ProductId(int value) => Value = value;

    public static ProductId New() => new(0);
    public static ProductId From(int value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}