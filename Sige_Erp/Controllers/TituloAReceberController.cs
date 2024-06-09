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
    public class TituloAReceberController : Controller
    {

        public IActionResult Index(TituloAReceberModel titulo)
        {
            titulo.AbaAtiva = false;
            titulo.ListaTitulos = new List<TituloAReceberModel>();
            titulo.DeDataCadastro = null;
            titulo.AteDataCadastro = null;

            return View("Index", titulo);
        }
        [HttpPost]
        public IActionResult Pesquisar(TituloAReceberModel titulo)
        {
            // Simula uma pesquisa fictícia
            var data = new List<TituloAReceberModel>
            {
                new TituloAReceberModel { NrSeqNfv = 1, NomeProduto = "Produto A", ValorTituloReceber = 100.00M },
                new TituloAReceberModel { NrSeqNfv = 2, NomeProduto = "Produto B", ValorTituloReceber = 150.00M },
                new TituloAReceberModel { NrSeqNfv = 3, NomeProduto = "Produto C", ValorTituloReceber = 200.00M }
            };

            // Aplica o filtro de pesquisa (simulado)
            if (!string.IsNullOrEmpty(titulo.NomeProduto))
            {
                data = data.Where(t => t.NomeProduto.Contains(titulo.NomeProduto, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            titulo.ListaTitulos = data;
            return View("Index", titulo);
        }

        public bool ValidacaoCadastro(TituloAReceberModel titulo)
        {
            if (string.IsNullOrEmpty(titulo.NomeProduto))
            {
                throw new Exception($"Por favor, informe o nome do produto.");
            }
            if (string.IsNullOrEmpty(titulo.NomeDoCliente))
            {
                throw new Exception($"Por favor, informe o Cliente.");
            }

            if (titulo.DataCadastro == DateTime.MinValue)
            {
                throw new Exception($"Por favor, informe a Data de Cadastro.");

            }

            if (string.IsNullOrEmpty(titulo.NrNfv))
            {
                throw new Exception($"Por favor, informe o Nr do Documento.");

            }

            if (string.IsNullOrEmpty(titulo.Serie))
            {
                throw new Exception($"Por favor, informe a serie.");
            }


            if (titulo.ValorTituloReceber <= 0)
            {
                throw new Exception($"Por favor, informe o valor do titulo.");
            }

            return true;
        }

        [HttpPost]
        public IActionResult CadastroTituloAReceberManutencao(TituloAReceberModel titulo)
        {
            try
            {
                if (titulo.NrSeqNfv <= 0)
                {
                    if (ValidacaoCadastro(titulo))
                    {
                        using (DAL objDAL = new DAL())
                        {
                            objDAL.OpenTransaction();

                            try
                            {

                                if (titulo.ValorPago <= 0)
                                {
                                    titulo.ValorPago = 0;
                                }
                                titulo.Cadastrar(objDAL);

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
                        return View("Manutencao", titulo);
                    }
                }
                else
                {
                    if (ValidacaoCadastro(titulo))
                    {
                        using (DAL objDAL = new DAL())
                        {
                            objDAL.OpenTransaction();

                            try
                            {
                                if (titulo.NrSeqNfv > 0)
                                {
                                    if (titulo.ValorPago <= 0)
                                    {
                                        titulo.ValorPago = 0;
                                    }
                                    titulo.Atualizar(objDAL);
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
            return View("Manutencao", titulo);
        }

        [HttpPost]
        public IActionResult AcaoDesejadaTituloAReceber(string acao, string IdsSelecionados, TituloAReceberModel titulo)
        {

            titulo.ListaTitulos ??= new List<TituloAReceberModel>();
            List<TituloAReceberModel> data = new List<TituloAReceberModel>();
            switch (acao)
            {

                case "Pesquisar":
                    data = titulo.Pesquisar(titulo);
                    titulo.ListaTitulos = data;
                    break;
                case "Excluir":
                    if (titulo.IdsSelecionados != null && titulo.IdsSelecionados.Any())
                    {
                        foreach (int idSelecionado in titulo.IdsSelecionados)
                        {
                            using (DAL objDAL = new DAL())
                            {
                                objDAL.OpenTransaction();


                                try
                                {


                                    titulo.NrSeqNfv = idSelecionado;
                                    if (titulo.NrSeqNfv > 0) titulo.Excluir(objDAL);


                                    objDAL.CommitTransaction();
                                    TempData["MensagemSucesso"] = "Excluido com sucesso!";

                                    data = titulo.Pesquisar(titulo);
                                    titulo.ListaTitulos = data;

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
                    List<TituloAReceberModel> lista = new List<TituloAReceberModel>();
                    if (!string.IsNullOrEmpty(IdsSelecionados))
                    {
                        titulo.AbaAtiva = true;
                        string[] idsArray = IdsSelecionados.Split(',');

                        foreach (var id in idsArray)
                        {
                            TituloAReceberModel item = new TituloAReceberModel();
                            item.NrSeqNfv = Convert.ToInt32(id);
                            item.CarregarDados();
                            lista.Add(item);
                        }

                        try
                        {
                            MemoryStream file = new MemoryStream();
                            Document document = new Document(PageSize.A4, 25f, 25f, 15f, 35f);
                            PdfWriter writer = PdfWriter.GetInstance(document, file);
                            document.Open();
                            float[] columnWidths = { 0.5f, 1.6f, 1.1f, 1.9f, 1.1f };
                            BaseColor[] rowColors = { BaseColor.LIGHT_GRAY, BaseColor.WHITE };
                            PdfPTable table = new PdfPTable(columnWidths);
                            table.WidthPercentage = 100;

                            // Adicionar título à tabela
                            PdfPCell titleCell = new PdfPCell(new Phrase("Relatório de Titulos A Receber", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD)));
                            titleCell.Colspan = columnWidths.Length;
                            titleCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            titleCell.BackgroundColor = BaseColor.GRAY;
                            table.AddCell(titleCell);

                            // Adicionar cabeçalhos
                            foreach (var header in new[] { "Nr Documento", "Nome Produto", "Valor Titulo", "Cliente", "Data Cadastro" })
                            {
                                PdfPCell headerCell = new PdfPCell(new Phrase(header, new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));
                                headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                                table.AddCell(headerCell);
                            }

                            // Adicionar dados
                            foreach (var item in lista)
                            {
                                foreach (var property in new[] { item.NrNfv.ToString(), item.NomeProduto, item.ValorTituloReceber.ToString(), item.NomeDoCliente, item.DataCadastro.ToString() })
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
                            titulo.PdfBase64 = base64;

                        }
                        catch (Exception ex)
                        {
                            // Lidar com exceções, por exemplo, registrar ou retornar uma mensagem de erro
                            TempData["MensagemErro"] = $"Erro ao criar o arquivo PDF: {ex.Message}";
                        }
                    }

                    data = lista;
                    titulo.ListaTitulos = data;
                    break;


            }

            return View("Index", titulo);
        }

        public IActionResult OnLoadManutencao(int id, string modo)
        {
            TituloAReceberModel titulo = new TituloAReceberModel();
            if (modo == "edicao")
            {
                titulo.NrSeqNfv = id;
                titulo.CarregarDados();
                titulo.FlgEdicao = true;
                return View("Manutencao", titulo);
            }
            else
            {
                titulo.FlgEdicao = false;
                titulo.NrSeqNfv = id;
                titulo.DataCadastro = DateTime.Now;

                return View("Manutencao", titulo);
            }
        }
       
        [HttpGet]
        public JsonResult BuscarFuncionarios(string termo)
        {
            ClienteModel clienteModel = new ClienteModel();
            var clientes = clienteModel.Pesquisar(clienteModel)
                                              .Select(cliente => new { value = cliente.NomeDoCliente, label = cliente.NomeDoCliente, nrSeqPessoaCli = cliente.NrSeqPessoa });

            return Json(clientes);
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
