USE [BitacoraProsis]
GO
/****** Object:  StoredProcedure [dbo].[GetHeaderAnexo]    Script Date: 2/24/2022 10:42:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetHeaderAnexo]
	@referenceAnexo NVARCHAR(20)
AS
	DECLARE @ultimaVersionAnexo NVARCHAR(20)
	DECLARE @conteoVersion INT
	DECLARE @userId INT
	DECLARE @userName NVARCHAR(25)	
	
	SELECT @userId = UserId FROM DTCData WHERE ReferenceNumber = (SELECT ReferenceNumber FROM AnexosDTC WHERE AnexoReference = @referenceAnexo)
	SELECT @userName = U.UserName FROM DTCData D INNER JOIN DTCUsers U ON D.UserId = U.UserId WHERE D.ReferenceNumber = (SELECT ReferenceNumber FROM AnexosDTC WHERE AnexoReference = @referenceAnexo)
	SELECT @conteoVersion = COUNT(*) FROM AnexosDTC WHERE SUBSTRING(AnexoReference, 0, LEN(@referenceAnexo) + 1) = @referenceAnexo AND IsSubVersion = 1

	--SI ES MAYOR A 0 ES QUE TIENE SUBVERSION SE DEBE BUSCAR LA ULTIMA SUBVERSION
	IF @conteoVersion > 0
	BEGIN		
		SElECT TOP 1 @ultimaVersionAnexo = AnexoReference FROM AnexosDTC WHERE SUBSTRING(AnexoReference, 0, LEN(@referenceAnexo) + 1) = @referenceAnexo AND IsSubVersion = 1 ORDER BY AnexoReference DESC 	
		SELECT *, @userId AS UserId, @userName AS UserName FROM AnexosDTC WHERE AnexoReference = @ultimaVersionAnexo
	END
	ELSE 
	BEGIN
		SELECT *, @userId AS UserId, @userName AS UserName FROM AnexosDTC WHERE AnexoReference = @referenceAnexo
	END
