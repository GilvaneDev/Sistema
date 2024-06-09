using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Uteis;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Sige_Erp.Models
{
    public class NFModel
    {
        public int NrSeqPedido { get; set; }
        public int NrSeqNf { get; set; }
        public int NrSeqImpostos { get; set; }
        public int NrSeqPessoa { get; set; }
        public int NrSeqProduto { get; set; }
        public int NrSeqFornecedor { get; set; }
        public decimal ValorTotal { get; set; }
        public string NomeDoFornecedor { get; set; }
        public decimal ValorImpostos { get; set; }
        public int Quantidade { get; set; }
        public string NomeDoProduto { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? deCadastro { get; set; }
        public DateTime? ateCadastro { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime? deVencimento { get; set; }
        public DateTime? ateVencimento { get; set; }
        public string Serie { get; set; }
        public string NrNf { get; set; }
        public string ChaveNf { get; set; }
        public decimal ValorFrete { get; set; }
        public string EnderecoFornecedor { get; set; }
        public string CnpjFornecedor { get; set; }
        
        public List<NFModel> ListaNotas { get; set; }
        public List<Int32> IdsSelecionados { get; set; }
        public bool AbaAtiva { get; set; }
        public bool FlgEdicao { get; set; }
        public string PdfBase64 { get; set; }

        private int ObterUltimoNrSeqNfInserido(DAL objDAL)
        {
            int ultimoNrSeqNf = 0;

            try
            {
                string sql = "SELECT NrSeqNf FROM NF ORDER BY NrSeqNf DESC LIMIT 1";
                DataTable dt = objDAL.RetDataTable(sql);

                if (dt.Rows.Count == 1)
                {
                    ultimoNrSeqNf = Convert.ToInt32(dt.Rows[0]["NrSeqNf"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter o último número sequencial de NF: {ex.Message}");
            }

            return ultimoNrSeqNf;
        }

        public void Cadastrar(DAL objDAL)
        {
            try
            {
                // Obtém o último NrSeqNf
                int ultimoNrSeqNf = ObterUltimoNrSeqNfInserido(objDAL);

                // Incrementa o NrSeqNf
                NrSeqNf = ultimoNrSeqNf + 1;

                // Define a instrução SQL com parâmetros
                string sql = "INSERT INTO NF (NrSeqPedido, NrSeqNf, NrSeqImpostos, NrSeqPessoa, NrSeqFornecedor, ValorTotal, " +
                             "NomeDoFornecedor, ValorImpostos, Quantidade, NomeDoProduto, DataVencimento, DataCadastro, Serie, " +
                             "NrNf, ChaveNf) " +
                             "VALUES (@NrSeqPedido, @NrSeqNf, @NrSeqImpostos, @NrSeqPessoa, @NrSeqFornecedor, @ValorTotal, " +
                             "@NomeDoFornecedor, @ValorImpostos, @Quantidade, @NomeDoProduto, @DataVencimento, " +
                             "@DataCadastro, @Serie, @NrNf, @ChaveNf)";

                // Cria e configura os parâmetros
                List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@NrSeqPedido", NrSeqPedido),
                new MySqlParameter("@NrSeqNf", NrSeqNf),
                new MySqlParameter("@NrSeqImpostos", NrSeqImpostos),
                new MySqlParameter("@NrSeqPessoa", NrSeqPessoa),
                new MySqlParameter("@NrSeqFornecedor", NrSeqFornecedor),
                new MySqlParameter("@ValorTotal", ValorTotal),
                new MySqlParameter("@NomeDoFornecedor", NomeDoFornecedor),
                new MySqlParameter("@ValorImpostos", ValorImpostos),
                new MySqlParameter("@Quantidade", Quantidade),
                new MySqlParameter("@NomeDoProduto", NomeDoProduto),
                new MySqlParameter("@DataVencimento", DataVencimento.ToString("yyyy-MM-dd")),
                new MySqlParameter("@DataCadastro", DataCadastro.ToString("yyyy-MM-dd")),
                new MySqlParameter("@Serie", Serie),
                new MySqlParameter("@NrNf", NrNf),
                new MySqlParameter("@ChaveNf", ChaveNf)
            };

                // Executa o comando SQL com parâmetros
                objDAL.ExecutarComandoSQL(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar NF: {ex.Message}");
            }
        }

        public void Atualizar(DAL objDAL)
        {
            try
            {
                // Define a instrução SQL com parâmetros
                string sql = "UPDATE NF SET NrSeqPedido = @NrSeqPedido, NrSeqImpostos = @NrSeqImpostos, NrSeqPessoa = @NrSeqPessoa, " +
                             "NrSeqFornecedor = @NrSeqFornecedor, ValorTotal = @ValorTotal, NomeDoFornecedor = @NomeDoFornecedor, " +
                             "ValorImpostos = @ValorImpostos, Quantidade = @Quantidade, NomeDoProduto = @NomeDoProduto, " +
                             "DataVencimento = @DataVencimento, DataCadastro = @DataCadastro, Serie = @Serie, " +
                             "NrNf = @NrNf, ChaveNf = @ChaveNf " +
                             "WHERE NrSeqNf = @NrSeqNf";

                // Cria e configura os parâmetros
                List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@NrSeqPedido", NrSeqPedido),
                new MySqlParameter("@NrSeqImpostos", NrSeqImpostos),
                new MySqlParameter("@NrSeqPessoa", NrSeqPessoa),
                new MySqlParameter("@NrSeqFornecedor", NrSeqFornecedor),
                new MySqlParameter("@ValorTotal", ValorTotal),
                new MySqlParameter("@NomeDoFornecedor", NomeDoFornecedor),
                new MySqlParameter("@ValorImpostos", ValorImpostos),
                new MySqlParameter("@Quantidade", Quantidade),
                new MySqlParameter("@NomeDoProduto", NomeDoProduto),
                new MySqlParameter("@DataVencimento", DataVencimento.ToString("yyyy-MM-dd")),
                new MySqlParameter("@DataCadastro", DataCadastro.ToString("yyyy-MM-dd")),
                new MySqlParameter("@Serie", Serie),
                new MySqlParameter("@NrNf", NrNf),
                new MySqlParameter("@ChaveNf", ChaveNf),
                new MySqlParameter("@NrSeqNf", NrSeqNf)
            };

                // Executa o comando SQL com parâmetros
                objDAL.ExecutarComandoSQL(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar NF: {ex.Message}");
            }
        }

        public void Excluir(DAL objDAL)
        {
            try
            {
              
                    string sql = $"DELETE FROM NF WHERE NrSeqNf = {NrSeqNf}";
                    objDAL.ExecutarComandoSQL(sql);
                
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
                    string sql = "SELECT * FROM NF";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                // Trate as exceções apropriadamente (por exemplo, registre ou lance)
            }
            return dt;
        }

        public List<NFModel> Pesquisar(NFModel nota)
        {
            string where = "1=1";

            if (!string.IsNullOrEmpty(nota.NomeDoProduto))
            {
                where += $@" AND NF.NOMEDOPRODUTO LIKE '%{nota.NomeDoProduto}%'";
            }
            if (!string.IsNullOrEmpty(nota.NomeDoFornecedor))
            {
                where += $@" AND NF.NOMEDOFORNECEDOR LIKE '%{nota.NomeDoFornecedor}%'";
            }
            if (nota.ValorTotal> 0)
            {
                where += $@" AND NF.VALORTOTAL ={nota.ValorTotal}";
            }
            if (nota.ValorImpostos > 0)
            {
                where += $@" AND NF.VALORIMPOSTOS ={nota.ValorImpostos}";
            }

            if (!string.IsNullOrEmpty(nota.NrNf))
            {
                where += $@" AND NF.NRNF LIKE '%{nota.NrNf}%'";
            }
            if (nota.NrSeqPedido > 0)
            {
                where += $@" AND NF.NRSEQPEDIDO = {nota.NrSeqPedido}";
            }
            if (nota.Quantidade > 0)
            {
                where += $@" AND NF.QUANTIDADE = {nota.Quantidade}";
            }
            if (!string.IsNullOrEmpty(nota.Serie))
            {
                where += $@" AND NF.SERIE LIKE '%{nota.Serie}%'";
            }
            if (!string.IsNullOrEmpty(nota.ChaveNf))
            {
                where += $@" AND NF.CHAVENF LIKE '%{nota.ChaveNf}%'";
            }
            if (nota.deCadastro.HasValue && nota.ateCadastro.HasValue &&
                nota.deCadastro.Value > DateTime.MinValue && nota.ateCadastro.Value > DateTime.MinValue)
            {
                string dataCadastroInicio = nota.deCadastro.Value.ToString("yyyy-MM-dd");
                string dataCadastroFinal = nota.ateCadastro.Value.ToString("yyyy-MM-dd");

                where += $@" AND NF.DATACADASTRO BETWEEN '{dataCadastroInicio}' AND '{dataCadastroFinal}'";
            }

            if (nota.deVencimento.HasValue && nota.ateVencimento.HasValue &&
                nota.deVencimento.Value > DateTime.MinValue && nota.ateVencimento.Value > DateTime.MinValue)
            {
                string dataVencimentoInicio = nota.deVencimento.Value.ToString("yyyy-MM-dd");
                string dataVencimentoFinal = nota.ateVencimento.Value.ToString("yyyy-MM-dd");

                where += $@" AND NF.DATAVENCIMENTO BETWEEN '{dataVencimentoInicio}' AND '{dataVencimentoFinal}'";
            }


            string query = $@"
                SELECT
                    *
                    
                FROM `NF`
                WHERE {where}";

            List<NFModel> notas = new List<NFModel>();

            try
            {
                using (DAL objDAL = new DAL())
                {
                    DataTable dt = objDAL.RetDataTable(query);

                    foreach (DataRow row in dt.Rows)
                    {
                        NFModel notaf = new NFModel();
                        notaf.NomeDoFornecedor = row["NomeDoFornecedor"] == DBNull.Value ? string.Empty : Convert.ToString(row["NomeDoFornecedor"]);                       
                        notaf.ValorTotal = row["ValorTotal"] == DBNull.Value ? 0 : Convert.ToDecimal(row["ValorTotal"]);
                        notaf.NrSeqNf = row["NrSeqNf"] == DBNull.Value ? 0 : Convert.ToInt32(row["NrSeqNf"]);
                        notaf.NrSeqPedido = row["NrSeqNf"] == DBNull.Value ? 0 : Convert.ToInt32(row["NrSeqNf"]);
                        notaf.NrSeqImpostos = row["NrSeqImpostos"] == DBNull.Value ? 0 : Convert.ToInt32(row["NrSeqImpostos"]);
                        notaf.NrSeqPessoa = row["NrSeqPessoa"] == DBNull.Value ? 0 : Convert.ToInt32(row["NrSeqPessoa"]);
                        notaf.NrSeqFornecedor = row["NrSeqFornecedor"] == DBNull.Value ? 0 : Convert.ToInt32(row["NrSeqFornecedor"]);
                        notaf.Quantidade = row["Quantidade"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["Quantidade"]);
                        notaf.DataCadastro = row["DataCadastro"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["DataCadastro"]);
                        notaf.DataVencimento = row["DataVencimento"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["DataVencimento"]);
                        notaf.Serie = row["Serie"] == DBNull.Value ? string.Empty : Convert.ToString(row["Serie"]);
                        notaf.NomeDoProduto = row["NomeDoProduto"] == DBNull.Value ? string.Empty : Convert.ToString(row["NomeDoProduto"]);
                        notaf.ValorImpostos = row["ValorImpostos"] == DBNull.Value ? 0 : Convert.ToDecimal(row["ValorImpostos"]);
                        notaf.NrNf = row["NrNf"] == DBNull.Value ? string.Empty : Convert.ToString(row["NrNf"]);
                        notaf.ChaveNf = row["ChaveNf"] == DBNull.Value ? string.Empty : Convert.ToString(row["ChaveNf"]);

                        if(notaf.NrSeqPedido > 0)
                        {
                            PedidoModel pedido = new PedidoModel();
                            pedido.NrSeqPedido = notaf.NrSeqPedido;
                            pedido.CarregarDados();

                            notaf.NrSeqProduto = pedido.NrSeqProduto;
                            notaf.CnpjFornecedor = pedido.CpfCnpj;

                            FreteModel frete = new FreteModel();
                            frete.NrSeqPedido = notaf.NrSeqPedido;
                            frete.PesquisaPorNrSeqPedido();

                            if(frete.NrSeqFrete > 0)
                            {
                                notaf.ValorFrete = frete.ValorFrete;
                            }
                            else
                            {
                                notaf.ValorFrete = 0;
                            }   
                        }
                        if(notaf.NrSeqPessoa > 0)
                        {
                            EnderecoModel endereco = new EnderecoModel();
                            endereco.NrSeqPessoa= notaf.NrSeqPessoa;
                            endereco.CarregarDados();
                            notaf.EnderecoFornecedor = endereco.Rua;



                        }
                        notas.Add(notaf);
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao pesquisar: {ex.Message}");
            }

            return notas;
        }

        public DataTable ListarNFEspecifico(int nrSeqNf)
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM NF WHERE NrSeqNf = {nrSeqNf}";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao pesquisar: {ex.Message}");
            }
            return dt;
        }

        public void CarregarDados()
        {
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM NF WHERE NrSeqNf = {NrSeqNf}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        NrSeqPedido = dt.Rows[0]["NrSeqPedido"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["NrSeqPedido"]);
                        NrSeqImpostos = dt.Rows[0]["NrSeqImpostos"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["NrSeqImpostos"]);
                        NrSeqPessoa = dt.Rows[0]["NrSeqPessoa"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["NrSeqPessoa"]);
                        NrSeqFornecedor = dt.Rows[0]["NrSeqFornecedor"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["NrSeqFornecedor"]);
                        ValorTotal = dt.Rows[0]["ValorTotal"] == DBNull.Value ? 0 : Convert.ToDecimal(dt.Rows[0]["ValorTotal"]);
                        NomeDoFornecedor = dt.Rows[0]["NomeDoFornecedor"] == DBNull.Value ? string.Empty : dt.Rows[0]["NomeDoFornecedor"].ToString();
                        ValorImpostos = dt.Rows[0]["ValorImpostos"] == DBNull.Value ? 0 : Convert.ToDecimal(dt.Rows[0]["ValorImpostos"]);
                        Quantidade = dt.Rows[0]["Quantidade"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["Quantidade"]);
                        NomeDoProduto = dt.Rows[0]["NomeDoProduto"] == DBNull.Value ? string.Empty : dt.Rows[0]["NomeDoProduto"].ToString();
                        DataVencimento = dt.Rows[0]["DataVencimento"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["DataVencimento"]);
                        DataCadastro = dt.Rows[0]["DataCadastro"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["DataCadastro"]);
                        Serie = dt.Rows[0]["Serie"] == DBNull.Value ? string.Empty : dt.Rows[0]["Serie"].ToString();
                        NrNf = dt.Rows[0]["NrNf"] == DBNull.Value ? string.Empty : dt.Rows[0]["NrNf"].ToString();
                        ChaveNf = dt.Rows[0]["ChaveNf"] == DBNull.Value ? string.Empty : dt.Rows[0]["ChaveNf"].ToString();

                        if (NrSeqPedido > 0)
                        {
                            PedidoModel pedido = new PedidoModel();
                            pedido.NrSeqPedido = NrSeqPedido;
                            pedido.CarregarDados();

                            NrSeqProduto = pedido.NrSeqProduto;
                            CnpjFornecedor = pedido.CpfCnpj;

                            FreteModel frete = new FreteModel();
                            frete.NrSeqPedido = NrSeqPedido;
                            frete.PesquisaPorNrSeqPedido();

                            if (frete.NrSeqFrete > 0)
                            {
                                ValorFrete = frete.ValorFrete;
                            }
                            else
                            {
                                ValorFrete = 0;
                            }
                        }
                        if (NrSeqPessoa > 0)
                        {
                            EnderecoModel endereco = new EnderecoModel();
                            endereco.NrSeqPessoa = NrSeqPessoa;
                            endereco.CarregarDados();
                            EnderecoFornecedor = endereco.Rua;



                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao pesquisar: {ex.Message}");
            }
        }
    }
}
