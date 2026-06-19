namespace ISOCodex.Countries.Tests;

public sealed class DataIntegrityTests
{
    [Fact]
    public void Country_Seed_Data_Has_No_Duplicate_Alpha2_Codes()
    {
        AssertNoDuplicates(CountryRegistry.All.Select(country => country.Alpha2.Value));
    }

    [Fact]
    public void Country_Seed_Data_Has_No_Duplicate_Alpha3_Codes()
    {
        AssertNoDuplicates(CountryRegistry.All.Select(country => country.Alpha3.Value));
    }

    [Fact]
    public void Country_Seed_Data_Has_No_Duplicate_Numeric_Codes()
    {
        AssertNoDuplicates(CountryRegistry.All.Select(country => country.Numeric.Value));
    }

    [Fact]
    public void Country_Seed_Codes_All_Pass_Syntax_Validation()
    {
        foreach (CountryInfo country in CountryRegistry.All)
        {
            Assert.True(CountryAlpha2Code.IsValidSyntax(country.Alpha2.Value));
            Assert.True(CountryAlpha3Code.IsValidSyntax(country.Alpha3.Value));
            Assert.True(CountryNumericCode.IsValidSyntax(country.Numeric.Value));
        }
    }

    [Fact]
    public void Subdivision_Seed_Data_Refers_To_Known_Countries()
    {
        foreach (CountrySubdivisionInfo subdivision in CountrySubdivisionRegistry.All)
        {
            Assert.True(CountryRegistry.TryGetByAlpha2(subdivision.CountryCode, out _));
        }
    }

    [Fact]
    public void Subdivision_Code_Prefix_Matches_Country_Field()
    {
        foreach (CountrySubdivisionInfo subdivision in CountrySubdivisionRegistry.All)
        {
            Assert.Equal(subdivision.CountryCode, subdivision.Code.CountryCode);
        }
    }

    private static void AssertNoDuplicates(IEnumerable<string> values)
    {
        string[] duplicates = values
            .GroupBy(value => value, StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToArray();

        Assert.Empty(duplicates);
    }
}
