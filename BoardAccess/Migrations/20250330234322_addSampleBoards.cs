using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardAccess.Migrations
{
    /// <inheritdoc />
    public partial class addSampleBoards : Migration
    {
        private const string BlinkerId = "ec5bc9c0-ff70-4831-9c63-5c0f506021ea";
        private const string TubId = "e0c29b6e-8352-4da4-891a-c4d2e192a763";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Boards",
                columns: new[] { "Id", "Name", "InitialState" },
                values: new object[,]
                {
                    { Guid.Parse(BlinkerId),"Blinker", "[" +
                    "[false,false,false,false,false]," +
                    "[false,false,true ,false,false]," +
                    "[false,false,true ,false,false]," +
                    "[false,false,true ,false,false]," +
                    "[false,false,false,false,false]"  +
                    "]" }
                });
            migrationBuilder.InsertData(
                table: "Boards",
                columns: new[] { "Id", "Name", "InitialState" },
                values: new object[,]
                {
                    { Guid.Parse(TubId), "Tub", "[" +
                    "[false,false,false,false,false]," +
                    "[false,false,true ,false,false]," +
                    "[false,true ,false,true ,false]," +
                    "[false,false,true ,false,false]," +
                    "[false,false,false,false,false]"  +
                    "]" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Boards",
                keyColumn: "Id",
                keyValues: new object[] {
                    Guid.Parse(BlinkerId),Guid.Parse(TubId)
                });
        }
    }
}
