TRUNCATE TABLE "transaction", "import_settings", "nomenclature", "category", "employee", "organization" RESTART IDENTITY CASCADE;

INSERT INTO "organization" ("name", "inn", "address") VALUES
('ООО "Ромашка"', '7701234567', 'г. Москва, ул. Ленина, д. 10'),
('ЗАО "Торговый Дом"', '7809876543', 'г. Санкт-Петербург, Невский пр., д. 25');

INSERT INTO "employee" ("organization_id", "name", "phone") VALUES
(1, 'Иванов Иван Иванович', '+79001112233'), 
(1, 'Петрова Мария Сергеевна', '+79004445566'),
(2, 'Сидоров Алексей', '+79004445567');

INSERT INTO "import_settings" ("id", "organization_id", "source_type", "description", "start_position", "batch_size") VALUES
('a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11', 1, 1, 'Основной импорт (Excel)', 0, 100),
('b5f2c123-1d4a-4b99-cc88-1aa2bd380b22', 2, 4, 'Интеграция с 1С', 1500, 500);

INSERT INTO "category" ("name", "parent_id") VALUES
('Продукты', NULL),
('Бытовая химия', NULL),
('Молочные изделия', 1),
('Фрукты и Овощи', 1),  
('Стиральные порошки', 2);

INSERT INTO "nomenclature" ("category_id", "name", "price", "unit_of_measure") VALUES
(3, 'Молоко "Домик в деревне" 3.2%', 89.90, 'л'),
(3, 'Сыр "Российский"', 650.00, 'кг'),    
(4, 'Яблоки "Гренни Смит"', 120.50, 'кг'),
(4, 'Бананы Эквадор', 80.00, 'кг'),       
(5, 'Порошок "Ariel" 3кг', 450.00, 'шт');

INSERT INTO "transaction" ("nomenclature_id", "employee_id", "operation_date", "created_at", "quantity", "amount", "operation_type") VALUES
(1, 1, NOW(), NOW(), 2, 179.80, 101),
(2, 1, NOW(), NOW(), 0.5, 325.00, 101),
(5, 2, NOW(), NOW(), 1, 450.00, 101),
(3, 3, NOW(), NOW(), 1.5, 180.75, 101);