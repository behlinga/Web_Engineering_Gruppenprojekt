using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Web_Engineering_Gruppenprojekt.Migrations
{
    /// <inheritdoc />
    public partial class SeedDemoExam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Exams",
                columns: new[] { "Id", "CourseId", "Date" },
                values: new object[] { 1, 1, new DateTime(2026, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "ExamQuestions",
                columns: new[] { "ExamId", "QuestionId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 3 },
                    { 1, 4 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ExamQuestions",
                keyColumns: new[] { "ExamId", "QuestionId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "ExamQuestions",
                keyColumns: new[] { "ExamId", "QuestionId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "ExamQuestions",
                keyColumns: new[] { "ExamId", "QuestionId" },
                keyValues: new object[] { 1, 4 });

            migrationBuilder.DeleteData(
                table: "Exams",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
