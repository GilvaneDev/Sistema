using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Uteis;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Sige_Erp.Models
{
    public class FornecedorModel
    {
        public string InscricaoMunicipal { get; set; }
        public DateTime DataCadastro { get; set; }
        public int NrSeqPessoa { get; set; }
        public int NrSeqFornecedor { get; set; }

        [Required(ErrorMessage = "Informe o nome fantasia do fornecedor")]
        public string NomeFantasia { get; set; }

        [Required(ErrorMessage = "Informe o nome do fornecedor")]
        public string NomeDoFornecedor { get; set; }

        public string InscricaoEstadual { get; set; }

        public int ObterUltimoNrSeqFornecedorInserido(DAL objDAL)
        {
            int ultimoNrSeqFornecedor = 0;

            try
            {
                string sql = "SELECT NrSeqFornecedor FROM Fornecedor ORDER BY NrSeqFornecedor DESC LIMIT 1";
                DataTable dt = objDAL.RetDataTable(sql);

                if (dt.Rows.Count == 1)
                {
                    ultimoNrSeqFornecedor = Convert.ToInt32(dt.Rows[0]["NrSeqFornecedor"]);
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter ultimo Fornecedor: {ex.Message}");
            }

            return ultimoNrSeqFornecedor;
        }


        public void Cadastrar(DAL objDAL)
        {
            try
            {
                // Obtém o último NrSeqFornecedor
                int ultimoNrSeqFornecedor = ObterUltimoNrSeqFornecedorInserido(objDAL);

                // Incrementa o NrSeqFornecedor
                NrSeqFornecedor = ultimoNrSeqFornecedor + 1;
                string sql = $"INSERT INTO Fornecedor (InscricaoMunicipal, DataCadastro, NrSeqPessoa, NrSeqFornecedor, NomeFantasia, NomeDoFornecedor, InscricaoEstadual) " +
                                 $"VALUES ('{InscricaoMunicipal}', '{DataCadastro:yyyy-MM-dd}', {NrSeqPessoa}, {NrSeqFornecedor}, '{NomeFantasia}', '{NomeDoFornecedor}', '{InscricaoEstadual}')";
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
               
                    string sql = $"UPDATE Fornecedor SET InscricaoMunicipal = '{InscricaoMunicipal}', DataCadastro = '{DataCadastro:yyyy-MM-dd}', " +
                                 $"NrSeqPessoa = {NrSeqPessoa}, NomeFantasia = '{NomeFantasia}', NomeDoFornecedor = '{NomeDoFornecedor}', " +
                                 $"InscricaoEstadual = '{InscricaoEstadual}' " +
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

                    string sql = $"DELETE FROM Fornecedor WHERE NrSeqFornecedor = {NrSeqFornecedor}";
                    objDAL.ExecutarComandoSQL(sql);
                
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao excluir Fornecedor com NrSeqFornecedor {NrSeqFornecedor}", ex);
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
                    string sql = "SELECT * FROM Fornecedor";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao Listar: {ex.Message}");
            }
            return dt;
        }

        public DataTable ListarFornecedorEspecifico(int nrSeqFornecedor)
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM Fornecedor WHERE NrSeqFornecedor = {nrSeqFornecedor}";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar: {ex.Message}");
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
                    if (NrSeqFornecedor > 0)
                    {
                        where += $@" and NrSeqFornecedor = '{NrSeqFornecedor}'";
                    }
                    string sql = $"SELECT * FROM Fornecedor WHERE {where}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        NrSeqFornecedor = Convert.ToInt32(dt.Rows[0]["NrSeqFornecedor"]);
                        InscricaoMunicipal = dt.Rows[0]["InscricaoMunicipal"].ToString();
                        DataCadastro = Convert.ToDateTime(dt.Rows[0]["DataCadastro"]);
                        NrSeqPessoa = Convert.ToInt32(dt.Rows[0]["NrSeqPessoa"]);
                        NomeFantasia = dt.Rows[0]["NomeFantasia"].ToString();
                        NomeDoFornecedor = dt.Rows[0]["NomeDoFornecedor"].ToString();
                        InscricaoEstadual = dt.Rows[0]["InscricaoEstadual"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar: {ex.Message}");
            }
        }
        public List<FornecedorModel> Pesquisar(FornecedorModel fornecedorModel)
        {
            string where = "1=1";

            string query = $@"
                SELECT
                    FORNECEDOR.`NRSEQPESSOA`,
                    FORNECEDOR.`NRSEQFORNECEDOR`,
                    FORNECEDOR.`NOMEDOFORNECEDOR`,
                    PESSOA.`NRSEQPESSOA`
                FROM `FORNECEDOR`
                LEFT JOIN `PESSOA` ON FORNECEDOR.`NRSEQPESSOA` = PESSOA.`NRSEQPESSOA`
                WHERE {where}";

            List<FornecedorModel> fornecedores = new List<FornecedorModel>();

            try
            {
                using (DAL objDAL = new DAL())
                {
                    DataTable dt = objDAL.RetDataTable(query);

                    foreach (DataRow row in dt.Rows)
                    {
                        FornecedorModel fornecedor = new FornecedorModel();
                        fornecedor.NrSeqPessoa = row["NRSEQPESSOA"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["NRSEQPESSOA"]);
                        fornecedor.NrSeqFornecedor = row["NRSEQFORNECEDOR"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["NRSEQFORNECEDOR"]);
                        fornecedor.NomeDoFornecedor = row["NomeDoFornecedor"] == DBNull.Value ? string.Empty : Convert.ToString(row["NomeDoFornecedor"]);
                        fornecedores.Add(fornecedor);
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao pesquisar: {ex.Message}");
            }

            return fornecedores;
        }

    }
}
