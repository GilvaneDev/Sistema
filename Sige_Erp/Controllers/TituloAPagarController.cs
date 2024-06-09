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
    public class TituloAPagarController : Controller
    {
     
        public IActionResult Index(TituloAPagarModel titulo)
        {
            titulo.AbaAtiva = false;
            titulo.ListaTitulos = new List<TituloAPagarModel>();
            titulo.DeDataCadastro = null;
            titulo.AteDataCadastro = null;

            return View("Index", titulo);
        }
        [HttpPost]
        public IActionResult Pesquisar(TituloAPagarModel titulo)
        {
            // Simula uma pesquisa fictícia
            var data = new List<TituloAPagarModel>
            {
                new TituloAPagarModel { NrSeqNf = 1, NomeProduto = "Produto A", ValorTituloPagar = 100.00M },
                new TituloAPagarModel { NrSeqNf = 2, NomeProduto = "Produto B", ValorTituloPagar = 150.00M },
                new TituloAPagarModel { NrSeqNf = 3, NomeProduto = "Produto C", ValorTituloPagar = 200.00M }
            };

            // Aplica o filtro de pesquisa (simulado)
            if (!string.IsNullOrEmpty(titulo.NomeProduto))
            {
                data = data.Where(t => t.NomeProduto.Contains(titulo.NomeProduto, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            titulo.ListaTitulos = data;
            return View("Index", titulo);
        }

        public bool ValidacaoCadastro(TituloAPagarModel titulo)
        {
            if (string.IsNullOrEmpty(titulo.NomeProduto))
            {
                throw new Exception($"Por favor, informe o nome do produto.");
            }
            if (string.IsNullOrEmpty(titulo.NomeDoFornecedor))
            {
                throw new Exception($"Por favor, informe o Fornecedor.");
            }

            if (titulo.DataCadastro == DateTime.MinValue)
            {
                throw new Exception($"Por favor, informe a Data de Cadastro.");

            }

            if (string.IsNullOrEmpty(titulo.NrNf))
            {
                throw new Exception($"Por favor, informe o Nr do Documento.");

            }

            if (string.IsNullOrEmpty(titulo.Serie))
            {
                throw new Exception($"Por favor, informe a serie.");
            }


            if (titulo.ValorTituloPagar <= 0)
            {
                throw new Exception($"Por favor, informe o valor do titulo.");
            }         

            return true;
        }

        [HttpPost]
        public IActionResult CadastroTituloAPagarManutencao(TituloAPagarModel titulo)
        {
            try
            {
                if (titulo.NrSeqNf <= 0)
                {                                   
                    if (ValidacaoCadastro(titulo))
                    {
                        using (DAL objDAL = new DAL())
                        {
                            objDAL.OpenTransaction();

                            try
                            {

                                if(titulo.ValorPago <= 0)
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
                                if (titulo.NrSeqNf > 0)
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
        public IActionResult AcaoDesejadaTituloAPagar(string acao, string IdsSelecionados, TituloAPagarModel titulo)
        {

            titulo.ListaTitulos ??= new List<TituloAPagarModel>();
            List<TituloAPagarModel> data = new List<TituloAPagarModel>();
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


                                    titulo.NrSeqNf = idSelecionado;
                                    if (titulo.NrSeqNf > 0) titulo.Excluir(objDAL);


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
                    List<TituloAPagarModel> lista = new List<TituloAPagarModel>();
                    if (!string.IsNullOrEmpty(IdsSelecionados))
                    {
                        titulo.AbaAtiva = true;
                        string[] idsArray = IdsSelecionados.Split(',');

                        foreach (var id in idsArray)
                        {
                            TituloAPagarModel item = new TituloAPagarModel();
                            item.NrSeqNf = Convert.ToInt32(id);
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
                            PdfPCell titleCell = new PdfPCell(new Phrase("Relatório de Titulos A Pagar", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD)));
                            titleCell.Colspan = columnWidths.Length;
                            titleCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            titleCell.BackgroundColor = BaseColor.GRAY;
                            table.AddCell(titleCell);

                            // Adicionar cabeçalhos
                            foreach (var header in new[] { "Nr Documento", "Nome Produto", "Valor Titulo", "Fornecedor", "Data Cadastro" })
                            {
                                PdfPCell headerCell = new PdfPCell(new Phrase(header, new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));
                                headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                                table.AddCell(headerCell);
                            }

                            // Adicionar dados
                            foreach (var item in lista)
                            {
                                foreach (var property in new[] { item.NrNf.ToString(), item.NomeProduto, item.ValorTituloPagar.ToString(), item.NomeDoFornecedor, item.DataCadastro.ToString() })
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
            TituloAPagarModel titulo = new TituloAPagarModel();
            if (modo == "edicao")
            {
                titulo.NrSeqNf = id;
                titulo.CarregarDados();
                titulo.FlgEdicao = true;
                return View("Manutencao", titulo);
            }
            else
            {
                titulo.FlgEdicao = false;
                titulo.NrSeqNf = id;
                titulo.DataCadastro = DateTime.Now;
 
                return View("Manutencao", titulo);
            }
        }
        [HttpGet]
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
