using Sige_Erp.Uteis;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Sige_Erp.Models
{
    public class PessoaModel
    {
        public int NrSeqPessoa { get; set; }
        public int NrSeqCliente { get; set; }
        public int NrSeqFornecedor { get; set; }
        public int NrSeqFuncionario { get; set; }
        public int NrSeqCadastro { get; set; }
        public int NrSeqUsuario { get; set; }
        public int NrSeqLogin { get; set; }
        public string NomePessoa { get; set; }
        public string TipoUsuario { get; set; }
        public string TipoUsuarioLogado { get; set; }
        public string TipoPessoa { get; set; }
        public string Nacionalidade { get; set; }
        public string Email { get; set; }
        public string Cargo { get; set; }
        public decimal Salario{ get; set; }
        public string CpfCnpj { get; set; }
        public string NomeDeUsuario { get; set; }
        public string Senha { get; set; }
        public bool TrocarSenha { get; set; }
        public string Telefone { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataCadastroInicio { get; set; }
        public DateTime DataCadastroFinal { get; set; }
        public DateTime DataAdmissao { get; set; }
        public DateTime DataDemissao { get; set; }
        public string InscricaoEstadual { get; set; }
        public string InscricaoMunicipal { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DataNascimento { get; set; }
        public string Complemento { get; set; }
        public string Cidade { get; set; }
        public string Pais { get; set; }
        public string Bairro { get; set; }
        public string Estado { get; set; }
        public int Numero { get; set; }
        public string Rua { get; set; }
        public string Avenida { get; set; }
        public char Web { get; set; }
        public string PdfBase64 { get; set; }
        public bool AbaAtiva { get; set; }
        public List<PessoaModel> ListaPessoas { get; set; }
        public List<Int32> IdsSelecionados { get; set; }

        public bool IsEdit { get; set; }
        public bool IsInsert { get; set; }

        public bool IsValid()
        {
            var validationResults = new List<ValidationResult>();
            return Validator.TryValidateObject(this, new ValidationContext(this), validationResults, true);
        }

        public int ObterUltimoNrSeqPessoaInserido(DAL objDAL)
        {
            int ultimoNrSeqPessoa = 0;

            try
            {
                    string sql = "SELECT NrSeqPessoa FROM Pessoa ORDER BY NrSeqPessoa DESC LIMIT 1";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        ultimoNrSeqPessoa = Convert.ToInt32(dt.Rows[0]["NrSeqPessoa"]);
                    }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar: {ex.Message}");
            }

            return ultimoNrSeqPessoa;
        }

        public void Cadastrar(DAL objDAL)
        {
            try
            {
 
                    // Obtém o último NrSeqPessoa
                    int ultimoNrSeqPessoa = ObterUltimoNrSeqPessoaInserido(objDAL);

                    // Incrementa o NrSeqPessoa
                    NrSeqPessoa = ultimoNrSeqPessoa + 1;
                    string sql = $"INSERT INTO Pessoa (NrSeqPessoa, NrSeqCadastro, NomePessoa, Nacionalidade, DataNascimento) VALUES ('{NrSeqPessoa}',{NrSeqCadastro}, '{NomePessoa}', '{Nacionalidade}', '{DataNascimento:yyyy-MM-dd}')";
                    objDAL.ExecutarComandoSQL(sql);
                
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar: {ex.Message}");
            }
        }

        public void Atualizar(DAL objDAL)
        {
            try
            {
                
                    string sql = $"UPDATE Pessoa SET NrSeqCadastro = {NrSeqCadastro}, NomePessoa = '{NomePessoa}', Nacionalidade = '{Nacionalidade}', DataNascimento = '{DataNascimento:yyyy-MM-dd}' WHERE NrSeqPessoa = {NrSeqPessoa}";
                    objDAL.ExecutarComandoSQL(sql);
                
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar: {ex.Message}");
            }
        }

        public void Excluir(DAL objDAL)
        {
            try
            {
                string sql = $"DELETE FROM Pessoa WHERE NrSeqPessoa = {NrSeqPessoa}";
                objDAL.ExecutarComandoSQL(sql);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao excluir pessoa com NrSeqPessoa {NrSeqPessoa}");
            }
            finally
            {
                if (objDAL != null)
                {
                    objDAL.Dispose();
                }
            }
        }


        public List<PessoaModel> Pesquisar(PessoaModel pessoaModel)
        {
            string where = "1=1";

            if (!string.IsNullOrEmpty(pessoaModel.NomePessoa))
            {
                where += $@" AND PESSOA.NOMEPESSOA LIKE '%{pessoaModel.NomePessoa}%'";
            }
            if (!string.IsNullOrEmpty(pessoaModel.Nacionalidade))
            {
                where += $@" AND PESSOA.NACIONALIDADE LIKE '%{pessoaModel.Nacionalidade}%'";
            }

            if (pessoaModel.NrSeqPessoa > 0)
            {
                where += $@" AND PESSOA.NRSEQPESSOA = {pessoaModel.NrSeqPessoa}";
            }
            if (!string.IsNullOrEmpty(pessoaModel.CpfCnpj))
            {
                where += $@" AND TIPOPESSOA.CPFCNPJ LIKE '%{pessoaModel.CpfCnpj}%'";
            }
            if (!string.IsNullOrEmpty(pessoaModel.TipoPessoa) && !string.IsNullOrEmpty(pessoaModel.TipoUsuario))
            {
                if(pessoaModel.TipoPessoa == "PessoaJuridica")
                    where += $@" AND USUARIO.TIPOUSUARIO = 'Fornecedor'";
                else
                    where += $@" AND USUARIO.TIPOUSUARIO <> 'Fornecedor'";
            }
            else if (!string.IsNullOrEmpty(pessoaModel.TipoUsuario))
            {
                where += $@" AND USUARIO.TIPOUSUARIO = '{pessoaModel.TipoUsuario}'";
            }

            if (!string.IsNullOrEmpty(pessoaModel.TipoPessoa) && pessoaModel.TipoPessoa == "PessoaJuridica")
            {
                where += $@" AND USUARIO.TIPOUSUARIO = 'Fornecedor'";
            }
            



            if (pessoaModel.DataCadastroInicio > DateTime.MinValue && pessoaModel.DataCadastroFinal != DateTime.MinValue)
            {

                string dataCadastroInicio = pessoaModel.DataCadastroInicio.ToString("yyyy-MM-dd HH:mm:ss");
                string dataCadastroFinal = pessoaModel.DataCadastroFinal.ToString("yyyy-MM-dd HH:mm:ss");

                where += $@" AND CADASTRO.DATACADASTRO BETWEEN '{dataCadastroInicio}' AND '{dataCadastroFinal}'";
            }

            string query = $@"
                SELECT
                    PESSOA.`NRSEQPESSOA`,
                    PESSOA.`NOMEPESSOA`,
                    PESSOA.`NRSEQCADASTRO`,
                    PESSOA.`NACIONALIDADE`,
                    PESSOA.`DATANASCIMENTO`,
                    USUARIO.`TIPOUSUARIO`,
                    CADASTRO.`DATACADASTRO`,
                    TIPOPESSOA.CPFCNPJ
                FROM `PESSOA`
                LEFT JOIN `CADASTRO` ON PESSOA.`NRSEQCADASTRO` = CADASTRO.`NRSEQCADASTRO`
                LEFT JOIN `USUARIO` ON PESSOA.`NRSEQPESSOA` = USUARIO.`NRSEQPESSOA`
                LEFT JOIN `TIPOPESSOA` ON PESSOA.`NRSEQPESSOA` = TIPOPESSOA.`NRSEQPESSOA`
                WHERE {where}";

            List<PessoaModel> pessoas = new List<PessoaModel>();

            try
            {
                using (DAL objDAL = new DAL())
                {
                    DataTable dt = objDAL.RetDataTable(query);

                    foreach (DataRow row in dt.Rows)
                    {
                        PessoaModel pessoa = new PessoaModel();
                        pessoa.NrSeqPessoa = row["NRSEQPESSOA"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["NRSEQPESSOA"]);
                        pessoa.NomePessoa = row["NOMEPESSOA"] == DBNull.Value ? string.Empty : Convert.ToString(row["NOMEPESSOA"]);
                        pessoa.NrSeqCadastro = row["NRSEQCADASTRO"] == DBNull.Value ? int.MinValue : Convert.ToInt32(row["NRSEQCADASTRO"]);
                        pessoa.Nacionalidade = row["NACIONALIDADE"] == DBNull.Value ? string.Empty : Convert.ToString(row["NACIONALIDADE"]);
                        pessoa.TipoUsuario = row["TIPOUSUARIO"] == DBNull.Value ? string.Empty : Convert.ToString(row["TIPOUSUARIO"]);
                        pessoa.TipoPessoa = pessoa.TipoUsuario == "Fornecedor" ? "PessoaJuridica" : "PessoaFisica";
                        pessoa.CpfCnpj = row["CPFCNPJ"] == DBNull.Value ? string.Empty : Convert.ToString(row["CPFCNPJ"]);
                        pessoa.DataCadastro = row["DATACADASTRO"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["DATACADASTRO"]);
                        pessoa.DataNascimento = row["DATANASCIMENTO"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["DATANASCIMENTO"]);

                        pessoas.Add(pessoa);
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao pesquisar: {ex.Message}");
            }

            return pessoas;
        }
        public void Carregar()
        {
            string query = $@"
            SELECT
                PESSOA.`NRSEQPESSOA`,
                PESSOA.`NOMEPESSOA`,
                PESSOA.`NRSEQCADASTRO`,
                PESSOA.`NACIONALIDADE`,
                PESSOA.`DATANASCIMENTO`,
                USUARIO.`TIPOUSUARIO`,
                USUARIO.`NRSEQUSUARIO`,
                CADASTRO.`DATACADASTRO`,
                ENDERECO.`COMPLEMENTO`,
                ENDERECO.`CIDADE`,
                ENDERECO.`PAIS`,
                ENDERECO.`BAIRRO`,
                ENDERECO.`ESTADO`,
                ENDERECO.`NUMERO`,
                ENDERECO.`RUA`,
                ENDERECO.`AVENIDA`,
                TIPOPESSOA.CPFCNPJ,
                CONTATO.`TELEFONE`,
                CONTATO.`EMAIL`,
                FUNCIONARIO.`DATAADMISSAO`,
                FUNCIONARIO.`DATADEMISSAO`,
                FUNCIONARIO.`SALARIO`,
                FUNCIONARIO.`CARGO`,
                FUNCIONARIO.`NRSEQFUNCIONARIO`,
                FORNECEDOR.`NRSEQFORNECEDOR`,
                CLIENTE.`NRSEQCLIENTE`,
                LOGIN.`NRSEQLOGIN`

                
            FROM `PESSOA`
            LEFT JOIN `CADASTRO` ON PESSOA.`NRSEQCADASTRO` = CADASTRO.`NRSEQCADASTRO`
            LEFT JOIN `USUARIO` ON PESSOA.`NRSEQPESSOA` = USUARIO.`NRSEQPESSOA`
            LEFT JOIN `TIPOPESSOA` ON PESSOA.`NRSEQPESSOA` = TIPOPESSOA.`NRSEQPESSOA`
            LEFT JOIN `ENDERECO` ON PESSOA.`NRSEQPESSOA` = ENDERECO.`NRSEQPESSOA`
            LEFT JOIN `CONTATO` ON PESSOA.`NRSEQPESSOA` = CONTATO.`NRSEQPESSOA`
            LEFT JOIN `FUNCIONARIO` ON PESSOA.`NRSEQPESSOA` = FUNCIONARIO.`NRSEQPESSOA`
            LEFT JOIN `CLIENTE` ON PESSOA.`NRSEQPESSOA` = CLIENTE.`NRSEQPESSOA`
            LEFT JOIN `FORNECEDOR` ON PESSOA.`NRSEQPESSOA` = FORNECEDOR.`NRSEQPESSOA`
            LEFT JOIN `LOGIN` ON USUARIO.NRSEQUSUARIO = LOGIN.`NRSEQUSUARIO`
            WHERE PESSOA.NRSEQPESSOA = {NrSeqPessoa}";


            try
            {
                using (DAL objDAL = new DAL())
                {
                    DataTable dt = objDAL.RetDataTable(query);

                    if (dt.Rows.Count == 1)
                    {
                        NrSeqCadastro = dt.Rows[0]["NRSEQCADASTRO"] == DBNull.Value ? Int32.MinValue : Convert.ToInt32(dt.Rows[0]["NRSEQCADASTRO"]);
                        NrSeqUsuario = dt.Rows[0]["NRSEQUSUARIO"] == DBNull.Value ? Int32.MinValue : Convert.ToInt32(dt.Rows[0]["NRSEQUSUARIO"]);
                        NrSeqLogin = dt.Rows[0]["NRSEQLOGIN"] == DBNull.Value ? Int32.MinValue : Convert.ToInt32(dt.Rows[0]["NRSEQLOGIN"]);
                        NomePessoa = dt.Rows[0]["NOMEPESSOA"].ToString();                      
                        Nacionalidade = dt.Rows[0]["NACIONALIDADE"].ToString();
                        DataNascimento = Convert.ToDateTime(dt.Rows[0]["DATANASCIMENTO"]);
                        TipoUsuario = dt.Rows[0]["TIPOUSUARIO"].ToString();
                        TipoPessoa = TipoUsuario == "Fornecedor" ? "PessoaJuridica" : "PessoaFisica";
                        DataCadastro = Convert.ToDateTime(dt.Rows[0]["DATACADASTRO"]);
                        DataAdmissao = dt.Rows[0]["DATAADMISSAO"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["DATAADMISSAO"]);
                        DataDemissao = dt.Rows[0]["DATADEMISSAO"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[0]["DATADEMISSAO"]);
                        Complemento = dt.Rows[0]["COMPLEMENTO"].ToString();
                        Cargo = dt.Rows[0]["CARGO"].ToString();
                        Cidade = dt.Rows[0]["CIDADE"].ToString();
                        Pais = dt.Rows[0]["PAIS"].ToString();
                        Bairro = dt.Rows[0]["BAIRRO"].ToString();
                        Estado = dt.Rows[0]["ESTADO"].ToString();
                        Numero = dt.Rows[0]["NUMERO"] == DBNull.Value ? Int32.MinValue : Convert.ToInt32(dt.Rows[0]["NUMERO"]);                       
                        Salario = dt.Rows[0]["SALARIO"] == DBNull.Value ? Decimal.MinValue : Convert.ToDecimal(dt.Rows[0]["SALARIO"]);
                        NrSeqFuncionario = dt.Rows[0]["NRSEQFUNCIONARIO"] == DBNull.Value ? Int32.MinValue : Convert.ToInt32(dt.Rows[0]["NRSEQFUNCIONARIO"]);
                        NrSeqFornecedor = dt.Rows[0]["NRSEQFORNECEDOR"] == DBNull.Value ? Int32.MinValue : Convert.ToInt32(dt.Rows[0]["NRSEQFORNECEDOR"]);
                        NrSeqCliente = dt.Rows[0]["NRSEQCLIENTE"] == DBNull.Value ? Int32.MinValue : Convert.ToInt32(dt.Rows[0]["NRSEQCLIENTE"]);
                        Rua = dt.Rows[0]["RUA"].ToString();
                        Avenida = dt.Rows[0]["AVENIDA"].ToString();
                        CpfCnpj = dt.Rows[0]["CPFCNPJ"].ToString();
                        Telefone = dt.Rows[0]["TELEFONE"].ToString();
                        Email = dt.Rows[0]["EMAIL"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar: {ex.Message}");
            }
        }


        public DataTable ListarPessoaEspecifica(int nrSeqPessoa)
        {
            DataTable dt = new DataTable();
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM Pessoa WHERE NrSeqPessoa = {nrSeqPessoa}";
                    dt = objDAL.RetDataTable(sql);
                    if (dt.Rows.Count == 1)
                    {
                        NrSeqCadastro = Convert.ToInt32(dt.Rows[0]["NrSeqCadastro"]);
                        NomePessoa = dt.Rows[0]["NomePessoa"].ToString();
                        Nacionalidade = dt.Rows[0]["Nacionalidade"].ToString();
                        DataNascimento = Convert.ToDateTime(dt.Rows[0]["DataNascimento"]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar: {ex.Message}");
            }
            return dt;
        }


        public void CarregarDados()
        {
            try
            {
                using (DAL objDAL = new DAL())
                {
                    string sql = $"SELECT * FROM Pessoa WHERE NrSeqPessoa = {NrSeqPessoa}";
                    DataTable dt = objDAL.RetDataTable(sql);

                    if (dt.Rows.Count == 1)
                    {
                        NrSeqCadastro = Convert.ToInt32(dt.Rows[0]["NrSeqCadastro"]);
                        NomePessoa = dt.Rows[0]["NomePessoa"].ToString();
                        Nacionalidade = dt.Rows[0]["Nacionalidade"].ToString();
                        DataNascimento = Convert.ToDateTime(dt.Rows[0]["DataNascimento"]);
                    }
                }
            }
            catch (Exception ex)



            {
                throw new Exception($"Erro ao carregar: {ex.Message}");
            }
        }
    }
}
