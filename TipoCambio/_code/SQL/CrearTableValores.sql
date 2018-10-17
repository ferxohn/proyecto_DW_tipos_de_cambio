USE [tipo_cambio]
GO

/****** Object:  Table [dbo].[valores]    Script Date: 16/10/2018 20:43:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[valores](
	[id_valor] [int] IDENTITY(1,1) NOT NULL,
	[fecha] [date] NOT NULL,
	[compra] [money] NOT NULL,
	[venta] [money] NOT NULL,
	[usuario_alta] [varchar](30) NOT NULL,
	[id_moneda] [int] NOT NULL,
 CONSTRAINT [PK_cambio] PRIMARY KEY CLUSTERED 
(
	[id_valor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[valores]  WITH CHECK ADD  CONSTRAINT [FK_cambio_moneda] FOREIGN KEY([id_moneda])
REFERENCES [dbo].[monedas] ([id_moneda])
GO

ALTER TABLE [dbo].[valores] CHECK CONSTRAINT [FK_cambio_moneda]
GO

