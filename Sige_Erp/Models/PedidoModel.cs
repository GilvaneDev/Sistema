using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Uteis;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Sige_Erp.Models
{
    public class PedidoModel
    {
        public decimal Desconto { get; set; }
        public DateTime DataVencimento { get; set; }
        public string CpfCnpj { get; set; }
        public int NrSeqPedido { get; set; }
        public int NrSeqPessoa { get; set; }
        public int NrSeqPessoaCli { get; set; }
        public int NrSeqPessoaFor { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DeDataCadastro { get; set; }
        public DateTime? AteDataCadastro { get; set; }
        public DateTime? DeVencimento { get; set; }
        public DateTime? AteVencimento { get; set; }
        public int Quantidade { get; set; }
        public string NomeProduto { get; set; }
        public string Cliente { get; set; }
        public string Fornecedor { get; set; }
        public string TipoUsuario { get; set; }
        public string StatusPedido { get; set; }
        public string Etiqueta { get; set; }
        public string Motivo { get; set; }
        public string Marca { get; set; }
        public string Frete { get; set; }
        public int NrSeqProduto { get; set; }
        public int CodigoProduto { get; set; }
        public bool CheckboxTroca { get; set; }
        public bool CheckboxDevolucao { get; set; }
        public string PrecoUnitario { get; set; }
        public decimal Preco { get; set; }

        public string Descricao { get; set; }
        public List<PedidoModel> ListaPedidos { get; set; }
        public List<Int32> IdsSelecionados { get; set; }
        public bool AbaAtiva { get; set; }
        public bool FlgEdicao { get; set; }
        public string PdfBase64 { get; set; }
        

        public int ObterUltimoNrSeqPedidoInserido(DAL objDAL)
        {
            int ultimoNrSeqPedido = 0;

            try
            {
                string sql = "SELECT NrSeqPedido FROM Pedido ORDER BY NrSeqPedido DESC LIMIT 1";
                DataTable dt = objDAL.RetDataTable(sql);

                if (dt.Rows.Count == 1)
                {
                    ultimoNrSeqPedido = Convert.ToInt32(dt.Rows[0]["NrSeqPedido"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar: {ex.Message}");
            }

            return ultimoNrSeqPedido;
        }


        public void Cadastrar(DAL objDAL)
        {
            try
            {
                // Obtém o último NrSeqPedido
                int ultimoNrSeqPedido = ObterUltimoNrSeqPedidoInserido(objDAL);

                // Incrementa o NrSeqPedido
                NrSeqPedido = ultimoNrSeqPedido + 1;

                // Define a instrução SQL com parâmetros
                string sql = "INSERT INTO Pedido (Desconto, DataVencimento, CpfCnpj, NrSeqPedido, NrSeqPessoa, DataCadastro, Quantidade, NomeProduto, NrSeqProduto, CodigoProduto, Preco, Descricao) " +
                             "VALUES (@Desconto, @DataVencimento, @CpfCnpj, @NrSeqPedido, @NrSeqPessoa, @DataCadastro, @Quantidade, @NomeProduto, @NrSeqProduto, @CodigoProduto, @Preco, @Descricao)";

                // Cria e configura os parâmetros
                List<MySqlParameter> parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@Desconto", Desconto),
            new MySqlParameter("@DataVencimento", DataVencimento.ToString("yyyy-MM-dd")),
            new MySqlParameter("@CpfCnpj", CpfCnpj),
            new MySqlParameter("@NrSeqPedido", NrSeqPedido),
            new MySqlParameter("@NrSeqPessoa", NrSeqPessoa),
            new MySqlParameter("@DataCadastro", DataCadastro.ToString("yyyy-MM-dd")),
            new MySqlParameter("@Quantidade", Quantidade),
            new MySqlParameter("@NomeProduto", NomeProduto),
            new MySqlParameter("@NrSeqProduto", NrSeqProduto),
            new MySqlParameter("@CodigoProduto", CodigoProduto),
            new MySqlParameter("@Preco", Preco),
            new MySqlParameter("@Descricao", Descricao)
        };

                // Executa o comando SQL com parâmetros
                objDAL.ExecutarComandoSQL(sql, parameters);
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
                
                    string sql = $"UPDATE Pedido SET Desconto = {Desconto}, DataVencimento = '{DataVencimento:yyyy-MM-dd}', CpfCnpj = '{CpfCnpj}', " +
                                 $"NrSeqPessoa = {NrSeqPessoa}, DataCadastro = '{DataCadastro:yyyy-MM-dd}', Quantidade = {Quantidade}, " +
                                 $"NomeProduto = '{NomeProduto}', NrSeqProduto = {NrSeqProduto}, CodigoProduto = {CodigoProduto}, " +
                                 $"Preco = {Preco}, Descricao = '{Descricao}' " +
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
                    string sql = $"DELETE FROM Pedido WHERE NrSeqPedido = {NrSeqPedido}";
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
                    string sql = "SELECT * FROM Pedido";
                    dt = objDAL.RetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                // Trate as exceções apropriadamente (por exemplo, registre ou lance)
            }
            return dt;
        }

        public DataTable ListarPedidoEspecifico(int nrSeqPedido)
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM Pedido WHERE NrSeqPedido = {nrSeqPedido}";
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
                    string sql = $"SELECT * FROM Pedido WHERE NrSeqPedido = {NrSeqPedido}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        Desconto = Convert.ToDecimal(dt.Rows[0]["Desconto"]);
                        DataVencimento = Convert.ToDateTime(dt.Rows[0]["DataVencimento"]);
                        CpfCnpj = dt.Rows[0]["CpfCnpj"].ToString();
                        NrSeqPessoa = Convert.ToInt32(dt.Rows[0]["NrSeqPessoa"]);
                        NrSeqPedido = Convert.ToInt32(dt.Rows[0]["NrSeqPedido"]);
                        DataCadastro = Convert.ToDateTime(dt.Rows[0]["DataCadastro"]);
                        Quantidade = Convert.ToInt32(dt.Rows[0]["Quantidade"]);
                        NomeProduto = dt.Rows[0]["NomeProduto"].ToString();
                        NrSeqProduto = Convert.ToInt32(dt.Rows[0]["NrSeqProduto"]);
                        CodigoProduto = Convert.ToInt32(dt.Rows[0]["CodigoProduto"]);
                        Preco = Convert.ToDecimal(dt.Rows[0]["Preco"]);
                        Descricao = dt.Rows[0]["Descricao"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar: {ex.Message}");
            }
        }

        public List<PedidoModel> Pesquisar(PedidoModel pedidoModel)
        {
            string where = "1=1";

            if (!string.IsNullOrEmpty(pedidoModel.NomeProduto))
            {
                where += $@" AND PEDIDO.NOMEPRODUTO LIKE '%{pedidoModel.NomeProduto}%'";
            }
            if (!string.IsNullOrEmpty(pedidoModel.Cliente))
            {
                where += $@" AND PESSOA.NOMEPESSOA LIKE '%{pedidoModel.Cliente}%'";
            }
            if (!string.IsNullOrEmpty(pedidoModel.Fornecedor))
            {
                where += $@" AND PESSOA.NOMEPESSOA LIKE '%{pedidoModel.Fornecedor}%'";
            }
            if (pedidoModel.CodigoProduto > 0)
            {
                where += $@" AND PEDIDO.CODIGOPRODUTO ={pedidoModel.CodigoProduto}";
            }

            if (pedidoModel.NrSeqPessoaCli > 0)
            {
                where += $@" AND PESSOA.NRSEQPESSOA = {pedidoModel.NrSeqPessoaCli}";
            }
            if (pedidoModel.NrSeqPessoaFor > 0)
            {
                where += $@" AND PESSOA.NRSEQPESSOA = {pedidoModel.NrSeqPessoaFor}";
            }
            if (!string.IsNullOrEmpty(pedidoModel.CpfCnpj))
            {
                where += $@" AND TIPOPESSOA.CPFCNPJ LIKE '%{pedidoModel.CpfCnpj}%'";
            }
            if (!string.IsNullOrEmpty(pedidoModel.TipoUsuario))
            {
                if(pedidoModel.TipoUsuario == "Cliente")
                {
                    where += $@" AND USUARIO.TIPOUSUARIO LIKE '%{pedidoModel.TipoUsuario}%'";
                }
                else
                {
                    where += $@" AND USUARIO.TIPOUSUARIO LIKE '%{pedidoModel.TipoUsuario}%'";
                }
                
            }
            

            if (pedidoModel.DeDataCadastro.HasValue && pedidoModel.AteDataCadastro.HasValue &&
                pedidoModel.DeDataCadastro.Value > DateTime.MinValue && pedidoModel.AteDataCadastro.Value > DateTime.MinValue)
            {
                string dataCadastroInicio = pedidoModel.DeDataCadastro.Value.ToString("yyyy-MM-dd");
                string dataCadastroFinal = pedidoModel.AteDataCadastro.Value.ToString("yyyy-MM-dd");

                where += $@" AND PEDIDO.DATACADASTRO BETWEEN '{dataCadastroInicio}' AND '{dataCadastroFinal}'";
            }

            if (pedidoModel.DeVencimento.HasValue && pedidoModel.AteVencimento.HasValue &&
                pedidoModel.DeVencimento.Value > DateTime.MinValue && pedidoModel.AteVencimento.Value > DateTime.MinValue)
            {
                string dataVencimentoInicio = pedidoModel.DeVencimento.Value.ToString("yyyy-MM-dd");
                string dataVencimentoFinal = pedidoModel.AteVencimento.Value.ToString("yyyy-MM-dd");

                where += $@" AND PEDIDO.DATAVENCIMENTO BETWEEN '{dataVencimentoInicio}' AND '{dataVencimentoFinal}'";
            }


            string query = $@"
                SELECT
                    PESSOA.`NOMEPESSOA`,
                    PEDIDO.`NRSEQPESSOA`,
                    PEDIDO.`NRSEQPEDIDO`,
                    PEDIDO.`DATAVENCIMENTO`,
                    PEDIDO.`DATACADASTRO`,
                    PEDIDO.`CPFCNPJ`,
                    PEDIDO.`QUANTIDADE`,
                    PEDIDO.`NOMEPRODUTO`,
                    PEDIDO.`NRSEQPRODUTO`,
                    PEDIDO.`CODIGOPRODUTO`,
                    PEDIDO.`PRECO`,
                    PEDIDO.`DESCRICAO`
                    
                FROM `PEDIDO`
                LEFT JOIN `PESSOA` ON PEDIDO.`NRSEQPESSOA` = PESSOA.`NRSEQPESSOA`
                LEFT JOIN `PRODUTO` ON PEDIDO.`NRSEQPRODUTO` = PRODUTO.`NRSEQPRODUTO`
                LEFT JOIN `USUARIO` ON PEDIDO.`NRSEQPESSOA` = USUARIO.`NRSEQPESSOA`
                LEFT JOIN `TIPOPESSOA` ON PEDIDO.`NRSEQPESSOA` = TIPOPESSOA.`NRSEQPESSOA` 
                WHERE {where}";

            List<PedidoModel> pedidos = new List<PedidoModel>();

            try
            {
                using (DAL objDAL = new DAL())
                {
                    DataTable dt = objDAL.RetDataTable(query);

                    foreach (DataRow row in dt.Rows)
                    {
                        PedidoModel pedido = new PedidoModel();
                        pedido.NrSeqPessoa = row["NRSEQPESSOA"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["NRSEQPESSOA"]);
                        
                       
                       pedido.Quantidade = row["QUANTIDADE"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["QUANTIDADE"]);
                        pedido.CodigoProduto = row["CODIGOPRODUTO"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["CODIGOPRODUTO"]);
                        pedido.Preco = row["PRECO"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["PRECO"]);

                        pedido.Descricao = row["DESCRICAO"] == DBNull.Value ? string.Empty : Convert.ToString(row["DESCRICAO"]);
                        pedido.NrSeqPedido = row["NRSEQPEDIDO"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["NRSEQPEDIDO"]);                   
                       pedido.NomeProduto = row["NOMEPRODUTO"] == DBNull.Value ? string.Empty : Convert.ToString(row["NOMEPRODUTO"]);
                        pedido.CpfCnpj = row["CPFCNPJ"] == DBNull.Value ? string.Empty : Convert.ToString(row["CPFCNPJ"]);
                        pedido.DataCadastro = row["DATACADASTRO"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["DATACADASTRO"]);
                        pedido.DataVencimento = row["DATAVENCIMENTO"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["DATAVENCIMENTO"]);

                        PessoaModel pessoaModel = new PessoaModel();
                        pessoaModel.NrSeqPessoa = pedido.NrSeqPessoa;
                        pessoaModel.Carregar();

                        if (pessoaModel.NrSeqCliente > 0)
                            pedido.Cliente = row["NOMEPESSOA"] == DBNull.Value ? string.Empty : Convert.ToString(row["NOMEPESSOA"]);
                        if (pessoaModel.NrSeqFornecedor > 0)
                            pedido.Fornecedor = row["NOMEPESSOA"] == DBNull.Value ? string.Empty : Convert.ToString(row["NOMEPESSOA"]);



                        pedidos.Add(pedido);
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao pesquisar: {ex.Message}");
            }

            return pedidos;
        }

        public void Carregar()
        {
            string query = $@"
            SELECT
                PESSOA.`NOMEPESSOA`,
                    PEDIDO.`NRSEQPESSOA`,
                    PEDIDO.`NRSEQPEDIDO`,
                    PEDIDO.`DATAVENCIMENTO`,
                    PEDIDO.`DATACADASTRO`,
                    PEDIDO.`CPFCNPJ`,
                    PEDIDO.`QUANTIDADE`,
                    PEDIDO.`NOMEPRODUTO`,
                    PEDIDO.`NRSEQPRODUTO`,
                    PEDIDO.`CODIGOPRODUTO`,
                    PEDIDO.`PRECO`,                    
                    PEDIDO.`DESCONTO`,
                    PEDIDO.`DESCRICAO`
            FROM `PEDIDO`
            LEFT JOIN `PESSOA` ON PEDIDO.`NRSEQPESSOA` = PESSOA.`NRSEQPESSOA`
            LEFT JOIN `PRODUTO` ON PEDIDO.`NRSEQPRODUTO` = PRODUTO.`NRSEQPRODUTO`
            LEFT JOIN `USUARIO` ON PEDIDO.`NRSEQPESSOA` = USUARIO.`NRSEQPESSOA`
            LEFT JOIN `TIPOPESSOA` ON PEDIDO.`NRSEQPESSOA` = TIPOPESSOA.`NRSEQPESSOA`
            LEFT JOIN `ETIQUETAGEM` ON PEDIDO.`NRSEQPEDIDO` = ETIQUETAGEM.`NRSEQPEDIDO` 
            LEFT JOIN `STATUSPEDIDO` ON PEDIDO.`NRSEQPEDIDO` = STATUSPEDIDO.`NRSEQPEDIDO` 
            WHERE PEDIDO.NRSEQPEDIDO = {NrSeqPedido}";

            try
            {
                using (DAL objDAL = new DAL())
                {
                    DataTable dt = objDAL.RetDataTable(query);

                    if (dt.Rows.Count == 1)
                    {
                        NrSeqPessoa = dt.Rows[0]["NRSEQPESSOA"] == DBNull.Value ? int.MinValue : Convert.ToInt32(dt.Rows[0]["NRSEQPESSOA"]);
                        NrSeqProduto = dt.Rows[0]["NRSEQPRODUTO"] == DBNull.Value ? int.MinValue : Convert.ToInt32(dt.Rows[0]["NRSEQPRODUTO"]);
                        NrSeqPedido = dt.Rows[0]["NRSEQPEDIDO"] == DBNull.Value ? int.MinValue : Convert.ToInt32(dt.Rows[0]["NRSEQPEDIDO"]);
                        Quantidade = dt.Rows[0]["QUANTIDADE"] == DBNull.Value ? int.MinValue : Convert.ToInt32(dt.Rows[0]["QUANTIDADE"]);
                        CodigoProduto = dt.Rows[0]["CODIGOPRODUTO"] == DBNull.Value ? int.MinValue : Convert.ToInt32(dt.Rows[0]["CODIGOPRODUTO"]);
                        Preco = dt.Rows[0]["PRECO"] == DBNull.Value ? decimal.MinValue : Convert.ToDecimal(dt.Rows[0]["PRECO"]);
                        PrecoUnitario = dt.Rows[0]["PRECO"] == DBNull.Value ?  string.Empty : Convert.ToString(dt.Rows[0]["PRECO"]);                      
                        if (Preco > 0)
                            Frete = ((Preco * 0.08M) < 12.90M ? 12.90M : (Preco * 0.08M)).ToString("F2");
                        Descricao = dt.Rows[0]["DESCRICAO"] == DBNull.Value ? string.Empty : Convert.ToString(dt.Rows[0]["DESCRICAO"]);
                        NomeProduto = dt.Rows[0]["NOMEPRODUTO"] == DBNull.Value ? string.Empty : Convert.ToString(dt.Rows[0]["NOMEPRODUTO"]);
                        CpfCnpj = dt.Rows[0]["CPFCNPJ"] == DBNull.Value ? string.Empty : Convert.ToString(dt.Rows[0]["CPFCNPJ"]);
                        DataCadastro = dt.Rows[0]["DATACADASTRO"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["DATACADASTRO"]);
                        DataVencimento = dt.Rows[0]["DATAVENCIMENTO"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["DATAVENCIMENTO"]);

                        PessoaModel pessoaModel = new PessoaModel();
                        pessoaModel.NrSeqPessoa = NrSeqPessoa;
                        pessoaModel.Carregar();

                        if (pessoaModel.NrSeqCliente > 0)
                        {
                            Cliente = dt.Rows[0]["NOMEPESSOA"] == DBNull.Value ? string.Empty : Convert.ToString(dt.Rows[0]["NOMEPESSOA"]);
                            NrSeqPessoaCli = NrSeqPessoa;
                            TipoUsuario = "Cliente";
                        }
                            
                        if (pessoaModel.NrSeqFornecedor > 0)
                        {
                            Fornecedor = dt.Rows[0]["NOMEPESSOA"] == DBNull.Value ? string.Empty : Convert.ToString(dt.Rows[0]["NOMEPESSOA"]);
                            NrSeqPessoaFor = NrSeqPessoa;
                            TipoUsuario = "Fornecedor";
                        }
                        if(NrSeqPedido > 0)
                        {
                            StatusPedidoModel status = new StatusPedidoModel();
                            status.NrSeqPedido = NrSeqPedido;
                            status.CarregarDadosPorPedido();
                            if (status.Entregue == 'N' && status.Aprovado == 'N' && status.Enviado == 'N')
                            {
                                StatusPedido = "Aguardando";
                                
                            }
                            else if (status.Entregue == 'N' && status.Aprovado == 'S' && status.Enviado == 'N')
                            {
                                StatusPedido = "Aprovado";
  
                            }
                            else if (status.Entregue == 'N' && status.Aprovado == 'S' && status.Enviado == 'S')
                            {
                                StatusPedido = "Enviado";

                            }
                            else if (status.Entregue == 'S' && status.Aprovado == 'S' && status.Enviado == 'S')
                            {
                                StatusPedido = "Entregue";
         
                            }

                            EtiquetagemModel etiquetagem = new EtiquetagemModel();
                            etiquetagem.NrSeqPedido = NrSeqPedido;
                            etiquetagem.CarregarDadosPorPedido();
                            if(etiquetagem.Embalado == 'N' && etiquetagem.Enviado == 'N')
                            {
                                Etiqueta = "A Embalar";
                            }
                            else if (etiquetagem.Embalado == 'S' && etiquetagem.Enviado == 'N')
                            {
                                Etiqueta = "Embalado";
                            }
                            else if (etiquetagem.Embalado == 'S' && etiquetagem.Enviado == 'S')
                            {
                                Etiqueta = "Enviado";
                            }
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

