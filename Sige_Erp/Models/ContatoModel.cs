using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Uteis;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace Sige_Erp.Models
{
    public class ContatoModel
    {
        public int NrSeqContato { get; set; }
        public int NrSeqPessoa { get; set; }

        [Required(ErrorMessage = "Informe o e-mail de contato")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Informe o número de telefone de contato")]
        [Phone(ErrorMessage = "Formato de número de telefone inválido")]
        public string Telefone { get; set; }

        public int ObterUltimoNrSeqContatoInserido(DAL objDAL)
        {
            int ultimoNrSeqContato = 0;

            try
            {
                string sql = "SELECT NrSeqContato FROM Contato ORDER BY NrSeqContato DESC LIMIT 1";
                DataTable dt = objDAL.RetDataTable(sql);

                if (dt.Rows.Count == 1)
                {
                    ultimoNrSeqContato = Convert.ToInt32(dt.Rows[0]["NrSeqContato"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao encontrar ultimo contato: {ex.Message}");
            }

            return ultimoNrSeqContato;
        }

        public void Cadastrar(DAL objDAL)
        {
            try
            {

                // Obtém o último NrSeqContato
                int ultimoNrSeqContato = ObterUltimoNrSeqContatoInserido(objDAL);

                // Incrementa o NrSeqContato
                NrSeqContato = ultimoNrSeqContato + 1;
                string sql = $"INSERT INTO Contato (NrSeqContato, NrSeqPessoa, Email, Telefone) " +
                                 $"VALUES ({NrSeqContato}, {NrSeqPessoa}, '{Email}', '{Telefone}')";
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
                
                    string sql = $"UPDATE Contato SET  Email = '{Email}', Telefone = '{Telefone}' " +
                                 $"WHERE NrSeqPessoa = {NrSeqPessoa}";
                    objDAL.ExecutarComandoSQL(sql);
                
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar: {ex.Message}");
            }
        }

        public void Excluir(DAL objDAL)
        {
            try
            {
                string sql = $"DELETE FROM Contato WHERE NrSeqContato = {NrSeqContato}";
                objDAL.ExecutarComandoSQL(sql);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao excluir contato com NrSeqContato {NrSeqContato}");
            }
            finally
            {
                if (objDAL != null)
                {
                    objDAL.Dispose();
                }
            }
        }


        public DataTable ListarTodos()
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = "SELECT * FROM Contato";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                // Trate as exceções apropriadamente (por exemplo, registre ou lance)
            }
            return dt;
        }

        public DataTable ListarContatoEspecifico(int nrSeqContato)
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM Contato WHERE NrSeqContato = {nrSeqContato}";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                // Trate as exceções apropriadamente (por exemplo, registre ou lance)
            }
            return dt;
        }

        public void CarregarDados()
        {
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string where = "1=1";
                    if (NrSeqPessoa > 0)
                    {
                        where += $@" and NrSeqPessoa = '{NrSeqPessoa}'";
                    }
                    if (NrSeqContato > 0)
                    {
                        where += $@" and NrSeqContato = '{NrSeqContato}'";
                    }

                    string sql = $"SELECT * FROM Contato WHERE {where}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        NrSeqContato = Convert.ToInt32(dt.Rows[0]["NrSeqContato"]);
                        NrSeqPessoa = Convert.ToInt32(dt.Rows[0]["NrSeqPessoa"]);
                        Email = dt.Rows[0]["Email"].ToString();
                        Telefone = dt.Rows[0]["Telefone"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar dados do Contato: {ex.Message}");
            }
        }
    }
}
