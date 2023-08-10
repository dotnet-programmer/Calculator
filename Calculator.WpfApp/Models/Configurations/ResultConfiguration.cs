using Calculator.WpfApp.Models.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calculator.WpfApp.Models.Configurations;

internal class ResultConfiguration : IEntityTypeConfiguration<Result>
{
	public void Configure(EntityTypeBuilder<Result> builder)
	{
		builder
			.Property(x => x.Expression)
			.IsRequired();

		builder
			.Property(x => x.Value)
			.IsRequired();
	}
}