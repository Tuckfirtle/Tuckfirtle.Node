// Copyright (C) 2020, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using Microsoft.Extensions.DependencyInjection;

namespace Tuckfirtle.Node.Database
{
    internal sealed class SqliteDatabaseFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SqliteDatabaseFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public SqliteDatabaseScopedContext GetSqliteDatabaseContext(bool isReadOnly)
        {
            return new SqliteDatabaseScopedContext(_serviceProvider.CreateScope(), isReadOnly);
        }
    }
}