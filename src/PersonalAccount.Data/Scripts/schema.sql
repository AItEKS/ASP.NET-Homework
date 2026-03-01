CREATE TABLE IF NOT EXISTS "organization" (
	"id" bigserial NOT NULL UNIQUE,
	"name" varchar(255) NOT NULL,
	"inn" varchar(20) NOT NULL,
	"address" text NOT NULL,
	PRIMARY KEY ("id")
);

CREATE TABLE IF NOT EXISTS "users" (
	"id" bigserial NOT NULL UNIQUE,
	"organization_id" bigint NOT NULL,
	"login" varchar(100) NOT NULL UNIQUE,
	"password_hash" text NOT NULL,
	"role" varchar(50) NOT NULL DEFAULT 'User',
	PRIMARY KEY ("id")
);

CREATE TABLE IF NOT EXISTS "employee" (
	"id" bigserial NOT NULL UNIQUE,
	"organization_id" bigint NOT NULL,
	"name" varchar(255) NOT NULL,
	"phone" varchar(50),
	PRIMARY KEY ("id")
);

CREATE TABLE IF NOT EXISTS "category" (
	"id" bigserial NOT NULL UNIQUE,
	"parent_id" bigint,
	"name" varchar(255) NOT NULL,
	PRIMARY KEY ("id")
);

CREATE TABLE IF NOT EXISTS "nomenclature" (
	"id" bigserial NOT NULL UNIQUE,
	"category_id" bigint NOT NULL,
	"name" varchar(255) NOT NULL,
	"price" numeric(18, 2) NOT NULL,
	"unit_of_measure" varchar(50) NOT NULL,
	PRIMARY KEY ("id")
);

CREATE TABLE IF NOT EXISTS "transaction" (
	"id" bigserial NOT NULL UNIQUE,
	"nomenclature_id" bigint NOT NULL,
	"employee_id" bigint NOT NULL,
	
	"operation_date" timestamptz NOT NULL, 
	"created_at" timestamptz NOT NULL DEFAULT NOW(),
	
	"quantity" numeric(18, 3) NOT NULL,
	"amount" numeric(18, 2) NOT NULL,
	
	"operation_type" integer NOT NULL,
	PRIMARY KEY ("id")
);

CREATE TABLE IF NOT EXISTS "import_settings" (
	"id" uuid NOT NULL UNIQUE,
	"organization_id" bigint NOT NULL,
	"source_type" integer NOT NULL,
	"description" varchar(255) NOT NULL,
	"start_position" bigint NOT NULL DEFAULT 0,
	"batch_size" integer NOT NULL DEFAULT 100,
	PRIMARY KEY ("id")
);

ALTER TABLE "users" 
    ADD CONSTRAINT "users_fk_org" 
    FOREIGN KEY ("organization_id") REFERENCES "organization"("id");

ALTER TABLE "employee" 
    ADD CONSTRAINT "employee_fk_org" 
    FOREIGN KEY ("organization_id") REFERENCES "organization"("id");

ALTER TABLE "category" 
    ADD CONSTRAINT "category_fk_parent" 
    FOREIGN KEY ("parent_id") REFERENCES "category"("id");

ALTER TABLE "nomenclature" 
    ADD CONSTRAINT "nomenclature_fk_cat" 
    FOREIGN KEY ("category_id") REFERENCES "category"("id");

ALTER TABLE "transaction" 
    ADD CONSTRAINT "transaction_fk_nom" 
    FOREIGN KEY ("nomenclature_id") REFERENCES "nomenclature"("id");

ALTER TABLE "transaction" 
    ADD CONSTRAINT "transaction_fk_emp" 
    FOREIGN KEY ("employee_id") REFERENCES "employee"("id");

ALTER TABLE "import_settings" 
    ADD CONSTRAINT "import_settings_fk_org" 
    FOREIGN KEY ("organization_id") REFERENCES "organization"("id");