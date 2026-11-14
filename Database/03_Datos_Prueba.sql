-- MI PULPERIA DE BARRIO
-- Datos de prueba - Primer entregable

BEGIN;

SET LOCAL search_path TO user_12;

-- Sucursales
INSERT INTO mp_sucursales (nombre, direccion, telefono)
VALUES
('Pulperia Cartago', 'Centro de Cartago', '2550-1001'),
('Pulperia San Jose', 'Centro de San Jose', '2220-1002'),
('Pulperia Heredia', 'Centro de Heredia', '2260-1003');

-- Clientes
INSERT INTO mp_clientes (identificacion, nombre, correo, telefono)
VALUES
('DEMO-001', 'Daniel Mora', 'daniel.demo@example.com', '8888-1001'),
('DEMO-002', 'Sofia Vargas', 'sofia.demo@example.com', '8888-1002'),
('DEMO-003', 'Andres Rojas', 'andres.demo@example.com', '8888-1003');

-- Productos con atributos JSON diferentes
INSERT INTO mp_productos
(codigo, nombre, descripcion, precio, atributos,
 maximo_reserva, horas_reserva, existencia_minima)
VALUES
(
    'ARR-001',
    'Arroz 1 kg',
    'Arroz blanco',
    1250,
    '{"marca":"Del Barrio","peso":"1 kg","tipo":"blanco"}',
    5, 24, 3
),
(
    'CAF-001',
    'Cafe molido 500 g',
    'Cafe tostado y molido',
    3500,
    '{"origen":"Costa Rica","peso":"500 g","tueste":"medio"}',
    3, 48, 2
),
(
    'ZAP-001',
    'Zapatos deportivos',
    'Zapatos deportivos para adulto',
    25900,
    '{"color":"negro","talla":42,"material":"sintetico"}',
    2, 12, 1
);

COMMIT;


-- Inventarios por sucursal
-- Ejecutar este bloque una sola vez.

BEGIN;

SET LOCAL search_path TO user_12;

INSERT INTO mp_inventarios (id_sucursal, id_producto)
SELECT s.id_sucursal, p.id_producto
FROM mp_sucursales s
CROSS JOIN mp_productos p
WHERE s.nombre IN (
    'Pulperia Cartago',
    'Pulperia San Jose',
    'Pulperia Heredia'
)
AND p.codigo IN (
    'ARR-001',
    'CAF-001',
    'ZAP-001'
)
ON CONFLICT (id_sucursal, id_producto) DO NOTHING;

COMMIT;
