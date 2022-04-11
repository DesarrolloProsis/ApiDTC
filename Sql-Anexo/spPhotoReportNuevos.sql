USE [BitacoraProsisDev]
GO
/****** Object:  StoredProcedure [dbo].[spPhotoReportNuevos]    Script Date: 11/04/2022 12:41:51 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--SP PARA PDF ANEXODTC
ALTER PROCEDURE [dbo].[spPhotoReportNuevos]
@ReferenceNumber nvarchar(20),
@ReferenceNumberAnexo nvarchar(20)
as
begin
	

	declare @strLanes nvarchar(150)
	declare @strLane nvarchar(5)
	declare @intCount int = 0


	declare c_Lanes cursor for select b.Lane from RequestedComponents a join LanesCatalog b
 									on (a.CapufeLaneNum = b.CapufeLaneNum and a.IdGare = b.IdGare)
								where ReferenceNumber = @ReferenceNumber
								group by Lane


	open c_Lanes
	fetch next from c_Lanes into @strLane

	while @@FETCH_STATUS = 0
	begin

		if @intCount = 0
		begin

			set @strLanes = @strLane

		end
		else
		begin

			set @strLanes = @strLanes + '-' + @strLane

		end
		
		--print cast(@intCount as nvarchar)

		set @intCount += 1

		fetch next from c_Lanes into @strLane
	end

	close c_Lanes
	deallocate c_Lanes


	select a.ReferenceNumber Referencia,
		   b.SquareCatalogId + ' ' + b.SquareName Plaza,
		   @strLanes Ubicacion,
		   e.[Start] Inicio,
		   e.[End] Fin,
		   e.DiagnosisDate Fecha,
		   c.Name+' '+c.LastName1+' '+c.LastName2 Tecnico,
		   d.Name+' '+d.LastName1+' '+d.LastName2 PersonalCapufe,
		   isnull(a.SinisterNumber,'No Asignado') NumeroSiniestro,
		   e.CauseFailure Observation,
		   a.DiagnosisReference,
		   f.TypeFaultId,
		   j.FechaApertura,
		   j.FechaCierre,
		   j.Observaciones
	from DTCData a join SquaresCatalog b 
		on a.SquareId = b.SquareCatalogId
	join DTCUsers c	
		on a.UserId = c.UserId
	join AdminsSquares d
		on a.SquareId = d.SquareCatalogId
	join FaultDiagnosis e
		on a.DiagnosisReference = e.ReferenceNumber
	join TechnicalSheet f
		on e.ReferenceNumber = f.ReferenceNumber
	join AnexosDTC j
		on (a.ReferenceNumber = j.DTCReference and j.AnexoReference = @ReferenceNumberAnexo)
	where a.ReferenceNumber = @ReferenceNumber and a.AdminId = d.AdminSquareId
	
end