----------------------------------------------------------------------------
-- Autor.......: FABIANO
-- Data........: 16/05/2013
-- Descricao...: PARAMETROS PARA WEBPEDIDOS
-- Solicitacao.: 48426
-- Tela........: FRMPARAMFERRAMENTAS
----------------------------------------------------------------------------
ALTER TABLE PARAMETROS ADD EnviarEmailWeb smallint NULL
GO
ALTER TABLE PARAMETROS ADD HostSmtp VARCHAR(120) NULL
GO
ALTER TABLE PARAMETROS ADD PortaSmtp VARCHAR(6) NULL
GO
ALTER TABLE PARAMETROS ADD UsuarioSmtp VARCHAR(120) NULL
GO
ALTER TABLE PARAMETROS ADD SenhaSmtp VARCHAR(20) NULL
GO




