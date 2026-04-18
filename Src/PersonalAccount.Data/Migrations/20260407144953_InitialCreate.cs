using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PersonalAccount.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "companies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    inn = table.Column<string>(type: "text", nullable: true),
                    address = table.Column<string>(type: "text", nullable: true),
                    load_options = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("companies_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "journal_rows",
                columns: table => new
                {
                    code = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type_code = table.Column<long>(type: "bigint", nullable: false),
                    receipt_number = table.Column<long>(type: "bigint", nullable: false),
                    product_code = table.Column<long>(type: "bigint", nullable: true),
                    category_code = table.Column<long>(type: "bigint", nullable: true),
                    emploee_code = table.Column<long>(type: "bigint", nullable: true),
                    emploee_name = table.Column<string>(type: "text", nullable: true),
                    category_name = table.Column<string>(type: "text", nullable: true),
                    nomenclature_name = table.Column<string>(type: "text", nullable: true),
                    period = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    quantity = table.Column<double>(type: "double precision", nullable: false),
                    price = table.Column<double>(type: "double precision", nullable: false),
                    discount = table.Column<double>(type: "double precision", nullable: false),
                    uploaded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journal_rows", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "schemaversions",
                columns: table => new
                {
                    schemaversionsid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    scriptname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    applied = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schemaversions_Id", x => x.schemaversionsid);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "text", nullable: true),
                    password = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "text", nullable: true),
                    company_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("categories_pkey", x => x.id);
                    table.ForeignKey(
                        name: "categories_company_id_fk",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "emploees",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    company_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("emploees_pkey", x => x.id);
                    table.ForeignKey(
                        name: "emploees_company_id_fk",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "links_user_company",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    company_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("links_user_company_pkey", x => x.id);
                    table.ForeignKey(
                        name: "links_user_company_company_id_fk",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "links_user_company_user_id_fk",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "nomenclatures",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "text", nullable: true),
                    category_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("nomenclatures_pkey", x => x.id);
                    table.ForeignKey(
                        name: "nomenclatures_category_id_fk",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    transaction_type = table.Column<int>(type: "integer", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    change_period = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    nomenclature_id = table.Column<Guid>(type: "uuid", nullable: true),
                    emloee_id = table.Column<Guid>(type: "uuid", nullable: true),
                    price = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: true),
                    quantity = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: true),
                    discount = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("transactions_pkey", x => x.id);
                    table.ForeignKey(
                        name: "transactions_company_id_fk",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "transactions_emloee_id_fk",
                        column: x => x.emloee_id,
                        principalTable: "emploees",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "transactions_nomenclature_id_fk",
                        column: x => x.nomenclature_id,
                        principalTable: "nomenclatures",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_categories_company_id",
                table: "categories",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "company_inn_ix",
                table: "companies",
                column: "inn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_emploees_company_id",
                table: "emploees",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_links_user_company_company_id",
                table: "links_user_company",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_links_user_company_user_id",
                table: "links_user_company",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_nomenclatures_category_id",
                table: "nomenclatures",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_company_id",
                table: "transactions",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_emloee_id",
                table: "transactions",
                column: "emloee_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_nomenclature_id",
                table: "transactions",
                column: "nomenclature_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "journal_rows");

            migrationBuilder.DropTable(
                name: "links_user_company");

            migrationBuilder.DropTable(
                name: "schemaversions");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "emploees");

            migrationBuilder.DropTable(
                name: "nomenclatures");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "companies");
        }
    }
}
