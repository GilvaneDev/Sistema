using Microsoft.AspNetCore.Mvc;
using Sige_Erp.Controllers;
using Sige_Erp.Models;
using Xunit;

namespace Sige_Erp.Test.Tests
{
    public class NFControllerTest
    {
        [Fact]
        public void OnLoadManutencao_Edicao_CarregaNotaExistente()
        {
            // Arrange
            var controller = new NFController();
            int id = 1; // Defina o ID existente que você deseja testar
            string modo = "edicao";

            // Act
            var result = controller.OnLoadManutencao(id, modo) as ViewResult;
            var model = result.Model as NFModel;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Manutencao", result.ViewName); // Verifica se a view retornada é "Manutencao"
            Assert.True(model.FlgEdicao); // Verifica se o modo de edição está definido como verdadeiro
            Assert.Equal(id, model.NrSeqNf); // Verifica se o ID da nota carregada é igual ao ID fornecido
        }

        [Fact]
        public void OnLoadManutencao_Criacao_CarregaNovaNota()
        {
            // Arrange
            var controller = new NFController();
            int id = 0; // Defina um ID que simboliza uma nova nota
            string modo = "criacao"; // Seu método está usando "criacao" ao invés de "edicao"

            // Act
            var result = controller.OnLoadManutencao(id, modo) as ViewResult;
            var model = result.Model as NFModel;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Manutencao", result.ViewName); // Verifica se a view retornada é "Manutencao"
            Assert.False(model.FlgEdicao); // Verifica se o modo de edição está definido como falso
            Assert.Equal(DateTime.Now.Date, model.DataCadastro.Date); // Verifica se a data de cadastro é hoje
            Assert.Equal(DateTime.Now.Date.AddDays(30), model.DataVencimento.Date); // Verifica se a data de vencimento é daqui a 30 dias
        }
    }
}
