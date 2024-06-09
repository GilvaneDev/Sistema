using Microsoft.AspNetCore.Mvc;
using Moq;
using Sige_Erp.Controllers;
using Sige_Erp.Models;
using Sige_Erp.Uteis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sige_Erp.Test.Tests
{
    public class NFVControllerTest
    {
       
        [Fact]
        public void OnLoadManutencao_Edicao_CarregaNotaExistente()
        {
            // Arrange
            var controller = new NFVController();
            int id = 1; // Supondo que o ID existente seja 1
            string modo = "edicao";

            // Act
            var result = controller.OnLoadManutencao(id, modo) as ViewResult;
            var model = result.Model as NFVModel;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Manutencao", result.ViewName); // Verifica se a view retornada é "Manutencao"
            Assert.True(model.FlgEdicao); // Verifica se o modo de edição está definido como verdadeiro
            Assert.Equal(id, model.NrSeqNfv); // Verifica se o ID da nota carregada é igual ao ID fornecido
        }

        // Teste para carregar uma nova nota (modo criação)
        [Fact]
        public void OnLoadManutencao_Criacao_CarregaNovaNota()
        {
            // Arrange
            var controller = new NFVController();
            int id = 0; // Supondo que o ID 0 indica uma nova nota
            string modo = "criacao";

            // Act
            var result = controller.OnLoadManutencao(id, modo) as ViewResult;
            var model = result.Model as NFVModel;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Manutencao", result.ViewName); // Verifica se a view retornada é "Manutencao"
            Assert.False(model.FlgEdicao); // Verifica se o modo de edição está definido como falso
            Assert.Equal(DateTime.Now.Date, model.DataCadastro.Date); // Verifica se a data de cadastro é hoje
            Assert.Equal(DateTime.Now.Date.AddDays(30), model.DataVencimento.Date); // Verifica se a data de vencimento é daqui a 30 dias
        }

    }
}
