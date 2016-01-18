using System;
using WebPedidos.WSClasses;



public partial class Cliente : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected bool ValidarExistenciaCnpj(string cgc_cpf)
    {
        ClasseBanco cs = new ClasseBanco();

        cs.AbrirBanco();

        var temp = Funcoes.RetiraCaracteres(cgc_cpf);

        var r = cs.Query("SELECT 1 FROM CLIENTE WHERE CGC_CPF LIKE '%" + temp + "%'");

        if (r.Read())
        {
            return false;
        }
        r.Close();
        return true;

    }

    protected bool ValidarCampos()
    {

        if (this.tbCNPF.Visible)
        {
            if (Funcoes.validaCnpj(this.tbCNPF.Text) == false)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "respostaScript", "<script language = 'javascript'>alert('CNPJ Inválido.')</script>");
                return false;
            }
        }
        else
        {
            if (Funcoes.validaCPF(this.tbCPF.Text) == false)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "respostaScript", "<script language = 'javascript'>alert('CPF Inválido.')</script>");
                return false;
            }
        }


        return true;

    }

    protected void ButtonGravar_Click(object sender, EventArgs e)
    {
        //instancia a classe cliente
        ClasseCliente c = new ClasseCliente();
        
        string cgc_cpf = "";

        if (this.tbCNPF.Visible)
        {
            cgc_cpf  = this.tbCNPF.Text;
        }
        else
        {
            cgc_cpf = this.tbCPF.Text;
        }

        if (ValidarExistenciaCnpj(cgc_cpf) == false)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "respostaScript", "<script language = 'javascript'>alert('CNPJ / CPF Já cadastrado.')</script>");
            return;
        }

        if (ValidarCampos() == false)
        {
            return;
        }

        //Envia valor as propriedades

        c.Obs1 = tbObs.Text.ToUpper() + "\r\n" + tbObs2.Text.ToUpper() + "\r\n" + tbObs3.Text.ToUpper(); 
        c.Ativo = "S";
        c.bCliente = "S";
        c.Fornecedor = "S";
        c.Prospect = "1";
        c.RazSoc = tbRazaoSocial.Text.ToUpper();
        c.NomFan = tbNomeFantasia.Text.ToUpper();
        c.CGC_CPF = cgc_cpf;
        c.RgCli = tbRG.Text;
        c.EndCli = tbEndereco.Text.ToUpper();
        c.Numero = tbNumero.Text;
        c.Bairro = tbBairro.Text.ToUpper();
        c.CidCli = tbCidade.Text.ToUpper();
        c.Complemento = tbComplemento.Text.ToUpper();
        c.Cep = tbCep.Text;
        c.Celular = tbCelular.Text;
        c.Contato = tbContato.Text.ToUpper();
        c.DatCad = DateTime.Today.ToString();
        c.Email = tbEmail.Text.ToUpper();
        c.EstCli = tbEstado.Text.ToUpper();
        c.FaxCli = tbFax.Text;
        c.RamCli = tbRamal.Text;
        c.TelCli = tbTelefone.Text;
        c.TipoCli = opFisica.Checked == true ? "F" : "J"; //Pessao fisica ou juridica
        c.RefBancaria = tbRefBanc.Text.ToUpper();
        c.RefBancaria2 = tbRefBanc2.Text.ToUpper();
        c.RefBancaria3 = tbRefBanc3.Text.ToUpper();
        
        c.RefComercial = tbRefCom.Text.ToUpper();
        c.RefComercial2 = tbRefCom2.Text.ToUpper();
        c.RefComercial3 = tbRefCom3.Text.ToUpper();

        c.RefContador = tbRefContador.Text.ToUpper();
        c.RefContador2 = tbRefContador2.Text.ToUpper();
        c.RefContador3 = tbRefContador3.Text.ToUpper();
        
        //Insere cliente

        if (c.IncluirCliente())
        {
            if (!ClientScript.IsClientScriptBlockRegistered("respostaScript"))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "respostaScript", "<script language = 'javascript'>alert('Cliente cadastrado com sucesso.')</script>");
            }

            ButtonLimpar_Click(null, null);
        }
        else
        {
            if (!ClientScript.IsClientScriptBlockRegistered("respostaScript"))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "respostaScript", "<script language = 'javascript'>alert('Erro ao cadastrar o Cliente.')</script>");
            }

        }
        
        c.Dispose();

                
    }

    protected void ScriptManager1_Init(object sender, EventArgs e)
    {

    }

    protected void ButtonLimpar_Click(object sender, EventArgs e)
    {

        //Envia valor as propriedades
        tbRazaoSocial.Text = String.Empty;
        tbNomeFantasia.Text = String.Empty;
        tbCNPF.Text = String.Empty;
        tbCPF.Text = String.Empty;
        tbRG.Text = String.Empty;
        tbEndereco.Text = String.Empty;
        tbNumero.Text = String.Empty;
        tbBairro.Text = String.Empty;
        tbCidade.Text = String.Empty;
        tbComplemento.Text = String.Empty;
        tbCep.Text = String.Empty;
        tbCelular.Text = String.Empty;
        tbContato.Text = String.Empty;
        tbEmail.Text = String.Empty;
        tbEstado.Text = String.Empty;
        tbFax.Text = String.Empty;
        tbRamal.Text = String.Empty;
        tbTelefone.Text = String.Empty;
        tbRefBanc.Text = String.Empty;
        tbRefCom.Text = String.Empty;
        tbRefContador.Text= String.Empty;
        opFisica.Checked = false;
        opJuridica.Checked = false;
        tbObs.Text = String.Empty;
        tbObs2.Text = String.Empty;
        tbObs3.Text = String.Empty;
    }
    
    protected void opFisica_CheckedChanged(object sender, EventArgs e)
    {
        lbTipo.Text = "CPF";
        lbRG.Text = "RG";
        tbCNPF.Visible = false;
        tbCPF.Visible = true;
        tbTipoCliente.Text = "CPF";
        lbNome.Text = "Nome";

    }
    protected void opJuridica_CheckedChanged(object sender, EventArgs e)
    {
        lbTipo.Text = "CNPJ";
        lbRG.Text = "Insc. Estadual";
        tbCNPF.Visible = true;
        tbCPF.Visible = false;
        tbTipoCliente.Text = "CNPJ";
        lbNome.Text = "Razão Social";
    }
}