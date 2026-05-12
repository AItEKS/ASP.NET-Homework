-- Скрипт миграции
-- 2026-05-12
-- Инициализация тестовых данных для филиалов с настройками загрузки

-- Проверяем наличие компании
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM companies WHERE id = '14e54725-0efc-42b8-a27d-a84f9a7257c5') THEN
        INSERT INTO companies(id, name, inn, address, load_options)
        VALUES('14e54725-0efc-42b8-a27d-a84f9a7257c5', 'ООО "Ромашка"', '7701234567', 'Россия,Москва,Москва,Тверская,12,1', '{}');
    ELSE
        UPDATE companies
        SET name = 'ООО "Ромашка"',
            inn = '7701234567',
            address = 'Россия,Москва,Москва,Тверская,12,1'
        WHERE id = '14e54725-0efc-42b8-a27d-a84f9a7257c5';
    END IF;
END $$;

-- Удаляем старые тестовые филиалы если они есть
DELETE FROM branches WHERE company_id = '14e54725-0efc-42b8-a27d-a84f9a7257c5';

-- Добавляем тестовые филиалы с настройками загрузки
INSERT INTO branches(id, company_id, name, load_options)
VALUES
    ('a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d',
     '14e54725-0efc-42b8-a27d-a84f9a7257c5',
     'Филиал "Центральный"',
     '{"Description": "Настройки загрузки для Филиал \"Центральный\"", "StartPosition": 0, "BatchSize": 1000}'::jsonb),

    ('b2c3d4e5-f6a7-4b5c-9d0e-1f2a3b4c5d6e',
     '14e54725-0efc-42b8-a27d-a84f9a7257c5',
     'Филиал "Северный"',
     '{"Description": "Настройки загрузки для Филиал \"Северный\"", "StartPosition": 5000, "BatchSize": 500}'::jsonb),

    ('c3d4e5f6-a7b8-4c5d-0e1f-2a3b4c5d6e7f',
     '14e54725-0efc-42b8-a27d-a84f9a7257c5',
     'Филиал "Южный"',
     '{"Description": "Настройки загрузки для Филиал \"Южный\"", "StartPosition": 12000, "BatchSize": 2000}'::jsonb);
