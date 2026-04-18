-- Actualizar obsequios para asignar contactos emisores y receptores
-- y asegurar que cumplan con los criterios de los reportes

-- Primero, actualizar los obsequios con id_contacto_emisor y id_contacto_receptor
UPDATE public.obsequios
SET id_contacto_emisor = 86
WHERE id_obsequio = 1;

UPDATE public.obsequios
SET id_contacto_emisor = 87, id_contacto_receptor = 88
WHERE id_obsequio = 2;

UPDATE public.obsequios
SET id_contacto_emisor = 88, id_contacto_receptor = 89
WHERE id_obsequio = 3;

UPDATE public.obsequios
SET id_contacto_emisor = 89, id_contacto_receptor = 90
WHERE id_obsequio = 4;

UPDATE public.obsequios
SET id_contacto_emisor = 90, id_contacto_receptor = 91
WHERE id_obsequio = 5;

UPDATE public.obsequios
SET id_contacto_emisor = 91, id_contacto_receptor = 92
WHERE id_obsequio = 6;

UPDATE public.obsequios
SET id_contacto_emisor = 92, id_contacto_receptor = 93
WHERE id_obsequio = 7;

UPDATE public.obsequios
SET id_contacto_emisor = 93, id_contacto_receptor = 94
WHERE id_obsequio = 8;

UPDATE public.obsequios
SET id_contacto_emisor = 94, id_contacto_receptor = 95
WHERE id_obsequio = 9;

UPDATE public.obsequios
SET id_contacto_emisor = 95, id_contacto_receptor = 86
WHERE id_obsequio = 10;

-- Confirmar los cambios
SELECT id_obsequio, id_contacto_emisor, id_contacto_receptor, anotacion, fecha_emision
FROM public.obsequios;
