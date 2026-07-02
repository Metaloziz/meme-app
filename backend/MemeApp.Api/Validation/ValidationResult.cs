namespace MemeApp.Api.Validation;

public readonly struct ValidationResult
{
    private ValidationResult(string? errorMessage) => ErrorMessage = errorMessage;

    public string? ErrorMessage { get; }

    public bool IsValid => ErrorMessage is null;

    public static ValidationResult Valid() => new(null);

    public static ValidationResult Invalid(string message) => new(message);
}
