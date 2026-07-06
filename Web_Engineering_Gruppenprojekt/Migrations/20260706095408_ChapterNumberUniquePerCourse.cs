using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Web_Engineering_Gruppenprojekt.Migrations
{
    /// <inheritdoc />
    public partial class ChapterNumberUniquePerCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Chapters_CourseId",
                table: "Chapters");

            migrationBuilder.InsertData(
                table: "Chapters",
                columns: new[] { "Id", "ChapterNumber", "CourseId", "SlideFileName", "SlidePath", "Title" },
                values: new object[,]
                {
                    { 5, 3, 1, null, null, "Laufzeitanalyse und Landau-Notation" },
                    { 6, 4, 1, null, null, "Dynamische Programmierung" },
                    { 7, 3, 2, null, null, "Support Vector Machines" },
                    { 8, 4, 2, null, null, "Clustering-Verfahren" }
                });

            migrationBuilder.InsertData(
                table: "Exams",
                columns: new[] { "Id", "CourseId", "Date" },
                values: new object[] { 2, 2, new DateTime(2026, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "ExamQuestions",
                columns: new[] { "ExamId", "QuestionId" },
                values: new object[,]
                {
                    { 2, 5 },
                    { 2, 7 }
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "ChapterId", "QuestionText" },
                values: new object[,]
                {
                    { 9, 5, "Was beschreibt die Landau-Notation O(f(n))?" },
                    { 10, 5, "Welche Komplexitätsklasse hat die binäre Suche?" },
                    { 11, 6, "Was ist ein zentrales Prinzip der dynamischen Programmierung?" },
                    { 12, 6, "Welches Problem wird klassischerweise mit dynamischer Programmierung gelöst?" },
                    { 13, 7, "Was maximiert eine Support Vector Machine?" },
                    { 14, 7, "Was bewirkt der Kernel-Trick bei SVMs?" },
                    { 15, 8, "Was ist das Ziel von k-Means-Clustering?" },
                    { 16, 8, "Welches Verfahren ist ein hierarchisches Clustering-Verfahren?" }
                });

            migrationBuilder.InsertData(
                table: "AnswerOptions",
                columns: new[] { "Id", "AnswerText", "IsCorrect", "QuestionId" },
                values: new object[,]
                {
                    { 33, "Eine obere Schranke für das asymptotische Wachstum", true, 9 },
                    { 34, "Die exakte Anzahl an Operationen", false, 9 },
                    { 35, "Die untere Schranke der Laufzeit", false, 9 },
                    { 36, "Den Speicherverbrauch eines Algorithmus", false, 9 },
                    { 37, "O(n)", false, 10 },
                    { 38, "O(log n)", true, 10 },
                    { 39, "O(n²)", false, 10 },
                    { 40, "O(1)", false, 10 },
                    { 41, "Wiederverwendung bereits berechneter Teilergebnisse", true, 11 },
                    { 42, "Zufällige Auswahl von Teilproblemen", false, 11 },
                    { 43, "Rekursion ohne Speicherung von Zwischenergebnissen", false, 11 },
                    { 44, "Sortierung der Eingabedaten", false, 11 },
                    { 45, "Rucksackproblem", true, 12 },
                    { 46, "Sortieren von Arrays", false, 12 },
                    { 47, "Traversierung eines Baums", false, 12 },
                    { 48, "Hashing von Strings", false, 12 },
                    { 49, "Den Abstand (Margin) zwischen den Klassen", true, 13 },
                    { 50, "Die Anzahl der Trainingsdaten", false, 13 },
                    { 51, "Die Tiefe des Entscheidungsbaums", false, 13 },
                    { 52, "Die Lernrate", false, 13 },
                    { 53, "Transformation der Daten in einen höherdimensionalen Raum", true, 14 },
                    { 54, "Reduktion der Trainingsdaten", false, 14 },
                    { 55, "Normalisierung der Eingabewerte", false, 14 },
                    { 56, "Entfernen von Ausreißern", false, 14 },
                    { 57, "Datenpunkte in k Gruppen mit minimaler Varianz einzuteilen", true, 15 },
                    { 58, "Die Klassifikation von Daten mit Labels", false, 15 },
                    { 59, "Die Vorhersage kontinuierlicher Werte", false, 15 },
                    { 60, "Die Reduktion der Merkmalsanzahl", false, 15 },
                    { 61, "Agglomeratives Clustering", true, 16 },
                    { 62, "k-Means", false, 16 },
                    { 63, "Lineare Regression", false, 16 },
                    { 64, "Random Forest", false, 16 }
                });

            migrationBuilder.InsertData(
                table: "ExamQuestions",
                columns: new[] { "ExamId", "QuestionId" },
                values: new object[,]
                {
                    { 2, 13 },
                    { 2, 15 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_CourseId_ChapterNumber",
                table: "Chapters",
                columns: new[] { "CourseId", "ChapterNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Chapters_CourseId_ChapterNumber",
                table: "Chapters");

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "ExamQuestions",
                keyColumns: new[] { "ExamId", "QuestionId" },
                keyValues: new object[] { 2, 5 });

            migrationBuilder.DeleteData(
                table: "ExamQuestions",
                keyColumns: new[] { "ExamId", "QuestionId" },
                keyValues: new object[] { 2, 7 });

            migrationBuilder.DeleteData(
                table: "ExamQuestions",
                keyColumns: new[] { "ExamId", "QuestionId" },
                keyValues: new object[] { 2, 13 });

            migrationBuilder.DeleteData(
                table: "ExamQuestions",
                keyColumns: new[] { "ExamId", "QuestionId" },
                keyValues: new object[] { 2, 15 });

            migrationBuilder.DeleteData(
                table: "Exams",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_CourseId",
                table: "Chapters",
                column: "CourseId");
        }
    }
}
