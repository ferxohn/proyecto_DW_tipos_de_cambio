USE [tipo_cambio]
GO

/****** Object:  Table [dbo].[monedas]    Script Date: 16/10/2018 20:43:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[monedas](
	[id_moneda] [int] IDENTITY(1,1) NOT NULL,
	[moneda] [varchar](3) NOT NULL,
	[pais] [varchar](100) NOT NULL,
	[usuario_alta] [varchar](30) NOT NULL,
 CONSTRAINT [PK_moneda] PRIMARY KEY CLUSTERED 
(
	[id_moneda] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

