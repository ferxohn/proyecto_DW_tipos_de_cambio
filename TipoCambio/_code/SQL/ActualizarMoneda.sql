USE [tipo_cambio]
GO

CREATE PROCEDURE update_moneda

	@moneda_a_cambiar varchar(30),
	@pais_a_cambiar varchar(30),
	@moneda varchar(30),
	@pais varchar(30)
AS

	UPDATE [moneda] SET pais = @moneda WHERE pais = @moneda_a_cambiar;
	UPDATE [moneda] SET moneda = @pais WHERE moneda= @pais_a_cambiar;

GO