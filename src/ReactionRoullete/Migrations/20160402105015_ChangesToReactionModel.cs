using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace ReactionRoullete.Migrations
{
    public partial class ChangesToReactionModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId", table: "AspNetRoleClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId", table: "AspNetUserClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId", table: "AspNetUserLogins");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_IdentityRole_RoleId", table: "AspNetUserRoles");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_ApplicationUser_UserId", table: "AspNetUserRoles");
            migrationBuilder.DropForeignKey(name: "FK_Reaction_YoutubeVideoDescription_YoutubeVideoDescriptionID", table: "Reaction");
            migrationBuilder.DropColumn(name: "Date", table: "Reaction");
            migrationBuilder.AlterColumn<double>(
                name: "Surprise",
                table: "Reaction",
                nullable: true);
            migrationBuilder.AlterColumn<double>(
                name: "Sadness",
                table: "Reaction",
                nullable: true);
            migrationBuilder.AlterColumn<double>(
                name: "Neutral",
                table: "Reaction",
                nullable: true);
            migrationBuilder.AlterColumn<double>(
                name: "Happiness",
                table: "Reaction",
                nullable: true);
            migrationBuilder.AlterColumn<double>(
                name: "Fear",
                table: "Reaction",
                nullable: true);
            migrationBuilder.AlterColumn<double>(
                name: "Disgust",
                table: "Reaction",
                nullable: true);
            migrationBuilder.AlterColumn<double>(
                name: "Contempt",
                table: "Reaction",
                nullable: true);
            migrationBuilder.AlterColumn<double>(
                name: "Anger",
                table: "Reaction",
                nullable: true);
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateCreated",
                table: "Reaction",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateProcessed",
                table: "Reaction",
                nullable: true);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_IdentityRole_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_ApplicationUser_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_Reaction_YoutubeVideoDescription_YoutubeVideoDescriptionID",
                table: "Reaction",
                column: "YoutubeVideoDescriptionID",
                principalTable: "YoutubeVideoDescription",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId", table: "AspNetRoleClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId", table: "AspNetUserClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId", table: "AspNetUserLogins");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_IdentityRole_RoleId", table: "AspNetUserRoles");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_ApplicationUser_UserId", table: "AspNetUserRoles");
            migrationBuilder.DropForeignKey(name: "FK_Reaction_YoutubeVideoDescription_YoutubeVideoDescriptionID", table: "Reaction");
            migrationBuilder.DropColumn(name: "DateCreated", table: "Reaction");
            migrationBuilder.DropColumn(name: "DateProcessed", table: "Reaction");
            migrationBuilder.AlterColumn<double>(
                name: "Surprise",
                table: "Reaction",
                nullable: false);
            migrationBuilder.AlterColumn<double>(
                name: "Sadness",
                table: "Reaction",
                nullable: false);
            migrationBuilder.AlterColumn<double>(
                name: "Neutral",
                table: "Reaction",
                nullable: false);
            migrationBuilder.AlterColumn<double>(
                name: "Happiness",
                table: "Reaction",
                nullable: false);
            migrationBuilder.AlterColumn<double>(
                name: "Fear",
                table: "Reaction",
                nullable: false);
            migrationBuilder.AlterColumn<double>(
                name: "Disgust",
                table: "Reaction",
                nullable: false);
            migrationBuilder.AlterColumn<double>(
                name: "Contempt",
                table: "Reaction",
                nullable: false);
            migrationBuilder.AlterColumn<double>(
                name: "Anger",
                table: "Reaction",
                nullable: false);
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Date",
                table: "Reaction",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_IdentityRole_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_ApplicationUser_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Reaction_YoutubeVideoDescription_YoutubeVideoDescriptionID",
                table: "Reaction",
                column: "YoutubeVideoDescriptionID",
                principalTable: "YoutubeVideoDescription",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
