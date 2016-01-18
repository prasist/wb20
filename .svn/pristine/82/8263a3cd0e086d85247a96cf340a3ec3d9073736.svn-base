-----------------------------------------------------------------------------------------------
--   Autor        : FABIANO
--   Data         : 28/08/2014
--   Descricao    : CRIACAO CAMPOS TABELA PARAMETROS
--   Solicitacao  : 51528
-----------------------------------------------------------------------------------------------
ALTER TABLE PARAMETROS ADD IPExterno varchar(300) null
GO
ALTER TABLE PARAMETROS ADD IPInterno varchar(300) null
GO
ALTER TABLE PARAMETROS ADD HostFtp varchar(300) null
GO
ALTER TABLE PARAMETROS ADD FtpUsuario varchar(30) null
GO
ALTER TABLE PARAMETROS ADD FtpSenha varchar(20) null
GO
ALTER TABLE PARAMETROS ADD PastaServidor varchar(100) null
GO

CREATE TABLE LOG_ARQUIVOS_RECEBIDOS (
	CODVEN INT NOT NULL,
	ARQUIVO VARCHAR(400) NULL,
	NOME_ARQUIVO	VARCHAR(80) NULL
) ON [PRIMARY]
GO
ALTER TABLE LOG_IMPORTACAO_WEB ADD NUMERO_OFFLINE VARCHAR(10) NULL
GO