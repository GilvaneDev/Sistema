using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Uteis;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;


namespace Sige_Erp.Models
{
    public class LoginModel 
    {
        private const int SaltSize = 16;
        private const int Iterations = 10000;
        public int NrSeqUsuario { get; set; }
        public int NrSeqLogin { get; set; }
        public string? NomeDeUsuario { get; set; }


        [Required(ErrorMessage ="Informe o e-mail do usuario")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage ="O e-mail informado é invalido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Informe a senha do usuario")]
        public string Senha { get; set; }
        public bool ValidarLogin()
        {
            string sql = $"SELECT * FROM LOGIN WHERE EMAIL='{Email}'";
            DAL objDAL = new DAL();
            DataTable dt = objDAL.RetDataTable(sql);

            if (dt.Rows.Count == 1)
            {
                string senhaCriptografadaNoBanco = dt.Rows[0]["SENHA"].ToString();

               // string hashedPassword = HashPassword("12345");

                if (VerificarSenha(Senha, senhaCriptografadaNoBanco))
                {
                    NrSeqUsuario = Convert.ToInt32(dt.Rows[0]["NRSEQUSUARIO"]);
                    NomeDeUsuario = dt.Rows[0]["NOMEDEUSUARIO"].ToString();
                    return true;
                }
            }

            return false;
        }

        private bool VerificarSenha(string senhaDigitada, string senhaCriptografada)
        {
            byte[] hashBytes = Convert.FromBase64String(senhaCriptografada);
            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            using (var pbkdf2 = new Rfc2898DeriveBytes(senhaDigitada, salt, Iterations))
            {
                byte[] hash = pbkdf2.GetBytes(20); 
                byte[] hashBytesParaComparacao = new byte[SaltSize + 20];
                Array.Copy(salt, 0, hashBytesParaComparacao, 0, SaltSize);
                Array.Copy(hash, 0, hashBytesParaComparacao, SaltSize, 20);

                return Convert.ToBase64String(hashBytesParaComparacao) == senhaCriptografada;
            }
        }

        public int ObterUltimoNrSeqLoginInserido(DAL objDAL)
        {
            int ultimoNrSeqLogin = 0;

            try
            {
                string sql = "SELECT NrSeqLogin FROM Login ORDER BY NrSeqLogin DESC LIMIT 1";
                DataTable dt = objDAL.RetDataTable(sql);

                if (dt.Rows.Count == 1)
                {
                    ultimoNrSeqLogin = Convert.ToInt32(dt.Rows[0]["NrSeqLogin"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar: {ex.Message}");
            }

            return ultimoNrSeqLogin;
        }
        public void Cadastrar(DAL objDAL)
        {
            try
            {
                // Obtém o último NrSeqLogiin
                int ultimoNrSeqLogin = ObterUltimoNrSeqLoginInserido(objDAL);

                // Incrementa o NrSeqLogin
                NrSeqLogin = ultimoNrSeqLogin + 1;
                string hashedPassword = HashPassword(Senha); // Hash the password before storing
                    string sql = $"INSERT INTO LOGIN (NRSEQUSUARIO,NRSEQLOGIN,EMAIL,NOMEDEUSUARIO, SENHA) VALUES ({NrSeqUsuario},{NrSeqLogin},'{Email}','{NomeDeUsuario}','{hashedPassword}')";
                    objDAL.ExecutarComandoSQL(sql);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar: {ex.Message}");
            }
        }
        public void Atualizar(DAL objDAL)
        {
            try
            {
                
                    string sql = $"UPDATE LOGIN SET NOMEDEUSUARIO = '{NomeDeUsuario}', SENHA = '{HashPassword(Senha)}', EMAIL = '{Email}' WHERE NRSEQUSUARIO = {NrSeqUsuario}";
                    objDAL.ExecutarComandoSQL(sql);
                
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao Atualizar: {ex.Message}");
            }
        }
        public void AtualizarEmail(DAL objDAL)
        {
            try
            {

                string sql = $"UPDATE LOGIN SET  EMAIL = '{Email}' WHERE NRSEQLOGIN = {NrSeqLogin}";
                objDAL.ExecutarComandoSQL(sql);

            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao Atualizar: {ex.Message}");
            }
        }
        public void Excluir(DAL objDAL)
        {
            try
            {
                string sql = $"DELETE FROM LOGIN WHERE NRSEQLOGIN = {NrSeqLogin}";
                objDAL.ExecutarComandoSQL(sql);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao excluir usuário com NRSEQLOGIN {NrSeqLogin}", ex);
            }
            finally
            {
                if (objDAL != null)
                {
                    objDAL.Dispose();
                }
            }
        }


        public void CarregarDados()
        {
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string where = "1=1";
                    if (NrSeqLogin > 0)
                    {
                        where += $@" and NrSeqLogin = '{NrSeqLogin}'";
                    }
                    if (NrSeqUsuario > 0)
                    {
                        where += $@" and NrSeqUsuario = '{NrSeqUsuario}'";
                    }
                    string sql = $"SELECT * FROM LOGIN WHERE {where}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        NrSeqLogin = Convert.ToInt32(dt.Rows[0]["NrSeqLogin"]);
                        NrSeqUsuario = Convert.ToInt32(dt.Rows[0]["NrSeqUsuario"]);
                        Email = dt.Rows[0]["Email"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar: {ex.Message}");
            }
        }



        private string HashPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                byte[] hash = pbkdf2.GetBytes(20); // 20 bytes for SHA-1
                byte[] hashBytes = new byte[SaltSize + 20];
                Array.Copy(salt, 0, hashBytes, 0, SaltSize);
                Array.Copy(hash, 0, hashBytes, SaltSize, 20);
                string hashedPassword = Convert.ToBase64String(hashBytes);

                return hashedPassword;
            }
        }

    }
}
