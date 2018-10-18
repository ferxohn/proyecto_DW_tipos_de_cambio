USE [tipo_cambio]
GO

CREATE PROCEDURE delete_cambio

	@fecha date,
	@compra money,
	@venta money
AS
	DELETE FROM [cambio] WHERE fecha = @fecha;
	DELETE FROM [cambio] WHERE compra = @compra;
	DELETE FROM [cambio] WHERE venta = @venta;

GO