USE [BitacoraProsis]
GO
/****** Object:  StoredProcedure [dbo].[spGetDTCView]    Script Date: 18/02/2022 08:30:09 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

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
	where d.StatusId >= 2 AND (d.UserId NOT IN (1,37,5,7,63,71))  
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
	and (d.UserId NOT IN (1, 37, 5, 70, 7,63,71))
	and d.StatusId != 0  order by d.DateStamp desc

end

