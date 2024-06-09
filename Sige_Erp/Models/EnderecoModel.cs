using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Uteis;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace Sige_Erp.Models
{
    public class EnderecoModel
    {
        public string Complemento { get; set; }
        public string Cidade { get; set; }
        public string Pais { get; set; }
        public string Bairro { get; set; }
        public string Estado { get; set; }
        public int Numero { get; set; }
        public int NrSeqPessoa { get; set; }
        public int NrSeqEndereco { get; set; }
        public string Rua { get; set; }
        public string Avenida { get; set; }

        public int ObterUltimoNrSeqEnderecoInserido(DAL objDAL)
        {
            int ultimoNrSeqEndereco = 0;

            try
            {
                string sql = "SELECT NrSeqEndereco FROM Endereco ORDER BY NrSeqEndereco DESC LIMIT 1";
                DataTable dt = objDAL.RetDataTable(sql);

                if (dt.Rows.Count == 1)
                {
                    ultimoNrSeqEndereco = Convert.ToInt32(dt.Rows[0]["NrSeqEndereco"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar: {ex.Message}");
            }

            return ultimoNrSeqEndereco;
        }

        public void Cadastrar(DAL objDAL)
        {
            try
            {
                // Obtém o último NrSeqEndereco
                int ultimoNrSeqEndereco = ObterUltimoNrSeqEnderecoInserido(objDAL);

                // Incrementa o NrSeqEndereco
                NrSeqEndereco = ultimoNrSeqEndereco + 1;
                string sql = $"INSERT INTO Endereco (Complemento, Cidade, Pais, Bairro, Estado, Numero, NrSeqPessoa, NrSeqEndereco, Rua, Avenida) " +
                                 $"VALUES ('{Complemento}', '{Cidade}', '{Pais}', '{Bairro}', '{Estado}', {Numero}, {NrSeqPessoa}, {NrSeqEndereco}, '{Rua}', '{Avenida}')";
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
             
                    string sql = $"UPDATE Endereco SET Complemento = '{Complemento}', Cidade = '{Cidade}', Pais = '{Pais}', Bairro = '{Bairro}', " +
                                 $"Estado = '{Estado}', Numero = {Numero}, Rua = '{Rua}', Avenida = '{Avenida}' WHERE NrSeqPessoa = {NrSeqPessoa}";
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
                string sql = $"DELETE FROM Endereco WHERE NrSeqEndereco = {NrSeqEndereco}";
                objDAL.ExecutarComandoSQL(sql);

            }
            catch (Exception ex)
            {

                throw new InvalidOperationException($"Erro ao excluir endereço com NrSeqEndereco {NrSeqEndereco}", ex);
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
                    string sql = "SELECT * FROM Endereco";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                // Trate as exceções apropriadamente (por exemplo, registre ou lance)
            }
            return dt;
        }

        public DataTable ListarEnderecoEspecifico(int nrSeqEndereco)
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM Endereco WHERE NrSeqEndereco = {nrSeqEndereco}";
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
                    if (NrSeqEndereco > 0)
                    {
                        where += $@" and NrSeqEndereco = '{NrSeqEndereco}'";
                    }
                    string sql = $"SELECT * FROM Endereco WHERE {where}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        NrSeqEndereco = Convert.ToInt32(dt.Rows[0]["NrSeqEndereco"]);
                        Complemento = dt.Rows[0]["Complemento"].ToString();
                        Cidade = dt.Rows[0]["Cidade"].ToString();
                        Pais = dt.Rows[0]["Pais"].ToString();
                        Bairro = dt.Rows[0]["Bairro"].ToString();
                        Estado = dt.Rows[0]["Estado"].ToString();
                        Numero = Convert.ToInt32(dt.Rows[0]["Numero"]);
                        NrSeqPessoa = Convert.ToInt32(dt.Rows[0]["NrSeqPessoa"]);
                        Rua = dt.Rows[0]["Rua"].ToString();
                        Avenida = dt.Rows[0]["Avenida"].ToString();
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
