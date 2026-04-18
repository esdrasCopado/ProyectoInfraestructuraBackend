-- Primero eliminamos la tabla contactos que tiene dependencia de caracteristicas
DROP TABLE IF EXISTS public.obsequios;
DROP TABLE IF EXISTS public.contactos;
DROP TABLE IF EXISTS public.usuarios;

-- Luego eliminamos caracteristicas que depende de cargos, instituciones y direcciones
DROP TABLE IF EXISTS public.caracteristicas;

-- Ahora podemos eliminar las tablas que no tienen dependencias
DROP TABLE IF EXISTS public.cargos;
DROP TABLE IF EXISTS public.dependencias;
DROP TABLE IF EXISTS public.direcciones;
DROP TABLE IF EXISTS public.puestos;
DROP TABLE IF EXISTS public.unidad_responsables;
DROP TABLE IF EXISTS public.redes_sociales;
DROP TABLE IF EXISTS public.categorias;