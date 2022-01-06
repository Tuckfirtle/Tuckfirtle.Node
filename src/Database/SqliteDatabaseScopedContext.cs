// Copyright (C) 2020, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tuckfirtle.Node.Database.Table;

namespace Tuckfirtle.Node.Database
{
    internal sealed class SqliteDatabaseScopedContext : IDisposable
    {
        private readonly IServiceScope _serviceScope;
        private readonly SqliteDatabaseContext _sqliteDatabaseContext;

        public DbSet<Block>? Blocks => _sqliteDatabaseContext.Blocks;

        public DbSet<Transaction>? Transactions => _sqliteDatabaseContext.Transactions;

        public DbSet<TransactionInput>? TransactionInputs => _sqliteDatabaseContext.TransactionInputs;

        public DbSet<TransactionOutput>? TransactionOutputs => _sqliteDatabaseContext.TransactionOutputs;

        public SqliteDatabaseScopedContext(IServiceScope serviceScope, bool isReadOnly)
        {
            _serviceScope = serviceScope;
            _sqliteDatabaseContext = serviceScope.ServiceProvider.GetRequiredService<SqliteDatabaseContext>();

            if (isReadOnly)
            {
                _sqliteDatabaseContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            }
        }

        public void SaveChanges()
        {
            _sqliteDatabaseContext.SaveChanges();
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _sqliteDatabaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task MigrateAsync(CancellationToken cancellationToken = default)
        {
            await _sqliteDatabaseContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
        }

        public void Dispose()
        {
            _sqliteDatabaseContext.Dispose();
            _serviceScope.Dispose();
        }
    }
}