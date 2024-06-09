using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Uteis;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using MySqlX.XDevAPI;
using MySql.Data.MySqlClient;

namespace Sige_Erp.Models
{
    public class ProdutoModel
    {
        public DateTime DataCadastro { get; set; }
        public DateTime deCadastro { get; set; }
        public DateTime ateCadastro { get; set; }
        public int Quantidade { get; set; }
        public string Marca { get; set; }
        public int NrSeqProduto { get; set; }      
        public int CodigoProduto { get; set; }
        public decimal Preco { get; set; }
        
        public string NomeDoProduto { get; set; }

        public string Descricao { get; set; }
        public List<ProdutoModel> ListaProdutos { get; set; }
        public List<Int32> IdsSelecionados { get; set; }
        public bool AbaAtiva { get; set; }
        public string PdfBase64 { get; set; }

        public int ObterUltimoNrSeqProdutoInserido(DAL objDAL)
        {
            int ultimoNrSeqProduto = 0;

            try
            {
                string sql = "SELECT NrSeqProduto FROM Produto ORDER BY NrSeqProduto DESC LIMIT 1";
                DataTable dt = objDAL.RetDataTable(sql);

                if (dt.Rows.Count == 1)
                {
                    ultimoNrSeqProduto = Convert.ToInt32(dt.Rows[0]["NrSeqProduto"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar: {ex.Message}");
            }

            return ultimoNrSeqProduto;
        }

        public void Cadastrar(DAL objDAL)
        {
            try
            {
                // Obtém o último NrSeqProduto
                int ultimoNrSeqProduto = ObterUltimoNrSeqProdutoInserido(objDAL);

                // Incrementa o NrSeqProduto
                NrSeqProduto = ultimoNrSeqProduto + 1;
                // Monta a string SQL usando parâmetros para evitar SQL Injection
                string sql = "INSERT INTO Produto (DataCadastro, Quantidade, Marca, NrSeqProduto, CodigoProduto, Preco, NomeDoProduto, Descricao) " +
                             "VALUES (@DataCadastro, @Quantidade, @Marca, @NrSeqProduto, @CodigoProduto, @Preco, @NomeDoProduto, @Descricao)";

                // Cria e configura os parâmetros
                List<MySqlParameter> parameters = new List<MySqlParameter>
        {
            
            new MySqlParameter("@DataCadastro", DataCadastro.ToString("yyyy-MM-dd")),
            new MySqlParameter("@Quantidade", Quantidade),
            new MySqlParameter("@Marca", Marca),
            new MySqlParameter("@NrSeqProduto", NrSeqProduto),
            new MySqlParameter("@CodigoProduto", CodigoProduto),
            new MySqlParameter("@Preco", Preco),
            new MySqlParameter("@NomeDoProduto", NomeDoProduto),
            new MySqlParameter("@Descricao", Descricao),
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
                    string sql = $"UPDATE Produto SET DataCadastro = '{DataCadastro:yyyy-MM-dd}', Quantidade = {Quantidade}, " +
                                 $"Marca = '{Marca}', CodigoProduto = {CodigoProduto}, Preco = {Preco}, " +
                                 $"NomeDoProduto = '{NomeDoProduto}', Descricao = '{Descricao}' " +
                                 $"WHERE NrSeqProduto = {NrSeqProduto}";
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
     
                    string sql = $"DELETE FROM Produto WHERE NrSeqProduto = {NrSeqProduto}";
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
                    string sql = "SELECT * FROM Produto";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar: {ex.Message}");
            }
            return dt;
        }

        public void ListarProdutoEspecifico(int nrSeqProduto)       
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM Produto WHERE NrSeqProduto = {nrSeqProduto}";
                    dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        NrSeqProduto = Convert.ToInt32(dt.Rows[0]["NrSeqProduto"]);
                        DataCadastro = Convert.ToDateTime(dt.Rows[0]["DataCadastro"]);
                        Quantidade = Convert.ToInt32(dt.Rows[0]["Quantidade"]);
                        Marca = dt.Rows[0]["Marca"].ToString();
                        CodigoProduto = Convert.ToInt32(dt.Rows[0]["CodigoProduto"]);
                        Preco = Convert.ToDecimal(dt.Rows[0]["Preco"]);
                        NomeDoProduto = dt.Rows[0]["NomeDoProduto"].ToString();
                        Descricao = dt.Rows[0]["Descricao"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar: {ex.Message}");
            }
        }

        public void CarregarDados()
        {
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM Produto WHERE NrSeqProduto = {NrSeqProduto}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        NrSeqProduto = Convert.ToInt32(dt.Rows[0]["NrSeqProduto"]);
                        DataCadastro = Convert.ToDateTime(dt.Rows[0]["DataCadastro"]);
                        Quantidade = Convert.ToInt32(dt.Rows[0]["Quantidade"]);
                        Marca = dt.Rows[0]["Marca"].ToString();
                        CodigoProduto = Convert.ToInt32(dt.Rows[0]["CodigoProduto"]);
                        Preco = Convert.ToDecimal(dt.Rows[0]["Preco"]);
                        NomeDoProduto = dt.Rows[0]["NomeDoProduto"].ToString();
                        Descricao = dt.Rows[0]["Descricao"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar: {ex.Message}");
            }
        }
        public List<ProdutoModel> Pesquisar(ProdutoModel produtoModel)
        {
            string where = "1=1";
            if (!string.IsNullOrEmpty(produtoModel.NomeDoProduto))
            {
                where += $@" AND PRODUTO.NOMEDOPRODUTO LIKE '%{produtoModel.NomeDoProduto}%'";
            }

            if (produtoModel.Preco > 0)
            {
                where += $@" AND PRODUTO.PRECO = {produtoModel.Preco}";
            }

            if (produtoModel.CodigoProduto > 0)
            {
                where += $@" AND PRODUTO.CODIGOPRODUTO = {produtoModel.CodigoProduto}";
            }
            if (!string.IsNullOrEmpty(produtoModel.Marca))
            {
                where += $@" AND PRODUTO.MARCA LIKE '%{produtoModel.Marca}%'";
            }
            if (!string.IsNullOrEmpty(produtoModel.Descricao))
            {
                where += $@" AND PRODUTO.DESCRICAO LIKE '%{produtoModel.Descricao}%'";
            }

            if (produtoModel.Quantidade > 0)
            {
                where += $@" AND PRODUTO.QUANTIDADE = {produtoModel.Quantidade}";
            }


            if (produtoModel.deCadastro > DateTime.MinValue && produtoModel.ateCadastro > DateTime.MinValue)
            {

                string dataCadastroInicio = produtoModel.deCadastro.ToString("yyyy-MM-dd");
                string dataCadastroFinal = produtoModel.ateCadastro.ToString("yyyy-MM-dd");

                where += $@" AND PRODUTO.DATACADASTRO BETWEEN '{dataCadastroInicio}' AND '{dataCadastroFinal}'";
            }


            string query = $@"
            SELECT
                *
            FROM `Produto`
            WHERE {where}";

            List<ProdutoModel> produtos = new List<ProdutoModel>();

            try
            {
                using (DAL objDAL = new DAL())
                {
                    DataTable dt = objDAL.RetDataTable(query);

                    foreach (DataRow row in dt.Rows)
                    {
                        ProdutoModel produto = new ProdutoModel();
                        produto.NrSeqProduto = row["NrSeqProduto"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["NrSeqProduto"]);
                        produto.Quantidade = row["Quantidade"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["Quantidade"]);
                        produto.DataCadastro = row["DataCadastro"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["DataCadastro"]);
                        produto.CodigoProduto = row["CodigoProduto"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["CodigoProduto"]);
                        produto.Preco = row["Preco"] == DBNull.Value ? 0.0m : Convert.ToDecimal(row["Preco"]);
                        produto.NomeDoProduto = row["NomeDoProduto"] == DBNull.Value ? string.Empty : Convert.ToString(row["NomeDoProduto"]);
                        produto.Descricao = row["Descricao"] == DBNull.Value ? string.Empty : Convert.ToString(row["Descricao"]);
                        produto.Marca = row["Marca"] == DBNull.Value ? string.Empty : Convert.ToString(row["Marca"]);

                        produtos.Add(produto);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao pesquisar produto por nome: {ex.Message}");
            }

            return produtos;
        }
        public void Carregar()
        {
            string query = $@"
            SELECT
                *
            FROM `PRODUTO`         
            WHERE PRODUTO.NRSEQPRODUTO = {NrSeqProduto}";

            try
            {
                using (DAL objDAL = new DAL())
                {
                    DataTable dt = objDAL.RetDataTable(query);

                    if (dt.Rows.Count == 1)
                    {
                        NrSeqProduto = dt.Rows[0]["NRSEQPRODUTO"] == DBNull.Value ? int.MinValue : Convert.ToInt32(dt.Rows[0]["NRSEQPRODUTO"]);
                        Quantidade = dt.Rows[0]["QUANTIDADE"] == DBNull.Value ? int.MinValue : Convert.ToInt32(dt.Rows[0]["QUANTIDADE"]);
                        CodigoProduto = dt.Rows[0]["CODIGOPRODUTO"] == DBNull.Value ? int.MinValue : Convert.ToInt32(dt.Rows[0]["CODIGOPRODUTO"]);
                        Preco = dt.Rows[0]["PRECO"] == DBNull.Value ? int.MinValue : Convert.ToInt32(dt.Rows[0]["PRECO"]);
                        Descricao = dt.Rows[0]["DESCRICAO"] == DBNull.Value ? string.Empty : Convert.ToString(dt.Rows[0]["DESCRICAO"]);
                        NomeDoProduto = dt.Rows[0]["NOMEDOPRODUTO"] == DBNull.Value ? string.Empty : Convert.ToString(dt.Rows[0]["NOMEDOPRODUTO"]);
                        Marca = dt.Rows[0]["MARCA"] == DBNull.Value ? string.Empty : Convert.ToString(dt.Rows[0]["MARCA"]);
                        DataCadastro = dt.Rows[0]["DATACADASTRO"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["DATACADASTRO"]);
                   
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar: {ex.Message}");
            }
        }
    }
}

