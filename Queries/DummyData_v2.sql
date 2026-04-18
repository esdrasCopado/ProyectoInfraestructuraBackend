-- Script correcto para popular la base de datos con dummy data
-- Estructura basada en los modelos de EF Core reales

-- ============================================================
-- INSERTAR CONTACTOS (10 registros)
-- ============================================================
INSERT INTO public.contactos (nombres, apellido_paterno, apellido_materno, correo, telefono_celular, telefono_oficina, fecha_nacimiento)
VALUES
    ('Juan', 'García', 'López', 'juan.garcia@example.com', '5551234567', '5559876543', '1985-03-15'),
    ('María', 'Rodríguez', 'Martínez', 'maria.rodriguez@example.com', '5552345678', '5558765432', '1990-07-22'),
    ('Carlos', 'Fernández', 'González', 'carlos.fernandez@example.com', '5553456789', '5557654321', '1988-11-10'),
    ('Ana', 'López', 'Pérez', 'ana.lopez@example.com', '5554567890', '5556543210', '1992-05-18'),
    ('Roberto', 'Martínez', 'Hernández', 'roberto.martinez@example.com', '5555678901', '5555432109', '1980-01-25'),
    ('Patricia', 'Gómez', 'Ramírez', 'patricia.gomez@example.com', '5556789012', '5554321098', '1995-09-30'),
    ('Luis', 'Sánchez', 'López', 'luis.sanchez@example.com', '5557890123', '5553210987', '1987-04-12'),
    ('Laura', 'Jiménez', 'García', 'laura.jimenez@example.com', '5558901234', '5552109876', '1993-12-08'),
    ('Diego', 'Moreno', 'Ruiz', 'diego.moreno@example.com', '5559012345', '5551098765', '1989-06-20'),
    ('Sofia', 'Castro', 'Díaz', 'sofia.castro@example.com', '5550123456', '5550987654', '1994-08-14');

-- ============================================================
-- INSERTAR EVENTOS (10 registros)
-- tipo: 0=REUNION, 1=CONFERENCIA, 2=VISITA_OFICIAL
-- importancia: 0=ALTO, 1=MEDIO, 2=BAJO
-- ============================================================
INSERT INTO public.eventos (logo, croquis, nombre, nombre_ingles, nombre_lugar, nombre_responsable, descripcion, fecha_evento, tipo, importancia)
VALUES
    ('logo1.png', 'croquis1.png', 'Conferencia de Tecnología', 'Technology Conference', 'Auditorio Principal', 'Juan García', 'Conferencia internacional sobre innovación tecnológica', '2024-12-15', 1, 0),
    ('logo2.png', 'croquis2.png', 'Encuentro de Directivos', 'Executive Meeting', 'Sala de Juntas', 'María Rodríguez', 'Reunión trimestral con directivos de la organización', '2024-12-10', 0, 0),
    ('logo3.png', NULL, 'Capacitación Administrativa', 'Administrative Training', 'Centro de Capacitación', 'Carlos Fernández', 'Sesión de capacitación en procesos administrativos', '2024-11-28', 0, 1),
    ('logo4.png', 'croquis4.png', 'Gala de Premiación', 'Awards Ceremony', 'Hotel Ejecutivo', 'Ana López', 'Ceremonia de premiación anual 2024', '2024-11-25', 0, 0),
    ('logo5.png', NULL, 'Taller de Comunicación', 'Communication Workshop', 'Sala Multiusos', 'Roberto Martínez', 'Taller interactivo sobre comunicación efectiva', '2024-12-05', 0, 1),
    ('logo6.png', 'croquis6.png', 'Desayuno de Trabajo', 'Working Breakfast', 'Restaurante Corporativo', 'Patricia Gómez', 'Desayuno informal para discutir proyectos', '2024-11-30', 0, 2),
    ('logo7.png', NULL, 'Seminario de Finanzas', 'Financial Seminar', 'Auditorio Secundario', 'Luis Sánchez', 'Seminario sobre análisis financiero y presupuesto', '2024-12-12', 1, 0),
    ('logo8.png', 'croquis8.png', 'Celebración de Aniversario', 'Anniversary Celebration', 'Patio Central', 'Laura Jiménez', 'Celebración del 20 aniversario de la institución', '2024-11-20', 0, 0),
    ('logo9.png', NULL, 'Reunión de Equipo', 'Team Meeting', 'Sala de Videoconferencia', 'Diego Moreno', 'Sesión de seguimiento de proyectos en curso', '2024-12-01', 0, 1),
    ('logo10.png', 'croquis10.png', 'Lanzamiento de Producto', 'Product Launch', 'Teatro Auditorio', 'Sofia Castro', 'Presentación oficial del nuevo producto/servicio', '2024-12-20', 1, 0);

-- ============================================================
-- INSERTAR OBSEQUIOS (10 registros)
-- ============================================================
INSERT INTO public.obsequios (path_imagen, fecha_emision, emisor_externo, anotacion)
VALUES
    ('obsequio1.jpg', '2024-11-15', 'Empresa XYZ', 'Presente corporativo por alianza estratégica'),
    ('obsequio2.jpg', '2024-11-18', 'Interna', 'Reconocimiento por desempeño excepcional'),
    ('obsequio3.jpg', '2024-11-20', 'Distribuidora ABC', 'Cortesía por visita a instalaciones'),
    ('obsequio4.jpg', '2024-11-22', 'Interna', 'Premio por cumplimiento de meta anual'),
    ('obsequio5.jpg', '2024-11-25', 'Proveedor 123', 'Detalle empresarial de inicio de año'),
    ('obsequio6.jpg', '2024-11-28', 'Interna', 'Obsequio por retiro de colaborador'),
    ('obsequio7.jpg', '2024-12-01', 'Consultora DEF', 'Presente por conclusión de proyecto exitoso'),
    ('obsequio8.jpg', '2024-12-03', 'Interna', 'Reconocimiento por iniciativa de mejora'),
    ('obsequio9.jpg', '2024-12-05', 'Agencia GHI', 'Artículo promocional con logo'),
    ('obsequio10.jpg', '2024-12-08', 'Interna', 'Certificado y presente por formación completada');

-- ============================================================
-- INSERTAR ASIENTOS (Registros en la tabla de distribución de mesas/eventos)
-- tipo: 0=FILA, 1=RECTANGULAR, 2=REDONDA
-- estado: 0=LIBRE, 1=OCUPADO, 2=RESERVADO
-- ============================================================

-- Asientos para Evento 1 (Conferencia de Tecnología)
INSERT INTO public.asientos (numero, estado, id_contacto, color, tipo, id_evento)
VALUES
    (101, 1, 1, '#FF6B6B', 0, 1),
    (102, 1, 2, '#FF6B6B', 0, 1),
    (103, 0, 3, '#FF6B6B', 0, 1),
    (104, 1, 5, '#FF6B6B', 0, 1),
    (105, 2, 7, '#FF6B6B', 0, 1);

-- Asientos para Evento 2 (Encuentro de Directivos)
INSERT INTO public.asientos (numero, estado, id_contacto, color, tipo, id_evento)
VALUES
    (201, 1, 1, '#4ECDC4', 1, 2),
    (202, 1, 5, '#4ECDC4', 1, 2),
    (203, 1, 8, '#4ECDC4', 1, 2),
    (204, 0, 9, '#4ECDC4', 1, 2);

-- Asientos para Evento 3 (Capacitación)
INSERT INTO public.asientos (numero, estado, id_contacto, color, tipo, id_evento)
VALUES
    (301, 1, 3, '#95E1D3', 0, 3),
    (302, 1, 4, '#95E1D3', 0, 3),
    (303, 1, 6, '#95E1D3', 0, 3),
    (304, 1, 9, '#95E1D3', 0, 3),
    (305, 1, 10, '#95E1D3', 0, 3);

-- Asientos para Evento 4 (Gala de Premiación)
INSERT INTO public.asientos (numero, estado, id_contacto, color, tipo, id_evento)
VALUES
    (401, 1, 1, '#F38181', 2, 4),
    (402, 1, 2, '#F38181', 2, 4),
    (403, 1, 5, '#F38181', 2, 4),
    (404, 1, 8, '#F38181', 2, 4),
    (405, 1, 10, '#F38181', 2, 4);

-- Asientos para Evento 5 (Taller)
INSERT INTO public.asientos (numero, estado, id_contacto, color, tipo, id_evento)
VALUES
    (501, 1, 2, '#AA96DA', 0, 5),
    (502, 1, 4, '#AA96DA', 0, 5),
    (503, 2, 6, '#AA96DA', 0, 5),
    (504, 1, 9, '#AA96DA', 0, 5);

-- Asientos para Evento 6 (Desayuno)
INSERT INTO public.asientos (numero, estado, id_contacto, color, tipo, id_evento)
VALUES
    (601, 1, 1, '#FCBAD3', 1, 6),
    (602, 1, 5, '#FCBAD3', 1, 6),
    (603, 1, 7, '#FCBAD3', 1, 6);

-- Asientos para Evento 7 (Seminario)
INSERT INTO public.asientos (numero, estado, id_contacto, color, tipo, id_evento)
VALUES
    (701, 1, 3, '#FFFFD2', 0, 7),
    (702, 1, 6, '#FFFFD2', 0, 7),
    (703, 1, 8, '#FFFFD2', 0, 7),
    (704, 1, 9, '#FFFFD2', 0, 7);

-- Asientos para Evento 8 (Celebración)
INSERT INTO public.asientos (numero, estado, id_contacto, color, tipo, id_evento)
VALUES
    (801, 1, 1, '#FFA07A', 2, 8),
    (802, 1, 2, '#FFA07A', 2, 8),
    (803, 1, 3, '#FFA07A', 2, 8),
    (804, 1, 4, '#FFA07A', 2, 8),
    (805, 1, 5, '#FFA07A', 2, 8),
    (806, 1, 6, '#FFA07A', 2, 8),
    (807, 1, 7, '#FFA07A', 2, 8),
    (808, 1, 8, '#FFA07A', 2, 8),
    (809, 1, 9, '#FFA07A', 2, 8),
    (810, 1, 10, '#FFA07A', 2, 8);

-- Asientos para Evento 9 (Reunión de Equipo)
INSERT INTO public.asientos (numero, estado, id_contacto, color, tipo, id_evento)
VALUES
    (901, 1, 1, '#B19CD9', 1, 9),
    (902, 1, 2, '#B19CD9', 1, 9),
    (903, 1, 3, '#B19CD9', 1, 9),
    (904, 1, 4, '#B19CD9', 1, 9);

-- Asientos para Evento 10 (Lanzamiento)
INSERT INTO public.asientos (numero, estado, id_contacto, color, tipo, id_evento)
VALUES
    (1001, 1, 1, '#FFB347', 0, 10),
    (1002, 1, 2, '#FFB347', 0, 10),
    (1003, 1, 5, '#FFB347', 0, 10),
    (1004, 1, 8, '#FFB347', 0, 10),
    (1005, 1, 10, '#FFB347', 0, 10);

-- ============================================================
-- INSERTAR REPORTES DE CUMPLEAÑOS
-- ============================================================
INSERT INTO public.reportes (nombre, tipo, contenido, creado_en)
VALUES
    ('Reporte Cumpleaños Marzo', 'cumpleanos', '{"tipo":"cumpleanos","monthSelected":true,"month":3}', NOW()),
    ('Reporte Cumpleaños Noviembre', 'cumpleanos', '{"tipo":"cumpleanos","monthSelected":true,"month":11}', NOW()),
    ('Reporte Cumpleaños Diciembre', 'cumpleanos', '{"tipo":"cumpleanos","monthSelected":true,"month":12}', NOW());

-- ============================================================
-- INSERTAR REPORTES DE EVENTOS
-- ============================================================
INSERT INTO public.reportes (nombre, tipo, contenido, creado_en)
VALUES
    ('Eventos Críticos', 'Eventos', '{"tipo":"Eventos","eventoTipo":"","eventoImportancia":"0","mesEvento":12}', NOW()),
    ('Conferencias 2024', 'Eventos', '{"tipo":"Eventos","eventoTipo":"Conferencia","eventoImportancia":"","mesEvento":12}', NOW()),
    ('Eventos Importantes Diciembre', 'Eventos', '{"tipo":"Eventos","eventoTipo":"","eventoImportancia":"0","mesEvento":12}', NOW()),
    ('Todas las Reuniones', 'Eventos', '{"tipo":"Eventos","eventoTipo":"Reunión","eventoImportancia":"","mesEvento":12}', NOW());

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
-- Confirmación
-- ============================================================
SELECT 
    (SELECT COUNT(*) FROM public.contactos) as contactos,
    (SELECT COUNT(*) FROM public.eventos) as eventos,
    (SELECT COUNT(*) FROM public.obsequios) as obsequios,
    (SELECT COUNT(*) FROM public.asientos) as asientos,
    (SELECT COUNT(*) FROM public.reportes) as reportes;
