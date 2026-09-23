using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SalesDashboard.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Company = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Segment = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Managers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Team = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Position = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    AvatarColor = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Managers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    BasePrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    BaseCost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ManagerId = table.Column<int>(type: "integer", nullable: false),
                    CustomerId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sales_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sales_Managers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Managers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaleItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SaleId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    UnitCost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleItems_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Ноутбуки" },
                    { 2, "Смартфоны" },
                    { 3, "Аксессуары" },
                    { 4, "Периферия" },
                    { 5, "Мониторы" },
                    { 6, "Сетевое оборудование" }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Company", "Name", "Segment" },
                values: new object[,]
                {
                    { 1, "АО Вектор #1", "Контакт 1", "Mid-Market" },
                    { 2, "ИП Сидоров #2", "Контакт 2", "Enterprise" },
                    { 3, "ООО ТехноМир #3", "Контакт 3", "Mid-Market" },
                    { 4, "АО Прогресс #4", "Контакт 4", "SMB" },
                    { 5, "ООО Альфа #5", "Контакт 5", "Mid-Market" },
                    { 6, "ООО Бета #6", "Контакт 6", "Enterprise" },
                    { 7, "ЗАО Гамма #7", "Контакт 7", "SMB" },
                    { 8, "ООО Дельта #8", "Контакт 8", "Enterprise" },
                    { 9, "АО Эпсилон #9", "Контакт 9", "Enterprise" },
                    { 10, "ООО Ромашка #10", "Контакт 10", "Enterprise" },
                    { 11, "АО Вектор #11", "Контакт 11", "Mid-Market" },
                    { 12, "ИП Сидоров #12", "Контакт 12", "Enterprise" },
                    { 13, "ООО ТехноМир #13", "Контакт 13", "Mid-Market" },
                    { 14, "АО Прогресс #14", "Контакт 14", "SMB" },
                    { 15, "ООО Альфа #15", "Контакт 15", "Enterprise" },
                    { 16, "ООО Бета #16", "Контакт 16", "Mid-Market" },
                    { 17, "ЗАО Гамма #17", "Контакт 17", "Enterprise" },
                    { 18, "ООО Дельта #18", "Контакт 18", "Mid-Market" },
                    { 19, "АО Эпсилон #19", "Контакт 19", "Enterprise" },
                    { 20, "ООО Ромашка #20", "Контакт 20", "SMB" },
                    { 21, "АО Вектор #21", "Контакт 21", "Enterprise" },
                    { 22, "ИП Сидоров #22", "Контакт 22", "Mid-Market" },
                    { 23, "ООО ТехноМир #23", "Контакт 23", "SMB" },
                    { 24, "АО Прогресс #24", "Контакт 24", "Mid-Market" },
                    { 25, "ООО Альфа #25", "Контакт 25", "Mid-Market" },
                    { 26, "ООО Бета #26", "Контакт 26", "SMB" },
                    { 27, "ЗАО Гамма #27", "Контакт 27", "Enterprise" },
                    { 28, "ООО Дельта #28", "Контакт 28", "SMB" },
                    { 29, "АО Эпсилон #29", "Контакт 29", "Mid-Market" },
                    { 30, "ООО Ромашка #30", "Контакт 30", "Enterprise" },
                    { 31, "АО Вектор #31", "Контакт 31", "Enterprise" },
                    { 32, "ИП Сидоров #32", "Контакт 32", "SMB" },
                    { 33, "ООО ТехноМир #33", "Контакт 33", "SMB" },
                    { 34, "АО Прогресс #34", "Контакт 34", "SMB" },
                    { 35, "ООО Альфа #35", "Контакт 35", "SMB" },
                    { 36, "ООО Бета #36", "Контакт 36", "SMB" },
                    { 37, "ЗАО Гамма #37", "Контакт 37", "Enterprise" },
                    { 38, "ООО Дельта #38", "Контакт 38", "SMB" },
                    { 39, "АО Эпсилон #39", "Контакт 39", "Mid-Market" },
                    { 40, "ООО Ромашка #40", "Контакт 40", "Enterprise" },
                    { 41, "АО Вектор #41", "Контакт 41", "Enterprise" },
                    { 42, "ИП Сидоров #42", "Контакт 42", "Mid-Market" },
                    { 43, "ООО ТехноМир #43", "Контакт 43", "SMB" },
                    { 44, "АО Прогресс #44", "Контакт 44", "SMB" },
                    { 45, "ООО Альфа #45", "Контакт 45", "Mid-Market" },
                    { 46, "ООО Бета #46", "Контакт 46", "Enterprise" },
                    { 47, "ЗАО Гамма #47", "Контакт 47", "Enterprise" },
                    { 48, "ООО Дельта #48", "Контакт 48", "Mid-Market" },
                    { 49, "АО Эпсилон #49", "Контакт 49", "Enterprise" },
                    { 50, "ООО Ромашка #50", "Контакт 50", "SMB" },
                    { 51, "АО Вектор #51", "Контакт 51", "SMB" },
                    { 52, "ИП Сидоров #52", "Контакт 52", "Enterprise" },
                    { 53, "ООО ТехноМир #53", "Контакт 53", "Enterprise" },
                    { 54, "АО Прогресс #54", "Контакт 54", "Enterprise" },
                    { 55, "ООО Альфа #55", "Контакт 55", "Mid-Market" },
                    { 56, "ООО Бета #56", "Контакт 56", "Enterprise" },
                    { 57, "ЗАО Гамма #57", "Контакт 57", "Enterprise" },
                    { 58, "ООО Дельта #58", "Контакт 58", "SMB" },
                    { 59, "АО Эпсилон #59", "Контакт 59", "Mid-Market" },
                    { 60, "ООО Ромашка #60", "Контакт 60", "Mid-Market" },
                    { 61, "АО Вектор #61", "Контакт 61", "Enterprise" },
                    { 62, "ИП Сидоров #62", "Контакт 62", "SMB" },
                    { 63, "ООО ТехноМир #63", "Контакт 63", "Mid-Market" },
                    { 64, "АО Прогресс #64", "Контакт 64", "SMB" },
                    { 65, "ООО Альфа #65", "Контакт 65", "SMB" },
                    { 66, "ООО Бета #66", "Контакт 66", "SMB" },
                    { 67, "ЗАО Гамма #67", "Контакт 67", "Enterprise" },
                    { 68, "ООО Дельта #68", "Контакт 68", "SMB" },
                    { 69, "АО Эпсилон #69", "Контакт 69", "SMB" },
                    { 70, "ООО Ромашка #70", "Контакт 70", "Enterprise" },
                    { 71, "АО Вектор #71", "Контакт 71", "Mid-Market" },
                    { 72, "ИП Сидоров #72", "Контакт 72", "Mid-Market" },
                    { 73, "ООО ТехноМир #73", "Контакт 73", "Enterprise" },
                    { 74, "АО Прогресс #74", "Контакт 74", "Enterprise" },
                    { 75, "ООО Альфа #75", "Контакт 75", "SMB" },
                    { 76, "ООО Бета #76", "Контакт 76", "SMB" },
                    { 77, "ЗАО Гамма #77", "Контакт 77", "Mid-Market" },
                    { 78, "ООО Дельта #78", "Контакт 78", "SMB" },
                    { 79, "АО Эпсилон #79", "Контакт 79", "Enterprise" },
                    { 80, "ООО Ромашка #80", "Контакт 80", "Mid-Market" }
                });

            migrationBuilder.InsertData(
                table: "Managers",
                columns: new[] { "Id", "AvatarColor", "FullName", "IsActive", "Position", "Team" },
                values: new object[,]
                {
                    { 1, "#6366f1", "Иван Петров", false, "Junior Sales", "Alpha" },
                    { 2, "#0ea5e9", "Мария Смирнова", true, "Junior Sales", "Bravo" },
                    { 3, "#10b981", "Алексей Кузнецов", true, "Sales Manager", "Charlie" },
                    { 4, "#f59e0b", "Ольга Попова", true, "Team Lead", "Delta" },
                    { 5, "#ef4444", "Дмитрий Соколов", true, "Senior Sales", "Alpha" },
                    { 6, "#8b5cf6", "Екатерина Лебедева", true, "Sales Manager", "Bravo" },
                    { 7, "#6366f1", "Сергей Новиков", true, "Sales Manager", "Charlie" },
                    { 8, "#0ea5e9", "Анна Морозова", true, "Team Lead", "Delta" },
                    { 9, "#10b981", "Павел Волков", true, "Junior Sales", "Alpha" },
                    { 10, "#f59e0b", "Юлия Зайцева", true, "Senior Sales", "Bravo" },
                    { 11, "#ef4444", "Артём Егоров", false, "Junior Sales", "Charlie" },
                    { 12, "#8b5cf6", "Наталья Павлова", true, "Junior Sales", "Delta" },
                    { 13, "#6366f1", "Роман Козлов", true, "Sales Manager", "Alpha" },
                    { 14, "#0ea5e9", "Виктория Степанова", true, "Team Lead", "Bravo" },
                    { 15, "#10b981", "Михаил Николаев", true, "Senior Sales", "Charlie" },
                    { 16, "#f59e0b", "Татьяна Орлова", true, "Senior Sales", "Delta" },
                    { 17, "#ef4444", "Игорь Фёдоров", true, "Sales Manager", "Alpha" },
                    { 18, "#8b5cf6", "Светлана Михайлова", true, "Senior Sales", "Bravo" },
                    { 19, "#6366f1", "Никита Беляев", true, "Senior Sales", "Charlie" },
                    { 20, "#0ea5e9", "Елена Виноградова", true, "Senior Sales", "Delta" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BaseCost", "BasePrice", "CategoryId", "Name" },
                values: new object[,]
                {
                    { 1, 584.49m, 750.80m, 1, "Ноутбуки модель 1" },
                    { 2, 150.41m, 227.67m, 1, "Ноутбуки модель 2" },
                    { 3, 184.75m, 250.81m, 1, "Ноутбуки модель 3" },
                    { 4, 629.53m, 949.18m, 1, "Ноутбуки модель 4" },
                    { 5, 188.92m, 312.99m, 1, "Ноутбуки модель 5" },
                    { 6, 237.67m, 321.90m, 1, "Ноутбуки модель 6" },
                    { 7, 454.48m, 632.70m, 1, "Ноутбуки модель 7" },
                    { 8, 354.78m, 481.13m, 1, "Ноутбуки модель 8" },
                    { 9, 463.96m, 566.58m, 1, "Ноутбуки модель 9" },
                    { 10, 701.31m, 1084.45m, 1, "Ноутбуки модель 10" },
                    { 11, 367.98m, 475.18m, 2, "Смартфоны модель 1" },
                    { 12, 122.09m, 198.16m, 2, "Смартфоны модель 2" },
                    { 13, 702.95m, 1071.28m, 2, "Смартфоны модель 3" },
                    { 14, 85.32m, 138.79m, 2, "Смартфоны модель 4" },
                    { 15, 168.21m, 293.31m, 2, "Смартфоны модель 5" },
                    { 16, 604.07m, 912.05m, 2, "Смартфоны модель 6" },
                    { 17, 199.10m, 243.88m, 2, "Смартфоны модель 7" },
                    { 18, 296.56m, 459.64m, 2, "Смартфоны модель 8" },
                    { 19, 706.85m, 849.90m, 2, "Смартфоны модель 9" },
                    { 20, 478.58m, 805.50m, 2, "Смартфоны модель 10" },
                    { 21, 648.69m, 928.11m, 3, "Аксессуары модель 1" },
                    { 22, 169.30m, 205.84m, 3, "Аксессуары модель 2" },
                    { 23, 90.81m, 151.56m, 3, "Аксессуары модель 3" },
                    { 24, 551.17m, 699.49m, 3, "Аксессуары модель 4" },
                    { 25, 249.97m, 415.81m, 3, "Аксессуары модель 5" },
                    { 26, 281.69m, 447.57m, 3, "Аксессуары модель 6" },
                    { 27, 164.92m, 202.31m, 3, "Аксессуары модель 7" },
                    { 28, 533.65m, 805.57m, 3, "Аксессуары модель 8" },
                    { 29, 90.64m, 131.63m, 3, "Аксессуары модель 9" },
                    { 30, 615.27m, 970.33m, 3, "Аксессуары модель 10" },
                    { 31, 224.76m, 271.50m, 4, "Периферия модель 1" },
                    { 32, 342.13m, 465.46m, 4, "Периферия модель 2" },
                    { 33, 104.93m, 171.13m, 4, "Периферия модель 3" },
                    { 34, 106.75m, 157.83m, 4, "Периферия модель 4" },
                    { 35, 59.63m, 100.10m, 4, "Периферия модель 5" },
                    { 36, 401.34m, 605.26m, 4, "Периферия модель 6" },
                    { 37, 449.68m, 542.03m, 4, "Периферия модель 7" },
                    { 38, 713.07m, 861.20m, 4, "Периферия модель 8" },
                    { 39, 52.46m, 64.96m, 4, "Периферия модель 9" },
                    { 40, 573.31m, 699.85m, 4, "Периферия модель 10" },
                    { 41, 780.81m, 1372.18m, 5, "Мониторы модель 1" },
                    { 42, 418.96m, 597.10m, 5, "Мониторы модель 2" },
                    { 43, 543.28m, 666.42m, 5, "Мониторы модель 3" },
                    { 44, 348.21m, 447.49m, 5, "Мониторы модель 4" },
                    { 45, 399.56m, 669.41m, 5, "Мониторы модель 5" },
                    { 46, 475.89m, 685.48m, 5, "Мониторы модель 6" },
                    { 47, 287.90m, 502.20m, 5, "Мониторы модель 7" },
                    { 48, 519.39m, 852.35m, 5, "Мониторы модель 8" },
                    { 49, 65.53m, 113.30m, 5, "Мониторы модель 9" },
                    { 50, 816.14m, 1142.35m, 5, "Мониторы модель 10" },
                    { 51, 618.56m, 802.53m, 6, "Сетевое оборудование модель 1" },
                    { 52, 132.51m, 194.96m, 6, "Сетевое оборудование модель 2" },
                    { 53, 316.60m, 527.35m, 6, "Сетевое оборудование модель 3" },
                    { 54, 168.84m, 216.25m, 6, "Сетевое оборудование модель 4" },
                    { 55, 222.53m, 346.03m, 6, "Сетевое оборудование модель 5" },
                    { 56, 460.27m, 824.90m, 6, "Сетевое оборудование модель 6" },
                    { 57, 662.98m, 1062.91m, 6, "Сетевое оборудование модель 7" },
                    { 58, 621.96m, 854.38m, 6, "Сетевое оборудование модель 8" },
                    { 59, 491.65m, 881.97m, 6, "Сетевое оборудование модель 9" },
                    { 60, 570.67m, 693.10m, 6, "Сетевое оборудование модель 10" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_ProductId",
                table: "SaleItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_SaleId",
                table: "SaleItems",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_CustomerId",
                table: "Sales",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_Date",
                table: "Sales",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_ManagerId",
                table: "Sales",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_Status_Date",
                table: "Sales",
                columns: new[] { "Status", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleItems");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Managers");
        }
    }
}
