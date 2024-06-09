using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Uteis;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace Sige_Erp.Models
{
    public class StatusPedidoModel
    {
        [Required(ErrorMessage = "Selecione Entregue (E) ou Não Entregue (N)")]
        [RegularExpression("^[EN]$", ErrorMessage = "Selecione Entregue (E) ou Não Entregue (N)")]
        public char Entregue { get; set; }

        [Required(ErrorMessage = "Selecione Enviado (E) ou Não Enviado (N)")]
        [RegularExpression("^[EN]$", ErrorMessage = "Selecione Enviado (E) ou Não Enviado (N)")]
        public char Enviado { get; set; }

        public int NrSeqStatusPedido { get; set; }
        public int NrSeqEtiquetagem { get; set; }

        [Required(ErrorMessage = "Informe o número do pedido")]
        public int NrSeqPedido { get; set; }

        [Required(ErrorMessage = "Selecione Aprovado (A) ou Não Aprovado (N)")]
        [RegularExpression("^[AN]$", ErrorMessage = "Selecione Aprovado (A) ou Não Aprovado (N)")]
        public char Aprovado { get; set; }
        public int ObterUltimoNrSeqStatusPedidoInserido(DAL objDAL)
        {
            int ultimoNrSeqStatusPedido = 0;

            try
            {
                string sql = "SELECT NrSeqStatusPedido FROM StatusPedido ORDER BY NrSeqStatusPedido DESC LIMIT 1";
                DataTable dt = objDAL.RetDataTable(sql);

                if (dt.Rows.Count == 1)
                {
                    ultimoNrSeqStatusPedido = Convert.ToInt32(dt.Rows[0]["NrSeqStatusPedido"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar: {ex.Message}");
            }

            return ultimoNrSeqStatusPedido;
        }

        public void Cadastrar(DAL objDAL)
        {
            try
            {
                NrSeqStatusPedido = ObterUltimoNrSeqStatusPedidoInserido(objDAL) + 1;

                    string sql = $"INSERT INTO StatusPedido (Entregue, Enviado, NrSeqStatusPedido, NrSeqEtiquetagem, NrSeqPedido, Aprovado) " +
                                 $"VALUES ('{Entregue}', '{Enviado}', {NrSeqStatusPedido}, {NrSeqEtiquetagem}, {NrSeqPedido}, '{Aprovado}')";
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
                    string sql = $"UPDATE StatusPedido SET Entregue = '{Entregue}', Enviado = '{Enviado}', " +
                                 $"NrSeqEtiquetagem = {NrSeqEtiquetagem},  Aprovado = '{Aprovado}' " +
                                 $"WHERE NrSeqPedido = {NrSeqPedido}";
                    objDAL.ExecutarComandoSQL(sql);
                
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar: {ex.Message}");
            }
        }

        public void Excluir()
        {
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"DELETE FROM StatusPedido WHERE NrSeqStatusPedido = {NrSeqStatusPedido}";
                    objDAL.ExecutarComandoSQL(sql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao excluir: {ex.Message}");
            }
        }
        public void ExcluirPorPedido(DAL objDAL)
        {
            try
            {
                
                    string sql = $"DELETE FROM StatusPedido WHERE NrSeqPedido = {NrSeqPedido}";
                    objDAL.ExecutarComandoSQL(sql);
                
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao excluir: {ex.Message}");
            }
        }

        public DataTable ListarTodos()
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = "SELECT * FROM StatusPedido";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar: {ex.Message}");
            }
            return dt;
        }

        public DataTable ListarStatusPedidoEspecifico(int nrSeqStatusPedido)
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM StatusPedido WHERE NrSeqStatusPedido = {nrSeqStatusPedido}";
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
                    string sql = $"SELECT * FROM StatusPedido WHERE NrSeqStatusPedido = {NrSeqStatusPedido}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        Entregue = Convert.ToChar(dt.Rows[0]["Entregue"]);
                        Enviado = Convert.ToChar(dt.Rows[0]["Enviado"]);
                        NrSeqStatusPedido = Convert.ToInt32(dt.Rows[0]["NrSeqStatusPedido"]);
                        NrSeqEtiquetagem = Convert.ToInt32(dt.Rows[0]["NrSeqEtiquetagem"]);
                        NrSeqPedido = Convert.ToInt32(dt.Rows[0]["NrSeqPedido"]);
                        Aprovado = Convert.ToChar(dt.Rows[0]["Aprovado"]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar: {ex.Message}");
            }
        }
        public void CarregarDadosPorPedido()
        {
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM StatusPedido WHERE NrSeqPedido = {NrSeqPedido}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        Entregue = Convert.ToChar(dt.Rows[0]["Entregue"]);
                        Enviado = Convert.ToChar(dt.Rows[0]["Enviado"]);
                        NrSeqStatusPedido = Convert.ToInt32(dt.Rows[0]["NrSeqStatusPedido"]);
                        NrSeqEtiquetagem = Convert.ToInt32(dt.Rows[0]["NrSeqEtiquetagem"]);
                        NrSeqPedido = Convert.ToInt32(dt.Rows[0]["NrSeqPedido"]);
                        Aprovado = Convert.ToChar(dt.Rows[0]["Aprovado"]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar: {ex.Message}");
            }
        }
    }
}
