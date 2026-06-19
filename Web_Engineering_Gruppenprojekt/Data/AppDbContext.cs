using Microsoft.EntityFrameworkCore;
using Web_Engineering_Gruppenprojekt.Models;

namespace Web_Engineering_Gruppenprojekt.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Chapter> Chapters => Set<Chapter>();
    public DbSet<MCQuestion> Questions => Set<MCQuestion>();
    public DbSet<MCAnswerOption> AnswerOptions => Set<MCAnswerOption>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<ExamQuestion> ExamQuestions => Set<ExamQuestion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Composite PK for join table
        modelBuilder.Entity<ExamQuestion>()
            .HasKey(eq => new { eq.ExamId, eq.QuestionId });

        // Cascade: Course → Chapter → Question → Option
        modelBuilder.Entity<Chapter>()
            .HasOne(c => c.Course)
            .WithMany(co => co.Chapters)
            .HasForeignKey(c => c.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MCQuestion>()
            .HasOne(q => q.Chapter)
            .WithMany(c => c.Questions)
            .HasForeignKey(q => q.ChapterId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MCAnswerOption>()
            .HasOne(a => a.Question)
            .WithMany(q => q.Options)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Cascade: Course → Exam
        modelBuilder.Entity<Exam>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Exams)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Cascade: Exam → ExamQuestion (both sides)
        modelBuilder.Entity<ExamQuestion>()
            .HasOne(eq => eq.Exam)
            .WithMany(e => e.ExamQuestions)
            .HasForeignKey(eq => eq.ExamId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ExamQuestion>()
            .HasOne(eq => eq.Question)
            .WithMany(q => q.ExamQuestions)
            .HasForeignKey(eq => eq.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed data
        modelBuilder.Entity<Course>().HasData(
            new Course { Id = 1, Title = "Algorithmen und Datenstrukturen", LecturerName = "Prof. Dr. Müller", Level = CourseLevel.Bachelor },
            new Course { Id = 2, Title = "Maschinelles Lernen", LecturerName = "Prof. Dr. Schmidt", Level = CourseLevel.Master }
        );

        modelBuilder.Entity<Chapter>().HasData(
            new Chapter { Id = 1, Title = "Grundlagen der Sortieralgorithmen", ChapterNumber = 1, CourseId = 1 },
            new Chapter { Id = 2, Title = "Graphenalgorithmen", ChapterNumber = 2, CourseId = 1 },
            new Chapter { Id = 3, Title = "Neuronale Netze", ChapterNumber = 1, CourseId = 2 },
            new Chapter { Id = 4, Title = "Entscheidungsbäume", ChapterNumber = 2, CourseId = 2 }
        );

        modelBuilder.Entity<MCQuestion>().HasData(
            new MCQuestion { Id = 1, QuestionText = "Was ist die durchschnittliche Zeitkomplexität von QuickSort?", ChapterId = 1 },
            new MCQuestion { Id = 2, QuestionText = "Welcher Algorithmus verwendet das Divide-and-Conquer-Prinzip?", ChapterId = 1 },
            new MCQuestion { Id = 3, QuestionText = "Was beschreibt der Dijkstra-Algorithmus?", ChapterId = 2 },
            new MCQuestion { Id = 4, QuestionText = "Was ist ein azyklischer Graph?", ChapterId = 2 },
            new MCQuestion { Id = 5, QuestionText = "Was ist eine Aktivierungsfunktion in neuronalen Netzen?", ChapterId = 3 },
            new MCQuestion { Id = 6, QuestionText = "Was beschreibt Overfitting beim maschinellen Lernen?", ChapterId = 3 },
            new MCQuestion { Id = 7, QuestionText = "Was ist der Gini-Index bei Entscheidungsbäumen?", ChapterId = 4 },
            new MCQuestion { Id = 8, QuestionText = "Wann sollte man einen Entscheidungsbaum beschneiden (Pruning)?", ChapterId = 4 }
        );

        modelBuilder.Entity<MCAnswerOption>().HasData(
            // Q1
            new MCAnswerOption { Id = 1, AnswerText = "O(n²)", IsCorrect = false, QuestionId = 1 },
            new MCAnswerOption { Id = 2, AnswerText = "O(n log n)", IsCorrect = true, QuestionId = 1 },
            new MCAnswerOption { Id = 3, AnswerText = "O(n)", IsCorrect = false, QuestionId = 1 },
            new MCAnswerOption { Id = 4, AnswerText = "O(log n)", IsCorrect = false, QuestionId = 1 },
            // Q2
            new MCAnswerOption { Id = 5, AnswerText = "BubbleSort", IsCorrect = false, QuestionId = 2 },
            new MCAnswerOption { Id = 6, AnswerText = "MergeSort", IsCorrect = true, QuestionId = 2 },
            new MCAnswerOption { Id = 7, AnswerText = "InsertionSort", IsCorrect = false, QuestionId = 2 },
            new MCAnswerOption { Id = 8, AnswerText = "SelectionSort", IsCorrect = false, QuestionId = 2 },
            // Q3
            new MCAnswerOption { Id = 9, AnswerText = "Den kürzesten Weg in einem gewichteten Graphen", IsCorrect = true, QuestionId = 3 },
            new MCAnswerOption { Id = 10, AnswerText = "Die Tiefensuche in einem Graphen", IsCorrect = false, QuestionId = 3 },
            new MCAnswerOption { Id = 11, AnswerText = "Das Einfärben von Graphen", IsCorrect = false, QuestionId = 3 },
            new MCAnswerOption { Id = 12, AnswerText = "Den maximalen Fluss in einem Netzwerk", IsCorrect = false, QuestionId = 3 },
            // Q4
            new MCAnswerOption { Id = 13, AnswerText = "Ein Graph ohne Zyklen", IsCorrect = true, QuestionId = 4 },
            new MCAnswerOption { Id = 14, AnswerText = "Ein Graph mit genau einem Knoten", IsCorrect = false, QuestionId = 4 },
            new MCAnswerOption { Id = 15, AnswerText = "Ein vollständig verbundener Graph", IsCorrect = false, QuestionId = 4 },
            new MCAnswerOption { Id = 16, AnswerText = "Ein Graph mit negativen Kanten", IsCorrect = false, QuestionId = 4 },
            // Q5
            new MCAnswerOption { Id = 17, AnswerText = "Eine Funktion, die die Ausgabe eines Neurons bestimmt", IsCorrect = true, QuestionId = 5 },
            new MCAnswerOption { Id = 18, AnswerText = "Eine Methode zur Initialisierung der Gewichte", IsCorrect = false, QuestionId = 5 },
            new MCAnswerOption { Id = 19, AnswerText = "Ein Algorithmus zum Training des Netzes", IsCorrect = false, QuestionId = 5 },
            new MCAnswerOption { Id = 20, AnswerText = "Eine Schicht im neuronalen Netz", IsCorrect = false, QuestionId = 5 },
            // Q6
            new MCAnswerOption { Id = 21, AnswerText = "Das Modell passt sich zu stark an die Trainingsdaten an", IsCorrect = true, QuestionId = 6 },
            new MCAnswerOption { Id = 22, AnswerText = "Das Modell ist zu einfach für die Daten", IsCorrect = false, QuestionId = 6 },
            new MCAnswerOption { Id = 23, AnswerText = "Die Trainingsdaten enthalten Fehler", IsCorrect = false, QuestionId = 6 },
            new MCAnswerOption { Id = 24, AnswerText = "Das Modell hat zu wenige Parameter", IsCorrect = false, QuestionId = 6 },
            // Q7
            new MCAnswerOption { Id = 25, AnswerText = "Ein Maß für die Unreinheit eines Knotens", IsCorrect = true, QuestionId = 7 },
            new MCAnswerOption { Id = 26, AnswerText = "Ein Maß für die Tiefe des Baums", IsCorrect = false, QuestionId = 7 },
            new MCAnswerOption { Id = 27, AnswerText = "Die Anzahl der Blätter im Baum", IsCorrect = false, QuestionId = 7 },
            new MCAnswerOption { Id = 28, AnswerText = "Die Genauigkeit des Modells", IsCorrect = false, QuestionId = 7 },
            // Q8
            new MCAnswerOption { Id = 29, AnswerText = "Um Overfitting zu reduzieren", IsCorrect = true, QuestionId = 8 },
            new MCAnswerOption { Id = 30, AnswerText = "Um die Trainingsgeschwindigkeit zu erhöhen", IsCorrect = false, QuestionId = 8 },
            new MCAnswerOption { Id = 31, AnswerText = "Um mehr Daten zu verarbeiten", IsCorrect = false, QuestionId = 8 },
            new MCAnswerOption { Id = 32, AnswerText = "Um die Genauigkeit auf Trainingsdaten zu steigern", IsCorrect = false, QuestionId = 8 }
        );

        // Demo exam for course 1 (Algorithmen und Datenstrukturen)
        modelBuilder.Entity<Exam>().HasData(
            new Exam { Id = 1, Date = new DateTime(2026, 7, 15), CourseId = 1 }
        );

        modelBuilder.Entity<ExamQuestion>().HasData(
            new ExamQuestion { ExamId = 1, QuestionId = 1 },
            new ExamQuestion { ExamId = 1, QuestionId = 3 },
            new ExamQuestion { ExamId = 1, QuestionId = 4 }
        );
    }
}
