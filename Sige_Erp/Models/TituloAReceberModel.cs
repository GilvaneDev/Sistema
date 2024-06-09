using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Uteis;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Sige_Erp.Models
{
    public class TituloAReceberModel
    {
        public string NomeDoCliente { get; set; }
        public decimal ValorPago { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal SaldoTotal { get; set; }
        public int NrSeqNfv { get; set; }
        public int NrSeqPedido { get; set; }
        public int NrSeqProduto { get; set; }
        public int NrSeqPessoaCli { get; set; }
        public int NrSeqPessoaFor { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DeDataCadastro { get; set; }
        public DateTime? AteDataCadastro { get; set; }
        public string Serie { get; set; }
        public string NomeProduto { get; set; }
        public decimal ValorTituloReceber { get; set; }
        public List<TituloAReceberModel> ListaTitulos { get; set; }
        public List<Int32> IdsSelecionados { get; set; }
        public string NrNfv { get; set; }
        public bool AbaAtiva { get; set; }
        public bool FlgEdicao { get; set; }
        public string PdfBase64 { get; set; }



        public int ObterUltimoNrSeqNfvInserido(DAL objDAL)
        {
            int ultimoNrSeqNfv = 0;

            try
            {
                string sql = "SELECT NrSeqNfv FROM TituloAReceber ORDER BY NrSeqNfv DESC LIMIT 1";
                DataTable dt = objDAL.RetDataTable(sql);

                if (dt.Rows.Count == 1)
                {
                    ultimoNrSeqNfv = Convert.ToInt32(dt.Rows[0]["NrSeqNfv"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar: {ex.Message}");
            }

            return ultimoNrSeqNfv;
        }
        public void Cadastrar(DAL objDAL)
        {
            try
            {
                // Obtém o último NrSeqNfv
                int ultimoNrSeqNfv = ObterUltimoNrSeqNfvInserido(objDAL);

                // Incrementa o NrSeqNfv
                NrSeqNfv = ultimoNrSeqNfv + 1;

                // Define a instrução SQL com parâmetros
                string sql = "INSERT INTO TituloAReceber (NomeDoCliente, ValorPago, NrSeqNfv, DataCadastro, Serie, NomeProduto, ValorTituloReceber, NrNfv) " +
                             "VALUES (@NomeDoCliente, @ValorPago, @NrSeqNfv, @DataCadastro, @Serie, @NomeProduto, @ValorTituloReceber, @NrNfv)";

                // Cria e configura os parâmetros
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    new MySqlParameter("@NomeDoCliente", NomeDoCliente),
                    new MySqlParameter("@ValorPago", ValorPago),
                    new MySqlParameter("@NrSeqNfv", NrSeqNfv),
                    new MySqlParameter("@DataCadastro", DataCadastro.ToString("yyyy-MM-dd")),
                    new MySqlParameter("@Serie", Serie),
                    new MySqlParameter("@NomeProduto", NomeProduto),
                    new MySqlParameter("@ValorTituloReceber", ValorTituloReceber),
                    new MySqlParameter("@NrNfv", NrNfv)
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
                string sql = "UPDATE TituloAReceber SET NomeDoCliente = @NomeDoCliente, ValorPago = @ValorPago, " +
                             "NrSeqNfv = @NrSeqNfv, DataCadastro = @DataCadastro, Serie = @Serie, " +
                             "NomeProduto = @NomeProduto, ValorTituloReceber = @ValorTituloReceber, NrNfv = @NrNfv " +
                             "WHERE NrSeqNfv = @NrSeqNfv";

                // Cria e configura os parâmetros
                List<MySqlParameter> parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@NomeDoCliente", NomeDoCliente),
            new MySqlParameter("@ValorPago", ValorPago),
            new MySqlParameter("@NrSeqNfv", NrSeqNfv),
            new MySqlParameter("@DataCadastro", DataCadastro.ToString("yyyy-MM-dd")),
            new MySqlParameter("@Serie", Serie),
            new MySqlParameter("@NomeProduto", NomeProduto),
            new MySqlParameter("@ValorTituloReceber", ValorTituloReceber),
            new MySqlParameter("@NrNfv", NrNfv)
        };

                // Executa o comando SQL com parâmetros
                objDAL.ExecutarComandoSQL(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar: {ex.Message}");
            }
        }

        public List<TituloAReceberModel> Pesquisar(TituloAReceberModel titulo)
        {
            string where = "1=1";

            if (!string.IsNullOrEmpty(titulo.NomeProduto))
            {
                where += $@" AND TITULOAReceber.NOMEPRODUTO LIKE '%{titulo.NomeProduto}%'";
            }
            if (!string.IsNullOrEmpty(titulo.NomeDoCliente))
            {
                where += $@" AND TITULOAReceber.NomeDoCliente LIKE '%{titulo.NomeDoCliente}%'";
            }
            if (titulo.ValorPago > 0)
            {
                where += $@" AND TITULOAReceber.VALORPAGO ={titulo.ValorPago}";
            }
            if (titulo.ValorTituloReceber > 0)
            {
                where += $@" AND TITULOAReceber.VALORTITULOReceber ={titulo.ValorTituloReceber}";
            }

            if (!string.IsNullOrEmpty(titulo.NrNfv))
            {
                where += $@" AND TITULOAReceber.NrNfv LIKE '%{titulo.NrNfv}%'";
            }
            if (!string.IsNullOrEmpty(titulo.Serie))
            {
                where += $@" AND TITULOAReceber.SERIE LIKE '%{titulo.Serie}%'";
            }



            string query = $@"
                SELECT
                    *
                    
                FROM `TITULOAReceber`
                WHERE {where}";

            List<TituloAReceberModel> titulos = new List<TituloAReceberModel>();

            try
            {
                using (DAL objDAL = new DAL())
                {
                    DataTable dt = objDAL.RetDataTable(query);

                    foreach (DataRow row in dt.Rows)
                    {
                        TituloAReceberModel ttitulo = new TituloAReceberModel();
                        ttitulo.NomeDoCliente = row["NomeDoCliente"] == DBNull.Value ? string.Empty : Convert.ToString(row["NomeDoCliente"]);
                        ttitulo.ValorPago = row["ValorPago"] == DBNull.Value ? 0 : Convert.ToDecimal(row["ValorPago"]);
                        ttitulo.NrSeqNfv = row["NrSeqNfv"] == DBNull.Value ? 0 : Convert.ToInt32(row["NrSeqNfv"]);
                        ttitulo.DataCadastro = row["DataCadastro"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["DataCadastro"]);
                        ttitulo.Serie = row["Serie"] == DBNull.Value ? string.Empty : Convert.ToString(row["Serie"]);
                        ttitulo.NomeProduto = row["NomeProduto"] == DBNull.Value ? string.Empty : Convert.ToString(row["NomeProduto"]);
                        ttitulo.ValorTituloReceber = row["ValorTituloReceber"] == DBNull.Value ? 0 : Convert.ToDecimal(row["ValorTituloReceber"]);
                        ttitulo.NrNfv = row["NrNfv"] == DBNull.Value ? string.Empty : Convert.ToString(row["NrNfv"]);


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

                string sql = $"DELETE FROM TituloAReceber WHERE NrSeqNfv = {NrSeqNfv}";
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
                    string sql = "SELECT * FROM TituloAReceber";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                // Trate as exceções apropriadamente (por exemplo, registre ou lance)
            }
            return dt;
        }

        public DataTable ListarTituloAReceberEspecifico(int NrSeqNfv)
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM TituloAReceber WHERE NrSeqNfv = {NrSeqNfv}";
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
                    string sql = $"SELECT * FROM TituloAReceber WHERE NrSeqNfv = {NrSeqNfv}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        NomeDoCliente = dt.Rows[0]["NomeDoCliente"].ToString();
                        ValorPago = Convert.ToDecimal(dt.Rows[0]["ValorPago"]);
                        NrSeqNfv = Convert.ToInt32(dt.Rows[0]["NrSeqNfv"]);
                        DataCadastro = Convert.ToDateTime(dt.Rows[0]["DataCadastro"]);
                        Serie = dt.Rows[0]["Serie"].ToString();
                        NomeProduto = dt.Rows[0]["NomeProduto"].ToString();
                        ValorTituloReceber = Convert.ToDecimal(dt.Rows[0]["ValorTituloReceber"]);
                        NrNfv = dt.Rows[0]["NrNfv"].ToString();
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
