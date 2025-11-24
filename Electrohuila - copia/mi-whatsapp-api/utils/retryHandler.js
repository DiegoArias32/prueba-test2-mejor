/**
 * Sistema de reintentos con backoff exponencial
 * Útil para operaciones que pueden fallar temporalmente (envío de mensajes, conexiones, etc.)
 */

/**
 * Ejecuta una función con reintentos y backoff exponencial
 * @param {Function} fn - Función asíncrona a ejecutar
 * @param {number} maxRetries - Número máximo de reintentos (default: 3)
 * @param {number} initialDelay - Delay inicial en ms (default: 1000)
 * @param {number} maxDelay - Delay máximo en ms (default: 30000)
 * @param {Function} onRetry - Callback opcional que se ejecuta en cada reintento
 * @returns {Promise} Resultado de la función
 */
async function retryWithBackoff(
    fn,
    maxRetries = 3,
    initialDelay = 1000,
    maxDelay = 30000,
    onRetry = null
) {
    let lastError;
    let delay = initialDelay;

    for (let attempt = 1; attempt <= maxRetries + 1; attempt++) {
        try {
            // Intentar ejecutar la función
            const result = await fn();

            // Si es el primer intento, no hay reintento
            if (attempt > 1) {
                console.log(`✅ Operación exitosa después de ${attempt - 1} reintento(s)`);
            }

            return result;

        } catch (error) {
            lastError = error;

            // Si ya agotamos los reintentos, lanzar el error
            if (attempt > maxRetries) {
                console.error(`❌ Operación falló después de ${maxRetries} reintentos: ${error.message}`);
                throw error;
            }

            // Calcular el delay con backoff exponencial
            const currentDelay = Math.min(delay, maxDelay);

            console.warn(
                `⚠️ Intento ${attempt}/${maxRetries + 1} falló: ${error.message}. ` +
                `Reintentando en ${currentDelay}ms...`
            );

            // Ejecutar callback de reintento si existe
            if (onRetry) {
                try {
                    await onRetry(attempt, error, currentDelay);
                } catch (callbackError) {
                    console.error(`Error en callback onRetry: ${callbackError.message}`);
                }
            }

            // Esperar antes del siguiente intento
            await sleep(currentDelay);

            // Incrementar el delay exponencialmente (2x) con jitter aleatorio
            delay = delay * 2 * (0.9 + Math.random() * 0.2); // Jitter del 10%
        }
    }

    // Esto no debería alcanzarse, pero por seguridad
    throw lastError;
}

/**
 * Función auxiliar para esperar un tiempo determinado
 * @param {number} ms - Milisegundos a esperar
 * @returns {Promise<void>}
 */
function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

/**
 * Wrapper para reintentos con configuración específica para envío de mensajes
 * @param {Function} sendFn - Función de envío de mensaje
 * @param {object} options - Opciones de configuración
 * @returns {Promise}
 */
async function retryMessageSend(sendFn, options = {}) {
    const {
        maxRetries = 3,
        initialDelay = 2000,
        maxDelay = 15000,
        phoneNumber = 'desconocido'
    } = options;

    return retryWithBackoff(
        sendFn,
        maxRetries,
        initialDelay,
        maxDelay,
        (attempt, error, delay) => {
            console.log(`📱 Reintentando envío a ${phoneNumber} (intento ${attempt})`);
        }
    );
}

/**
 * Ejecuta múltiples funciones con reintentos en paralelo
 * @param {Array<Function>} functions - Array de funciones a ejecutar
 * @param {object} retryOptions - Opciones de reintento
 * @returns {Promise<Array>} Resultados de todas las funciones
 */
async function retryBatch(functions, retryOptions = {}) {
    const promises = functions.map(fn =>
        retryWithBackoff(fn, retryOptions.maxRetries, retryOptions.initialDelay)
    );

    return Promise.allSettled(promises);
}

/**
 * Ejecuta una función con timeout y reintentos
 * @param {Function} fn - Función a ejecutar
 * @param {number} timeoutMs - Timeout en milisegundos
 * @param {number} maxRetries - Número de reintentos
 * @returns {Promise}
 */
async function retryWithTimeout(fn, timeoutMs = 30000, maxRetries = 3) {
    return retryWithBackoff(async () => {
        return Promise.race([
            fn(),
            new Promise((_, reject) =>
                setTimeout(() => reject(new Error('Timeout excedido')), timeoutMs)
            )
        ]);
    }, maxRetries);
}

module.exports = {
    retryWithBackoff,
    retryMessageSend,
    retryBatch,
    retryWithTimeout,
    sleep
};
