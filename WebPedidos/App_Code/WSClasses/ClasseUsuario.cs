using System;
using System.Collections.Generic;
using System.Linq;

namespace WebPedidos.WSClasses
{
    public class ClasseUsuario
    {
        public static List<UsuarioResumido> Usuarios()
        {
            DataClassesDataContext dcdc = new DataClassesDataContext();
            List<UsuarioResumido> usuarios = new List<UsuarioResumido>();
            dcdc.USUARIOs.ToList().ForEach(u => usuarios.Add(new UsuarioResumido(u.CodEmp, u.CodUsu, u.NomUsu, u.IDUSUARIOS, (char)u.ATIVO == 'S' ? true : false, u.Senha)));
            return usuarios;
        }
        public static List<UsuarioResumido> Usuarios(int codEmp)
        {
            DataClassesDataContext dcdc = new DataClassesDataContext();
            List<UsuarioResumido> usuarios = new List<UsuarioResumido>();
            dcdc.USUARIOs.Where(u => u.CodEmp == codEmp).ToList().ForEach(u => usuarios.Add(new UsuarioResumido(u.CodEmp, u.CodUsu, u.NomUsu, u.IDUSUARIOS, (char)u.ATIVO == 'S' ? true : false, u.Senha)));
            return usuarios;
        }
        public static List<UsuarioResumido> Usuario(int codEmp, int codUsu)
        {
            DataClassesDataContext dcdc = new DataClassesDataContext();
            List<UsuarioResumido> usuarios = new List<UsuarioResumido>();
            
            dcdc.USUARIOs.Where(u => u.CodEmp == codEmp && u.CodUsu == codUsu).ToList().ForEach(u => usuarios.Add(new UsuarioResumido(u.CodEmp, u.CodUsu, u.NomUsu, u.IDUSUARIOS, String.IsNullOrEmpty(u.ATIVO.ToString()) || u.ATIVO.ToString()=="N" ? false : true, u.Senha)));
            return usuarios;
        }
    }
}
