-- =====================================================================
-- Migration: Add CLIENT_ID to NOTIFICATIONS table
-- Description: Allows notifications to be sent to either USERS or CLIENTS
-- Date: 2025-11-23
-- Schema: ADMIN
-- =====================================================================

-- Step 1: Make USER_ID nullable (if it's currently NOT NULL)
ALTER TABLE ADMIN.NOTIFICATIONS MODIFY USER_ID NUMBER(10) NULL;

-- Step 2: Add CLIENT_ID column
ALTER TABLE ADMIN.NOTIFICATIONS ADD CLIENT_ID NUMBER(10) NULL;

-- Step 3: Add Foreign Key to CLIENTS table
ALTER TABLE ADMIN.NOTIFICATIONS
ADD CONSTRAINT FK_NOTIFICATIONS_CLIENT
FOREIGN KEY (CLIENT_ID) REFERENCES ADMIN.CLIENTS(ID);

-- Step 4: Add CHECK constraint to ensure exactly one of USER_ID or CLIENT_ID is set
ALTER TABLE ADMIN.NOTIFICATIONS
ADD CONSTRAINT CHK_NOTIFICATION_RECIPIENT
CHECK ((USER_ID IS NOT NULL AND CLIENT_ID IS NULL) OR (USER_ID IS NULL AND CLIENT_ID IS NOT NULL));

-- Step 5: Create index on CLIENT_ID for query performance
CREATE INDEX IX_NOTIFICATIONS_CLIENT_ID ON ADMIN.NOTIFICATIONS(CLIENT_ID);

-- Step 6: Create composite index for client notifications ordered by date
CREATE INDEX IX_NOTIFICATIONS_CLIENT_CREATED ON ADMIN.NOTIFICATIONS(CLIENT_ID, CREATED_AT);

-- Step 7: Add comments for documentation
COMMENT ON COLUMN ADMIN.NOTIFICATIONS.USER_ID IS 'ID del usuario (admin) destinatario. NULL si la notificación es para un cliente.';
COMMENT ON COLUMN ADMIN.NOTIFICATIONS.CLIENT_ID IS 'ID del cliente destinatario. NULL si la notificación es para un usuario (admin).';

-- Verification queries (run these to verify the migration)
-- SELECT COUNT(*) FROM NOTIFICATIONS WHERE USER_ID IS NOT NULL AND CLIENT_ID IS NULL; -- Should be all existing records
-- SELECT COUNT(*) FROM NOTIFICATIONS WHERE USER_ID IS NOT NULL AND CLIENT_ID IS NOT NULL; -- Should be 0
-- SELECT COUNT(*) FROM NOTIFICATIONS WHERE USER_ID IS NULL AND CLIENT_ID IS NULL; -- Should be 0

COMMIT;
