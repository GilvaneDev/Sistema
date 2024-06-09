using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Uteis;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Sige_Erp.Models
{
    public class NFVModel
    {
        public int NrSeqPedido { get; set; }
        public int NrSeqNfv { get; set; }
        public int NrSeqImpostos { get; set; }
        public int NrSeqPessoa { get; set; }
        public int NrSeqProduto { get; set; }
        public int NrSeqCliente { get; set; }
        public decimal ValorTotal { get; set; }
        public string? NomeDoCliente { get; set; }
        public decimal ValorImpostos { get; set; }
        public int Quantidade { get; set; }
        public string? NomeDoProduto { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? deCadastro { get; set; }
        public DateTime? ateCadastro { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime? deVencimento { get; set; }
        public DateTime? ateVencimento { get; set; }
        public string? Serie { get; set; }
        public string? NrNfv { get; set; }
        public string? ChaveNfv { get; set; }
        public decimal ValorFrete { get; set; }
        public string? EnderecoCliente { get; set; }
        public string CnpjCliente { get; set; }

        public List<NFVModel> ListaNotas { get; set; }
        public List<Int32> IdsSelecionados { get; set; }
        public bool AbaAtiva { get; set; }
        public bool FlgEdicao { get; set; }
        public string PdfBase64 { get; set; }

        private int ObterUltimoNrSeqNfvInserido(DAL objDAL)
        {
            int ultimoNrSeqNfv = 0;

            try
            {
                string sql = "SELECT NrSeqNfv FROM NFV ORDER BY NrSeqNfv DESC LIMIT 1";
                DataTable dt = objDAL.RetDataTable(sql);

                if (dt.Rows.Count == 1)
                {
                    ultimoNrSeqNfv = Convert.ToInt32(dt.Rows[0]["NrSeqNfv"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter o último número sequencial de NFV: {ex.Message}");
            }

            return ultimoNrSeqNfv;
        }

        public void Cadastrar(DAL objDAL)
        {
            try
            {
                // Obtém o último NrSeqNfv
                int ultimoNrSeqNfv = ObterUltimoNrSeqNfvInserido(objDAL);

                // Incrementa o NrSeqNfv
                NrSeqNfv = ultimoNrSeqNfv + 1;

                // Define a instrução SQL com parâmetros
                string sql = "INSERT INTO NFV (NrSeqPedido, NrSeqNfv, NrSeqImpostos, NrSeqPessoa, NrSeqCliente, ValorTotal, " +
                             "NomeDoCliente, ValorImpostos, Quantidade, NomeDoProduto, DataVencimento, DataCadastro, Serie, " +
                             "NrNfv, ChaveNfv) " +
                             "VALUES (@NrSeqPedido, @NrSeqNfv, @NrSeqImpostos, @NrSeqPessoa, @NrSeqCliente, @ValorTotal, " +
                             "@NomeDoCliente, @ValorImpostos, @Quantidade, @NomeDoProduto, @DataVencimento, " +
                             "@DataCadastro, @Serie, @NrNfv, @ChaveNfv)";

                // Cria e configura os parâmetros
                List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@NrSeqPedido", NrSeqPedido),
                new MySqlParameter("@NrSeqNfv", NrSeqNfv),
                new MySqlParameter("@NrSeqImpostos", NrSeqImpostos),
                new MySqlParameter("@NrSeqPessoa", NrSeqPessoa),
                new MySqlParameter("@NrSeqCliente", NrSeqCliente),
                new MySqlParameter("@ValorTotal", ValorTotal),
                new MySqlParameter("@NomeDoCliente", NomeDoCliente),
                new MySqlParameter("@ValorImpostos", ValorImpostos),
                new MySqlParameter("@Quantidade", Quantidade),
                new MySqlParameter("@NomeDoProduto", NomeDoProduto),
                new MySqlParameter("@DataVencimento", DataVencimento.ToString("yyyy-MM-dd")),
                new MySqlParameter("@DataCadastro", DataCadastro.ToString("yyyy-MM-dd")),
                new MySqlParameter("@Serie", Serie),
                new MySqlParameter("@NrNfv", NrNfv),
                new MySqlParameter("@ChaveNfv", ChaveNfv)
            };

                // Executa o comando SQL com parâmetros
                objDAL.ExecutarComandoSQL(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar NFV: {ex.Message}");
            }
        }

        public void Atualizar(DAL objDAL)
        {
            try
            {
                // Define a instrução SQL com parâmetros
                string sql = "UPDATE NFV SET NrSeqPedido = @NrSeqPedido, NrSeqImpostos = @NrSeqImpostos, NrSeqPessoa = @NrSeqPessoa, " +
                             "NrSeqCliente = @NrSeqCliente, ValorTotal = @ValorTotal, NomeDoCliente = @NomeDoCliente, " +
                             "ValorImpostos = @ValorImpostos, Quantidade = @Quantidade, NomeDoProduto = @NomeDoProduto, " +
                             "DataVencimento = @DataVencimento, DataCadastro = @DataCadastro, Serie = @Serie, " +
                             "NrNfv = @NrNfv, ChaveNfv = @ChaveNfv " +
                             "WHERE NrSeqNfv = @NrSeqNfv";

                // Cria e configura os parâmetros
                List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@NrSeqPedido", NrSeqPedido),
                new MySqlParameter("@NrSeqImpostos", NrSeqImpostos),
                new MySqlParameter("@NrSeqPessoa", NrSeqPessoa),
                new MySqlParameter("@NrSeqCliente", NrSeqCliente),
                new MySqlParameter("@ValorTotal", ValorTotal),
                new MySqlParameter("@NomeDoCliente", NomeDoCliente),
                new MySqlParameter("@ValorImpostos", ValorImpostos),
                new MySqlParameter("@Quantidade", Quantidade),
                new MySqlParameter("@NomeDoProduto", NomeDoProduto),
                new MySqlParameter("@DataVencimento", DataVencimento.ToString("yyyy-MM-dd")),
                new MySqlParameter("@DataCadastro", DataCadastro.ToString("yyyy-MM-dd")),
                new MySqlParameter("@Serie", Serie),
                new MySqlParameter("@NrNfv", NrNfv),
                new MySqlParameter("@ChaveNfv", ChaveNfv),
                new MySqlParameter("@NrSeqNfv", NrSeqNfv)
            };

                // Executa o comando SQL com parâmetros
                objDAL.ExecutarComandoSQL(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar NFV: {ex.Message}");
            }
        }

        public void Excluir(DAL objDAL)
        {
            try
            {

                string sql = $"DELETE FROM NFV WHERE NrSeqNfv = {NrSeqNfv}";
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
                    string sql = "SELECT * FROM NFV";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                // Trate as exceções apropriadamente (por exemplo, registre ou lance)
            }
            return dt;
        }

        public List<NFVModel> Pesquisar(NFVModel nota)
        {
            string where = "1=1";

            if (!string.IsNullOrEmpty(nota.NomeDoProduto))
            {
                where += $@" AND NFV.NOMEDOPRODUTO LIKE '%{nota.NomeDoProduto}%'";
            }
            if (!string.IsNullOrEmpty(nota.NomeDoCliente))
            {
                where += $@" AND NFV.NOMEDOFORNECEDOR LIKE '%{nota.NomeDoCliente}%'";
            }
            if (nota.ValorTotal > 0)
            {
                where += $@" AND NFV.VALORTOTAL ={nota.ValorTotal}";
            }
            if (nota.ValorImpostos > 0)
            {
                where += $@" AND NFV.VALORIMPOSTOS ={nota.ValorImpostos}";
            }

            if (!string.IsNullOrEmpty(nota.NrNfv))
            {
                where += $@" AND NFV.NRNFV LIKE '%{nota.NrNfv}%'";
            }
            if (nota.NrSeqPedido > 0)
            {
                where += $@" AND NFV.NRSEQPEDIDO = {nota.NrSeqPedido}";
            }
            if (nota.Quantidade > 0)
            {
                where += $@" AND NFV.QUANTIDADE = {nota.Quantidade}";
            }
            if (!string.IsNullOrEmpty(nota.Serie))
            {
                where += $@" AND NFV.SERIE LIKE '%{nota.Serie}%'";
            }
            if (!string.IsNullOrEmpty(nota.ChaveNfv))
            {
                where += $@" AND NFV.CHAVENFV LIKE '%{nota.ChaveNfv}%'";
            }
            if (nota.deCadastro.HasValue && nota.ateCadastro.HasValue &&
                nota.deCadastro.Value > DateTime.MinValue && nota.ateCadastro.Value > DateTime.MinValue)
            {
                string dataCadastroInicio = nota.deCadastro.Value.ToString("yyyy-MM-dd");
                string dataCadastroFinal = nota.ateCadastro.Value.ToString("yyyy-MM-dd");

                where += $@" AND NFV.DATACADASTRO BETWEEN '{dataCadastroInicio}' AND '{dataCadastroFinal}'";
            }

            if (nota.deVencimento.HasValue && nota.ateVencimento.HasValue &&
                nota.deVencimento.Value > DateTime.MinValue && nota.ateVencimento.Value > DateTime.MinValue)
            {
                string dataVencimentoInicio = nota.deVencimento.Value.ToString("yyyy-MM-dd");
                string dataVencimentoFinal = nota.ateVencimento.Value.ToString("yyyy-MM-dd");

                where += $@" AND NFV.DATAVENCIMENTO BETWEEN '{dataVencimentoInicio}' AND '{dataVencimentoFinal}'";
            }


            string query = $@"
                SELECT
                    *
                    
                FROM `NFV`
                WHERE {where}";

            List<NFVModel> notas = new List<NFVModel>();

            try
            {
                using (DAL objDAL = new DAL())
                {
                    DataTable dt = objDAL.RetDataTable(query);

                    foreach (DataRow row in dt.Rows)
                    {
                        NFVModel notaf = new NFVModel();
                        notaf.NomeDoCliente = row["NomeDoCliente"] == DBNull.Value ? string.Empty : Convert.ToString(row["NomeDoCliente"]);
                        notaf.ValorTotal = row["ValorTotal"] == DBNull.Value ? 0 : Convert.ToDecimal(row["ValorTotal"]);
                        notaf.NrSeqNfv = row["NrSeqNfv"] == DBNull.Value ? 0 : Convert.ToInt32(row["NrSeqNfv"]);
                        notaf.NrSeqPedido = row["NrSeqNfv"] == DBNull.Value ? 0 : Convert.ToInt32(row["NrSeqNfv"]);
                        notaf.NrSeqImpostos = row["NrSeqImpostos"] == DBNull.Value ? 0 : Convert.ToInt32(row["NrSeqImpostos"]);
                        notaf.NrSeqPessoa = row["NrSeqPessoa"] == DBNull.Value ? 0 : Convert.ToInt32(row["NrSeqPessoa"]);
                        notaf.NrSeqCliente = row["NrSeqCliente"] == DBNull.Value ? 0 : Convert.ToInt32(row["NrSeqCliente"]);
                        notaf.Quantidade = row["Quantidade"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["Quantidade"]);
                        notaf.DataCadastro = row["DataCadastro"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["DataCadastro"]);
                        notaf.DataVencimento = row["DataVencimento"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["DataVencimento"]);
                        notaf.Serie = row["Serie"] == DBNull.Value ? string.Empty : Convert.ToString(row["Serie"]);
                        notaf.NomeDoProduto = row["NomeDoProduto"] == DBNull.Value ? string.Empty : Convert.ToString(row["NomeDoProduto"]);
                        notaf.ValorImpostos = row["ValorImpostos"] == DBNull.Value ? 0 : Convert.ToDecimal(row["ValorImpostos"]);
                        notaf.NrNfv = row["NrNfv"] == DBNull.Value ? string.Empty : Convert.ToString(row["NrNfv"]);
                        notaf.ChaveNfv = row["ChaveNfv"] == DBNull.Value ? string.Empty : Convert.ToString(row["ChaveNfv"]);

                        if (notaf.NrSeqPedido > 0)
                        {
                            PedidoModel pedido = new PedidoModel();
                            pedido.NrSeqPedido = notaf.NrSeqPedido;
                            pedido.CarregarDados();

                            notaf.NrSeqProduto = pedido.NrSeqProduto;
                            notaf.CnpjCliente = pedido.CpfCnpj;

                            FreteModel frete = new FreteModel();
                            frete.NrSeqPedido = notaf.NrSeqPedido;
                            frete.PesquisaPorNrSeqPedido();

                            if (frete.NrSeqFrete > 0)
                            {
                                notaf.ValorFrete = frete.ValorFrete;
                            }
                            else
                            {
                                notaf.ValorFrete = 0;
                            }
                        }
                        if (notaf.NrSeqPessoa > 0)
                        {
                            EnderecoModel endereco = new EnderecoModel();
                            endereco.NrSeqPessoa = notaf.NrSeqPessoa;
                            endereco.CarregarDados();
                            notaf.EnderecoCliente = endereco.Rua;



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

        public DataTable ListarNFVEspecifico(int nrSeqNfv)
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM NFV WHERE NrSeqNfv = {nrSeqNfv}";
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
                    string sql = $"SELECT * FROM NFV WHERE NrSeqNfv = {NrSeqNfv}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        NrSeqPedido = dt.Rows[0]["NrSeqPedido"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["NrSeqPedido"]);
                        NrSeqImpostos = dt.Rows[0]["NrSeqImpostos"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["NrSeqImpostos"]);
                        NrSeqPessoa = dt.Rows[0]["NrSeqPessoa"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["NrSeqPessoa"]);
                        NrSeqCliente = dt.Rows[0]["NrSeqCliente"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["NrSeqCliente"]);
                        ValorTotal = dt.Rows[0]["ValorTotal"] == DBNull.Value ? 0 : Convert.ToDecimal(dt.Rows[0]["ValorTotal"]);
                        NomeDoCliente = dt.Rows[0]["NomeDoCliente"] == DBNull.Value ? string.Empty : dt.Rows[0]["NomeDoCliente"].ToString();
                        ValorImpostos = dt.Rows[0]["ValorImpostos"] == DBNull.Value ? 0 : Convert.ToDecimal(dt.Rows[0]["ValorImpostos"]);
                        Quantidade = dt.Rows[0]["Quantidade"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["Quantidade"]);
                        NomeDoProduto = dt.Rows[0]["NomeDoProduto"] == DBNull.Value ? string.Empty : dt.Rows[0]["NomeDoProduto"].ToString();
                        DataVencimento = dt.Rows[0]["DataVencimento"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["DataVencimento"]);
                        DataCadastro = dt.Rows[0]["DataCadastro"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["DataCadastro"]);
                        Serie = dt.Rows[0]["Serie"] == DBNull.Value ? string.Empty : dt.Rows[0]["Serie"].ToString();
                        NrNfv = dt.Rows[0]["NrNfv"] == DBNull.Value ? string.Empty : dt.Rows[0]["NrNfv"].ToString();
                        ChaveNfv = dt.Rows[0]["ChaveNfv"] == DBNull.Value ? string.Empty : dt.Rows[0]["ChaveNfv"].ToString();

                        if (NrSeqPedido > 0)
                        {
                            PedidoModel pedido = new PedidoModel();
                            pedido.NrSeqPedido = NrSeqPedido;
                            pedido.CarregarDados();

                            NrSeqProduto = pedido.NrSeqProduto;
                            CnpjCliente = pedido.CpfCnpj;

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
                            EnderecoCliente = endereco.Rua;



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
