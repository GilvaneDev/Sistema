using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Uteis;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Sige_Erp.Models
{
    public class ImpostoModel
    {
        public int NrSeqImposto { get; set; }
        public int NrSeqProduto { get; set; }
        public decimal ValorTotalImposto { get; set; }
        public decimal PorcentageImposto { get; set; }

        public int ObterUltimoNrSeqImpostoInserido(DAL objDAL)
        {
            int ultimoNrSeqImposto = 0;

            try
            {
                string sql = "SELECT NrSeqImposto FROM Impostos ORDER BY NrSeqImposto DESC LIMIT 1";
                DataTable dt = objDAL.RetDataTable(sql);

                if (dt.Rows.Count == 1)
                {
                    ultimoNrSeqImposto = Convert.ToInt32(dt.Rows[0]["NrSeqImposto"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter o último número sequencial de imposto: {ex.Message}");
            }

            return ultimoNrSeqImposto;
        }

        public void Cadastrar(DAL objDAL)
        {
            try
            {
                // Obtém o último NrSeqImposto
                int ultimoNrSeqImposto = ObterUltimoNrSeqImpostoInserido(objDAL);

                // Incrementa o NrSeqImposto
                NrSeqImposto = ultimoNrSeqImposto + 1;

                // Define a instrução SQL com parâmetros
                string sql = "INSERT INTO Impostos (NrSeqImposto, NrSeqProduto, ValorTotalImposto, PorcentageImposto) " +
                             "VALUES (@NrSeqImposto, @NrSeqProduto, @ValorTotalImposto, @PorcentageImposto)";

                // Cria e configura os parâmetros
                List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@NrSeqImposto", NrSeqImposto),
                new MySqlParameter("@NrSeqProduto", NrSeqProduto),
                new MySqlParameter("@ValorTotalImposto", ValorTotalImposto),
                new MySqlParameter("@PorcentageImposto", PorcentageImposto)
            };

                // Executa o comando SQL com parâmetros
                objDAL.ExecutarComandoSQL(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar imposto: {ex.Message}");
            }
        }

        public void Atualizar(DAL objDAL)
        {
            try
            {
                // Define a instrução SQL com parâmetros
                string sql = "UPDATE Impostos SET NrSeqProduto = @NrSeqProduto, " +
                             "ValorTotalImposto = @ValorTotalImposto, PorcentageImposto = @PorcentageImposto " +
                             "WHERE NrSeqImposto = @NrSeqImposto";

                // Cria e configura os parâmetros
                List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@NrSeqProduto", NrSeqProduto),
                new MySqlParameter("@ValorTotalImposto", ValorTotalImposto),
                new MySqlParameter("@PorcentageImposto", PorcentageImposto),
                new MySqlParameter("@NrSeqImposto", NrSeqImposto)
            };

                // Executa o comando SQL com parâmetros
                objDAL.ExecutarComandoSQL(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar imposto: {ex.Message}");
            }
        }

        public void Excluir()
        {
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"DELETE FROM Impostos WHERE NrSeqImposto = {NrSeqImposto}";
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
                    string sql = "SELECT * FROM Impostos";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                // Trate as exceções apropriadamente (por exemplo, registre ou lance)
            }
            return dt;
        }

        public DataTable ListarImpostoEspecifico(int nrSeqImposto)
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM Impostos WHERE NrSeqImposto = {nrSeqImposto}";
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
                    string sql = $"SELECT * FROM Impostos WHERE NrSeqImposto = {NrSeqImposto}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        NrSeqProduto = Convert.ToInt32(dt.Rows[0]["NrSeqProduto"]);
                        ValorTotalImposto = Convert.ToDecimal(dt.Rows[0]["ValorTotalImposto"]);
                        PorcentageImposto = Convert.ToDecimal(dt.Rows[0]["PorcentageImposto"]);
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
