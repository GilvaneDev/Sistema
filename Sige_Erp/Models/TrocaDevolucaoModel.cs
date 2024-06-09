using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Uteis;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;

namespace Sige_Erp.Models
{
    public class TrocaDevolucaoModel
    {
        public DateTime DataTrocaDevolucao { get; set; }
        public int NrSeqTrocaDevolucao { get; set; }

        [Required(ErrorMessage = "Informe o motivo")]
        public string Motivo { get; set; }

        public int NrSeqProduto { get; set; }

        [Required(ErrorMessage = "Selecione Troca (T) ou Devolução (D)")]
        [RegularExpression("^[TD]$", ErrorMessage = "Selecione Troca (T) ou Devolução (D)")]
        public char Troca { get; set; }

        [Required(ErrorMessage = "Selecione Troca (T) ou Devolução (D)")]
        [RegularExpression("^[TD]$", ErrorMessage = "Selecione Troca (T) ou Devolução (D)")]
        public char Devolucao { get; set; }

        [Required(ErrorMessage = "Informe o nome do produto")]
        public string NomeDoProduto { get; set; }

        public int ObterUltimoNrSeqTrocaDevolucaoInserido(DAL objDAL)
        {
            int ultimoNrSeqTrocaDevolucao = 0;

            try
            {
                string sql = "SELECT NrSeqTrocaDevolucao FROM TrocaDevolucao ORDER BY NrSeqTrocaDevolucao DESC LIMIT 1";
                DataTable dt = objDAL.RetDataTable(sql);

                if (dt.Rows.Count == 1)
                {
                    ultimoNrSeqTrocaDevolucao = Convert.ToInt32(dt.Rows[0]["NrSeqTrocaDevolucao"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter o último NrSeqTrocaDevolucao: {ex.Message}");
            }

            return ultimoNrSeqTrocaDevolucao;
        }


        public void Cadastrar(DAL objDAL)
        {
            try
            {
               
                    // Obtém o último NrSeqTrocaDevolucao
                    int ultimoNrSeqTrocaDevolucao = ObterUltimoNrSeqTrocaDevolucaoInserido(objDAL);

                    // Incrementa o NrSeqTrocaDevolucao
                    NrSeqTrocaDevolucao = ultimoNrSeqTrocaDevolucao + 1;

                    // Define a instrução SQL com parâmetros
                    string sql = "INSERT INTO TrocaDevolucao (DataTrocaDevolucao, NrSeqTrocaDevolucao, Motivo, NrSeqProduto, Troca, Devolucao, NomeDoProduto) " +
                                 "VALUES (@DataTrocaDevolucao, @NrSeqTrocaDevolucao, @Motivo, @NrSeqProduto, @Troca, @Devolucao, @NomeDoProduto)";

                    // Cria e configura os parâmetros
                    List<MySqlParameter> parameters = new List<MySqlParameter>
                    {
                        new MySqlParameter("@DataTrocaDevolucao", DataTrocaDevolucao),
                        new MySqlParameter("@NrSeqTrocaDevolucao", NrSeqTrocaDevolucao),
                        new MySqlParameter("@Motivo", Motivo),
                        new MySqlParameter("@NrSeqProduto", NrSeqProduto),
                        new MySqlParameter("@Troca", Troca),
                        new MySqlParameter("@Devolucao", Devolucao),
                        new MySqlParameter("@NomeDoProduto", NomeDoProduto)
                    };

                    // Executa o comando SQL com parâmetros
                    objDAL.ExecutarComandoSQL(sql, parameters);
                
            }
            catch (Exception ex)
            {
                // Trate as exceções apropriadamente (por exemplo, registre ou lance)
                throw new Exception($"Erro ao cadastrar troca/devolução: {ex.Message}");
            }
        }


        public void Atualizar(DAL objDAL)
        {
            try
            {
                
                    // Define a instrução SQL com parâmetros
                    string sql = "UPDATE TrocaDevolucao SET DataTrocaDevolucao = @DataTrocaDevolucao, Motivo = @Motivo, " +
                                 "NrSeqProduto = @NrSeqProduto, Troca = @Troca, Devolucao = @Devolucao, NomeDoProduto = @NomeDoProduto " +
                                 "WHERE NrSeqTrocaDevolucao = @NrSeqTrocaDevolucao";

                    // Cria e configura os parâmetros
                    List<MySqlParameter> parameters = new List<MySqlParameter>
                    {
                        new MySqlParameter("@DataTrocaDevolucao", DataTrocaDevolucao),
                        new MySqlParameter("@Motivo", Motivo),
                        new MySqlParameter("@NrSeqProduto", NrSeqProduto),
                        new MySqlParameter("@Troca", Troca),
                        new MySqlParameter("@Devolucao", Devolucao),
                        new MySqlParameter("@NomeDoProduto", NomeDoProduto),
                        new MySqlParameter("@NrSeqTrocaDevolucao", NrSeqTrocaDevolucao)
                    };

                    // Executa o comando SQL com parâmetros
                    objDAL.ExecutarComandoSQL(sql, parameters);
                
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar troca/devolução: {ex.Message}");
            }
        }


        public void Excluir()
        {
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"DELETE FROM TrocaDevolucao WHERE NrSeqTrocaDevolucao = {NrSeqTrocaDevolucao}";
                    objDAL.ExecutarComandoSQL(sql);
                }
            }
            catch (Exception ex)
            {
                // Trate as exceções apropriadamente (por exemplo, registre ou lance)
            }
        }

        public DataTable ListarTodos()
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = "SELECT * FROM TrocaDevolucao";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                // Trate as exceções apropriadamente (por exemplo, registre ou lance)
            }
            return dt;
        }

        public DataTable ListarTrocaDevolucaoEspecifica(int nrSeqTrocaDevolucao)
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM TrocaDevolucao WHERE NrSeqTrocaDevolucao = {nrSeqTrocaDevolucao}";
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
                    string sql = $"SELECT * FROM TrocaDevolucao WHERE NrSeqTrocaDevolucao = {NrSeqTrocaDevolucao}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        DataTrocaDevolucao = Convert.ToDateTime(dt.Rows[0]["DataTrocaDevolucao"]);
                        Motivo = dt.Rows[0]["Motivo"].ToString();
                        NrSeqProduto = Convert.ToInt32(dt.Rows[0]["NrSeqProduto"]);
                        Troca = Convert.ToChar(dt.Rows[0]["Troca"]);
                        Devolucao = Convert.ToChar(dt.Rows[0]["Devolucao"]);
                        NomeDoProduto = dt.Rows[0]["NomeDoProduto"].ToString();
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
