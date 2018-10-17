USE [tipo_cambio]
GO

/****** Object:  StoredProcedure [dbo].[insert_valor]    Script Date: 16/10/2018 20:41:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- Este procedimiento permite insertar un nuevo valor de tipo de cambio en la tabla valores.
CREATE PROCEDURE [dbo].[insert_valor]   
    -- Variables de entrada.
	@fecha date,
    @compra money,
    @venta money,   
    @moneda varchar(3)
AS   
	-- Se verifica el id de la moneda ingresada.
	DECLARE @id_moneda int;  
	SET @id_moneda = (select id_moneda from monedas where moneda = @moneda);

	-- Se inserta el valor del tipo de cambio en la tabla. Si existe, se actualiza.
	INSERT INTO valores(fecha, compra, venta, usuario_alta, id_moneda)
	VALUES (@fecha, @compra, @venta, SYSTEM_USER, @id_moneda);

	-- NOTA: HACE FALTA LA VERIFICACION Y ACTUALIZACION DEL TIPO DE CAMBIO EN LA TABLA. --
GO

