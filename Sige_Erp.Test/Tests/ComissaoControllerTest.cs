using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Sige_Erp.Controllers;
using Sige_Erp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sige_Erp.Test.Tests
{
    public class ComissaoControllerTest
    {
        [Fact]
        public void Index_SetsPropertiesCorrectly_ReturnsViewResultWithModel()
        {
            // Arrange
            var controller = new ComissaoController();
            var comissao = new ComissaoModel();

            // Act
            var result = controller.Index(comissao) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as ComissaoModel;
            Assert.NotNull(model);
            Assert.False(model.AbaAtiva);
            Assert.NotNull(model.ListaComissao);
            Assert.Empty(model.ListaComissao);
            Assert.Null(model.deCadastro);
            Assert.Null(model.ateCadastro);
        }
        [Fact]
        public void AcaoDesejadaComissao_Pesquisar_SetsListaComissao()
        {
            // Arrange
            var controller = new ComissaoController();
            var comissao = new ComissaoModel
            {
                ListaComissao = new List<ComissaoModel>()
            };

            // Act
            var result = controller.AcaoDesejadaComissao("Pesquisar", null, comissao) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as ComissaoModel;
            Assert.NotNull(model);
            Assert.NotNull(model.ListaComissao);
        }   

    }
}
