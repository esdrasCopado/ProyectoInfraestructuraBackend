-- Insertar asientos usando los IDs de contactos actuales (86-95) y eventos 1-10
-- tipo: 0=FILA, 1=RECTANGULAR, 2=REDONDA
-- estado: 0=LIBRE, 1=OCUPADO, 2=RESERVADO

-- Evento 1
INSERT INTO public.asientos (numero, estado, id_contacto, color, tipo, id_evento)
VALUES
    (101, 1, 86, '#FF6B6B', 0, 1),
    (102, 1, 87, '#FF6B6B', 0, 1),
    (103, 0, 88, '#FF6B6B', 0, 1),
    (104, 1, 89, '#FF6B6B', 0, 1),
    (105, 2, 90, '#FF6B6B', 0, 1);

-- Evento 2
INSERT INTO public.asientos (numero, estado, id_contacto, color, tipo, id_evento)
VALUES
    (201, 1, 86, '#4ECDC4', 1, 2),
    (202, 1, 90, '#4ECDC4', 1, 2),
    (203, 1, 93, '#4ECDC4', 1, 2),
    (204, 0, 94, '#4ECDC4', 1, 2);

-- Evento 3
INSERT INTO public.asientos (numero, estado, id_contacto, color, tipo, id_evento)
VALUES
    (301, 1, 88, '#95E1D3', 0, 3),
    (302, 1, 89, '#95E1D3', 0, 3),
    (303, 1, 91, '#95E1D3', 0, 3),
    (304, 1, 94, '#95E1D3', 0, 3),
    (305, 1, 95, '#95E1D3', 0, 3);

-- Evento 4
INSERT INTO public.asientos (numero, estado, id_contacto, color, tipo, id_evento)
VALUES
    (401, 1, 86, '#F38181', 2, 4),
    (402, 1, 87, '#F38181', 2, 4),
    (403, 1, 90, '#F38181', 2, 4),
    (404, 1, 93, '#F38181', 2, 4),
    (405, 1, 95, '#F38181', 2, 4);

-- Evento 5
INSERT INTO public.asientos (numero, estado, id_contacto, color, tipo, id_evento)
VALUES
    (501, 1, 87, '#AA96DA', 0, 5),
    (502, 1, 89, '#AA96DA', 0, 5),
    (503, 2, 91, '#AA96DA', 0, 5),
    (504, 1, 94, '#AA96DA', 0, 5);

-- Evento 6
INSERT INTO public.asientos (numero, estado, id_contacto, color, tipo, id_evento)
VALUES
    (601, 1, 86, '#FCBAD3', 1, 6),
    (602, 1, 90, '#FCBAD3', 1, 6),
    (603, 1, 92, '#FCBAD3', 1, 6);

-- Evento 7
INSERT INTO public.asientos (numero, estado, id_contacto, color, tipo, id_evento)
VALUES
    (701, 1, 88, '#FFFFD2', 0, 7),
    (702, 1, 91, '#FFFFD2', 0, 7),
    (703, 1, 93, '#FFFFD2', 0, 7),
    (704, 1, 94, '#FFFFD2', 0, 7);

-- Evento 8
INSERT INTO public.asientos (numero, estado, id_contacto, color, tipo, id_evento)
VALUES
    (801, 1, 86, '#FFA07A', 2, 8),
    (802, 1, 87, '#FFA07A', 2, 8),
    (803, 1, 88, '#FFA07A', 2, 8),
    (804, 1, 89, '#FFA07A', 2, 8),
    (805, 1, 90, '#FFA07A', 2, 8),
    (806, 1, 91, '#FFA07A', 2, 8),
    (807, 1, 92, '#FFA07A', 2, 8),
    (808, 1, 93, '#FFA07A', 2, 8),
    (809, 1, 94, '#FFA07A', 2, 8),
    (810, 1, 95, '#FFA07A', 2, 8);

-- Evento 9
INSERT INTO public.asientos (numero, estado, id_contacto, color, tipo, id_evento)
VALUES
    (901, 1, 86, '#B19CD9', 1, 9),
    (902, 1, 87, '#B19CD9', 1, 9),
    (903, 1, 88, '#B19CD9', 1, 9),
    (904, 1, 89, '#B19CD9', 1, 9);

-- Evento 10
INSERT INTO public.asientos (numero, estado, id_contacto, color, tipo, id_evento)
VALUES
    (1001, 1, 86, '#FFB347', 0, 10),
    (1002, 1, 87, '#FFB347', 0, 10),
    (1003, 1, 90, '#FFB347', 0, 10),
    (1004, 1, 93, '#FFB347', 0, 10),
    (1005, 1, 95, '#FFB347', 0, 10);

-- Confirmación
SELECT (SELECT COUNT(*) FROM public.asientos) AS asientos_total;
