
namespace WebPedidos.WSClasses
{
    public class ParametroResumido
    {
        decimal?    _PercBloqueio;
        short       _CodEmp;
        int?        _CodTipMov_Estadual;
        int?        _CodTipMov_InterEstadual;
        int?      _CodTipPrc;
        int?      _CodTipPrz;
        int?        _NumPedIni;
        char?       _PARA_CasasDecimais;
        int?        _Prioridade_Cliente;
        short?      _Prioridade_Merc;
        short?      _Prioridade_Repr;
        short?      _Prioridade_Tab;
        short?      _PARA_PriorGrupo;
        short?      _para_unidadevenda;
        char?       _SaldoPed;
        int?        _CondicaoTabLivreWeb;
        int?        _ExibirRazaoSocial;
        string _HostFtp;
        string _PastaServidor;
        string _FtpUsuario;
        string _FtpSenha;
        short? _LayoutCombo;
        
        public short? LayoutCombo
        {
            get { return _LayoutCombo; }
            set { _LayoutCombo = value; }
        }
                
        public string HostFtp
        {
            get { return _HostFtp; }
            set { _HostFtp=value;}
        }

        public string PastaServidor
        {
            get { return _PastaServidor; }
            set { _PastaServidor = value; }
        }
              
        public string FtpUsuario
        {
            get { return _FtpUsuario; }
            set { _FtpUsuario = value; }
        }

        public string FtpSenha
        {
            get { return _FtpSenha; }
            set { _FtpSenha = value; }
        }

        //51502
        public int? ExibirRazaoSocial
        {
            get { return _ExibirRazaoSocial; }
            set { _ExibirRazaoSocial = value; }
        }

        public int? CondicaoTabLivreWeb
        {
            get { return _CondicaoTabLivreWeb; }
            set { _CondicaoTabLivreWeb = value; }
        }
        //51502

        public char? SaldoPed
        {
            get { return _SaldoPed; }
            set { _SaldoPed = value; }
        }

        public short? para_unidadevenda
        {
            get { return _para_unidadevenda; }
            set { _para_unidadevenda = value; }
        }

        public short CodEmp
        {
            get { return _CodEmp; }
            set { _CodEmp = value; }
        }
        public int? CodTipMov_Estadual
        {
            get { return _CodTipMov_Estadual; }
            set { _CodTipMov_Estadual = value; }
        }

        public int? CodTipMov_InterEstadual
        {
            get { return _CodTipMov_InterEstadual; }
            set { _CodTipMov_InterEstadual = value; }
        }

        public int? CodTipPrc
        {
            get { return _CodTipPrc; }
            set { _CodTipPrc = value; }
        }
        public int? CodTipPrz
        {
            get { return _CodTipPrz; }
            set { _CodTipPrz = value; }
        }
        public int? NumPedIni
        {
            get { return _NumPedIni; }
            set { _NumPedIni = value; }
        }
        public char? PARA_CasasDecimais
        {
            get { return _PARA_CasasDecimais; }
            set { _PARA_CasasDecimais = value; }
        }
        public int? Prioridade_Cliente
        {
            get { return _Prioridade_Cliente; }
            set { _Prioridade_Cliente = value; }
        }
        public short? Prioridade_Merc
        {
            get { return _Prioridade_Merc; }
            set { _Prioridade_Merc = value; }
        }
        public short? Prioridade_Repr
        {
            get { return _Prioridade_Repr; }
            set { _Prioridade_Repr = value; }
        }
        public short? Prioridade_Tab
        {
            get { return _Prioridade_Tab; }
            set { _Prioridade_Tab = value; }
        }
        public short? PARA_PriorGrupo
        {
            get { return _PARA_PriorGrupo; }
            set { _PARA_PriorGrupo = value; }
        }

        public decimal? PercBloqueio
        {
            get { return _PercBloqueio; }
            set { _PercBloqueio = value; }
        }

        public ParametroResumido() { }
        public ParametroResumido(short CodEmp, int? CodTipMovEstadual, int? CodTipMovInterEstadual, short? CodTipPrc, short? CodTipPrz, int? NumPedIni, char? PARA_CasasDecimais, int? Prioridade_Cliente, short? Prioridade_Merc, short? Prioridade_Repr, short? Prioridade_Tab, short? PARA_PriorGrupo, decimal? PercBloqueio, char? SaldoPed, short? para_unidadevenda, int? CondicaoTabLivreWeb, int? ExibirRazaoSocial, string HostFtp, string FtpUsuario, string FtpSenha, string PastaServidor, short? LayoutCombo)
        {
            this._CodEmp = CodEmp;
            this._CodTipMov_Estadual = CodTipMovEstadual;
            this._CodTipMov_InterEstadual = CodTipMovInterEstadual;            
            this._CodTipPrc = CodTipPrc;
            this._CodTipPrz = CodTipPrz;
            this._NumPedIni = NumPedIni;
            this._PARA_CasasDecimais = PARA_CasasDecimais;
            this._Prioridade_Cliente = Prioridade_Cliente;
            this._Prioridade_Merc = Prioridade_Merc;
            this._Prioridade_Repr = Prioridade_Repr;
            this._Prioridade_Tab = Prioridade_Tab;
            this._PARA_PriorGrupo = PARA_PriorGrupo;
            this._para_unidadevenda = para_unidadevenda;
            this._SaldoPed = SaldoPed;
            this._PercBloqueio = PercBloqueio;
            this._CondicaoTabLivreWeb = CondicaoTabLivreWeb;//51502
            this._ExibirRazaoSocial = ExibirRazaoSocial;//51502
            this._HostFtp = HostFtp;
            this._FtpSenha = FtpSenha;
            this._FtpUsuario = FtpUsuario;
            this._PastaServidor = PastaServidor;
            this._LayoutCombo = LayoutCombo;
        }
    }
}
