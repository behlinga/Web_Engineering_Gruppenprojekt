using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Web_Engineering_Gruppenprojekt.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    LecturerName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Chapters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ChapterNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    SlideFileName = table.Column<string>(type: "TEXT", nullable: true),
                    SlidePath = table.Column<string>(type: "TEXT", nullable: true),
                    CourseId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chapters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chapters_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CourseId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exams_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    QuestionText = table.Column<string>(type: "TEXT", nullable: false),
                    ChapterId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "Chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnswerOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnswerText = table.Column<string>(type: "TEXT", nullable: false),
                    IsCorrect = table.Column<bool>(type: "INTEGER", nullable: false),
                    QuestionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerOptions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamQuestions",
                columns: table => new
                {
                    ExamId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamQuestions", x => new { x.ExamId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_ExamQuestions_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamQuestions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "LecturerName", "Level", "Title" },
                values: new object[,]
                {
                    { 1, "Prof. Dr. Müller", 0, "Algorithmen und Datenstrukturen" },
                    { 2, "Prof. Dr. Schmidt", 1, "Maschinelles Lernen" }
                });

            migrationBuilder.InsertData(
                table: "Chapters",
                columns: new[] { "Id", "ChapterNumber", "CourseId", "SlideFileName", "SlidePath", "Title" },
                values: new object[,]
                {
                    { 1, 1, 1, null, null, "Grundlagen der Sortieralgorithmen" },
                    { 2, 2, 1, null, null, "Graphenalgorithmen" },
                    { 3, 1, 2, null, null, "Neuronale Netze" },
                    { 4, 2, 2, null, null, "Entscheidungsbäume" }
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "ChapterId", "QuestionText" },
                values: new object[,]
                {
                    { 1, 1, "Was ist die durchschnittliche Zeitkomplexität von QuickSort?" },
                    { 2, 1, "Welcher Algorithmus verwendet das Divide-and-Conquer-Prinzip?" },
                    { 3, 2, "Was beschreibt der Dijkstra-Algorithmus?" },
                    { 4, 2, "Was ist ein azyklischer Graph?" },
                    { 5, 3, "Was ist eine Aktivierungsfunktion in neuronalen Netzen?" },
                    { 6, 3, "Was beschreibt Overfitting beim maschinellen Lernen?" },
                    { 7, 4, "Was ist der Gini-Index bei Entscheidungsbäumen?" },
                    { 8, 4, "Wann sollte man einen Entscheidungsbaum beschneiden (Pruning)?" }
                });

            migrationBuilder.InsertData(
                table: "AnswerOptions",
                columns: new[] { "Id", "AnswerText", "IsCorrect", "QuestionId" },
                values: new object[,]
                {
                    { 1, "O(n²)", false, 1 },
                    { 2, "O(n log n)", true, 1 },
                    { 3, "O(n)", false, 1 },
                    { 4, "O(log n)", false, 1 },
                    { 5, "BubbleSort", false, 2 },
                    { 6, "MergeSort", true, 2 },
                    { 7, "InsertionSort", false, 2 },
                    { 8, "SelectionSort", false, 2 },
                    { 9, "Den kürzesten Weg in einem gewichteten Graphen", true, 3 },
                    { 10, "Die Tiefensuche in einem Graphen", false, 3 },
                    { 11, "Das Einfärben von Graphen", false, 3 },
                    { 12, "Den maximalen Fluss in einem Netzwerk", false, 3 },
                    { 13, "Ein Graph ohne Zyklen", true, 4 },
                    { 14, "Ein Graph mit genau einem Knoten", false, 4 },
                    { 15, "Ein vollständig verbundener Graph", false, 4 },
                    { 16, "Ein Graph mit negativen Kanten", false, 4 },
                    { 17, "Eine Funktion, die die Ausgabe eines Neurons bestimmt", true, 5 },
                    { 18, "Eine Methode zur Initialisierung der Gewichte", false, 5 },
                    { 19, "Ein Algorithmus zum Training des Netzes", false, 5 },
                    { 20, "Eine Schicht im neuronalen Netz", false, 5 },
                    { 21, "Das Modell passt sich zu stark an die Trainingsdaten an", true, 6 },
                    { 22, "Das Modell ist zu einfach für die Daten", false, 6 },
                    { 23, "Die Trainingsdaten enthalten Fehler", false, 6 },
                    { 24, "Das Modell hat zu wenige Parameter", false, 6 },
                    { 25, "Ein Maß für die Unreinheit eines Knotens", true, 7 },
                    { 26, "Ein Maß für die Tiefe des Baums", false, 7 },
                    { 27, "Die Anzahl der Blätter im Baum", false, 7 },
                    { 28, "Die Genauigkeit des Modells", false, 7 },
                    { 29, "Um Overfitting zu reduzieren", true, 8 },
                    { 30, "Um die Trainingsgeschwindigkeit zu erhöhen", false, 8 },
                    { 31, "Um mehr Daten zu verarbeiten", false, 8 },
                    { 32, "Um die Genauigkeit auf Trainingsdaten zu steigern", false, 8 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnswerOptions_QuestionId",
                table: "AnswerOptions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_CourseId",
                table: "Chapters",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestions_QuestionId",
                table: "ExamQuestions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_CourseId",
                table: "Exams",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ChapterId",
                table: "Questions",
                column: "ChapterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerOptions");

            migrationBuilder.DropTable(
                name: "ExamQuestions");

            migrationBuilder.DropTable(
                name: "Exams");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Chapters");

            migrationBuilder.DropTable(
                name: "Courses");
        }
    }
}
