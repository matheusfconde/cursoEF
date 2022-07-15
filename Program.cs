using System;
using System.Linq;
using CursoEFCore.Data;
using Microsoft.EntityFrameworkCore;

namespace CursoEF
{
    class Program
    {
        static void Main(string[] args)
        {
            using var db = new ApplicationContext();
            //db.Database.Migrate(); verificar andamento da migração
            var existe = db.Database.GetPendingMigrations().Any();

            if (existe)
            {
                Console.WriteLine("existe validação pendente!");
            }

            Console.WriteLine("Hello World!");
        }
    }
}
