USE [master]
GO
/****** Object:  Database [RAGgedEdgeDemo] ******/
CREATE DATABASE [RAGgedEdgeDemo]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Learning_AI', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\RAGgedEdgeDemo.mdf' , SIZE = 1024KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Learning_AI_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\RAGgedEdgeDemo_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO

USE [RAGgedEdgeDemo]
GO
/****** Object:  UserDefinedTableType [dbo].[VectorsUdt] ******/
CREATE TYPE [dbo].[VectorsUdt] AS TABLE(
	[VectorValueId] [int] NULL,
	[VectorValue] [float] NULL
)
GO
/****** Object:  Table [dbo].[Wiki] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Wiki](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](max) NULL,
	[Content] [nvarchar](max) NULL,
	[Subject] [nvarchar](max) NULL,
 CONSTRAINT [PK__Wiki_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WikiVector] ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WikiVector](
	[WikiId] [int] NOT NULL,
	[VectorValueId] [int] NOT NULL,
	[VectorValue] [float] NOT NULL,
 CONSTRAINT [UC_WikiVector_WikiVectorValue] UNIQUE NONCLUSTERED 
(
	[WikiId] ASC,
	[VectorValueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [WikiVectorIndex] ******/
CREATE CLUSTERED COLUMNSTORE INDEX [WikiVectorIndex] ON [dbo].[WikiVector] WITH (DROP_EXISTING = OFF, COMPRESSION_DELAY = 0, DATA_COMPRESSION = COLUMNSTORE) ON [PRIMARY]
GO
ALTER TABLE [dbo].[WikiVector]  WITH NOCHECK ADD  CONSTRAINT [FK_WikiVector_Wiki] FOREIGN KEY([WikiId])
REFERENCES [dbo].[Wiki] ([Id])
GO
ALTER TABLE [dbo].[WikiVector] CHECK CONSTRAINT [FK_WikiVector_Wiki]
GO
/****** Object:  StoredProcedure [dbo].[RunWikiVectorSearch] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[RunWikiVectorSearch]
	@Vectors VectorsUdt READONLY
AS
BEGIN

	DROP TABLE IF EXISTS #SimilarityResults

	SELECT TOP 3
		wv.WikiId, 
		CosineDistance =
			SUM(qv.VectorValue * wv.VectorValue) /	-- https://github.com/Azure-Samples/azure-sql-db-openai/blob/classic/vector-embeddings/05-find-similar-articles.sql
		    (
		        SQRT(SUM(qv.VectorValue * qv.VectorValue)) 
		        * 
		        SQRT(SUM(wv.VectorValue * wv.VectorValue))
		    )
	INTO
		#SimilarityResults
	FROM 
		@Vectors AS qv
		INNER JOIN WikiVector AS wv on qv.VectorValueId = wv.VectorValueId
	GROUP BY
		wv.WikiId
	ORDER BY
		CosineDistance DESC

	SELECT
		w.Id,
		w.Title,
		w.[subject],
		w.[Content],
		r.CosineDistance
	FROM
		#SimilarityResults AS r
		INNER JOIN Wiki AS w ON w.Id = r.WikiId
	ORDER BY
		CosineDistance DESC
	
END

GO
USE [master]
GO
ALTER DATABASE [RAGgedEdgeDemo] SET  READ_WRITE 
GO
