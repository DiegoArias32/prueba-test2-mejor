/**
 * Sistema de logging para mensajes de WhatsApp
 * Guarda logs en archivos JSON organizados por fecha
 */

const fs = require('fs');
const path = require('path');

class MessageLogger {
    constructor(logDir = 'logs') {
        // Usar ruta absoluta para el directorio de logs
        this.logDir = path.isAbsolute(logDir)
            ? logDir
            : path.join(process.cwd(), logDir);

        this.ensureLogDirectory();
    }

    /**
     * Asegura que el directorio de logs existe
     */
    ensureLogDirectory() {
        if (!fs.existsSync(this.logDir)) {
            fs.mkdirSync(this.logDir, { recursive: true });
            console.log(`📁 Directorio de logs creado: ${this.logDir}`);
        }
    }

    /**
     * Obtiene la ruta del archivo de log para una fecha específica
     * @param {Date} date - Fecha para el log
     * @returns {string} Ruta del archivo de log
     */
    getLogFilePath(date = new Date()) {
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        const filename = `whatsapp-${year}-${month}-${day}.log`;
        return path.join(this.logDir, filename);
    }

    /**
     * Registra un mensaje enviado
     * @param {object} data - Datos del mensaje
     * @returns {boolean} true si se guardó correctamente
     */
    logMessage(data) {
        try {
            const timestamp = new Date().toISOString();

            const logEntry = {
                timestamp,
                phoneNumber: data.phoneNumber,
                phoneFormatted: data.phoneFormatted,
                template: data.template || null,
                success: data.success,
                error: data.error || null,
                retries: data.retries || 0,
                messageLength: data.messageLength || 0,
                data: data.templateData || null
            };

            const logLine = JSON.stringify(logEntry) + '\n';
            const logFile = this.getLogFilePath();

            // Escribir el log de forma síncrona para asegurar que se guarda
            fs.appendFileSync(logFile, logLine, 'utf8');

            return true;
        } catch (error) {
            console.error(`❌ Error guardando log: ${error.message}`);
            return false;
        }
    }

    /**
     * Lee los logs de una fecha específica
     * @param {string} dateString - Fecha en formato YYYY-MM-DD
     * @returns {Array} Array de entradas de log
     */
    readLogs(dateString) {
        try {
            const date = dateString ? new Date(dateString) : new Date();
            const logFile = this.getLogFilePath(date);

            if (!fs.existsSync(logFile)) {
                return [];
            }

            const content = fs.readFileSync(logFile, 'utf8');
            const lines = content.trim().split('\n').filter(line => line.length > 0);

            return lines.map(line => {
                try {
                    return JSON.parse(line);
                } catch (e) {
                    console.error(`Error parseando línea de log: ${e.message}`);
                    return null;
                }
            }).filter(entry => entry !== null);

        } catch (error) {
            console.error(`❌ Error leyendo logs: ${error.message}`);
            return [];
        }
    }

    /**
     * Obtiene estadísticas de mensajes para una fecha específica
     * @param {string} dateString - Fecha en formato YYYY-MM-DD (opcional)
     * @returns {object} Estadísticas del día
     */
    getStats(dateString = null) {
        const logs = this.readLogs(dateString);

        const stats = {
            date: dateString || new Date().toISOString().split('T')[0],
            totalMessages: logs.length,
            successful: 0,
            failed: 0,
            byTemplate: {},
            errors: {},
            totalRetries: 0,
            averageRetries: 0,
            uniquePhones: new Set()
        };

        logs.forEach(log => {
            // Contadores de éxito/fallo
            if (log.success) {
                stats.successful++;
            } else {
                stats.failed++;

                // Contar errores por tipo
                const errorType = log.error || 'unknown';
                stats.errors[errorType] = (stats.errors[errorType] || 0) + 1;
            }

            // Estadísticas por template
            if (log.template) {
                if (!stats.byTemplate[log.template]) {
                    stats.byTemplate[log.template] = {
                        total: 0,
                        successful: 0,
                        failed: 0
                    };
                }
                stats.byTemplate[log.template].total++;
                if (log.success) {
                    stats.byTemplate[log.template].successful++;
                } else {
                    stats.byTemplate[log.template].failed++;
                }
            }

            // Reintentos
            stats.totalRetries += log.retries || 0;

            // Teléfonos únicos
            if (log.phoneNumber) {
                stats.uniquePhones.add(log.phoneNumber);
            }
        });

        // Calcular promedio de reintentos
        stats.averageRetries = stats.totalMessages > 0
            ? (stats.totalRetries / stats.totalMessages).toFixed(2)
            : 0;

        // Calcular tasa de éxito
        stats.successRate = stats.totalMessages > 0
            ? ((stats.successful / stats.totalMessages) * 100).toFixed(2) + '%'
            : '0%';

        // Convertir Set a número
        stats.uniquePhones = stats.uniquePhones.size;

        return stats;
    }

    /**
     * Obtiene los logs de los últimos N días
     * @param {number} days - Número de días hacia atrás
     * @returns {Array} Logs de los últimos N días
     */
    getRecentLogs(days = 7) {
        const logs = [];
        const today = new Date();

        for (let i = 0; i < days; i++) {
            const date = new Date(today);
            date.setDate(date.getDate() - i);
            const dateString = date.toISOString().split('T')[0];
            const dayLogs = this.readLogs(dateString);
            logs.push(...dayLogs);
        }

        return logs;
    }

    /**
     * Limpia logs antiguos (más viejos que X días)
     * @param {number} daysToKeep - Días de logs a mantener
     * @returns {number} Número de archivos eliminados
     */
    cleanOldLogs(daysToKeep = 30) {
        try {
            const files = fs.readdirSync(this.logDir);
            const cutoffDate = new Date();
            cutoffDate.setDate(cutoffDate.getDate() - daysToKeep);

            let deletedCount = 0;

            files.forEach(file => {
                if (!file.startsWith('whatsapp-') || !file.endsWith('.log')) {
                    return;
                }

                const filePath = path.join(this.logDir, file);
                const stats = fs.statSync(filePath);

                if (stats.mtime < cutoffDate) {
                    fs.unlinkSync(filePath);
                    deletedCount++;
                    console.log(`🗑️ Log antiguo eliminado: ${file}`);
                }
            });

            return deletedCount;
        } catch (error) {
            console.error(`❌ Error limpiando logs antiguos: ${error.message}`);
            return 0;
        }
    }

    /**
     * Lista todos los archivos de log disponibles
     * @returns {Array} Lista de archivos de log con sus estadísticas
     */
    listLogFiles() {
        try {
            const files = fs.readdirSync(this.logDir);

            return files
                .filter(file => file.startsWith('whatsapp-') && file.endsWith('.log'))
                .map(file => {
                    const filePath = path.join(this.logDir, file);
                    const stats = fs.statSync(filePath);

                    return {
                        filename: file,
                        size: stats.size,
                        modified: stats.mtime,
                        path: filePath
                    };
                })
                .sort((a, b) => b.modified - a.modified); // Más recientes primero

        } catch (error) {
            console.error(`❌ Error listando archivos de log: ${error.message}`);
            return [];
        }
    }
}

// Crear instancia global del logger
const logger = new MessageLogger();

module.exports = {
    MessageLogger,
    logger
};
