// <auto-generated>
// ReSharper disable ConvertPropertyToExpressionBody
// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable EmptyNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable PartialMethodWithSinglePart
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantNameQualifier
// ReSharper disable RedundantOverridenMember
// ReSharper disable UseNameofExpression
// TargetFrameworkVersion = 4.6
#pragma warning disable 1591    //  Ignore "Missing XML Comment" warning


namespace Veripark.PenaltyCalculation.MvcUI.Models
{

    // Country
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.37.2.0")]
    public class CountryConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Country>
    {
        public CountryConfiguration()
            : this("dbo")
        {
        }

        public CountryConfiguration(string schema)
        {
            ToTable("Country", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").HasColumnType("int").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(x => x.Name).HasColumnName(@"Name").HasColumnType("nvarchar").IsRequired().HasMaxLength(300);
        }
    }

}
// </auto-generated>
