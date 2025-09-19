update Sungero_Content_EDoc
set LettersStatus_Outgoin_lenspec = 'Formed'
where Sungero_Content_EDoc.Discriminator = 'FA18BE6A-226C-4868-B24A-1AE60E0C30A4' and Sungero_Content_EDoc.LettersStatus_Outgoin_lenspec is null and Sungero_Content_EDoc.Id in
(
	select sourceDoc.Id
	from Sungero_Content_EDoc sourceDoc
	inner join Sungero_Content_Relation on Sungero_Content_Relation.Source = sourceDoc.Id
	inner join Sungero_Content_EDoc targetDoc on Sungero_Content_Relation.Target = targetDoc.Id
	inner join Sungero_Core_RelationType on Sungero_Content_Relation.RelationType = Sungero_Core_RelationType.Id
	where sourceDoc.Discriminator = 'FA18BE6A-226C-4868-B24A-1AE60E0C30A4' and targetDoc.Discriminator = 'D1D2A452-7732-4BA8-B199-0A4DC78898AC' and Sungero_Core_RelationType.Name = 'Addendum'
);
update Sungero_Content_EDoc
set LettersStatus_Outgoin_lenspec = 'No'
where Sungero_Content_EDoc.Discriminator = 'FA18BE6A-226C-4868-B24A-1AE60E0C30A4' and Sungero_Content_EDoc.LettersStatus_Outgoin_lenspec is null and Sungero_Content_EDoc.Id not in
(
	select sourceDoc.Id
	from Sungero_Content_EDoc sourceDoc
	inner join Sungero_Content_Relation on Sungero_Content_Relation.Source = sourceDoc.Id
	inner join Sungero_Content_EDoc targetDoc on Sungero_Content_Relation.Target = targetDoc.Id
	inner join Sungero_Core_RelationType on Sungero_Content_Relation.RelationType = Sungero_Core_RelationType.Id
	where sourceDoc.Discriminator = 'FA18BE6A-226C-4868-B24A-1AE60E0C30A4' and targetDoc.Discriminator = 'D1D2A452-7732-4BA8-B199-0A4DC78898AC' and Sungero_Core_RelationType.Name = 'Addendum'
);