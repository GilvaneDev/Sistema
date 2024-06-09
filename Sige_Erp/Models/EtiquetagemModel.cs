using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Uteis;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace Sige_Erp.Models
{
    public class EtiquetagemModel
    {
        [Required(ErrorMessage = "Selecione Embalado (E) ou Não Embalado (N)")]
        [RegularExpression("^[EN]$", ErrorMessage = "Selecione Embalado (E) ou Não Embalado (N)")]
        public char Embalado { get; set; }

        [Required(ErrorMessage = "Selecione Enviado (E) ou Não Enviado (N)")]
        [RegularExpression("^[EN]$", ErrorMessage = "Selecione Enviado (E) ou Não Enviado (N)")]
        public char Enviado { get; set; }

        public int NrSeqEtiquetagem { get; set; }
        public int NrSeqPedido { get; set; }

        [Required(ErrorMessage = "Informe o nome do produto")]
        public string NomeDoProduto { get; set; }
        public int ObterUltimoNrSeqEtiquetagemInserido(DAL objDAL)
        {
            int ultimoNrSeqEtiquetagem = 0;

            try
            {
                string sql = "SELECT NrSeqEtiquetagem FROM Etiquetagem ORDER BY NrSeqEtiquetagem DESC LIMIT 1";
                DataTable dt = objDAL.RetDataTable(sql);

                if (dt.Rows.Count == 1)
                {
                    ultimoNrSeqEtiquetagem = Convert.ToInt32(dt.Rows[0]["NrSeqEtiquetagem"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar: {ex.Message}");
            }

            return ultimoNrSeqEtiquetagem;
        }

        public void Cadastrar(DAL objDAL)
        {
            try
            {
                NrSeqEtiquetagem = ObterUltimoNrSeqEtiquetagemInserido(objDAL) + 1;


                string sql = $"INSERT INTO Etiquetagem (Embalado, Enviado, NrSeqEtiquetagem, NrSeqPedido, NomeDoProduto) " +
                                 $"VALUES ('{Embalado}', '{Enviado}', {NrSeqEtiquetagem}, {NrSeqPedido}, '{NomeDoProduto}')";
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
                
                    string sql = $"UPDATE Etiquetagem SET Embalado = '{Embalado}', Enviado = '{Enviado}', " +
                                 $"NomeDoProduto = '{NomeDoProduto}' " +
                                 $"WHERE NrSeqPedido = {NrSeqPedido}";
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
                
                    string sql = $"DELETE FROM Etiquetagem WHERE NrSeqEtiquetagem = {NrSeqEtiquetagem}";
                    objDAL.ExecutarComandoSQL(sql);
                
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
                
                    string sql = $"DELETE FROM Etiquetagem WHERE NrSeqPedido = {NrSeqPedido}";
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
                    string sql = "SELECT * FROM Etiquetagem";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar: {ex.Message}");
            }
            return dt;
        }

        public DataTable ListarEtiquetagemEspecifica(int nrSeqEtiquetagem)
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM Etiquetagem WHERE NrSeqEtiquetagem = {nrSeqEtiquetagem}";
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
                    string sql = $"SELECT * FROM Etiquetagem WHERE NrSeqEtiquetagem = {NrSeqEtiquetagem}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        Embalado = Convert.ToChar(dt.Rows[0]["Embalado"]);
                        Enviado = Convert.ToChar(dt.Rows[0]["Enviado"]);
                        NrSeqEtiquetagem = Convert.ToInt32(dt.Rows[0]["NrSeqEtiquetagem"]);
                        NrSeqPedido = Convert.ToInt32(dt.Rows[0]["NrSeqPedido"]);
                        NomeDoProduto = dt.Rows[0]["NomeDoProduto"].ToString();
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
                    string sql = $"SELECT * FROM Etiquetagem WHERE NrSeqPedido = {NrSeqPedido}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        Embalado = Convert.ToChar(dt.Rows[0]["Embalado"]);
                        Enviado = Convert.ToChar(dt.Rows[0]["Enviado"]);
                        NrSeqEtiquetagem = Convert.ToInt32(dt.Rows[0]["NrSeqEtiquetagem"]);
                        NrSeqPedido = Convert.ToInt32(dt.Rows[0]["NrSeqPedido"]);
                        NomeDoProduto = dt.Rows[0]["NomeDoProduto"].ToString();
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
