// Copyright (C) 2020, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tuckfirtle.Node.Database.Table
{
    internal sealed class Block
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string BlockHash { get; set; }

        public Guid NetworkIdentifier { get; set; }

        public int Version { get; set; }

        public string Height { get; set; }

        public long Timestamp { get; set; }

        public string Nonce { get; set; }

        public string TargetDifficulty { get; set; }

        public string PreviousHash { get; set; }

        public Transaction[] Transactions { get; set; }
    }
}