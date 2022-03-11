CREATE TABLE CatalogoUserAnexo(
	Id INT NOT NULL  IDENTITY(1,1) PRIMARY KEY,
	Nombre NVARCHAR(40) NOT NULL,
	SquareId NVARCHAR(4) NOT NULL,
	RollId INT NOT NULL,
	FOREIGN KEY(SquareId) REFERENCES SquaresCatalog(SquareCatalogId),
	FOREIGN KEY(RollId) REFERENCES RollsCatalog(RollId),
)

--LLENAR CATALOGO ANEXO PARA PRUEBAS
INSERT INTO RollsCatalog VALUES(11, 'Testigos')
INSERT INTO RollsCatalog VALUES(12, 'Supervisor')
INSERT INTO CatalogoUserAnexo VALUES('SUPERVISOR', '001', 11)
INSERT INTO CatalogoUserAnexo VALUES('TESTIGO 1', '001', 12)
INSERT INTO CatalogoUserAnexo VALUES('TESTIGO 2', '001', 12)

CREATE TABLE AnexosDTC(
	DTCReference NVARCHAR(20) NOT NULL,
	AnexoReference NVARCHAR(20) NOT NULL PRIMARY KEY,
	FechaApertura DATETIME NOT NULL,
	FechaCierre DATETIME NOT NULL,
	Solicitud NVARCHAR(50) NULL,
	FechaSolicitudInicio DATETIME NULL,
	FechaSolicitudFin DATETIME NULL,
	FolioOficio NVARCHAR(50) NULL,
    FechaOficioInicio DATETIME NULL,
    FechaOficioFin DATETIME NULL,
	Testigo1Id INT NOT NULL,
	Testigo2Id INT NOT NULL,
	SupervisorId INT NOT NULL,	
    FechaUltimoCambio DATETIME NULL,
    Comentarios NVARCHAR(300) NULL,
    TipoAnexo CHAR NOT NULL,
    Activo BIT,
	IsSubVersion BIT NOT NULL,
	PDFFirmardo BIT NOT NULL DEFAULT(0),
	PDFFotografico BIT NOT NULL DEFAULT(0)

	FOREIGN KEY(DTCReference) REFERENCES DTCData(ReferenceNumber),
	FOREIGN KEY(Testigo1Id) REFERENCES CatalogoUserAnexo(Id),
	FOREIGN KEY(Testigo2Id) REFERENCES CatalogoUserAnexo(Id),
	FOREIGN KEY(SupervisorId) REFERENCES CatalogoUserAnexo(Id)
)


--CREATE TABLE TestigosAnexo(
--	TestigoId INT NOT NULL,
--	AnexoId NVARCHAR(20) NOT NULL,
--	PRIMARY KEY(TestigoId,AnexoId),
--	FOREIGN KEY(TestigoId) REFERENCES CatalogoUserAnexo(Id),
--	FOREIGN KEY(AnexoId) REFERENCES AnexosDTC(AnexoReference)
--)

CREATE TABLE ComponentAnexo(
	Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	AnexoId NVARCHAR(20) NOT NULL,
	ComponentDTCId INT NOT NULL,
	NumeroSerie NVARCHAR(30) NOT NULL,	
	FOREIGN KEY(AnexoId) REFERENCES AnexosDTC(AnexoReference),
	FOREIGN KEY(ComponentDTCId) REFERENCES RequestedComponents(RequestedComponentId)
)

--NUEVA COLUMNA PARA VALIDAR COMPONENTES OCUPADOS
ALTER TABLE RequestedComponents ADD UseInAnexo BIT NOT NULL DEFAULT(0)


--NUEVO STORE PARA PODER LLENAR EL HEADER DEL ANEXO
ALTER PROCEDURE InsertHeaderAnexo
	@referenceDTC NVARCHAR(20),
	@referenceAnexoAnterior NVARCHAR(20),
	@referenceAnexo NVARCHAR(20),
	@fechaApertura DATETIME,
	@fechaCierre DATETIME,
	@folioOficio NVARCHAR(50),
	@fechaOficioInicio DATETIME,
	@fechaOficioFin DATETIME,	
	@supervisorId INT,
	@testigo1 INT,
	@testigo2 INT,
	@tipoAnexo CHAR,
	@isSubVersion BIT
AS
BEGIN TRY
	IF @tipoAnexo = 'A'
	BEGIN
		INSERT INTO AnexosDTC(DTCReference, AnexoReference, FechaApertura, FechaCierre, FolioOficio, FechaOficioInicio, FechaOficioFin, SupervisorId, Activo, TipoAnexo, FechaUltimoCambio, IsSubVersion) 
		VALUES(@referenceDTC, @referenceAnexo, @fechaApertura, @fechaCierre, @folioOficio, @fechaOficioInicio, @fechaOficioFin, @supervisorId, 1, 'A', GETDATE(), @isSubVersion)	
		
		INSERT INTO TestigosAnexo VALUES(@testigo1, @referenceAnexo)
		INSERT INTO TestigosAnexo VALUES(@testigo2, @referenceAnexo)	
		
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
		INSERT INTO AnexosDTC(DTCReference, AnexoReference, FechaApertura, FechaCierre, FolioOficio, FechaOficioInicio, FechaOficioFin, SupervisorId, Activo, TipoAnexo, FechaUltimoCambio, IsSubVersion) 
		VALUES(@referenceDTC, @referenceAnexo, @fechaApertura, @fechaCierre, @folioOficio, @fechaOficioInicio, @fechaOficioFin, @supervisorId, 1, 'B', GETDATE(), @isSubVersion)	

		INSERT INTO TestigosAnexo VALUES(@testigo1, @referenceAnexo)
		INSERT INTO TestigosAnexo VALUES(@testigo2, @referenceAnexo)	

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
	SELECT NULL AS SqlResult, 'Línea: ' + CAST(ERROR_LINE() AS VARCHAR) + ' ' + CAST(@@ERROR AS VARCHAR) AS SqlMessage
END CATCH
GO


--NUEVO STORE PARA CONTAR LAS VERSIONES DE UN ANEXO SE OCUPA EN METODO PARA GENERAR LOS IDS IMPORTANTE!!
ALTER PROCEDURE GetConteoVersionesAnexos
	@referenceDTC NVARCHAR(20),
	@referenceAnexo NVARCHAR(20),
	@isSubVersion BIT
AS
	IF @isSubVersion = 1
	BEGIN
		SELECT AnexoReference FROM AnexosDTC WHERE DTCReference = @referenceDTC AND SUBSTRING(AnexoReference, 0, LEN(@referenceAnexo) + 1) = @referenceAnexo AND IsSubVersion = 1 ORDER BY AnexoReference DESC 
	END
	ELSE
	BEGIN 
		SELECT AnexoReference FROM AnexosDTC WHERE DTCReference = @referenceDTC AND IsSubVersion = 0 ORDER BY AnexoReference DESC 
	END
GO

--NUEVO STORE PARA PODER LLENAR EL COMPONENTE DEL ANEXO
ALTER PROCEDURE InsertComponentesAnexo
	@referenceNumber NVARCHAR(20),	
	@AnexoId NVARCHAR(20),
	@ComponentDTCId INT,
	@NumeroSerie NVARCHAR(30)
AS
	BEGIN TRY
		INSERT INTO ComponentAnexo VALUES(@AnexoId, @ComponentDTCId, @NumeroSerie)
		UPDATE RequestedComponents SET UseInAnexo = 1 WHERE ReferenceNumber = @referenceNumber AND RequestedComponentId = @ComponentDTCId
		SELECT 'Insertado' SqlMessage, 'Se inserto un componente al anexo: '+  @AnexoId  SqlResult
	END TRY
	BEGIN CATCH
		SELECT NULL AS SqlResult, 'Línea: ' + CAST(ERROR_LINE() AS VARCHAR) + ' ' + CAST(@@ERROR AS VARCHAR) AS SqlMessage
	END CATCH
GO

--NUEVO STORE PARA PODER LLENAR EL COMPONENTE DEL ANEXO
ALTER PROCEDURE GetHistoricoAnexo
	@referenceNumber NVARCHAR(20)
AS
	SELECT * FROM AnexosDTC WHERE DTCReference = @referenceNumber ORDER BY AnexoReference
GO

--NUEVO STORE PARA LLENAR BUSCAR LOS TESTIGOS POR PLAZA
ALTER PROCEDURE GetTestigosPlaza
	@plazaId NVARCHAR(4)
AS
	--11 IDROLL DE TESTIGOS
	SELECT * FROM CatalogoUserAnexo WHERE SquareId = @plazaId AND RollId = 11
GO

--NUEVO STORE PARA LLENAR BUSCAR LOS TESTIGOS POR PLAZA
ALTER PROCEDURE GetSupervisorAnexoPlaza
	@plazaId NVARCHAR(4)
AS
	--12 IDROLL DE SUPERVISOR ANEXO
	SELECT * FROM CatalogoUserAnexo WHERE SquareId = @plazaId AND RollId = 12
GO

CREATE PROCEDURE InsertSupervisor
	@nombreSupervisor NVARCHAR(40),
	@numeroPlaza NVARCHAR(4)
AS	
	BEGIN TRY
		INSERT INTO CatalogoUserAnexo VALUES(@nombreSupervisor, @numeroPlaza, 11)
		SELECT 'Insertado' SqlMessage, 'Se inserto el usuario anexo: '+  @nombreSupervisor + 'en la plaza' + @numeroPlaza  SqlResult
	END TRY
	BEGIN CATCH
		SELECT NULL AS SqlResult, 'Línea: ' + CAST(ERROR_LINE() AS VARCHAR) + ' ' + CAST(@@ERROR AS VARCHAR) AS SqlMessage
	END CATCH
GO

CREATE PROCEDURE InsertTestigo
	@nombreTestigo NVARCHAR(40),
	@numeroPlaza NVARCHAR(4)
AS	
	BEGIN TRY
		INSERT INTO CatalogoUserAnexo VALUES(@nombreTestigo, @numeroPlaza, 12)
		SELECT 'Insertado' SqlMessage, 'Se inserto el usuario anexo: '+  @nombreTestigo + 'en la plaza' + @numeroPlaza  SqlResult
	END TRY
	BEGIN CATCH
		SELECT NULL AS SqlResult, 'Línea: ' + CAST(ERROR_LINE() AS VARCHAR) + ' ' + CAST(@@ERROR AS VARCHAR) AS SqlMessage
	END CATCH
GO

ALTER PROCEDURE GetComponentesAnexo
	@referenceAnexo NVARCHAR(20)
AS	
	DECLARE @ultimaVersionAnexo NVARCHAR(20)
	DECLARE @conteoVersion INT
	
	SELECT @conteoVersion = COUNT(*) FROM AnexosDTC WHERE SUBSTRING(AnexoReference, 0, LEN(@referenceAnexo) + 1) = @referenceAnexo AND IsSubVersion = 1

	IF @conteoVersion > 0
	BEGIN	
		SElECT TOP 1 @ultimaVersionAnexo = AnexoReference FROM AnexosDTC WHERE SUBSTRING(AnexoReference, 0, LEN(@referenceAnexo) + 1) = @referenceAnexo AND IsSubVersion = 1 ORDER BY AnexoReference DESC 
		SELECT * FROM ComponentAnexo WHERE AnexoId = @ultimaVersionAnexo
	END
	ELSE
	BEGIN		
		SELECT * FROM ComponentAnexo WHERE AnexoId = @referenceAnexo
	END
GO

ALTER PROCEDURE GetCompRequestAnexo
	@referenceNumber NVARCHAR(20)
AS
	SELECT R.RequestedComponentId,
		S.Description AS NameComponent,
		R.Brand, 		
		R.Model, 		
		R.SerialNumber, 
		L.Lane,
		D.Observation,
		R.UseInAnexo
	FROM RequestedComponents R 
		INNER JOIN ComponentsStock S 	
	ON R.ComponentsStockId = S.ComponentsStockId
		INNER JOIN LanesCatalog L
	ON R.IdGare = L.IdGare AND R.CapufeLaneNum = L.CapufeLaneNum
		INNER JOIN DTCData D
	ON R.ReferenceNumber = D.ReferenceNumber
	WHERE R.ReferenceNumber = @referenceNumber
GO


ALTER PROCEDURE GetHeaderAnexo
	@referenceAnexo NVARCHAR(20)
AS
	DECLARE @ultimaVersionAnexo NVARCHAR(20)
	DECLARE @conteoVersion INT
	
	SELECT @conteoVersion = COUNT(*) FROM AnexosDTC WHERE SUBSTRING(AnexoReference, 0, LEN(@referenceAnexo) + 1) = @referenceAnexo AND IsSubVersion = 1

	--SI ES MAYOR A 0 ES QUE TIENE SUBVERSION SE DEBE BUSCAR LA ULTIMA SUBVERSION
	IF @conteoVersion > 0
	BEGIN		
		SElECT TOP 1 @ultimaVersionAnexo = AnexoReference FROM AnexosDTC WHERE SUBSTRING(AnexoReference, 0, LEN(@referenceAnexo) + 1) = @referenceAnexo AND IsSubVersion = 1 ORDER BY AnexoReference DESC 	
		SELECT * FROM AnexosDTC WHERE AnexoReference = @ultimaVersionAnexo
	END
	ELSE 
	BEGIN
		SELECT * FROM AnexosDTC WHERE AnexoReference = @referenceAnexo
	END
GO

