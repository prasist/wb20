using System;
using System.Collections.Generic;
using System.Linq;

namespace WebPedidos.WSClasses
{
    public class ClasseFormaPagto
    {
        public static List<FormaPagtoResumido> ListarFormasPagto(UsuarioResumido Usuario)
        {
            DataClassesDataContext dcdc = new DataClassesDataContext();

            /* SELECT * FROM ITCONPAGTO, FORMAPAGTO, TIPOPRAZO  
             * WHERE 
             *      ITCONPAGTO.CodFrmPgt = FORMAPAGTO.CodFrmPgt  AND 
             *      ITCONPAGTO.CodTipPrz = TIPOPRAZO.CodTipPrz  and 
             *      codemp = 1 AND 
             *      FILTROVENDA = 1 
             *      ORDER BY FORMAPAGTO.DesFrmPgt, FORMAPAGTO.CodFrmPgt
             * 
             */

            var dados = (from it in dcdc.ITCONPAGTOs 
                         join fp in dcdc.FORMAPAGTOs on it.CodFrmPgt equals fp.CodFrmPgt
                         join tp in dcdc.TIPOPRAZOs on it.CodTipPrz equals tp.CodTipPrz
                         where it.CodEmp.Equals(Usuario.CodEmp)
                         where fp.FiltroVenda.Equals(1)
                         orderby fp.DesFrmPgt
                         orderby fp.CodFrmPgt

                         select new FormaPagtoResumido  (
                                                            it.CodEmp, 
                                                            fp.CodFrmPgt, 
                                                            tp.CodTipPrz, 
                                                            fp.DesFrmPgt, 
                                                            tp.DesTipPrz,
                                                            String.Empty,
                                                            String.Empty,
                                                            fp.GeraParcelas.ToString()
                                                        )).ToList();




            //dcdc.Dispose();
            String vrGeraParcelas = "";
            String vrFormaPagto ="";

            foreach (FormaPagtoResumido item in dados)
            {
                if (vrGeraParcelas == "S" && item.CodFrmPgt.ToString() == vrFormaPagto)
                {
                    vrGeraParcelas = item.GeraParcelas.ToString();                    
                } 
                else 
                {
                    item.ValorCombo = (item.CodFrmPgt.ToString() + "|" + item.CodTipPrz.ToString().Trim());
                    item.LinhaCombo = (item.CodFrmPgt.ToString("000") + " - " + item.DesFrmPgt.Trim());

                    vrFormaPagto = item.CodFrmPgt.ToString();
                    vrGeraParcelas = item.GeraParcelas.ToString(); 
                }
            }

            dcdc.Dispose();
            return dados;
        }
        public static FORMAPAGTO FormaPagto(int CodFrmPgto)
        {
            return new DataClassesDataContext().FORMAPAGTOs.SingleOrDefault(cfp => cfp.CodFrmPgt == CodFrmPgto);
        }
    }
}