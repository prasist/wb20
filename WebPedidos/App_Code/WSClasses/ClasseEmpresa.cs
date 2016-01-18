using System.Collections.Generic;
using System.Linq;

namespace WebPedidos.WSClasses
{
    public class ClasseEmpresa
    {
        public static List<EmpresaResumido> ListarEmpresas(UsuarioResumido u)
        {
            DataClassesDataContext dcdc = new DataClassesDataContext();
            List<EmpresaResumido> empresas = new List<EmpresaResumido>();
            //dcdc.EMPRESAs.OrderBy(e => e.CodEmp).ToList().ForEach(e => empresas.Add(new EmpresaResumido(e.CodEmp, e.Nome, e.NomeFan, e.CGC, e.InscEst, e.Endereco, e.Cidade, e.Estado, e.Bairro, e.Cep, e.Fone, e.Fax, e.Ramal, e.Numero, e.contato_ant3, e.Email, e.SITE, e.CONTATO, e.CONTATOVENDA, e.EMAILVENDA, (int)e.CODIGOPRASIST)));
            dcdc.EMPRESAs.OrderBy(e => e.CodEmp).ToList().ForEach(e => empresas.Add(new EmpresaResumido(e.CodEmp, e.Nome, e.NomeFan, e.CGC, e.InscEst, e.Endereco, e.Cidade, e.Estado, e.Bairro, e.Cep, e.Fone, e.Fax, e.Ramal, e.Numero, e.contato_ant3, e.Email, e.SITE, e.CONTATO, e.CONTATOVENDA, e.EMAILVENDA, e.CODIGOPRASIST == null ? 0 : (int)e.CODIGOPRASIST)));
            return empresas;
        }
        public static List<EmpresaResumido> ListarEmpresas()
        {
            DataClassesDataContext dcdc = new DataClassesDataContext();
            List<EmpresaResumido> empresas = new List<EmpresaResumido>();
            //dcdc.EMPRESAs.OrderBy(e => e.CodEmp).ToList().ForEach(e => empresas.Add(new EmpresaResumido(e.CodEmp, e.Nome, e.NomeFan, e.CGC, e.InscEst, e.Endereco, e.Cidade, e.Estado, e.Bairro, e.Cep, e.Fone, e.Fax, e.Ramal, e.Numero, e.contato_ant3, e.Email, e.SITE, e.CONTATO, e.CONTATOVENDA, e.EMAILVENDA, (int)e.CODIGOPRASIST)));
            dcdc.EMPRESAs.OrderBy(e => e.CodEmp).ToList().ForEach(e => empresas.Add(new EmpresaResumido(e.CodEmp, e.Nome, e.NomeFan, e.CGC, e.InscEst, e.Endereco, e.Cidade, e.Estado, e.Bairro, e.Cep, e.Fone, e.Fax, e.Ramal, e.Numero, e.contato_ant3, e.Email, e.SITE, e.CONTATO, e.CONTATOVENDA, e.EMAILVENDA, e.CODIGOPRASIST == null ? 0 : (int)e.CODIGOPRASIST)));
            return empresas;
        }
        public static List<EmpresaResumido> GetEmpresa(UsuarioResumido u, int CodEmp)
        {
            DataClassesDataContext dcdc = new DataClassesDataContext();
            List<EmpresaResumido> empresas = new List<EmpresaResumido>();
            dcdc.EMPRESAs.Where(e => e.CodEmp == CodEmp).ToList().ForEach(e => empresas.Add(new EmpresaResumido(e.CodEmp, e.Nome, e.NomeFan, e.CGC, e.InscEst, e.Endereco, e.Cidade, e.Estado, e.Bairro, e.Cep, e.Fone, e.Fax, e.Ramal, e.Numero, e.contato_ant3, e.Email, e.SITE, e.CONTATO, e.CONTATOVENDA, e.EMAILVENDA, e.CODIGOPRASIST == null ? 0 : (int)e.CODIGOPRASIST)));
            return empresas;
        }
    }
}
