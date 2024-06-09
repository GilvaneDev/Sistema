using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Uteis;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using MySqlX.XDevAPI;

namespace Sige_Erp.Models
{
    public class EstoqueModel
    {
        public DateTime DataCadastro { get; set; }
        public DateTime? DTSaida { get; set; }
        public DateTime? DTEntrada { get; set; }
        public DateTime? deCadastro { get; set; }
        public DateTime? ateCadastro { get; set; }
        public int NrSeqFornecedor { get; set; }
        public int NrSeqProduto { get; set; }
        public int NrSeqPedido { get; set; }
        public int NrSeqEstoque { get; set; }
        public int Quantidade { get; set; }
        public string NomeDoFornecedor { get; set; }
        public Decimal PrecoUnitario { get; set; }

        public string NomeDoProduto { get; set; }
        public string Pessoa { get; set; }
        public string Valor { get; set; }
        public string Marca { get; set; }
        public bool CheckboxMovimento { get; set; }
        public bool CheckboxEntrada { get; set; }
        public bool CheckboxSaida { get; set; }
        public bool AbaAtiva { get; set; }
        public bool FlgEdicao { get; set; }
        public string PdfBase64 { get; set; }
        public string TipoMovimento { get; set; }

        public List<EstoqueModel> ListaEstoques { get; set; }
        public List<Int32> IdsSelecionados { get; set; }

        public int ObterUltimoNrSeqEstoqueInserido(DAL objDAL)
        {
            int ultimoNrSeqEstoque = 0;

            try
            {
                string sql = "SELECT NrSeqEstoque FROM Estoque ORDER BY NrSeqEstoque DESC LIMIT 1";
                DataTable dt = objDAL.RetDataTable(sql);

                if (dt.Rows.Count == 1)
                {
                    ultimoNrSeqEstoque = Convert.ToInt32(dt.Rows[0]["NrSeqEstoque"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar: {ex.Message}");
            }

            return ultimoNrSeqEstoque;
        }

        public void Cadastrar(DAL objDAL)
        {
            try
            {
                // Obtém o último NrSeqEstoque
                int ultimoNrSeqEstoque = ObterUltimoNrSeqEstoqueInserido(objDAL);

                // Incrementa o NrSeqEstoque
                NrSeqEstoque = ultimoNrSeqEstoque + 1;
                string sql = $"INSERT INTO Estoque (DataCadastro, NrSeqFornecedor, NrSeqEstoque, Quantidade, NomeDoFornecedor, NomeDoProduto, NrSeqPedido) " +
                                 $"VALUES ('{DataCadastro:yyyy-MM-dd}', {NrSeqFornecedor}, {NrSeqEstoque}, {Quantidade}, '{NomeDoFornecedor}', '{NomeDoProduto}', '{NrSeqPedido}')";
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
                    string sql = $"UPDATE Estoque SET DataCadastro = '{DataCadastro:yyyy-MM-dd}', NrSeqPedido = '{NrSeqPedido}', " +
                                 $"NrSeqFornecedor = {NrSeqFornecedor}, Quantidade = {Quantidade}, " +
                                 $"NomeDoFornecedor = '{NomeDoFornecedor}', NomeDoProduto = '{NomeDoProduto}' " +
                                 $"WHERE NrSeqEstoque = {NrSeqEstoque}";
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
 
                    string sql = $"DELETE FROM Estoque WHERE NrSeqEstoque = {NrSeqEstoque}";
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
                    string sql = "SELECT * FROM Estoque";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar: {ex.Message}");
            }
            return dt;
        }

        public DataTable ListarEstoqueEspecifico(int nrSeqEstoque)
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM Estoque WHERE NrSeqEstoque = {nrSeqEstoque}";
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
                    string sql = $"SELECT * FROM Estoque WHERE NrSeqEstoque = {NrSeqEstoque}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        DataCadastro = Convert.ToDateTime(dt.Rows[0]["DataCadastro"]);
                        NrSeqEstoque = Convert.ToInt32(dt.Rows[0]["NrSeqEstoque"]);
                        NrSeqFornecedor = Convert.ToInt32(dt.Rows[0]["NrSeqFornecedor"]);
                        Quantidade = Convert.ToInt32(dt.Rows[0]["Quantidade"]);
                        NomeDoFornecedor = dt.Rows[0]["NomeDoFornecedor"].ToString();
                        NomeDoProduto = dt.Rows[0]["NomeDoProduto"].ToString();
                        NrSeqPedido = Convert.ToInt32(dt.Rows[0]["NrSeqPedido"]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar: {ex.Message}");
            }
        }
        public void Pesquisar()
        {
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string where = "1  = 1";

                    string sql = string.Empty;

                    if(NrSeqEstoque > 0)
                    {
                        where += $@" AND NrSeqEstoque = {NrSeqEstoque}";
                    }
                    if (!string.IsNullOrEmpty(NomeDoProduto))
                    {
                        where += $@" AND NomeDoProduto like '%{NomeDoProduto}%'";
                    }
                    sql = $"SELECT * FROM Estoque WHERE {where}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        DataCadastro = Convert.ToDateTime(dt.Rows[0]["DataCadastro"]);
                        NrSeqEstoque = Convert.ToInt32(dt.Rows[0]["NrSeqEstoque"]);
                        NrSeqFornecedor = Convert.ToInt32(dt.Rows[0]["NrSeqFornecedor"]);
                        Quantidade = Convert.ToInt32(dt.Rows[0]["Quantidade"]);
                        NomeDoFornecedor = dt.Rows[0]["NomeDoFornecedor"].ToString();
                        NomeDoProduto = dt.Rows[0]["NomeDoProduto"].ToString();
                        NomeDoProduto = dt.Rows[0]["NomeDoProduto"].ToString();
                        NrSeqPedido = Convert.ToInt32(dt.Rows[0]["NrSeqPedido"]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar: {ex.Message}");
            }
        }
        public List<EstoqueModel> Pesquisar(EstoqueModel estoqueModel)
        {
            string where = "1=1";
            string joins = string.Empty;
            string selects = string.Empty;

                   
            
                             
            if (estoqueModel.CheckboxMovimento)
            {
                selects += $@", MOVIMENTACAO.`NOMEPESSOA`,
                    MOVIMENTACAO.`NOMEDOPRODUTO`,
                    MOVIMENTACAO.`VALORPAGO`,
                    MOVIMENTACAO.`MARCA`,
                    MOVIMENTACAO.`ENTRADA`,
                    MOVIMENTACAO.`SAIDA`";

                if (!string.IsNullOrEmpty(estoqueModel.NomeDoProduto))
                {
                    
                    where += $@" AND MOVIMENTACAO.NOMEDOPRODUTO LIKE '%{estoqueModel.NomeDoProduto}%'";
                }
                if (!string.IsNullOrEmpty(estoqueModel.NomeDoFornecedor))
                {
                    
                    where += $@" AND MOVIMENTACAO.NOMEPESSOA LIKE '%{estoqueModel.NomeDoFornecedor}%'";
                }
                if (!string.IsNullOrEmpty(estoqueModel.Marca))
                {

                    where += $@" AND MOVIMENTACAO.MARCA LIKE '%{estoqueModel.Marca}%'";
                }
                if (estoqueModel.CheckboxEntrada && estoqueModel.CheckboxSaida == false)
                {
                    if (estoqueModel.deCadastro != null && estoqueModel.ateCadastro != null)
                    {
 
                        string dataCadastroInicio = estoqueModel.deCadastro.Value.ToString("yyyy-MM-dd");
                        string dataCadastroFinal = estoqueModel.ateCadastro.Value.ToString("yyyy-MM-dd");

                        where += $@" AND MOVIMENTACAO.DATAENTRADA BETWEEN '{dataCadastroInicio}' AND '{dataCadastroFinal}'";

                    }
                    where += $@" AND MOVIMENTACAO.ENTRADA = 'S'";
                    joins += @"INNER JOIN PEDIDO ON MOVIMENTACAO.NRSEQPEDIDO = PEDIDO.NRSEQPEDIDO";
                }
                else if (estoqueModel.CheckboxSaida && estoqueModel.CheckboxEntrada == false)
                {
                    if (estoqueModel.deCadastro != null && estoqueModel.ateCadastro != null)
                    {
                        string dataCadastroInicio = estoqueModel.deCadastro.Value.ToString("yyyy-MM-dd");
                        string dataCadastroFinal = estoqueModel.ateCadastro.Value.ToString("yyyy-MM-dd");

                        where += $@" AND MOVIMENTACAO.DATASAIDA BETWEEN '{dataCadastroInicio}' AND '{dataCadastroFinal}'";
                    }
                    where += $@" AND MOVIMENTACAO.SAIDA = 'S'";
                }
                else
                {
                    if (estoqueModel.deCadastro > DateTime.MinValue && estoqueModel.ateCadastro != DateTime.MinValue)
                    {                     
                        string dataCadastroInicio = estoqueModel.deCadastro.Value.ToString("yyyy-MM-dd");
                        string dataCadastroFinal = estoqueModel.ateCadastro.Value.ToString("yyyy-MM-dd");

                        where += $@" AND MOVIMENTACAO.DATAENTRADA BETWEEN '{dataCadastroInicio}' AND '{dataCadastroFinal}'";

                        where += $@" AND MOVIMENTACAO.DATASAIDA BETWEEN '{dataCadastroInicio}' AND '{dataCadastroFinal}'";

                    }                   
                }
                
            }
            else
            {
                selects += $@",ESTOQUE.`NOMEDOFORNECEDOR`,ESTOQUE.`NOMEDOPRODUTO`,PRODUTO.`PRECO`,PRODUTO.`MARCA`";
                if (estoqueModel.deCadastro != null && estoqueModel.ateCadastro != null)
                {
                    
                    string dataCadastroInicio = estoqueModel.deCadastro.Value.ToString("yyyy-MM-dd");
                    string dataCadastroFinal = estoqueModel.ateCadastro.Value.ToString("yyyy-MM-dd");

                    where += $@" AND ESTOQUE.DATACADASTRO BETWEEN '{dataCadastroInicio}' AND '{dataCadastroFinal}'";
                }
                if (estoqueModel.Quantidade > 0)
                {
                    
                    where += $@" AND ESTOQUE.QUANTIDADE ={estoqueModel.Quantidade}";
                }
                if (estoqueModel.NrSeqEstoque > 0)
                {

                    where += $@" AND ESTOQUE.NRSEQESTOQUE ={estoqueModel.NrSeqEstoque}";
                }

                if (!string.IsNullOrEmpty(estoqueModel.NomeDoProduto))
                {
                    where += $@" AND ESTOQUE.NOMEDOPRODUTO LIKE '%{estoqueModel.NomeDoProduto}%'";
                }
                if (!string.IsNullOrEmpty(estoqueModel.NomeDoFornecedor))
                {
                    
                    where += $@" AND ESTOQUE.NOMEDOFORNECEDOR LIKE '%{estoqueModel.NomeDoFornecedor}%'";
                }
                if (!string.IsNullOrEmpty(estoqueModel.Marca))
                {
                   
                    where += $@" AND PRODUTO.MARCA LIKE '%{estoqueModel.Marca}%'";
                }
                joins += @"INNER JOIN PRODUTO ON ESTOQUE.NOMEDOPRODUTO = PRODUTO.NOMEDOPRODUTO";
            }

            string query = string.Empty;

            if (estoqueModel.CheckboxMovimento) 
            {
                if (estoqueModel.CheckboxEntrada && estoqueModel.CheckboxSaida == false)
                {
                    query = $@"
                SELECT
                    *
                    
                        
                    
                FROM `MOVIMENTACAO`                                        
                {joins} 
                WHERE {where}";
                }
                else if (estoqueModel.CheckboxSaida && estoqueModel.CheckboxEntrada == false)
                {
                    query = $@"
                SELECT
                   *
                    
                        
                    
                FROM `MOVIMENTACAO`
                INNER JOIN PEDIDO ON MOVIMENTACAO.NRSEQPEDIDO = PEDIDO.NRSEQPEDIDO                            
                {joins} 
                WHERE {where}";
                }
                else
                {
                    query = $@"
                SELECT
                   *    
                FROM `MOVIMENTACAO`
                INNER JOIN PEDIDO ON MOVIMENTACAO.NRSEQPEDIDO = PEDIDO.NRSEQPEDIDO                            
                {joins} 
                WHERE {where}";
                }
            } 
            else
            {
                query = $@"
                SELECT
                    ESTOQUE.`NRSEQFORNECEDOR`,
                    ESTOQUE.`QUANTIDADE`,
                    ESTOQUE.`NRSEQESTOQUE`,
                    ESTOQUE.`DATACADASTRO`,
                    ESTOQUE.`NRSEQPEDIDO`
                    {selects}
                    
                        
                    
                FROM `ESTOQUE`
                INNER JOIN PEDIDO ON ESTOQUE.NRSEQPEDIDO = PEDIDO.NRSEQPEDIDO                            
                {joins} 
                WHERE {where}";
            }

            

            List<EstoqueModel> estoques = new List<EstoqueModel>();

            try
            {
                using (DAL objDAL = new DAL())
                {
                    DataTable dt = objDAL.RetDataTable(query);

                    foreach (DataRow row in dt.Rows)
                    {
                        EstoqueModel estoque = new EstoqueModel();

                        if (estoqueModel.CheckboxMovimento)
                        {
                            if (estoqueModel.CheckboxEntrada && estoqueModel.CheckboxSaida == false)
                            {
                                
                                estoque.Quantidade = row["QUANTIDADE"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["QUANTIDADE"]);
                                
                                estoque.NrSeqPedido = row["NrSeqPedido"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["NrSeqPedido"]);
                                estoque.Pessoa = row["NOMEPESSOA"] == DBNull.Value ? string.Empty : Convert.ToString(row["NOMEPESSOA"]);
                                estoque.NomeDoProduto = row["NOMEDOPRODUTO"] == DBNull.Value ? string.Empty : Convert.ToString(row["NOMEDOPRODUTO"]);
                                estoque.DataCadastro = row["DATACADASTRO"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["DATACADASTRO"]);

                                MovimentacaoModel movimentacao = new MovimentacaoModel();
                                movimentacao.NrSeqPedido = estoque.NrSeqPedido;
                                movimentacao.PesquisarPorNrSeqPedido();

                                estoque.PrecoUnitario = movimentacao.ValorPago;
                                estoque.Marca = movimentacao.Marca;
                                if (movimentacao.Saida == 'S')
                                {
                                    estoque.TipoMovimento = "Saida";
                                }
                                else if (movimentacao.Entrada == 'S')
                                {
                                    estoque.TipoMovimento = "Entrada";
                                }
                                else
                                {
                                    estoque.TipoMovimento = "Entrada";
                                }

                            }
                            else if (estoqueModel.CheckboxSaida && estoqueModel.CheckboxEntrada == false)
                            {
                                
                                estoque.Quantidade = row["QUANTIDADE"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["QUANTIDADE"]);
                                
                                estoque.NrSeqPedido = row["NrSeqPedido"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["NrSeqPedido"]);
                                estoque.Pessoa = row["NOMEPESSOA"] == DBNull.Value ? string.Empty : Convert.ToString(row["NOMEPESSOA"]);
                                estoque.NomeDoProduto = row["NOMEDOPRODUTO"] == DBNull.Value ? string.Empty : Convert.ToString(row["NOMEDOPRODUTO"]);
                                estoque.DataCadastro = row["DATASAIDA"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["DATASAIDA"]);

                                MovimentacaoModel movimentacao = new MovimentacaoModel();
                                movimentacao.NrSeqPedido = estoque.NrSeqPedido;
                                movimentacao.PesquisarPorNrSeqPedido();

                                estoque.PrecoUnitario = movimentacao.ValorPago;
                                estoque.Marca = movimentacao.Marca;
                                if (movimentacao.Saida == 'S')
                                {
                                    estoque.TipoMovimento = "Saida";
                                }
                                else if (movimentacao.Entrada == 'S')
                                {
                                    estoque.TipoMovimento = "Entrada";
                                }
                                else
                                {
                                    estoque.TipoMovimento = "Entrada";
                                }

                            }
                            else
                            {
                                
                                estoque.Quantidade = row["QUANTIDADE"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["QUANTIDADE"]);
                               
                                estoque.NrSeqPedido = row["NrSeqPedido"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["NrSeqPedido"]);
                                estoque.Pessoa = row["NOMEPESSOA"] == DBNull.Value ? string.Empty : Convert.ToString(row["NOMEPESSOA"]);
                                estoque.NomeDoProduto = row["NOMEDOPRODUTO"] == DBNull.Value ? string.Empty : Convert.ToString(row["NOMEDOPRODUTO"]);
                                

                                MovimentacaoModel movimentacao = new MovimentacaoModel();
                                movimentacao.NrSeqPedido = estoque.NrSeqPedido;
                                movimentacao.PesquisarPorNrSeqPedido();

                                estoque.PrecoUnitario = movimentacao.ValorPago;
                                estoque.Marca = movimentacao.Marca;
                                if (movimentacao.Saida == 'S')
                                {
                                    estoque.TipoMovimento = "Saida";
                                    estoque.DataCadastro = movimentacao.DataSaida;
                                }
                                else if (movimentacao.Entrada == 'S')
                                {
                                    estoque.TipoMovimento = "Entrada";
                                    estoque.DataCadastro = movimentacao.DataEntrada;
                                }
                                else
                                {
                                    estoque.TipoMovimento = "Entrada";
                                    estoque.DataCadastro = movimentacao.DataEntrada;
                                }

                            }
                            
                        }
                        else
                        {
                            estoque.NrSeqFornecedor = row["NRSEQFORNECEDOR"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["NRSEQFORNECEDOR"]);
                            estoque.Quantidade = row["QUANTIDADE"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["QUANTIDADE"]);
                            estoque.NrSeqEstoque = row["NrSeqEstoque"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["NrSeqEstoque"]);
                            estoque.NrSeqPedido = row["NrSeqPedido"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["NrSeqPedido"]);
                            estoque.Pessoa = row["NOMEDOFORNECEDOR"] == DBNull.Value ? string.Empty : Convert.ToString(row["NOMEDOFORNECEDOR"]);
                            estoque.NomeDoProduto = row["NOMEDOPRODUTO"] == DBNull.Value ? string.Empty : Convert.ToString(row["NOMEDOPRODUTO"]);
                            estoque.DataCadastro = row["DATACADASTRO"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["DATACADASTRO"]);
                            estoque.PrecoUnitario = row["PRECO"] == DBNull.Value ? Decimal.MinValue : Convert.ToDecimal(row["PRECO"]);
                            estoque.Marca = row["MARCA"] == DBNull.Value ? string.Empty : Convert.ToString(row["MARCA"]);

                            if(estoque.NrSeqPedido > 0)
                            {
                                PedidoModel pedido = new PedidoModel();
                                pedido.NrSeqPedido = estoque.NrSeqPedido;
                                pedido.Carregar();

                                if(pedido.TipoUsuario == "Fornecedor")
                                {
                                    estoque.TipoMovimento = "Entrada";
                                }
                                else
                                {
                                    estoque.TipoMovimento = "Saida";
                                }
                            }
                        }
                      
                      
                      
                        
                         
                        
                       
                      

                        

                        estoques.Add(estoque);
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao pesquisar: {ex.Message}");
            }

            return estoques;
        }

        public void Pesquisa(EstoqueModel estoqueModel)
        {
            string where = "1=1";
            string joins = string.Empty;
            string selects = string.Empty;

            // Definir selects e condições com base em CheckboxMovimento
            if (estoqueModel.CheckboxMovimento)
            {
                selects += @" MOVIMENTACAO.`NOMEPESSOA`,
                      MOVIMENTACAO.`NOMEDOPRODUTO`,
                      MOVIMENTACAO.`VALORPAGO`,
                      MOVIMENTACAO.`MARCA`,
                      MOVIMENTACAO.`ENTRADA`,
                      MOVIMENTACAO.`SAIDA`";

                if (!string.IsNullOrEmpty(estoqueModel.NomeDoProduto))
                    where += $" AND MOVIMENTACAO.NOMEDOPRODUTO LIKE '%{estoqueModel.NomeDoProduto}%'";

                if (estoqueModel.NrSeqPedido > 0)
                    where += $" AND MOVIMENTACAO.NRSEQPEDIDO = {estoqueModel.NrSeqPedido}";

                if (!string.IsNullOrEmpty(estoqueModel.NomeDoFornecedor))
                    where += $" AND MOVIMENTACAO.NOMEPESSOA LIKE '%{estoqueModel.NomeDoFornecedor}%'";

                if (!string.IsNullOrEmpty(estoqueModel.Marca))
                    where += $" AND MOVIMENTACAO.MARCA LIKE '%{estoqueModel.Marca}%'";

                if (estoqueModel.CheckboxEntrada && !estoqueModel.CheckboxSaida)
                {
                    if (estoqueModel.deCadastro != null && estoqueModel.ateCadastro != null)
                    {
                        string dataInicio = estoqueModel.deCadastro.Value.ToString("yyyy-MM-dd");
                        string dataFinal = estoqueModel.ateCadastro.Value.ToString("yyyy-MM-dd");
                        where += $" AND MOVIMENTACAO.DATAENTRADA BETWEEN '{dataInicio}' AND '{dataFinal}'";
                    }
                    where += " AND MOVIMENTACAO.ENTRADA = 'S'";
                    joins += " INNER JOIN PEDIDO ON MOVIMENTACAO.NRSEQPEDIDO = PEDIDO.NRSEQPEDIDO";
                }
                else if (estoqueModel.CheckboxSaida && !estoqueModel.CheckboxEntrada)
                {
                    if (estoqueModel.deCadastro != null && estoqueModel.ateCadastro != null)
                    {
                        string dataInicio = estoqueModel.deCadastro.Value.ToString("yyyy-MM-dd");
                        string dataFinal = estoqueModel.ateCadastro.Value.ToString("yyyy-MM-dd");
                        where += $" AND MOVIMENTACAO.DATASAIDA BETWEEN '{dataInicio}' AND '{dataFinal}'";
                    }
                    where += " AND MOVIMENTACAO.SAIDA = 'S'";
                }
                else if (estoqueModel.deCadastro != null && estoqueModel.ateCadastro != null)
                {
                    string dataInicio = estoqueModel.deCadastro.Value.ToString("yyyy-MM-dd");
                    string dataFinal = estoqueModel.ateCadastro.Value.ToString("yyyy-MM-dd");
                    where += $" AND (MOVIMENTACAO.DATAENTRADA BETWEEN '{dataInicio}' AND '{dataFinal}' OR MOVIMENTACAO.DATASAIDA BETWEEN '{dataInicio}' AND '{dataFinal}')";
                }
            }
            else
            {
                selects += @" ESTOQUE.`NOMEDOFORNECEDOR`,
                      ESTOQUE.`NOMEDOPRODUTO`,
                      PRODUTO.`PRECO`,
                      PRODUTO.`MARCA`";

                if (estoqueModel.deCadastro != null && estoqueModel.ateCadastro != null)
                {
                    string dataInicio = estoqueModel.deCadastro.Value.ToString("yyyy-MM-dd");
                    string dataFinal = estoqueModel.ateCadastro.Value.ToString("yyyy-MM-dd");
                    where += $" AND ESTOQUE.DATACADASTRO BETWEEN '{dataInicio}' AND '{dataFinal}'";
                }

                if (estoqueModel.Quantidade > 0)
                    where += $" AND ESTOQUE.QUANTIDADE = {estoqueModel.Quantidade}";

                if (estoqueModel.NrSeqEstoque > 0)
                    where += $" AND ESTOQUE.NRSEQESTOQUE = {estoqueModel.NrSeqEstoque}";

                if (!string.IsNullOrEmpty(estoqueModel.NomeDoProduto))
                    where += $" AND ESTOQUE.NOMEDOPRODUTO LIKE '%{estoqueModel.NomeDoProduto}%'";

                if (!string.IsNullOrEmpty(estoqueModel.NomeDoFornecedor))
                    where += $" AND ESTOQUE.NOMEDOFORNECEDOR LIKE '%{estoqueModel.NomeDoFornecedor}%'";

                if (!string.IsNullOrEmpty(estoqueModel.Marca))
                    where += $" AND PRODUTO.MARCA LIKE '%{estoqueModel.Marca}%'";

                joins += " LEFT JOIN PRODUTO ON ESTOQUE.NOMEDOPRODUTO = PRODUTO.NOMEDOPRODUTO";
            }

            // Construir a consulta
            string query = estoqueModel.CheckboxMovimento ?
        $@"SELECT * FROM `MOVIMENTACAO` {joins} WHERE {where} LIMIT 1" :
        $@"SELECT ESTOQUE.`NRSEQFORNECEDOR`,
                   ESTOQUE.`QUANTIDADE`,
                   ESTOQUE.`NRSEQESTOQUE`,
                   ESTOQUE.`DATACADASTRO`,
                   ESTOQUE.`NRSEQPEDIDO`
                   {selects}
           FROM `ESTOQUE`
           LEFT JOIN PEDIDO ON ESTOQUE.NRSEQPEDIDO = PEDIDO.NRSEQPEDIDO
           {joins}
           WHERE {where} LIMIT 1";


            // Execução da consulta e mapeamento dos resultados
            try
            {
                using (DAL objDAL = new DAL())
                {
                    DataTable dt = objDAL.RetDataTable(query);
                    EstoqueModel estoque = new EstoqueModel();

                    foreach (DataRow row in dt.Rows)
                    {
                        if (estoqueModel.CheckboxMovimento)
                        {
                            estoque = MapearMovimentacao(row, estoqueModel);
                        }
                        else
                        {
                            estoque = MapearEstoque(row, estoqueModel);
                        }
                    }

                    Marca = !string.IsNullOrEmpty(estoque.Marca) ? estoque.Marca : string.Empty;
                    Pessoa = !string.IsNullOrEmpty(estoque.Pessoa) ? estoque.Pessoa : string.Empty;
                    TipoMovimento = !string.IsNullOrEmpty(estoque.TipoMovimento) ? estoque.TipoMovimento : string.Empty;
                    NomeDoProduto = !string.IsNullOrEmpty(estoque.NomeDoProduto) ? estoque.NomeDoProduto : string.Empty;
                    Quantidade = estoque.Quantidade;
                    PrecoUnitario = estoque.PrecoUnitario;                          
                    DataCadastro = estoque.DataCadastro;
                    NrSeqPedido = estoque.NrSeqPedido;
                    
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao pesquisar: {ex.Message}");
            }
        }

        // Métodos de mapeamento para Movimentacao e Estoque
        private EstoqueModel MapearMovimentacao(DataRow row, EstoqueModel estoqueModel)
        {
            EstoqueModel estoque = new EstoqueModel();
            estoque.Quantidade = row["QUANTIDADE"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["QUANTIDADE"]);
            estoque.NrSeqPedido = row["NrSeqPedido"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["NrSeqPedido"]);
            estoque.Pessoa = row["NOMEPESSOA"] == DBNull.Value ? string.Empty : Convert.ToString(row["NOMEPESSOA"]);
            estoque.NomeDoProduto = row["NOMEDOPRODUTO"] == DBNull.Value ? string.Empty : Convert.ToString(row["NOMEDOPRODUTO"]);

            MovimentacaoModel movimentacao = new MovimentacaoModel();
            movimentacao.NrSeqPedido = estoque.NrSeqPedido;
            movimentacao.PesquisarPorNrSeqPedido();

            estoque.PrecoUnitario = movimentacao.ValorPago;
            estoque.Marca = movimentacao.Marca;
            estoque.TipoMovimento = movimentacao.Saida == 'S' ? "Saida" : "Entrada";
            estoque.DataCadastro = movimentacao.Saida == 'S' ? movimentacao.DataSaida : movimentacao.DataEntrada;

            return estoque;
        }

        private EstoqueModel MapearEstoque(DataRow row, EstoqueModel estoqueModel)
        {
            EstoqueModel estoque = new EstoqueModel();
            estoque.NrSeqFornecedor = row["NRSEQFORNECEDOR"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["NRSEQFORNECEDOR"]);
            estoque.Quantidade = row["QUANTIDADE"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["QUANTIDADE"]);
            estoque.NrSeqEstoque = row["NrSeqEstoque"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["NrSeqEstoque"]);
            estoque.NrSeqPedido = row["NrSeqPedido"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["NrSeqPedido"]);
            estoque.Pessoa = row["NOMEDOFORNECEDOR"] == DBNull.Value ? string.Empty : Convert.ToString(row["NOMEDOFORNECEDOR"]);
            estoque.NomeDoProduto = row["NOMEDOPRODUTO"] == DBNull.Value ? string.Empty : Convert.ToString(row["NOMEDOPRODUTO"]);
            estoque.DataCadastro = row["DATACADASTRO"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["DATACADASTRO"]);
            estoque.PrecoUnitario = row["PRECO"] == DBNull.Value ? Decimal.MinValue : Convert.ToDecimal(row["PRECO"]);
            estoque.Marca = row["MARCA"] == DBNull.Value ? string.Empty : Convert.ToString(row["MARCA"]);

            if (estoque.NrSeqPedido > 0)
            {
                PedidoModel pedido = new PedidoModel();
                pedido.NrSeqPedido = estoque.NrSeqPedido;
                pedido.Carregar();
                estoque.TipoMovimento = pedido.TipoUsuario == "Fornecedor" ? "Entrada" : "Saida";
            }

            return estoque;
        }

        public void Carregar()
        {
            string query = $@"
    SELECT
        ESTOQUE.`NRSEQFORNECEDOR`,
        ESTOQUE.`QUANTIDADE`,
        ESTOQUE.`NRSEQESTOQUE`,
        ESTOQUE.`DATACADASTRO`,
        ESTOQUE.`NOMEDOFORNECEDOR`,
        ESTOQUE.`NOMEDOPRODUTO`,
        ESTOQUE.`NRSEQPEDIDO`,
        PRODUTO.`PRECO`,
        PRODUTO.`MARCA`
    FROM `ESTOQUE`
    LEFT JOIN `PRODUTO` ON ESTOQUE.`NOMEDOPRODUTO` = PRODUTO.`NOMEDOPRODUTO`
    LEFT JOIN `FORNECEDOR` ON ESTOQUE.`NRSEQFORNECEDOR` = FORNECEDOR.`NRSEQFORNECEDOR`
    WHERE ESTOQUE.`NRSEQESTOQUE` = {NrSeqEstoque}";

            try
            {
                using (DAL objDAL = new DAL())
                {
                    DataTable dt = objDAL.RetDataTable(query);

                    if (dt.Rows.Count == 1)
                    {
                        Marca = dt.Rows[0]["MARCA"] == DBNull.Value ? string.Empty : Convert.ToString(dt.Rows[0]["MARCA"]);
                        NrSeqFornecedor = dt.Rows[0]["NRSEQFORNECEDOR"] == DBNull.Value ? int.MinValue : Convert.ToInt32(dt.Rows[0]["NRSEQFORNECEDOR"]);
                        Quantidade = dt.Rows[0]["QUANTIDADE"] == DBNull.Value ? int.MinValue : Convert.ToInt32(dt.Rows[0]["QUANTIDADE"]);
                        NrSeqEstoque = dt.Rows[0]["NRSEQESTOQUE"] == DBNull.Value ? int.MinValue : Convert.ToInt32(dt.Rows[0]["NRSEQESTOQUE"]);
                        PrecoUnitario = dt.Rows[0]["PRECO"] == DBNull.Value ? decimal.MinValue : Convert.ToDecimal(dt.Rows[0]["PRECO"]);
                        NomeDoFornecedor = dt.Rows[0]["NOMEDOFORNECEDOR"] == DBNull.Value ? string.Empty : Convert.ToString(dt.Rows[0]["NOMEDOFORNECEDOR"]);
                        NomeDoProduto = dt.Rows[0]["NOMEDOPRODUTO"] == DBNull.Value ? string.Empty : Convert.ToString(dt.Rows[0]["NOMEDOPRODUTO"]);
                        DataCadastro = dt.Rows[0]["DATACADASTRO"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["DATACADASTRO"]);
                        NrSeqPedido = dt.Rows[0]["NRSEQPEDIDO"] == DBNull.Value ? int.MinValue : Convert.ToInt32(dt.Rows[0]["NRSEQPEDIDO"]);
                        TipoMovimento = "Entrada";

                        if(NrSeqPedido > 0)
                        {
                            PedidoModel pedido = new PedidoModel();
                            pedido.NrSeqPedido = NrSeqPedido;
                            pedido.CarregarDados();

                            NrSeqProduto = pedido.NrSeqProduto;
                        }
                       

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
