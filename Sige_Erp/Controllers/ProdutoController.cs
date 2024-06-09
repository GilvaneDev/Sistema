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
    public class ProdutoController : Controller
    {
        public IActionResult Index(ProdutoModel produto)
        {
            produto.ListaProdutos = new List<ProdutoModel>();
            produto.AbaAtiva = false;
            produto.ateCadastro = DateTime.Now;
            produto.deCadastro = DateTime.Now;
           
            return View("Index", produto);
        }
        public bool ValidacaoCadastro(ProdutoModel produto)
        {
            if (string.IsNullOrEmpty(produto.NomeDoProduto))
            {
                throw new Exception($"Por favor, informe o nome do produto.");
            }

            if (produto.Preco <= 0)
            {
                throw new Exception($"Por favor, informe o Preço.");
               
            }

            if (produto.CodigoProduto <= 0)
            {
                throw new Exception($"Por favor, informe o codigo do produto.");
                
            }

            if (produto.Quantidade <= 0 && produto.NrSeqProduto > 0)
            {
                throw new Exception($"Por favor, informe a quantidade.");
                
            }


            if (produto.DataCadastro == DateTime.MinValue)
            {
                throw new Exception($"Por favor, informe a Data de cadastro.");
                
            }

            if (string.IsNullOrEmpty(produto.Descricao))
            {
                throw new Exception($"Por favor, coloque uma descrição.");
                
            }

            return true;
        }
        [HttpPost]
        public IActionResult CadastroProdutoManutencao(ProdutoModel produtoModel)
        {
            try
            {
                if (produtoModel.NrSeqProduto <= 0)
                {
                    if (ValidacaoCadastro(produtoModel))
                    {
                        using (DAL objDAL = new DAL())
                        {
                            objDAL.OpenTransaction();

                            try
                            {
                                produtoModel.Preco /= 100;
                                produtoModel.Cadastrar(objDAL);
                               
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
                        return View("Manutencao", produtoModel);
                    }
                }
                else
                {
                    if (ValidacaoCadastro(produtoModel))
                    {
                        using (DAL objDAL = new DAL())
                        {
                            objDAL.OpenTransaction();

                            try
                            {

                                    if (produtoModel.NrSeqProduto > 0) produtoModel.Atualizar(objDAL);
                                

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
            return View("Manutencao", produtoModel);
        }
        [HttpPost]
        public IActionResult AcaoDesejadaProduto(string acao, string IdsSelecionados, ProdutoModel produtoModel)
        {
            produtoModel ??= new ProdutoModel();
            produtoModel.ListaProdutos ??= new List<ProdutoModel>();
            List<ProdutoModel> data = new List<ProdutoModel>();
            switch (acao)
            {
                case "Pesquisar":
                    data = produtoModel.Pesquisar(produtoModel);
                    produtoModel.ListaProdutos = data;
                    break;
                case "Excluir":
                    if (produtoModel.IdsSelecionados != null && produtoModel.IdsSelecionados.Any())
                    {
                        foreach (int idSelecionado in produtoModel.IdsSelecionados)
                        {
                            using (DAL objDAL = new DAL())
                            {
                                objDAL.OpenTransaction();


                                try
                                {
                                    produtoModel.NrSeqProduto = idSelecionado;
                                    if (produtoModel.NrSeqProduto > 0) produtoModel.Excluir(objDAL);


                                    objDAL.CommitTransaction();
                                    TempData["MensagemSucesso"] = "Excluido com sucesso!";

                                    data = produtoModel.Pesquisar(produtoModel);
                                    produtoModel.ListaProdutos = data;

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
                    List<ProdutoModel> lista = new List<ProdutoModel>();
                    if (!string.IsNullOrEmpty(IdsSelecionados))
                    {
                        produtoModel.AbaAtiva = true;
                        string[] idsArray = IdsSelecionados.Split(',');

                        foreach (var id in idsArray)
                        {
                            ProdutoModel itempedido = new ProdutoModel();
                            itempedido.NrSeqProduto = Convert.ToInt32(id);
                            itempedido.Carregar();
                            lista.Add(itempedido);
                        }

                        
                        try
                        {


                            MemoryStream file = new MemoryStream();
                            Document document = new Document(PageSize.A4, 25f, 25f, 15f, 35f);
                            PdfWriter writer = PdfWriter.GetInstance(document, file);
                            document.Open();
                            float[] columnWidths = { 2f, 1.3f, 1.3f, 2f };
                            BaseColor[] rowColors = { BaseColor.LIGHT_GRAY, BaseColor.WHITE };
                            PdfPTable table = new PdfPTable(columnWidths);
                            table.WidthPercentage = 100;

                            // Adicionar título à tabela
                            PdfPCell titleCell = new PdfPCell(new Phrase("Relatório de Produtos", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD)));
                                titleCell.Colspan = columnWidths.Length;
                                titleCell.HorizontalAlignment = Element.ALIGN_CENTER;
                                titleCell.BackgroundColor = BaseColor.GRAY;
                                table.AddCell(titleCell);

                                // Adicionar cabeçalhos
                                foreach (var header in new[] { "Nome Produto", "Quantidade", "Preço", "Descriçao" })
                                {
                                    PdfPCell headerCell = new PdfPCell(new Phrase(header, new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));
                                    headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                                    table.AddCell(headerCell);
                                }

                                // Adicionar dados
                                foreach (var produto in lista)
                                {
                                    foreach (var property in new[] { produto.NomeDoProduto, produto.Quantidade.ToString(), produto.Preco.ToString(), produto.Descricao })
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
                            produtoModel.PdfBase64 = base64;

                        }
                        catch (Exception ex)
                        {
                            // Lidar com exceções, por exemplo, registrar ou retornar uma mensagem de erro
                            TempData["MensagemErro"] = $"Erro ao criar o arquivo PDF: {ex.Message}";
                        }
                    }

                    data = lista;
                    produtoModel.ListaProdutos = data;
                    break;


            }


            return View("Index", produtoModel);
        }

        public IActionResult OnLoadManutencao(int id, string modo)
        {
            if (modo == "edicao")
            {
                ProdutoModel produto = new ProdutoModel();
                produto.NrSeqProduto = id;
                produto.Carregar();
                return View("Manutencao", produto);
            }
            else
            {
                return View("Manutencao");
            }
        }


    }
}
