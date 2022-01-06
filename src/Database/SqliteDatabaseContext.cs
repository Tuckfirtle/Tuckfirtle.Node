// Copyright (C) 2020, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using Microsoft.EntityFrameworkCore;
using Tuckfirtle.Node.Database.Table;

namespace Tuckfirtle.Node.Database
{
    internal sealed class SqliteDatabaseContext : DbContext
    {
        public DbSet<Block>? Blocks { get; }

        public DbSet<Transaction>? Transactions { get; }

        public DbSet<TransactionInput>? TransactionInputs { get; }

        public DbSet<TransactionOutput>? TransactionOutputs { get; }

        public SqliteDatabaseContext(DbContextOptions<SqliteDatabaseContext> contextOptions) : base(contextOptions)
        {
        }
    }
}