namespace AcmeIntegreatedAssessement.Infrastructure.Configuration;

public class PaymentDatabaseSettings
{
    public const string SectionName = "PaymentDatabase";
    public const string DefaultConnectionStringName = "PaymentDatabase";

    public string ConnectionStringName { get; set; } = DefaultConnectionStringName;
}
