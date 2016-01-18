using System;

namespace WebPedidos.WSClasses
{
    public class UsuarioResumido
    {
        int codEmp;
        public int CodEmp
        {
            get { return codEmp; }
            set { codEmp = value; }
        }

        int codUsu;
        public int CodUsu
        {
            get { return codUsu; }
            set { codUsu = value; }
        }

        String nomUsu;
        public String NomUsu
        {
            get { return nomUsu; }
            set { nomUsu = value; }
        }

        int idUsuarios;
        public int IdUsuarios
        {
            get { return idUsuarios; }
            set { idUsuarios = value; }
        }

        bool ativo;
        public bool Ativo
        {
            get { return ativo; }
            set { ativo = value; }
        }

        String senha;
        public String Senha
        {
            get { return senha; }
            set { senha = value; }
        }

        Int32 codvend;
        public Int32 CodVend
        {
            get { return codvend; }
            set { codvend = value; }
        }

        String nomvend;
        public String NomVend
        {
            get { return nomvend; }
            set { nomvend = value; }
        }

        public UsuarioResumido() { }
        public UsuarioResumido(int codEmp, int codUsu, String nomUsu, int idUsuarios, bool ativo, String senha)
        {
            CodEmp = codEmp;
            CodUsu = codUsu;
            NomUsu = nomUsu;
            IdUsuarios = idUsuarios;
            Ativo = ativo;
            Senha = senha;
        }

        public UsuarioResumido(int codEmp, int codUsu, String nomUsu, int idUsuarios, bool ativo, String senha, Int32 CodVend, String NomVend)
        {
            CodEmp = codEmp;
            CodUsu = codUsu;
            NomUsu = nomUsu;
            IdUsuarios = idUsuarios;
            Ativo = ativo;
            Senha = senha;
            codvend = CodVend;
            nomvend = NomVend;
        }

    }
}
