﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class CommentThreadSubscribers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string[]>(
                name: "Hashtags",
                table: "Blogposts",
                type: "text[]",
                maxLength: 10,
                nullable: false,
                defaultValue: new string[0],
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldMaxLength: 10,
                oldDefaultValue: new string[0]);

            migrationBuilder.CreateTable(
                name: "CommentsThreadOgmaUser",
                columns: table => new
                {
                    SubcribersId = table.Column<long>(type: "bigint", nullable: false),
                    SubscribedThreadsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentsThreadOgmaUser", x => new { x.SubcribersId, x.SubscribedThreadsId });
                    table.ForeignKey(
                        name: "FK_CommentsThreadOgmaUser_AspNetUsers_SubcribersId",
                        column: x => x.SubcribersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentsThreadOgmaUser_CommentThreads_SubscribedThreadsId",
                        column: x => x.SubscribedThreadsId,
                        principalTable: "CommentThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentsThreadOgmaUser_SubscribedThreadsId",
                table: "CommentsThreadOgmaUser",
                column: "SubscribedThreadsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentsThreadOgmaUser");

            migrationBuilder.AlterColumn<string[]>(
                name: "Hashtags",
                table: "Blogposts",
                type: "text[]",
                maxLength: 10,
                nullable: false,
                defaultValue: new string[0],
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldMaxLength: 10,
                oldDefaultValue: new string[0]);
        }
    }
}
