use ruraldb;
CREATE TABLE CLIENTE (
    Identificador INTEGER NOT NULL AUTO_INCREMENT,
    Nombre VARCHAR(32),
    Apellidos VARCHAR(64),
    Teléfono INTEGER,
    CodigoPostal INTEGER,
    Email VARCHAR(64),
    PRIMARY KEY (Identificador)
 );
 
 CREATE TABLE RESERVA (
    Identificador INTEGER NOT NULL AUTO_INCREMENT,
    Nombre VARCHAR(32) NOT NULL,
    Apellidos VARCHAR(64),
    Telefono VARCHAR(16) NOT NULL,
    CodigoPostal INTEGER,
    Email VARCHAR(64),
    Tarifa VARCHAR(32),
    Apartamento INTEGER NOT NULL,
    Personas INTEGER NOT NULL,
    Checkin DATE NOT NULL,
    Checkout DATE NOT NULL,
    Notas TINYTEXT,
    Importe FLOAT,
    NumeroTarjeta VARCHAR(16),
    FechaCaducidadTarjeta VARCHAR(10),
	Pagado BOOL,
    FacturaAsociada INT,
    PRIMARY KEY (Identificador)
 );
 
 CREATE TABLE APARTAMENTO (
    Identificador INTEGER NOT NULL AUTO_INCREMENT,
    Nombre VARCHAR(32),
    CapacidadMax INT,
    CapacidadBase INT,
    PrecioBase FLOAT,
    PRIMARY KEY (Identificador)
 );
 
 CREATE TABLE TARIFA (
	Identificador INTEGER NOT NULL AUTO_INCREMENT,
    Dia DATE,
    Apartamento INT,    
    Precio FLOAT,
    PRIMARY KEY (Identificador)
 );
 
 CREATE TABLE FACTURA (
	NumeroFactura INTEGER NOT NULL AUTO_INCREMENT,
    NombreyApellidos VARCHAR(128),
    DNI VARCHAR(10),
    Calle VARCHAR(128),
    CP INT,
    Fecha VARCHAR(18), 
	Observaciones TINYTEXT,
    PRIMARY KEY (NumeroFactura)
 );
 
 
INSERT INTO APARTAMENTO (Nombre, CapacidadMax, CapacidadBase, PrecioBase) VALUES ('Apartamento 1', '6', '2', '120');
INSERT INTO APARTAMENTO (Nombre, CapacidadMax, CapacidadBase, PrecioBase) VALUES ('Apartamento 2', '4', '1', '100');
INSERT INTO APARTAMENTO (Nombre, CapacidadMax, CapacidadBase, PrecioBase) VALUES ('Apartamento 3', '6', '2', '120');
INSERT INTO APARTAMENTO (Nombre, CapacidadMax, CapacidadBase, PrecioBase) VALUES ('Apartamento 4', '4', '1', '100');
INSERT INTO APARTAMENTO (Nombre, CapacidadMax, CapacidadBase, PrecioBase) VALUES ('Apartamento 5', '6', '2', '120');

INSERT INTO APARTAMENTO (Nombre, CapacidadMax, CapacidadBase, PrecioBase) VALUES ('Estudio A', '2', '1', '80');
INSERT INTO APARTAMENTO (Nombre, CapacidadMax, CapacidadBase, PrecioBase) VALUES ('Estudio B', '2', '1', '80');
INSERT INTO APARTAMENTO (Nombre, CapacidadMax, CapacidadBase, PrecioBase) VALUES ('Estudio C', '2', '1', '80');
INSERT INTO APARTAMENTO (Nombre, CapacidadMax, CapacidadBase, PrecioBase) VALUES ('Estudio D', '2', '1', '80');
INSERT INTO APARTAMENTO (Nombre, CapacidadMax, CapacidadBase, PrecioBase) VALUES ('Estudio E', '2', '1', '80');
INSERT INTO APARTAMENTO (Nombre, CapacidadMax, CapacidadBase, PrecioBase) VALUES ('Estudio F', '2', '1', '80');
INSERT INTO APARTAMENTO (Nombre, CapacidadMax, CapacidadBase, PrecioBase) VALUES ('Estudio G', '2', '1', '80');
INSERT INTO APARTAMENTO (Nombre, CapacidadMax, CapacidadBase, PrecioBase) VALUES ('Estudio H', '2', '1', '80');
INSERT INTO APARTAMENTO (Nombre, CapacidadMax, CapacidadBase, PrecioBase) VALUES ('Estudio I', '2', '1', '80');
 

