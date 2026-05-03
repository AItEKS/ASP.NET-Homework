-- Добавляем колонки для хранения внешних кодов
ALTER TABLE "categories" ADD COLUMN IF NOT EXISTS "code" bigint;
ALTER TABLE "nomenclatures" ADD COLUMN IF NOT EXISTS "code" bigint;
ALTER TABLE "emploees" ADD COLUMN IF NOT EXISTS "code" bigint;

CREATE INDEX IF NOT EXISTS "ix_categories_code" ON "categories"("code");
CREATE INDEX IF NOT EXISTS "ix_nomenclature_code" ON "nomenclatures"("code");
CREATE INDEX IF NOT EXISTS "ix_emploees_code" ON "emploees"("code");