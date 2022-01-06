// Copyright (C) 2020, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tuckfirtle.Node.Database.Table
{
    internal sealed class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string TransactionHash { get; set; }

        [ForeignKey(nameof(Block))]
        public string BlockHash { get; set; }

        public int Version { get; set; }

        public long Timestamp { get; set; }

        public TransactionInput[] TransactionInputs { get; set; }

        public TransactionOutput[] TransactionOutputs { get; set; }
    }
}