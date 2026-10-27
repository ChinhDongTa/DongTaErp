namespace DongTaErp.Application.Common.Models;

public enum SqlArgDirection
{
    Input = 1,
    Output = 2,
    InputOutput = 3
}

public sealed class SqlArg
{
    public string Name { get; init; } = "";
    public object? Value { get; private set; }
    public SqlArgDirection Direction { get; init; } = SqlArgDirection.Input;
    public int? Size { get; init; }

    public void SetValue(object? value) => Value = value;

    public static SqlArg In(string name, object? value) => new() { Name = name, Value = value };
    public static SqlArg Out(string name, int size = 100) => new() { Name = name, Direction = SqlArgDirection.Output, Size = size };
}
