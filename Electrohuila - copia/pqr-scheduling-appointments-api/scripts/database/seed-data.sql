-- =============================================
-- ElectroHuila - Seed Data Script
-- Datos iniciales para el sistema
-- =============================================

-- =============================================
-- 1. BRANCHES (Sucursales)
-- =============================================
INSERT INTO ADMIN.BRANCHES (ID, NAME, CODE, ADDRESS, PHONE, CITY, STATE, IS_MAIN, IS_ACTIVE, CREATED_AT)
VALUES
(1, 'Sede Principal Neiva', 'MAIN001', 'Calle 8 # 5-20, Centro', '8711234', 'Neiva', 'Huila', 1, 1, SYSTIMESTAMP),
(2, 'Sucursal Norte Neiva', 'NORTE001', 'Carrera 5 # 52-10', '8715678', 'Neiva', 'Huila', 0, 1, SYSTIMESTAMP),
(3, 'Sucursal Garzón', 'GARZON001', 'Calle 7 # 9-15', '8331234', 'Garzón', 'Huila', 0, 1, SYSTIMESTAMP),
(4, 'Sucursal La Plata', 'PLATA001', 'Carrera 3 # 4-22', '8371234', 'La Plata', 'Huila', 0, 1, SYSTIMESTAMP),
(5, 'Sucursal Pitalito', 'PITA001', 'Calle 5 # 3-10', '8361234', 'Pitalito', 'Huila', 0, 1, SYSTIMESTAMP);

-- =============================================
-- 2. ROLES
-- =============================================
INSERT INTO ADMIN.ROLES (ID, NAME, CODE, DESCRIPTION, IS_ACTIVE, CREATED_AT)
VALUES
(1, 'Administrador', 'ADMIN', 'Acceso completo al sistema', 1, SYSTIMESTAMP),
(2, 'Empleado', 'EMPLOYEE', 'Empleado de ElectroHuila', 1, SYSTIMESTAMP),
(3, 'Cliente', 'CLIENT', 'Usuario cliente', 1, SYSTIMESTAMP),
(4, 'Supervisor', 'SUPERVISOR', 'Supervisor de sucursal', 1, SYSTIMESTAMP);

-- =============================================
-- 3. USERS
-- Nota: Password por defecto: Admin123!
-- Hash BCrypt de "Admin123!"
-- =============================================
INSERT INTO ADMIN.USERS (ID, USERNAME, EMAIL, PASSWORD_HASH, FULL_NAME, IS_ACTIVE, CREATED_AT)
VALUES
(1, 'admin', 'admin@electrohuila.com', '$2a$11$K7ZQWZqFwxYz4JKn5h9WqOJxvxHZQx5n3FZqWZqFwxYz4JKn5h9Wq', 'Administrador del Sistema', 1, SYSTIMESTAMP),
(2, 'empleado1', 'empleado1@electrohuila.com', '$2a$11$K7ZQWZqFwxYz4JKn5h9WqOJxvxHZQx5n3FZqWZqFwxYz4JKn5h9Wq', 'Juan Pérez', 1, SYSTIMESTAMP),
(3, 'supervisor1', 'supervisor1@electrohuila.com', '$2a$11$K7ZQWZqFwxYz4JKn5h9WqOJxvxHZQx5n3FZqWZqFwxYz4JKn5h9Wq', 'María García', 1, SYSTIMESTAMP);

-- =============================================
-- 4. USER_ROLES (Asignación de roles)
-- =============================================
INSERT INTO ADMIN.USER_ROLES (USER_ID, ROL_ID)
VALUES
(1, 1), -- admin -> ADMIN
(2, 2), -- empleado1 -> EMPLOYEE
(3, 4); -- supervisor1 -> SUPERVISOR

-- =============================================
-- 5. PERMISSIONS
-- =============================================
INSERT INTO ADMIN.PERMISSIONS (ID, MODULE, ACTION, DESCRIPTION, IS_ACTIVE, CREATED_AT)
VALUES
-- Appointments
(1, 'Appointments', 'View', 'Ver citas', 1, SYSTIMESTAMP),
(2, 'Appointments', 'Create', 'Crear citas', 1, SYSTIMESTAMP),
(3, 'Appointments', 'Update', 'Actualizar citas', 1, SYSTIMESTAMP),
(4, 'Appointments', 'Delete', 'Eliminar citas', 1, SYSTIMESTAMP),
-- Users
(5, 'Users', 'View', 'Ver usuarios', 1, SYSTIMESTAMP),
(6, 'Users', 'Create', 'Crear usuarios', 1, SYSTIMESTAMP),
(7, 'Users', 'Update', 'Actualizar usuarios', 1, SYSTIMESTAMP),
(8, 'Users', 'Delete', 'Eliminar usuarios', 1, SYSTIMESTAMP),
-- Branches
(9, 'Branches', 'View', 'Ver sucursales', 1, SYSTIMESTAMP),
(10, 'Branches', 'Create', 'Crear sucursales', 1, SYSTIMESTAMP),
(11, 'Branches', 'Update', 'Actualizar sucursales', 1, SYSTIMESTAMP),
(12, 'Branches', 'Delete', 'Eliminar sucursales', 1, SYSTIMESTAMP);

-- =============================================
-- 6. ROL_PERMISSIONS
-- =============================================
-- Admin: Todos los permisos
INSERT INTO ADMIN.ROL_PERMISSIONS (ROL_ID, PERMISSION_ID)
SELECT 1, ID FROM ADMIN.PERMISSIONS;

-- Employee: Ver y crear citas
INSERT INTO ADMIN.ROL_PERMISSIONS (ROL_ID, PERMISSION_ID)
VALUES
(2, 1), (2, 2), (2, 9);

-- Supervisor: Ver todo, crear/actualizar citas
INSERT INTO ADMIN.ROL_PERMISSIONS (ROL_ID, PERMISSION_ID)
VALUES
(4, 1), (4, 2), (4, 3), (4, 5), (4, 9);

-- =============================================
-- 7. APPOINTMENT_TYPES
-- =============================================
INSERT INTO ADMIN.APPOINTMENT_TYPES (ID, NAME, DESCRIPTION, DURATION_MINUTES, IS_ACTIVE, CREATED_AT)
VALUES
(1, 'Reclamo Servicio', 'Reclamo por fallas en el servicio eléctrico', 30, 1, SYSTIMESTAMP),
(2, 'Nueva Acometida', 'Solicitud de nueva acometida eléctrica', 45, 1, SYSTIMESTAMP),
(3, 'Revisión Medidor', 'Revisión y calibración de medidor', 30, 1, SYSTIMESTAMP),
(4, 'Pago en Oficina', 'Pago de factura en oficina', 15, 1, SYSTIMESTAMP),
(5, 'Información General', 'Solicitud de información general', 20, 1, SYSTIMESTAMP),
(6, 'Reconexión Servicio', 'Reconexión del servicio eléctrico', 30, 1, SYSTIMESTAMP);

-- =============================================
-- 8. AVAILABLE_TIMES (Horarios disponibles)
-- =============================================
-- Horarios para Sede Principal (8am - 5pm)
DECLARE
  v_branch_id NUMBER := 1;
  v_appointment_type_id NUMBER;
BEGIN
  FOR apt_type IN (SELECT ID FROM ADMIN.APPOINTMENT_TYPES WHERE IS_ACTIVE = 1)
  LOOP
    -- Lunes a Viernes
    FOR day IN 1..5 LOOP
      FOR hour IN 8..16 LOOP
        INSERT INTO ADMIN.AVAILABLE_TIMES
        (BRANCH_ID, APPOINTMENT_TYPE_ID, DAY_OF_WEEK, START_TIME, END_TIME, MAX_APPOINTMENTS, IS_ACTIVE, CREATED_AT)
        VALUES
        (v_branch_id, apt_type.ID, day,
         LPAD(hour, 2, '0') || ':00',
         LPAD(hour+1, 2, '0') || ':00',
         5, 1, SYSTIMESTAMP);
      END LOOP;
    END LOOP;
  END LOOP;
  COMMIT;
END;
/

-- =============================================
-- 9. CLIENTS (Clientes de ejemplo)
-- =============================================
INSERT INTO ADMIN.CLIENTS (ID, DOCUMENT_NUMBER, DOCUMENT_TYPE, FULL_NAME, EMAIL, PHONE, ADDRESS, CITY, IS_ACTIVE, CREATED_AT)
VALUES
(1, '12345678', 'CC', 'Carlos Ramírez', 'carlos.ramirez@example.com', '3101234567', 'Calle 10 # 5-20', 'Neiva', 1, SYSTIMESTAMP),
(2, '23456789', 'CC', 'Ana López', 'ana.lopez@example.com', '3109876543', 'Carrera 7 # 15-30', 'Neiva', 1, SYSTIMESTAMP),
(3, '34567890', 'CC', 'Pedro González', 'pedro.gonzalez@example.com', '3112345678', 'Calle 5 # 8-10', 'Garzón', 1, SYSTIMESTAMP);

-- =============================================
-- RESUMEN
-- =============================================
SELECT 'Seed data insertado correctamente' AS STATUS FROM DUAL;
SELECT COUNT(*) AS BRANCHES FROM ADMIN.BRANCHES;
SELECT COUNT(*) AS ROLES FROM ADMIN.ROLES;
SELECT COUNT(*) AS USERS FROM ADMIN.USERS;
SELECT COUNT(*) AS PERMISSIONS FROM ADMIN.PERMISSIONS;
SELECT COUNT(*) AS APPOINTMENT_TYPES FROM ADMIN.APPOINTMENT_TYPES;
SELECT COUNT(*) AS CLIENTS FROM ADMIN.CLIENTS;

COMMIT;