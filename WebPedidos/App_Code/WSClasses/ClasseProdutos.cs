using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;

namespace WebPedidos.WSClasses
{
    public class ClasseProdutos
    {
        static decimal _cPrecoPromocao;
        static decimal _cPercPromocao;
        static string _sValidadePromocao;

        public static string sValidadePromocao
        {
            get { return _sValidadePromocao; }
            set { _sValidadePromocao = value; }
        }

        public static decimal cPrecoPromocao
        {
            get { return _cPrecoPromocao; }
            set { _cPrecoPromocao = value; }
        }

        public static decimal cPercPromocao
        {
            get { return _cPercPromocao; }
            set { _cPercPromocao = value; }
        }
        
        public static DataSet ListarProdutos(UsuarioResumido u, Int16 CodPrcTab, Int16 CodPrzTab)
        {
            ClasseBanco conn = new ClasseBanco();

            StringBuilder strSql = new StringBuilder();
            conn.AbrirBanco();

            strSql.Length = 0;

            //BUSCAR PRECO DO PRODUTO NA TABELA PADRAO
            strSql.Append(" SELECT S.CodServMerc AS Codigo, S.DesServMerc as Nome, S.QtdEmb, S.PercDesc as percentDesconto, ISNULL(T.PRECO,0) AS Preco, S.Saldo, S.Comissao, S.CaminhoImagem ");
            strSql.Append(" FROM ");
            strSql.Append(" SERVMERC S INNER JOIN TABPRECO T ON S.CODSERVMERC = T.CODSERVMERC ");
            strSql.Append(" LEFT JOIN REGRAS_PRODUTO R ON S.CODSERVMERC = R.CODPRO ");
            strSql.Append(" WHERE ISNULL(R.REGRA1,0)=0 AND CODEMP = " + u.CodEmp + " AND S.ATIVO = 'S'");

            if (!CodPrcTab.Equals(0) && !CodPrzTab.Equals(0))
            {
                strSql.Append(" AND CodTipPrc = " + CodPrcTab + " AND CodTipPrz = " + CodPrzTab + "");
            }

            strSql.Append(" ORDER BY S.DESSERVMERC ");

            var r = conn.retornaQueryDataSet(strSql.ToString());

            return r;

        }
        public static List<ProdutoResumido> Produto(UsuarioResumido u, Int32 iCodProduto, ParametroResumido pr, Int16 CodPrcTab, Int16 CodPrzTab)
        {
            DataClassesDataContext dcdc = new DataClassesDataContext();
            ClasseBanco conn = new ClasseBanco();

            decimal var_preco = 0;
            var strSql = "";
            conn.AbrirBanco();

            //BUSCAR PRECO DO PRODUTO NA TABELA PADRAO
            strSql = "SELECT ISNULL(T.PRECO,0) AS Preco FROM ";
            strSql = strSql + " TABPRECO T LEFT JOIN REGRAS_PRODUTO R ON T.CODSERVMERC = R.CODPRO ";
            strSql = strSql + " WHERE ISNULL(R.REGRA1,0)=0 AND CODSERVMERC = " + iCodProduto + " AND CODEMP = " + u.CodEmp + "";

            if (!CodPrcTab.Equals(0) && !CodPrzTab.Equals(0))
            {
                strSql = strSql + " AND CodTipPrc = " + CodPrcTab + " AND CodTipPrz = " + CodPrzTab + "";
            }

            var r = conn.Query(strSql);

            if (r.Read())
            {
                var_preco = Convert.ToDecimal(String.Format("{0:" + Funcoes.Decimais(pr) + "}", r[0].ToString()));
            }
            r.Close();

            //Verifica se produto est� em promo��o ou pra mocinha...
            VerificaSeTemPromocao(iCodProduto, CodPrcTab);

            if (cPrecoPromocao!=0) {
                var_preco = cPrecoPromocao;
                
            } else if (cPercPromocao!=0) {
                var_preco = var_preco - (var_preco * cPercPromocao / 100);                  
            }
            
            List<ProdutoResumido> produtos = new List<ProdutoResumido>();

            try
            {

                dcdc.SERVMERCs.Where(s => s.CodServMerc == iCodProduto).Where(s => s.Ativo.Equals("S")).OrderBy(s => s.DesServMerc).ToList().ForEach(s => produtos.Add(
                        new ProdutoResumido(
                                                s.CodServMerc, (s.DesServMerc == null ? "" : s.DesServMerc),
                                                (var_preco == 0 ? (decimal)s.PrecoBase : var_preco), //PRECO DA TABELA
                                                0,
                                                0,
                                                s.Saldo == null ? 0 : Convert.ToDecimal(s.Saldo) - (s.QtdRes == null ? 0 : (Convert.ToDecimal(s.QtdRes) <= 0 ? 0 : Convert.ToDecimal(s.QtdRes))),
                                                s.Comissao == null ? 0 : Convert.ToDecimal(s.Comissao),
                                                0,
                                                s.DescCliente > 0 ? Convert.ToDecimal(s.DescCliente) : (s.PercDesc == null ? 0 : Convert.ToDecimal(s.PercDesc)),
                                                s.ComissaoTel == null ? 0 : Convert.ToDecimal(s.ComissaoTel),
                                                Convert.ToDecimal(s.QtdCaixa), s.M_Unidade, s.Unidade, "0",
                                                Convert.ToDecimal(String.Format("{0:" + Funcoes.Decimais(pr) + "}", Convert.ToDecimal(s.PesoBruto))), sValidadePromocao, CodPrzTab,0,0,0,"")

                                            ));


                if (produtos.Count > 0)
                {

                    // retorna os itens da mercadoria selecionada em outros pedidos  //
                    var Itens = (from o in dcdc.ITPEDIDOWEBs
                                 where (o.CODSERVMERC == produtos[0].Codigo)
                                 where (o.CODEMP != u.CodEmp || o.CODUSU != u.CodUsu)
                                 select new
                                 {
                                     o.CODSERVMERC,
                                     o.QTDE
                                 }).ToList();

                    // soma a quantidade dos itens retornados //
                    var Soma = (decimal)Itens.Select(c => c.QTDE).Sum();

                    // atualiza a quantidade do saldo do produto na cole��o //
                    produtos[0].Saldo -= Soma;

                    dcdc.Dispose();
                }
                else
                {
                    produtos = null;
                }
            }
            catch (Exception exps)
            {
                throw new Exception(exps.ToString());
            }
                return produtos;
            
        }

        public static void VerificaSeTemPromocao(Int32 iCodigo, Int16 iTipoPreco)
        {
            ClasseBanco conn = new ClasseBanco();

            conn.AbrirBanco();

            cPercPromocao = 0;
            cPrecoPromocao = 0;
            sValidadePromocao = "";

            string strSql;

            strSql = "SELECT DtaFim, DtaIni, PercProm, PrecoProm,Comissao ";
            strSql = strSql + " FROM ITPROMOCAO WHERE CodServMerc = " + iCodigo;
            strSql = strSql + " and CodTipPrc = " + iTipoPreco;
            var r = conn.Query(strSql);

            while (r.Read())
            {
                if (Convert.ToDateTime(String.Format("{0:d}", DateTime.Now)) >= Convert.ToDateTime(String.Format("{0:d}", r["DtaIni"])) && Convert.ToDateTime(String.Format("{0:d}", DateTime.Now)) <= Convert.ToDateTime(String.Format("{0:d}", r["DtaFim"])))
                {
                    cPercPromocao     = Convert.ToDecimal(r["PercProm"].ToString());
                    cPrecoPromocao    = Convert.ToDecimal(r["Precoprom"].ToString());
                    sValidadePromocao = "Pre�o promocional no per�odo de " + String.Format("{0:d}", r["DtaIni"]) + " at� " + String.Format("{0:d}", r["DtaFim"]);
                }
            }
            r.Close();

        }

        public static DataSet buscaProdutosPorNome(UsuarioResumido u, ParametroResumido pr, string nome, Int16 CodPrcTab, Int16 CodPrzTab)
        {
            ClasseBanco conn = new ClasseBanco();

            StringBuilder strSql = new StringBuilder();

            conn.AbrirBanco();
            
            strSql.Length = 0;
            
            //BUSCAR PRECO DO PRODUTO NA TABELA PADRAO
            strSql.Append(" SELECT S.UNIDADE, S.M_UNIDADE, S.QTDCAIXA, S.CodServMerc AS Codigo, (replicate(0, 9 - len(CAST(S.CODSERVMERC AS VARCHAR)))+ CAST(S.CODSERVMERC AS VARCHAR) + ' - ' + rtrim(S.DesServMerc) + ' - ' + rtrim(upper(Isnull(CodSec_ServMerc,0))) + ' x ' + convert(varchar,Isnull(S.QtdEmb,0))) as Nome, S.QtdEmb, S.PercDesc as percentDesconto, ISNULL(T.PRECO, S.PRECOBASE) AS Preco, S.Saldo, S.PESOBRUTO, S.Comissao, S.CaminhoImagem  FROM ");
            //strSql.Append(" SELECT S.UNIDADE, S.M_UNIDADE, S.QTDCAIXA, S.CodServMerc AS Codigo, (replicate(0, 9 - len(CAST(S.CODSERVMERC AS VARCHAR)))+ CAST(S.CODSERVMERC AS VARCHAR) + ' - ' + rtrim(S.DesServMerc)) as Nome, S.QtdEmb, S.PercDesc as percentDesconto, ISNULL(T.PRECO,0) AS Preco, S.Saldo FROM ");
            strSql.Append(" SERVMERC S INNER JOIN TABPRECO T ON S.CODSERVMERC = T.CODSERVMERC ");
            strSql.Append(" LEFT JOIN REGRAS_PRODUTO R ON S.CODSERVMERC = R.CODPRO ");
            strSql.Append(" WHERE ISNULL(R.REGRA1,0)=0  AND ");
            strSql.Append(" S.DESSERVMERC LIKE '%" + nome.ToLower() + "%' AND ");
            strSql.Append(" CODEMP = " + u.CodEmp + "  ");

            if (!CodPrcTab.Equals(0) && !CodPrzTab.Equals(0))
            {
                strSql.Append(" AND CodTipPrc = " + CodPrcTab + " ");
                strSql.Append(" AND CodTipPrz = " + CodPrzTab + " ");
            }
            
            strSql.Append(" AND S.ATIVO = 'S' ");
            strSql.Append(" ORDER BY S.DESSERVMERC");

            var r = conn.retornaQueryDataSet(strSql.ToString());

            if (r.Tables[0].Rows.Count.Equals(0))
            {
                throw new Exception("N�o h� mercadoria(s) para a tabela de pre�o selecionada ou o(s) produto(s) est�(�o) inativo(s).");                
            }

            return r;

        }

        public static List<ProdutoResumido> buscaProdutosPorCodigo(UsuarioResumido u, ParametroResumido pr, Int32 iCodServMerc, Int16 CodPrcTab, Int16 CodPrzTab)
        {

            ClasseBanco             conn    = new ClasseBanco();
            StringBuilder           strSql  = new StringBuilder();
            List<ProdutoResumido>   retorno = new List<ProdutoResumido>();
            ProdutoResumido         p       = new ProdutoResumido();

            conn.AbrirBanco();

            decimal dSaldo = 0;
            strSql.Length = 0;

            strSql.Append(" SELECT S.UNIDADE, S.M_UNIDADE, S.QTDCAIXA, S.CodServMerc AS Codigo, (replicate(0, 9 - len(CAST(S.CODSERVMERC AS VARCHAR)))+ CAST(S.CODSERVMERC AS VARCHAR) + ' - ' + rtrim(S.DesServMerc) + ' - ' + rtrim(upper(Isnull(CodSec_ServMerc,0))) + ' x ' + convert(varchar,Isnull(S.QtdEmb,0))) as Nome, S.QtdEmb, S.PercDesc as percentDesconto, ISNULL(T.PRECO, S.PRECOBASE) AS Preco, S.Saldo, S.CodSec_ServMerc, S.QTDRES, S.PESOBRUTO, S.Comissao, S.CaminhoImagem FROM ");
            strSql.Append(" SERVMERC S INNER JOIN TABPRECO T ON S.CODSERVMERC = T.CODSERVMERC ");
            strSql.Append(" LEFT JOIN REGRAS_PRODUTO R ON S.CODSERVMERC = R.CODPRO ");
            strSql.Append(" WHERE ISNULL(R.REGRA1,0)=0  AND ");
            strSql.Append(" S.CODSERVMERC = " + iCodServMerc + " AND ");
            strSql.Append(" CODEMP = " + u.CodEmp + " ");

            if (!CodPrcTab.Equals(0))
            {
                strSql.Append(" AND CodTipPrc = " + CodPrcTab + "  ");                
            }

            if (!CodPrzTab.Equals(0))
            {
                strSql.Append(" AND CodTipPrz = " + CodPrzTab + "  ");
            }
            

            strSql.Append(" AND S.ATIVO = 'S' ");
            strSql.Append(" ORDER BY S.DESSERVMERC");

            using (var r = conn.Query(strSql.ToString()))
            {

                if (r.Read())
                {
                    p.Codigo = Convert.ToInt32(r["Codigo"].ToString());
                    p.Nome = r["Nome"] + " - " + r["CodSec_ServMerc"];// +" x " + r["QtdEmb"];
                    p.percentDesconto = Convert.ToDecimal(r["percentDesconto"]);
                    p.Preco = Convert.ToDecimal(r["Preco"].ToString()); ;
                    dSaldo = (r["Saldo"] == null ? 0 : Convert.ToDecimal(r["Saldo"])) - (r["QtdRes"] == null ? 0 : Convert.ToDecimal(r["QtdRes"]));
                    p.Saldo = (dSaldo == null ? 0 : dSaldo);
                    p.M_UNIDADE = r["M_UNIDADE"].ToString();
                    p.Unidade = r["UNIDADE"].ToString();
                    p.QtdCaixa = Convert.ToDecimal(r["QTDCAIXA"].ToString());
                    p.Peso = Convert.ToDecimal(r["PESOBRUTO"].ToString());
                    p.Comissao = Convert.ToDecimal(r["COMISSAO"].ToString());
                    p.caminhoimagem = r["CaminhoImagem"].ToString();
                    //Verifica se produto est� em promo��o ou pra mocinha...
                    VerificaSeTemPromocao(iCodServMerc, CodPrcTab);

                    if (cPrecoPromocao != 0)
                    {
                        p.Preco = cPrecoPromocao;
                        p.MensagemPromocao = sValidadePromocao;
                    }
                    else if (cPercPromocao != 0)
                    {
                        p.Preco = p.Preco - (p.Preco * cPercPromocao / 100);
                        p.MensagemPromocao = sValidadePromocao;
                    }

                    retorno.Add(p);
                }
                else
                {
                    throw new Exception("N�o h� mercadoria(s) para a tabela de pre�o selecionada ou o(s) produto(s) est�(�o) inativo(s).");
                }
            }
            return retorno;
            
              
        }

        public static DataSet buscarProdutosPorGrupo(UsuarioResumido u, ParametroResumido pr, Int16 codGru, Int16 CodPrcTab, Int16 CodPrzTab)
        {

            ClasseBanco conn = new ClasseBanco();

            StringBuilder strSql = new StringBuilder();
            conn.AbrirBanco();
            
            strSql.Length = 0;
            
            /*BUSCAR PRECO DO PRODUTO NA TABELA PADRAO*/
            strSql.Append(" SELECT S.UNIDADE, S.M_UNIDADE, S.QTDCAIXA, S.CodServMerc AS Codigo, S.DesServMerc as Nome, S.QtdEmb, S.PercDesc as percentDesconto, ISNULL(T.PRECO, S.PRECOBASE) AS Preco, S.Saldo, S.PESOBRUTO, S.Comissao, S.CaminhoImagem  FROM ");
            strSql.Append(" SERVMERC S INNER JOIN TABPRECO T ON S.CODSERVMERC = T.CODSERVMERC ");
            strSql.Append(" LEFT JOIN REGRAS_PRODUTO R ON S.CODSERVMERC = R.CODPRO ");
            strSql.Append(" WHERE ISNULL(R.REGRA1,0) = 0  AND ");
            strSql.Append(" S.CODGRU = " + codGru + " AND ");
            strSql.Append(" CODEMP   = " + u.CodEmp + " ");

            if (!CodPrcTab.Equals(0) && !CodPrzTab.Equals(0))
            {
                strSql.Append(" AND CodTipPrc                = " + CodPrcTab + " ");
                strSql.Append(" AND CodTipPrz            = " + CodPrzTab + " ");
            }
            
            strSql.Append(" AND S.ATIVO                  = 'S'");
            strSql.Append(" ORDER BY S.DESSERVMERC");

            var r = conn.retornaQueryDataSet(strSql.ToString());

            if (r.Tables[0].Rows.Count.Equals(0))
            {
                throw new Exception("N�o h� mercadoria(s) para a tabela de pre�o selecionada ou o(s) produto(s) est�(�o) inativo(s).");                
            }
            else            
            {
                return r;
            }                        

        }
    }
}
