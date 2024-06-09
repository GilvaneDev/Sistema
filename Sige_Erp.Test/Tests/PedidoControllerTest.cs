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
    public class PedidoControllerTest
    {
        [Fact]
        public void Index_ReturnsViewResult_WithPedidoModel()
        {
            // Arrange
            var controller = new PedidoController();
            var pedido = new PedidoModel();

            // Act
            var result = controller.Index(pedido) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ViewName);
            Assert.IsType<PedidoModel>(result.Model);
        }
        [Fact]
        public void ValidacaoCadastro_ValidInputs_ReturnsTrue()
        {
            // Arrange
            var controller = new PedidoController();
            var pedido = new PedidoModel
            {
                NomeProduto = "Produto",
                Etiqueta = "Etiqueta",
                Preco = 10.0m,
                Quantidade = 1,
                TipoUsuario = "Fornecedor",
                Fornecedor = "Fornecedor 1",
                StatusPedido = "Status",
                DataCadastro = System.DateTime.Now,
                DataVencimento = System.DateTime.Now.AddDays(1),
                Descricao = "Descrição",
                Frete = "Frete",
                CheckboxTroca = false,
                CheckboxDevolucao = true,
                Motivo = "Motivo de devolução"
            };

            // Act
            var result = controller.ValidacaoCadastro(pedido);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidacaoCadastro_InvalidInputs_ThrowsException()
        {
            // Arrange
            var controller = new PedidoController();
            var pedido = new PedidoModel
            {
                NomeProduto = "",
                Etiqueta = "Etiqueta",
                Preco = 10.0m,
                Quantidade = 0,
                TipoUsuario = "Fornecedor",
                Fornecedor = "",
                StatusPedido = "Status",
                DataCadastro = System.DateTime.Now,
                DataVencimento = System.DateTime.Now.AddDays(1),
                Descricao = "Descrição",
                Frete = "Frete",
                CheckboxTroca = false,
                CheckboxDevolucao = false,
                Motivo = ""
            };

            // Act & Assert
            Assert.Throws<Exception>(() => controller.ValidacaoCadastro(pedido));
        }
    }
}
