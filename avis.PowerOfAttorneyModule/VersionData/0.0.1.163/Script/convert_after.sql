UPDATE dbo.Sungero_Content_EDoc
SET FpoaKindlenspe_Etalon_lenspec = 'NonState'
FROM dbo.Sungero_Content_EDoc
WHERE FpoaKindlenspe_Etalon_lenspec IS NULL AND Discriminator = '104472DB-B71B-42A8-BCA5-581A08D1CA7B';