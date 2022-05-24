namespace Core;

public record ServiceId(string Value)
{
    public static implicit operator string(ServiceId value) => value.Value;
    public static implicit operator ServiceId(string value) => new(value);
    
    public override string ToString() => Value;
}