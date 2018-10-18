USE [tipo_cambio]
GO

CREATE PROCEDURE delete_moneda

	@moneda varchar(30)
AS

	DELETE FROM [moneda] WHERE moneda = @moneda;
	
GO