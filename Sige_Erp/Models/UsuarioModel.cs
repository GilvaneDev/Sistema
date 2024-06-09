using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Uteis;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace Sige_Erp.Models
{
    public class UsuarioModel
    {
        public int NrSeqPessoa { get; set; }
        public int NrSeqUsuario { get; set; }

        [Required(ErrorMessage = "Informe o tipo de usuário")]
        public string TipoUsuario { get; set; }

        [Required(ErrorMessage = "Informe o nome do usuário")]
        public string NomeDoUsuario { get; set; }

        public int ObterUltimoNrSeqUsuarioInserido(DAL objDAL)
        {
            int ultimoNrSeqUsuario = 0;

            try
            {
                    string sql = "SELECT NrSeqUsuario FROM Usuario ORDER BY NrSeqUsuario DESC LIMIT 1";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        ultimoNrSeqUsuario = Convert.ToInt32(dt.Rows[0]["NrSeqUsuario"]);
                    }

            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter ultimo Usuario: {ex.Message}");
            }

            return ultimoNrSeqUsuario;
        }

        public void Cadastrar(DAL objDAL)
        {
            try
            {
                // Obtém o último NrSeqUsuario
                int ultimoNrSeqUsuario = ObterUltimoNrSeqUsuarioInserido(objDAL);

                // Incrementa o NrSeqUsuario
                NrSeqUsuario = ultimoNrSeqUsuario + 1;
                    string sql = $"INSERT INTO Usuario (NrSeqPessoa, NrSeqUsuario, TipoUsuario, NomeDoUsuario) " +
                                 $"VALUES ({NrSeqPessoa}, {NrSeqUsuario}, '{TipoUsuario}', '{NomeDoUsuario}')";
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
                
                    string sql = $"UPDATE Usuario SET  TipoUsuario = '{TipoUsuario}', NomeDoUsuario = '{NomeDoUsuario}' " +
                                 $"WHERE NrSeqPessoa = {NrSeqPessoa}";
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
                string sql = $"DELETE FROM Usuario WHERE NrSeqUsuario = {NrSeqUsuario}";
                objDAL.ExecutarComandoSQL(sql);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao excluir usuário com NrSeqUsuario {NrSeqUsuario}", ex);
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
                    string sql = "SELECT * FROM Usuario";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar: {ex.Message}");
            }
            return dt;
        }

        public DataTable ListarUsuarioEspecifico(int nrSeqUsuario)
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM Usuario WHERE NrSeqUsuario = {nrSeqUsuario}";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar: {ex.Message}");
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
                    if(NrSeqPessoa > 0)
                    {
                        where += $@" and NrSeqPessoa = '{NrSeqPessoa}'";
                    }
                    if (NrSeqUsuario > 0)
                    {
                        where += $@" and NrSeqUsuario = '{NrSeqUsuario}'";
                    }
                    string sql = $"SELECT * FROM Usuario WHERE {where}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        NrSeqPessoa = Convert.ToInt32(dt.Rows[0]["NrSeqPessoa"]);
                        NrSeqUsuario = Convert.ToInt32(dt.Rows[0]["NrSeqUsuario"]);
                        TipoUsuario = dt.Rows[0]["TipoUsuario"].ToString();
                        NomeDoUsuario = dt.Rows[0]["NomeDoUsuario"].ToString();
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
