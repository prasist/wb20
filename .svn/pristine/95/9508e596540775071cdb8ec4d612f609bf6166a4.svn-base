using System;
using System.Collections.Generic;
using System.Linq;

namespace WebPedidos.WSClasses
{
    public class ClasseLogin
    {
        public static UsuarioResumido Login(int empresa, int usuario, String senha)
        {
            string spwd = new Krypto().EncryptString(senha, AcaoKrypto.cnDECRYPT);

            DataClassesDataContext dcdc = new DataClassesDataContext();
            List<UsuarioResumido> user =    (
                                                from u in dcdc.USUARIOs
                                                join v in dcdc.VENDEDORs on u.CodUsu equals v.CodUsu
                                                where u.CodEmp.Equals(empresa)
                                                where u.CodUsu.Equals(usuario)
                                                where u.Senha.Equals(new Krypto().EncryptString(senha, AcaoKrypto.cnENCRYPT))
                                                where u.ATIVO.Equals('A')
                                                orderby u.NomUsu
                                                select new UsuarioResumido  (
                                                                                u.CodEmp, 
                                                                                u.CodUsu, 
                                                                                u.NomUsu, 
                                                                                u.IDUSUARIOS, 
                                                                                u.ATIVO == 'S' ? true : false, 
                                                                                u.Senha, 
                                                                                v.CodVend, 
                                                                                v.NomVend
                                                                            )
                                            ).ToList();
            if (user.Count == 1)                
            {
                return user[0];
            }
            return null;
        }
    }
}
