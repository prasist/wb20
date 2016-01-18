using System;
using System.Data;
using System.Data.SqlClient;

namespace WebPedidos.WSClasses
{

    public class ClasseBloqueioFinancerio
    {
        public string BloqueioFinancerio(
            int Cliente,
            Int32 Pedido,
            int FormaPagto,
            int PrazoPagto,
            int Empresa,
            string ValorPedido,
            string DataInadimplente,
            int BloquearDebitos,
            decimal MargemMinimaPedido,
            decimal MargemLucro)
        {

            ClasseBanco conn = new ClasseBanco();

            /*
             * procedure SP_BloqueioPedidos 
             * parametros :             
                Cliente				AS INT,
                Pedido				AS INT,
                FormaPagto			AS INT,
                PrazoPagto			AS INT,
                Empresa				AS INT,
                ValorPedido			AS MONEY,
                DataInadimplente	AS CHAR,
                BloquearDebitos		AS INT,
                MargemMinimaPedido	AS MONEY,
                MargemLucro			AS MONEY
            */


            SqlCommand cmd = new SqlCommand("SP_BloqueioPedidos", conn.AbrirBanco());
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@iCliente", Cliente);
            cmd.Parameters["@iCliente"].Value = Cliente;

            cmd.Parameters.AddWithValue("@iPedido", Pedido);
            cmd.Parameters["@iPedido"].Value = Pedido;

            cmd.Parameters.AddWithValue("@iFormaPagto", FormaPagto);
            cmd.Parameters["@iFormaPagto"].Value = FormaPagto;

            cmd.Parameters.AddWithValue("@iPrazoPagto", PrazoPagto);
            cmd.Parameters["@iPrazoPagto"].Value = PrazoPagto;

            cmd.Parameters.AddWithValue("@iEmpresa", Empresa);
            cmd.Parameters["@iEmpresa"].Value = Empresa;

            cmd.Parameters.AddWithValue("@cValorPedido", ValorPedido);
            //SQL SERVER 2008
            //cmd.Parameters["@cValorPedido"].Value = (ValorPedido); //retirado Convert.ToDecimal() - 45834

            //SQL SERVER 2000
            cmd.Parameters["@cValorPedido"].Value = Convert.ToDecimal(ValorPedido); //retirado Convert.ToDecimal() - 45834

            cmd.Parameters.AddWithValue("@dDataInadimplente", DataInadimplente);
            cmd.Parameters["@dDataInadimplente"].Value = DataInadimplente;

            cmd.Parameters.AddWithValue("@bBloquearDebitos", BloquearDebitos);
            cmd.Parameters["@bBloquearDebitos"].Value = BloquearDebitos;

            cmd.Parameters.AddWithValue("@cMargemMinimaPedido", MargemMinimaPedido);
            cmd.Parameters["@cMargemMinimaPedido"].Value = MargemMinimaPedido;

            cmd.Parameters.AddWithValue("@cMargemLucro", MargemLucro);
            cmd.Parameters["@cMargemLucro"].Value = MargemLucro;


            SqlParameter retornoSP = new SqlParameter("retorno", SqlDbType.Int);
            retornoSP.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(retornoSP);

            cmd.ExecuteNonQuery();

            var r = Convert.ToInt32(retornoSP.Value);


            switch (r)
            {

                case 1:

                    return "Pedido Bloqueado por Bloqueio do Cliente no Financeiro. O Pedido será gravado, mas não será Faturado.";


                case 2:

                    return "Pedido Bloqueado por Inadimplencia. O Pedido será gravado, mas não será Faturado.";


                case 3:
                    return "Cliente com Limite de Crédito Excedido. O Pedido será gravado, mas não será Faturado.";


                case 4:
                    return "Cliente Bloqueado por Débitos. O Pedido será gravado, mas não será Faturado.";


                case 5:
                    return "Valor mínimo para a Condição de Pagamento não atingido. O Pedido será gravado, mas não será Faturado.";


                case 6:
                    return "Pedido Bloqueado por Margem Mínima não atingida. O Pedido será gravado, mas não será Faturado.";


                case 7:
                    return "OK";


                default:

                    return "OK";

            }


        }
    }
}