using System;

namespace WebPedidos.WSClasses
{
    [Serializable()]
    public class ProdutoResumido
    {
        Int32   codigo;
        string  nome;
        decimal preco;
        decimal quantidade;
        decimal _preco_liquido;
        decimal totalParcial;
        decimal saldo;
        decimal comissao;
        decimal _desconto;      //FABIANO - 07/10/2011
        decimal _percDesconto;  //FABIANO - 11/10/2011
        decimal _QtdEmb;
        decimal _ComissaoTel;
        string  _unidade_venda;
        decimal _peso;
        string  _MensagemPromocao;
        int     _Itp_CodTabPrz;
        decimal _QtdTributavel;
        decimal _VlrTributavel;
        decimal _PrecoReal;
        string _caminhoimagem;


        public decimal QtdTributavel
        {
            get { return _QtdTributavel; }
            set { _QtdTributavel = value; }
        }

        public decimal VlrTributavel
        {
            get { return _VlrTributavel; }
            set { _VlrTributavel = value; }
        }

        public int Itp_CodTabPrz
        {
            get { return _Itp_CodTabPrz; }
            set { _Itp_CodTabPrz = value; }
        }

        public string caminhoimagem
        {
            get { return _caminhoimagem; }
            set { _caminhoimagem = value; }
        }

        public string MensagemPromocao
        {
            get { return _MensagemPromocao; }
            set { _MensagemPromocao = value; }
        }

        //52557
        public decimal PrecoReal
        {
            get { return _PrecoReal; }
            set { _PrecoReal = value; }
        }
        //52557

        public decimal Peso
        {
            get { return _peso; }
            set { _peso = value; }
        }

        public decimal preco_liquido
        {
            get { return _preco_liquido; }
            set { _preco_liquido = value; }
        }

        public decimal percentDesconto
        {
            get { return _percDesconto; }
            set { _percDesconto = value; }
        }
        
        public decimal Comissao
        {
            get { return comissao; }
            set { comissao = value; }
        }
        public decimal Saldo
        {
            get { return saldo; }
            set { saldo = value; }
        }
        public decimal Quantidade
        {
            get { return quantidade; }
            set { quantidade = value; }
        }
        public decimal TotalParcial
        {
            get { return totalParcial; }
            set { totalParcial = value; }
        }
        public decimal Preco
        {
            get { return preco; }
            set { preco = value; }
        }
        public Int32 Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }
        public string Nome
        {
            get { return nome; }
            set { nome = value; }
        }

        public string UnidadeVenda
        {
            get { return _unidade_venda; }
            set { _unidade_venda = value; }
        }

        //FABIANO - 07/10/2011
        public decimal Desconto
        {
            get { return _desconto; }
            set { _desconto = value; }
        }

        public decimal QtdEmb
        {
            get { return _QtdEmb; }
            set { _QtdEmb = value; }
        }


        public decimal ComissaoTel
        {
            get { return _ComissaoTel; }
            set { _ComissaoTel = value; }
        }

        string _Unidade;
        string _M_UNIDADE;
        decimal _QtdCaixa;


        public decimal QtdCaixa
        {
            get { return _QtdCaixa; }
            set { _QtdCaixa = value; }
        }

        public string M_UNIDADE
        {
            get { return _M_UNIDADE; }
            set { _M_UNIDADE = value; }
        }

        public string Unidade
        {
            get { return _Unidade; }
            set { _Unidade =value; }
        }

        public ProdutoResumido() { }

        //public ProdutoResumido(Int32 codigo, string nome, decimal preco, decimal quantidade, decimal totalParcial, decimal saldo, decimal comissao, decimal _desconto, decimal _percDesconto, decimal _ComissaoTel, decimal _QtdCaixa, string _M_Unidade, string _Unidade, string _unidade_venda, decimal _peso, string _MensagemPromocao, int _Itp_CodTabPrz, decimal QtdTributavel, decimal VlrTributavel, decimal PrecoReal)
        //{
        //    Codigo = codigo;
        //    Nome = nome;
        //    Preco = preco;
        //    Quantidade = quantidade;
        //    TotalParcial = totalParcial;
        //    Saldo = saldo;
        //    Comissao = comissao;
        //    Desconto = _desconto;
        //    percentDesconto = _percDesconto;
        //    ComissaoTel = _ComissaoTel; //comissao televendedor
        //    preco_liquido = _preco_liquido;
        //    QtdCaixa = _QtdCaixa;
        //    M_UNIDADE = _M_Unidade;
        //    Unidade = _Unidade;
        //    UnidadeVenda = _unidade_venda;
        //    Peso = _peso;
        //    MensagemPromocao = _MensagemPromocao;
        //    Itp_CodTabPrz = _Itp_CodTabPrz;
        //    QtdTributavel = _QtdTributavel;
        //    VlrTributavel = _VlrTributavel;
        //}
        
        //52557
        public ProdutoResumido(Int32 codigo, string nome, decimal preco, decimal quantidade, decimal totalParcial, decimal saldo, decimal comissao, decimal _desconto, decimal _percDesconto, decimal _ComissaoTel, decimal _QtdCaixa, string _M_Unidade, string _Unidade, string _unidade_venda, decimal _peso, string _MensagemPromocao, int _Itp_CodTabPrz, decimal QtdTributavel, decimal VlrTributavel, decimal PrecoReal, string _caminhoimagem)
        {
            Codigo = codigo;
            Nome = nome;
            Preco = preco;
            Quantidade = quantidade;
            TotalParcial = totalParcial;
            Saldo = saldo;
            Comissao = comissao;
            Desconto = _desconto;
            percentDesconto = _percDesconto;
            ComissaoTel = _ComissaoTel; //comissao televendedor
            preco_liquido = _preco_liquido;
            QtdCaixa = _QtdCaixa;
            M_UNIDADE = _M_Unidade;
            Unidade = _Unidade;
            UnidadeVenda = _unidade_venda;
            Peso = _peso;
            MensagemPromocao = _MensagemPromocao;
            Itp_CodTabPrz = _Itp_CodTabPrz;
            QtdTributavel = _QtdTributavel;
            VlrTributavel = _VlrTributavel;
            PrecoReal = _PrecoReal;
            caminhoimagem= _caminhoimagem;
        }
    }
}
