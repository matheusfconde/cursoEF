using System;
using System.Linq;
using CursoEFCore.Data;
using CursoEFCore.Domain;
using CursoEFCore.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CursoEF
{
    class Program
    {
        static void Main(string[] args)
        {
            //using var db = new ApplicationContext();
            ////db.Database.Migrate(); verificar andamento da migração
            //var existe = db.Database.GetPendingMigrations().Any();

            //if (existe)
            //{
            //    Console.WriteLine("existe validação pendente!");
            //}


            //InserirDados();
            //InserirDadosEmMassa();
            ConsultarDados();

        }

        private static void InserirDados()
        {
            var produto = new Produto
            {
                Descricao = "Produto Teste",
                CodigoBarras = "1234567891231",
                Valor = 10m,
                TipoProduto = TipoProduto.MercadoriaParaRevenda,
                Ativo = true
            };

            using var db = new ApplicationContext();
            //db.Produtos.Add(produto);                              //Usando o dbSet do ApplicationContext.cs 1 opção INDICADO
            //db.Set<Produto>().Add(produto);                        //Segunda opção  INDICADO
            //db.Entry(produto).State = EntityState.Added;           // terceira opção
            db.Add(produto);                                       // quarta opção
            var registros = db.SaveChanges();

            Console.WriteLine($"Total registro (s): {registros}");
        }

        private static void InserirDadosEmMassa()
        {
            var produto = new Produto
            {
                Descricao = "Produto Teste4",
                CodigoBarras = "1234567891234",
                Valor = 10m,
                TipoProduto = TipoProduto.MercadoriaParaRevenda,
                Ativo = true
            };

            var cliente = new Cliente
            {
                Nome = "Matheus Filipe",
                CEP = "32013460",
                Cidade = "Contagem",
                Estado = "MG",
                Telefone = "31987540035"
            };

            var listaClientes = new[]
            {
                new Cliente
                {
                    Nome = "Matheus Filipe",
                    CEP = "32013460",
                    Cidade = "Contagem",
                    Estado = "MG",
                    Telefone = "31987540035"
                },
                new Cliente
                {
                    Nome = "Andressa Silva",
                    CEP = "32013460",
                    Cidade = "Contagem",
                    Estado = "MG",
                    Telefone = "31987547565"
                }
            };

            using var db = new ApplicationContext();

            //db.AddRange(produto, cliente);

            db.AddRange(listaClientes);

            var registros = db.SaveChanges();

            Console.WriteLine($"Total registro (s) em massa: {registros}");
        }

        private static void ConsultarDados()
        {
            using var db = new ApplicationContext();
            //var consultaPorSintaxe = (from c in db.Clientes where c.Id > 0 select c).ToList();
            var consultaPorMetodo = db.Clientes.Where(p => p.Id > 0).
                OrderBy(p=>p.Id).
                ToList();

            foreach (var cliente in consultaPorMetodo)
            {
                Console.WriteLine($"Consultado o cliente: {cliente.Id}");
                //db.Clientes.Find(cliente.Id); //Consulta em memória.
                db.Clientes.FirstOrDefault(p => p.Id == cliente.Id);

                //Console.WriteLine(consultaPorSintaxe);
            }
        }

    }
}
