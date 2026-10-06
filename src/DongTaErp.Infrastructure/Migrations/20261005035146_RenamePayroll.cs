using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DongTaErp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenamePayroll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payroll_Employees_EmployeeId",
                table: "Payroll");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payroll",
                table: "Payroll");

            migrationBuilder.RenameTable(
                name: "Payroll",
                newName: "Payrolls");

            migrationBuilder.RenameIndex(
                name: "IX_Payroll_PeriodStartDate",
                table: "Payrolls",
                newName: "IX_Payrolls_PeriodStartDate");

            migrationBuilder.RenameIndex(
                name: "IX_Payroll_PeriodEndDate",
                table: "Payrolls",
                newName: "IX_Payrolls_PeriodEndDate");

            migrationBuilder.RenameIndex(
                name: "IX_Payroll_PaymentStatus",
                table: "Payrolls",
                newName: "IX_Payrolls_PaymentStatus");

            migrationBuilder.RenameIndex(
                name: "IX_Payroll_EmployeeId_PeriodStartDate_PeriodEndDate",
                table: "Payrolls",
                newName: "IX_Payrolls_EmployeeId_PeriodStartDate_PeriodEndDate");

            migrationBuilder.RenameIndex(
                name: "IX_Payroll_EmployeeId",
                table: "Payrolls",
                newName: "IX_Payrolls_EmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payrolls",
                table: "Payrolls",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payrolls_Employees_EmployeeId",
                table: "Payrolls",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payrolls_Employees_EmployeeId",
                table: "Payrolls");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payrolls",
                table: "Payrolls");

            migrationBuilder.RenameTable(
                name: "Payrolls",
                newName: "Payroll");

            migrationBuilder.RenameIndex(
                name: "IX_Payrolls_PeriodStartDate",
                table: "Payroll",
                newName: "IX_Payroll_PeriodStartDate");

            migrationBuilder.RenameIndex(
                name: "IX_Payrolls_PeriodEndDate",
                table: "Payroll",
                newName: "IX_Payroll_PeriodEndDate");

            migrationBuilder.RenameIndex(
                name: "IX_Payrolls_PaymentStatus",
                table: "Payroll",
                newName: "IX_Payroll_PaymentStatus");

            migrationBuilder.RenameIndex(
                name: "IX_Payrolls_EmployeeId_PeriodStartDate_PeriodEndDate",
                table: "Payroll",
                newName: "IX_Payroll_EmployeeId_PeriodStartDate_PeriodEndDate");

            migrationBuilder.RenameIndex(
                name: "IX_Payrolls_EmployeeId",
                table: "Payroll",
                newName: "IX_Payroll_EmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payroll",
                table: "Payroll",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payroll_Employees_EmployeeId",
                table: "Payroll",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
