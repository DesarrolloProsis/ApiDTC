--CATALOGOS PARA LOS ANEXO PENDIENTE DE BORRAR
CREATE TABLE [dbo].[CatalogoUserAnexo](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Nombre] [nvarchar](40) NOT NULL,
	[SquareId] [nvarchar](4) NOT NULL,
	[RollId] [int] NOT NULL
	FOREIGN KEY(SquareId) REFERENCES SquaresCatalog(SquareCatalogId),
	FOREIGN KEY(RollId) REFERENCES RollsCatalog(RollId),
)

--CREACION DE ANEXOS DTC
CREATE TABLE [dbo].[AnexosDTC](
	[DTCReference] [nvarchar](20) NOT NULL,
	[AnexoReference] [nvarchar](20) NOT NULL PRIMARY KEY,
	[FechaApertura] [datetime] NOT NULL,
	[FechaCierre] [datetime] NULL,
	[Solicitud] [nvarchar](50) NULL,
	[FechaSolicitudInicio] [datetime] NULL,
	[FolioOficio] [nvarchar](50) NULL,
	[FechaOficioInicio] [datetime] NULL,
	[Testigo1Id] [int] NOT NULL,
	[Testigo2Id] [int] NOT NULL,
	--[SupervisorId] [int] NOT NULL,
	[FechaUltimoCambio] [datetime] NULL,
	[Comentarios] [nvarchar](300) NULL,
	[TipoAnexo] [char](1) NOT NULL,
	[Activo] [bit] NULL,
	[IsSubVersion] [bit] NOT NULL,
	[PDFFirmardo] [bit] NOT NULL,
	[PDFFotografico] [bit] NOT NULL,
	
	FOREIGN KEY(DTCReference) REFERENCES DTCData(ReferenceNumber),
	FOREIGN KEY(Testigo1Id) REFERENCES AdminsSquares(AdminSquareId),
	FOREIGN KEY(Testigo2Id) REFERENCES AdminsSquares(AdminSquareId),
	--FOREIGN KEY(SupervisorId) REFERENCES CatalogoUserAnexo(Id)
)
--AGREGAMOS COLUMNA NUEVA DE OBSERVACIONES A LA TABLAD EANEXO DTC
ALTER TABLE AnexosDTC
ADD Observaciones NVARCHAR(300) NULL


--AGREGAMOS LOS ROLLES QUE FALTAN PARA DISTINGUIR LOS OPERADORES Y LOS ADMIN EN LA TABLA DE AdminsSquares PARA USAR COMO TESTIGOS EN LOS ANEXOS

INSERT INTO RollsCatalog VALUES (11,'Administrar Plaza')
INSERT INTO RollsCatalog VALUES (12,'Operador Plaza')

--MODIFICAR TABLA DE AdminsSquares
ALTER TABLE AdminsSquares	
	ADD IdRoll INT FOREIGN KEY(IdRoll) REFERENCES RollsCatalog(RollId)

UPDATE AdminsSquares SET IdRoll = 11
SELECT * FROM AdminsSquares

--AGREGAR COLUMNA PARA IDENTIFICAR COMPONETES YA USADOS EN ANEXO
ALTER TABLE RequestedComponents ADD UseInAnexo BIT NOT NULL DEFAULT(0)

--CREACION DE COMPONENTES ANEXO
CREATE TABLE [dbo].[ComponentAnexo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AnexoId] [nvarchar](20) NOT NULL,
	[ComponentDTCId] [int] NOT NULL,
	[NumeroSerie] [nvarchar](30) NOT NULL
	FOREIGN KEY(AnexoId) REFERENCES AnexosDTC(AnexoReference),
	FOREIGN KEY(ComponentDTCId) REFERENCES RequestedComponents(RequestedComponentId)
)

--ULTIMOS COMPONENTES DEL SUB ANEXO
CREATE OR ALTER PROCEDURE GetComponentesAnexo
	@referenceAnexo NVARCHAR(20)
AS	
	DECLARE @ultimaVersionAnexo NVARCHAR(20)
	DECLARE @conteoVersion INT
	DECLARE @referenceDTC NVARCHAR(20)
	
	SELECT @conteoVersion = COUNT(*) FROM AnexosDTC WHERE SUBSTRING(AnexoReference, 0, LEN(@referenceAnexo) + 1) = @referenceAnexo AND IsSubVersion = 1	

	SELECT TOP 1 @referenceDTC = DTCReference FROM AnexosDTC WHERE AnexoReference = @referenceAnexo

	IF @conteoVersion > 0
	BEGIN	
		DECLARE @LenDTC NVARCHAR(20)
		SET @LenDTC = (SELECT LEN(@referenceDTC))
		--SElECT TOP 1 @ultimaVersionAnexo = AnexoReference FROM AnexosDTC WHERE SUBSTRING(AnexoReference, 0, LEN(@referenceAnexo) + 1) = @referenceAnexo AND IsSubVersion = 1 ORDER BY AnexoReference DESC 
		SElECT TOP 1 @ultimaVersionAnexo = AnexoReference FROM AnexosDTC WHERE DTCReference = @referenceDTC AND SUBSTRING(AnexoReference, 0, LEN(@referenceAnexo) + 1) = @referenceAnexo AND IsSubVersion = 1 ORDER BY CAST(SUBSTRING(AnexoReference,(@LenDTC + 5),2) AS int) DESC 
		SELECT * FROM ComponentAnexo WHERE AnexoId = @ultimaVersionAnexo
	END
	ELSE
	BEGIN		
		SELECT * FROM ComponentAnexo WHERE AnexoId = @referenceAnexo
	END
GO

--BUSCA LOS COMPONETES QUE TIENE UN DTC 
CREATE OR ALTER PROCEDURE [dbo].[GetCompRequestAnexo]
  @referenceNumber NVARCHAR(20)
AS
        SELECT DISTINCT R.RequestedComponentId,
                S.Description AS NameComponent,
                R.Brand,                 
                R.Model, 
                P.Brand AS BrandPropuesto,
                P.Model AS ModelPropuesto,
                R.SerialNumber, 
                L.Lane,
                D.Observation,
                R.UseInAnexo                           
        FROM RequestedComponents R 
                INNER JOIN ComponentsStock S         
        ON R.ComponentsStockId = S.ComponentsStockId        
                INNER JOIN ProposedComponents P 
        ON R.ComponentsStockId = P.ComponentsStockId AND R.ReferenceNumber = P.ReferenceNumber AND R.CapufeLaneNum = P.CapufeLaneNum AND R.IdGare = P.IdGare
                INNER JOIN LanesCatalog L
        ON R.IdGare = L.IdGare AND R.CapufeLaneNum = L.CapufeLaneNum
                INNER JOIN DTCData D
        ON R.ReferenceNumber = D.ReferenceNumber
        WHERE R.ReferenceNumber = @referenceNumber
GO

--VALIDAR CUAL ES EL SP QUE NOS SIRVE A AMBOS
CREATE OR ALTER PROCEDURE [dbo].[GetConteoVersionesAnexo]
	@referenceDTC NVARCHAR(20),
	@referenceAnexo NVARCHAR(20),
	@isSubVersion BIT
AS
	DECLARE @LenDTC NVARCHAR(20)
	SET @LenDTC = (SELECT LEN(@referenceDTC))
	IF @isSubVersion = 1
	BEGIN
		SELECT AnexoReference
		FROM AnexosDTC  WHERE DTCReference = @referenceDTC AND SUBSTRING(AnexoReference, 0, LEN(@referenceAnexo) + 1) = @referenceAnexo AND IsSubVersion = 1 ORDER BY CAST(SUBSTRING(AnexoReference,(@LenDTC + 5),2) AS int) DESC 
	END
	ELSE
	BEGIN 
		SELECT AnexoReference FROM AnexosDTC WHERE DTCReference = @referenceDTC AND IsSubVersion = 0 ORDER BY CAST(SUBSTRING(AnexoReference, (@LenDTC + 3), 2) AS int) DESC 
	END
GO

--CREATE PROCEDURE [dbo].[GetConteoVersionesAnexos]
--	@referenceDTC NVARCHAR(20),
--	@referenceAnexo NVARCHAR(20),
--	@isSubVersion BIT
--AS
--	IF @isSubVersion = 1
--	BEGIN
--		SELECT AnexoReference FROM AnexosDTC  WHERE DTCReference = @referenceDTC AND SUBSTRING(AnexoReference, 0, LEN(@referenceAnexo) + 1) = @referenceAnexo AND IsSubVersion = 1 ORDER BY AnexoReference DESC 
--	END
--	ELSE
--	BEGIN 
--		SELECT AnexoReference FROM AnexosDTC WHERE DTCReference = @referenceDTC AND IsSubVersion = 0 ORDER BY AnexoReference DESC 
--	END
--GO


--BUSCA EL HEADER DE UN ANEXO EN ESPECIFICO
CREATE OR ALTER PROCEDURE [dbo].[GetHeaderAnexo]
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
		--SElECT TOP 1 @ultimaVersionAnexo = AnexoReference FROM AnexosDTC WHERE SUBSTRING(AnexoReference, 0, LEN(@referenceAnexo) + 1) = @referenceAnexo AND IsSubVersion = 1 ORDER BY AnexoReference DESC 	
		SElECT TOP 1 @ultimaVersionAnexo = AnexoReference FROM AnexosDTC WHERE SUBSTRING(AnexoReference, 0, LEN(@referenceAnexo) + 1) = @referenceAnexo AND IsSubVersion = 1 ORDER BY AnexoReference DESC
		SELECT *, @userId AS UserId, @userName AS UserName FROM AnexosDTC WHERE AnexoReference = @ultimaVersionAnexo
	END
	ELSE 
	BEGIN
		SELECT *, @userId AS UserId, @userName AS UserName FROM AnexosDTC WHERE AnexoReference = @referenceAnexo
	END
GO

--ANEXOS CREADOS EN UN DTC EN ESPECIFICO
CREATE OR ALTER PROCEDURE [dbo].[GetHistoricoAnexo]	
	@referenceDTC NVARCHAR(20)
AS
	SELECT A.DTCReference, 
		A.AnexoReference, 
		A.FechaApertura, 
		A.FechaCierre, 
		A.FolioOficio, 
		A.FechaOficioInicio,
		A.Solicitud, 
		A.FechaSolicitudInicio, 
		--A.FechaSolicitudFin, 
		--A.FechaOficioFin, 
		A.Testigo1Id, 
		A.Testigo2Id, 
		--A.SupervisorId, 
		A.FechaUltimoCambio, 
		A.Comentarios, 
		A.TipoAnexo, 
		A.Activo, 
		A.IsSubVersion, 
		A.PDFFirmardo, 
		A.PDFFotografico, 
		D.UserId, 
		U.UserName
	FROM AnexosDTC A 
		INNER JOIN DTCData D 
	ON A.DTCReference = D.ReferenceNumber 
		INNER JOIN DTCUsers U 
	ON D.UserId = U.UserId 
	WHERE DTCReference = @referenceDTC 
		AND IsSubVersion = 0 
	ORDER BY AnexoReference ASC
GO


----BUSCAR LOS SUPERVISORES DE ANEXO
--CREATE PROCEDURE [dbo].[GetSupervisorAnexoPlaza]
--	@plazaId NVARCHAR(4)
--AS
--	--11 IDROLL DE SUPERVISOR ANEXO
--	SELECT * FROM CatalogoUserAnexo WHERE SquareId = @plazaId AND RollId = 11
--GO

--BUSCAR LOS TESTIGOS DE ANEXO
CREATE or ALTER PROCEDURE [dbo].[GetTestigosPlaza]
	@plazaId NVARCHAR(4)
AS
	--11 IDROLL DE TESTIGOS == AdministradorPlaza
	SELECT AdminSquareId AS Id, Name + ' ' + LastName1 + ' ' + LastName2 AS Nombre, SquareCatalogId AS SquareId, IdRoll AS RollId  FROM AdminsSquares WHERE SquareCatalogId = @plazaId AND IdRoll = 11
GO

--INSERTA LOS COMPONETE CONTIENE LOGICA PARA OCUPAR LOS COMPONENTES DE UN DTC CAMBIO DE BANDERA
CREATE OR ALTER PROCEDURE [dbo].[InsertComponentesAnexo]
	@referenceNumber NVARCHAR(20),	
	@AnexoId NVARCHAR(20),
	@ComponentDTCId INT,
	@NumeroSerie NVARCHAR(30)
AS
	BEGIN TRY
		INSERT INTO ComponentAnexo VALUES(@AnexoId, @ComponentDTCId, @NumeroSerie)
		UPDATE RequestedComponents SET UseInAnexo = 1 WHERE ReferenceNumber = @referenceNumber AND RequestedComponentId = @ComponentDTCId
		SELECT 'Insertado' SqlMessage, 'Se inserto un componente al anexo: '+  @AnexoId  SqlResult

		/*INSERTA HISTORIAL DEL COMPONENTE*/
				INSERT INTO COMPONENTES_LOG
				SELECT B.RequestedComponentId, B.ComponentsStockId, B.TableFolio, B.ReferenceNumber, @AnexoId, B.CapufeLaneNum, B.IdGare, B.RequestDate, B.InstallationDate, B.MaintenanceDate, C.StatusId, '1', GETDATE()
				FROM ComponentAnexo A
				INNER JOIN RequestedComponents B ON A.ComponentDTCId = B.RequestedComponentId 
				INNER JOIN DTCData C ON B.ReferenceNumber = C.ReferenceNumber
				WHERE RequestedComponentId = @ComponentDTCId
				AND B.ReferenceNumber = @referenceNumber
	END TRY
	BEGIN CATCH
		SELECT NULL AS SqlResult, 'Línea: ' + CAST(ERROR_LINE() AS VARCHAR) + ' ' + CAST(@@ERROR AS VARCHAR) AS SqlMessage
	END CATCH
GO

--INSERTA LOS COMPONETE CONTIENE LOGICA PARA LIBERAR ESPACIOS DE LOS DTC
CREATE OR ALTER PROCEDURE [dbo].[InsertHeaderAnexo]
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
	--@supervisorId INT,
	@observaciones NVARCHAR(300),
	@testigo1 INT,
	@testigo2 INT,
	@tipoAnexo CHAR,
	@isSubVersion BIT
AS
BEGIN TRY
	IF @tipoAnexo = 'A'
	BEGIN
		INSERT INTO AnexosDTC(DTCReference, AnexoReference, FechaApertura, FechaCierre, Solicitud, FechaSolicitudInicio, FolioOficio, FechaOficioInicio, Observaciones, Testigo1Id, Testigo2Id, Activo, TipoAnexo, FechaUltimoCambio, IsSubVersion, PDFFirmardo, PDFFotografico) 
		VALUES(@referenceDTC, @referenceAnexo, @fechaApertura, @fechaCierre, @solicitud, @fechaSolicitudInicio, @folioOficio, @fechaOficioInicio, @observaciones, @testigo1, @testigo2, 1, 'A', GETDATE(), @isSubVersion, 0 , 0)	
		
		IF @@ROWCOUNT = 1
		BEGIN 
			IF (@referenceAnexoAnterior IS NOT NULL)
			BEGIN
				UPDATE RequestedComponents SET UseInAnexo = 0 WHERE RequestedComponentId IN(SELECT ComponentDTCId FROM ComponentAnexo WHERE AnexoId = @referenceAnexoAnterior)		
				/*INSERTA HISTORIAL DEL COMPONENTE*/
				INSERT INTO COMPONENTES_LOG
					SELECT B.RequestedComponentId, B.ComponentsStockId, B.TableFolio, B.ReferenceNumber, @referenceAnexoAnterior, B.CapufeLaneNum, B.IdGare, B.RequestDate, B.InstallationDate, B.MaintenanceDate, C.StatusId, '0', GETDATE()
					FROM ComponentAnexo A
					INNER JOIN RequestedComponents B ON A.ComponentDTCId = B.RequestedComponentId 
					INNER JOIN DTCData C ON B.ReferenceNumber = C.ReferenceNumber
					WHERE RequestedComponentId IN(SELECT ComponentDTCId FROM ComponentAnexo WHERE AnexoId = @referenceAnexoAnterior)		
			END
			SELECT 'NEWANEXO!' As SqlResult, 'Anexo con Id' + @referenceAnexo + 'agregado con exito' as SqlMessage
		END		
	END
	ELSE
	BEGIN	
		INSERT INTO AnexosDTC(DTCReference, AnexoReference, FechaApertura, FechaCierre, Solicitud, FechaSolicitudInicio, FolioOficio, FechaOficioInicio, Observaciones, Testigo1Id, Testigo2Id, Activo, TipoAnexo, FechaUltimoCambio, IsSubVersion, PDFFirmardo, PDFFotografico) 
		VALUES(@referenceDTC, @referenceAnexo, @fechaApertura, @fechaCierre, @solicitud, @fechaSolicitudInicio, @folioOficio, @fechaOficioInicio, @observaciones, @testigo1, @testigo2, 1, 'B', GETDATE(), @isSubVersion, 0, 0)	

		IF @@ROWCOUNT = 1
		BEGIN
			IF (@referenceAnexoAnterior IS NOT NULL)
			BEGIN
				UPDATE RequestedComponents SET UseInAnexo = 0 WHERE RequestedComponentId IN(SELECT ComponentDTCId FROM ComponentAnexo WHERE AnexoId = @referenceAnexoAnterior)
				/*INSERTA HISTORIAL DEL COMPONENTE*/
				INSERT INTO COMPONENTES_LOG
					SELECT B.RequestedComponentId, B.ComponentsStockId, B.TableFolio, B.ReferenceNumber, @referenceAnexoAnterior, B.CapufeLaneNum, B.IdGare, B.RequestDate, B.InstallationDate, B.MaintenanceDate, C.StatusId, '0', GETDATE()
					FROM ComponentAnexo A
					INNER JOIN RequestedComponents B ON A.ComponentDTCId = B.RequestedComponentId 
					INNER JOIN DTCData C ON B.ReferenceNumber = C.ReferenceNumber
					WHERE RequestedComponentId IN(SELECT ComponentDTCId FROM ComponentAnexo WHERE AnexoId = @referenceAnexoAnterior)				
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
GO

--CREATE PROCEDURE [dbo].[InsertSupervisor]
--	@nombreSupervisor NVARCHAR(40),
--	@numeroPlaza NVARCHAR(4)
--AS	
--	BEGIN TRY
--		INSERT INTO CatalogoUserAnexo VALUES(@nombreSupervisor, @numeroPlaza, 11)
--		SELECT 'Insertado' SqlMessage, 'Se inserto el usuario anexo: '+  @nombreSupervisor + 'en la plaza' + @numeroPlaza  SqlResult
--	END TRY
--	BEGIN CATCH
--		SELECT NULL AS SqlResult, 'Línea: ' + CAST(ERROR_LINE() AS VARCHAR) + ' ' + CAST(@@ERROR AS VARCHAR) AS SqlMessage
--	END CATCH
--GO

--CREATE PROCEDURE [dbo].[InsertTestigo]
--	@nombreTestigo NVARCHAR(40),
--	@numeroPlaza NVARCHAR(4)
--AS	
--	BEGIN TRY
--		INSERT INTO CatalogoUserAnexo VALUES(@nombreTestigo, @numeroPlaza, 12)
--		SELECT 'Insertado' SqlMessage, 'Se inserto el usuario anexo: '+  @nombreTestigo + 'en la plaza' + @numeroPlaza  SqlResult
--	END TRY
--	BEGIN CATCH
--		SELECT NULL AS SqlResult, 'Línea: ' + CAST(ERROR_LINE() AS VARCHAR) + ' ' + CAST(@@ERROR AS VARCHAR) AS SqlMessage
--	END CATCH
--GO

--SP PARA PDF ANEXODTC
CREATE PROCEDURE [dbo].[spPhotoReportNuevos]
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
		   j.FechaCierre
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
GO

CREATE OR ALTER PROCEDURE [dbo].[GetAnexoPdf]

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
		[Testigo Uno] = (SELECT Nombre 
						from CatalogoUserAnexo a
						join AnexosDTC b
							on a.Id = b.Testigo1Id
						WHERE b.AnexoReference = @SetAnexoReference),
		[Testigo Dos] = (SELECT Nombre 
						from CatalogoUserAnexo a
						join AnexosDTC b
							on a.Id = b.Testigo2Id
						WHERE b.AnexoReference = @SetAnexoReference),
		h.Lane as 'Carril',
		d.SinisterNumber as 'No. de Siniestro',
		d.ReportNumber as 'No. de Reporte',
		d.FailureNumber as 'No. de Fallo',
		d.SinisterDate,
		i.ManagerName as 'Vo.Bo.',
		i.RegionalCoordination as 'Region',
		j.Description as 'Descripcion',
		i.ManagerName as 'Subgerente'
	FROM AnexosDTC a
	join CatalogoUserAnexo b
		on a.SupervisorId = b.Id
	join SquaresCatalog c
		on b.SquareId = c.SquareCatalogId
	join DTCData d
		on a.DTCReference = d.ReferenceNumber
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
GO

--CAMBIO EN EL SP DE ANEXOSDTC
ALTER procedure [dbo].[spGetDTCView]
@UserId int , --UsuarioId
@SquareId nvarchar(10) --PlazaID

as

	--Para llenar las cartas de DTC segun el rol de usuario

declare
@intRol int,
@intDelegationId int

select @intRol = RollId from DTCUsers where UserId = @UserId

select @intDelegationId = DelegationId from SquaresCatalog where SquareCatalogId = @SquareId


if @intRol = 2
begin
	
	--select 'Admin'

	select 
			e.UserName,
			a.ReferenceNumber,
			a.AdminId,
		   a.SinisterNumber,
		   a.ReportNumber,
		   a.SinisterDate,
		   a.StatusId,
		   a.FailureDate,
		   a.FailureNumber,
		   a.ShippingDate,
		   a.ElaborationDate,
		   a.DateStamp,
		   a.TypeDescriptionId,
		   c.Description as TypeDescription,
		   a.Observation,
		   a.Diagnosis,
		   d.StatusDescription,
		   a.OpenMode,
		   b.SquareCatalogId,
		   b.ReferenceSquare,
		   a.UserId,
		   ts.TypeFaultId,
		   ts.ReferenceNumber TechnicalSheetReference,		
		   fa.FaultDescription,
			IsAnexoCreate =  CASE (SELECT COUNT(*) FROM AnexosDTC WHERE DTCReference = a.ReferenceNumber)
		WHEN 0 THEN CAST(0 AS BIT)
		ELSE CAST(1 AS BIT)
	  END,
		IsValidCreate =  CASE (SELECT COUNT(*) FROM RequestedComponents WHERE ReferenceNumber = a.ReferenceNumber AND UseInAnexo = 0)
		WHEN 0 THEN CAST(0 AS BIT)
		ELSE CAST(1 AS BIT)
		END,
	e.Name + ' ' + e.LastName1 + ' ' + e.LastName2 as Name		   
	from DTCData a join SquaresCatalog b
		on a.SquareId = b.SquareCatalogId
	join TypeDescriptions c
		on a.TypeDescriptionId = c.TypeDescriptionId
	join DTCStatusCatalog d
		on a.StatusId = d.StatusId
	join DTCUsers e
		on a.UserId = e.UserId
	right join FaultDiagnosis fd
		on a.DiagnosisReference = fd.ReferenceNumber
	join TechnicalSheet ts
		on fd.ReferenceNumber = ts.ReferenceNumber
	join TypeFault fa
		on ts.TypeFaultId = fa.TypeFaultId
	where 
	((b.DelegationId = @intDelegationId and a.StatusId > 0 and e.RollId = 1) 
	--((a.StatusId > 0 and e.RollId = 1) 
	or (a.UserId = @UserId))
	AND (a.UserId NOT IN (1, 37, 5, 7,63,71))
	and a.ReferenceNumber is not null
	order by a.DateStamp desc
	

end
else if @intRol = 4 
begin
	
	--Administradores

	select 
	du.UserName,
	  d.ReferenceNumber, 
	  d.AdminId,
	  d.SinisterNumber, 
	  ReportNumber, 
	  SinisterDate, 
	  d.StatusId, 
	  FailureDate, 
	  d.FailureNumber, 
	  ShippingDate, 
	  ElaborationDate, 
	  d.DateStamp, 
	  d.TypeDescriptionId, 
	  t.Description as TypeDescription, 
	  Observation, 
	  Diagnosis, 
	  s.StatusDescription, 
	  d.OpenMode,
	  sc.SquareCatalogId,
	  sc.ReferenceSquare,
	  d.UserId,
	  ts.TypeFaultId,
	  ts.ReferenceNumber TechnicalSheetReference,
	  fa.FaultDescription,	  	  
	  IsAnexoCreate = CASE (SELECT COUNT(*) FROM AnexosDTC WHERE DTCReference = D.ReferenceNumber)
		WHEN 0 THEN CAST(0 AS BIT)
		ELSE CAST(1 AS BIT)
	  END,
	  		IsValidCreate =  CASE (SELECT COUNT(*) FROM RequestedComponents WHERE ReferenceNumber = d.ReferenceNumber AND UseInAnexo = 0)
		WHEN 0 THEN CAST(0 AS BIT)
		ELSE CAST(1 AS BIT)
		END,
	  du.Name + ' ' + du.LastName1 + ' ' + du.LastName2 as Name		   
	from DTCData d
	 join TypeDescriptions t 
		on d.TypeDescriptionId = t.TypeDescriptionId 
	join DTCStatusCatalog s 
		on d.StatusId = s.StatusId 
	join SquaresCatalog sc 
		on d.SquareId = sc.SquareCatalogId
	right join FaultDiagnosis fd
		on d.DiagnosisReference = fd.ReferenceNumber
	join TechnicalSheet ts
		on fd.ReferenceNumber = ts.ReferenceNumber
	join TypeFault fa
		on ts.TypeFaultId = fa.TypeFaultId
	join DTCUsers du
		on d.UserId = du.UserId
	where d.StatusId >= 2 AND (d.UserId NOT IN (1,37,5,63,71))  
	and d.ReferenceNumber is not null
	order by d.DateStamp desc
	

end
else if @intRol = 5
begin
	
	--AdminSistemas

	select 
		e.UserName,
		a.ReferenceNumber,
			a.AdminId,
		   a.SinisterNumber,
		   a.ReportNumber,
		   a.SinisterDate,
		   a.StatusId,
		   a.FailureDate,
		   a.FailureNumber,
		   a.ShippingDate,
		   a.ElaborationDate,
		   a.DateStamp,
		   a.TypeDescriptionId,
		   c.Description as TypeDescription,
		   a.Observation,
		   a.Diagnosis,
		   d.StatusDescription,
		   a.OpenMode,
		   b.SquareCatalogId,
		   b.ReferenceSquare,
		   a.UserId,
		   ts.TypeFaultId,
		   ts.ReferenceNumber TechnicalSheetReference,
		   fa.FaultDescription,		   	  
	  IsAnexoCreate = CASE (SELECT COUNT(*) FROM AnexosDTC WHERE DTCReference = a.ReferenceNumber)
		WHEN 0 THEN CAST(0 AS BIT)
		ELSE CAST(1 AS BIT)
	  END,
	  		IsValidCreate =  CASE (SELECT COUNT(*) FROM RequestedComponents WHERE ReferenceNumber = a.ReferenceNumber AND UseInAnexo = 0)
		WHEN 0 THEN CAST(0 AS BIT)
		ELSE CAST(1 AS BIT)
		END,
		  e.Name + ' ' + e.LastName1 + ' ' + e.LastName2 as Name		   
	from DTCData a join SquaresCatalog b
		on a.SquareId = b.SquareCatalogId
	join TypeDescriptions c
		on a.TypeDescriptionId = c.TypeDescriptionId
	join DTCStatusCatalog d
		on a.StatusId = d.StatusId
	join DTCUsers e
		on a.UserId = e.UserId
	right join FaultDiagnosis fd
		on a.DiagnosisReference = fd.ReferenceNumber
	join TechnicalSheet ts
		on fd.ReferenceNumber = ts.ReferenceNumber
	join TypeFault fa
		on ts.TypeFaultId = fa.TypeFaultId
	where ((b.DelegationId = @intDelegationId and a.StatusId > 0 and e.RollId = 3) or (a.UserId = @UserId))
	AND (a.UserId NOT IN (1, 37, 5, 7,63,71)) and a.ReferenceNumber is not null
	order by DateStamp desc	

end
else if @intRol = 7
begin 
	
	--CAPUFE

	select
	du.UserName,
	  d.ReferenceNumber, 
	  d.AdminId,
	  d.SinisterNumber, 
	  ReportNumber, 
	  SinisterDate, 
	  d.StatusId, 
	  FailureDate, 
	  d.FailureNumber, 
	  ShippingDate, 
	  ElaborationDate, 
	  d.DateStamp, 
	  d.TypeDescriptionId, 
	  t.Description as TypeDescription, 
	  Observation, 
	  Diagnosis, 
	  s.StatusDescription, 
	  d.OpenMode,
	  sc.SquareCatalogId,
	  sc.ReferenceSquare,
	  d.UserId,
	  ts.TypeFaultId,
	  ts.ReferenceNumber TechnicalSheetReference,
	  fa.FaultDescription,	  	  
	  IsAnexoCreate =  CASE (SELECT COUNT(*) FROM AnexosDTC WHERE DTCReference = D.ReferenceNumber)
		WHEN 0 THEN CAST(0 AS BIT)
		ELSE CAST(1 AS BIT)
	  END,
	  IsValidCreate =  CASE (SELECT COUNT(*) FROM RequestedComponents WHERE ReferenceNumber = d.ReferenceNumber AND UseInAnexo = 0)
		WHEN 0 THEN CAST(0 AS BIT)
		ELSE CAST(1 AS BIT)
		END,
	  du.Name + ' ' + du.LastName1 + ' ' + du.LastName2 as Name		   
	from DTCData d
	 join TypeDescriptions t 
		on d.TypeDescriptionId = t.TypeDescriptionId 
	join DTCStatusCatalog s 
		on d.StatusId = s.StatusId 
	join SquaresCatalog sc 
		on d.SquareId = sc.SquareCatalogId
	right join FaultDiagnosis fd
		on d.DiagnosisReference = fd.ReferenceNumber
	join TechnicalSheet ts
		on fd.ReferenceNumber = ts.ReferenceNumber
	join TypeFault fa
		on ts.TypeFaultId = fa.TypeFaultId
	join DTCUsers du
		on d.UserId = du.UserId
	where (d.StatusId between 3 and 4) AND (d.UserId NOT IN (1, 37, 5, 7,63,71)) 
	and d.ReferenceNumber is not null and  Convert(DATE,d.DateStamp) like '2022%'
	order by d.DateStamp desc



end
else if  @intRol = 8
begin
	
	--Contabilidad

	select 
	du.UserName,
	  d.ReferenceNumber, 
	  d.AdminId,
	  d.SinisterNumber, 
	  ReportNumber, 
	  SinisterDate, 
	  d.StatusId, 
	  FailureDate, 
	  d.FailureNumber, 
	  ShippingDate, 
	  ElaborationDate, 
	  d.DateStamp, 
	  d.TypeDescriptionId, 
	  t.Description as TypeDescription, 
	  Observation, 
	  Diagnosis, 
	  s.StatusDescription, 
	  d.OpenMode,
	  sc.SquareCatalogId,
	  sc.ReferenceSquare,
	  d.UserId,
	  ts.TypeFaultId,
	  ts.ReferenceNumber TechnicalSheetReference,
	  fa.FaultDescription,	
	  IsAnexoCreate = 
	  CASE (SELECT COUNT(*) FROM AnexosDTC WHERE DTCReference = D.ReferenceNumber)
		WHEN 0 THEN CAST(0 AS BIT)
		ELSE CAST(1 AS BIT)
	  END,
	  IsValidCreate =  CASE (SELECT COUNT(*) FROM RequestedComponents WHERE ReferenceNumber = d.ReferenceNumber AND UseInAnexo = 0)
		WHEN 0 THEN CAST(0 AS BIT)
		ELSE CAST(1 AS BIT)
		END,
	  du.Name + ' ' + du.LastName1 + ' ' + du.LastName2 as Name		   
	from DTCData d
	 join TypeDescriptions t 
		on d.TypeDescriptionId = t.TypeDescriptionId 
	join DTCStatusCatalog s 
		on d.StatusId = s.StatusId 
	join SquaresCatalog sc 
		on d.SquareId = sc.SquareCatalogId
	right join FaultDiagnosis fd
		on d.DiagnosisReference = fd.ReferenceNumber
	join TechnicalSheet ts
		on fd.ReferenceNumber = ts.ReferenceNumber
	join TypeFault fa
		on ts.TypeFaultId = fa.TypeFaultId
	join DTCUsers du
		on d.UserId = du.UserId
	where d.StatusId = 4 AND (d.UserId NOT IN (1, 37, 5, 7,63,71)) 
	and d.ReferenceNumber is not null
	order by d.DateStamp desc



end
else if @intRol = 9
begin
	--Documentacion

	select
		e.UserName,
		a.ReferenceNumber,
			a.AdminId,
		   a.SinisterNumber,
		   a.ReportNumber,
		   a.SinisterDate,
		   a.StatusId,
		   a.FailureDate,
		   a.FailureNumber,
		   a.ShippingDate,
		   a.ElaborationDate,
		   a.DateStamp,
		   a.TypeDescriptionId,
		   c.Description as TypeDescription,
		   a.Observation,
		   a.Diagnosis,
		   d.StatusDescription,
		   a.OpenMode,
		   b.SquareCatalogId,
		   b.ReferenceSquare,
		   a.UserId,
		   ts.TypeFaultId,
		   ts.ReferenceNumber TechnicalSheetReference,
		   fa.FaultDescription,
		   	IsAnexoCreate =  CASE (SELECT COUNT(*) FROM AnexosDTC WHERE DTCReference = a.ReferenceNumber)
		WHEN 0 THEN CAST(0 AS BIT)
		ELSE CAST(1 AS BIT)
	  END,
	  	IsValidCreate =  CASE (SELECT COUNT(*) FROM RequestedComponents WHERE ReferenceNumber = a.ReferenceNumber AND UseInAnexo = 0)
		WHEN 0 THEN CAST(0 AS BIT)
		ELSE CAST(1 AS BIT)
		END,
		   e.Name + ' ' + e.LastName1 + ' ' + e.LastName2 as Name		   
	from DTCData a join SquaresCatalog b
		on a.SquareId = b.SquareCatalogId
	join TypeDescriptions c
		on a.TypeDescriptionId =	c.TypeDescriptionId
	join DTCStatusCatalog d
		on a.StatusId = d.StatusId
	join DTCUsers e
		on a.UserId = e.UserId
	right join FaultDiagnosis fd
		on a.DiagnosisReference = fd.ReferenceNumber
	join TechnicalSheet ts
		on fd.ReferenceNumber = ts.ReferenceNumber
	join TypeFault fa
		on ts.TypeFaultId = fa.TypeFaultId
	where 
	((b.DelegationId = @intDelegationId and		a.StatusId > 1 and e.RollId = 1) 
	or (a.UserId = @UserId))
	AND (a.UserId NOT IN (1, 37, 5, 7,63,71))
	and a.ReferenceNumber is not null
	order by DateStamp desc
		
end 
else if @intRol = 10
begin
	
	select 
		du.UserName,
	  d.ReferenceNumber, 
	  d.AdminId,
	  d.SinisterNumber, 
	  ReportNumber, 
	  SinisterDate, 
	  d.StatusId, 
	  FailureDate, 
	  d.FailureNumber, 
	  ShippingDate, 
	  ElaborationDate, 
	  d.DateStamp, 
	  d.TypeDescriptionId, 
	  t.Description as TypeDescription, 
	  Observation, 
	  Diagnosis, 
	  s.StatusDescription, 
	  d.OpenMode,
	  sc.SquareCatalogId,
	  sc.ReferenceSquare,
	  d.UserId,
	  ts.TypeFaultId,
	  ts.ReferenceNumber TechnicalSheetReference,
	  fa.FaultDescription,
	  IsAnexoCreate = 
	  CASE (SELECT COUNT(*) FROM AnexosDTC WHERE DTCReference = D.ReferenceNumber)
		WHEN 0 THEN CAST(0 AS BIT)
		ELSE CAST(1 AS BIT)
	  END,
	  IsValidCreate =  CASE (SELECT COUNT(*) FROM RequestedComponents WHERE ReferenceNumber = d.ReferenceNumber AND UseInAnexo = 0)
		WHEN 0 THEN CAST(0 AS BIT)
		ELSE CAST(1 AS BIT)
		END,
	  du.Name + ' ' + du.LastName1 + ' ' + du.LastName2 as Name		   
	from DTCData d
	 join TypeDescriptions t 
		on d.TypeDescriptionId = t.TypeDescriptionId 
	join DTCStatusCatalog s 
		on d.StatusId = s.StatusId 
	join SquaresCatalog sc 
		on d.SquareId = sc.SquareCatalogId
	right join FaultDiagnosis fd
		on d.DiagnosisReference = fd.ReferenceNumber
	join TechnicalSheet ts
		on fd.ReferenceNumber = ts.ReferenceNumber
	join DTCUsers du
		on d.UserId = du.UserId
	join TypeFault fa
		on ts.TypeFaultId = fa.TypeFaultId
	--join UserSquare us on d.UserId = us.UserId
	where d.ReferenceNumber is not null
	 order by d.DateStamp desc


end
else
begin

	--select 'Tecnico/Sistemas'


	select 
	du.userName,
	  d.ReferenceNumber, 
	  d.AdminId,
	  d.SinisterNumber, 
	  ReportNumber, 
	  SinisterDate, 
	  d.StatusId, 
	  FailureDate, 
	  d.FailureNumber, 
	  ShippingDate, 
	  ElaborationDate, 
	  d.DateStamp, 
	  d.TypeDescriptionId, 
	  t.Description as TypeDescription, 
	  Observation, 
	  Diagnosis, 
	  s.StatusDescription, 
	  d.OpenMode,
	  sc.SquareCatalogId,
	  sc.ReferenceSquare,
	  d.UserId,
	  ts.TypeFaultId,
	  ts.ReferenceNumber TechnicalSheetReference,
	  fa.FaultDescription,
	  IsAnexoCreate = 
	  CASE (SELECT COUNT(*) FROM AnexosDTC WHERE DTCReference = D.ReferenceNumber)
		WHEN 0 THEN CAST(0 AS BIT)
		ELSE CAST(1 AS BIT)
	  END,
	  		IsValidCreate =  CASE (SELECT COUNT(*) FROM RequestedComponents WHERE ReferenceNumber = d.ReferenceNumber AND UseInAnexo = 0)
		WHEN 0 THEN CAST(0 AS BIT)
		ELSE CAST(1 AS BIT)
		END,
	  du.Name + ' ' + du.LastName1 + ' ' + du.LastName2 as Name		   
	from DTCData d
	 join TypeDescriptions t 
		on d.TypeDescriptionId = t.TypeDescriptionId 
	join DTCStatusCatalog s 
		on d.StatusId = s.StatusId 
	join SquaresCatalog sc 
		on d.SquareId = sc.SquareCatalogId
	right join FaultDiagnosis fd
		on d.DiagnosisReference = fd.ReferenceNumber
	join TechnicalSheet ts
		on fd.ReferenceNumber = ts.ReferenceNumber
	join TypeFault fa
		on ts.TypeFaultId = fa.TypeFaultId
	join DTCUsers du
		on d.UserId = du.UserId
	--join UserSquare us on d.UserId = us.UserId
	---where (d.UserId = @UserId ) and d.ReferenceNumber is not null
	where (d.SquareId in (select SquareCatalogId from UserSquare where UserId = @UserId)  ) and d.ReferenceNumber is not null 
	and (d.UserId NOT IN (1, 37, 5, 70,63,71))
	and d.StatusId != 0  order by d.DateStamp desc

end
GO


--NUEVA COLUMNA CATALOGO DE PLAZAS

ALTER TABLE SquaresCatalog 
ADD Ciudad NVARCHAR(35)

ALTER TABLE SquaresCatalog 
ADD Estado NVARCHAR(35)

UPDATE SquaresCatalog SET Ciudad = 'Tlalpan', Estado = 'Ciudad de México' WHERE SquareCatalogId = '001' 
UPDATE SquaresCatalog SET Ciudad = 'Huitzilac', Estado = 'Morelos' WHERE SquareCatalogId = '1Bis' 
UPDATE SquaresCatalog SET Ciudad = 'Temixco', Estado = 'Morelos' WHERE SquareCatalogId = '107' 
UPDATE SquaresCatalog SET Ciudad = 'Alpuyeca', Estado = 'Morelos' WHERE SquareCatalogId = '105' 
UPDATE SquaresCatalog SET Ciudad = 'Alpuyeca', Estado = 'Morelos' WHERE SquareCatalogId = '106'
UPDATE SquaresCatalog SET Ciudad = 'Alpuyeca', Estado = 'Morelos' WHERE SquareCatalogId = '101'
UPDATE SquaresCatalog SET Ciudad = 'Ahuehuetzingo', Estado = 'Morelos' WHERE SquareCatalogId = '184'
UPDATE SquaresCatalog SET Ciudad = 'Huitzuco de los Figueroa', Estado = 'Guerrero' WHERE SquareCatalogId = '102'
UPDATE SquaresCatalog SET Ciudad = 'Mazatlan', Estado = 'Guerrero' WHERE SquareCatalogId = '103'
UPDATE SquaresCatalog SET Ciudad = 'Acapulco', Estado = 'Guerrero' WHERE SquareCatalogId = '104'
UPDATE SquaresCatalog SET Ciudad = 'Tepotzotlan', Estado = 'Estado de México' WHERE SquareCatalogId = '004' 
UPDATE SquaresCatalog SET Ciudad = 'Huehuetoca', Estado = 'Estado de México' WHERE SquareCatalogId = '069' 
UPDATE SquaresCatalog SET Ciudad = 'Polotitlan', Estado = 'Estado de México' WHERE SquareCatalogId = '070' 
UPDATE SquaresCatalog SET Ciudad = 'San Juan de Rio', Estado = 'Queretaro' WHERE SquareCatalogId = '005' 
UPDATE SquaresCatalog SET Ciudad = 'El Marques', Estado = 'Queretaro' WHERE SquareCatalogId = '127' 
UPDATE SquaresCatalog SET Ciudad = 'Queretaro', Estado = 'Queretaro' WHERE SquareCatalogId = '006' 
UPDATE SquaresCatalog SET Ciudad = 'Queretaro', Estado = 'Queretaro' WHERE SquareCatalogId = '061' 
UPDATE SquaresCatalog SET Ciudad = 'Villagran de los Figueroa', Estado = 'Guanajuato' WHERE SquareCatalogId = '183'
UPDATE SquaresCatalog SET Ciudad = 'Cerro Gordo', Estado = 'Guanajuato' WHERE SquareCatalogId = '186'
UPDATE SquaresCatalog SET Ciudad = 'Salamanca', Estado = 'Guanajuato' WHERE SquareCatalogId = '041'


--QUERYS KAREN MODIFICACIONES
CREATE TABLE Users_log
(
ID_USER INT,
OldName nvarchar(60),
NewName nvarchar(60),
OldPass nvarchar(100),
NewPass nvarchar(100),
OldMail nvarchar(100),
NewMail nvarchar(100),
UpdatedUser int,
UpdateDate datetime
)


CREATE TABLE AdminsSquares_Log
(
AdminId INT,
OldName nvarchar(60),
NewName nvarchar(60),
OldMail nvarchar(100),
NewMail nvarchar(100),
UpdatedUser int,
UpdateDate datetime
)



CREATE TABLE [dbo].[COMPONENTES_LOG](
        [ComponentID] [int] NULL,
        [ComponentsStockId] [int] NULL,
        [TableFolio] [int] NULL,
        [ReferenceNumber] [nvarchar](20) NULL,
        [AnexoReference] [nvarchar](20) NULL,
        [CapufeLaneNum] [nvarchar](10) NULL,
        [IdGare] [nvarchar](3) NULL,
        [RequestDate] [datetime] NULL,
        [InstallationDate] [date] NULL,
        [MaintenanceDate] [date] NULL,
        [EstatusFinal] [int] NULL,
        [Instalado] [bit] NULL,
        [FechaActualizacion] [datetime] NULL
) ON [PRIMARY]
GO