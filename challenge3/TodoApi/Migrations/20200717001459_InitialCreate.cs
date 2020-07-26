using Microsoft.EntityFrameworkCore.Migrations;

namespace TodoApi.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TodoItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    IsComplete = table.Column<bool>(nullable: false),
                    Version = table.Column<byte[]>(nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoItems", x => x.Id);
                });

            migrationBuilder.Sql(
                @"
                CREATE TRIGGER SetTodoItemVersionOnUpdate
                AFTER UPDATE ON TodoItems
                BEGIN
                    UPDATE TodoItems
                    SET Version = randomblob(8)
                    WHERE rowid = NEW.rowid;
                END
            ");
            
            migrationBuilder.Sql(
                @"
                CREATE TRIGGER SetTodoItemVersionOnInsert
                AFTER INSERT ON TodoItems
                BEGIN
                    UPDATE TodoItems
                    SET Version = randomblob(8)
                    WHERE rowid = NEW.rowid;
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TodoItems");

            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS SetTodoItemVersionOnUpdate");

            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS SetTodoItemVersionOnInsert");
        }
    }
}
