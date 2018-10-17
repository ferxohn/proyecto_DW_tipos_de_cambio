USE [tipo_cambio]
GO

/****** Object:  StoredProcedure [dbo].[insert_moneda]    Script Date: 16/10/2018 20:40:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- Este procedimiento permite insertar un nuevo tipo de moneda en la tabla monedas.
CREATE PROCEDURE [dbo].[insert_moneda]   
    -- Variables de entrada.
	@moneda varchar(3),   
    @pais varchar(100)   
AS   
	-- Se inserta el valor de la moneda en la tabla. Si existe, se actualiza.
	INSERT INTO monedas(moneda, pais, usuario_alta) VALUES (@moneda, @pais, SYSTEM_USER);

	-- NOTA: HACE FALTA LA VERIFICACION Y ACTUALIZACION DE LA MONEDA EN LA TABLA. --
GO

