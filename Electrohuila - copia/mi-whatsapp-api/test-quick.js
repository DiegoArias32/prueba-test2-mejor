/**
 * Script de prueba rápida para verificar que la API funciona correctamente
 * Ejecutar: node test-quick.js
 * IMPORTANTE: Actualiza API_KEY con tu clave real antes de ejecutar
 */

const API_KEY = 'tu-api-key-aqui'; // Cambiar por tu API key real
const BASE_URL = 'http://localhost:3000';
const TEST_PHONE = '3001234567'; // Cambiar por un número de prueba válido

async function testQuick() {
    console.log('\n' + '='.repeat(60));
    console.log('PRUEBA RÁPIDA DE API DE WHATSAPP');
    console.log('='.repeat(60) + '\n');

    try {
        // 1. Health check
        console.log('1️⃣  Health check...');
        const health = await fetch(`${BASE_URL}/health`);
        const healthData = await health.json();
        console.log('   ✓ Status:', healthData.status);
        console.log('   ✓ WhatsApp ready:', healthData.whatsappReady);
        console.log('');

        if (!healthData.whatsappReady) {
            console.warn('⚠️  WhatsApp no está listo. Asegúrate de haber escaneado el QR.\n');
        }

        // 2. Estado de WhatsApp
        console.log('2️⃣  Estado de WhatsApp...');
        const status = await fetch(`${BASE_URL}/whatsapp/status`);
        const statusData = await status.json();
        console.log('   ✓ Status:', statusData.status);
        if (statusData.clientInfo) {
            console.log('   ✓ Plataforma:', statusData.clientInfo.platform);
            console.log('   ✓ Teléfono:', statusData.clientInfo.phone);
        }
        console.log('');

        // 3. Templates disponibles
        console.log('3️⃣  Templates disponibles...');
        const templates = await fetch(`${BASE_URL}/whatsapp/templates`, {
            headers: { 'X-API-Key': API_KEY }
        });
        const templatesData = await templates.json();

        if (templatesData.success) {
            console.log('   ✓ Templates cargados:', templatesData.templates.length);
            templatesData.templates.forEach(t => {
                console.log(`      - ${t.name} (campos: ${t.requiredFields.join(', ')})`);
            });
        } else {
            console.error('   ✗ Error:', templatesData.error);
        }
        console.log('');

        // 4. Prueba de validación (número inválido)
        console.log('4️⃣  Prueba de validación (número inválido)...');
        const invalidTest = await fetch(`${BASE_URL}/whatsapp/appointment-confirmation`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-API-Key': API_KEY
            },
            body: JSON.stringify({
                phoneNumber: '123',
                data: { nombre: 'Test' }
            })
        });
        const invalidData = await invalidTest.json();
        console.log('   ✓ Validación funciona:', !invalidData.success ? 'Sí' : 'No');
        console.log('   ✓ Error esperado:', invalidData.error);
        console.log('');

        // 5. Estadísticas
        console.log('5️⃣  Estadísticas de hoy...');
        const stats = await fetch(`${BASE_URL}/whatsapp/stats`, {
            headers: { 'X-API-Key': API_KEY }
        });
        const statsData = await stats.json();

        if (statsData.success) {
            console.log('   ✓ Fecha:', statsData.stats.date);
            console.log('   ✓ Total mensajes:', statsData.stats.totalMessages);
            console.log('   ✓ Exitosos:', statsData.stats.successful);
            console.log('   ✓ Fallidos:', statsData.stats.failed);
            console.log('   ✓ Tasa de éxito:', statsData.stats.successRate);
        } else {
            console.error('   ✗ Error:', statsData.error);
        }
        console.log('');

        console.log('='.repeat(60));
        console.log('✅ PRUEBAS BÁSICAS COMPLETADAS');
        console.log('='.repeat(60));
        console.log('\n💡 Para enviar un mensaje de prueba, descomenta la sección');
        console.log('   de "Enviar confirmación" en este archivo.\n');

        // DESCOMENTAR PARA ENVIAR MENSAJE REAL:
        /*
        console.log('6️⃣  Enviando confirmación de cita...');
        const confirmation = await fetch(`${BASE_URL}/whatsapp/appointment-confirmation`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-API-Key': API_KEY
            },
            body: JSON.stringify({
                phoneNumber: TEST_PHONE,
                data: {
                    nombre: 'Prueba API',
                    fecha: '2025-11-25',
                    hora: '10:00 AM',
                    servicio: 'Test de API',
                    direccion: 'Dirección de prueba'
                }
            })
        });
        const confirmationData = await confirmation.json();
        console.log('   Resultado:', confirmationData);
        */

    } catch (error) {
        console.error('\n❌ ERROR:', error.message);
        console.error('\n⚠️  Asegúrate de que:');
        console.error('   1. El servidor está corriendo (npm start)');
        console.error('   2. Has configurado la API key correctamente');
        console.error('   3. El puerto 3000 está disponible\n');
    }
}

// Ejecutar pruebas
testQuick().catch(console.error);
