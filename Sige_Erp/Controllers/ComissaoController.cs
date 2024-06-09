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
    public class ComissaoController : Controller
    {
        public IActionResult Index(ComissaoModel comissao)
        {
            comissao.AbaAtiva = false;
            comissao.ListaComissao = new List<ComissaoModel>();
            comissao.deCadastro = null;
            comissao.ateCadastro = null;

            return View("Index", comissao);
        }
        [HttpPost]
        public IActionResult AcaoDesejadaComissao(string acao, string IdsSelecionados, ComissaoModel comissao)
        {

            comissao.ListaComissao ??= new List<ComissaoModel>();
            List<ComissaoModel> data = new List<ComissaoModel>();
            switch (acao)
            {

                case "Pesquisar":
                    data = comissao.Pesquisar(comissao);
                    comissao.ListaComissao = data;
                    break;
                case "Excluir":
                    if (comissao.IdsSelecionados != null && comissao.IdsSelecionados.Any())
                    {
                        foreach (int idSelecionado in comissao.IdsSelecionados)
                        {
                            using (DAL objDAL = new DAL())
                            {
                                objDAL.OpenTransaction();


                                try
                                {


                                    comissao.NrSeqComissao = idSelecionado;
                                    if (comissao.NrSeqComissao > 0) comissao.Excluir(objDAL);


                                    objDAL.CommitTransaction();
                                    TempData["MensagemSucesso"] = "Excluido com sucesso!";

                                    data = comissao.Pesquisar(comissao);
                                    comissao.ListaComissao = data;

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
                    List<ComissaoModel> lista = new List<ComissaoModel>();
                    if (!string.IsNullOrEmpty(IdsSelecionados))
                    {
                        comissao.AbaAtiva = true;
                        string[] idsArray = IdsSelecionados.Split(',');

                        foreach (var id in idsArray)
                        {
                            ComissaoModel item = new ComissaoModel();
                            item.NrSeqComissao = Convert.ToInt32(id);
                            item.CarregarDados();
                            lista.Add(item);
                        }

                        try
                        {
                            MemoryStream file = new MemoryStream();
                            Document document = new Document(PageSize.A4, 14.17f, 14.17f, 14.17f, 14.17f);
                            PdfWriter writer = PdfWriter.GetInstance(document, file);
                            document.Open();
                            float[] columnWidths = { 0.6f, 1.2f, 1.2f, 0.7f, 0.7f, 0.7f, 0.7f };
                            BaseColor[] rowColors = { BaseColor.LIGHT_GRAY, BaseColor.WHITE };
                            PdfPTable table = new PdfPTable(columnWidths);
                            table.WidthPercentage = 100;

                            // Adicionar título à tabela
                            PdfPCell titleCell = new PdfPCell(new Phrase("Relatório de Comissão", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));
                            titleCell.Colspan = columnWidths.Length;
                            titleCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            titleCell.BackgroundColor = BaseColor.GRAY;
                            table.AddCell(titleCell);

                            // Adicionar cabeçalhos
                            foreach (var header in new[] { "Nr Pedido", "Funcionario", "Produto", "Valor Venda", "Vlr Comissão", "Comissão", "Data Cad" })
                            {
                                PdfPCell headerCell = new PdfPCell(new Phrase(header, new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                                headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                                table.AddCell(headerCell);
                            }

                            // Adicionar dados
                            foreach (var item in lista)
                            {
                                table.AddCell(new PdfPCell(new Phrase(item.NrSeqPedido.ToString(), new Font(Font.FontFamily.HELVETICA, 8))));
                                table.AddCell(new PdfPCell(new Phrase(item.NomeFuncionario, new Font(Font.FontFamily.HELVETICA, 8))));
                                table.AddCell(new PdfPCell(new Phrase(item.NomeDoProduto, new Font(Font.FontFamily.HELVETICA, 8))));
                                table.AddCell(new PdfPCell(new Phrase(item.ValorVenda.ToString("F2"), new Font(Font.FontFamily.HELVETICA, 8))));
                                table.AddCell(new PdfPCell(new Phrase(item.ValorComissao.ToString("F2"), new Font(Font.FontFamily.HELVETICA, 8))));
                                table.AddCell(new PdfPCell(new Phrase(item.PorcentageComissao.ToString("P2"), new Font(Font.FontFamily.HELVETICA, 8))));
                                table.AddCell(new PdfPCell(new Phrase(item.DataCadastro.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.HELVETICA, 8))));
                            }

                            // Adicione a tabela ao documento
                            document.Add(table);
                            document.Close();

                            byte[] bytes = file.GetBuffer();
                            string base64 = Convert.ToBase64String(bytes);
                            comissao.PdfBase64 = base64;
                        }
                        catch (Exception ex)
                        {
                            // Lidar com exceções, por exemplo, registrar ou retornar uma mensagem de erro
                            TempData["MensagemErro"] = $"Erro ao criar o arquivo PDF: {ex.Message}";
                        }
                    }

                    data = lista;
                    comissao.ListaComissao = data;
                    break;



            }

            return View("Index", comissao);
        }

        [HttpGet]
        public JsonResult BuscarFuncionarios(string termo)
        {
            FuncionarioModel funcionarioModel = new FuncionarioModel();
            var funcionarios = funcionarioModel.Pesquisar(funcionarioModel)
                                              .Select(funcionario => new { value = funcionario.NomeDoFuncionario, label = funcionario.NomeDoFuncionario, nrSeqPessoa = funcionario.NrSeqPessoa, nrSeqFuncionario = funcionario.NrSeqFuncionario });

            return Json(funcionarios);
        }


        [HttpGet]
        public JsonResult BuscarProduto(string termo)
        {
            ProdutoModel produtoModel = new ProdutoModel();
            var produtos = produtoModel.Pesquisar(produtoModel)
                .Select(produto => new
                {
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
