using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Uteis;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace Sige_Erp.Models
{
    public class AtenderClienteModel
    {
        public int NrSeqAtendimento { get; set; }
        public int NrSeqFuncionario { get; set; }
        public string NomeDoFuncionario { get; set; }

        public void Cadastrar(DAL objDAL)
        {
            try
            {
                
                    string sql = $"INSERT INTO AtenderCliente (NrSeqAtendimento, NrSeqFuncionario, NomeDoFuncionario) " +
                                 $"VALUES ({NrSeqAtendimento}, {NrSeqFuncionario}, '{NomeDoFuncionario}')";
                    objDAL.ExecutarComandoSQL(sql);
                
            }
            catch (Exception ex)
            {
                // Trate as exceções apropriadamente (por exemplo, registre ou lance)
            }
        }

        public void Atualizar()
        {
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"UPDATE AtenderCliente SET NrSeqFuncionario = {NrSeqFuncionario}, NomeDoFuncionario = '{NomeDoFuncionario}' " +
                                 $"WHERE NrSeqAtendimento = {NrSeqAtendimento}";
                    objDAL.ExecutarComandoSQL(sql);
                }
            }
            catch (Exception ex)
            {
                // Trate as exceções apropriadamente (por exemplo, registre ou lance)
            }
        }

        public void Excluir()
        {
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"DELETE FROM AtenderCliente WHERE NrSeqAtendimento = {NrSeqAtendimento}";
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
                    string sql = "SELECT * FROM AtenderCliente";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                // Trate as exceções apropriadamente (por exemplo, registre ou lance)
            }
            return dt;
        }

        public DataTable ListarAtendimentoEspecifico(int nrSeqAtendimento)
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM AtenderCliente WHERE NrSeqAtendimento = {nrSeqAtendimento}";
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
                    string sql = $"SELECT * FROM AtenderCliente WHERE NrSeqAtendimento = {NrSeqAtendimento}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        NrSeqFuncionario = Convert.ToInt32(dt.Rows[0]["NrSeqFuncionario"]);
                        NomeDoFuncionario = dt.Rows[0]["NomeDoFuncionario"].ToString();
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
