using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace Sige_Erp.Uteis
{
    // Camada de Acesso a Dados
    public class DAL : IDisposable
    {
        private static string Servidor = "localhost";
        private static string BancoDados = "sige_vendas";
        private static string Usuario = "root";
        private static string Senha = "1234";
        private static string StringConexao = $"Server={Servidor};Database={BancoDados};Uid={Usuario};Pwd={Senha};Sslmode=none;Charset=utf8;";
        private MySqlConnection Conexao;
        private MySqlTransaction Transacao;

    //    public MySqlConnection Connection { get; internal set; }
        public MySqlConnection Connection { get { return Conexao; } }

        public DAL()
        {
            Conexao = new MySqlConnection(StringConexao);
        }

        // Abre a transação
        public void OpenTransaction()
        {
            if (Conexao.State == ConnectionState.Closed)
            {
                Conexao.Open();
            }

            Transacao = Conexao.BeginTransaction();
        }

        // Confirma a transação
        public void CommitTransaction()
        {
            if (Transacao != null)
            {
                Transacao.Commit();
                Transacao = null;
                Conexao.Close();
            }
        }

        // Reverte a transação em caso de erro
        public void RollbackTransaction()
        {
            if (Transacao != null)
            {
                Transacao.Rollback();
                Transacao = null;
                Conexao.Close();
            }
        }

        public void Dispose()
        {
            if (Transacao != null)
            {
                RollbackTransaction();
            }

            if (Conexao.State == ConnectionState.Open)
            {
                Conexao.Close();
            }
        }


        //public DataTable RetDataTable(string sql)
        //{
        //    try
        //    {
        //        using (MySqlCommand comando = new MySqlCommand(sql, Conexao))
        //        {
        //            using (MySqlDataAdapter da = new MySqlDataAdapter(comando))
        //            {
        //                DataTable dados = new DataTable();
        //                da.Fill(dados);
        //                return dados;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw new Exception($"Erro ao executar RetDataTable: {ex.Message}");
        //    }
        //}
        public DataTable RetDataTable(string sql)
        {
            try
            {
                if (Conexao.State == ConnectionState.Closed)
                {
                    Conexao.Open();
                }

                using (MySqlCommand comando = new MySqlCommand(sql, Conexao))
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(comando))
                    {
                        DataTable dados = new DataTable();
                        da.Fill(dados);
                        return dados;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao executar RetDataTable: {ex.Message}");
            }
        }


        // Espera um parâmetro do tipo string contendo um comando SQL do tipo INSERT, UPDATE, DELETE
        public void ExecutarComandoSQL(string sql)
        {
            if (Conexao.State == ConnectionState.Closed)
            {
                Conexao.Open();
            }

            using (MySqlCommand comando = new MySqlCommand(sql, Conexao))
            {
                comando.ExecuteNonQuery();
            }
        }
        public void ExecutarComandoSQL(string sql, List<MySqlParameter> parameters = null)
        {
            try
            {
                if (Conexao.State == ConnectionState.Closed)
                {
                    Conexao.Open();
                }

                using (MySqlCommand comando = new MySqlCommand(sql, Conexao))
                {
                    if (parameters != null)
                    {
                        comando.Parameters.AddRange(parameters.ToArray());
                    }

                    comando.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao executar comando SQL: {ex.Message}");
            }
            finally
            {
                if (Transacao == null && Conexao.State == ConnectionState.Open)
                {
                    Conexao.Close();
                }
            }
        }

    }
}
