using System;
using System.IO;
using System.Net;


namespace WebPedidos.WSClasses
{
    public class ClasseFtp
    {
        private FtpWebRequest ftpRequest = null;
        private FtpWebResponse ftpResponse = null;
        private Stream ftpStream = null;
        private int bufferSize = 2048;
        private string _sErro;
        ClasseBanco conn = new ClasseBanco();
        public string sErro
        {
            get { return _sErro; }
            set { _sErro = value; }
        }

        public bool VerificaSeExiste(string destinfilepath, string ftphost, string ftpfilepath, string user, string pass)
        {
            ftpRequest = (FtpWebRequest)FtpWebRequest.Create("ftp://" + ftphost + "//" + ftpfilepath);

            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;

            ftpRequest.Credentials = new NetworkCredential(user, pass);

            ftpRequest.UseBinary = true;
            ftpRequest.UsePassive = true;
            ftpRequest.KeepAlive = true;

            try {
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpResponse.Close();                
                ftpRequest = null;
                return true;
            }
            catch (Exception exc)
            {
                this.sErro = exc.Message;                
                return false;
            }
             
        }

        public bool DownloadFileFTP(string destinfilepath, string ftphost, string ftpfilepath, string user, string pass)
        {
            try
            {

                ftpRequest = (FtpWebRequest)FtpWebRequest.Create("ftp://" + ftphost + "//" + ftpfilepath);             
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpRequest.Credentials = new NetworkCredential(user, pass);             
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;                             
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();             
                ftpStream = ftpResponse.GetResponseStream();                
                FileStream localFileStream = new FileStream(destinfilepath, FileMode.Create);
                
                byte[] byteBuffer = new byte[bufferSize];
                int bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
                
                while (bytesRead > 0)
                {
                    localFileStream.Write(byteBuffer, 0, bytesRead);
                    bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
                }
                
                localFileStream.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
                return true;

            }
            catch (Exception erro)
            {
                throw new Exception(erro.Message);                
            }
        }

        public void VerificarArquivos(string sPathDestino, string ftphost, string ftpfilepath, string user, string pass)
        {

            try
            {
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create("ftp://" + ftphost + "//" + ftpfilepath);

                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;

                ftpRequest.Credentials = new NetworkCredential(user, pass);

                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                            
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();

                StreamReader sr = new StreamReader(ftpResponse.GetResponseStream());
    
                string str = sr.ReadLine();

                while (str != null)
                  {
                    Console.WriteLine(str);
                    str = sr.ReadLine();

                    if (str != ".." && str!=null)
                    {                        
                        if (!DownloadFileFTP((sPathDestino + "\\" + str), ftphost, (ftpfilepath.Replace("\\", "//") + "//" + str), user, pass))
                        {
                            return;
                        }
                        else
                        {
                            conn.ExecutarComando("INSERT INTO LOG_ARQUIVOS_RECEBIDOS (CODVEN, ARQUIVO, NOME_ARQUIVO) VALUES (" + str.Substring(0, 2) + ", '" + (sPathDestino + "\\" + str) + "', '" + str + "')");

                            //Excluir arquivo já recebido do FTP
                            DeleteFileFTP(ftphost, (ftpfilepath.Replace("\\", "//") + "//" + str), user, pass);
                        }
                    }

                 }

                ftpResponse.Close();                
                ftpRequest = null;
               
            }
            catch (Exception exc)
            {
                this.sErro = exc.Message;     
            }

        }

        public void UploadFileFTP(string arquivoOrigem, string ftphost, string ftpfilepath, string nomeArquivo, string user, string pass)
        {
            try
            {
                string ftpfullpath = "ftp://" + ftphost + ftpfilepath + nomeArquivo;
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpfullpath);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(user, pass);
                byte[] bytes = File.ReadAllBytes(arquivoOrigem);
                request.ContentLength = bytes.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();                
            }
            catch (Exception erro)
            {
                throw new Exception(erro.Message);      
            }
        }

        public bool DeleteFileFTP(string ftphost, string ftpfilepath, string user, string pass)
        {
            try
            {
                string ftpfullpath = "ftp://" + ftphost + "//" + ftpfilepath;
                var requestFileDelete = (FtpWebRequest)WebRequest.Create(ftpfullpath);
                requestFileDelete.Credentials = new NetworkCredential(user, pass);
                requestFileDelete.Method = WebRequestMethods.Ftp.DeleteFile;
                var responseFileDelete = (FtpWebResponse)requestFileDelete.GetResponse();
                if (responseFileDelete != null) responseFileDelete.Close();
                return true;
            }
            catch (Exception exc)
            {
                this.sErro = exc.Message;
                return false;
            }
        }
                
    }
}