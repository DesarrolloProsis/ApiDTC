USE [BitacoraProsisAnexo]
GO
/****** Object:  StoredProcedure [dbo].[GetAnexoPdf]    Script Date: 28/03/2022 11:39:38 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetAnexoPdf]

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
		b.Nombre as 'Supervisor', 'No. ' + c.SquareCatalogId + ', ' + c.SquareName as 'Plaza',
		e.Name + ' ' + e.LastName1 + ' ' + e.LastName2 as 'Admin' ,
		a.Testigo1Id as 'Testigo Uno',
		a.Testigo2Id as 'Testigo Dos',
		h.Lane as 'Carril',
		d.SinisterNumber as 'No. de Siniestro',
		d.ReportNumber as 'No. de Reporte',
		d.FailureNumber as 'No. de Fallo',
		d.SinisterDate,
		i.ManagerName as 'Vo.Bo.',
		i.RegionalCoordination as 'Region',
		j.Description as 'Descripcion'
	FROM AnexosDTC a
	join CatalogoUserAnexo b
		on a.SupervisorId = b.Id
	join SquaresCatalog c
		on b.SquareId = c.SquareCatalogId
	join DTCData d
		on a.DTCReference = d.ReferenceNumber
	join AdminsSquares e
		on (d.AdminId = e.AdminSquareId and e.StatusAdmin = 1)
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
