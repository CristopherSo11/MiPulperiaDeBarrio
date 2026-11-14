-- MI PULPERIA DE BARRIO
-- Triggers de seguridad - Primer entregable

BEGIN;

SET LOCAL search_path TO user_12;

-- Funcion que impide modificar o eliminar ingresos.
CREATE OR REPLACE FUNCTION mp_bloquear_cambios_ingresos()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
BEGIN
    RAISE EXCEPTION
        'Operacion no permitida: los ingresos de inventario son inmutables.';
END;
$$;

-- Bloquear UPDATE y DELETE.
CREATE TRIGGER trg_mp_ingresos_inmutables
BEFORE UPDATE OR DELETE ON mp_ingresos
FOR EACH ROW
EXECUTE FUNCTION mp_bloquear_cambios_ingresos();

-- Bloquear TRUNCATE.
CREATE TRIGGER trg_mp_ingresos_no_truncate
BEFORE TRUNCATE ON mp_ingresos
FOR EACH STATEMENT
EXECUTE FUNCTION mp_bloquear_cambios_ingresos();

COMMIT;


-- PRUEBA: bloquear UPDATE y DELETE
-- Los cambios de esta prueba no se guardan.

BEGIN;

SET LOCAL search_path TO user_12;

DO $$
DECLARE
    v_ingreso_id INTEGER;
BEGIN
    SELECT id_ingreso
    INTO v_ingreso_id
    FROM mp_ingresos
    LIMIT 1;

    -- Probar UPDATE
    BEGIN
        UPDATE mp_ingresos
        SET cantidad = 999
        WHERE id_ingreso = v_ingreso_id;

        RAISE NOTICE 'PRUEBA FALLIDA: UPDATE permitido';

    EXCEPTION WHEN raise_exception THEN
        RAISE NOTICE 'PRUEBA EXITOSA: UPDATE bloqueado';
    END;

    -- Probar DELETE
    BEGIN
        DELETE FROM mp_ingresos
        WHERE id_ingreso = v_ingreso_id;

        RAISE NOTICE 'PRUEBA FALLIDA: DELETE permitido';

    EXCEPTION WHEN raise_exception THEN
        RAISE NOTICE 'PRUEBA EXITOSA: DELETE bloqueado';
    END;
END;
$$;

ROLLBACK;


-- PROTECCION DE RESERVAS

BEGIN;

SET LOCAL search_path TO user_12;

CREATE OR REPLACE FUNCTION mp_proteger_reservas()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
BEGIN
    IF TG_OP = 'DELETE' THEN
        RAISE EXCEPTION
            'No se permite eliminar reservas.';
    END IF;

    IF TG_OP = 'UPDATE' THEN

        IF OLD.estado <> 'RESERVADO'
           OR NEW.estado NOT IN ('COMPRADO', 'CANCELADO')
           OR NEW.id_reserva IS DISTINCT FROM OLD.id_reserva
           OR NEW.id_inventario IS DISTINCT FROM OLD.id_inventario
           OR NEW.id_cliente IS DISTINCT FROM OLD.id_cliente
           OR NEW.cantidad IS DISTINCT FROM OLD.cantidad
           OR NEW.precio_unitario IS DISTINCT FROM OLD.precio_unitario
           OR NEW.fecha_reserva IS DISTINCT FROM OLD.fecha_reserva
           OR NEW.fecha_vencimiento IS DISTINCT FROM OLD.fecha_vencimiento
        THEN
            RAISE EXCEPTION
                'Modificacion de reserva no permitida.';
        END IF;

    END IF;

    RETURN NEW;
END;
$$;

CREATE TRIGGER trg_mp_proteger_reservas
BEFORE UPDATE OR DELETE ON mp_reservas
FOR EACH ROW
EXECUTE FUNCTION mp_proteger_reservas();

COMMIT;
