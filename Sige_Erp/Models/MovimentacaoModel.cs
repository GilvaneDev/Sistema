using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Uteis;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Sige_Erp.Models
{
    public class MovimentacaoModel
    {
        public DateTime DataSaida { get; set; }
        public string Marca { get; set; }

        [Required(ErrorMessage = "Informe o nome do produto")]
        public string NomeDoProduto { get; set; }

        [Required(ErrorMessage = "Informe a quantidade")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "Informe o nome da pessoa")]
        public string NomePessoa { get; set; }

        [Required(ErrorMessage = "Informe o valor pago")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor pago deve ser maior que zero")]
        public decimal ValorPago { get; set; }

        public int NrSeqMovimentacao { get; set; }
        public DateTime DataEntrada { get; set; }

        [Required(ErrorMessage = "Selecione Saída (S) ou Entrada (E)")]
        [RegularExpression("^[SE]$", ErrorMessage = "Selecione Saída (S) ou Entrada (E)")]
        public char Saida { get; set; }

        [Required(ErrorMessage = "Selecione Saída (S) ou Entrada (E)")]
        [RegularExpression("^[SE]$", ErrorMessage = "Selecione Saída (S) ou Entrada (E)")]
        public char Entrada { get; set; }

        public int NrSeqPedido { get; set; }
        public int NrSeqProduto { get; set; }

        public int ObterUltimoNrSeqMovimentacaoInserido(DAL objDAL)
        {
            int ultimoNrSeqMovimentacao = 0;

            try
            {
                string sql = "SELECT NrSeqMovimentacao FROM Movimentacao ORDER BY NrSeqMovimentacao DESC LIMIT 1";
                DataTable dt = objDAL.RetDataTable(sql);

                if (dt.Rows.Count == 1)
                {
                    ultimoNrSeqMovimentacao = Convert.ToInt32(dt.Rows[0]["NrSeqMovimentacao"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter o último número de sequência da movimentação: {ex.Message}");
            }

            return ultimoNrSeqMovimentacao;
        }

        public void Cadastrar(DAL objDAL)
        {
            try
            {
                // Obtém o último NrSeqMovimentacao
                int ultimoNrSeqMovimentacao = ObterUltimoNrSeqMovimentacaoInserido(objDAL);

                // Incrementa o NrSeqMovimentacao
                NrSeqMovimentacao = ultimoNrSeqMovimentacao + 1;

                // Monta a string SQL usando parâmetros para evitar SQL Injection
                string sql = "INSERT INTO Movimentacao (DataSaida, Marca, NomeDoProduto, Quantidade, NomePessoa, " +
                             "ValorPago, NrSeqMovimentacao, DataEntrada, Saida, Entrada, NrSeqPedido, NrSeqProduto) " +
                             "VALUES (@DataSaida, @Marca, @NomeDoProduto, @Quantidade, @NomePessoa, " +
                             "@ValorPago, @NrSeqMovimentacao, @DataEntrada, @Saida, @Entrada, @NrSeqPedido, @NrSeqProduto)";

                // Cria e configura os parâmetros
                List<MySqlParameter> parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@DataSaida", DataSaida.ToString("yyyy-MM-dd")),
            new MySqlParameter("@Marca", Marca),
            new MySqlParameter("@NomeDoProduto", NomeDoProduto),
            new MySqlParameter("@Quantidade", Quantidade),
            new MySqlParameter("@NomePessoa", NomePessoa),
            new MySqlParameter("@ValorPago", ValorPago),
            new MySqlParameter("@NrSeqMovimentacao", NrSeqMovimentacao),
            new MySqlParameter("@DataEntrada", DataEntrada.ToString("yyyy-MM-dd")),
            new MySqlParameter("@Saida", Saida),
            new MySqlParameter("@Entrada", Entrada),
            new MySqlParameter("@NrSeqPedido", NrSeqPedido),
            new MySqlParameter("@NrSeqProduto", NrSeqProduto)
        };

                // Executa o comando SQL com parâmetros
                objDAL.ExecutarComandoSQL(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar movimentação: {ex.Message}");
            }
        }


        public void Atualizar()
        {
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"UPDATE Movimentacao SET DataSaida = '{DataSaida:yyyy-MM-dd}', Marca = '{Marca}', " +
                                 $"NomeDoProduto = '{NomeDoProduto}', Quantidade = {Quantidade}, NomePessoa = '{NomePessoa}', " +
                                 $"ValorPago = {ValorPago}, DataEntrada = '{DataEntrada:yyyy-MM-dd}', Saida = '{Saida}', " +
                                 $"Entrada = '{Entrada}', NrSeqPedido = {NrSeqPedido}, NrSeqProduto = {NrSeqProduto} " +
                                 $"WHERE NrSeqMovimentacao = {NrSeqMovimentacao}";
                    objDAL.ExecutarComandoSQL(sql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar movimentação: {ex.Message}");
            }
        }

        public void Excluir(DAL objDAL)
        {
            try
            {
               
                    string whereClause = "";

                    if (NrSeqMovimentacao > 0)
                    {
                        whereClause = $"NrSeqMovimentacao = {NrSeqMovimentacao}";
                    }
                    else if (NrSeqPedido > 0)
                    {
                        whereClause = $"NrSeqPedido = {NrSeqPedido}";
                    }
                    else
                    {
                        throw new Exception("Número de sequência de movimentação ou pedido não especificado para exclusão.");
                    }

                    string sql = $"DELETE FROM Movimentacao WHERE {whereClause}";
                    objDAL.ExecutarComandoSQL(sql);
                
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao excluir movimentação: {ex.Message}");
            }
        }


        public DataTable ListarTodos()
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = "SELECT * FROM Movimentacao";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar movimentação: {ex.Message}");
            }
            return dt;
        }

        public DataTable ListarMovimentacaoEspecifica(int nrSeqMovimentacao)
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM Movimentacao WHERE NrSeqMovimentacao = {nrSeqMovimentacao}";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar movimentação: {ex.Message}");
            }
            return dt;
        }
        public void PesquisarPorNrSeqPedido()
        {
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM Movimentacao WHERE NrSeqPedido = {NrSeqPedido}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count >= 1) // Verifica se há pelo menos uma linha retornada
                    {
                        DataSaida = Convert.ToDateTime(dt.Rows[0]["DataSaida"]);
                        Marca = dt.Rows[0]["Marca"].ToString();
                        NomeDoProduto = dt.Rows[0]["NomeDoProduto"].ToString();
                        Quantidade = Convert.ToInt32(dt.Rows[0]["Quantidade"]);
                        NomePessoa = dt.Rows[0]["NomePessoa"].ToString();
                        ValorPago = Convert.ToDecimal(dt.Rows[0]["ValorPago"]);
                        NrSeqMovimentacao = Convert.ToInt32(dt.Rows[0]["NrSeqMovimentacao"]);
                        DataEntrada = Convert.ToDateTime(dt.Rows[0]["DataEntrada"]);
                        Saida = Convert.ToChar(dt.Rows[0]["Saida"]);
                        Entrada = Convert.ToChar(dt.Rows[0]["Entrada"]);
                        NrSeqPedido = Convert.ToInt32(dt.Rows[0]["NrSeqPedido"]);
                        NrSeqProduto = Convert.ToInt32(dt.Rows[0]["NrSeqProduto"]);
                    }
                    else
                    {
                        // Caso não haja resultados, limpar os campos ou tomar outra ação adequada
                        // Exemplo: Limpar todos os campos para indicar que não houve resultados encontrados
                        LimparCampos();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar movimentação: {ex.Message}");
            }
        }

        private void LimparCampos()
        {
            // Define todos os campos como vazios ou valores padrão para indicar que não houve resultados encontrados
            DataSaida = DateTime.MinValue;
            Marca = string.Empty;
            NomeDoProduto = string.Empty;
            Quantidade = 0;
            NomePessoa = string.Empty;
            ValorPago = 0;
            NrSeqMovimentacao = 0;
            DataEntrada = DateTime.MinValue;
            Saida = '\0';
            Entrada = '\0';
            NrSeqPedido = 0;
            NrSeqProduto = 0;
        }


        public void CarregarDados()
        {
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM Movimentacao WHERE NrSeqMovimentacao = {NrSeqMovimentacao}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        DataSaida = Convert.ToDateTime(dt.Rows[0]["DataSaida"]);
                        Marca = dt.Rows[0]["Marca"].ToString();
                        NomeDoProduto = dt.Rows[0]["NomeDoProduto"].ToString();
                        Quantidade = Convert.ToInt32(dt.Rows[0]["Quantidade"]);
                        NomePessoa = dt.Rows[0]["NomePessoa"].ToString();
                        ValorPago = Convert.ToDecimal(dt.Rows[0]["ValorPago"]);
                        NrSeqMovimentacao = Convert.ToInt32(dt.Rows[0]["NrSeqMovimentacao"]);
                        DataEntrada = Convert.ToDateTime(dt.Rows[0]["DataEntrada"]);
                        Saida = Convert.ToChar(dt.Rows[0]["Saida"]);
                        Entrada = Convert.ToChar(dt.Rows[0]["Entrada"]);
                        NrSeqPedido = Convert.ToInt32(dt.Rows[0]["NrSeqPedido"]);
                        NrSeqProduto = Convert.ToInt32(dt.Rows[0]["NrSeqProduto"]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar movimentação: {ex.Message}");
            }
        }
    }
}
