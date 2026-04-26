DO $$ 
DECLARE 
    company_count INT;
    branch_count INT;
    trans_count INT;
BEGIN
    CREATE TABLE IF NOT EXISTS "branches" (
        "id" uuid NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
        "company_id" uuid NOT NULL,
        "name" varchar(255) NOT NULL,
        "load_options" jsonb,
        CONSTRAINT "fk_branches_company" FOREIGN KEY ("company_id") REFERENCES "companies"("id")
    );

    INSERT INTO "branches" ("company_id", "name", "load_options")
    SELECT "id", "name" || ' (Центральный)', "load_options"
    FROM "companies"
    WHERE "id" NOT IN (SELECT "company_id" FROM "branches");

    ALTER TABLE "transactions" ADD COLUMN IF NOT EXISTS "branch_id" uuid;

    UPDATE "transactions" t
    SET "branch_id" = b.id
    FROM "branches" b
    WHERE t.company_id = b.company_id AND t.branch_id IS NULL;

    ALTER TABLE "companies" DROP COLUMN IF EXISTS "load_options";
END $$;