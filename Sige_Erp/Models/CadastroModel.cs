using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Uteis;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace Sige_Erp.Models
{
    public class CadastroModel
    {
        public int NrSeqCadastro { get; set; }

        [Required(ErrorMessage = "Informe o tipo Web")]
        public char Web { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DataCadastro { get; set; }

        public int ObterUltimoNrSeqCadastroInserido(DAL objDAL)
        {
            int ultimoNrSeqCadastro = 0;

            try
            {

                    string sql = "SELECT NrSeqCadastro FROM Cadastro ORDER BY NrSeqCadastro DESC LIMIT 1";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        ultimoNrSeqCadastro = Convert.ToInt32(dt.Rows[0]["NrSeqCadastro"]);
                    }
                
            }
            catch (NotSupportedException nex)
            {
                // Log ou mensagem para NotSupportedException
                Console.WriteLine($"Erro de NotSupportedException: {nex.Message}");
            }
            catch (Exception ex)
            {
                // Log ou mensagem para outras exceções
                Console.WriteLine($"Erro ao obter o último NrSeqCadastro: {ex.Message}");
            }

            return ultimoNrSeqCadastro;
        }



        public void Cadastrar(DAL objDAL)
        {
            try
            {           
                int ultimoNrSeqCadastro = ObterUltimoNrSeqCadastroInserido(objDAL);
                NrSeqCadastro = ultimoNrSeqCadastro + 1;

                string sql = $"INSERT INTO Cadastro (NrSeqCadastro, Web, DataCadastro) VALUES ({NrSeqCadastro}, '{Web}', '{DataCadastro:yyyy-MM-dd}')";
                objDAL.ExecutarComandoSQL(sql);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar: {ex.Message}");
            }
        }




        public void Atualizar()
        {
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"UPDATE Cadastro SET Web = '{Web}', DataCadastro = '{DataCadastro:yyyy-MM-dd}' WHERE NrSeqCadastro = {NrSeqCadastro}";
                    objDAL.ExecutarComandoSQL(sql);
                }
            }
            catch (Exception ex)
            {
                // Trate as exceções apropriadamente (por exemplo, registre ou lance)
            }
        }

        public void Excluir(DAL objDAL)
        {
            try
            {
                string sql = $"DELETE FROM Cadastro WHERE NrSeqCadastro = {NrSeqCadastro}";
                objDAL.ExecutarComandoSQL(sql);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao excluir cadastro com NrSeqCadastro {NrSeqCadastro}");
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
                    string sql = "SELECT * FROM Cadastro";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                // Trate as exceções apropriadamente (por exemplo, registre ou lance)
            }
            return dt;
        }

        public DataTable ListarCadastroEspecifico(int nrSeqCadastro)
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM Cadastro WHERE NrSeqCadastro = {nrSeqCadastro}";
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
                    string sql = $"SELECT * FROM Cadastro WHERE NrSeqCadastro = {NrSeqCadastro}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        Web = Convert.ToChar(dt.Rows[0]["Web"]);
                        DataCadastro = Convert.ToDateTime(dt.Rows[0]["DataCadastro"]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar dados do Cadastro: {ex.Message}");
            }
        }
    }
}
