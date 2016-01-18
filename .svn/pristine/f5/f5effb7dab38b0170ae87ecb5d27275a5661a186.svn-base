
CREATE TABLE [dbo].[ITPEDIDOWEB](
	[IDPEDIDOWEB] [int] IDENTITY(1,1) NOT NULL,
	[NUMPEDWEB] [int] NOT NULL,
	[CODSERVMERC] [int] NOT NULL,
	[CODEMP] [int] NOT NULL,
	[CODUSU] [int] NOT NULL,
	[QTDE] [money] NOT NULL,
 CONSTRAINT [PK_ITPEDIDOWEB] PRIMARY KEY CLUSTERED 
(
	[IDPEDIDOWEB] ASC
)
) ON [PRIMARY]

GO


CREATE TABLE [dbo].[LOG_IMPORTACAO_WEB](
	[NUMPED] [int] NOT NULL,
	[CODCLI] [int] NOT NULL,
	[DATA]	 [date] NULL,
	[CODVEN] [int] NULL,
	[STATUS] [char](1) NULL,
	[MSG] [varchar](500) NULL,
	[VALOR] [float] NULL
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[LOG_ITENS_NAO_ATENDIDOS_WEB](
	[NUMPED] [int] NOT NULL,
	[CODSERVMERC] [int] NOT NULL,
	[QTD] [float] NULL,
	[VALOR] [float] NULL
) ON [PRIMARY]
GO

ALTER TABLE NUMPED ADD NUMPEDWEB INT NULL DEFAULT 0
GO
update numped set numpedweb=0
go

CREATE PROCEDURE [dbo].[sp_corrige_reserva_webpedido]
	
	
AS
BEGIN
	
	SET NOCOUNT ON;

	DECLARE @numped_aux AS INT
	DECLARE @mercadoria AS INT
	DECLARE @qtde		AS MONEY
	 
    
	DECLARE ITENS CURSOR FOR (SELECT numpedweb, codservmerc, qtde FROM ITPEDIDOWEB)

    OPEN ITENS FETCH NEXT FROM ITENS INTO @numped_aux, @mercadoria, @qtde

    WHILE @@fetch_status=0
      BEGIN
		  
		  IF NOT EXISTS ( SELECT 1 FROM PEDIDO WHERE NumPedWeb = @numped_aux )
			BEGIN
						
			   UPDATE SERVMERC SET QtdRes = (IsNull(QtdRes,0) - @qtde)
			   WHERE CodServMerc = @mercadoria
			   			
			END
			
		  
          FETCH NEXT FROM ITENS INTO @numped_aux, @mercadoria, @qtde
      END
     

      CLOSE ITENS
      DEALLOCATE ITENS

END

GO

/****** Object:  StoredProcedure [dbo].[SP_BloqueioPedidos]    Script Date: 08/07/2012 16:49:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[SP_BloqueioPedidos] 
(

@iCliente				AS INT,
@iPedido				AS INT,
@iFormaPagto			AS INT,
@iPrazoPagto			AS INT,
@iEmpresa				AS INT,
@cValorPedido			AS MONEY,
@dDataInadimplente		AS CHAR,
@bBloquearDebitos		AS INT,
@cMargemMinimaPedido	AS MONEY,
@cMargemLucro			AS MONEY

) AS

	/* Declaracao das variaveis */
     DECLARE @sValidaLimite			AS CHAR
     DECLARE @dDataBloqueio			AS CHAR
     DECLARE @cLimiteCredito		AS MONEY
     DECLARE @cValorDebitos			AS MONEY
     DECLARE @cValorCheques			AS MONEY
     DECLARE @cValorMinimo			AS MONEY     
     DECLARE @sBloqueioBBF			AS VARCHAR(20)
     DECLARE @sBloqueioBI			AS VARCHAR(20)
     DECLARE @sBloqueioBL			AS VARCHAR(20)
     DECLARE @sBloqueioBD			AS VARCHAR(20)
     DECLARE @sBloqueioBCP			AS VARCHAR(20)
     DECLARE @sBloqueioBMM			AS VARCHAR(20)
     DECLARE @sSql					AS NVARCHAR(4000)
     DECLARE @sMensagem				AS NVARCHAR(3000)
     
     /* Verifica se tem validacao do limite de credito */
     SELECT @sValidaLimite = ValidaLimCred FROM ITCONPAGTO 
     WHERE CodEmp = @iEmpresa AND CodFrmPgt = @iFormaPagto AND CodTipPrz = @iPrazoPagto
     
     /* Busca Valor Minimo para compras */
     SELECT @cValorMinimo = ValorMinimo FROM CONDPAGTO 
     WHERE CodTipPrz = @iPrazoPagto AND CodFrmPgt = @iFormaPagto AND CODEMP = @iEmpresa
        
     
     /* Verifica qual campo atualizar na tabela PEDIDO conforme o tipo de bloqueio.
	    Se for FIN, atualiza o campo StatusPedido.
	    Se for COM, atualiza o campo StatusComercial     
     */
     SELECT @sBloqueioBBF = CASE TIPOBLOQUEIOS WHEN 'FIN' THEN 'StatusPedido' ELSE 'StatusComercial' END 				
     FROM BLOQUEIOS WHERE SIGLABLOQUEIOS = 'BBF'     
     
     SELECT @sBloqueioBI = CASE TIPOBLOQUEIOS WHEN 'FIN' THEN 'StatusPedido' ELSE 'StatusComercial' END 				
     FROM BLOQUEIOS WHERE SIGLABLOQUEIOS = 'BI'
     
     SELECT @sBloqueioBL = CASE TIPOBLOQUEIOS WHEN 'FIN' THEN 'StatusPedido' ELSE 'StatusComercial' END 				
     FROM BLOQUEIOS WHERE SIGLABLOQUEIOS = 'BL'
     
     SELECT @sBloqueioBD = CASE TIPOBLOQUEIOS WHEN 'FIN' THEN 'StatusPedido' ELSE 'StatusComercial' END 				
     FROM BLOQUEIOS WHERE SIGLABLOQUEIOS = 'BD'
     
     SELECT @sBloqueioBCP = CASE TIPOBLOQUEIOS WHEN 'FIN' THEN 'StatusPedido' ELSE 'StatusComercial' END 				
     FROM BLOQUEIOS WHERE SIGLABLOQUEIOS = 'BCP'
     
     SELECT @sBloqueioBMM = CASE TIPOBLOQUEIOS WHEN 'FIN' THEN 'StatusPedido' ELSE 'StatusComercial' END 				
     FROM BLOQUEIOS WHERE SIGLABLOQUEIOS = 'BMM'
     
                  	
     /* Bloqueio por Cliente Bloqueado no Financeiro */
     /************************************************/
     SELECT @dDataBloqueio = DtaBloq, @cLimiteCredito = Isnull(LimCred,0) FROM FINANCLI WHERE CodCli = @iCliente
     
	 IF @dDataBloqueio <> '' AND @dDataBloqueio <> '1900-01-01 00:00:00.000'
		BEGIN
			
			SET @sSql = ''
			SET @sSql = @sSql + ' UPDATE PEDIDO '
			SET @sSql = @sSql + ' SET ' + @sBloqueioBBF + ' = ''BBF'' '
			SET @sSql = @sSql + ' WHERE NUMPED = ' + CAST(@iPedido AS NVARCHAR(8))
			SET @sSql = @sSql + ' AND CODEMP = ' + CAST(@iEmpresa AS NVARCHAR(5)) 
			EXEC SP_EXECUTESQL @sSql
			
			SET @sMensagem = 'Pedido Bloqueado por Bloqueio do Cliente no Financeiro. O Pedido será gravado, mas não será Faturado.'			
			PRINT @sMensagem 
			RETURN 1
			
		END
     
     
     /* Bloqueio por Cliente Inadimplente */ 
     /*************************************/ 
     
     IF @dDataInadimplente <> '' AND @dDataInadimplente <> '1900/01/01'
		BEGIN
		
			SET @sSql = ''
			SET @sSql = @sSql + ' UPDATE PEDIDO '
			SET @sSql = @sSql + ' SET ' + @sBloqueioBI + ' = ''BI'' '
			SET @sSql = @sSql + ' WHERE NUMPED = ' + CAST(@iPedido AS NVARCHAR(8))
			SET @sSql = @sSql + ' AND CODEMP = ' + CAST(@iEmpresa AS NVARCHAR(5)) 
			EXEC SP_EXECUTESQL @sSql
			
			SET @sMensagem =  'Pedido Bloqueado por Inadimplencia. O Pedido será gravado, mas não será Faturado.'
			PRINT @sMensagem 
			RETURN 2
		END
		
		
     /* Bloqueio por Limite de Credito */ 
     /*************************************/
      IF @cLimiteCredito > 0 
		 BEGIN
	
				SELECT @cValorDebitos = SUM((VlrDoc - VlrPago - (Desc1 + Desc2)) + AcrFin) 
				FROM TITRECEB 
				WHERE CodCli = @iCliente
				AND Status <> 'B'
				
				SELECT @cValorCheques = SUM(CHEQUES.valor)
				FROM CHEQUES 
				WHERE CHEQUES.codcli = @iCliente
				AND	CHEQUES.Tipo = 'R' 
				AND CHEQUES.Status = 'ABE'
						
				DECLARE @Total AS MONEY
				SET @Total = (CONVERT(MONEY,ISNULL(@cValorDebitos,0)) + CONVERT(MONEY,ISNULL(@cValorCheques,0)) + CONVERT(MONEY,ISNULL(@cValorPedido,0)))
				
				IF ((CONVERT(MONEY,@Total)) > CONVERT(MONEY,@cLimiteCredito)) 
					BEGIN
					
						SET @sSql = ''
						SET @sSql = @sSql + ' UPDATE PEDIDO '
						SET @sSql = @sSql + ' SET ' + @sBloqueioBL + ' = ''BL'' '
						SET @sSql = @sSql + ' WHERE NUMPED = ' + CAST(@iPedido AS NVARCHAR(8))
						SET @sSql = @sSql + ' AND CODEMP = ' + CAST(@iEmpresa AS NVARCHAR(5)) 
						EXEC SP_EXECUTESQL @sSql
						
						SET @sMensagem =  'Cliente com Limite de Crédito Excedido. O Pedido será gravado, mas não será Faturado.'
						PRINT @sMensagem 
						RETURN 3
					END
				
		 END
		 
	/* Bloqueio por Debitos */ 
    /*************************************/	 
	IF @bBloquearDebitos = -1 
		BEGIN

			SET @sSql = ''
			SET @sSql = @sSql + ' UPDATE PEDIDO '
			SET @sSql = @sSql + ' SET ' + @sBloqueioBD + ' = ''BD'' '
			SET @sSql = @sSql + ' WHERE NUMPED = ' + CAST(@iPedido AS NVARCHAR(8))
			SET @sSql = @sSql + ' AND CODEMP = ' + CAST(@iEmpresa AS NVARCHAR(5)) 
			EXEC SP_EXECUTESQL @sSql
						
			SET @sMensagem =  'Cliente Bloqueado por Débitos. O Pedido será gravado, mas não será Faturado.'		
			PRINT @sMensagem 
			RETURN 4
		END
		
		
	/* Bloqueio por Limite minimo nao atingido */ 
    /*************************************/	 		
	IF CONVERT(MONEY,@cValorPedido) < CONVERT(MONEY,ISNULL(@cValorMinimo,0))	
		BEGIN
			SET @sSql = ''
			SET @sSql = @sSql + ' UPDATE PEDIDO '
			SET @sSql = @sSql + ' SET ' + @sBloqueioBCP + ' = ''BCP'' '
			SET @sSql = @sSql + ' WHERE NUMPED = ' + CAST(@iPedido AS NVARCHAR(8))
			SET @sSql = @sSql + ' AND CODEMP = ' + CAST(@iEmpresa AS NVARCHAR(5)) 
			EXEC SP_EXECUTESQL @sSql
			
			SET @sMensagem =  'Valor mínimo para a Condição de Pagamento não atingido. O Pedido será gravado, mas não será Faturado.'
			PRINT @sMensagem 
			RETURN 5
			
		END	
		
		
	IF CONVERT(MONEY,ISNULL(@cMargemLucro,0)) <> 0 
		BEGIN
			IF CONVERT(MONEY,ISNULL(@cMargemMinimaPedido,0)) < CONVERT(MONEY,ISNULL(@cMargemLucro,0)) 
				BEGIN
					SET @sSql = ''
					SET @sSql = @sSql + ' UPDATE PEDIDO '
					SET @sSql = @sSql + ' SET ' + @sBloqueioBMM + ' = ''BMM'' '
					SET @sSql = @sSql + ' WHERE NUMPED = ' + CAST(@iPedido AS NVARCHAR(8))
					SET @sSql = @sSql + ' AND CODEMP = ' + CAST(@iEmpresa AS NVARCHAR(5)) 
					EXEC SP_EXECUTESQL @sSql
			
					SET @sMensagem = 'Pedido Bloqueado por Margem Mínima não atingida. O Pedido será gravado, mas não será Faturado.'
					PRINT @sMensagem 
					RETURN 6
				END				
		END
	
	SET @sMensagem = 'OK'	
	PRINT @sMensagem 	
	RETURN 7
	

GO
