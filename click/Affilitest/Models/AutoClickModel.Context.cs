﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Affilitest.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class AffilitestdbEntities : DbContext
    {
        public AffilitestdbEntities()
            : base("name=AffilitestdbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<AffilitestAccount> AffilitestAccounts { get; set; }
        public DbSet<History> Histories { get; set; }
        public DbSet<SequenceUrl> SequenceUrls { get; set; }
        public DbSet<URLSaving> URLSavings { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
