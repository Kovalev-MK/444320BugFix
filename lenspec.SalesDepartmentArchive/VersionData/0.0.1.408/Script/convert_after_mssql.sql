UPDATE Sungero_Content_EDoc
SET ClientNames_SalesDe_lenspec = 
STUFF
(
	(
		SELECT '; ' + 
		(
			SELECT Sungero_Parties_Counterparty.Name
			FROM Sungero_Parties_Counterparty
			WHERE Sungero_Parties_Counterparty.Id = ClientItem
		)
		FROM lenspec_SalesDe_SDAClientDocum t2
		WHERE t2.EDoc = Sungero_Content_EDoc.Id
		FOR XML PATH ('')
	), 
	1,
	2, 
	''
)
WHERE Sungero_Content_EDoc.Discriminator = 'c4f798a1-be37-4701-8036-f81e45f44e5d';