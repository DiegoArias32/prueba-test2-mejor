/**
 * Sistema de reintentos con backoff exponencial
 * Para envío de emails con manejo de errores
 */

async function retryWithBackoff(fn, maxRetries = 3, delay = 1000) {
  let lastError;

  for (let attempt = 1; attempt <= maxRetries; attempt++) {
    try {
      const result = await fn();

      if (attempt > 1) {
        console.log(`✅ Éxito en intento ${attempt}/${maxRetries}`);
      }

      return result;
    } catch (error) {
      lastError = error;
      console.error(`❌ Intento ${attempt}/${maxRetries} falló:`, error.message);

      if (attempt === maxRetries) {
        throw new Error(`Falló después de ${maxRetries} intentos: ${error.message}`);
      }

      const waitTime = delay * Math.pow(2, attempt - 1);
      console.log(`⏳ Esperando ${waitTime}ms antes del siguiente intento...`);
      await new Promise(resolve => setTimeout(resolve, waitTime));
    }
  }

  throw lastError;
}

module.exports = { retryWithBackoff };
