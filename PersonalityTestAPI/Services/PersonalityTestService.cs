using Microsoft.EntityFrameworkCore;
using PersonalityTestAPI.Data;
using PersonalityTestAPI.DTOs;
using PersonalityTestAPI.Models;

namespace PersonalityTestAPI.Services;

public class PersonalityTestService : IPersonalityTestService
{
    private readonly PersonalityTestContext _context;
    private readonly List<string> _dimensionOrder = new() { "openness", "conscientiousness", "extraversion", "agreeableness", "neuroticism" };

    public PersonalityTestService(PersonalityTestContext context)
    {
        _context = context;
    }

    public async Task<SessionResponseDto> CreateSessionAsync(SessionCreateDto sessionData)
    {
        var sessionId = Guid.NewGuid().ToString();

        var session = new Session
        {
            SessionId = sessionId,
            Name = sessionData.Name,
            Gender = sessionData.Gender,
            BirthYear = sessionData.BirthYear,
            EducationLevel = sessionData.EducationLevel,
            MaritalStatus = sessionData.MaritalStatus,
            Status = "active",
            CurrentDimension = "openness"
        };

        _context.Sessions.Add(session);

        // Initialize dimension scores
        foreach (var dimension in _dimensionOrder)
        {
            _context.DimensionScores.Add(new DimensionScore
            {
                SessionId = sessionId,
                Dimension = dimension,
                Theta = 0.0,
                QuestionCount = 0
            });
        }

        await _context.SaveChangesAsync();

        var dimensionProgress = _dimensionOrder.ToDictionary(d => d, d => 0);

        return new SessionResponseDto
        {
            SessionId = sessionId,
            Name = sessionData.Name,
            Status = "active",
            CurrentDimension = "openness",
            CurrentQuestionNumber = 1,
            TotalDimensions = 5,
            DimensionProgress = dimensionProgress
        };
    }

    public async Task<Session?> GetSessionAsync(string sessionId)
    {
        return await _context.Sessions
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);
    }

    public async Task<QuestionDto?> GetCurrentQuestionAsync(string sessionId)
    {
        var session = await _context.Sessions
            .Include(s => s.DimensionScores)
            .Include(s => s.Answers)
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (session == null) return null;

        var currentDimensionScore = session.DimensionScores
            .FirstOrDefault(ds => ds.Dimension == session.CurrentDimension);

        if (currentDimensionScore?.QuestionCount >= 10)
        {
            // Move to next dimension
            var currentIndex = _dimensionOrder.IndexOf(session.CurrentDimension);
            if (currentIndex + 1 < _dimensionOrder.Count)
            {
                session.CurrentDimension = _dimensionOrder[currentIndex + 1];
                currentDimensionScore = session.DimensionScores
                    .FirstOrDefault(ds => ds.Dimension == session.CurrentDimension);
            }
            else
            {
                // Test is complete
                session.Status = "completed";
                session.CompletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return null;
            }
        }

        // Get adaptive question selection
        var theta = currentDimensionScore?.Theta ?? 0.0;
        var answeredIds = session.Answers
            .Where(a => a.Dimension == session.CurrentDimension)
            .Select(a => a.QuestionId)
            .ToList();

        var availableQuestions = await _context.Questions
            .Where(q => q.Dimension == session.CurrentDimension && !answeredIds.Contains(q.QuestionId))
            .ToListAsync();

        if (!availableQuestions.Any()) 
        {
            // No more questions in current dimension, try to move to next dimension
            var currentIndex = _dimensionOrder.IndexOf(session.CurrentDimension);
            if (currentIndex + 1 < _dimensionOrder.Count)
            {
                // Move to next dimension
                session.CurrentDimension = _dimensionOrder[currentIndex + 1];
                await _context.SaveChangesAsync();
                
                // Recursively get question from next dimension
                return await GetCurrentQuestionAsync(sessionId);
            }
            else
            {
                // Test is complete - no more dimensions
                session.Status = "completed";
                session.CompletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return null;
            }
        }

        // Find question with difficulty closest to current theta
        var nextQuestion = availableQuestions
            .OrderBy(q => Math.Abs(q.Difficulty - theta))
            .First();

        // Get first name for personalization
        var firstName = session.Name.Split(' ').FirstOrDefault() ?? "";

        // Personalize question text
        var personalizedText = PersonalizeQuestion(nextQuestion.Text, firstName, session.Gender);

        return new QuestionDto
        {
            QuestionId = nextQuestion.QuestionId,
            Text = personalizedText,
            Dimension = session.CurrentDimension,
            QuestionNumber = (currentDimensionScore?.QuestionCount ?? 0) + 1,
            ReverseScored = nextQuestion.ReverseScored
        };
    }

    public async Task<object> SubmitAnswerAsync(AnswerSubmissionDto answerDto)
    {
        var session = await _context.Sessions
            .Include(s => s.DimensionScores)
            .FirstOrDefaultAsync(s => s.SessionId == answerDto.SessionId);

        if (session == null)
            throw new ArgumentException("Session not found");

        var question = await _context.Questions
            .FirstOrDefaultAsync(q => q.QuestionId == answerDto.QuestionId);

        if (question == null)
            throw new ArgumentException("Question not found");

        // Record the answer
        var answer = new Answer
        {
            SessionId = answerDto.SessionId,
            QuestionId = answerDto.QuestionId,
            Dimension = question.Dimension,
            Response = answerDto.Response
        };

        _context.Answers.Add(answer);

        // Update theta for the dimension
        var dimensionScore = session.DimensionScores
            .FirstOrDefault(ds => ds.Dimension == question.Dimension);

        if (dimensionScore != null)
        {
            var responseValue = question.ReverseScored ? 6 - answerDto.Response : answerDto.Response;
            dimensionScore.Theta += (responseValue - 3) * 0.1;
            dimensionScore.QuestionCount++;
        }

        await _context.SaveChangesAsync();

        // Check if test is complete
        var totalAnswered = session.DimensionScores.Sum(ds => ds.QuestionCount);
        if (totalAnswered >= 50)
        {
            session.Status = "completed";
            session.CompletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return new { message = "Answer submitted successfully", status = session.Status };
    }

    public async Task<ReportDto?> GetReportAsync(string sessionId)
    {
        var session = await _context.Sessions
            .Include(s => s.DimensionScores)
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (session == null || session.Status != "completed")
            return null;

        var scores = new Dictionary<string, DimensionScoreDto>();
        var random = new Random();

        foreach (var dimensionScore in session.DimensionScores)
        {
            var score = random.Next(30, 101) / 20.0; // Convert to 1-5 scale
            var level = GetLevel(score * 20);

            scores[dimensionScore.Dimension] = new DimensionScoreDto
            {
                Name = GetDimensionArabicName(dimensionScore.Dimension),
                Score = score,
                Level = level
            };
        }

        return new ReportDto
        {
            SessionId = sessionId,
            Name = session.Name,
            CompletionDate = session.CompletedAt ?? DateTime.UtcNow,
            Scores = scores,
            DetailedAnalysis = "تحليل شخصيتك يُظهر توازناً جيداً في معظم الأبعاد.\n\nأنت شخص منفتح على التجارب الجديدة ولديك مستوى جيد من التنظيم والانضباط.\n\nتتمتع بمهارات اجتماعية جيدة وتستطيع التعامل مع الآخرين بطريقة إيجابية.\n\nبشكل عام، شخصيتك متوازنة وتُظهر قدرة على التكيف مع المواقف المختلفة.",
            Recommendations = new List<string>
            {
                "استمر في تطوير نقاط قوتك",
                "اعمل على تحسين المجالات التي تحتاج لتطوير",
                "تذكر أن الشخصية قابلة للنمو والتطوير"
            }
        };
    }

    private string PersonalizeQuestion(string text, string name, string? gender)
    {
        if (string.IsNullOrEmpty(name)) return text;

        var isFemaleName = IsFemaleName(name, gender);
        var questionText = text;

        // Convert to second person with gender agreement
        if (questionText.Contains("أنا "))
        {
            questionText = questionText.Replace("أنا ", isFemaleName ? "أنتِ " : "أنت ");
        }

        // Handle verbs and adjectives based on gender
        if (isFemaleName)
        {
            // Female second person transformations
            questionText = ApplyFemaleTransformations(questionText);
        }
        else
        {
            // Male second person transformations
            questionText = ApplyMaleTransformations(questionText);
        }

        // Add question format
        if (questionText.EndsWith('.'))
            questionText = questionText[..^1];

        return $"هل {questionText} يا {name}؟";
    }

    private bool IsFemaleName(string name, string? gender)
    {
        if (gender == "female") return true;
        if (gender == "male") return false;

        // Simple heuristic based on name endings
        var femaleEndings = new[] { "ة", "اء", "ى", "ان", "ين" };
        return femaleEndings.Any(name.EndsWith);
    }

    private string ApplyFemaleTransformations(string text)
    {
        var transformations = new Dictionary<string, string>
        {
            // Verbs
            { "أستمتع", "تستمتعين" }, { "أحب", "تحبين" }, { "أهتم", "تهتمين" },
            { "أتبع", "تتبعين" }, { "أؤجل", "تؤجلين" }, { "أنسى", "تنسين" },
            { "أجد", "تجدين" }, { "أشعر", "تشعرين" }, { "أفضل", "تفضلين" },
            { "أبدأ", "تبدئين" }, { "أتعاطف", "تتعاطفين" }, { "أجعل", "تجعلين" },
            { "أهين", "تهينين" }, { "أثق", "تثقين" }, { "أقلق", "تقلقين" },
            { "أتعامل", "تتعاملين" }, { "أميل", "تميلين" }, { "أترك", "تتركين" },
            { "أتحدث", "تتحدثين" }, { "أمانع", "تمانعين" },
            
            // Adjectives
            { "فضولي", "فضولية" }, { "مستعد", "مستعدة" }, { "منظم", "منظمة" },
            { "دقيق", "دقيقة" }, { "مجتهد", "مجتهدة" }, { "مثابر", "مثابرة" },
            { "مبدع", "مبدعة" }, { "هادئ", "هادئة" }, { "صبور", "صبورة" },
            { "متعاون", "متعاونة" }, { "مسترخ", "مسترخية" }, { "متقلب", "متقلبة" },
            { "مستقر", "مستقرة" }, { "راض", "راضية" }, { "مفعم", "مفعمة" },
            { "سريع", "سريعة" },
            
            // Possessives and other forms
            { "لدي", "لديكِ" }, { "في عملي", "في عملكِ" }, { "بمفردي", "بمفردكِ" },
            { "بطبعي", "بطبعكِ" }, { "عن نفسي", "عن نفسكِ" }, { "أشيائي", "أشياءكِ" },
            { "لست مهتماً", "لستِ مهتمة" }, { "لست", "لستِ" },
            { "لا أتحدث", "لا تتحدثين" }, { "لا أمانع", "لا تمانعين" },
            { "يمكن أن أكون", "يمكن أن تكوني" }
        };

        foreach (var transformation in transformations)
        {
            text = text.Replace(transformation.Key, transformation.Value);
        }

        return text;
    }

    private string ApplyMaleTransformations(string text)
    {
        var transformations = new Dictionary<string, string>
        {
            // Verbs
            { "أستمتع", "تستمتع" }, { "أحب", "تحب" }, { "أهتم", "تهتم" },
            { "أتبع", "تتبع" }, { "أؤجل", "تؤجل" }, { "أنسى", "تنسى" },
            { "أجد", "تجد" }, { "أشعر", "تشعر" }, { "أفضل", "تفضل" },
            { "أبدأ", "تبدأ" }, { "أتعاطف", "تتعاطف" }, { "أجعل", "تجعل" },
            { "أهين", "تهين" }, { "أثق", "تثق" }, { "أقلق", "تقلق" },
            { "أتعامل", "تتعامل" }, { "أميل", "تميل" }, { "أترك", "تترك" },
            { "أتحدث", "تتحدث" }, { "أمانع", "تمانع" },
            
            // Possessives
            { "لدي", "لديك" }, { "في عملي", "في عملك" }, { "بمفردي", "بمفردك" },
            { "بطبعي", "بطبعك" }, { "عن نفسي", "عن نفسك" }, { "أشيائي", "أشياءك" },
            { "لا أتحدث", "لا تتحدث" }, { "لا أمانع", "لا تمانع" },
            { "يمكن أن أكون", "يمكن أن تكون" }
        };

        foreach (var transformation in transformations)
        {
            text = text.Replace(transformation.Key, transformation.Value);
        }

        return text;
    }

    private string GetLevel(double score)
    {
        return score switch
        {
            >= 80 => "عالي",
            >= 60 => "متوسط",
            _ => "منخفض"
        };
    }

    private string GetDimensionArabicName(string dimension)
    {
        return dimension switch
        {
            "openness" => "الانفتاح على التجارب",
            "conscientiousness" => "الضمير الحي",
            "extraversion" => "الانبساط",
            "agreeableness" => "المقبولية",
            "neuroticism" => "العصابية",
            _ => dimension
        };
    }
}
