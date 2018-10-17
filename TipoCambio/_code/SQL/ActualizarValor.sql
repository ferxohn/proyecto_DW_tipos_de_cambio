USE [tipo_cambio]
GO

CREATE PROCEDURE update_cambio

	@fecha date,
	@fehca_a_cambiar date,
	@compra money,
	@compra_a_cambiar money,
	@venta money,
	@venta_a_cambiar money
AS

	UPDATE [cambio] SET fecha = @fecha WHERE fecha = @fecha_a_cambiar;
	UPDATE [cambio] SET compra = @compra WHERE compra = @compra_a_cambiar;
	UPDATE [cambio] SET venta = @venta WHERE venta = @venta_a_cambiar;

GO