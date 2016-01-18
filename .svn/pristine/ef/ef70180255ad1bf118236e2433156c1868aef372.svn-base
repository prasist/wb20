using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace WebPedidos.WSClasses
{
    public class ClasseParametro
    {
        public static List<ParametroResumido> GetParametro(int CodEmp)
        {
            DataClassesDataContext dcdc = new DataClassesDataContext();
            List<ParametroResumido> parametros = new List<ParametroResumido>();
            dcdc.PARAMETROs.Where(e => e.CodEmp == CodEmp).ToList().ForEach(e => parametros.Add(new ParametroResumido(
                e.CodEmp,
                e.TipMovPedPALM      == null ? 0 : e.TipMovPedPALM,
                e.TipMovPedPALMFE    == null ? 0 : e.TipMovPedPALMFE,
                e.CodTipPrc          == null ? 0 : e.CodTipPrc, 
                e.CodTipPrz          == null ? 0 : e.CodTipPrz, 
                e.NumPedIni          == null ? 0 : e.NumPedIni, 
                e.PARA_CasasDecimais == null ? '2' : e.PARA_CasasDecimais, 
                e.Prioridade_Cliente == null ? 0 : e.Prioridade_Cliente, 
                e.Prioridade_Merc    == null ? 0 : e.Prioridade_Merc, 
                e.Prioridade_Repr    == null ? 0 : e.Prioridade_Repr, 
                e.Prioridade_Tab     == null ? 0 : e.Prioridade_Tab,
                e.PARA_PriorGrupo == null ? 0 : e.PARA_PriorGrupo, e.PERCBLOQUEIO, e.SaldoPed, (e.PARA_UnidadeVenda == null ? 0 : e.PARA_UnidadeVenda), e.CondicaoTabLivreWeb, e.ExibirRazaoSocial, e.HostFtp, e.FtpUsuario, e.FtpSenha, e.PastaServidor, e.LayoutCombo)));
            dcdc.Dispose();
            return parametros;            
        }
    }
}
