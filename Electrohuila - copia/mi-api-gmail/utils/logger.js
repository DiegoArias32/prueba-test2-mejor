/**
 * Sistema de logging para emails enviados
 */

const fs = require('fs').promises;
const path = require('path');

class EmailLogger {
  constructor(logDir = './logs') {
    this.logDir = logDir;
    this.ensureLogDirectory();
  }

  async ensureLogDirectory() {
    try {
      await fs.mkdir(this.logDir, { recursive: true });
    } catch (error) {
      console.error('Error creando directorio de logs:', error);
    }
  }

  async logEmail(data) {
    const timestamp = new Date().toISOString();
    const date = timestamp.split('T')[0];
    const logFile = path.join(this.logDir, `gmail-${date}.log`);

    const logEntry = {
      timestamp,
      email: data.email,
      template: data.template,
      subject: data.subject,
      success: data.success,
      error: data.error || null,
      messageId: data.messageId || null
    };

    const logLine = JSON.stringify(logEntry) + '\n';

    try {
      await fs.appendFile(logFile, logLine);
    } catch (error) {
      console.error('Error escribiendo log:', error);
    }
  }

  async getStats(date = null) {
    const targetDate = date || new Date().toISOString().split('T')[0];
    const logFile = path.join(this.logDir, `gmail-${targetDate}.log`);

    try {
      const content = await fs.readFile(logFile, 'utf-8');
      const lines = content.trim().split('\n').filter(line => line);
      const entries = lines.map(line => JSON.parse(line));

      const stats = {
        total: entries.length,
        success: entries.filter(e => e.success).length,
        failed: entries.filter(e => !e.success).length,
        byTemplate: {}
      };

      entries.forEach(entry => {
        if (!stats.byTemplate[entry.template]) {
          stats.byTemplate[entry.template] = { total: 0, success: 0, failed: 0 };
        }
        stats.byTemplate[entry.template].total++;
        if (entry.success) {
          stats.byTemplate[entry.template].success++;
        } else {
          stats.byTemplate[entry.template].failed++;
        }
      });

      return stats;
    } catch (error) {
      return { total: 0, success: 0, failed: 0, byTemplate: {} };
    }
  }

  async getLogs(date = null, limit = 100) {
    const targetDate = date || new Date().toISOString().split('T')[0];
    const logFile = path.join(this.logDir, `gmail-${targetDate}.log`);

    try {
      const content = await fs.readFile(logFile, 'utf-8');
      const lines = content.trim().split('\n').filter(line => line);
      const entries = lines.map(line => JSON.parse(line));

      // Retornar los últimos N logs
      return entries.slice(-limit).reverse();
    } catch (error) {
      return [];
    }
  }
}

module.exports = new EmailLogger();
