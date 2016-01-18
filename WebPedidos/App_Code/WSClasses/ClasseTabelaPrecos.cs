using System;
using System.Collections.Generic;
using System.Text;

namespace WebPedidos.WSClasses
{

    public class ClasseTabelaPrecos
    {

        #region Definicoes 
        
        int     _CodTipPrz;
        int     _CodTipPrc;
        string  _DesTipPrc;
        string  _DesTipPrz;

        public string DesTipPrc
        {
            get { return _DesTipPrc; }
            set { _DesTipPrc = value; }
        }

        public string DesTipPrz
        {
            get { return _DesTipPrz; }
            set { _DesTipPrz = value; }
        }

        public int CodTipPrc
        {
            get { return _CodTipPrc; }
            set { _CodTipPrc = value; }
        }


        public int CodTipPrz
        {
            get { return _CodTipPrz; }
            set { _CodTipPrz = value; }
        }
        #endregion

        public ClasseTabelaPrecos(int CodTipPrc, string DesTipPrc, int CodTipPrz, string DesTipPrz)
        {
            this._CodTipPrc = CodTipPrc;
            this._CodTipPrz = CodTipPrz;            
            this._DesTipPrc = DesTipPrc;
            this._DesTipPrz = DesTipPrz;
        }

        public static List<ClasseTabelaPrecos> BuscaTabelaPreco(int? CodTipPrc, int? CodTipPrz)
        {
            ClasseBanco     csBanco = new ClasseBanco();            
            StringBuilder   sQuery  = new StringBuilder();
            List<ClasseTabelaPrecos> tabela = new List<ClasseTabelaPrecos>();

            sQuery.Length = 0;
                        
            sQuery.Append(" SELECT TB.CODTIPPRC, TB.CODTIPPRZ, TP.DESTIPPRC, TPZ.DESTIPPRZ ");
            sQuery.Append(" FROM PRCPRAZO TB ");
            sQuery.Append(" INNER JOIN TIPO_PRECO TP ON TP.CodTipPrc = TB.CODTIPPRC");
            sQuery.Append(" INNER JOIN TIPOPRAZO TPZ ON TPZ.CodTipPrz = TB.CodTipPrz ");
            sQuery.Append(" WHERE 1=1 ");

            if (!CodTipPrc.Equals(null))            
                sQuery.Append(" AND TB.CODTIPPRC = " + CodTipPrc + "");
            
            if (!CodTipPrz.Equals(null) && !CodTipPrz.Equals(0))            
                sQuery.Append(" AND TB.CodTipPrz = " + CodTipPrz + "");
            
            sQuery.Append(" ORDER BY TB.CODTIPPRC DESC, TB.CODTIPPRZ DESC ");

            var rsTemp = csBanco.Query(sQuery.ToString());

            while (rsTemp.Read())
            {
                tabela.Add(new ClasseTabelaPrecos(Convert.ToInt16(rsTemp["CODTIPPRC"]), rsTemp["DESTIPPRC"].ToString().Trim(), Convert.ToInt16(rsTemp["CODTIPPRZ"]), rsTemp["DESTIPPRZ"].ToString().Trim()));
            }

            return tabela;
        }
        
    }

}