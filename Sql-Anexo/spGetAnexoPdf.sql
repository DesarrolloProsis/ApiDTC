USE [BitacoraProsisDev]
GO
/****** Object:  StoredProcedure [dbo].[GetAnexoPdf]    Script Date: 11/04/2022 12:41:00 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[GetAnexoPdf]

	@referenceAnexo NVARCHAR(20),
	@isSubVersion BIT
AS
	DECLARE @SetAnexoReference NVARCHAR(20)
	DECLARE @referenceDTC NVARCHAR(20)

	SET @referenceDTC = (SELECT DISTINCT DTCReference FROM AnexosDTC WHERE AnexoReference = @referenceAnexo)

	IF @isSubVersion = 1
	begin
		SET @SetAnexoReference = (SELECT @referenceAnexo + '-' + CAST(
                (SELECT MAX(CAST(SUBSTRING(AnexoReference,(LEN(@referenceDTC) + 5),2) AS int))
                FROM AnexosDTC
                WHERE DTCReference = @referenceDTC and IsSubVersion = 1
                ) AS varchar))
	end
	else
	begin
		SET @SetAnexoReference = @referenceAnexo

	end

BEGIN
	SELECT 
		a.FechaApertura, 
		a.FechaCierre, 
		a.FechaSolicitudInicio, 
		a.FechaOficioInicio, 
		a.FolioOficio as 'Folio',
		a.Solicitud,
		[Supervisor] = (SELECT a.Name + ' ' + a.LastName1 + ' ' + a.LastName2
						FROM DTCUsers a
						join DTCData b
							on a.UserId = b.USerId
						join AnexosDTC c
							on b.ReferenceNumber = c.DTCReference
						WHERE c.AnexoReference = @SetAnexoReference),
		'No. ' + c.SquareCatalogId + ' ' + c.SquareName as 'Plaza',
		e.Name + ' ' + e.LastName1 + ' ' + e.LastName2 as 'Admin' ,
		[Testigo Uno] = (SELECT a.Name + ' ' + a.LastName1 + ' ' + a.LastName2
						FROM AdminsSquares a
						join AnexosDTC b
							on a.AdminSquareId = b.Testigo1Id
						WHERE b.AnexoReference = @SetAnexoReference),
		[Testigo Dos] = (SELECT a.Name + ' ' + a.LastName1 + ' ' + a.LastName2
						FROM AdminsSquares a
						join AnexosDTC b
							on a.AdminSquareId = b.Testigo2Id
						WHERE b.AnexoReference = @SetAnexoReference),
		h.Lane as 'Carril',
		d.SinisterNumber as 'No. de Siniestro',
		d.ReportNumber as 'No. de Reporte',
		d.FailureNumber as 'No. de Fallo',
		d.SinisterDate,
		i.ManagerName as 'Vo.Bo.',
		i.RegionalCoordination as 'Region',
		j.Description as 'Descripcion',
		c.Ciudad,
		c.Estado,
		i.DelegationId
	FROM AnexosDTC a
	join DTCData d
		on a.DTCReference = d.ReferenceNumber
	join SquaresCatalog c
		on d.SquareId = c.SquareCatalogId
	join AdminsSquares e
		on (d.AdminId = e.AdminSquareId)
	join ComponentAnexo f
		on a.AnexoReference =  f.AnexoId
	join RequestedComponents g
		on f.ComponentDTCId = g.RequestedComponentId
	join LanesCatalog h
		on (g.IdGare = h.IdGare and g.CapufeLaneNum = h.CapufeLaneNum)
	join AgreementInfo i
		on (c.DelegationId = i.DelegationId and i.Status = 1)
	join TypeDescriptions j
		on d.TypeDescriptionId = j.TypeDescriptionId
	WHERE A.AnexoReference = @SetAnexoReference

	SELECT 
		e.Lane as  'Carril',
		f.Component  as 'Componente', 
		c.Model + ' / ' + c.Brand as 'MOD/MARCA', 
		c.SerialNumber as 'Serie',
		f.InventaryNumCapufe as 'Inventario'
	FROM AnexosDTC a
	join ComponentAnexo b
		on a.AnexoReference = b.AnexoId
	join RequestedComponents c
		on b.ComponentDTCId = c.RequestedComponentId
	join ComponentsStock x
		on c.ComponentsStockId = x.ComponentsStockId
	join ProposedComponents d
		on (c.ComponentsStockId = d.ComponentsStockId and c.ReferenceNumber = d.ReferenceNumber and c.CapufeLaneNum = d.CapufeLaneNum and c.IdGare = d.IdGare and c.TableFolio = d.TableFolio)
	join LanesCatalog e
		on (c.IdGare = e.IdGare and c.CapufeLaneNum = e.CapufeLaneNum)
	join SquareInventory f
		on c.TableFolio = f.TableFolio
	WHERE a.AnexoReference = @SetAnexoReference

	SELECT 
		e.Lane as  'Carril',
		f.Component  as 'Componente', 
		d.Model + ' / ' + d.Brand as 'MOD/MARCA', 
		b.NumeroSerie as 'Serie', 
		f.InventaryNumCapufe as 'Inventario'
	FROM AnexosDTC a
	join ComponentAnexo b
		on a.AnexoReference = b.AnexoId
	join RequestedComponents c
		on b.ComponentDTCId = c.RequestedComponentId
	join ComponentsStock x
		on c.ComponentsStockId = x.ComponentsStockId
	join ProposedComponents d
		on (c.ComponentsStockId = d.ComponentsStockId and c.ReferenceNumber = d.ReferenceNumber and c.CapufeLaneNum = d.CapufeLaneNum and c.IdGare = d.IdGare and c.TableFolio = d.TableFolio)
	join LanesCatalog e
		on (c.IdGare = e.IdGare and c.CapufeLaneNum = e.CapufeLaneNum)
	join SquareInventory f
		on c.TableFolio = f.TableFolio
	WHERE A.AnexoReference = @SetAnexoReference

END
