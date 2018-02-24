using Bengo.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Bengo.DAL
{
    public class BengoContext: IdentityDbContext<ApplicationUser>
    {        
        public DbSet<Vocabulary> Vocabulary { get; set; }
        public DbSet<Kanji> Kanji { get; set; }
        public DbSet<Grammar> Grammar { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Example> Example { get; set; }
        public DbSet<GrammarQuestion> GrammarQuestion { get; set; }
        public DbSet<Text> Text { get; set; }
        public DbSet<Grammar_Practice> Grammar_Practice { get; set; }
        public DbSet<RepeatInterval> RepeatInterval { get; set; }
        public DbSet<Kana> Kana { get; set; }
        public DbSet<Kanji_Practice> Kanji_Practice { get; set; }
        public DbSet<UserData> UserData { get; set; }
        public DbSet<Vocabulary_Practice> Vocabulary_Practice { get; set; }

        public BengoContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static BengoContext Create()
        {
            return new BengoContext();
        }

    }
}