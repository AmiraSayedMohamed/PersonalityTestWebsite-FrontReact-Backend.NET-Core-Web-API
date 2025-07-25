using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalityTestAPI.Models;

public class Session
{
    [Key]
    public string SessionId { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(10)]
    public string? Gender { get; set; }
    
    public int? BirthYear { get; set; }
    
    [MaxLength(50)]
    public string? EducationLevel { get; set; }
    
    [MaxLength(50)]
    public string? MaritalStatus { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "active";
    
    [Required]
    [MaxLength(50)]
    public string CurrentDimension { get; set; } = "openness";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();
    public virtual ICollection<DimensionScore> DimensionScores { get; set; } = new List<DimensionScore>();
}

public class Answer
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string SessionId { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(10)]
    public string QuestionId { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Dimension { get; set; } = string.Empty;
    
    [Required]
    [Range(1, 5)]
    public int Response { get; set; }
    
    public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    [ForeignKey("SessionId")]
    public virtual Session Session { get; set; } = null!;
}

public class DimensionScore
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string SessionId { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Dimension { get; set; } = string.Empty;
    
    public double Theta { get; set; } = 0.0;
    
    public int QuestionCount { get; set; } = 0;
    
    // Navigation property
    [ForeignKey("SessionId")]
    public virtual Session Session { get; set; } = null!;
}

public class Question
{
    [Key]
    [MaxLength(10)]
    public string QuestionId { get; set; } = string.Empty;
    
    [Required]
    public string Text { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Dimension { get; set; } = string.Empty;
    
    public bool ReverseScored { get; set; } = false;
    
    public double Difficulty { get; set; } = 0.0;
}
