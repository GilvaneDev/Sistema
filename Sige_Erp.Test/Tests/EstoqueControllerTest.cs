using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Controllers;
using Sige_Erp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Sige_Erp.Test.Tests
{
    public class EstoqueControllerTest
    {
        [Fact]
        public void AcaoDesejadaEstoque_Pesquisar_SetsListaEstoques()
        {
            // Arrange
            var controller = new EstoqueController();
            var estoqueModel = new EstoqueModel
            {
                ListaEstoques = new List<EstoqueModel>()
            };

            // Act
            var result = controller.AcaoDesejadaEstoque("Pesquisar", null, estoqueModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ViewName); // Verifica se está retornando a view correta
            var model = result.Model as EstoqueModel;
            Assert.NotNull(model);
            Assert.NotNull(model.ListaEstoques);
        }

        [Fact]
        public void OnLoadManutencao_Edicao_IdValido_CarregaEstoque()
        {
            // Arrange
            var controller = new EstoqueController();
            var estoqueModel = new EstoqueModel();

            // Act
            var result = controller.OnLoadManutencao(1, "edicao") as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Manutencao", result.ViewName); // Verifica se está retornando a view correta
            var model = result.Model as EstoqueModel;
            Assert.NotNull(model);
            Assert.Equal(1, model.NrSeqEstoque); // Verifica se o número sequencial do estoque foi corretamente carregado
        }

       



        [Fact]
        public void OnLoadManutencao_Criacao_RetornaManutencao()
        {
            // Arrange
            var controller = new EstoqueController();
            var estoqueModel = new EstoqueModel();

            // Act
            var result = controller.OnLoadManutencao(0, "criacao") as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Manutencao", result.ViewName); // Verifica se está retornando a view de Manutencao para criação
        }
    }
}
