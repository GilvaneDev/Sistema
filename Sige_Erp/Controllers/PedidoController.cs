using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Models;
using Sige_Erp.Uteis;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using iTextSharp.text.pdf.qrcode;

namespace Sige_Erp.Controllers
{
    public class PedidoController : Controller
    {
        public IActionResult Index(PedidoModel pedido)
        {
            pedido.AbaAtiva = false;
            pedido.ListaPedidos = new List<PedidoModel>();
            pedido.DeDataCadastro = null;
            pedido.AteDataCadastro = null;
            pedido.DeVencimento = null;
            pedido.AteVencimento = null;
            return View("Index", pedido);
        }
        public bool ValidacaoCadastro(PedidoModel pedido)
        {
            if (string.IsNullOrEmpty(pedido.NomeProduto))
            {
                throw new Exception($"Por favor, informe o nome do produto.");
            }
            if (string.IsNullOrEmpty(pedido.Etiqueta))
            {
                throw new Exception($"Por favor, informe uma etiqueta.");
            }

            if (pedido.Preco <= 0)
            {
                throw new Exception($"Por favor, informe o Preço.");
            }



            if (pedido.Quantidade <= 0)
            {
                throw new Exception($"Quantidade não pede ser menor 1");
                
            }

            if (!string.IsNullOrEmpty(pedido.TipoUsuario)){
                if(pedido.TipoUsuario == "Fornecedor" && string.IsNullOrEmpty(pedido.Fornecedor))
                {
                    throw new Exception($"Por favor, informe o Fornecedor.");
                }
                else if(pedido.TipoUsuario == "Cliente" && string.IsNullOrEmpty(pedido.Cliente))
                {
                    throw new Exception($"Por favor, informe o Cliente.");
                }
            } 
                
      


           

            if (string.IsNullOrEmpty(pedido.StatusPedido))
            {
                throw new Exception($"Por favor, informe o status do pedido.");         
            }

            if (pedido.DataCadastro == DateTime.MinValue)
            {
                throw new Exception($"Por favor, informe a Data de cadastro.");
               
            }
            if (pedido.DataVencimento == DateTime.MinValue)
            {
                throw new Exception($"Por favor, informe a Data de vencimento.");
            }

            if (string.IsNullOrEmpty(pedido.Descricao))
            {
                throw new Exception($"Por favor, coloque uma descrição.");
            }   
            if (string.IsNullOrEmpty(pedido.Frete))
            {
                throw new Exception($"Por favor, informe o frete.");         
            }
            if (pedido.CheckboxTroca)
            {
                if(string.IsNullOrEmpty(pedido.Motivo))
                {
                    throw new Exception("Informe o Motivo da troca");
                }
            }
            if (pedido.CheckboxDevolucao)
            {
                if (string.IsNullOrEmpty(pedido.Motivo))
                {
                    throw new Exception("Informe o Motivo da devolução");
                }
            }
            if (!string.IsNullOrEmpty(pedido.Motivo) && pedido.CheckboxDevolucao == false && pedido.CheckboxTroca == false)
            {
                throw new Exception("Informe se é Troca ou Devolução");
            }

            return true;
        }

        [HttpPost]
        public IActionResult CadastroPedidoManutencao(PedidoModel pedidoModel)
        {
            try
            {
                if(pedidoModel.NrSeqPedido <= 0)
                {
                    if (pedidoModel.NrSeqProduto > 0)
                    {
                        ProdutoModel produto = new ProdutoModel();
                        produto.ListarProdutoEspecifico(pedidoModel.NrSeqProduto);
                        pedidoModel.CodigoProduto = produto.CodigoProduto;
                        if(pedidoModel.Quantidade > produto.Quantidade && pedidoModel.NrSeqPessoaCli > 0)
                        {
                            throw new Exception($"Produto tem apenas {produto.Quantidade} itens disponivel" );
                        }
                    }
                    else
                    {
                        throw new Exception($"Por favor, informe o nome do produto.");
                    }
                    if (pedidoModel.NrSeqPessoaCli > 0)
                    {
                        PessoaModel pessoa = new PessoaModel();
                        pessoa.NrSeqPessoa = pedidoModel.NrSeqPessoaCli;
                        pessoa.Carregar();
                        pedidoModel.CpfCnpj = pessoa.CpfCnpj;
                        pedidoModel.NrSeqPessoa = pedidoModel.NrSeqPessoaCli;
                    }
                    else if (pedidoModel.NrSeqPessoaFor > 0)
                    {
                        PessoaModel pessoa = new PessoaModel();
                        pessoa.NrSeqPessoa = pedidoModel.NrSeqPessoaFor;
                        pessoa.Carregar();
                        pedidoModel.CpfCnpj = pessoa.CpfCnpj;
                        pedidoModel.NrSeqPessoa = pedidoModel.NrSeqPessoaFor;
                    }
                    

                    if (ValidacaoCadastro(pedidoModel))
                    {
                        using (DAL objDAL = new DAL())
                        {
                            objDAL.OpenTransaction();

                            try
                            {
                                pedidoModel.Preco = (pedidoModel.Preco / 100);
                                decimal desconto = 0m;
                                decimal valortotal = 0;
                                decimal resultado = 0;

                                valortotal = Math.Round(pedidoModel.Preco * pedidoModel.Quantidade, 2);

                                // Utilizando o operador ternário 
                                resultado = valortotal >= 500.00m ? Math.Round(valortotal * 0.05m, 2) : 0m;

                                // Atribuindo o valor de resultado a desconto
                                desconto = resultado;
                                pedidoModel.Desconto= desconto;
                                pedidoModel.Cadastrar(objDAL);

                                if(pedidoModel.NrSeqPedido > 0)
                                {
                                    FreteModel frete = new FreteModel();
                                    frete.DataExpedicao = pedidoModel.DataCadastro;
                                    frete.NrSeqPedido = pedidoModel.NrSeqPedido;
                                    frete.ValorFrete = Math.Round(Convert.ToDecimal(pedidoModel.Frete),2);
                                    frete.Cadastrar(objDAL);


                                    EtiquetagemModel etiquetagem = new EtiquetagemModel();
                                    etiquetagem.NrSeqPedido = pedidoModel.NrSeqPedido;
                                    etiquetagem.NomeDoProduto = pedidoModel.NomeProduto;
                                    if(pedidoModel.Etiqueta == "A Embalar")
                                    {
                                        etiquetagem.Embalado = 'N';
                                        etiquetagem.Enviado = 'N';
                                    }
                                    else if(pedidoModel.Etiqueta == "Embalado")
                                    {
                                        etiquetagem.Embalado = 'S';
                                        etiquetagem.Enviado = 'N';
                                    }
                                    else
                                    {
                                        etiquetagem.Embalado = 'S';
                                        etiquetagem.Enviado = 'S';
                                    }
                                    etiquetagem.Cadastrar(objDAL);
                                    if(etiquetagem.NrSeqEtiquetagem > 0)
                                    {
                                        StatusPedidoModel status = new StatusPedidoModel();
                                        status.NrSeqPedido = pedidoModel.NrSeqPedido;
                                        status.NrSeqEtiquetagem = etiquetagem.NrSeqEtiquetagem;
                                        if(pedidoModel.StatusPedido == "Aguardando")
                                        {
                                            status.Aprovado = 'N';
                                            status.Enviado = 'N';
                                            status.Entregue = 'N';
                                        }
                                        else if (pedidoModel.StatusPedido == "Aprovado")
                                        {
                                            status.Aprovado = 'S';
                                            status.Enviado = 'N';
                                            status.Entregue = 'N';
                                        }
                                        else if (pedidoModel.StatusPedido == "Enviado")
                                        {
                                            status.Aprovado = 'S';
                                            status.Enviado = 'S';
                                            status.Entregue = 'N';
                                        }
                                        else
                                        {
                                            status.Aprovado = 'S';
                                            status.Enviado = 'S';
                                            status.Entregue = 'S';
                                        }
                                        status.Cadastrar(objDAL);
                                    }

                                    if(pedidoModel.TipoUsuario == "Cliente")
                                    {

                                        


                                        // Recuperar o objeto PessoaModelPesquisa da sessão
                                        string NrSeqUsuarioLogado = HttpContext.Session.GetString("NrSeqUsuarioLogado");

                                        UsuarioModel usuarioLogado = new UsuarioModel();
                                        usuarioLogado.NrSeqUsuario = Convert.ToInt32(NrSeqUsuarioLogado);
                                        usuarioLogado.CarregarDados();

                                        FuncionarioModel funcionario = new FuncionarioModel();
                                        funcionario.NrSeqPessoa = usuarioLogado.NrSeqPessoa;
                                        funcionario.CarregarDados();

                                        if(funcionario.NrSeqFuncionario > 0)
                                        {
                                            AtenderClienteModel atender = new AtenderClienteModel();
                                            atender.NrSeqFuncionario = funcionario.NrSeqFuncionario;
                                            atender.NomeDoFuncionario = funcionario.NomeDoFuncionario;
                                            atender.Cadastrar(objDAL);

                                            ComissaoModel comissao = new ComissaoModel();
                                            comissao.NrSeqPedido = pedidoModel.NrSeqPedido;
                                            comissao.NrSeqProduto = pedidoModel.NrSeqProduto;
                                            comissao.NrSeqFuncionario = funcionario.NrSeqFuncionario;
                                            comissao.NomeFuncionario = funcionario.NomeDoFuncionario;
                                            comissao.DataCadastro = pedidoModel.DataCadastro;
                                            comissao.ValorVenda = (pedidoModel.Preco * pedidoModel.Quantidade);
                                            comissao.PorcentageComissao = 3.5M;
                                            comissao.ValorComissao = Math.Round(comissao.ValorVenda * (comissao.PorcentageComissao / 100),2);
                                            comissao.Cadastrar(objDAL);

                                        }
                                        

                                        ProdutoModel produto = new ProdutoModel();
                                        produto.NrSeqProduto = pedidoModel.NrSeqProduto;
                                        produto.CarregarDados();

                                        EstoqueModel estoque = new EstoqueModel();
                                        estoque.NomeDoProduto = produto.NomeDoProduto;
                                        estoque.Pesquisar();

                                        if (estoque.NrSeqEstoque > 0)
                                        {
                                            estoque.Quantidade = estoque.Quantidade - pedidoModel.Quantidade;
                                            estoque.Atualizar(objDAL);

                                            MovimentacaoModel movimentacao = new MovimentacaoModel();
                                            movimentacao.DataSaida = estoque.DataCadastro; 
                                            movimentacao.Saida = 'S';
                                            movimentacao.Entrada = 'N';
                                            movimentacao.Quantidade = estoque.Quantidade;
                                            movimentacao.DataEntrada = DateTime.MinValue;
                                            movimentacao.NomePessoa = estoque.NomeDoFornecedor;
                                            movimentacao.ValorPago = estoque.PrecoUnitario;
                                            movimentacao.NrSeqPedido = pedidoModel.NrSeqPedido;
                                            movimentacao.NrSeqProduto = produto.NrSeqProduto;
                                            movimentacao.Marca = produto.Marca;
                                            movimentacao.NomeDoProduto = produto.NomeDoProduto;
                                            movimentacao.Cadastrar(objDAL);


                                        }
                                        else
                                        {
                                            throw new Exception("ESte produto nao tem em estoque");
                                        }
                                    }
                                }
                           
                                objDAL.CommitTransaction();
                                TempData["MensagemSucesso"] = "Cadastro realizado com sucesso!";

                            }
                            catch (Exception ex)
                            {
                                objDAL.RollbackTransaction();
                                TempData["MensagemErro"] = $"Erro: {ex.Message}";
                            }
                        }
                    }
                    else
                    {
                        TempData["MensagemErro"] = "Preencha todos os campos.";
                        return View("Manutencao", pedidoModel);
                    }
                }
                else
                {
                    pedidoModel.Preco =  Convert.ToDecimal((pedidoModel.PrecoUnitario).Replace(",",".") ) / 100;
                    if (ValidacaoCadastro(pedidoModel))
                    {
                        using (DAL objDAL = new DAL())
                        {
                            objDAL.OpenTransaction();

                            try
                            {
                                if (pedidoModel.NrSeqPedido > 0)
                                {
                                    TrocaDevolucaoModel trocaDevolucao = new TrocaDevolucaoModel();
                                    trocaDevolucao.DataTrocaDevolucao = DateTime.Now;
                                    trocaDevolucao.Motivo = pedidoModel.Motivo;
                                    trocaDevolucao.NomeDoProduto = pedidoModel.NomeProduto;
                                    trocaDevolucao.NrSeqProduto = pedidoModel.NrSeqProduto;
                                    if (pedidoModel.CheckboxTroca)
                                    {
                                        trocaDevolucao.Devolucao = 'N';
                                        trocaDevolucao.Troca = 'S';
                                        trocaDevolucao.Cadastrar(objDAL);
                                    }
                                    if (pedidoModel.CheckboxDevolucao)
                                    {
                                        trocaDevolucao.Devolucao = 'S';
                                        trocaDevolucao.Troca = 'N';
                                        trocaDevolucao.Cadastrar(objDAL);
                                    }

                                    FreteModel freteModel = new FreteModel();
                                    freteModel.NrSeqPedido = pedidoModel.NrSeqPedido;
                                    freteModel.PesquisaPorNrSeqPedido();

                                    if(freteModel.NrSeqFrete > 0)
                                    {
                                        FreteModel frete = new FreteModel();
                                        frete.DataExpedicao = pedidoModel.DataCadastro;
                                        frete.NrSeqPedido = pedidoModel.NrSeqPedido;
                                        frete.ValorFrete = Math.Round(Convert.ToDecimal(pedidoModel.Frete), 2);
                                        frete.Atualizar(objDAL);
                                    }
                                  

                                    EtiquetagemModel etiquetagem = new EtiquetagemModel();
                                    etiquetagem.NrSeqPedido = pedidoModel.NrSeqPedido;
                                    etiquetagem.NomeDoProduto = pedidoModel.NomeProduto;
                                    if (pedidoModel.Etiqueta == "A Embalar")
                                    {
                                        etiquetagem.Embalado = 'N';
                                        etiquetagem.Enviado = 'N';
                                    }
                                    else if (pedidoModel.Etiqueta == "Embalado")
                                    {
                                        etiquetagem.Embalado = 'S';
                                        etiquetagem.Enviado = 'N';
                                    }
                                    else
                                    {
                                        etiquetagem.Embalado = 'S';
                                        etiquetagem.Enviado = 'S';
                                    }
                                    etiquetagem.Atualizar(objDAL);
                                    if (etiquetagem.NrSeqEtiquetagem > 0)
                                    {
                                        StatusPedidoModel status = new StatusPedidoModel();
                                        status.NrSeqPedido = pedidoModel.NrSeqPedido;
                                        status.NrSeqEtiquetagem = etiquetagem.NrSeqEtiquetagem;
                                        if (pedidoModel.StatusPedido == "Aguardando")
                                        {
                                            status.Aprovado = 'N';
                                            status.Enviado = 'N';
                                            status.Entregue = 'N';
                                        }
                                        else if (pedidoModel.StatusPedido == "Aprovado")
                                        {
                                            status.Aprovado = 'S';
                                            status.Enviado = 'N';
                                            status.Entregue = 'N';
                                        }
                                        else if (pedidoModel.StatusPedido == "Enviado")
                                        {
                                            status.Aprovado = 'S';
                                            status.Enviado = 'S';
                                            status.Entregue = 'N';
                                        }
                                        else
                                        {
                                            status.Aprovado = 'S';
                                            status.Enviado = 'S';
                                            status.Entregue = 'S';
                                        }
                                        status.Atualizar(objDAL);
                                    }
                                    if (pedidoModel.TipoUsuario == "Cliente")
                                    {




                                        // Recuperar o objeto PessoaModelPesquisa da sessão
                                        string NrSeqUsuarioLogado = HttpContext.Session.GetString("NrSeqUsuarioLogado");

                                        UsuarioModel usuarioLogado = new UsuarioModel();
                                        usuarioLogado.NrSeqUsuario = Convert.ToInt32(NrSeqUsuarioLogado);
                                        usuarioLogado.CarregarDados();

                                        FuncionarioModel funcionario = new FuncionarioModel();
                                        funcionario.NrSeqPessoa = usuarioLogado.NrSeqPessoa;
                                        funcionario.CarregarDados();

                                        if (funcionario.NrSeqFuncionario > 0)
                                        {
                                            ComissaoModel com = new ComissaoModel();
                                            com.NrSeqPedido = pedidoModel.NrSeqPedido;
                                            com.BuscarPorNrSeqPedido(pedidoModel.NrSeqPedido);
                                            

                                            if(com.NrSeqComissao> 0)
                                            {
                                                ComissaoModel comissao = new ComissaoModel();
                                                comissao.NrSeqPedido = pedidoModel.NrSeqPedido;
                                                comissao.NrSeqProduto = pedidoModel.NrSeqProduto;
                                                comissao.NrSeqFuncionario = funcionario.NrSeqFuncionario;
                                                comissao.NomeFuncionario = funcionario.NomeDoFuncionario;
                                                comissao.DataCadastro = pedidoModel.DataCadastro;
                                                comissao.ValorVenda = (pedidoModel.Preco * pedidoModel.Quantidade);
                                                comissao.PorcentageComissao = 3.5M;
                                                comissao.ValorComissao = comissao.ValorVenda * (comissao.PorcentageComissao / 100);
                                                comissao.Atualizar(objDAL);
                                            }


                                        }


                                        ProdutoModel produto = new ProdutoModel();
                                        produto.NrSeqProduto = pedidoModel.NrSeqProduto;
                                        produto.CarregarDados();

                                        EstoqueModel estoque = new EstoqueModel();
                                        estoque.NomeDoProduto = produto.NomeDoProduto;
                                        estoque.Pesquisar();

                                        if (estoque.NrSeqEstoque > 0)
                                        {
                                            estoque.Quantidade = estoque.Quantidade - pedidoModel.Quantidade;
                                            estoque.Atualizar(objDAL);

                                            MovimentacaoModel movimentacao = new MovimentacaoModel();
                                            movimentacao.DataSaida = estoque.DataCadastro;
                                            movimentacao.Saida = 'S';
                                            movimentacao.Entrada = 'N';
                                            movimentacao.Quantidade = estoque.Quantidade;
                                            movimentacao.DataEntrada = DateTime.MinValue;
                                            movimentacao.NomePessoa = estoque.NomeDoFornecedor;
                                            movimentacao.ValorPago = estoque.PrecoUnitario;
                                            movimentacao.NrSeqPedido = pedidoModel.NrSeqPedido;
                                            movimentacao.NrSeqProduto = produto.NrSeqProduto;
                                            movimentacao.Marca = produto.Marca;
                                            movimentacao.NomeDoProduto = produto.NomeDoProduto;
                                            movimentacao.Cadastrar(objDAL);


                                        }
                                    }
                                }


                                objDAL.CommitTransaction();
                                TempData["MensagemSucesso"] = "Atualizado com sucesso!";

                            }
                            catch (Exception ex)
                            {
                                objDAL.RollbackTransaction();
                                TempData["MensagemErro"] = $"Erro: {ex.Message}";
                            }
                        }
                    }
              
                }
               
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro: {ex.Message}";
                return View("Manutencao", pedidoModel);
            }
            return  View("Manutencao", pedidoModel);
        }

        [HttpPost]
        public IActionResult AcaoDesejadaPedido(string acao, string IdsSelecionados, PedidoModel pedidoModel)
        {
            
            pedidoModel.ListaPedidos ??= new List<PedidoModel>();
            List<PedidoModel> data = new List<PedidoModel>();
            switch (acao)
            {

                case "Pesquisar":
                    data = pedidoModel.Pesquisar(pedidoModel);
                    pedidoModel.ListaPedidos = data;
                    break;
                case "Excluir":
                    if (pedidoModel.IdsSelecionados != null && pedidoModel.IdsSelecionados.Any())
                    {
                        foreach (int idSelecionado in pedidoModel.IdsSelecionados)
                        {
                            using (DAL objDAL = new DAL())
                            {
                                objDAL.OpenTransaction();


                                try
                                {
                                    StatusPedidoModel status = new StatusPedidoModel();
                                    status.NrSeqPedido= idSelecionado;
                                    status.ExcluirPorPedido(objDAL);

                                    EtiquetagemModel etiquetagem = new EtiquetagemModel();
                                    etiquetagem.NrSeqPedido= idSelecionado;
                                    etiquetagem.ExcluirPorPedido(objDAL);

                                    pedidoModel.NrSeqPedido = idSelecionado;
                                    if (pedidoModel.NrSeqPedido > 0) pedidoModel.Excluir(objDAL);

                                    
                                    objDAL.CommitTransaction();
                                    TempData["MensagemSucesso"] = "Excluido com sucesso!";

                                    data = pedidoModel.Pesquisar(pedidoModel);
                                    pedidoModel.ListaPedidos = data;

                                }
                                catch (Exception ex)
                                {
                                    objDAL.RollbackTransaction();
                                    TempData["MensagemErro"] = $"Erro: {ex.Message}";
                                }
                            }
                        }

                    }
                    break;
                case "Imprimir":
                    List<PedidoModel> lista = new List<PedidoModel>();
                    if (!string.IsNullOrEmpty(IdsSelecionados))
                    {
                        pedidoModel.AbaAtiva = true;
                        string[] idsArray = IdsSelecionados.Split(',');

                        foreach (var id in idsArray)
                        {
                            PedidoModel itempedido = new PedidoModel();
                            itempedido.NrSeqPedido = Convert.ToInt32(id);
                            itempedido.Carregar();
                            lista.Add(itempedido);
                        }
                        
                        try
                        {
                            MemoryStream file = new MemoryStream();
                            Document document = new Document(PageSize.A4, 25f, 25f, 15f, 35f);
                            PdfWriter writer = PdfWriter.GetInstance(document, file);
                            document.Open();
                            float[] columnWidths = { 0.6f, 1.8f, 1.2f, 1.2f, 1.8f };
                            BaseColor[] rowColors = { BaseColor.LIGHT_GRAY, BaseColor.WHITE };
                            PdfPTable table = new PdfPTable(columnWidths);
                            table.WidthPercentage = 100;

                                // Adicionar título à tabela
                                PdfPCell titleCell = new PdfPCell(new Phrase("Relatório de Pedidos", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD)));
                                titleCell.Colspan = columnWidths.Length;
                                titleCell.HorizontalAlignment = Element.ALIGN_CENTER;
                                titleCell.BackgroundColor = BaseColor.GRAY;
                                table.AddCell(titleCell);

                                // Adicionar cabeçalhos
                                foreach (var header in new[] { "Nr Pedido", "Nome Produto", "Preço", "Descrição", "Data Pedido" })
                                {
                                    PdfPCell headerCell = new PdfPCell(new Phrase(header, new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));
                                    headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                                    table.AddCell(headerCell);
                                }

                                // Adicionar dados
                                foreach (var pedido in lista)
                                {
                                    foreach (var property in new[] { pedido.NrSeqPedido.ToString(), pedido.NomeProduto, pedido.Preco.ToString(), pedido.Descricao, pedido.DataCadastro.ToString() })
                                    {
                                        PdfPCell dataCell = new PdfPCell(new Phrase(property, new Font(Font.FontFamily.HELVETICA, 10)));
                                        table.AddCell(dataCell);
                                    }
                                }

                                // Adicione a tabela ao documento
                                document.Add(table);

                                document.Close();
                                byte[] bytes = file.GetBuffer();

                            string base64 = Convert.ToBase64String(bytes);
                            pedidoModel.PdfBase64 = base64;

                        }
                        catch (Exception ex)
                        {
                            // Lidar com exceções, por exemplo, registrar ou retornar uma mensagem de erro
                            TempData["MensagemErro"] = $"Erro ao criar o arquivo PDF: {ex.Message}";
                        }
                    }

                    data = lista;
                    pedidoModel.ListaPedidos = data;
                    break;


            }
                
            return View("Index", pedidoModel);
        }
      
        public IActionResult OnLoadManutencao(int id, string modo)
        {
            PedidoModel pedido = new PedidoModel();
            if (modo == "edicao")
            {                              
                pedido.NrSeqPedido = id;
                pedido.Carregar();
                pedido.FlgEdicao = true;           
                return View("Manutencao", pedido);
            }
            else
            {
                pedido.FlgEdicao = false;
                pedido.NrSeqPedido = id;
                pedido.DataCadastro = DateTime.Now;
                pedido.DataVencimento = DateTime.Now;
                pedido.Preco = 0.00m;
                return View("Manutencao", pedido);
            }
        }
        [HttpGet]
        public JsonResult BuscarClientes(string termo)
        {
            ClienteModel lista = new ClienteModel();
            var clientes = lista.Pesquisar(lista)
                                .Select(cliente => new { value = cliente.NomeDoCliente, label = cliente.NomeDoCliente, nrSeqPessoaCli = cliente.NrSeqPessoa });

            return Json(clientes);
        }

        [HttpGet]
        public JsonResult BuscarFornecedores(string termo)
        {
            FornecedorModel fornecedorModel = new FornecedorModel();
            var fornecedores = fornecedorModel.Pesquisar(fornecedorModel)
                                              .Select(fornecedor => new { value = fornecedor.NomeDoFornecedor, label = fornecedor.NomeDoFornecedor, nrSeqPessoaFor = fornecedor.NrSeqPessoa });

            return Json(fornecedores);
        }

        [HttpGet]
        public JsonResult BuscarProduto(string termo)
        {
            ProdutoModel produtoModel = new ProdutoModel();
            var produtos = produtoModel.Pesquisar(produtoModel)
                .Select(produto => new {
                    value = produto.NomeDoProduto,
                    label = $"{produto.CodigoProduto} - {produto.NomeDoProduto}",
                    codigo = produto.CodigoProduto,
                    descricao = produto.Descricao,
                    preco = produto.Preco,
                    nrSeqProduto = produto.NrSeqProduto,
                    frete = ((produto.Preco * 0.08M) < 12.90M ? 12.90M : (produto.Preco * 0.08M)).ToString("F2")
                });
            return Json(produtos);
        }

    }
}
