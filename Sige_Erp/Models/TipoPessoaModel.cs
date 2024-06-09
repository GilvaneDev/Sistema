using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Uteis;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace Sige_Erp.Models
{
    public class TipoPessoaModel
    {
        public string CpfCnpj { get; set; }
        public int NrSeqTipoPessoa { get; set; }
        public int NrSeqContato { get; set; }
        public int NrSeqPessoa { get; set; }

        [Required(ErrorMessage = "Informe o e-mail")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Informe o número de telefone")]
        [Phone(ErrorMessage = "Formato de número de telefone inválido")]
        public string Telefone { get; set; }

        public int ObterUltimoNrSeqTipoPessoaInserido(DAL objDAL)
        {
            int ultimoNrSeqTipoPessoa = 0;

            try
            {
                string sql = "SELECT NrSeqTipoPessoa FROM TipoPessoa ORDER BY NrSeqTipoPessoa DESC LIMIT 1";
                DataTable dt = objDAL.RetDataTable(sql);

                if (dt.Rows.Count == 1)
                {
                    ultimoNrSeqTipoPessoa = Convert.ToInt32(dt.Rows[0]["NrSeqTipoPessoa"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar: {ex.Message}");
            }

            return ultimoNrSeqTipoPessoa;
        }

        public void Cadastrar(DAL objDAL)
        {
            try
            {
                // Obtém o último NrSeqTipoPessoa
                int ultimoNrSeqTipoPessoa = ObterUltimoNrSeqTipoPessoaInserido(objDAL);

                // Incrementa o NrSeqTipoPessoa
                NrSeqTipoPessoa = ultimoNrSeqTipoPessoa + 1;
                string sql = $"INSERT INTO TipoPessoa (CpfCnpj, NrSeqTipoPessoa, NrSeqContato, NrSeqPessoa, Email, Telefone) " +
                                 $"VALUES ('{CpfCnpj}', {NrSeqTipoPessoa}, {NrSeqContato}, {NrSeqPessoa}, '{Email}', '{Telefone}')";
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
                
                    string sql = $"UPDATE TipoPessoa SET CpfCnpj = '{CpfCnpj}', Email = '{Email}', Telefone = '{Telefone}' " +
                                 $"WHERE  NrSeqPessoa = {NrSeqPessoa}";
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

                string sql = $"DELETE FROM TipoPessoa WHERE NrSeqTipoPessoa = {NrSeqTipoPessoa}";
                objDAL.ExecutarComandoSQL(sql);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao excluir TipoPessoa com NrSeqTipoPessoa {NrSeqTipoPessoa}");
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
                    string sql = "SELECT * FROM TipoPessoa";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                // Trate as exceções apropriadamente (por exemplo, registre ou lance)
            }
            return dt;
        }

        public DataTable ListarTipoPessoaEspecifico(int nrSeqTipoPessoa, int nrSeqPessoa)
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM TipoPessoa WHERE NrSeqTipoPessoa = {nrSeqTipoPessoa} AND NrSeqPessoa = {nrSeqPessoa}";
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
                    if (NrSeqTipoPessoa > 0)
                    {
                        where += $@" and NrSeqTipoPessoa = '{NrSeqTipoPessoa}'";
                    }

                    string sql = $"SELECT * FROM TipoPessoa WHERE {where}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        CpfCnpj = dt.Rows[0]["CpfCnpj"].ToString();
                        NrSeqTipoPessoa = Convert.ToInt32(dt.Rows[0]["NrSeqTipoPessoa"]);
                        NrSeqPessoa = Convert.ToInt32(dt.Rows[0]["NrSeqPessoa"]);
                        NrSeqContato = Convert.ToInt32(dt.Rows[0]["NrSeqContato"]);
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
