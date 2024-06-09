using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Uteis;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Sige_Erp.Models
{
    public class ClienteModel
    {
        public int NrSeqPessoa { get; set; }
        public int NrSeqCliente { get; set; }

        [Required(ErrorMessage = "Informe o e-mail do cliente")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido")]
        public string EmailCliente { get; set; }

        [Required(ErrorMessage = "Informe o nome do cliente")]
        public string NomeDoCliente { get; set; }

        public int NrSeqLogin { get; set; }
        public int ObterUltimoNrSeqClienteInserido(DAL objDAL)
        {
            int ultimoNrSeqCliente = 0;

            try
            {
                string sql = "SELECT NrSeqCliente FROM Cliente ORDER BY NrSeqCliente DESC LIMIT 1";
                DataTable dt = objDAL.RetDataTable(sql);

                if (dt.Rows.Count == 1)
                {
                    ultimoNrSeqCliente = Convert.ToInt32(dt.Rows[0]["NrSeqCliente"]);
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter ultimo Cliente: {ex.Message}");
            }

            return ultimoNrSeqCliente;
        }

        public void Cadastrar(DAL objDAL)
        {
            try
            {
                // Obtém o último NrSeqCliente
                int ultimoNrSeqCliente = ObterUltimoNrSeqClienteInserido(objDAL);

                // Incrementa o NrSeqCliente
                NrSeqCliente = ultimoNrSeqCliente + 1;
                string sql = $"INSERT INTO Cliente (NrSeqPessoa, NrSeqCliente, EmailCliente, NomeDoCliente, NrSeqLogin) VALUES ({NrSeqPessoa}, {NrSeqCliente}, '{EmailCliente}', '{NomeDoCliente}', {NrSeqLogin})";
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
               
                    string sql = $"UPDATE Cliente SET EmailCliente = '{EmailCliente}', NomeDoCliente = '{NomeDoCliente}', NrSeqLogin = {NrSeqLogin} WHERE NrSeqPessoa = {NrSeqPessoa}";
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
   
                    string sql = $"DELETE FROM Cliente WHERE NrSeqCliente = {NrSeqCliente}";
                    objDAL.ExecutarComandoSQL(sql);
                
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao excluir Cliente com NrSeqCliente {NrSeqLogin}", ex);
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
                    string sql = "SELECT * FROM Cliente";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                // Trate as exceções apropriadamente (por exemplo, registre ou lance)
            }
            return dt;
        }

        public DataTable ListarClienteEspecifico(int nrSeqCliente)
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM Cliente WHERE NrSeqCliente = {nrSeqCliente}";
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
                    if (NrSeqCliente > 0)
                    {
                        where += $@" and NrSeqCliente = '{NrSeqCliente}'";
                    }
                    string sql = $"SELECT * FROM Cliente WHERE {where}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        NrSeqCliente = Convert.ToInt32(dt.Rows[0]["NrSeqCliente"]);
                        NrSeqPessoa = Convert.ToInt32(dt.Rows[0]["NrSeqPessoa"]);
                        EmailCliente = dt.Rows[0]["EmailCliente"].ToString();
                        NomeDoCliente = dt.Rows[0]["NomeDoCliente"].ToString();
                        NrSeqLogin = Convert.ToInt32(dt.Rows[0]["NrSeqLogin"]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar: {ex.Message}");
            }
        }
        public List<ClienteModel> Pesquisar(ClienteModel clienteModel)
        {
            string where = "1=1";

            string query = $@"
                SELECT
                    CLIENTE.`NRSEQPESSOA`,
                    CLIENTE.`NRSEQCLIENTE`,
                    CLIENTE.`EMAILCLIENTE`,
                    CLIENTE.`NOMEDOCLIENTE`,
                    CLIENTE.`NRSEQLOGIN`,
                    PESSOA.`NRSEQPESSOA`
                FROM `CLIENTE`
                LEFT JOIN `PESSOA` ON CLIENTE.`NRSEQPESSOA` = PESSOA.`NRSEQPESSOA`
                WHERE {where}";

            List<ClienteModel> clientes = new List<ClienteModel>();

            try
            {
                using (DAL objDAL = new DAL())
                {
                    DataTable dt = objDAL.RetDataTable(query);

                    foreach (DataRow row in dt.Rows)
                    {
                        ClienteModel cliente = new ClienteModel();
                        cliente.NrSeqPessoa = row["NRSEQPESSOA"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["NRSEQPESSOA"]);
                        cliente.NrSeqCliente = row["NRSEQCLIENTE"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["NRSEQCLIENTE"]);
                        cliente.NomeDoCliente = row["NomeDoCliente"] == DBNull.Value ? string.Empty : Convert.ToString(row["NomeDoCliente"]);
                        cliente.EmailCliente = row["EmailCliente"] == DBNull.Value ? string.Empty : Convert.ToString(row["EmailCliente"]);


                        clientes.Add(cliente);
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao pesquisar: {ex.Message}");
            }

            return clientes;
        }

    }
}
