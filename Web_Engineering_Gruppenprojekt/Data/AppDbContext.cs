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

        // Kapitelnummer muss innerhalb eines Kurses eindeutig sein (verschiedene Kurse dürfen je bei 1 starten)
        modelBuilder.Entity<Chapter>()
            .HasIndex(c => new { c.CourseId, c.ChapterNumber })
            .IsUnique();

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
            new Chapter { Id = 4, Title = "Entscheidungsbäume", ChapterNumber = 2, CourseId = 2 },
            new Chapter { Id = 5, Title = "Laufzeitanalyse und Landau-Notation", ChapterNumber = 3, CourseId = 1 },
            new Chapter { Id = 6, Title = "Dynamische Programmierung", ChapterNumber = 4, CourseId = 1 },
            new Chapter { Id = 7, Title = "Support Vector Machines", ChapterNumber = 3, CourseId = 2 },
            new Chapter { Id = 8, Title = "Clustering-Verfahren", ChapterNumber = 4, CourseId = 2 }
        );

        modelBuilder.Entity<MCQuestion>().HasData(
            new MCQuestion { Id = 1, QuestionText = "Was ist die durchschnittliche Zeitkomplexität von QuickSort?", ChapterId = 1 },
            new MCQuestion { Id = 2, QuestionText = "Welcher Algorithmus verwendet das Divide-and-Conquer-Prinzip?", ChapterId = 1 },
            new MCQuestion { Id = 3, QuestionText = "Was beschreibt der Dijkstra-Algorithmus?", ChapterId = 2 },
            new MCQuestion { Id = 4, QuestionText = "Was ist ein azyklischer Graph?", ChapterId = 2 },
            new MCQuestion { Id = 5, QuestionText = "Was ist eine Aktivierungsfunktion in neuronalen Netzen?", ChapterId = 3 },
            new MCQuestion { Id = 6, QuestionText = "Was beschreibt Overfitting beim maschinellen Lernen?", ChapterId = 3 },
            new MCQuestion { Id = 7, QuestionText = "Was ist der Gini-Index bei Entscheidungsbäumen?", ChapterId = 4 },
            new MCQuestion { Id = 8, QuestionText = "Wann sollte man einen Entscheidungsbaum beschneiden (Pruning)?", ChapterId = 4 },
            new MCQuestion { Id = 9, QuestionText = "Was beschreibt die Landau-Notation O(f(n))?", ChapterId = 5 },
            new MCQuestion { Id = 10, QuestionText = "Welche Komplexitätsklasse hat die binäre Suche?", ChapterId = 5 },
            new MCQuestion { Id = 11, QuestionText = "Was ist ein zentrales Prinzip der dynamischen Programmierung?", ChapterId = 6 },
            new MCQuestion { Id = 12, QuestionText = "Welches Problem wird klassischerweise mit dynamischer Programmierung gelöst?", ChapterId = 6 },
            new MCQuestion { Id = 13, QuestionText = "Was maximiert eine Support Vector Machine?", ChapterId = 7 },
            new MCQuestion { Id = 14, QuestionText = "Was bewirkt der Kernel-Trick bei SVMs?", ChapterId = 7 },
            new MCQuestion { Id = 15, QuestionText = "Was ist das Ziel von k-Means-Clustering?", ChapterId = 8 },
            new MCQuestion { Id = 16, QuestionText = "Welches Verfahren ist ein hierarchisches Clustering-Verfahren?", ChapterId = 8 }
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
            new MCAnswerOption { Id = 32, AnswerText = "Um die Genauigkeit auf Trainingsdaten zu steigern", IsCorrect = false, QuestionId = 8 },
            // Q9
            new MCAnswerOption { Id = 33, AnswerText = "Eine obere Schranke für das asymptotische Wachstum", IsCorrect = true, QuestionId = 9 },
            new MCAnswerOption { Id = 34, AnswerText = "Die exakte Anzahl an Operationen", IsCorrect = false, QuestionId = 9 },
            new MCAnswerOption { Id = 35, AnswerText = "Die untere Schranke der Laufzeit", IsCorrect = false, QuestionId = 9 },
            new MCAnswerOption { Id = 36, AnswerText = "Den Speicherverbrauch eines Algorithmus", IsCorrect = false, QuestionId = 9 },
            // Q10
            new MCAnswerOption { Id = 37, AnswerText = "O(n)", IsCorrect = false, QuestionId = 10 },
            new MCAnswerOption { Id = 38, AnswerText = "O(log n)", IsCorrect = true, QuestionId = 10 },
            new MCAnswerOption { Id = 39, AnswerText = "O(n²)", IsCorrect = false, QuestionId = 10 },
            new MCAnswerOption { Id = 40, AnswerText = "O(1)", IsCorrect = false, QuestionId = 10 },
            // Q11
            new MCAnswerOption { Id = 41, AnswerText = "Wiederverwendung bereits berechneter Teilergebnisse", IsCorrect = true, QuestionId = 11 },
            new MCAnswerOption { Id = 42, AnswerText = "Zufällige Auswahl von Teilproblemen", IsCorrect = false, QuestionId = 11 },
            new MCAnswerOption { Id = 43, AnswerText = "Rekursion ohne Speicherung von Zwischenergebnissen", IsCorrect = false, QuestionId = 11 },
            new MCAnswerOption { Id = 44, AnswerText = "Sortierung der Eingabedaten", IsCorrect = false, QuestionId = 11 },
            // Q12
            new MCAnswerOption { Id = 45, AnswerText = "Rucksackproblem", IsCorrect = true, QuestionId = 12 },
            new MCAnswerOption { Id = 46, AnswerText = "Sortieren von Arrays", IsCorrect = false, QuestionId = 12 },
            new MCAnswerOption { Id = 47, AnswerText = "Traversierung eines Baums", IsCorrect = false, QuestionId = 12 },
            new MCAnswerOption { Id = 48, AnswerText = "Hashing von Strings", IsCorrect = false, QuestionId = 12 },
            // Q13
            new MCAnswerOption { Id = 49, AnswerText = "Den Abstand (Margin) zwischen den Klassen", IsCorrect = true, QuestionId = 13 },
            new MCAnswerOption { Id = 50, AnswerText = "Die Anzahl der Trainingsdaten", IsCorrect = false, QuestionId = 13 },
            new MCAnswerOption { Id = 51, AnswerText = "Die Tiefe des Entscheidungsbaums", IsCorrect = false, QuestionId = 13 },
            new MCAnswerOption { Id = 52, AnswerText = "Die Lernrate", IsCorrect = false, QuestionId = 13 },
            // Q14
            new MCAnswerOption { Id = 53, AnswerText = "Transformation der Daten in einen höherdimensionalen Raum", IsCorrect = true, QuestionId = 14 },
            new MCAnswerOption { Id = 54, AnswerText = "Reduktion der Trainingsdaten", IsCorrect = false, QuestionId = 14 },
            new MCAnswerOption { Id = 55, AnswerText = "Normalisierung der Eingabewerte", IsCorrect = false, QuestionId = 14 },
            new MCAnswerOption { Id = 56, AnswerText = "Entfernen von Ausreißern", IsCorrect = false, QuestionId = 14 },
            // Q15
            new MCAnswerOption { Id = 57, AnswerText = "Datenpunkte in k Gruppen mit minimaler Varianz einzuteilen", IsCorrect = true, QuestionId = 15 },
            new MCAnswerOption { Id = 58, AnswerText = "Die Klassifikation von Daten mit Labels", IsCorrect = false, QuestionId = 15 },
            new MCAnswerOption { Id = 59, AnswerText = "Die Vorhersage kontinuierlicher Werte", IsCorrect = false, QuestionId = 15 },
            new MCAnswerOption { Id = 60, AnswerText = "Die Reduktion der Merkmalsanzahl", IsCorrect = false, QuestionId = 15 },
            // Q16
            new MCAnswerOption { Id = 61, AnswerText = "Agglomeratives Clustering", IsCorrect = true, QuestionId = 16 },
            new MCAnswerOption { Id = 62, AnswerText = "k-Means", IsCorrect = false, QuestionId = 16 },
            new MCAnswerOption { Id = 63, AnswerText = "Lineare Regression", IsCorrect = false, QuestionId = 16 },
            new MCAnswerOption { Id = 64, AnswerText = "Random Forest", IsCorrect = false, QuestionId = 16 }
        );

        // Demo exams
        modelBuilder.Entity<Exam>().HasData(
            new Exam { Id = 1, Date = new DateTime(2026, 7, 15), CourseId = 1 },
            new Exam { Id = 2, Date = new DateTime(2026, 7, 22), CourseId = 2 }
        );

        modelBuilder.Entity<ExamQuestion>().HasData(
            new ExamQuestion { ExamId = 1, QuestionId = 1 },
            new ExamQuestion { ExamId = 1, QuestionId = 3 },
            new ExamQuestion { ExamId = 1, QuestionId = 4 },
            new ExamQuestion { ExamId = 2, QuestionId = 5 },
            new ExamQuestion { ExamId = 2, QuestionId = 7 },
            new ExamQuestion { ExamId = 2, QuestionId = 13 },
            new ExamQuestion { ExamId = 2, QuestionId = 15 }
        );
    }
}
