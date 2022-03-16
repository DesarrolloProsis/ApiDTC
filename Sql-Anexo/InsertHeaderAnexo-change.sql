USE [BitacoraProsisaNEXO]
GO
/****** Object:  StoredProcedure [dbo].[InsertHeaderAnexo]    Script Date: 15/03/2022 11:57:01 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[InsertHeaderAnexo]
	@referenceDTC NVARCHAR(20),
	@referenceAnexoAnterior NVARCHAR(20),
	@referenceAnexo NVARCHAR(20),
	@fechaApertura DATETIME,
	@fechaCierre DATETIME,
	@folioOficio NVARCHAR(50),
	@fechaOficioInicio DATETIME,
	--@fechaOficioFin DATETIME,	
	@solicitud NVARCHAR(50),
	@fechaSolicitudInicio DATETIME,
	--@fechaSolicitudFin DATETIME,
	@supervisorId INT,
	@testigo1 INT,
	@testigo2 INT,
	@tipoAnexo CHAR,
	@isSubVersion BIT
AS
BEGIN TRY
	IF @tipoAnexo = 'A'
	BEGIN
		INSERT INTO AnexosDTC(DTCReference, AnexoReference, FechaApertura, FechaCierre, Solicitud, FechaSolicitudInicio, FolioOficio, FechaOficioInicio,Testigo1Id, Testigo2Id, SupervisorId, Activo, TipoAnexo, FechaUltimoCambio, IsSubVersion) 
		VALUES(@referenceDTC, @referenceAnexo, @fechaApertura, @fechaCierre, @solicitud, @fechaSolicitudInicio, @folioOficio, @fechaOficioInicio, @testigo1, @testigo2, @supervisorId, 1, 'A', GETDATE(), @isSubVersion)	
		
		IF @@ROWCOUNT = 1
		BEGIN 
			IF (@referenceAnexoAnterior IS NOT NULL)
			BEGIN
				UPDATE RequestedComponents SET UseInAnexo = 0 WHERE RequestedComponentId IN(SELECT ComponentDTCId FROM ComponentAnexo WHERE AnexoId = @referenceAnexoAnterior)				
			END
			SELECT 'NEWANEXO!' As SqlResult, 'Anexo con Id' + @referenceAnexo + 'agregado con exito' as SqlMessage
		END		
	END
	ELSE
	BEGIN	
		INSERT INTO AnexosDTC(DTCReference, AnexoReference, FechaApertura, FechaCierre, Solicitud, FechaSolicitudInicio, FolioOficio, FechaOficioInicio, Testigo1Id, Testigo2Id, SupervisorId, Activo, TipoAnexo, FechaUltimoCambio, IsSubVersion) 
		VALUES(@referenceDTC, @referenceAnexo, @fechaApertura, @fechaCierre, @solicitud, @fechaSolicitudInicio, @folioOficio, @fechaOficioInicio,@testigo1, @testigo2, @supervisorId, 1, 'B', GETDATE(), @isSubVersion)	

		IF @@ROWCOUNT = 1
		BEGIN
			IF (@referenceAnexoAnterior IS NOT NULL)
			BEGIN
				UPDATE RequestedComponents SET UseInAnexo = 0 WHERE RequestedComponentId IN(SELECT ComponentDTCId FROM ComponentAnexo WHERE AnexoId = @referenceAnexoAnterior)				
			END
			SELECT 'NEWANEXO!' As SqlResult, 'Anexo con Id' + @referenceAnexo + 'agregado con exito' as SqlMessage
		END		
	END	
END TRY
BEGIN CATCH
-- Error
PRINT(CAST(@@ERROR AS VARCHAR))
	SELECT NULL AS SqlResult, 'Línea: ' + CAST(ERROR_LINE() AS VARCHAR) + ' ' + CAST(@@ERROR AS VARCHAR) AS SqlMessage
END CATCH
