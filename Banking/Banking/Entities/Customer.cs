using CsvHelper.Configuration.Attributes;

namespace Banking.Entities
{
    public sealed class Customer
    {
        [Index(0)]
        [Name("id")]
        public int Id { get; set; }

        [Index(1)]
        [Name("first_name")]
        public string Name { get; set; }

        [Index(2)]
        [Name("last_name")]
        public string Lastname { get; set; }

        [Index(3)]
        [Name("company_name")]
        public string Companyname { get; set; }

        [Index(4)]
        [Name("address")]
        public string Address { get; set; }

        [Index(5)]
        [Name("city")]
        public string City { get; set; }

        [Index(6)]
        [Name("county")]
        public string Country { get; set; }

        [Index(7)]
        [Name("state")]
        public string State { get; set; }

        [Index(8)]
        [Name("zip")]
        public int Zip { get; set; }

        [Index(9)]
        [Name("phone1")]
        public string FirstPhone { get; set; }

        [Index(10)]
        [Name("phone2")]
        public string SecondPhone { get; set; }

        [Index(11)]
        [Name("email")]
        public string Email { get; set; }

        [Index(12)]
        [Name("web")]
        public string Web { get; set; }

        [Index(13)]
        [Name("type")]
        public string Type { get; set; }

        [Index(14)]
        [Name("balance")]
        public double Balance { get; set; }
    }
}
