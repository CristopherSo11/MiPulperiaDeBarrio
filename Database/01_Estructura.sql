-- MI PULPERIA DE BARRIO
-- Primer entregable - Estructura de base de datos
BEGIN;
SET LOCAL search_path TO user_12;

-- 1. SUCURSALES
CREATE TABLE mp_sucursales (
    id_sucursal INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL UNIQUE,
    direccion VARCHAR(250) NOT NULL,
    telefono VARCHAR(20),
    activa BOOLEAN NOT NULL DEFAULT TRUE
);

-- 2. CLIENTES
CREATE TABLE mp_clientes (
    id_cliente INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    identificacion VARCHAR(30) NOT NULL UNIQUE,
    nombre VARCHAR(150) NOT NULL,
    correo VARCHAR(150) NOT NULL UNIQUE,
    telefono VARCHAR(20)
);

-- 3. PRODUCTOS
CREATE TABLE mp_productos (
    id_producto INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    codigo VARCHAR(30) NOT NULL UNIQUE,
    nombre VARCHAR(150) NOT NULL,
    descripcion VARCHAR(300),
    precio NUMERIC(12,2) NOT NULL CHECK (precio > 0),
    atributos JSONB NOT NULL DEFAULT '{}'::jsonb,
    maximo_reserva INTEGER NOT NULL CHECK (maximo_reserva > 0),
    horas_reserva INTEGER NOT NULL CHECK (horas_reserva > 0),
    existencia_minima INTEGER NOT NULL DEFAULT 0
        CHECK (existencia_minima >= 0),
    CONSTRAINT ck_atributos_objeto
        CHECK (jsonb_typeof(atributos) = 'object')
);

-- 4. INVENTARIOS POR SUCURSAL
CREATE TABLE mp_inventarios (
    id_inventario INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    id_sucursal INTEGER NOT NULL,
    id_producto INTEGER NOT NULL,
    CONSTRAINT fk_inventario_sucursal
        FOREIGN KEY (id_sucursal)
        REFERENCES mp_sucursales(id_sucursal)
        ON DELETE RESTRICT,
    CONSTRAINT fk_inventario_producto
        FOREIGN KEY (id_producto)
        REFERENCES mp_productos(id_producto)
        ON DELETE RESTRICT,
    CONSTRAINT uq_inventario_sucursal_producto
        UNIQUE (id_sucursal, id_producto)
);

-- 5. INGRESOS DE INVENTARIO
CREATE TABLE mp_ingresos (
    id_ingreso INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    id_inventario INTEGER NOT NULL,
    cantidad INTEGER NOT NULL CHECK (cantidad > 0),
    fecha_ingreso TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    referencia VARCHAR(150),
    CONSTRAINT fk_ingreso_inventario
        FOREIGN KEY (id_inventario)
        REFERENCES mp_inventarios(id_inventario)
        ON DELETE RESTRICT
);

-- 6. RESERVAS
CREATE TABLE mp_reservas (
    id_reserva INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    id_inventario INTEGER NOT NULL,
    id_cliente INTEGER NOT NULL,
    cantidad INTEGER NOT NULL CHECK (cantidad > 0),
    precio_unitario NUMERIC(12,2) NOT NULL
        CHECK (precio_unitario > 0),
    fecha_reserva TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    fecha_vencimiento TIMESTAMPTZ NOT NULL,
    estado VARCHAR(15) NOT NULL DEFAULT 'RESERVADO',
    CONSTRAINT fk_reserva_inventario
        FOREIGN KEY (id_inventario)
        REFERENCES mp_inventarios(id_inventario)
        ON DELETE RESTRICT,
    CONSTRAINT fk_reserva_cliente
        FOREIGN KEY (id_cliente)
        REFERENCES mp_clientes(id_cliente)
        ON DELETE RESTRICT,
    CONSTRAINT ck_estado_reserva
        CHECK (estado IN ('RESERVADO', 'COMPRADO', 'CANCELADO')),
    CONSTRAINT ck_fecha_vencimiento
        CHECK (fecha_vencimiento > fecha_reserva)
);
COMMIT;
