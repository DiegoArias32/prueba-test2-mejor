-- =====================================================================
-- SCRIPT DE ASIGNACIÓN DE EMPLEADOS A TIPOS DE CITA
-- =====================================================================
-- VERSION: 1.0
-- Propósito: Permitir asignar empleados a tipos de cita específicos
-- para que solo vean las citas que les corresponden
-- =====================================================================

SET SERVEROUTPUT ON;

PROMPT ===============================================
PROMPT    CREACIÓN DE TABLA USER_APPOINTMENT_TYPE_ASSIGNMENTS
PROMPT ===============================================

-- ===================== DROP EXISTING OBJECTS =====================
BEGIN
    EXECUTE IMMEDIATE 'DROP TABLE ADMIN.UserAppointmentTypeAssignments CASCADE CONSTRAINTS PURGE';
    DBMS_OUTPUT.PUT_LINE('Table UserAppointmentTypeAssignments dropped');
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -942 THEN
            RAISE;
        END IF;
        DBMS_OUTPUT.PUT_LINE('Table UserAppointmentTypeAssignments does not exist, skipping drop');
END;
/

BEGIN
    EXECUTE IMMEDIATE 'DROP SEQUENCE ADMIN.SEQ_USER_APPT_TYPE_ASSIGNS';
    DBMS_OUTPUT.PUT_LINE('Sequence SEQ_USER_APPT_TYPE_ASSIGNS dropped');
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -2289 THEN
            RAISE;
        END IF;
        DBMS_OUTPUT.PUT_LINE('Sequence SEQ_USER_APPT_TYPE_ASSIGNS does not exist, skipping drop');
END;
/

BEGIN
    EXECUTE IMMEDIATE 'DROP VIEW ADMIN.VW_USER_ASSIGNMENTS_DETAIL';
    DBMS_OUTPUT.PUT_LINE('View VW_USER_ASSIGNMENTS_DETAIL dropped');
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -942 THEN
            RAISE;
        END IF;
        DBMS_OUTPUT.PUT_LINE('View VW_USER_ASSIGNMENTS_DETAIL does not exist, skipping drop');
END;
/

-- ===================== CREATE TABLE =====================
PROMPT 📋 Creando tabla UserAppointmentTypeAssignments...

CREATE TABLE ADMIN.UserAppointmentTypeAssignments (
    ID NUMBER PRIMARY KEY,
    USER_ID NUMBER NOT NULL,
    APPOINTMENT_TYPE_ID NUMBER NOT NULL,
    CREATED_AT TIMESTAMP(7) DEFAULT CURRENT_TIMESTAMP NOT NULL,
    UPDATED_AT TIMESTAMP(7),
    IS_ACTIVE NUMBER(1) DEFAULT 1 NOT NULL,

    -- Foreign Keys
    CONSTRAINT FK_USER_ASSIGN_USER FOREIGN KEY (USER_ID)
        REFERENCES ADMIN.Users(ID) ON DELETE CASCADE,
    CONSTRAINT FK_USER_ASSIGN_APPT_TYPE FOREIGN KEY (APPOINTMENT_TYPE_ID)
        REFERENCES ADMIN.AppointmentTypes(ID) ON DELETE CASCADE,

    -- Unique constraint to prevent duplicate assignments
    CONSTRAINT UK_USER_APPT_TYPE_ASSIGN UNIQUE (USER_ID, APPOINTMENT_TYPE_ID)
);

-- ===================== CREATE INDEXES =====================
PROMPT 📊 Creando índices...

CREATE INDEX IDX_USER_ASSIGN_USER_ID
    ON ADMIN.UserAppointmentTypeAssignments(USER_ID);

CREATE INDEX IDX_USER_ASSIGN_APPT_TYPE_ID
    ON ADMIN.UserAppointmentTypeAssignments(APPOINTMENT_TYPE_ID);

CREATE INDEX IDX_USER_ASSIGN_ACTIVE
    ON ADMIN.UserAppointmentTypeAssignments(IS_ACTIVE);

-- ===================== CREATE SEQUENCE =====================
PROMPT 🔢 Creando secuencia...

CREATE SEQUENCE ADMIN.SEQ_USER_APPT_TYPE_ASSIGNS
    START WITH 1
    INCREMENT BY 1
    NOCACHE
    NOCYCLE;

-- ===================== CREATE TRIGGER =====================
PROMPT ⚡ Creando trigger para auto-incremento...

CREATE OR REPLACE TRIGGER ADMIN.TRG_USER_APPT_TYPE_ASSIGNS_BI
    BEFORE INSERT ON ADMIN.UserAppointmentTypeAssignments
    FOR EACH ROW
BEGIN
    IF :NEW.ID IS NULL THEN
        SELECT ADMIN.SEQ_USER_APPT_TYPE_ASSIGNS.NEXTVAL
        INTO :NEW.ID
        FROM DUAL;
    END IF;

    :NEW.CREATED_AT := CURRENT_TIMESTAMP;
    :NEW.IS_ACTIVE := 1;
END;
/

-- ===================== CREATE VIEW =====================
PROMPT 👁️  Creando vista VW_USER_ASSIGNMENTS_DETAIL...

CREATE OR REPLACE VIEW ADMIN.VW_USER_ASSIGNMENTS_DETAIL AS
SELECT
    ua.ID,
    ua.USER_ID,
    u.USERNAME,
    u.EMAIL,
    ua.APPOINTMENT_TYPE_ID,
    at.CODE AS APPOINTMENT_TYPE_CODE,
    at.NAME AS APPOINTMENT_TYPE_NAME,
    at.DESCRIPTION AS APPOINTMENT_TYPE_DESCRIPTION,
    at.ICON_NAME AS APPOINTMENT_TYPE_ICON,
    at.COLOR_PRIMARY AS APPOINTMENT_TYPE_COLOR,
    ua.CREATED_AT,
    ua.UPDATED_AT,
    ua.IS_ACTIVE
FROM
    ADMIN.UserAppointmentTypeAssignments ua
    INNER JOIN ADMIN.Users u ON ua.USER_ID = u.ID
    INNER JOIN ADMIN.AppointmentTypes at ON ua.APPOINTMENT_TYPE_ID = at.ID
WHERE
    ua.IS_ACTIVE = 1
    AND u.IS_ACTIVE = 1
    AND at.IS_ACTIVE = 1;

-- ===================== INSERT TEST DATA =====================
PROMPT 📝 Insertando datos de prueba...

-- Nota: Ajusta los IDs de usuarios y tipos de cita según tu base de datos
-- Este script asume que existen usuarios con ID 1, 2, 3 y tipos de cita con ID 1, 2, 3, 4

DECLARE
    v_user_count NUMBER;
    v_appt_type_count NUMBER;
BEGIN
    -- Verificar que existan usuarios
    SELECT COUNT(*) INTO v_user_count FROM ADMIN.Users WHERE IS_ACTIVE = 1;

    -- Verificar que existan tipos de cita
    SELECT COUNT(*) INTO v_appt_type_count FROM ADMIN.AppointmentTypes WHERE IS_ACTIVE = 1;

    IF v_user_count > 0 AND v_appt_type_count > 0 THEN
        -- Asignar primer tipo de cita al primer usuario (si existe)
        BEGIN
            INSERT INTO ADMIN.UserAppointmentTypeAssignments (USER_ID, APPOINTMENT_TYPE_ID)
            SELECT
                (SELECT MIN(ID) FROM ADMIN.Users WHERE IS_ACTIVE = 1) AS USER_ID,
                (SELECT MIN(ID) FROM ADMIN.AppointmentTypes WHERE IS_ACTIVE = 1) AS APPOINTMENT_TYPE_ID
            FROM DUAL
            WHERE EXISTS (SELECT 1 FROM ADMIN.Users WHERE IS_ACTIVE = 1)
              AND EXISTS (SELECT 1 FROM ADMIN.AppointmentTypes WHERE IS_ACTIVE = 1);

            DBMS_OUTPUT.PUT_LINE('✅ Asignación de prueba creada para primer usuario');
        EXCEPTION
            WHEN DUP_VAL_ON_INDEX THEN
                DBMS_OUTPUT.PUT_LINE('⚠️  Asignación ya existe, saltando...');
            WHEN OTHERS THEN
                DBMS_OUTPUT.PUT_LINE('⚠️  No se pudo crear asignación de prueba: ' || SQLERRM);
        END;
    ELSE
        DBMS_OUTPUT.PUT_LINE('⚠️  No hay usuarios o tipos de cita activos para crear datos de prueba');
    END IF;
END;
/

-- ===================== VERIFY INSTALLATION =====================
PROMPT ===============================================
PROMPT    VERIFICACIÓN DE INSTALACIÓN
PROMPT ===============================================

SELECT
    'UserAppointmentTypeAssignments' AS OBJECT_NAME,
    'TABLE' AS OBJECT_TYPE,
    CASE WHEN COUNT(*) > 0 THEN '✅ EXISTS' ELSE '❌ NOT FOUND' END AS STATUS
FROM all_tables
WHERE owner = 'ADMIN' AND table_name = 'USERAPPOINTMENTTYPEASSIGNMENTS'

UNION ALL

SELECT
    'SEQ_USER_APPT_TYPE_ASSIGNS' AS OBJECT_NAME,
    'SEQUENCE' AS OBJECT_TYPE,
    CASE WHEN COUNT(*) > 0 THEN '✅ EXISTS' ELSE '❌ NOT FOUND' END AS STATUS
FROM all_sequences
WHERE sequence_owner = 'ADMIN' AND sequence_name = 'SEQ_USER_APPT_TYPE_ASSIGNS'

UNION ALL

SELECT
    'VW_USER_ASSIGNMENTS_DETAIL' AS OBJECT_NAME,
    'VIEW' AS OBJECT_TYPE,
    CASE WHEN COUNT(*) > 0 THEN '✅ EXISTS' ELSE '❌ NOT FOUND' END AS STATUS
FROM all_views
WHERE owner = 'ADMIN' AND view_name = 'VW_USER_ASSIGNMENTS_DETAIL';

-- Mostrar conteo de registros
SELECT
    'UserAppointmentTypeAssignments' AS TABLE_NAME,
    COUNT(*) AS TOTAL_RECORDS,
    SUM(CASE WHEN IS_ACTIVE = 1 THEN 1 ELSE 0 END) AS ACTIVE_RECORDS
FROM ADMIN.UserAppointmentTypeAssignments;

PROMPT ===============================================
PROMPT    INSTALACIÓN COMPLETADA
PROMPT ===============================================
