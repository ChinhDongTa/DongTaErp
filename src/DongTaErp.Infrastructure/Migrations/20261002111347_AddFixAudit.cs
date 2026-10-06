using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DongTaErp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFixAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Created",
                table: "GoodsIssues",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "GoodsIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "GoodsIssues",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastModified",
                table: "GoodsIssues",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "GoodsIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Created",
                table: "GoodsIssueLines",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "GoodsIssueLines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "GoodsIssueLines",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastModified",
                table: "GoodsIssueLines",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "GoodsIssueLines",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "GoodsIssues");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "GoodsIssues");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "GoodsIssues");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "GoodsIssues");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "GoodsIssues");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "GoodsIssueLines");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "GoodsIssueLines");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "GoodsIssueLines");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "GoodsIssueLines");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "GoodsIssueLines");
        }
    }
}
