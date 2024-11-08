CREATE DATABASE formulario;
USE formulario;

CREATE TABLE datos (
    nombre VARCHAR(50) PRIMARY KEY,        
    temperatura DOUBLE,
    estado_servomotor VARCHAR(10),
    fecha DATETIME DEFAULT CURRENT_TIMESTAMP
);