using Microsoft.EntityFrameworkCore;
using PersonalityTestAPI.Models;

namespace PersonalityTestAPI.Data;

public class PersonalityTestContext : DbContext
{
    public PersonalityTestContext(DbContextOptions<PersonalityTestContext> options)
        : base(options)
    {
    }

    public DbSet<Session> Sessions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<DimensionScore> DimensionScores { get; set; }
    public DbSet<Question> Questions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships
        modelBuilder.Entity<Answer>()
            .HasOne(a => a.Session)
            .WithMany(s => s.Answers)
            .HasForeignKey(a => a.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DimensionScore>()
            .HasOne(ds => ds.Session)
            .WithMany(s => s.DimensionScores)
            .HasForeignKey(ds => ds.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed Questions Data
        SeedQuestions(modelBuilder);
    }

    private void SeedQuestions(ModelBuilder modelBuilder)
    {
        var questions = new List<Question>
        {
            // Openness questions
            new Question { QuestionId = "o1", Text = "أستمتع بالتفكير في الأفكار المجردة والمفاهيم النظرية.", Dimension = "openness", ReverseScored = false, Difficulty = -1 },
            new Question { QuestionId = "o2", Text = "لدي خيال خصب جداً.", Dimension = "openness", ReverseScored = false, Difficulty = -0.5 },
            new Question { QuestionId = "o3", Text = "أنا فضولي بشأن كل شيء تقريباً.", Dimension = "openness", ReverseScored = false, Difficulty = 0 },
            new Question { QuestionId = "o4", Text = "أفضل الروتين على التغيير.", Dimension = "openness", ReverseScored = true, Difficulty = 0.5 },
            new Question { QuestionId = "o5", Text = "أنا مبدع وأحب ابتكار أشياء جديدة.", Dimension = "openness", ReverseScored = false, Difficulty = 1 },
            new Question { QuestionId = "o6", Text = "أجد صعوبة في فهم الأفكار المجردة.", Dimension = "openness", ReverseScored = true, Difficulty = -1 },
            new Question { QuestionId = "o7", Text = "أحب تجربة الأنشطة الجديدة.", Dimension = "openness", ReverseScored = false, Difficulty = -0.5 },
            new Question { QuestionId = "o8", Text = "لست مهتماً بالفنون.", Dimension = "openness", ReverseScored = true, Difficulty = 0 },
            new Question { QuestionId = "o9", Text = "أحب حل المشكلات المعقدة.", Dimension = "openness", ReverseScored = false, Difficulty = 0.5 },
            new Question { QuestionId = "o10", Text = "أميل إلى التصويت للمرشحين المحافظين.", Dimension = "openness", ReverseScored = true, Difficulty = 1 },

            // Conscientiousness questions
            new Question { QuestionId = "c1", Text = "أنا دائماً مستعد ومنظم.", Dimension = "conscientiousness", ReverseScored = false, Difficulty = -1 },
            new Question { QuestionId = "c2", Text = "أترك أشيائي فوضوياً.", Dimension = "conscientiousness", ReverseScored = true, Difficulty = -0.5 },
            new Question { QuestionId = "c3", Text = "أهتم بالتفاصيل.", Dimension = "conscientiousness", ReverseScored = false, Difficulty = 0 },
            new Question { QuestionId = "c4", Text = "أؤجل المهام المهمة.", Dimension = "conscientiousness", ReverseScored = true, Difficulty = 0.5 },
            new Question { QuestionId = "c5", Text = "أتبع جدولاً زمنياً.", Dimension = "conscientiousness", ReverseScored = false, Difficulty = 1 },
            new Question { QuestionId = "c6", Text = "أنا دقيق في عملي.", Dimension = "conscientiousness", ReverseScored = false, Difficulty = -1 },
            new Question { QuestionId = "c7", Text = "أنسى أحياناً إعادة الأشياء إلى مكانها الصحيح.", Dimension = "conscientiousness", ReverseScored = true, Difficulty = -0.5 },
            new Question { QuestionId = "c8", Text = "أحب النظام.", Dimension = "conscientiousness", ReverseScored = false, Difficulty = 0 },
            new Question { QuestionId = "c9", Text = "أجد صعوبة في الالتزام بالخطط.", Dimension = "conscientiousness", ReverseScored = true, Difficulty = 0.5 },
            new Question { QuestionId = "c10", Text = "أنا مجتهد ومثابر.", Dimension = "conscientiousness", ReverseScored = false, Difficulty = 1 },

            // Extraversion questions
            new Question { QuestionId = "e1", Text = "أنا محور الاهتمام في الحفلات.", Dimension = "extraversion", ReverseScored = false, Difficulty = -1 },
            new Question { QuestionId = "e2", Text = "لا أتحدث كثيراً.", Dimension = "extraversion", ReverseScored = true, Difficulty = -0.5 },
            new Question { QuestionId = "e3", Text = "أشعر بالراحة حول الناس.", Dimension = "extraversion", ReverseScored = false, Difficulty = 0 },
            new Question { QuestionId = "e4", Text = "أفضل البقاء في الخلفية.", Dimension = "extraversion", ReverseScored = true, Difficulty = 0.5 },
            new Question { QuestionId = "e5", Text = "أبدأ المحادثات.", Dimension = "extraversion", ReverseScored = false, Difficulty = 1 },
            new Question { QuestionId = "e6", Text = "لدي دائرة واسعة من المعارف.", Dimension = "extraversion", ReverseScored = false, Difficulty = -1 },
            new Question { QuestionId = "e7", Text = "أنا هادئ حول الغرباء.", Dimension = "extraversion", ReverseScored = true, Difficulty = -0.5 },
            new Question { QuestionId = "e8", Text = "لا أمانع أن أكون مركز الاهتمام.", Dimension = "extraversion", ReverseScored = false, Difficulty = 0 },
            new Question { QuestionId = "e9", Text = "أفضل قضاء الوقت بمفردي.", Dimension = "extraversion", ReverseScored = true, Difficulty = 0.5 },
            new Question { QuestionId = "e10", Text = "أنا مفعم بالحيوية والنشاط.", Dimension = "extraversion", ReverseScored = false, Difficulty = 1 },

            // Agreeableness questions
            new Question { QuestionId = "a1", Text = "أتعاطف مع مشاعر الآخرين.", Dimension = "agreeableness", ReverseScored = false, Difficulty = -1 },
            new Question { QuestionId = "a2", Text = "لست مهتماً بمشاكل الآخرين.", Dimension = "agreeableness", ReverseScored = true, Difficulty = -0.5 },
            new Question { QuestionId = "a3", Text = "لدي قلب حنون.", Dimension = "agreeableness", ReverseScored = false, Difficulty = 0 },
            new Question { QuestionId = "a4", Text = "أهين الناس.", Dimension = "agreeableness", ReverseScored = true, Difficulty = 0.5 },
            new Question { QuestionId = "a5", Text = "أجعل الناس يشعرون بالراحة.", Dimension = "agreeableness", ReverseScored = false, Difficulty = 1 },
            new Question { QuestionId = "a6", Text = "أنا صبور مع الآخرين.", Dimension = "agreeableness", ReverseScored = false, Difficulty = -1 },
            new Question { QuestionId = "a7", Text = "أنا سريع الغضب.", Dimension = "agreeableness", ReverseScored = true, Difficulty = -0.5 },
            new Question { QuestionId = "a8", Text = "أثق بالآخرين.", Dimension = "agreeableness", ReverseScored = false, Difficulty = 0 },
            new Question { QuestionId = "a9", Text = "أنا متشكك في نوايا الآخرين.", Dimension = "agreeableness", ReverseScored = true, Difficulty = 0.5 },
            new Question { QuestionId = "a10", Text = "أنا متعاون بطبعي.", Dimension = "agreeableness", ReverseScored = false, Difficulty = 1 },

            // Neuroticism questions
            new Question { QuestionId = "n1", Text = "أشعر بالتوتر بسهولة.", Dimension = "neuroticism", ReverseScored = false, Difficulty = -1 },
            new Question { QuestionId = "n2", Text = "أنا مسترخٍ في معظم الأوقات.", Dimension = "neuroticism", ReverseScored = true, Difficulty = -0.5 },
            new Question { QuestionId = "n3", Text = "أقلق بشأن الأشياء.", Dimension = "neuroticism", ReverseScored = false, Difficulty = 0 },
            new Question { QuestionId = "n4", Text = "نادراً ما أشعر بالحزن.", Dimension = "neuroticism", ReverseScored = true, Difficulty = 0.5 },
            new Question { QuestionId = "n5", Text = "أنا متقلب المزاج.", Dimension = "neuroticism", ReverseScored = false, Difficulty = 1 },
            new Question { QuestionId = "n6", Text = "أتعامل مع التوتر بشكل جيد.", Dimension = "neuroticism", ReverseScored = true, Difficulty = -1 },
            new Question { QuestionId = "n7", Text = "أشعر بالقلق كثيراً.", Dimension = "neuroticism", ReverseScored = false, Difficulty = -0.5 },
            new Question { QuestionId = "n8", Text = "أنا مستقر عاطفياً.", Dimension = "neuroticism", ReverseScored = true, Difficulty = 0 },
            new Question { QuestionId = "n9", Text = "يمكن أن أكون سريع الانفعال.", Dimension = "neuroticism", ReverseScored = false, Difficulty = 0.5 },
            new Question { QuestionId = "n10", Text = "أنا راضٍ عن نفسي.", Dimension = "neuroticism", ReverseScored = true, Difficulty = 1 }
        };

        modelBuilder.Entity<Question>().HasData(questions);
    }
}
