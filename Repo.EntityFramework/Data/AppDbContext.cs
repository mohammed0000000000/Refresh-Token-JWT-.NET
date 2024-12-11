using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using refreshTokenJWT.Models;

namespace refreshTokenJWT.Repo.EntityFramework.Data
{
	public class AppDbContext:IdentityDbContext<User>
	{
		public DbSet<User> Users { get; set; }

		public AppDbContext() : base() { }
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			base.OnConfiguring(optionsBuilder);
			optionsBuilder.UseSqlServer();
		}
		protected override void OnModelCreating(ModelBuilder builder) {
			base.OnModelCreating(builder);
			builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
		}
	}
}
