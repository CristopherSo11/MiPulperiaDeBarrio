-- MI PULPERIA DE BARRIO
-- Vista de existencias por sucursal y producto

BEGIN;

SET LOCAL search_path TO user_12;

CREATE OR REPLACE VIEW mp_vw_existencias AS
SELECT
    i.id_inventario,
    s.id_sucursal,
    s.nombre AS sucursal,
    p.id_producto,
    p.codigo AS codigo_producto,
    p.nombre AS producto,
    p.precio,
    p.existencia_minima,
    COALESCE(ing.total_ingresado, 0) AS total_ingresado,
    COALESCE(res.total_reservado, 0) AS total_reservado,
    COALESCE(res.total_comprado, 0) AS total_comprado,
    COALESCE(ing.total_ingresado, 0)
        - COALESCE(res.total_comprado, 0)
        - COALESCE(res.total_reservado, 0)
        AS disponible
FROM mp_inventarios i
JOIN mp_sucursales s
    ON s.id_sucursal = i.id_sucursal
JOIN mp_productos p
    ON p.id_producto = i.id_producto
LEFT JOIN (
    SELECT
        id_inventario,
        SUM(cantidad) AS total_ingresado
    FROM mp_ingresos
    GROUP BY id_inventario
) ing
    ON ing.id_inventario = i.id_inventario
LEFT JOIN (
    SELECT
        id_inventario,
        SUM(cantidad) FILTER (
            WHERE estado = 'RESERVADO'
        ) AS total_reservado,
        SUM(cantidad) FILTER (
            WHERE estado = 'COMPRADO'
        ) AS total_comprado
    FROM mp_reservas
    GROUP BY id_inventario
) res
    ON res.id_inventario = i.id_inventario;

COMMIT;
