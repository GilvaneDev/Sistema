using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Controllers;
using Sige_Erp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sige_Erp.Test.Tests
{
    public class ProdutoControllerTest
    {
        [Fact]
        public void Index_Returns_ViewResult_With_ProdutoModel()
        {
            // Arrange
            var controller = new ProdutoController();
            var produto = new ProdutoModel();

            // Act
            var result = controller.Index(produto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ProdutoModel>(viewResult.Model);
            Assert.False(model.AbaAtiva);
            Assert.NotNull(model.ListaProdutos);
        }

        [Fact]
        public void ValidacaoCadastro_Throws_Exception_For_Invalid_Data()
        {
            // Arrange
            var controller = new ProdutoController();
            var produto = new ProdutoModel
            {
                NomeDoProduto = "",
                Preco = -1,
                CodigoProduto = -1,
                Quantidade = -1,
                DataCadastro = DateTime.MinValue,
                Descricao = ""
            };

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => controller.ValidacaoCadastro(produto));
            Assert.Contains("Por favor, informe o nome do produto.", ex.Message);
        }

    
      

        [Fact]
        public void OnLoadManutencao_Edicao_Returns_Manutencao_View_With_ProdutoModel()
        {
            // Arrange
            var controller = new ProdutoController();
            int id = 1;
            string modo = "edicao";

            // Act
            var result = controller.OnLoadManutencao(id, modo);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ProdutoModel>(viewResult.Model);
            Assert.Equal(id, model.NrSeqProduto);
        }
    }
}
