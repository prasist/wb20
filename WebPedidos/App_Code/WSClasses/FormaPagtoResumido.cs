using System;

namespace WebPedidos.WSClasses
{
    public class FormaPagtoResumido
    {
        short _CodEmp;
        short _CodFrmPgt;
        short _CodTipPrz;
        String _DesFrmPgt;
        String _DesTipPrz;

        String _ValorCombo;
        String _LinhaCombo;
        String _GeraParcelas;

        public short CodEmp
        {
            get { return _CodEmp; }
            set { _CodEmp = value; }
        }
        public short CodFrmPgt
        {
            get { return _CodFrmPgt; }
            set { _CodFrmPgt = value; }
        }
        public short CodTipPrz
        {
            get { return _CodTipPrz; }
            set { _CodTipPrz = value; }
        }
        public String DesFrmPgt
        {
            get { return _DesFrmPgt; }
            set { _DesFrmPgt = value; }
        }
        public String DesTipPrz
        {
            get { return _DesTipPrz; }
            set { _DesTipPrz = value; }
        }

        public String ValorCombo
        {
            get { return _ValorCombo; }
            set { _ValorCombo = value; }
        }
        public String LinhaCombo
        {
            get { return _LinhaCombo; }
            set { _LinhaCombo = value; }
        }

        public String GeraParcelas
        {
            get { return _GeraParcelas; }
            set { _GeraParcelas = value; }
        }

        public FormaPagtoResumido() { }
        public FormaPagtoResumido(short CodEmp, short CodFrmPgt, short CodTipPrz, String DesFrmPgt, String DesTipPrz)
        {
            _CodEmp = CodEmp;
            _CodFrmPgt = CodFrmPgt;
            _CodTipPrz = CodTipPrz;
            _DesFrmPgt = DesFrmPgt;
            _DesTipPrz = DesTipPrz;
        }
        public FormaPagtoResumido(short CodEmp, short CodFrmPgt, short CodTipPrz, String DesFrmPgt, String DesTipPrz, String ValorCombo, String LinhaCombo, String GeraParcelas)
        {
            _CodEmp = CodEmp;
            _CodFrmPgt = CodFrmPgt;
            _CodTipPrz = CodTipPrz;
            _DesFrmPgt = DesFrmPgt;
            _DesTipPrz = DesTipPrz;
            _ValorCombo = ValorCombo;
            _LinhaCombo = LinhaCombo;
            _GeraParcelas = GeraParcelas;
        }
    }
}
