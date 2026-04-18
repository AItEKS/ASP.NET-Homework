DO $$ 
DECLARE 
    company_count INT;
    branch_count INT;
BEGIN
    SELECT COUNT(*) INTO company_count FROM "companies";
    RAISE NOTICE 'ДО МИГРАЦИИ: Количество компаний = %', company_count;

    CREATE TABLE IF NOT EXISTS "branches" (
        "id" uuid NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
        "company_id" uuid NOT NULL,
        "name" varchar(255) NOT NULL,
        "load_options" jsonb,
        CONSTRAINT "fk_branches_company" FOREIGN KEY ("company_id") REFERENCES "companies"("id") ON DELETE CASCADE
    );

    INSERT INTO "branches" ("company_id", "name", "load_options")
    SELECT "id", "name" || ' - Главный филиал', "load_options"
    FROM "companies"
    WHERE "id" NOT IN (SELECT "company_id" FROM "branches");

    ALTER TABLE "companies" DROP COLUMN IF EXISTS "load_options";

    SELECT COUNT(*) INTO branch_count FROM "branches";
    RAISE NOTICE 'ПОСЛЕ МИГРАЦИИ: Количество созданных филиалов = %', branch_count;
END $$;