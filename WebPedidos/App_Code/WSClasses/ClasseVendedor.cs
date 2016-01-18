using System;
using System.Collections.Generic;
using System.Text;

namespace WebPedidos.WSClasses
{
    /// <summary>
    /// Busca Tabelas de Preco do vendedor
    /// </summary>
    public class ClasseVendedor
    {

        /// <summary>
        /// Busca vendedor e retorna list com todas tabelas de preco do mesmo
        /// </summary>
        public static List<ClasseVendedorAtributos> BuscaDados(int CodVend, int? CodTipPrc, int? CodTipPrz)
        {
            ClasseBanco csBanco = new ClasseBanco();
            List<ClasseVendedorAtributos> vendedor = new List<ClasseVendedorAtributos>();
            StringBuilder sQuery = new StringBuilder();
            sQuery.Length = 0;

            sQuery.Append("SELECT TV.IDTABELA, TV.CODTIPPRZ, TP.DESTIPPRC, TPZ.DESTIPPRZ ");
            sQuery.Append("FROM TABVENDEDOR TV ");
            sQuery.Append("INNER JOIN TIPO_PRECO TP ON TP.CodTipPrc = TV.IDTABELA ");
            sQuery.Append("INNER JOIN TIPOPRAZO TPZ ON TPZ.CodTipPrz = TV.CodTipPrz ");
            sQuery.Append("WHERE IDTABVENDEDOR = " + CodVend + "");

            if (!CodTipPrc.Equals(null))
            {
                sQuery.Append(" AND TV.IDTABELA = " + CodTipPrc + "");
            }

            if (!CodTipPrz.Equals(null))
            {
                sQuery.Append(" AND TV.CodTipPrz = " + CodTipPrz + "");
            }

            sQuery.Append(" ORDER BY TV.IDTABELA DESC, TV.CODTIPPRZ DESC ");

            var rsTemp = csBanco.Query(sQuery.ToString());
            int iCont = 0;

            while (rsTemp.Read())
            {
                iCont++;
                vendedor.Add(new ClasseVendedorAtributos(CodVend, Convert.ToInt16(rsTemp["IDTABELA"]), rsTemp["DESTIPPRC"].ToString(), Convert.ToInt16(rsTemp["CODTIPPRZ"]), rsTemp["DESTIPPRZ"].ToString()));
            }
            rsTemp.Close();


            if (iCont.Equals(0))
            {
                sQuery.Length = 0;

                sQuery.Append("SELECT TV.CodTipPrc, TV.CODTIPPRZ, TP.DESTIPPRC, TPZ.DESTIPPRZ ");
                sQuery.Append("FROM PRCPRAZO TV ");
                sQuery.Append("INNER JOIN TIPO_PRECO TP ON TP.CodTipPrc = TV.CodTipPrc ");
                sQuery.Append("INNER JOIN TIPOPRAZO TPZ ON TPZ.CodTipPrz = TV.CodTipPrz ");
                
                if (!CodTipPrc.Equals(null))
                {
                    sQuery.Append(" AND TV.CodTipPrc = " + CodTipPrc + "");
                }

                if (!CodTipPrz.Equals(null))
                {
                    sQuery.Append(" AND TV.CodTipPrz = " + CodTipPrz + "");
                }

                sQuery.Append(" ORDER BY TV.CodTipPrc DESC, TV.CODTIPPRZ DESC ");
                rsTemp = csBanco.Query(sQuery.ToString());
                
                while (rsTemp.Read())
                {
                    vendedor.Add(new ClasseVendedorAtributos(CodVend, Convert.ToInt16(rsTemp["CodTipPrc"]), rsTemp["DESTIPPRC"].ToString(), Convert.ToInt16(rsTemp["CODTIPPRZ"]), rsTemp["DESTIPPRZ"].ToString()));
                }
            }
            return vendedor;
        }
        
    }

}