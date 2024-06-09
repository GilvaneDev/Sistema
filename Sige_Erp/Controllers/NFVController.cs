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
    public class NFVController : Controller
    {

        public IActionResult Index(NFVModel nota)
        {
            nota.AbaAtiva = false;
            nota.ListaNotas = new List<NFVModel>();
            nota.deCadastro = null;
            nota.ateCadastro = null;

            return View("Index", nota);
        }

        public bool ValidacaoCadastro(NFVModel nota)
        {
            if (string.IsNullOrEmpty(nota.NomeDoProduto))
            {
                throw new Exception($"Por favor, informe o nome do produto.");
            }
            if (string.IsNullOrEmpty(nota.NomeDoCliente))
            {
                throw new Exception($"Por favor, informe o Cliente.");
            }

            if (nota.DataCadastro == DateTime.MinValue)
            {
                throw new Exception($"Por favor, informe a Data de Cadastro.");

            }
            if (nota.DataVencimento == DateTime.MinValue)
            {
                throw new Exception($"Por favor, informe a Data de Vencimento.");

            }
            if (nota.ValorImpostos <= 0)
            {
                throw new Exception($"Por favor, informe o Valor Do Imposto.");

            }

            if (string.IsNullOrEmpty(nota.NrNfv))
            {
                throw new Exception($"Por favor, informe o Nr do Documento.");

            }
            if (nota.NrSeqPedido <= 0)
            {
                throw new Exception($"Por favor, informe o Nr do Pedido.");

            }
            if (nota.Quantidade <= 0)
            {
                throw new Exception($"Por favor, informe a Quantidade.");

            }

            if (string.IsNullOrEmpty(nota.Serie))
            {
                throw new Exception($"Por favor, informe a serie.");
            }
            if (string.IsNullOrEmpty(nota.ChaveNfv))
            {
                throw new Exception($"Por favor, informe a ChaveNfv.");
            }


            if (nota.ValorTotal <= 0)
            {
                throw new Exception($"Por favor, informe o Valor Total.");
            }

            return true;
        }

        [HttpPost]
        public IActionResult CadastroNFVManutencao(NFVModel nota)
        {
            try
            {
                if (nota.NrSeqNfv <= 0)
                {
                    if (ValidacaoCadastro(nota))
                    {
                        using (DAL objDAL = new DAL())
                        {
                            objDAL.OpenTransaction();

                            try
                            {
                                decimal porcentagemImposto = Math.Round((nota.ValorImpostos / nota.ValorTotal) * 100, 2);
                                if (nota.NrSeqProduto > 0)
                                {
                                    ImpostoModel imposto = new ImpostoModel();
                                    imposto.NrSeqProduto = nota.NrSeqProduto;
                                    imposto.ValorTotalImposto = nota.ValorImpostos;
                                    imposto.PorcentageImposto = porcentagemImposto;
                                    imposto.Cadastrar(objDAL);

                                    if (imposto.NrSeqImposto > 0)
                                    {
                                        nota.NrSeqImpostos = imposto.NrSeqImposto;
                                        nota.Cadastrar(objDAL);
                                    }


                                }
                                else
                                {
                                    throw new Exception($"Erro ao Identificar Produto.");
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
                        return View("Manutencao", nota);
                    }
                }
                else
                {
                    if (ValidacaoCadastro(nota))
                    {
                        using (DAL objDAL = new DAL())
                        {
                            objDAL.OpenTransaction();

                            try
                            {
                                decimal porcentagemImposto = Math.Round((nota.ValorImpostos / nota.ValorTotal) * 100, 2);
                                if (nota.NrSeqProduto > 0 && nota.NrSeqImpostos > 0)
                                {
                                    ImpostoModel imposto = new ImpostoModel();
                                    imposto.NrSeqProduto = nota.NrSeqProduto;
                                    imposto.NrSeqImposto = nota.NrSeqImpostos;
                                    imposto.ValorTotalImposto = nota.ValorImpostos;
                                    imposto.PorcentageImposto = porcentagemImposto;
                                    imposto.Atualizar(objDAL); 
                                    
                                    nota.Atualizar(objDAL);
                                    
                                }
                                else
                                {
                                    throw new Exception($"Erro ao Identificar o Produto e impostos.");
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
            return View("Manutencao", nota);
        }

        [HttpPost]
        public IActionResult AcaoDesejadaNFV(string acao, string IdsSelecionados, NFVModel nota)
        {

            nota.ListaNotas ??= new List<NFVModel>();
            List<NFVModel> data = new List<NFVModel>();
            switch (acao)
            {

                case "Pesquisar":
                    data = nota.Pesquisar(nota);
                    nota.ListaNotas = data;
                    break;
                case "Excluir":
                    if (nota.IdsSelecionados != null && nota.IdsSelecionados.Any())
                    {
                        foreach (int idSelecionado in nota.IdsSelecionados)
                        {
                            using (DAL objDAL = new DAL())
                            {
                                objDAL.OpenTransaction();


                                try
                                {


                                    nota.NrSeqNfv = idSelecionado;
                                    if (nota.NrSeqNfv > 0) nota.Excluir(objDAL);


                                    objDAL.CommitTransaction();
                                    TempData["MensagemSucesso"] = "Excluido com sucesso!";

                                    data = nota.Pesquisar(nota);
                                    nota.ListaNotas = data;

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
                    List<NFVModel> lista = new List<NFVModel>();
                    if (!string.IsNullOrEmpty(IdsSelecionados))
                    {
                        try
                        {
                            string[] idsArray = IdsSelecionados.Split(',');
                            if (idsArray.Length > 1)
                            {
                                throw new Exception("Mais de um item foi selecionado. Selecione apenas um item.");
                            }
                            else
                            {
                                try
                                {
                                    nota.AbaAtiva = true;
                                    foreach (var id in idsArray)
                                    {
                                        NFVModel item = new NFVModel();
                                        item.NrSeqNfv = Convert.ToInt32(id);
                                        item.CarregarDados();
                                        lista.Add(item);
                                    }

                                    MemoryStream file = new MemoryStream();
                                    Document document = new Document(PageSize.A4, 25f, 25f, 15f, 35f);
                                    PdfWriter writer = PdfWriter.GetInstance(document, file);
                                    document.Open();

                                    // Fonte padrão
                                    Font fontePadrao = new Font(Font.FontFamily.HELVETICA, 10);
                                    Font fonteNegrito = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD);
                                    Font fonteTitulo = new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD);

                                    // Cabeçalho do documento
                                    PdfPTable tableHeader = new PdfPTable(2);
                                    tableHeader.WidthPercentage = 100;
                                    tableHeader.SetWidths(new float[] { 1, 2 });

                                    // Adicionando logo da empresa (se tiver)
                                    //Image logo = Image.GetInstance("path/to/logo.png");
                                    //logo.ScaleToFit(100f, 100f);
                                    //PdfPCell logoCell = new PdfPCell(logo);
                                    //logoCell.Border = PdfPCell.NO_BORDER;
                                    //tableHeader.AddCell(logoCell);

                                    // Informação da empresa
                                    PdfPCell empresaCell = new PdfPCell(new Phrase("Sige - Sistemas De Vendas \nRua Marinheiro João Candido, Nr: 085, Dom Feliciano - RS\nTelefone: (51) 99893-8601\nEmail: sistemadevendas@sige.com", fonteNegrito));
                                    empresaCell.Border = PdfPCell.NO_BORDER;
                                    tableHeader.AddCell(empresaCell);

                                    // Informação do documento
                                    PdfPCell tituloCell = new PdfPCell(new Phrase("Nota Fiscal", fonteTitulo));
                                    tituloCell.Colspan = 2;
                                    tituloCell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    tituloCell.Border = PdfPCell.NO_BORDER;
                                    tituloCell.PaddingTop = 10f;
                                    tituloCell.PaddingBottom = 10f;
                                    tableHeader.AddCell(tituloCell);

                                    document.Add(tableHeader);

                                    // Informações do cliente e da nota fiscal
                                    PdfPTable tableInfo = new PdfPTable(2);
                                    tableInfo.WidthPercentage = 100;
                                    tableInfo.SetWidths(new float[] { 1, 2 });

                                    // Dados do cliente
                                    PdfPCell clienteCell = new PdfPCell(new Phrase($"Cliente: {lista[0].NomeDoCliente}\nEndereço: {lista[0].EnderecoCliente}\nCNPJ: {lista[0].CnpjCliente}", fontePadrao));
                                    clienteCell.Border = PdfPCell.NO_BORDER;
                                    tableInfo.AddCell(clienteCell);

                                    // Dados da nota fiscal
                                    PdfPCell nfCell = new PdfPCell(new Phrase($"Número da NFV: {lista[0].NrNfv}\nSérie: {lista[0].Serie}\nData de Emissão: {lista[0].DataCadastro:dd/MM/yyyy}", fontePadrao));
                                    nfCell.Border = PdfPCell.NO_BORDER;
                                    tableInfo.AddCell(nfCell);

                                    document.Add(tableInfo);
                                    // Linha divisória
                                    document.Add(new Paragraph(" "));

                                    // Tabela de itens
                                    PdfPTable tableItens = new PdfPTable(new float[] { 0.5f, 1.6f, 1.1f, 1.9f, 1.1f });
                                    tableItens.WidthPercentage = 100;

                                    // Cabeçalho da tabela de itens
                                    foreach (var header in new[] { "Nr Nf", "Nr Pedido", "Cliente", "Produto", "Quantidade", "Valor Total", "Imposto", "Frete", "Data Cadastro" })
                                    {
                                        PdfPCell headerCell = new PdfPCell(new Phrase(header, fonteNegrito));
                                        headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                                        headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                                        tableItens.AddCell(headerCell);
                                    }

                                    // Dados dos itens
                                    foreach (var item in lista)
                                    {
                                        foreach (var property in new[] { item.NrNfv, item.NrSeqPedido.ToString(), item.NomeDoCliente, item.NomeDoProduto, item.Quantidade.ToString(), item.ValorTotal.ToString(), item.ValorImpostos.ToString(), item.ValorFrete.ToString(), item.DataCadastro.ToString("dd/MM/yyyy") })
                                        {
                                            PdfPCell dataCell = new PdfPCell(new Phrase(property, fontePadrao));
                                            dataCell.HorizontalAlignment = Element.ALIGN_CENTER;
                                            tableItens.AddCell(dataCell);
                                        }
                                    }

                                    document.Add(tableItens);

                                    // Linha divisória
                                    document.Add(new Paragraph(" "));

                                    // Totais
                                    PdfPTable tableTotal = new PdfPTable(2);
                                    tableTotal.WidthPercentage = 100;
                                    tableTotal.SetWidths(new float[] { 1, 2 });

                                    decimal totalValor = lista.Sum(x => x.ValorTotal);
                                    decimal totalImpostos = lista.Sum(x => x.ValorImpostos);
                                    decimal totalFrete = lista.Sum(x => x.ValorFrete);

                                    PdfPCell totalLabelCell = new PdfPCell(new Phrase("Total", fonteNegrito));
                                    totalLabelCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                    totalLabelCell.Border = PdfPCell.NO_BORDER;
                                    tableTotal.AddCell(totalLabelCell);

                                    PdfPCell totalValueCell = new PdfPCell(new Phrase($"{totalValor:C}", fontePadrao));
                                    totalValueCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                    totalValueCell.Border = PdfPCell.NO_BORDER;
                                    tableTotal.AddCell(totalValueCell);

                                    PdfPCell impostosLabelCell = new PdfPCell(new Phrase("Total de Impostos", fonteNegrito));
                                    impostosLabelCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                    impostosLabelCell.Border = PdfPCell.NO_BORDER;
                                    tableTotal.AddCell(impostosLabelCell);

                                    PdfPCell impostosValueCell = new PdfPCell(new Phrase($"{totalImpostos:C}", fontePadrao));
                                    impostosValueCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                    impostosValueCell.Border = PdfPCell.NO_BORDER;
                                    tableTotal.AddCell(impostosValueCell);

                                    PdfPCell freteLabelCell = new PdfPCell(new Phrase("Total de Frete", fonteNegrito));
                                    freteLabelCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                    freteLabelCell.Border = PdfPCell.NO_BORDER;
                                    tableTotal.AddCell(freteLabelCell);

                                    PdfPCell freteValueCell = new PdfPCell(new Phrase($"{totalFrete:C}", fontePadrao));
                                    freteValueCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                    freteValueCell.Border = PdfPCell.NO_BORDER;
                                    tableTotal.AddCell(freteValueCell);

                                    document.Add(tableTotal);

                                    document.Close();
                                    byte[] bytes = file.GetBuffer();

                                    string base64 = Convert.ToBase64String(bytes);
                                    nota.PdfBase64 = base64;


                                }
                                catch (Exception ex)
                                {
                                    // Lidar com exceções, por exemplo, registrar ou retornar uma mensagem de erro
                                    TempData["MensagemErro"] = $"Erro ao criar o arquivo PDF: {ex.Message}";
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Lidar com exceções, por exemplo, registrar ou retornar uma mensagem de erro
                            TempData["MensagemErro"] = $"Erro ao criar o arquivo PDF: {ex.Message}";
                        }


                    }

                    data = lista;
                    nota.ListaNotas = data;
                    break;




            }

            return View("Index", nota);
        }

        public IActionResult OnLoadManutencao(int id, string modo)
        {
            NFVModel nota = new NFVModel();
            if (modo == "edicao")
            {
                nota.NrSeqNfv = id;
                nota.CarregarDados();
                nota.FlgEdicao = true;
                return View("Manutencao", nota);
            }
            else
            {
                nota.FlgEdicao = false;
                nota.NrSeqNfv = id;
                nota.DataCadastro = DateTime.Now;
                nota.DataVencimento = nota.DataCadastro.AddDays(30);

                return View("Manutencao", nota);
            }
        }
        [HttpGet]
        public JsonResult BuscarClientes(string termo)
        {
            ClienteModel clienteModel = new ClienteModel();
            var clientes = clienteModel.Pesquisar(clienteModel)
                                              .Select(cliente => new { value = cliente.NomeDoCliente, label = cliente.NomeDoCliente, nrSeqPessoa = cliente.NrSeqPessoa, nrSeqCliente = cliente.NrSeqCliente });

            return Json(clientes);
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
