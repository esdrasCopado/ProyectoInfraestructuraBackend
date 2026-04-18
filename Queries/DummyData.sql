-- Script para popular la base de datos con dummy data de prueba
-- Este script incluye datos para contactos, eventos, obsequios y reportes

-- ============================================================
-- LIMPIAR DATOS EXISTENTES (opcional, comentar si no se necesita)
-- ============================================================
-- DELETE FROM public.reportes;
-- DELETE FROM public.obsequios;
-- DELETE FROM public.eventos;
-- DELETE FROM public.asientos;
-- DELETE FROM public.contactos;

-- ============================================================
-- INSERTAR CONTACTOS
-- ============================================================
INSERT INTO public.contactos (nombres, apellido_paterno, apellido_materno, correo, telefono_celular, telefono_oficina, fecha_nacimiento, nota, path_imagen)
VALUES
    ('Juan', 'García', 'López', 'juan.garcia@example.com', '5551234567', '5559876543', '1985-03-15', 'Contacto importante', NULL),
    ('María', 'Rodríguez', 'Martínez', 'maria.rodriguez@example.com', '5552345678', '5558765432', '1990-07-22', 'Coordinadora de eventos', NULL),
    ('Carlos', 'Fernández', 'González', 'carlos.fernandez@example.com', '5553456789', '5557654321', '1988-11-10', 'Asistente administrativo', NULL),
    ('Ana', 'López', 'Pérez', 'ana.lopez@example.com', '5554567890', '5556543210', '1992-05-18', 'Especialista en comunicación', NULL),
    ('Roberto', 'Martínez', 'Hernández', 'roberto.martinez@example.com', '5555678901', '5555432109', '1980-01-25', 'Gerente de proyecto', NULL),
    ('Patricia', 'Gómez', 'Ramírez', 'patricia.gomez@example.com', '5556789012', '5554321098', '1995-09-30', 'Analista de datos', NULL),
    ('Luis', 'Sánchez', 'López', 'luis.sanchez@example.com', '5557890123', '5553210987', '1987-04-12', 'Técnico en sistemas', NULL),
    ('Laura', 'Jiménez', 'García', 'laura.jimenez@example.com', '5558901234', '5552109876', '1993-12-08', 'Directora de área', NULL),
    ('Diego', 'Moreno', 'Ruiz', 'diego.moreno@example.com', '5559012345', '5551098765', '1989-06-20', 'Especialista en RH', NULL),
    ('Sofia', 'Castro', 'Díaz', 'sofia.castro@example.com', '5550123456', '5550987654', '1994-08-14', 'Coordinadora administrativa', NULL);

-- ============================================================
-- INSERTAR EVENTOS
-- ============================================================
INSERT INTO public.eventos (logo, croquis, nombre, nombre_ingles, nombre_lugar, nombre_responsable, descripcion, fecha_evento, tipo, importancia)
VALUES
    ('logo1.png', 'croquis1.png', 'Conferencia de Tecnología', 'Technology Conference', 'Auditorio Principal', 'Juan García', 'Conferencia internacional sobre innovación tecnológica', '2024-12-15', 'Conferencia', 'Crítico'),
    ('logo2.png', 'croquis2.png', 'Encuentro de Directivos', 'Executive Meeting', 'Sala de Juntas', 'María Rodríguez', 'Reunión trimestral con directivos de la organización', '2024-12-10', 'Reunión', 'Alto'),
    ('logo3.png', NULL, 'Capacitación Administrativa', 'Administrative Training', 'Centro de Capacitación', 'Carlos Fernández', 'Sesión de capacitación en procesos administrativos', '2024-11-28', 'Capacitación', 'Medio'),
    ('logo4.png', 'croquis4.png', 'Gala de Premiación', 'Awards Ceremony', 'Hotel Ejecutivo', 'Ana López', 'Ceremonia de premiación anual 2024', '2024-11-25', 'Ceremonia', 'Crítico'),
    ('logo5.png', NULL, 'Taller de Comunicación', 'Communication Workshop', 'Sala Multiusos', 'Roberto Martínez', 'Taller interactivo sobre comunicación efectiva', '2024-12-05', 'Taller', 'Medio'),
    ('logo6.png', 'croquis6.png', 'Desayuno de Trabajo', 'Working Breakfast', 'Restaurante Corporativo', 'Patricia Gómez', 'Desayuno informal para discutir proyectos', '2024-11-30', 'Desayuno', 'Bajo'),
    ('logo7.png', NULL, 'Seminario de Finanzas', 'Financial Seminar', 'Auditorio Secundario', 'Luis Sánchez', 'Seminario sobre análisis financiero y presupuesto', '2024-12-12', 'Seminario', 'Alto'),
    ('logo8.png', 'croquis8.png', 'Celebración de Aniversario', 'Anniversary Celebration', 'Patio Central', 'Laura Jiménez', 'Celebración del 20 aniversario de la institución', '2024-11-20', 'Celebración', 'Crítico'),
    ('logo9.png', NULL, 'Reunión de Equipo', 'Team Meeting', 'Sala de Videoconferencia', 'Diego Moreno', 'Sesión de seguimiento de proyectos en curso', '2024-12-01', 'Reunión', 'Medio'),
    ('logo10.png', 'croquis10.png', 'Lanzamiento de Producto', 'Product Launch', 'Teatro Auditorio', 'Sofia Castro', 'Presentación oficial del nuevo producto/servicio', '2024-12-20', 'Lanzamiento', 'Crítico');

-- ============================================================
-- INSERTAR OBSEQUIOS
-- ============================================================
INSERT INTO public.obsequios (path_imagen, fecha_emision, emisor_externo, receptor_externo, anotacion)
VALUES
    ('obsequio1.jpg', '2024-11-15', 'Empresa XYZ', NULL, 'Presente corporativo por alianza estratégica'),
    ('obsequio2.jpg', '2024-11-18', NULL, 'Juan García', 'Reconocimiento por desempeño excepcional'),
    ('obsequio3.jpg', '2024-11-20', 'Distribuidora ABC', NULL, 'Cortesía por visita a instalaciones'),
    ('obsequio4.jpg', '2024-11-22', NULL, 'María Rodríguez', 'Premio por cumplimiento de meta anual'),
    ('obsequio5.jpg', '2024-11-25', 'Proveedor 123', NULL, 'Detalle empresarial de inicio de año'),
    ('obsequio6.jpg', '2024-11-28', NULL, 'Carlos Fernández', 'Obsequio por retiro de colaborador'),
    ('obsequio7.jpg', '2024-12-01', 'Consultora DEF', NULL, 'Presente por conclusión de proyecto exitoso'),
    ('obsequio8.jpg', '2024-12-03', NULL, 'Ana López', 'Reconocimiento por iniciativa de mejora'),
    ('obsequio9.jpg', '2024-12-05', 'Agencia GHI', NULL, 'Artículo promocional con logo'),
    ('obsequio10.jpg', '2024-12-08', NULL, 'Roberto Martínez', 'Certificado y presente por formación completada');

-- ============================================================
-- INSERTAR ASIENTOS (Asistencias a Eventos)
-- ============================================================
-- Asientos para Evento 1 (Conferencia de Tecnología)
INSERT INTO public.asientos (id_evento, id_contacto, estado)
VALUES
    (1, 1, 'Confirmado'),
    (1, 2, 'Confirmado'),
    (1, 3, 'No confirmado'),
    (1, 5, 'Confirmado'),
    (1, 7, 'Ausente');

-- Asientos para Evento 2 (Encuentro de Directivos)
INSERT INTO public.asientos (id_evento, id_contacto, estado)
VALUES
    (2, 1, 'Confirmado'),
    (2, 5, 'Confirmado'),
    (2, 8, 'Confirmado'),
    (2, 9, 'No confirmado');

-- Asientos para Evento 3 (Capacitación)
INSERT INTO public.asientos (id_evento, id_contacto, estado)
VALUES
    (3, 3, 'Confirmado'),
    (3, 4, 'Confirmado'),
    (3, 6, 'Confirmado'),
    (3, 9, 'Confirmado'),
    (3, 10, 'Confirmado');

-- Asientos para Evento 4 (Gala de Premiación)
INSERT INTO public.asientos (id_evento, id_contacto, estado)
VALUES
    (4, 1, 'Confirmado'),
    (4, 2, 'Confirmado'),
    (4, 5, 'Confirmado'),
    (4, 8, 'Confirmado'),
    (4, 10, 'Confirmado');

-- Asientos para Evento 5 (Taller)
INSERT INTO public.asientos (id_evento, id_contacto, estado)
VALUES
    (5, 2, 'Confirmado'),
    (5, 4, 'Confirmado'),
    (5, 6, 'Ausente'),
    (5, 9, 'Confirmado');

-- Asientos para Evento 6 (Desayuno)
INSERT INTO public.asientos (id_evento, id_contacto, estado)
VALUES
    (6, 1, 'Confirmado'),
    (6, 5, 'Confirmado'),
    (6, 7, 'Confirmado');

-- Asientos para Evento 7 (Seminario)
INSERT INTO public.asientos (id_evento, id_contacto, estado)
VALUES
    (7, 3, 'Confirmado'),
    (7, 6, 'Confirmado'),
    (7, 8, 'Confirmado'),
    (7, 9, 'Confirmado');

-- Asientos para Evento 8 (Celebración)
INSERT INTO public.asientos (id_evento, id_contacto, estado)
VALUES
    (8, 1, 'Confirmado'),
    (8, 2, 'Confirmado'),
    (8, 3, 'Confirmado'),
    (8, 4, 'Confirmado'),
    (8, 5, 'Confirmado'),
    (8, 6, 'Confirmado'),
    (8, 7, 'Confirmado'),
    (8, 8, 'Confirmado'),
    (8, 9, 'Confirmado'),
    (8, 10, 'Confirmado');

-- Asientos para Evento 9 (Reunión de Equipo)
INSERT INTO public.asientos (id_evento, id_contacto, estado)
VALUES
    (9, 1, 'Confirmado'),
    (9, 2, 'Confirmado'),
    (9, 3, 'Confirmado'),
    (9, 4, 'Confirmado');

-- Asientos para Evento 10 (Lanzamiento)
INSERT INTO public.asientos (id_evento, id_contacto, estado)
VALUES
    (10, 1, 'Confirmado'),
    (10, 2, 'Confirmado'),
    (10, 5, 'Confirmado'),
    (10, 8, 'Confirmado'),
    (10, 10, 'Confirmado');

-- ============================================================
-- INSERTAR REPORTES DE CUMPLEAÑOS
-- ============================================================
INSERT INTO public.reportes (nombre, tipo, contenido, creado_en)
VALUES
    ('Reporte Cumpleaños Marzo', 'cumpleanos', '{"tipo":"cumpleanos","monthSelected":true,"month":3}', NOW()),
    ('Reporte Cumpleaños Mayo-Julio', 'cumpleanos', '{"tipo":"cumpleanos","monthSelected":false,"month":null}', NOW()),
    ('Reporte Cumpleaños Junio', 'cumpleanos', '{"tipo":"cumpleanos","monthSelected":true,"month":6}', NOW());

-- ============================================================
-- INSERTAR REPORTES DE EVENTOS
-- ============================================================
INSERT INTO public.reportes (nombre, tipo, contenido, creado_en)
VALUES
    ('Eventos Críticos', 'Eventos', '{"tipo":"Eventos","eventoTipo":"","eventoImportancia":"Crítico","mesEvento":12}', NOW()),
    ('Conferencias 2024', 'Eventos', '{"tipo":"Eventos","eventoTipo":"Conferencia","eventoImportancia":"","mesEvento":12}', NOW()),
    ('Eventos Importantes Diciembre', 'Eventos', '{"tipo":"Eventos","eventoTipo":"","eventoImportancia":"Alto","mesEvento":12}', NOW()),
    ('Todas las Capacitaciones', 'Eventos', '{"tipo":"Eventos","eventoTipo":"Capacitación","eventoImportancia":"","mesEvento":11}', NOW());

-- ============================================================
-- INSERTAR REPORTES DE OBSEQUIOS
-- ============================================================
INSERT INTO public.reportes (nombre, tipo, contenido, creado_en)
VALUES
    ('Obsequios Recibidos Noviembre', 'Obsequios', '{"tipo":"Obsequios","obsequioTipo":"","obsequioEmisor":"Empresa","obsequioReceptor":"","mesObsequio":11}', NOW()),
    ('Obsequios de Reconocimiento', 'Obsequios', '{"tipo":"Obsequios","obsequioTipo":"Reconocimiento","obsequioEmisor":"","obsequioReceptor":"","mesObsequio":12}', NOW()),
    ('Todos los Obsequios Diciembre', 'Obsequios', '{"tipo":"Obsequios","obsequioTipo":"","obsequioEmisor":"","obsequioReceptor":"","mesObsequio":12}', NOW()),
    ('Obsequios de Empresa XYZ', 'Obsequios', '{"tipo":"Obsequios","obsequioTipo":"","obsequioEmisor":"Empresa XYZ","obsequioReceptor":"","mesObsequio":11}', NOW());

-- ============================================================
-- Fin del script
-- ============================================================
-- Resumen: 
-- - 10 Contactos insertados
-- - 10 Eventos insertados
-- - 10 Obsequios insertados
-- - 48 Asientos (asistencias a eventos) insertados
-- - 3 Reportes de Cumpleaños insertados
-- - 4 Reportes de Eventos insertados
-- - 4 Reportes de Obsequios insertados
-- Total: 79 registros de dummy data listos para pruebas
