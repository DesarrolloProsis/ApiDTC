USE [BitacoraProsisaNEXO]
GO
/****** Object:  StoredProcedure [dbo].[GetHistoricoAnexo]    Script Date: 11/03/2022 11:09:21 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetHistoricoAnexo]	
	@referenceDTC NVARCHAR(20)
AS
	SELECT A.DTCReference, A.AnexoReference, A.FechaApertura, A.FechaCierre, A.Solicitud, A.FechaSolicitudInicio, A.FechaSolicitudFin, A.FolioOficio, A.FechaOficioInicio, A.FechaOficioFin, A.SupervisorId, A.FechaUltimoCambio, A.Comentarios, A.TipoAnexo, A.Activo, A.IsSubVersion, A.PDFFirmardo, A.PDFFotografico, D.UserId, U.UserName FROM AnexosDTC A INNER JOIN DTCData D ON A.DTCReference = D.ReferenceNumber INNER JOIN DTCUsers U ON D.UserId = U.UserId WHERE DTCReference = @referenceDTC AND IsSubVersion = 0 ORDER BY AnexoReference ASC
