// Copyright (C) 2020, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tuckfirtle.Node.Database.Table
{
    internal sealed class TransactionInput
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; set; }

        [ForeignKey(nameof(Transaction))]
        public string TransactionHash { get; set; }

        [ForeignKey(nameof(TransactionOutput))]
        public int TransactionOutputIndex { get; set; }

        public string ScriptName { get; set; }

        public string ScriptValue { get; set; }
    }
}