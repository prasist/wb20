-----------------------------------------------------------------------------------------------
--   Autor        : FABIANO
--   Data         : 11/09/2014
--   Descricao    : CRIACAO NOVOS CAMPOS TABELA PARAMETROS
--   Solicitacao  : 51642
-----------------------------------------------------------------------------------------------
ALTER TABLE PARAMETROS ADD CaminhoArquivosXmlWeb varchar(300) null
GO
ALTER TABLE PARAMETROS ADD IntervaloSincronizacaoXml_hora smallint null
GO
ALTER TABLE PARAMETROS ADD IntervaloSincronizacaoXml_minutos smallint null
GO
UPDATE PARAMETROS SET CaminhoArquivosXmlWeb = 'C:\SIGMA\WEBPEDIDOS\XML'
GO
UPDATE PARAMETROS SET IntervaloSincronizacaoXml_hora = 0
GO
UPDATE PARAMETROS SET IntervaloSincronizacaoXml_minutos = 1
GO