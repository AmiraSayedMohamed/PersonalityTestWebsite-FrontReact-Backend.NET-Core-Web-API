using System.ComponentModel.DataAnnotations;

namespace PersonalityTestAPI.DTOs;

public class SessionCreateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(10)]
    public string? Gender { get; set; }
    
    [Range(1900, 2100)]
    public int? BirthYear { get; set; }
    
    [MaxLength(50)]
    public string? EducationLevel { get; set; }
    
    [MaxLength(50)]
    public string? MaritalStatus { get; set; }
}

public class SessionResponseDto
{
    public string SessionId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CurrentDimension { get; set; } = string.Empty;
    public int CurrentQuestionNumber { get; set; }
    public int TotalDimensions { get; set; } = 5;
    public Dictionary<string, int> DimensionProgress { get; set; } = new();
}

public class QuestionDto
{
    public string QuestionId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string Dimension { get; set; } = string.Empty;
    public int QuestionNumber { get; set; }
    public bool ReverseScored { get; set; }
}

public class AnswerSubmissionDto
{
    [Required]
    public string SessionId { get; set; } = string.Empty;
    
    [Required]
    public string QuestionId { get; set; } = string.Empty;
    
    [Required]
    [Range(1, 5)]
    public int Response { get; set; }
}

public class ReportDto
{
    public string SessionId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CompletionDate { get; set; }
    public Dictionary<string, DimensionScoreDto> Scores { get; set; } = new();
    public string DetailedAnalysis { get; set; } = string.Empty;
    public List<string> Recommendations { get; set; } = new();
}

public class DimensionScoreDto
{
    public string Name { get; set; } = string.Empty;
    public double Score { get; set; }
    public string Level { get; set; } = string.Empty;
}
