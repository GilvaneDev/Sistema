using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Uteis;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;

namespace Sige_Erp.Models
{
    public class FreteModel
    {
        public DateTime DataExpedicao { get; set; }
        public int NrSeqFrete { get; set; }

        [Required(ErrorMessage = "Informe o número do pedido")]
        public int NrSeqPedido { get; set; }

        [Required(ErrorMessage = "Informe o valor do frete")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor do frete deve ser maior que zero")]
        public decimal ValorFrete { get; set; }

        public int ObterUltimoNrSeqFreteInserido(DAL objDAL)
        {
            int ultimoNrSeqFrete = 0;

            try
            {
                string sql = "SELECT NrSeqFrete FROM Frete ORDER BY NrSeqFrete DESC LIMIT 1";
                DataTable dt = objDAL.RetDataTable(sql);

                if (dt.Rows.Count == 1)
                {
                    ultimoNrSeqFrete = Convert.ToInt32(dt.Rows[0]["NrSeqFrete"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter o último NrSeqFrete: {ex.Message}");
            }

            return ultimoNrSeqFrete;
        }       


        public void Cadastrar(DAL objDAL)
        {
            try
            {
                // Obtém o último NrSeqFrete
                int ultimoNrSeqFrete = ObterUltimoNrSeqFreteInserido(objDAL);

                // Incrementa o NrSeqFrete
                NrSeqFrete = ultimoNrSeqFrete + 1;

                // Define a instrução SQL com parâmetros
                string sql = "INSERT INTO Frete (DataExpedicao, NrSeqFrete, NrSeqPedido, ValorFrete) " +
                             "VALUES (@DataExpedicao, @NrSeqFrete, @NrSeqPedido, @ValorFrete)";

                // Cria e configura os parâmetros
                List<MySqlParameter> parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@DataExpedicao", DataExpedicao),
            new MySqlParameter("@NrSeqFrete", NrSeqFrete),
            new MySqlParameter("@NrSeqPedido", NrSeqPedido),
            new MySqlParameter("@ValorFrete", ValorFrete)
        };

                // Executa o comando SQL com parâmetros
                objDAL.ExecutarComandoSQL(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar frete: {ex.Message}");
            }
        }
        public void Atualizar(DAL objDAL)
        {
            try
            {
                // Define a instrução SQL com parâmetros
                string sql = "UPDATE Frete SET DataExpedicao = @DataExpedicao, NrSeqPedido = @NrSeqPedido, ValorFrete = @ValorFrete " +
                             "WHERE NrSeqFrete = @NrSeqFrete";

                // Cria e configura os parâmetros
                List<MySqlParameter> parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@DataExpedicao", DataExpedicao),
            new MySqlParameter("@NrSeqPedido", NrSeqPedido),
            new MySqlParameter("@ValorFrete", ValorFrete),
            new MySqlParameter("@NrSeqFrete", NrSeqFrete)
        };

                // Executa o comando SQL com parâmetros
                objDAL.ExecutarComandoSQL(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar frete: {ex.Message}");
            }
        }




        public void Excluir()
        {
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"DELETE FROM Frete WHERE NrSeqFrete = {NrSeqFrete}";
                    objDAL.ExecutarComandoSQL(sql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao excluir frete: {ex.Message}");
            }
        }

        public DataTable ListarTodos()
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = "SELECT * FROM Frete";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar frete: {ex.Message}");
            }
            return dt;
        }

        public DataTable ListarFreteEspecifico(int nrSeqFrete)
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM Frete WHERE NrSeqFrete = {nrSeqFrete}";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar frete: {ex.Message}");
            }
            return dt;
        }

        public void CarregarDados()
        {
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM Frete WHERE NrSeqFrete = {NrSeqFrete}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        DataExpedicao = Convert.ToDateTime(dt.Rows[0]["DataExpedicao"]);
                        NrSeqFrete = Convert.ToInt32(dt.Rows[0]["NrSeqFrete"]);
                        NrSeqPedido = Convert.ToInt32(dt.Rows[0]["NrSeqPedido"]);
                        ValorFrete = Convert.ToDecimal(dt.Rows[0]["ValorFrete"]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar frete: {ex.Message}");
            }
        }
        public void PesquisaPorNrSeqPedido()
        {
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM Frete WHERE NrSeqPedido = {NrSeqPedido}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        DataExpedicao = Convert.ToDateTime(dt.Rows[0]["DataExpedicao"]);
                        NrSeqFrete = Convert.ToInt32(dt.Rows[0]["NrSeqFrete"]);
                        NrSeqPedido = Convert.ToInt32(dt.Rows[0]["NrSeqPedido"]);
                        ValorFrete = Convert.ToDecimal(dt.Rows[0]["ValorFrete"]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar frete: {ex.Message}");
            }
        }
    }
}
