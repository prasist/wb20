using System;
using System.Web;

namespace WebPedidos.WSClasses
{
    public class Funcoes
    {
        
        public static String Decimais(ParametroResumido pr)
        {
            Int32 iCasasDecimais = 0;
            String sCasasDecimais = String.Empty;

            sCasasDecimais = pr.PARA_CasasDecimais.ToString();

            Int32.TryParse(sCasasDecimais, out iCasasDecimais);

            if (iCasasDecimais > 0)
            {
                sCasasDecimais = "0.";
                sCasasDecimais = sCasasDecimais.PadRight(iCasasDecimais + 2, '0');
            }
            else
            {
                sCasasDecimais = "0.00";
                //sCasasDecimais = sCasasDecimais.PadRight(3, '0');
            }

            return sCasasDecimais;
        }

        //Retorna formato yyyymmdd 
        public static string RetornaDataQuery(string sData)
        {
            DateTime sDataTemp;            
            sDataTemp = Convert.ToDateTime(sData);
            return sDataTemp.Year + String.Format("{0:00}", sDataTemp.Month) + String.Format("{0:00}", sDataTemp.Day);            
        }

        public static Boolean validaCPF(String vrCPF)

        {

            string valor = vrCPF.Replace(".", "");
            valor = valor.Replace("-", "");

            if (valor.Length != 11)

                return false;
 

            bool igual = true;

            for (int i = 1; i < 11 && igual; i++)

                if (valor[i] != valor[0])

                    igual = false;
 

            if (igual || valor == "12345678909")

                return false;
             

            int[] numeros = new int[11];
             

            for (int i = 0; i < 11; i++)

              numeros[i] = int.Parse(

                valor[i].ToString());
             

            int soma = 0;

            for (int i = 0; i < 9; i++)

                soma += (10 - i) * numeros[i];
             

            int resultado = soma % 11;
             

            if (resultado == 1 || resultado == 0)

            {
                if (numeros[9] != 0)

                    return false;

            }

            else if (numeros[9] != 11 - resultado)

                return false;
             

            soma = 0;

            for (int i = 0; i < 10; i++)

                soma += (11 - i) * numeros[i];
            

            resultado = soma % 11;
             

            if (resultado == 1 || resultado == 0)

            {
                if (numeros[10] != 0)

                    return false;
            }

            else

                if (numeros[10] != 11 - resultado)

                    return false;
            

            return true;

        }
    
        public static Boolean validaCnpj(String cnpj)
        {

            Int32[] digitos, soma, resultado;

            Int32 nrDig;

            String ftmt;

            Boolean[] cnpjOk;

            cnpj = cnpj.Replace("/", "");

            cnpj = cnpj.Replace(".", "");

            cnpj = cnpj.Replace("-", "");

            if (cnpj == "00000000000000")
            {

                return false;

            }

            ftmt = "6543298765432";

            digitos = new Int32[14];

            soma = new Int32[2];

            soma[0] = 0;

            soma[1] = 0;

            resultado = new Int32[2];

            resultado[0] = 0;

            resultado[1] = 0;

            cnpjOk = new Boolean[2];

            cnpjOk[0] = false;

            cnpjOk[1] = false;

            try
            {

                for (nrDig = 0; nrDig < 14; nrDig++)
                {

                    digitos[nrDig] = int.Parse(cnpj.Substring(nrDig, 1));

                    if (nrDig <= 11)

                        soma[0] += (digitos[nrDig] *

                        int.Parse(ftmt.Substring(nrDig + 1, 1)));

                    if (nrDig <= 12)

                        soma[1] += (digitos[nrDig] *

                        int.Parse(ftmt.Substring(nrDig, 1)));

                }

                for (nrDig = 0; nrDig < 2; nrDig++)
                {

                    resultado[nrDig] = (soma[nrDig] % 11);

                    if ((resultado[nrDig] == 0) || (resultado[nrDig] == 1))

                        cnpjOk[nrDig] = (digitos[12 + nrDig] == 0);

                    else

                        cnpjOk[nrDig] = (digitos[12 + nrDig] == (

                        11 - resultado[nrDig]));

                }

                return (cnpjOk[0] && cnpjOk[1]);

            }

            catch
            {

                return false;

            }

        }

        public static String RetiraCaracteres(String Texto)
        {
            string sRetorno = Texto;
            sRetorno = sRetorno.Replace(".", "");
            sRetorno = sRetorno.Replace("/", "");
            sRetorno = sRetorno.Replace("-", "");
            sRetorno = sRetorno.Replace("(", "");
            sRetorno = sRetorno.Replace(")", "");
            sRetorno = sRetorno.Replace("[", "");
            
            return sRetorno;

        }

        public static string BuscaCampoTabela(string sCampo, string sTabela, string sWhere)
        {

            ClasseBanco csBanco = new ClasseBanco();
            string sSql, sRetorno = "";

            sSql = " SELECT " + sCampo + " FROM " + sTabela + " (NOLOCK) WHERE 1=1 " + sWhere;

            var r = csBanco.Query(sSql);

            if (r.Read())
            {
                sRetorno = r[0].ToString();
            }
            r.Close();

            return sRetorno;

        }
        
        public static bool isMobileBrowser()
        {
            //GETS THE CURRENT USER CONTEXT
            HttpContext context = HttpContext.Current;
            //FIRST TRY BUILT IN ASP.NT CHECK
            if (context.Request.Browser.IsMobileDevice)
            {
                return true;
            }
            //THEN TRY CHECKING FOR THE HTTP_X_WAP_PROFILE HEADER
            if (context.Request.ServerVariables["HTTP_X_WAP_PROFILE"] != null)
            {
                return true;
            }
            //THEN TRY CHECKING THAT HTTP_ACCEPT EXISTS AND CONTAINS WAP
            if (context.Request.ServerVariables["HTTP_ACCEPT"] != null && 
                context.Request.ServerVariables["HTTP_ACCEPT"].ToLower().Contains("wap"))
            {
                return true;
            }
            //AND FINALLY CHECK THE HTTP_USER_AGENT 
            //HEADER VARIABLE FOR ANY ONE OF THE FOLLOWING
            if (context.Request.ServerVariables["HTTP_USER_AGENT"] != null)
            {
                //Create a list of all mobile types
                string[] mobiles =
                    new[]
                        {
                            "midp", "j2me", "avant", "docomo", 
                            "novarra", "palmos", "palmsource", 
                            "240x320", "opwv", "chtml",
                            "pda", "windows ce", "mmp/", 
                            "blackberry", "mib/", "symbian", 
                            "wireless", "nokia", "hand", "mobi",
                            "phone", "cdm", "up.b", "audio", 
                            "SIE-", "SEC-", "samsung", "HTC", 
                            "mot-", "mitsu", "sagem", "sony"
                            , "alcatel", "lg", "eric", "vx", 
                            "NEC", "philips", "mmm", "xx", 
                            "panasonic", "sharp", "wap", "sch",
                            "rover", "pocket", "benq", "java", 
                            "pt", "pg", "vox", "amoi", 
                            "bird", "compal", "kg", "voda",
                            "sany", "kdd", "dbt", "sendo", 
                            "sgh", "gradi", "jb", "dddi", 
                            "moto", "iphone"
                        };

                //Loop through each item in the list created above 
                //and check if the header contains that text
                foreach (string s in mobiles)
                {
                    if (context.Request.ServerVariables["HTTP_USER_AGENT"].
                                                        ToLower().Contains(s.ToLower()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool EnviaEmail(ParametroResumido pr, string sDestinatario,
                               string sRemetente,
                               string sCopia,
                               string sCopiaOculta,
                               string sAssunto,
                               string sTexto,
                               string sAnexo,
                               string sReservado)
        {
            //cria objeto com dados do e-mail
            System.Net.Mail.MailMessage objEmail = new System.Net.Mail.MailMessage();

            //remetente do e-mail
            objEmail.From = new System.Net.Mail.MailAddress(sRemetente);

            //destinatrios do e-mail
            objEmail.To.Add(sDestinatario);

            //enviar cpia para
            if (!String.IsNullOrEmpty(sCopia))
            {
                objEmail.To.Add(sCopia);
            }

            //enviar cpia oculta para
            if (!String.IsNullOrEmpty(sCopiaOculta))
            {
                objEmail.Bcc.Add(sCopiaOculta);
            }

            //prioridade do e-mail
            objEmail.Priority = System.Net.Mail.MailPriority.Normal;

            //formato do e-mail HTML (caso no queira HTML alocar valor false)
            objEmail.IsBodyHtml = true;

            //ttulo do e-mail
            objEmail.Subject = sAssunto;

            //corpo do e-mail            
            objEmail.Body = sTexto;

            //Para evitar problemas de caracteres "estranhos", configuramos o charset para "ISO-8859-1"
            objEmail.SubjectEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
            objEmail.BodyEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");

            //cria objeto com os dados do SMTP
            System.Net.Mail.SmtpClient objSmtp = new System.Net.Mail.SmtpClient();

            #region login
            string sHost = BuscaCampoTabela("HostSmtp", "PARAMETROS", " AND CODEMP = " + pr.CodEmp);
            string sPorta = BuscaCampoTabela("PortaSmtp", "PARAMETROS", " AND CODEMP = " + pr.CodEmp);            
            string sSenha = BuscaCampoTabela("SenhaSmtp", "PARAMETROS", " AND CODEMP = " + pr.CodEmp);

            objSmtp.Host = sHost;
            objSmtp.Port = Convert.ToInt16(sPorta);
            objSmtp.Credentials = new System.Net.NetworkCredential(sRemetente, sSenha);
            #endregion

            try
            {
                objSmtp.Send(objEmail);
                objEmail.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
                //return false;
            }

        }

    }
}
