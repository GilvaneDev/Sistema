// Exemplo de teste para o controlador TituloAPagarController
using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Controllers;
using Sige_Erp.Models;
using Xunit;

namespace Sige_Erp.Test.Tests
{
    public class TituloAPagarControllerTests
    {
        [Fact]
        public void Index_ReturnsEmptyList()
        {
            // Arrange
            var controller = new TituloAPagarController();
            var tituloModel = new TituloAPagarModel();

            // Act
            var result = controller.Index(tituloModel) as ViewResult;
            var model = result.ViewData.Model as TituloAPagarModel;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.Empty(model.ListaTitulos);
        }

        [Fact]
        public void Pesquisar_ReturnsFilteredResults()
        {
            // Arrange
            var controller = new TituloAPagarController();
            var tituloModel = new TituloAPagarModel { NomeProduto = "Produto A" };

            // Act
            var result = controller.Pesquisar(tituloModel) as ViewResult;
            var model = result.ViewData.Model as TituloAPagarModel;
            var resultList = model.ListaTitulos;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.NotEmpty(resultList);
            Assert.Single(resultList); // Verifica se há apenas um item retornado
            Assert.Equal("Produto A", resultList[0].NomeProduto); // Verifica se o produto retornado é o esperado
        }
    }
}
