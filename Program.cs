using System;
using System.Collections.Generic;
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
            InserirDadosEmMassa();
            //ConsultarDados();
            //CadastrarPedido();
            //ConsultarPedidoCarregamentoAdiantado();
            //AtualizarDados();
            //RemoverRegistro();
            //RemoverRegistroDesconectado();

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
                OrderBy(p => p.Id).
                ToList();

            foreach (var cliente in consultaPorMetodo)
            {
                Console.WriteLine($"Consultado o cliente: {cliente.Id}");
                //db.Clientes.Find(cliente.Id); //Consulta em memória.
                db.Clientes.FirstOrDefault(p => p.Id == cliente.Id);

                //Console.WriteLine(consultaPorSintaxe);
            }
        }

        private static void CadastrarPedido()
        {
            using var db = new ApplicationContext();

            var cliente = db.Clientes.FirstOrDefault();  //primeiro cliente que encontrar
            var produto = db.Produtos.FirstOrDefault();  //primeiro produto que encontrar

            var pedido = new Pedido
            {
                ClienteId = cliente.Id,
                IniciadoEm = DateTime.Now,
                FinalizadoEm = DateTime.Now,
                Observacao = "Pedido teste",
                Status = StatusPedido.Analise,
                TipoFrete = TipoFrete.SemFrete,
                Itens = new List<PedidoItem>
                {
                    new PedidoItem
                    {
                        ProdutoId = produto.Id,
                        Desconto = 0,
                        Quantidade = 1,
                        Valor = 10,                    }
                }
            };

            db.Pedidos.Add(pedido);
            db.SaveChanges();
        }

        private static void ConsultarPedidoCarregamentoAdiantado()
        {
            using var db = new ApplicationContext();
            var pedidos = db.Pedidos.Include(p => p.Itens).
                ThenInclude(p => p.Produto).    //produto está dentro de itens, então se utiliza o ThenInclude
                ToList();
            Console.WriteLine($" quantidade de pedidos: {pedidos.Count}");
        }

        private static void AtualizarDados()
        {
            using var db = new ApplicationContext();
            //var cliente = db.Clientes.Find(2);

            var cliente = new Cliente
            {
                Id = 1
            };

            db.Attach(cliente); // rastrear esse objeto internamente, não indo ao banco. 100% desconectado.
            var clienteDesconectado = new   //criando um objeto anonimo, dinamico
            {
                Nome = "Cliente Desconectado passo 4",
                Telefone = "31987540000"
            };

            db.Entry(cliente).CurrentValues.SetValues(clienteDesconectado);
            //db.Entry(cliente).State = EntityState.Modified; avisando que o estado de cliente está em modificação, nesse caso irá atualizar todos os dados do cliente.

            //cliente.Nome = "Cliente Alterado passo 3";
            //db.Clientes.Update(cliente); //sob escreve todas as propriedades, se comenta ele, e apenas utiliza o db.SaveChanges.
            db.SaveChanges();
        }

        private static void RemoverRegistro()
        {
            using var db = new ApplicationContext();
            var cliente = db.Clientes.Find(4); //find utiliza a chave primária, já está implicito.

            //db.Clientes.Remove(cliente); //1 opção
            //db.Remove(cliente); //2 opção
            db.Entry(cliente).State = EntityState.Deleted; //3 opção

            db.SaveChanges();
        }

        //dessa forma ele só interage um comando com o banco de dados, só executa o comando delete.
        private static void RemoverRegistroDesconectado()
        {
            using var db = new ApplicationContext();

            var cliente = new Cliente { Id = 1 };

            //db.Clientes.Remove(cliente); //1 opção
            //db.Remove(cliente); //2 opção
            db.Entry(cliente).State = EntityState.Deleted; //3 opção

            db.SaveChanges();
        }
    }
}
