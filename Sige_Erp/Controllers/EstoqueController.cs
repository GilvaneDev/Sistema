using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Models;
using Sige_Erp.Uteis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sige_Erp.Controllers
{
    public class EstoqueController : Controller
    {
        public IActionResult Index(EstoqueModel estoque)
        {

            estoque.AbaAtiva = false;
            estoque.ListaEstoques = new List<EstoqueModel>();
            estoque.ateCadastro = null;
            estoque.deCadastro = null;
            estoque.DTEntrada = null;
            estoque.DTSaida = null;

            return View("Index", estoque);
        }

        public bool ValidacaoCadastro(EstoqueModel estoque)
        {
            if (string.IsNullOrEmpty(estoque.NomeDoProduto))
            {
                throw new Exception($"Por favor, informe o nome do produto.");
                
            }

            if (estoque.PrecoUnitario <= 0)
            {
                throw new Exception($"Por favor, informe o preco unitario.");
                
            }

            if (estoque.Quantidade <= 0)
            {
                throw new Exception($"Por favor, informe a quantidade.");
               
            }

            if (string.IsNullOrEmpty(estoque.NomeDoFornecedor))
            {
                throw new Exception($"Por favor, informe Fornecedor.");
               
            }

            if (estoque.DataCadastro == DateTime.MinValue)
            {
                throw new Exception($"Por favor, informe a Data de cadastro.");
               
            }


            return true;
        }

        [HttpPost]
        public IActionResult CadastroEstoqueManutencao(EstoqueModel estoqueModel)
        {
            try
            {
                if (estoqueModel.NrSeqEstoque <= 0)
                {
                    estoqueModel.NrSeqPedido = 3;
                    if (ValidacaoCadastro(estoqueModel))
                    {
                        using (DAL objDAL = new DAL())
                        {
                            objDAL.OpenTransaction();

                            try
                            {
                                if(estoqueModel.NrSeqFornecedor > 0)
                                {
                                    estoqueModel.PrecoUnitario /= 100;
                                    estoqueModel.Cadastrar(objDAL);
                                    if (estoqueModel.NrSeqProduto > 0)
                                    {

                                        ProdutoModel produto = new ProdutoModel();
                                        produto.NrSeqProduto = estoqueModel.NrSeqProduto;
                                        produto.CarregarDados();

                                        MovimentacaoModel movimentacao = new MovimentacaoModel();
                                        movimentacao.DataSaida = DateTime.MinValue;
                                        movimentacao.Saida = 'N';
                                        movimentacao.Entrada = 'S';
                                        movimentacao.Quantidade = estoqueModel.Quantidade;
                                        movimentacao.DataEntrada = estoqueModel.DataCadastro;
                                        movimentacao.NomePessoa = estoqueModel.NomeDoFornecedor;
                                        movimentacao.ValorPago = estoqueModel.PrecoUnitario;
                                        movimentacao.NrSeqPedido = estoqueModel.NrSeqPedido;
                                        movimentacao.NrSeqProduto = produto.NrSeqProduto;
                                        movimentacao.Marca= produto.Marca;
                                        movimentacao.NomeDoProduto = produto.NomeDoProduto;
                                        movimentacao.Cadastrar(objDAL);
                                    }
                                    else
                                    {
                                        TempData["MensagemErro"] = "Houve um erro ao cadastrar. Tente novamente.";
                                    }
                                }
                                else
                                {
                                    TempData["MensagemErro"] = "Houve um erro ao cadastrar. Tente novamente.";
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
                        return View("Manutencao", estoqueModel);
                    }
                }
                else
                {
                    if (ValidacaoCadastro(estoqueModel))
                    {
                        using (DAL objDAL = new DAL())
                        {
                            objDAL.OpenTransaction();

                            try
                            {
                                if (estoqueModel.NrSeqFornecedor > 0)
                                {
                                    estoqueModel.PrecoUnitario /= 100;
                                    estoqueModel.Atualizar(objDAL);
                                    if (estoqueModel.NrSeqProduto > 0)
                                    {

                                        ProdutoModel produto = new ProdutoModel();
                                        produto.NrSeqProduto = estoqueModel.NrSeqProduto;
                                        produto.CarregarDados();

                                        MovimentacaoModel movimentacao = new MovimentacaoModel();
                                        movimentacao.DataSaida = DateTime.MinValue;
                                        movimentacao.Saida = 'N';
                                        movimentacao.Entrada = 'S';
                                        movimentacao.Quantidade = estoqueModel.Quantidade;
                                        movimentacao.DataEntrada = estoqueModel.DataCadastro;
                                        movimentacao.NomePessoa = estoqueModel.NomeDoFornecedor;
                                        movimentacao.ValorPago = estoqueModel.PrecoUnitario;
                                        movimentacao.NrSeqPedido = estoqueModel.NrSeqPedido;
                                        movimentacao.NrSeqProduto = produto.NrSeqProduto;
                                        movimentacao.Marca = produto.Marca;
                                        movimentacao.NomeDoProduto = produto.NomeDoProduto;
                                        movimentacao.Cadastrar(objDAL);
                                    }
                                    else
                                    {
                                        TempData["MensagemErro"] = "Houve um erro ao cadastrar. Tente novamente.";
                                    }
                                }
                                else
                                {
                                    TempData["MensagemErro"] = "Houve um erro ao cadastrar. Tente novamente.";
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
            }
            return View("Manutencao", estoqueModel);
        }


        [HttpPost]
        public IActionResult AcaoDesejadaEstoque(string acao, string IdsSelecionados, EstoqueModel estoqueModel)
        {

            estoqueModel ??= new EstoqueModel();
            estoqueModel.ListaEstoques ??= new List<EstoqueModel>();
            List<EstoqueModel> data = new List<EstoqueModel>();
            switch (acao)
            {
                case "Pesquisar":
                    data = estoqueModel.Pesquisar(estoqueModel);
                    estoqueModel.ListaEstoques = data;
                    break;
                case "Excluir":
                    if (estoqueModel.IdsSelecionados != null && estoqueModel.IdsSelecionados.Any())
                    {
                        foreach (int idSelecionado in estoqueModel.IdsSelecionados)
                        {
                            using (DAL objDAL = new DAL())
                            {
                                objDAL.OpenTransaction();


                                try
                                {
                                    
                                    if (estoqueModel.CheckboxMovimento)
                                    {
                                        MovimentacaoModel movimentacao = new MovimentacaoModel();
                                        movimentacao.NrSeqPedido = idSelecionado;
                                        movimentacao.Excluir(objDAL);
                                    }
                                    else
                                    {
                                        estoqueModel.NrSeqEstoque = idSelecionado;
                                        if (estoqueModel.NrSeqEstoque > 0) estoqueModel.Excluir(objDAL);
                                    }
                                    


                                    objDAL.CommitTransaction();
                                    TempData["MensagemSucesso"] = "Excluido com sucesso!";

                                    data = estoqueModel.Pesquisar(estoqueModel);
                                    estoqueModel.ListaEstoques = data;

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
                    List<EstoqueModel> lista = new List<EstoqueModel>();
                    if (!string.IsNullOrEmpty(IdsSelecionados))
                    {
                        string[] idsArray = IdsSelecionados.Split(',');
                        estoqueModel.AbaAtiva = true;

                        foreach (var id in idsArray)
                        {
                            EstoqueModel itempedido = new EstoqueModel();
                            if (estoqueModel.CheckboxMovimento)
                            {
                                itempedido.CheckboxMovimento = estoqueModel.CheckboxMovimento;
                                itempedido.CheckboxEntrada = estoqueModel.CheckboxEntrada;
                                itempedido.CheckboxSaida = estoqueModel.CheckboxSaida;
                                itempedido.NrSeqPedido = Convert.ToInt32(id);
                                itempedido.Pesquisa(itempedido);
                            }
                            else
                            {
                                itempedido.NrSeqEstoque = Convert.ToInt32(id);
                                itempedido.Carregar();
                            }
                            
                            lista.Add(itempedido);
                        }
                       
                        try
                        {

                            MemoryStream file = new MemoryStream();
                            Document document = new Document(PageSize.A4, 25f, 25f, 15f, 35f);
                            PdfWriter writer = PdfWriter.GetInstance(document, file);
                            document.Open();
                            float[] columnWidths = {  2f, 1.6f, 1.6f, 2f };
                            BaseColor[] rowColors = { BaseColor.LIGHT_GRAY, BaseColor.WHITE };
                            PdfPTable table = new PdfPTable(columnWidths);
                            table.WidthPercentage = 100;

                            // Adicionar título à tabela
                            PdfPCell titleCell = new PdfPCell(new Phrase("Relatório de Estoques", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD)));
                                titleCell.Colspan = columnWidths.Length;
                                titleCell.HorizontalAlignment = Element.ALIGN_CENTER;
                                titleCell.BackgroundColor = BaseColor.GRAY;
                                table.AddCell(titleCell);

                                // Adicionar cabeçalhos
                                foreach (var header in new[] { "Nome Produto", "Preço", "Marca", "Data Cadastro" })
                                {
                                    PdfPCell headerCell = new PdfPCell(new Phrase(header, new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));
                                    headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                                    table.AddCell(headerCell);
                                }

                                // Adicionar dados
                                foreach (var pedido in lista)
                                {
                                    foreach (var property in new[] { pedido.NomeDoProduto, pedido.PrecoUnitario.ToString(), pedido.Marca, pedido.DataCadastro.ToString() })
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
                            estoqueModel.PdfBase64 = base64;

                        }
                        catch (Exception ex)
                        {
                            // Lidar com exceções, por exemplo, registrar ou retornar uma mensagem de erro
                            TempData["MensagemErro"] = $"Erro ao criar o arquivo PDF: {ex.Message}";
                        }
                    }

                    data = lista;
                    estoqueModel.ListaEstoques = data;
                    break;


            }         

            return View("Index", estoqueModel);
        }


        public IActionResult OnLoadManutencao(int id, string modo)
        {
            EstoqueModel estoque = new EstoqueModel();
            string viewretorno = "Manutencao";

            try
            {
                if (modo == "edicao")
                {
                    if (id <= 0)
                    {
                        TempData["MensagemErro"] = "Não é permitido a Edição de movimentação.";
                        return View("Index", estoque); // Retorna para Index se o ID for inválido
                    }

                    estoque.NrSeqEstoque = id;
                    estoque.Carregar(); // Carrega os dados do estoque

                    if (estoque.NrSeqEstoque == 0) // Verifica se o estoque não foi carregado corretamente
                    {
                        TempData["MensagemErro"] = "Estoque não encontrado.";
                        return View("Index", estoque); // Retorna para Index se o estoque não for encontrado
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro: {ex.Message}";
            }

            return View(viewretorno, estoque); // Retorna a view de Manutencao com os dados do estoque carregado
        }





        [HttpGet]
        public JsonResult BuscarFornecedores(string termo)
        {
            FornecedorModel fornecedorModel = new FornecedorModel();
            var fornecedores = fornecedorModel.Pesquisar(fornecedorModel)
                                              .Select(fornecedor => new { value = fornecedor.NomeDoFornecedor, label = fornecedor.NomeDoFornecedor, nrSeqFornecedor = fornecedor.NrSeqFornecedor });

            return Json(fornecedores);
        }

        [HttpGet]
        public JsonResult BuscarProduto(string termo)
        {
            ProdutoModel produtoModel = new ProdutoModel();
            var produtos = produtoModel.Pesquisar(produtoModel)
                .Select(produto => new {
                    value = produto.NomeDoProduto,
                    label = produto.NomeDoProduto,
                    preco = produto.Preco,
                    nrSeqProduto = produto.NrSeqProduto,
                    marca = produto.Marca
                });
            return Json(produtos);
        }


    }
}
