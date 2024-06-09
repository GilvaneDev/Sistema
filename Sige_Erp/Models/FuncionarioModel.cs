using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Uteis;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Sige_Erp.Models
{
    public class FuncionarioModel
    {
        public decimal PorcentagemComissao { get; set; }
        public DateTime DataAdmissao { get; set; }
        public DateTime? DataDemissao { get; set; }
        public int NrSeqPessoa { get; set; }
        public int NrSeqFuncionario { get; set; }

        [Required(ErrorMessage = "Informe o cargo do funcionário")]
        public string Cargo { get; set; }

        [Required(ErrorMessage = "Informe o nome do funcionário")]
        public string NomeDoFuncionario { get; set; }

        [Required(ErrorMessage = "Informe o salário do funcionário")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O salário deve ser maior que zero")]
        public decimal Salario { get; set; }

        public int ObterUltimoNrSeqFuncionarioInserido(DAL objDAL)
        {
            int ultimoNrSeqFuncionario = 0;

            try
            {
                string sql = "SELECT NrSeqFuncionario FROM Funcionario ORDER BY NrSeqFuncionario DESC LIMIT 1";
                DataTable dt = objDAL.RetDataTable(sql);

                if (dt.Rows.Count == 1)
                {
                    ultimoNrSeqFuncionario = Convert.ToInt32(dt.Rows[0]["NrSeqFuncionario"]);
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter ultimo Funcionario: {ex.Message}");
            }

            return ultimoNrSeqFuncionario;
        }

        public void Cadastrar(DAL objDAL)
        {
            try
            {
                // Obtém o último NrSeqFuncionario
                int ultimoNrSeqFuncionario = ObterUltimoNrSeqFuncionarioInserido(objDAL);

                // Incrementa o NrSeqFuncionario
                NrSeqFuncionario = ultimoNrSeqFuncionario + 1;
                    string sql = $"INSERT INTO Funcionario (PorcentageComissao, DataAdmissao, DataDemissao, NrSeqPessoa, NrSeqFuncionario, Cargo, NomeDoFuncionario, Salario) " +
                                 $"VALUES ({PorcentagemComissao.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}, " +
                                 $"'{DataAdmissao:yyyy-MM-dd}', " +
                                 $"{(DataDemissao.HasValue ? $"'{DataDemissao.Value:yyyy-MM-dd}'" : "NULL")}, " +
                                 $"{NrSeqPessoa}, {NrSeqFuncionario}, '{Cargo}', '{NomeDoFuncionario}', {Salario.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)})";
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
              
                    string sql = $"UPDATE Funcionario SET PorcentageComissao = {PorcentagemComissao.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}, " +
                                 $"DataAdmissao = '{DataAdmissao:yyyy-MM-dd}', " +
                                 $"DataDemissao = {(DataDemissao.HasValue ? $"'{DataDemissao.Value:yyyy-MM-dd}'" : "NULL")}, " +
                                 $"NrSeqPessoa = {NrSeqPessoa}, " +
                                 $"Cargo = '{Cargo}', " +
                                 $"NomeDoFuncionario = '{NomeDoFuncionario}', " +
                                 $"Salario = {Salario.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)} " +
                                 $"WHERE NrSeqPessoa = {NrSeqPessoa}";
                    objDAL.ExecutarComandoSQL(sql);
                
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao Atualizar: {ex.Message}");
            }
        }

        public void Excluir(DAL objDAL)
        {
            try
            {
                
                    string sql = $"DELETE FROM Funcionario WHERE NrSeqFuncionario = {NrSeqFuncionario}";
                    objDAL.ExecutarComandoSQL(sql);
                
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao excluir Funcionario com NrSeqFuncionario {NrSeqFuncionario}", ex);
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
                    string sql = "SELECT * FROM Funcionario";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                // Trate as exceções apropriadamente (por exemplo, registre ou lance)
            }
            return dt;
        }

        public DataTable ListarFuncionarioEspecifico(int nrSeqFuncionario)
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM Funcionario WHERE NrSeqFuncionario = {nrSeqFuncionario}";
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
                    if (NrSeqFuncionario > 0)
                    {
                        where += $@" and NrSeqFuncionario = '{NrSeqFuncionario}'";
                    }
                    string sql = $"SELECT * FROM Funcionario WHERE {where}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        PorcentagemComissao = Convert.ToDecimal(dt.Rows[0]["PorcentageComissao"], System.Globalization.CultureInfo.InvariantCulture);
                        DataAdmissao = Convert.ToDateTime(dt.Rows[0]["DataAdmissao"]);
                        DataDemissao = dt.Rows[0]["DataDemissao"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dt.Rows[0]["DataDemissao"]) : null;
                        NrSeqPessoa = Convert.ToInt32(dt.Rows[0]["NrSeqPessoa"]);
                        NrSeqFuncionario = Convert.ToInt32(dt.Rows[0]["NrSeqFuncionario"]);
                        Cargo = dt.Rows[0]["Cargo"].ToString();
                        NomeDoFuncionario = dt.Rows[0]["NomeDoFuncionario"].ToString();
                        Salario = Convert.ToDecimal(dt.Rows[0]["Salario"], System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar: {ex.Message}");
            }
        }

        public List<FuncionarioModel> Pesquisar(FuncionarioModel funcionario)
        {
            string where = "1=1";

            if (funcionario.NrSeqPessoa > 0)
            {
                where += $" AND FUNCIONARIO.NRSEQPESSOA = {funcionario.NrSeqPessoa}";
            }
            if (!string.IsNullOrEmpty(funcionario.NomeDoFuncionario))
            {
                where += $" AND PESSOA.NOMEPESSOA LIKE '%{funcionario.NomeDoFuncionario}%'";
            }
            if (funcionario.DataAdmissao != DateTime.MinValue)
            {
                where += $" AND FUNCIONARIO.DATAADMISSAO = '{funcionario.DataAdmissao:yyyy-MM-dd}'";
            }
            if (funcionario.DataDemissao != null)
            {
                where += $" AND FUNCIONARIO.DATADEMISSAO = '{funcionario.DataDemissao:yyyy-MM-dd}'";
            }
            if (!string.IsNullOrEmpty(funcionario.Cargo))
            {
                where += $" AND FUNCIONARIO.CARGO LIKE '%{funcionario.Cargo}%'";
            }

            string query = $@"
        SELECT
            FUNCIONARIO.NRSEQPESSOA,
            FUNCIONARIO.PORCENTAGECOMISSAO,
            FUNCIONARIO.DATAADMISSAO,
            FUNCIONARIO.DATADEMISSAO,
            FUNCIONARIO.NRSEQFUNCIONARIO,
            FUNCIONARIO.CARGO,
            PESSOA.NOMEPESSOA AS NOMEDOFUNCIONARIO,
            FUNCIONARIO.SALARIO
        FROM FUNCIONARIO
        LEFT JOIN PESSOA ON FUNCIONARIO.NRSEQPESSOA = PESSOA.NRSEQPESSOA
        WHERE {where}";

            List<FuncionarioModel> funcionarios = new List<FuncionarioModel>();

            try
            {
                using (DAL objDAL = new DAL())
                {
                    DataTable dt = objDAL.RetDataTable(query);

                    foreach (DataRow row in dt.Rows)
                    {
                        FuncionarioModel func = new FuncionarioModel
                        {
                            NrSeqPessoa = row["NRSEQPESSOA"] == DBNull.Value ? 0 : Convert.ToInt32(row["NRSEQPESSOA"]),
                            PorcentagemComissao = row["PORCENTAGECOMISSAO"] == DBNull.Value ? 0 : Convert.ToDecimal(row["PORCENTAGECOMISSAO"]),
                            DataAdmissao = row["DATAADMISSAO"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["DATAADMISSAO"]),
                            DataDemissao = row["DATADEMISSAO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["DATADEMISSAO"]),
                            NrSeqFuncionario = row["NRSEQFUNCIONARIO"] == DBNull.Value ? 0 : Convert.ToInt32(row["NRSEQFUNCIONARIO"]),
                            Cargo = row["CARGO"] == DBNull.Value ? string.Empty : Convert.ToString(row["CARGO"]),
                            NomeDoFuncionario = row["NOMEDOFUNCIONARIO"] == DBNull.Value ? string.Empty : Convert.ToString(row["NOMEDOFUNCIONARIO"]),
                            Salario = row["SALARIO"] == DBNull.Value ? 0 : Convert.ToDecimal(row["SALARIO"])
                        };

                        funcionarios.Add(func);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao pesquisar: {ex.Message}");
            }

            return funcionarios;
        }

    }
}
