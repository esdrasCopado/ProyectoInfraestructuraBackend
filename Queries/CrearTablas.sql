CREATE TABLE public."puestos"
(
    id_puesto bigserial NOT NULL,
    nombre character varying(50) NOT NULL,
    PRIMARY KEY (id_puesto),
	UNIQUE(nombre)
);

ALTER TABLE IF EXISTS public."puestos"
    OWNER to postgres;

CREATE TABLE public."redes_sociales"
(
    id_red bigserial NOT NULL,
    facebook text DEFAULT 'No tiene',
	instagram text DEFAULT 'No tiene',
	threads text DEFAULT 'No tiene',
	twitter text DEFAULT 'No tiene',
    PRIMARY KEY (id_red)
);

ALTER TABLE IF EXISTS public."redes_sociales"
    OWNER to postgres;

CREATE TABLE public."unidad_responsables"
(
    id_unidad bigserial NOT NULL,
    nombre character varying(80) NOT NULL,
    PRIMARY KEY (id_unidad),
	UNIQUE(nombre)
);

ALTER TABLE IF EXISTS public."unidad_responsables"
    OWNER to postgres;

CREATE TABLE public."dependencias"
(
    id_dependencia bigserial NOT NULL,
    nombre character varying(100) NOT NULL,
    PRIMARY KEY (id_dependencia),
	UNIQUE(nombre)
);

ALTER TABLE IF EXISTS public."dependencias"
    OWNER to postgres;

CREATE TABLE public."cargos"
(
    id_cargo bigserial NOT NULL,
    nombre character varying(50) NOT NULL,
    PRIMARY KEY (id_cargo),
	UNIQUE(nombre)
);

ALTER TABLE IF EXISTS public."cargos"
    OWNER to postgres;

CREATE TABLE public."categorias"
(
    id_categoria bigserial NOT NULL,
    nombre character varying(50) NOT NULL,
    PRIMARY KEY (id_categoria),
	UNIQUE(nombre)
);

ALTER TABLE IF EXISTS public."categorias"
    OWNER to postgres;


CREATE TABLE public.direcciones
(
    id_direccion bigserial NOT NULL,
    estado character varying(50) NOT NULL,
    ciudad character varying(100) NOT NULL,
    colonia character varying(100) NOT NULL,
    calle character varying(100) NOT NULL,
    numero_exterior character varying(20) NOT NULL,
    numero_interior character varying(10) DEFAULT 'No tiene',
    PRIMARY KEY (id_direccion)
);

ALTER TABLE IF EXISTS public.direcciones
    OWNER to postgres;

CREATE TABLE public.caracteristicas
(
    id_caracteristica bigserial NOT NULL,
    id_cargo bigint NOT NULL,
    id_dependencia bigint NOT NULL,
    id_direccion bigint NOT NULL,
	id_puesto bigint NOT NULL,
	id_unidad bigint NOT NULL,
	id_red bigint NOT NULL,
	id_categoria bigint NOT NULL,
	numero_empleado character varying(6) NOT NULL,
    PRIMARY KEY (id_caracteristica),
    FOREIGN KEY (id_cargo)
        REFERENCES public.cargos (id_cargo) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID,
    FOREIGN KEY (id_direccion)
        REFERENCES public.direcciones (id_direccion) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID,
    FOREIGN KEY (id_dependencia)
        REFERENCES public.dependencias (id_dependencia) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID,
	FOREIGN KEY (id_puesto)
        REFERENCES public.puestos (id_puesto) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID,
	FOREIGN KEY (id_unidad)
        REFERENCES public.unidad_responsables (id_unidad) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID,
	FOREIGN KEY (id_red)
        REFERENCES public.redes_sociales (id_red) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID,
	FOREIGN KEY (id_categoria)
        REFERENCES public.categorias (id_categoria) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID,
	UNIQUE(numero_empleado)
);

ALTER TABLE IF EXISTS public.caracteristicas
    OWNER to postgres;

CREATE TABLE public.contactos
(
    id_contacto bigserial NOT NULL,
    nombres character varying(80) NOT NULL,
    apellido_paterno character varying(30) NOT NULL,
    apellido_materno character varying(30) NOT NULL,
    email character varying(200) NOT NULL,
    telefono_celular character varying(15) NOT NULL,
    telefono_oficina character varying(15) DEFAULT NULL,
    fecha_nacimiento date NOT NULL,
    fotografia text DEFAULT 'No posee',
    id_caracteristica bigint NOT NULL,
    PRIMARY KEY (id_contacto),
    FOREIGN KEY (id_caracteristica)
       REFERENCES public.caracteristicas (id_caracteristica) MATCH SIMPLE
       ON UPDATE NO ACTION
       ON DELETE NO ACTION
       NOT VALID
);

ALTER TABLE IF EXISTS public.contactos
    OWNER to postgres;