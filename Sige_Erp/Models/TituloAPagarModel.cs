using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Uteis;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Sige_Erp.Models
{
    public class TituloAPagarModel
    {
        public string NomeDoFornecedor { get; set; }
        public decimal ValorPago { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal SaldoTotal { get; set; }
        public int NrSeqNf { get; set; }
        public int NrSeqPedido { get; set; }
        public int NrSeqProduto { get; set; }
        public int NrSeqPessoaCli { get; set; }
        public int NrSeqPessoaFor { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DeDataCadastro { get; set; }
        public DateTime? AteDataCadastro { get; set; }
        public string Serie { get; set; }
        public string NomeProduto { get; set; }
        public decimal ValorTituloPagar { get; set; }
        public List<TituloAPagarModel> ListaTitulos { get; set; }
        public List<Int32> IdsSelecionados { get; set; }
        public string NrNf { get; set; }
        public bool AbaAtiva { get; set; }
        public bool FlgEdicao { get; set; }
        public string PdfBase64 { get; set; }
       


        public int ObterUltimoNrSeqNfInserido(DAL objDAL)
        {
            int ultimoNrSeqNf = 0;

            try
            {
                string sql = "SELECT NrSeqNf FROM TituloAPagar ORDER BY NrSeqNf DESC LIMIT 1";
                DataTable dt = objDAL.RetDataTable(sql);

                if (dt.Rows.Count == 1)
                {
                    ultimoNrSeqNf = Convert.ToInt32(dt.Rows[0]["NrSeqNf"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar: {ex.Message}");
            }

            return ultimoNrSeqNf;
        }
        public void Cadastrar(DAL objDAL)
        {
            try
            {
                // Obtém o último NrSeqNf
                int ultimoNrSeqNf = ObterUltimoNrSeqNfInserido(objDAL);

                // Incrementa o NrSeqNf
                NrSeqNf = ultimoNrSeqNf + 1;

                // Define a instrução SQL com parâmetros
                string sql = "INSERT INTO TituloAPagar (NomeDoFornecedor, ValorPago, NrSeqNf, DataCadastro, Serie, NomeProduto, ValorTituloPagar, NrNf) " +
                             "VALUES (@NomeDoFornecedor, @ValorPago, @NrSeqNf, @DataCadastro, @Serie, @NomeProduto, @ValorTituloPagar, @NrNf)";

                // Cria e configura os parâmetros
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    new MySqlParameter("@NomeDoFornecedor", NomeDoFornecedor),
                    new MySqlParameter("@ValorPago", ValorPago),
                    new MySqlParameter("@NrSeqNf", NrSeqNf),
                    new MySqlParameter("@DataCadastro", DataCadastro.ToString("yyyy-MM-dd")),
                    new MySqlParameter("@Serie", Serie),
                    new MySqlParameter("@NomeProduto", NomeProduto),
                    new MySqlParameter("@ValorTituloPagar", ValorTituloPagar),
                    new MySqlParameter("@NrNf", NrNf)
                };

                // Executa o comando SQL com parâmetros
                objDAL.ExecutarComandoSQL(sql, parameters);
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
                // Define a instrução SQL com parâmetros
                string sql = "UPDATE TituloAPagar SET NomeDoFornecedor = @NomeDoFornecedor, ValorPago = @ValorPago, " +
                             "NrSeqNf = @NrSeqNf, DataCadastro = @DataCadastro, Serie = @Serie, " +
                             "NomeProduto = @NomeProduto, ValorTituloPagar = @ValorTituloPagar, NrNf = @NrNf " +
                             "WHERE NrSeqNf = @NrSeqNf";

                // Cria e configura os parâmetros
                List<MySqlParameter> parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@NomeDoFornecedor", NomeDoFornecedor),
            new MySqlParameter("@ValorPago", ValorPago),
            new MySqlParameter("@NrSeqNf", NrSeqNf),
            new MySqlParameter("@DataCadastro", DataCadastro.ToString("yyyy-MM-dd")),
            new MySqlParameter("@Serie", Serie),
            new MySqlParameter("@NomeProduto", NomeProduto),
            new MySqlParameter("@ValorTituloPagar", ValorTituloPagar),
            new MySqlParameter("@NrNf", NrNf)
        };

                // Executa o comando SQL com parâmetros
                objDAL.ExecutarComandoSQL(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar: {ex.Message}");
            }
        }

        public List<TituloAPagarModel> Pesquisar(TituloAPagarModel titulo)
        {
            string where = "1=1";

            if (!string.IsNullOrEmpty(titulo.NomeProduto))
            {
                where += $@" AND TITULOAPAGAR.NOMEPRODUTO LIKE '%{titulo.NomeProduto}%'";
            }
            if (!string.IsNullOrEmpty(titulo.NomeDoFornecedor))
            {
                where += $@" AND TITULOAPAGAR.NOMEDOFORNECEDOR LIKE '%{titulo.NomeDoFornecedor}%'";
            }
            if (titulo.ValorPago > 0)
            {
                where += $@" AND TITULOAPAGAR.VALORPAGO ={titulo.ValorPago}";
            }
            if (titulo.ValorTituloPagar > 0)
            {
                where += $@" AND TITULOAPAGAR.VALORTITULOPAGAR ={titulo.ValorTituloPagar}";
            }

            if (!string.IsNullOrEmpty(titulo.NrNf))
            {
                where += $@" AND TITULOAPAGAR.NRNF LIKE '%{titulo.NrNf}%'";
            }
            if (!string.IsNullOrEmpty(titulo.Serie))
            {
                where += $@" AND TITULOAPAGAR.SERIE LIKE '%{titulo.Serie}%'";
            }
           


            string query = $@"
                SELECT
                    *
                    
                FROM `TITULOAPAGAR`
                WHERE {where}";

            List<TituloAPagarModel> titulos = new List<TituloAPagarModel>();

            try
            {
                using (DAL objDAL = new DAL())
                {
                    DataTable dt = objDAL.RetDataTable(query);

                    foreach (DataRow row in dt.Rows)
                    {
                        TituloAPagarModel ttitulo = new TituloAPagarModel();
                        ttitulo.NomeDoFornecedor = row["NomeDoFornecedor"] == DBNull.Value ? string.Empty : Convert.ToString(row["NomeDoFornecedor"]);
                        ttitulo.ValorPago = row["ValorPago"] == DBNull.Value ? 0 : Convert.ToDecimal(row["ValorPago"]);
                        ttitulo.NrSeqNf = row["NrSeqNf"] == DBNull.Value ? 0 : Convert.ToInt32(row["NrSeqNf"]);
                        ttitulo.DataCadastro = row["DataCadastro"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["DataCadastro"]);
                        ttitulo.Serie = row["Serie"] == DBNull.Value ? string.Empty : Convert.ToString(row["Serie"]);
                        ttitulo.NomeProduto = row["NomeProduto"] == DBNull.Value ? string.Empty : Convert.ToString(row["NomeProduto"]);
                        ttitulo.ValorTituloPagar = row["ValorTituloPagar"] == DBNull.Value ? 0 : Convert.ToDecimal(row["ValorTituloPagar"]);
                        ttitulo.NrNf = row["NrNf"] == DBNull.Value ? string.Empty : Convert.ToString(row["NrNf"]);


                        titulos.Add(ttitulo);
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao pesquisar: {ex.Message}");
            }

            return titulos;
        }

        public void Excluir(DAL objDAL)
        {
            try
            {
  
                    string sql = $"DELETE FROM TituloAPagar WHERE NrSeqNf = {NrSeqNf}";
                    objDAL.ExecutarComandoSQL(sql);
                
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao excluir: {ex.Message}");
            }
        }

        public DataTable ListarTodos()
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = "SELECT * FROM TituloAPagar";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                // Trate as exceções apropriadamente (por exemplo, registre ou lance)
            }
            return dt;
        }

        public DataTable ListarTituloAPagarEspecifico(int nrSeqNf)
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM TituloAPagar WHERE NrSeqNf = {nrSeqNf}";
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
                    string sql = $"SELECT * FROM TituloAPagar WHERE NrSeqNf = {NrSeqNf}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        NomeDoFornecedor = dt.Rows[0]["NomeDoFornecedor"].ToString();
                        ValorPago = Convert.ToDecimal(dt.Rows[0]["ValorPago"]);
                        NrSeqNf = Convert.ToInt32(dt.Rows[0]["NrSeqNf"]);
                        DataCadastro = Convert.ToDateTime(dt.Rows[0]["DataCadastro"]);
                        Serie = dt.Rows[0]["Serie"].ToString();
                        NomeProduto = dt.Rows[0]["NomeProduto"].ToString();
                        ValorTituloPagar = Convert.ToDecimal(dt.Rows[0]["ValorTituloPagar"]);
                        NrNf = dt.Rows[0]["NrNf"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                // Trate as exceções apropriadamente (por exemplo, registre ou lance)
            }
        }
    }
}
