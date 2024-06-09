using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Uteis;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Sige_Erp.Models
{
    public class ComissaoModel
    {
        public DateTime DataCadastro { get; set; }
        public DateTime? deCadastro { get; set; }
        public DateTime? ateCadastro { get; set; }
        public decimal PorcentageComissao { get; set; }
        public int NrSeqComissao { get; set; }
        public decimal ValorComissao { get; set; }
        public string NomeFuncionario { get; set; }
        public decimal ValorVenda { get; set; }
        public int NrSeqFuncionario { get; set; }
        public int NrSeqPedido { get; set; }
        public int NrSeqProduto { get; set; }
        public string NomeDoProduto { get; set; }
        public bool AbaAtiva { get; set; }
        public bool FlgEdicao { get; set; }
        public string PdfBase64 { get; set; }
        public List<ComissaoModel> ListaComissao { get; set; }
        public List<Int32> IdsSelecionados { get; set; }

        public int ObterUltimoNrSeqComissaoInserido(DAL objDAL)
        {
            int ultimoNrSeqComissao = 0;

            try
            {
                string sql = "SELECT NrSeqComissao FROM Comissao ORDER BY NrSeqComissao DESC LIMIT 1";
                DataTable dt = objDAL.RetDataTable(sql);

                if (dt.Rows.Count == 1)
                {
                    ultimoNrSeqComissao = Convert.ToInt32(dt.Rows[0]["NrSeqComissao"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter o último NrSeqComissao: {ex.Message}");
            }

            return ultimoNrSeqComissao;
        }

        public void Cadastrar(DAL objDAL)
        {
            try
            {
                // Obtém o último NrSeqComissao
                int ultimoNrSeqComissao = ObterUltimoNrSeqComissaoInserido(objDAL);

                // Incrementa o NrSeqComissao
                NrSeqComissao = ultimoNrSeqComissao + 1;

                // Define a instrução SQL com parâmetros
                string sql = "INSERT INTO Comissao (DataCadastro, PorcentageComissao, NrSeqComissao, ValorComissao, NomeFuncionario, ValorVenda, NrSeqFuncionario, NrSeqPedido, NrSeqProduto) " +
                             "VALUES (@DataCadastro, @PorcentageComissao, @NrSeqComissao, @ValorComissao, @NomeFuncionario, @ValorVenda, @NrSeqFuncionario, @NrSeqPedido, @NrSeqProduto)";

                // Cria e configura os parâmetros
                List<MySqlParameter> parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@DataCadastro", DataCadastro.ToString("yyyy-MM-dd")),
            new MySqlParameter("@PorcentageComissao", PorcentageComissao),
            new MySqlParameter("@NrSeqComissao", NrSeqComissao),
            new MySqlParameter("@ValorComissao", ValorComissao),
            new MySqlParameter("@NomeFuncionario", NomeFuncionario),
            new MySqlParameter("@ValorVenda", ValorVenda),
            new MySqlParameter("@NrSeqFuncionario", NrSeqFuncionario),
            new MySqlParameter("@NrSeqPedido", NrSeqPedido),
            new MySqlParameter("@NrSeqProduto", NrSeqProduto)
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
                string sql = "UPDATE Comissao SET DataCadastro = @DataCadastro, PorcentageComissao = @PorcentageComissao, ValorComissao = @ValorComissao, " +
                             "NomeFuncionario = @NomeFuncionario, ValorVenda = @ValorVenda, NrSeqFuncionario = @NrSeqFuncionario, " +
                             "NrSeqPedido = @NrSeqPedido, NrSeqProduto = @NrSeqProduto WHERE NrSeqComissao = @NrSeqComissao";

                // Cria e configura os parâmetros
                List<MySqlParameter> parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@DataCadastro", DataCadastro.ToString("yyyy-MM-dd")),
            new MySqlParameter("@PorcentageComissao", PorcentageComissao),
            new MySqlParameter("@ValorComissao", ValorComissao),
            new MySqlParameter("@NomeFuncionario", NomeFuncionario),
            new MySqlParameter("@ValorVenda", ValorVenda),
            new MySqlParameter("@NrSeqFuncionario", NrSeqFuncionario),
            new MySqlParameter("@NrSeqPedido", NrSeqPedido),
            new MySqlParameter("@NrSeqProduto", NrSeqProduto),
            new MySqlParameter("@NrSeqComissao", NrSeqComissao)
        };

                // Executa o comando SQL com parâmetros
                objDAL.ExecutarComandoSQL(sql, parameters);
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
            
                    string sql = $"DELETE FROM Comissao WHERE NrSeqComissao = {NrSeqComissao}";
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
                    string sql = "SELECT * FROM Comissao";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                // Trate as exceções apropriadamente (por exemplo, registre ou lance)
            }
            return dt;
        }

        public List<ComissaoModel> Pesquisar(ComissaoModel comissao)
        {
            string where = "1=1";

            if (comissao.deCadastro.HasValue && comissao.ateCadastro.HasValue &&
               comissao.deCadastro.Value > DateTime.MinValue && comissao.ateCadastro.Value > DateTime.MinValue)
            {
                string dataCadastroInicio = comissao.deCadastro.Value.ToString("yyyy-MM-dd");
                string dataCadastroFinal = comissao.ateCadastro.Value.ToString("yyyy-MM-dd");

                where += $@" AND COMISSAO.DATACADASTRO BETWEEN '{dataCadastroInicio}' AND '{dataCadastroFinal}'";
            }
            if (comissao.PorcentageComissao > 0)
            {
                where += $@" AND Comissao.PorcentageComissao = {comissao.PorcentageComissao}";
            }
            if (!string.IsNullOrEmpty(comissao.NomeFuncionario))
            {
                where += $@" AND Comissao.NomeFuncionario LIKE '%{comissao.NomeFuncionario}%'";
            }
            if (comissao.ValorComissao > 0)
            {
                where += $@" AND Comissao.ValorComissao = {comissao.ValorComissao}";
            }
            if (comissao.ValorVenda > 0)
            {
                where += $@" AND Comissao.ValorVenda = {comissao.ValorVenda}";
            }
            if (comissao.NrSeqFuncionario > 0)
            {
                where += $@" AND Comissao.NrSeqFuncionario = {comissao.NrSeqFuncionario}";
            }
            if (comissao.NrSeqPedido > 0)
            {
                where += $@" AND Comissao.NrSeqPedido = {comissao.NrSeqPedido}";
            }
            if (comissao.NrSeqProduto > 0)
            {
                where += $@" AND Comissao.NrSeqProduto = {comissao.NrSeqProduto}";
            }

            string query = $@"
        SELECT
            *
        FROM Comissao
        WHERE {where}";

            List<ComissaoModel> comissoes = new List<ComissaoModel>();

            try
            {
                using (DAL objDAL = new DAL())
                {
                    DataTable dt = objDAL.RetDataTable(query);

                    foreach (DataRow row in dt.Rows)
                    {
                        ComissaoModel com = new ComissaoModel();

                        com.DataCadastro = row["DataCadastro"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["DataCadastro"]);
                        com.PorcentageComissao = row["PorcentageComissao"] == DBNull.Value ? 0 : Convert.ToDecimal(row["PorcentageComissao"]);
                        com.NrSeqComissao = row["NrSeqComissao"] == DBNull.Value ? 0 : Convert.ToInt32(row["NrSeqComissao"]);
                        com.ValorComissao = row["ValorComissao"] == DBNull.Value ? 0 : Convert.ToDecimal(row["ValorComissao"]);
                        com.NomeFuncionario = row["NomeFuncionario"] == DBNull.Value ? string.Empty : Convert.ToString(row["NomeFuncionario"]);
                        com.ValorVenda = row["ValorVenda"] == DBNull.Value ? 0 : Convert.ToDecimal(row["ValorVenda"]);
                        com.NrSeqFuncionario = row["NrSeqFuncionario"] == DBNull.Value ? 0 : Convert.ToInt32(row["NrSeqFuncionario"]);
                        com.NrSeqPedido = row["NrSeqPedido"] == DBNull.Value ? 0 : Convert.ToInt32(row["NrSeqPedido"]);
                        com.NrSeqProduto = row["NrSeqProduto"] == DBNull.Value ? 0 : Convert.ToInt32(row["NrSeqProduto"]);

                            if(com.NrSeqProduto > 0 )
                            {
                                ProdutoModel  produto = new ProdutoModel();
                                produto.NrSeqProduto = com.NrSeqProduto;
                                produto.CarregarDados();

                                com.NomeDoProduto= produto.NomeDoProduto;

                            }
                        

                        comissoes.Add(com);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao pesquisar: {ex.Message}");
            }

            return comissoes;
        }


        public DataTable ListarComissaoEspecifica(int nrSeqComissao)
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM Comissao WHERE NrSeqComissao = {nrSeqComissao}";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao pesquisar: {ex.Message}");
            }
            return dt;
        }

        public void CarregarDados()
        {
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM Comissao WHERE NrSeqComissao = {NrSeqComissao}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        DataCadastro = Convert.ToDateTime(dt.Rows[0]["DataCadastro"]);
                        PorcentageComissao = Convert.ToDecimal(dt.Rows[0]["PorcentageComissao"]);
                        NrSeqComissao = Convert.ToInt32(dt.Rows[0]["NrSeqComissao"]);
                        ValorComissao = Convert.ToDecimal(dt.Rows[0]["ValorComissao"]);
                        NomeFuncionario = dt.Rows[0]["NomeFuncionario"].ToString();
                        ValorVenda = Convert.ToDecimal(dt.Rows[0]["ValorVenda"]);
                        NrSeqFuncionario = Convert.ToInt32(dt.Rows[0]["NrSeqFuncionario"]);
                        NrSeqPedido = Convert.ToInt32(dt.Rows[0]["NrSeqPedido"]);
                        NrSeqProduto = Convert.ToInt32(dt.Rows[0]["NrSeqProduto"]);
                        if (NrSeqProduto > 0)
                        {
                            ProdutoModel produto = new ProdutoModel();
                            produto.NrSeqProduto = NrSeqProduto;
                            produto.CarregarDados();

                            NomeDoProduto = produto.NomeDoProduto;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao pesquisar: {ex.Message}");
            }
        }
        public void BuscarPorNrSeqPedido(int nrSeqPedido)
        {
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM Comissao WHERE NrSeqPedido = {nrSeqPedido}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        NrSeqComissao = Convert.ToInt32(dt.Rows[0]["NrSeqComissao"]);
                        DataCadastro = Convert.ToDateTime(dt.Rows[0]["DataCadastro"]);
                        PorcentageComissao = Convert.ToDecimal(dt.Rows[0]["PorcentageComissao"]);
                        ValorComissao = Convert.ToDecimal(dt.Rows[0]["ValorComissao"]);
                        NomeFuncionario = dt.Rows[0]["NomeFuncionario"].ToString();
                        ValorVenda = Convert.ToDecimal(dt.Rows[0]["ValorVenda"]);
                        NrSeqFuncionario = Convert.ToInt32(dt.Rows[0]["NrSeqFuncionario"]);
                        NrSeqProduto = Convert.ToInt32(dt.Rows[0]["NrSeqProduto"]);

                        if (NrSeqProduto > 0)
                        {
                            ProdutoModel produto = new ProdutoModel();
                            produto.NrSeqProduto = NrSeqProduto;
                            produto.CarregarDados();
                            NomeDoProduto = produto.NomeDoProduto;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar por NrSeqPedido: {ex.Message}");
            }
        }

    }
}
