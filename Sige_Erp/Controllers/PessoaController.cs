using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sige_Erp.Models;
using Sige_Erp.Uteis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Sige_Erp.Controllers
{
    public class PessoaController : Controller
    {
        private readonly ILogger<PessoaController> _logger;

        public PessoaController(ILogger<PessoaController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Index(int NrSeqPessoa)
        {
            PessoaModel model = new PessoaModel();
            // Recuperar o objeto PessoaModelPesquisa da sessão
            string pessoaModelPesquisaJson = HttpContext.Session.GetString("PessoaModelPesquisa");

            if (!string.IsNullOrEmpty(pessoaModelPesquisaJson))
            {
                PessoaModel pessoaModelPesquisa = JsonConvert.DeserializeObject<PessoaModel>(pessoaModelPesquisaJson);

                if(pessoaModelPesquisa != null)
                {
                    model = pessoaModelPesquisa;
                    HttpContext.Session.Remove("PessoaModelPesquisa");

                }
            }
            else
            {
                model.ListaPessoas = new List<PessoaModel>();
            }
            
            // Adicione itens à lista, se necessário
            return View(model);
        }

        public IActionResult CadastroPessoa()
        {
            return View("Cadastro");
        }
        [HttpPost]
        public IActionResult CadastroPessoa(PessoaModel pessoaModel)
        {
            try
            {
                if (ValidacaoCadastro(pessoaModel))
                {

                    CadastroModel cadastroModel = new CadastroModel();
                    cadastroModel.Web = 'S';

                    if (pessoaModel.DataCadastro != DateTime.MinValue)
                        cadastroModel.DataCadastro = pessoaModel.DataCadastro;

                    using (DAL objDAL = new DAL())
                    {
                        objDAL.OpenTransaction();

                        try
                        {
                            cadastroModel.Cadastrar(objDAL);

                            if (cadastroModel.NrSeqCadastro > 0)
                            {
                                pessoaModel.NrSeqCadastro = cadastroModel.NrSeqCadastro;
                                pessoaModel.Cadastrar(objDAL);

                                if (pessoaModel.NrSeqPessoa > 0)
                                {
                                    EnderecoModel endereco = new EnderecoModel();
                                    endereco.Rua = pessoaModel.Rua;
                                    endereco.Numero = pessoaModel.Numero;
                                    endereco.Bairro = pessoaModel.Bairro;
                                    endereco.Cidade = pessoaModel.Cidade;
                                    endereco.Pais = pessoaModel.Pais;
                                    endereco.Estado = pessoaModel.Estado;
                                    endereco.NrSeqPessoa = pessoaModel.NrSeqPessoa;
                                    endereco.Complemento = string.IsNullOrEmpty(pessoaModel.Complemento) ? string.Empty : pessoaModel.Complemento;
                                    endereco.Avenida = string.IsNullOrEmpty(pessoaModel.Avenida) ? string.Empty : pessoaModel.Avenida;
                                    endereco.Cadastrar(objDAL);

                                    ContatoModel contatoModel = new ContatoModel();
                                    contatoModel.NrSeqPessoa = pessoaModel.NrSeqPessoa;
                                    contatoModel.Email = pessoaModel.Email;
                                    contatoModel.Telefone = pessoaModel.Telefone;
                                    contatoModel.Cadastrar(objDAL);

                                    TipoPessoaModel tipoPessoaModel = new TipoPessoaModel();
                                    tipoPessoaModel.CpfCnpj = pessoaModel.CpfCnpj;
                                    tipoPessoaModel.NrSeqPessoa = pessoaModel.NrSeqPessoa;
                                    tipoPessoaModel.Email = contatoModel.Email;
                                    tipoPessoaModel.Telefone = contatoModel.Telefone;
                                    tipoPessoaModel.NrSeqContato = contatoModel.NrSeqContato;
                                    tipoPessoaModel.Cadastrar(objDAL);


                                    UsuarioModel usuarioModel = new UsuarioModel();
                                    usuarioModel.NomeDoUsuario = pessoaModel.NomeDeUsuario;
                                    usuarioModel.TipoUsuario = pessoaModel.TipoUsuario;
                                    usuarioModel.NrSeqPessoa = pessoaModel.NrSeqPessoa;
                                    usuarioModel.Cadastrar(objDAL);

                                    if (usuarioModel.NrSeqUsuario > 0)
                                    {
                                        LoginModel loginModel = new LoginModel();
                                        loginModel.NrSeqUsuario = usuarioModel.NrSeqUsuario;
                                        loginModel.Email = pessoaModel.Email;
                                        loginModel.NomeDeUsuario = pessoaModel.NomeDeUsuario;
                                        loginModel.Senha = pessoaModel.Senha;
                                        loginModel.Cadastrar(objDAL);
                                    }
                                }

                                objDAL.CommitTransaction();
                                TempData["MensagemSucesso"] = "Cadastro realizado com sucesso!";
                            }
                            else
                            {
                                TempData["MensagemErro"] = "Houve um erro ao cadastrar. Tente novamente.";
                            }
                        }
                        catch (Exception ex)
                        {
                            objDAL.RollbackTransaction();
                            TempData["MensagemErro"] = $"Erro: {ex.Message}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro: {ex.Message}";
            }
            return RedirectToAction("CadastroPessoa", "Pessoa");
        }

        [HttpPost]
        public IActionResult CadastroPessoaManutencao(PessoaModel pessoaModel)
        {
            try
            {
                // Recuperar o objeto PessoaModelPesquisa da sessão
                string NrSeqUsuarioLogado = HttpContext.Session.GetString("NrSeqUsuarioLogado");

                UsuarioModel usuarioLogado = new UsuarioModel();
                usuarioLogado.NrSeqUsuario = Convert.ToInt32(NrSeqUsuarioLogado);
                usuarioLogado.CarregarDados();

                if (usuarioLogado.NrSeqPessoa > 0)
                {
                    pessoaModel.TipoUsuarioLogado = usuarioLogado.TipoUsuario;
                }
                if (ValidacaoCadastro(pessoaModel))
                {
                    
                    using (DAL objDAL = new DAL())
                    {
                        objDAL.OpenTransaction();

                        try
                        {
                            if (pessoaModel.NrSeqPessoa > 0)
                            {
                                PessoaModel pessoa = new PessoaModel();
                                pessoa.NrSeqPessoa = pessoaModel.NrSeqPessoa;
                                pessoa.Carregar();

                                if (pessoa.DataAdmissao == DateTime.MinValue)
                                {
                                    pessoa.DataAdmissao = pessoa.DataCadastro;
                                }
                                if (pessoa.DataDemissao == DateTime.MinValue)
                                {
                                    pessoa.DataDemissao = DateTime.Today.AddYears(100);
                                }

                                if (pessoa.Numero == Int32.MinValue)
                                {
                                    pessoa.Numero = 0;
                                }
                                if (pessoa.NrSeqFuncionario == Int32.MinValue)
                                {
                                    pessoa.Salario = 0;
                                }
                                
                                if (pessoa.NrSeqCadastro > 0)
                                {

                                    if (pessoaModel.TipoPessoa == "PessoaJuridica")
                                    {
                                        pessoaModel.DataNascimento = DateTime.MinValue;
                                        pessoaModel.DataAdmissao = DateTime.MinValue;
                                        pessoaModel.DataDemissao = DateTime.MinValue;
                                        pessoaModel.Cargo = "Empresa";
                                        pessoaModel.Salario = 0;
                                    }
                                    pessoaModel.NrSeqCadastro = pessoa.NrSeqCadastro;
                                    pessoaModel.Atualizar(objDAL);
                                    if (pessoaModel.NrSeqPessoa > 0)
                                    {
                                        EnderecoModel endereco = new EnderecoModel();
                                        endereco.Rua = pessoaModel.Rua;
                                        endereco.Numero = pessoaModel.Numero;
                                        endereco.Bairro = pessoaModel.Bairro;
                                        endereco.Cidade = pessoaModel.Cidade;
                                        endereco.Pais = pessoaModel.Pais;
                                        endereco.Estado = pessoaModel.Estado;
                                        endereco.NrSeqPessoa = pessoaModel.NrSeqPessoa;
                                        endereco.Complemento = string.IsNullOrEmpty(pessoaModel.Complemento) ? string.Empty : pessoaModel.Complemento;
                                        endereco.Avenida = string.IsNullOrEmpty(pessoaModel.Avenida) ? string.Empty : pessoaModel.Avenida;
                                        endereco.Atualizar(objDAL);

                                        ContatoModel contatoModel = new ContatoModel();
                                        contatoModel.NrSeqPessoa = pessoaModel.NrSeqPessoa;
                                        contatoModel.Email = pessoaModel.Email;
                                        contatoModel.Telefone = pessoaModel.Telefone;
                                        contatoModel.Atualizar(objDAL);

                                        TipoPessoaModel tipoPessoaModel = new TipoPessoaModel();
                                        tipoPessoaModel.CpfCnpj = pessoaModel.CpfCnpj;
                                        tipoPessoaModel.Email = pessoaModel.Email;
                                        tipoPessoaModel.Telefone = pessoaModel.Telefone;                                      
                                        tipoPessoaModel.NrSeqPessoa = pessoaModel.NrSeqPessoa;
                                        tipoPessoaModel.Atualizar(objDAL);

                                        UsuarioModel usuarioModel = new UsuarioModel();
                                        if (pessoaModel.TipoUsuario == "Cliente" || pessoaModel.TipoUsuario == "Fornecedor")
                                        {
                                            usuarioModel.NomeDoUsuario = pessoaModel.NomePessoa;
                                        }
                                        else
                                        {
                                            usuarioModel.NomeDoUsuario = pessoaModel.NomeDeUsuario;
                                        }
                                        usuarioModel.TipoUsuario = pessoaModel.TipoUsuario;
                                        usuarioModel.NrSeqPessoa = pessoaModel.NrSeqPessoa;
                                        usuarioModel.Atualizar(objDAL);

                                        if (pessoa.NrSeqUsuario > 0)
                                        {
                                            LoginModel loginModel = new LoginModel();
                                            if (pessoaModel.TipoUsuarioLogado == "Administrador")
                                            {                                              
                                                loginModel.NrSeqUsuario = pessoa.NrSeqUsuario;
                                                loginModel.NrSeqLogin = pessoa.NrSeqLogin;
                                                loginModel.Email = pessoaModel.Email;
                                                loginModel.NomeDeUsuario = pessoaModel.NomeDeUsuario;
                                                loginModel.Senha = pessoaModel.Senha;
                                                loginModel.Atualizar(objDAL);
                                            }
                                            else
                                            {
                                                if (pessoaModel.TipoUsuario != "Cliente" && pessoaModel.TipoUsuario != "Fornecedor")
                                                {
                                                    loginModel.NrSeqLogin = pessoa.NrSeqLogin;
                                                    loginModel.Email = pessoaModel.Email;
                                                    loginModel.AtualizarEmail(objDAL);
                                                }                                                 
                                            }                                                                                    

                                            if (usuarioModel.TipoUsuario == "Funcionario")
                                            {
                                                FuncionarioModel funcionarioModel = new FuncionarioModel();
                                                if (pessoa.NrSeqFuncionario > 0)
                                                {
                                                    funcionarioModel.NrSeqPessoa = pessoaModel.NrSeqPessoa;
                                                    funcionarioModel.PorcentagemComissao = 3;
                                                    funcionarioModel.DataAdmissao = pessoaModel.DataAdmissao;
                                                    funcionarioModel.DataDemissao = pessoaModel.DataDemissao;
                                                    funcionarioModel.Cargo = pessoaModel.Cargo;
                                                    funcionarioModel.NomeDoFuncionario = pessoaModel.NomePessoa;
                                                    funcionarioModel.Salario = pessoaModel.Salario;
                                                    funcionarioModel.Atualizar(objDAL);
                                                }
                                                
                                                
                                            }
                                            if (usuarioModel.TipoUsuario == "Cliente")
                                            {
                                                ClienteModel clienteModel = new ClienteModel();
                                                if(pessoa.NrSeqCliente > 0)
                                                {
                                                    clienteModel.NrSeqPessoa = pessoaModel.NrSeqPessoa;
                                                    clienteModel.EmailCliente = pessoaModel.Email;
                                                    clienteModel.NomeDoCliente = pessoaModel.NomePessoa;
                                                    clienteModel.NrSeqLogin = int.MinValue; ;
                                                    clienteModel.Atualizar(objDAL);
                                                }
                                                
                                            }
                                            if (usuarioModel.TipoUsuario == "Fornecedor")
                                            {
                                                FornecedorModel fornecedorModel = new FornecedorModel();
                                                if(pessoa.NrSeqFornecedor > 0)
                                                {
                                                    fornecedorModel.NrSeqPessoa = pessoaModel.NrSeqPessoa;
                                                    fornecedorModel.InscricaoEstadual = pessoaModel.InscricaoEstadual;
                                                    fornecedorModel.InscricaoMunicipal = pessoaModel.InscricaoMunicipal;
                                                    fornecedorModel.DataCadastro = pessoaModel.DataCadastro;
                                                    fornecedorModel.NomeDoFornecedor = pessoaModel.NomePessoa;
                                                    fornecedorModel.NomeFantasia = pessoaModel.NomePessoa;
                                                    fornecedorModel.Atualizar(objDAL);
                                                }
                                                
                                            }


                                        }
                                    }

                                }                                                             

                                    objDAL.CommitTransaction();
                                    TempData["MensagemSucesso"] = "Atualização realizada com sucesso!";

                                
                            }
                            else
                            {
                                CadastroModel cadastroModel = new CadastroModel();
                                cadastroModel.Web = 'N';

                                if (pessoaModel.DataCadastro != DateTime.MinValue)
                                    cadastroModel.DataCadastro = pessoaModel.DataCadastro;

                                cadastroModel.Cadastrar(objDAL);

                                if (cadastroModel.NrSeqCadastro > 0)
                                {
                                    if (pessoaModel.TipoPessoa == "PessoaJuridica")
                                    {
                                        pessoaModel.DataNascimento = DateTime.MinValue;
                                        pessoaModel.DataAdmissao = DateTime.MinValue;
                                        pessoaModel.DataDemissao = DateTime.MinValue;
                                        pessoaModel.Cargo = "Empresa";
                                        pessoaModel.Salario = 0;
                                    }
                                    pessoaModel.NrSeqCadastro = cadastroModel.NrSeqCadastro;
                                    pessoaModel.Cadastrar(objDAL);

                                    if (pessoaModel.NrSeqPessoa > 0)
                                    {
                                        EnderecoModel endereco = new EnderecoModel();
                                        endereco.Rua = pessoaModel.Rua;
                                        endereco.Numero = pessoaModel.Numero;
                                        endereco.Bairro = pessoaModel.Bairro;
                                        endereco.Cidade = pessoaModel.Cidade;
                                        endereco.Pais = pessoaModel.Pais;
                                        endereco.Estado = pessoaModel.Estado;
                                        endereco.NrSeqPessoa = pessoaModel.NrSeqPessoa;
                                        endereco.Complemento = string.IsNullOrEmpty(pessoaModel.Complemento) ? string.Empty : pessoaModel.Complemento;
                                        endereco.Avenida = string.IsNullOrEmpty(pessoaModel.Avenida) ? string.Empty : pessoaModel.Avenida;
                                        endereco.Cadastrar(objDAL);

                                        ContatoModel contatoModel = new ContatoModel();
                                        contatoModel.NrSeqPessoa = pessoaModel.NrSeqPessoa;
                                        contatoModel.Email = pessoaModel.Email;
                                        contatoModel.Telefone = pessoaModel.Telefone;
                                        contatoModel.Cadastrar(objDAL);

                                        TipoPessoaModel tipoPessoaModel = new TipoPessoaModel();
                                        tipoPessoaModel.CpfCnpj = pessoaModel.CpfCnpj;
                                        tipoPessoaModel.NrSeqPessoa = pessoaModel.NrSeqPessoa;
                                        tipoPessoaModel.Email = contatoModel.Email;
                                        tipoPessoaModel.Telefone = contatoModel.Telefone;
                                        tipoPessoaModel.NrSeqContato = contatoModel.NrSeqContato;
                                        tipoPessoaModel.Cadastrar(objDAL);


                                        UsuarioModel usuarioModel = new UsuarioModel();
                                        if(pessoaModel.TipoUsuario == "Cliente" || pessoaModel.TipoUsuario == "Fornecedor")
                                        {
                                            usuarioModel.NomeDoUsuario = pessoaModel.NomePessoa;
                                        }
                                        else
                                        {
                                            usuarioModel.NomeDoUsuario = pessoaModel.NomeDeUsuario;
                                        }
                                        
                                        usuarioModel.TipoUsuario = pessoaModel.TipoUsuario;
                                        usuarioModel.NrSeqPessoa = pessoaModel.NrSeqPessoa;
                                        usuarioModel.Cadastrar(objDAL);



                                        if (usuarioModel.NrSeqUsuario > 0)
                                        {
                                            LoginModel loginModel = new LoginModel();
                                            if (pessoaModel.TipoUsuario != "Cliente" && pessoaModel.TipoUsuario != "Fornecedor")
                                            {
                                                loginModel.NrSeqUsuario = usuarioModel.NrSeqUsuario;
                                                loginModel.Email = pessoaModel.Email;
                                                loginModel.NomeDeUsuario = pessoaModel.NomeDeUsuario;
                                                loginModel.Senha = pessoaModel.Senha;
                                                loginModel.Cadastrar(objDAL);
                                            }
                                                

                                            if (usuarioModel.TipoUsuario == "Funcionario")
                                            {
                                                FuncionarioModel funcionarioModel = new FuncionarioModel();
                                                funcionarioModel.NrSeqPessoa = pessoaModel.NrSeqPessoa;
                                                funcionarioModel.PorcentagemComissao = 3;
                                                funcionarioModel.DataAdmissao = pessoaModel.DataAdmissao;
                                                funcionarioModel.DataDemissao = pessoaModel.DataDemissao;
                                                funcionarioModel.Cargo = pessoaModel.Cargo;
                                                funcionarioModel.NomeDoFuncionario = pessoaModel.NomePessoa;
                                                funcionarioModel.Salario = pessoaModel.Salario;
                                                funcionarioModel.Cadastrar(objDAL);
                                            }
                                            if (usuarioModel.TipoUsuario == "Cliente")
                                            {
                                                ClienteModel clienteModel = new ClienteModel();
                                                clienteModel.NrSeqPessoa = pessoaModel.NrSeqPessoa;
                                                clienteModel.EmailCliente = pessoaModel.Email;
                                                clienteModel.NomeDoCliente = pessoaModel.NomePessoa;
                                                clienteModel.NrSeqLogin = int.MinValue;
                                                clienteModel.Cadastrar(objDAL);
                                            }
                                            if (usuarioModel.TipoUsuario == "Fornecedor")
                                            {
                                                FornecedorModel fornecedorModel = new FornecedorModel();
                                                fornecedorModel.NrSeqPessoa = pessoaModel.NrSeqPessoa;
                                                fornecedorModel.InscricaoEstadual = pessoaModel.InscricaoEstadual;
                                                fornecedorModel.InscricaoMunicipal = pessoaModel.InscricaoMunicipal;
                                                fornecedorModel.DataCadastro = pessoaModel.DataCadastro;
                                                fornecedorModel.NomeDoFornecedor = pessoaModel.NomePessoa;
                                                fornecedorModel.NomeFantasia = pessoaModel.NomePessoa;
                                                fornecedorModel.Cadastrar(objDAL);
                                            }



                                        }
                                    }

                                    objDAL.CommitTransaction();
                                    TempData["MensagemSucesso"] = "Cadastro realizado com sucesso!";
                                }
                                else
                                {
                                    TempData["MensagemErro"] = "Houve um erro ao cadastrar. Tente novamente.";
                                }
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            objDAL.RollbackTransaction();
                            TempData["MensagemErro"] = $"Erro: {ex.Message}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro: {ex.Message}";
            }
             return View("Manutencao", pessoaModel);
        }

        public bool ValidacaoCadastro(PessoaModel pessoa)
        {
            if (string.IsNullOrEmpty(pessoa.NomePessoa))
            {
                TempData["MensagemErro"] = "Por favor, informe o nome da pessoa.";
                return false;
            }

            if (string.IsNullOrEmpty(pessoa.TipoUsuario))
            {
                TempData["MensagemErro"] = "Por favor, informe o Status Pessoa.";
                return false;
            }

            if (string.IsNullOrEmpty(pessoa.TipoPessoa))
            {
                TempData["MensagemErro"] = "Por favor, informe o tipo da pessoa.";
                return false;
            }
            if(pessoa.TipoUsuario == "Fornecedor" || pessoa.TipoPessoa == "PessoaJuridica")
            {
                if ((pessoa.TipoUsuario == "Fornecedor" && pessoa.TipoPessoa != "PessoaJuridica") || 
                    (pessoa.TipoPessoa == "PessoaJuridica" && pessoa.TipoUsuario != "Fornecedor"))
                {
                    TempData["MensagemErro"] = "O Tipo Pessoa 'Pessoa Jurídica' só pode ser selecionado com  Status Pessoa 'Fornecedor' e vice-versa.";
                    return false;
                }
                if (string.IsNullOrEmpty(pessoa.InscricaoEstadual))
                {
                    TempData["MensagemErro"] = "Por favor, informe a Inscricao Estadual.";
                    return false;
                }
                if (string.IsNullOrEmpty(pessoa.InscricaoMunicipal))
                {
                    TempData["MensagemErro"] = "Por favor, informe a Inscricao Municipal.";
                    return false;
                }
            }

            if (string.IsNullOrEmpty(pessoa.Nacionalidade))
            {
                TempData["MensagemErro"] = "Por favor, informe a nacionalidade.";
                return false;
            }

            if (string.IsNullOrEmpty(pessoa.Email))
            {
                TempData["MensagemErro"] = "Por favor, informe o e-mail.";
                return false;
            }


            if (string.IsNullOrEmpty(pessoa.CpfCnpj))
            {
                TempData["MensagemErro"] = "Por favor, informe o CpfCnpj.";
                return false;
            }

            
            if(pessoa.TipoUsuarioLogado == "Administrador"  && pessoa.TrocarSenha)
            {
                if (string.IsNullOrEmpty(pessoa.Senha))
                {
                    TempData["MensagemErro"] = "Por favor, informe a senha.";
                    return false;
                }
                if (string.IsNullOrEmpty(pessoa.NomeDeUsuario))
                {
                    TempData["MensagemErro"] = "Por favor, informe o nome de usuário.";
                    return false;
                }
            }
            

            if (string.IsNullOrEmpty(pessoa.Telefone))
            {
                TempData["MensagemErro"] = "Por favor, informe o telefone.";
                return false;
            }

            if (pessoa.DataCadastro == DateTime.MinValue)
            {
                TempData["MensagemErro"] = "Por favor, informe a data de cadastro.";
                return false;
            }

            if (pessoa.TipoPessoa == "PessoaFisica" && pessoa.DataNascimento == DateTime.MinValue)
            {
                TempData["MensagemErro"] = "Por favor, informe a data de nascimento.";
                return false;
            }

            if (string.IsNullOrEmpty(pessoa.Cidade))
            {
                TempData["MensagemErro"] = "Por favor, informe a cidade.";
                return false;
            }

            if (string.IsNullOrEmpty(pessoa.Pais))
            {
                TempData["MensagemErro"] = "Por favor, informe o país.";
                return false;
            }

            if (string.IsNullOrEmpty(pessoa.Bairro))
            {
                TempData["MensagemErro"] = "Por favor, informe o bairro.";
                return false;
            }

            if (string.IsNullOrEmpty(pessoa.Estado))
            {
                TempData["MensagemErro"] = "Por favor, informe o estado.";
                return false;
            }

            if (pessoa.Numero <= 0)
            {
                TempData["MensagemErro"] = "Por favor, informe um número válido.";
                return false;
            }

            if (string.IsNullOrEmpty(pessoa.Rua))
            {
                TempData["MensagemErro"] = "Por favor, informe a rua.";
                return false;
            }

            return true;
        }

        [HttpPost]
        public IActionResult AcaoDesejada(string acao, PessoaModel pessoaModel, string IdsSelecionados)
        {           
            List<PessoaModel> data = new List<PessoaModel>();
            // Recuperar o objeto PessoaModelPesquisa da sessão
            string NrSeqUsuarioLogado = HttpContext.Session.GetString("NrSeqUsuarioLogado");

            UsuarioModel usuarioLogado = new UsuarioModel();
            usuarioLogado.NrSeqUsuario = Convert.ToInt32(NrSeqUsuarioLogado);
            usuarioLogado.CarregarDados();
            switch (acao)
            {
                case "Pesquisar":
                    data = pessoaModel.Pesquisar(pessoaModel);

                    

                    if (usuarioLogado.NrSeqPessoa > 0)
                    {
                        if (usuarioLogado.TipoUsuario == "Administrador")
                        {                          
                            pessoaModel.ListaPessoas = data;
                        }
                        else
                        {
                            List<PessoaModel> dataSouce = new List<PessoaModel>();
                            foreach (var item in data)
                            {
                                if (item.TipoUsuario != "Administrador")
                                {
                                    dataSouce.Add(item);
                                }
                            }
                            pessoaModel.ListaPessoas = dataSouce;
                        }
                    }
                    else
                    {
                        pessoaModel.ListaPessoas =  new List<PessoaModel>();
                    }
                   
                    // Serializar e salvar o objeto PessoaModel na sessão com um nome diferente
                    HttpContext.Session.SetString("PessoaModelPesquisa", JsonConvert.SerializeObject(pessoaModel));
                    break;
                case "Novo":
                    // Lógica para a ação de Novo
                    break;
                case "Excluir":
                    if (pessoaModel.IdsSelecionados != null && pessoaModel.IdsSelecionados.Any())
                    {
                        foreach (int idSelecionado in pessoaModel.IdsSelecionados)
                        {
                            using (DAL objDAL = new DAL())
                            {
                                objDAL.OpenTransaction();
                               

                                try
                                {
                                    if(idSelecionado == usuarioLogado.NrSeqPessoa && usuarioLogado.TipoUsuario != "Administrador")
                                    {
                                        pessoaModel.ListaPessoas =  new List<PessoaModel>();
                                        TempData["MensagemErro"] = "Por favor, informe o bairro.";
                                        throw new Exception("Não é permitido Excluir seus proprios Dados");
                                    }
                                    else
                                    {
                                        PessoaModel pessoa = new PessoaModel();
                                        pessoa.NrSeqPessoa = idSelecionado;
                                        pessoa.ListarPessoaEspecifica(idSelecionado);

                                        CadastroModel cadastroModel = new CadastroModel();
                                        cadastroModel.NrSeqCadastro = pessoa.NrSeqCadastro;
                                        if (cadastroModel.NrSeqCadastro > 0) cadastroModel.Excluir(objDAL);

                                        EnderecoModel endereco = new EnderecoModel();
                                        endereco.NrSeqPessoa = idSelecionado;
                                        endereco.CarregarDados();
                                        if (endereco.NrSeqEndereco > 0) endereco.Excluir(objDAL);

                                        ContatoModel contatoModel = new ContatoModel();
                                        contatoModel.NrSeqPessoa = idSelecionado;
                                        contatoModel.CarregarDados();
                                        if (contatoModel.NrSeqContato > 0) contatoModel.Excluir(objDAL);

                                        TipoPessoaModel tipoPessoaModel = new TipoPessoaModel();
                                        tipoPessoaModel.NrSeqPessoa = idSelecionado;
                                        tipoPessoaModel.CarregarDados();
                                        if (tipoPessoaModel.NrSeqTipoPessoa > 0) tipoPessoaModel.Excluir(objDAL);

                                        UsuarioModel usuario = new UsuarioModel();
                                        usuario.NrSeqPessoa = idSelecionado;
                                        usuario.CarregarDados();

                                        FuncionarioModel funcionarioModel = new FuncionarioModel();
                                        funcionarioModel.NrSeqPessoa = idSelecionado;
                                        funcionarioModel.CarregarDados();
                                        if (funcionarioModel.NrSeqFuncionario > 0) funcionarioModel.Excluir(objDAL);

                                        ClienteModel clienteModel = new ClienteModel();
                                        clienteModel.NrSeqPessoa = idSelecionado;
                                        clienteModel.CarregarDados();
                                        if (clienteModel.NrSeqCliente > 0) clienteModel.Excluir(objDAL);

                                        FornecedorModel fornecedorModel = new FornecedorModel();
                                        fornecedorModel.NrSeqPessoa = idSelecionado;
                                        fornecedorModel.CarregarDados();
                                        if (fornecedorModel.NrSeqFornecedor > 0) fornecedorModel.Excluir(objDAL);


                                        LoginModel loginModel = new LoginModel();
                                        loginModel.NrSeqUsuario = usuario.NrSeqUsuario;
                                        loginModel.CarregarDados();
                                        if (loginModel.NrSeqLogin > 0) loginModel.Excluir(objDAL);


                                        if (usuario.NrSeqUsuario > 0) usuario.Excluir(objDAL);

                                        pessoa.Excluir(objDAL);
                                        objDAL.CommitTransaction();
                                        TempData["MensagemSucesso"] = "Excluido com sucesso!";

                                        data = pessoaModel.Pesquisar(pessoaModel);
                                        pessoaModel.ListaPessoas = data;
                                    }
                                   

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
                    List<PessoaModel> lista = new List<PessoaModel>();
                    if (!string.IsNullOrEmpty(IdsSelecionados))
                    {
                        pessoaModel.AbaAtiva = true;
                        string[] idsArray = IdsSelecionados.Split(',');

                        foreach (var id in idsArray)
                        {
                            PessoaModel itempessoa = new PessoaModel();
                            itempessoa.NrSeqPessoa = Convert.ToInt32(id);
                            itempessoa.Carregar();
                            lista.Add(itempessoa);
                        }
                        
                        try
                        {
                            MemoryStream file = new MemoryStream();
                            Document document = new Document(PageSize.A4, 25f, 25f, 15f, 35f);
                            PdfWriter writer = PdfWriter.GetInstance(document, file);                       
                            document.Open();
                            float[] columnWidths = { 2f, 1.3f, 1.3f, 2f };
                            BaseColor[] rowColors = { BaseColor.LIGHT_GRAY, BaseColor.WHITE };

                            PdfPTable table = new PdfPTable(columnWidths);
                            table.WidthPercentage = 100;

                            // Adicionar título à tabela
                            PdfPCell titleCell = new PdfPCell(new Phrase("Relatório de Pessoas", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD)));
                            titleCell.Colspan = columnWidths.Length;
                            titleCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            titleCell.BackgroundColor = BaseColor.GRAY;
                            table.AddCell(titleCell);

                            // Adicionar cabeçalhos
                            foreach (var header in new[] { "Nome", "Tipo Pessoa", "Status Pessoa", "Cpf/Cnpj" })
                            {
                              PdfPCell headerCell = new PdfPCell(new Phrase(header, new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));
                              headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                              table.AddCell(headerCell);
                            }

                           // Adicionar dados
                            foreach (var pessoa in lista)
                            {
                                foreach (var property in new[] { pessoa.NomePessoa, pessoa.TipoPessoa, pessoa.TipoUsuario, pessoa.CpfCnpj })
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
                            pessoaModel.PdfBase64 = base64;

                        }
                        catch (Exception ex)
                        {
                            // Lidar com exceções, por exemplo, registrar ou retornar uma mensagem de erro
                            TempData["MensagemErro"] = $"Erro ao criar o arquivo PDF: {ex.Message}";
                        }
                    }



                    data = lista;
                    pessoaModel.ListaPessoas = data;
                    break;


            }
            // Aqui você pode redirecionar para a página desejada após a execução da ação
            return View("Index", pessoaModel);

        }
        
        public IActionResult OnLoadManutencao(int id)
        {
            PessoaModel pessoa = new PessoaModel();
            pessoa.NrSeqPessoa = id;
            pessoa.Carregar();

            if (pessoa.DataAdmissao == DateTime.MinValue)
            {
                pessoa.DataAdmissao = pessoa.DataCadastro;
            }
            if (pessoa.DataDemissao == DateTime.MinValue)
            {
                pessoa.DataDemissao = DateTime.Today.AddYears(100);
            }

            if (pessoa.Numero == Int32.MinValue)
            {
                pessoa.Numero = 0;
            }
            if (pessoa.NrSeqFuncionario == Int32.MinValue)
            {
                pessoa.Salario = 0;
            }
            // Recuperar o objeto PessoaModelPesquisa da sessão
            string NrSeqUsuarioLogado = HttpContext.Session.GetString("NrSeqUsuarioLogado");

            UsuarioModel usuarioLogado = new UsuarioModel();
            usuarioLogado.NrSeqUsuario = Convert.ToInt32(NrSeqUsuarioLogado);
            usuarioLogado.CarregarDados();

            if(usuarioLogado.NrSeqPessoa > 0)
            {
                pessoa.TipoUsuarioLogado = usuarioLogado.TipoUsuario;
            }

            return View("Manutencao", pessoa);
        }
        public IActionResult Novo()
        {
            PessoaModel pessoa = new PessoaModel();
            pessoa.DataAdmissao = DateTime.Now;
            pessoa.DataDemissao = DateTime.Now;
            pessoa.DataCadastro = DateTime.Now;
            pessoa.DataNascimento = DateTime.Now;
            pessoa.NrSeqPessoa = int.MinValue;

            // Recuperar o objeto PessoaModelPesquisa da sessão
            string NrSeqUsuarioLogado = HttpContext.Session.GetString("NrSeqUsuarioLogado");

            UsuarioModel usuarioLogado = new UsuarioModel();
            usuarioLogado.NrSeqUsuario = Convert.ToInt32(NrSeqUsuarioLogado);
            usuarioLogado.CarregarDados();

            if (usuarioLogado.NrSeqPessoa > 0)
            {
                pessoa.TipoUsuarioLogado = usuarioLogado.TipoUsuario;
            }

            return View("Manutencao", pessoa);
        }
    }
}
