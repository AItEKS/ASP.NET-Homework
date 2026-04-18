CREATE TABLE IF NOT EXISTS "journal_rows" (
    "code" bigint NOT NULL PRIMARY KEY,
    
    "type_code" bigint NOT NULL,
    "receipt_number" bigint NOT NULL,
    
    "product_code" bigint,
    "category_code" bigint,
    "emploee_code" bigint,
    
    "emploee_name" varchar(255),
    "category_name" varchar(255),
    "nomenclature_name" varchar(500),
    
    "period" timestamp without time zone NOT NULL,
    
    "quantity" double precision NOT NULL,
    "price" double precision NOT NULL,
    "discount" double precision NOT NULL,
    
    "uploaded_at" timestamp without time zone NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS "ix_journal_rows_period" ON "journal_rows"("period");
CREATE INDEX IF NOT EXISTS "ix_journal_rows_type_code" ON "journal_rows"("type_code");