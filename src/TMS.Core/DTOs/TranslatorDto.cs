namespace TMS.Core.DTOs;

public class TranslatorDto
{
    public string Id { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public List<string> Languages { get; set; } = new();
    public string Specialization { get; set; } = string.Empty;
    public decimal RatePerWord { get; set; }
    public double AverageSpeed { get; set; }
    public double Rating { get; set; }
}

public class CreateTranslatorDto
{
    public string UserId { get; set; } = null!;
    public List<string> Languages { get; set; } = new();
    public string Specialization { get; set; } = string.Empty;
    public decimal RatePerWord { get; set; }
    public double AverageSpeed { get; set; }
}

public class UpdateTranslatorDto
{
    public List<string> Languages { get; set; } = new();
    public string Specialization { get; set; } = string.Empty;
    public decimal RatePerWord { get; set; }
    public double AverageSpeed { get; set; }
}
