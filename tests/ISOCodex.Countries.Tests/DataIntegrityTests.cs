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
    public void Country_Seed_Data_Has_Expected_Current_Country_Count()
    {
        Assert.Equal(249, CountryRegistry.All.Count);
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
    public void Country_Seed_Data_Has_No_Empty_Display_Metadata()
    {
        foreach (CountryInfo country in CountryRegistry.All)
        {
            Assert.False(string.IsNullOrWhiteSpace(country.EnglishShortName));
            Assert.All(country.CommonAliases, alias => Assert.False(string.IsNullOrWhiteSpace(alias)));
            Assert.All(country.Notes, note => Assert.False(string.IsNullOrWhiteSpace(note)));
        }
    }

    [Fact]
    public void Country_Seed_Data_Uses_Current_Status_Only()
    {
        foreach (CountryInfo country in CountryRegistry.All)
        {
            Assert.Equal(CountryEntryStatus.Current, country.Status);
        }
    }

    [Fact]
    public void Common_Aliases_Are_Not_Silent_Canonical_Lookup_Keys()
    {
        foreach (CountryInfo country in CountryRegistry.All)
        {
            foreach (string alias in country.CommonAliases)
            {
                CountryCodeLookupResult result = CountryRegistry.Lookup(alias);

                Assert.False(result.Success);
            }
        }
    }

    [Fact]
    public void Country_Data_Version_Is_Populated_And_Iso_Like()
    {
        Assert.False(string.IsNullOrWhiteSpace(CountryDataVersion.Identifier));
        Assert.False(string.IsNullOrWhiteSpace(CountryDataVersion.CheckedOn));
        Assert.False(string.IsNullOrWhiteSpace(CountryDataVersion.Description));
        Assert.True(
            DateOnly.TryParseExact(
                CountryDataVersion.CheckedOn,
                "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out _));
    }

    [Fact]
    public void Documentation_Discloses_Representative_Data_And_No_Iso_Endorsement()
    {
        string repositoryRoot = FindRepositoryRoot();
        string readme = File.ReadAllText(Path.Combine(repositoryRoot, "README.md"));
        string dataSources = File.ReadAllText(Path.Combine(repositoryRoot, "docs", "data-sources.md"));

        Assert.Contains("CLDR", readme, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("not an official ISO product", readme, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Unicode CLDR", dataSources, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("does not claim ISO endorsement", dataSources, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Third_Party_Notices_Disclose_Cldr_Source()
    {
        string repositoryRoot = FindRepositoryRoot();
        string notices = File.ReadAllText(Path.Combine(repositoryRoot, "THIRD-PARTY-NOTICES.md"));

        Assert.Contains("Unicode CLDR release 48.2", notices, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Unicode License v3", notices, StringComparison.OrdinalIgnoreCase);
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
    public void Subdivision_Seed_Data_Has_Expected_Cldr_Count()
    {
        Assert.Equal(5027, CountrySubdivisionRegistry.All.Count);
        Assert.Equal(200, CountrySubdivisionRegistry.All.Select(subdivision => subdivision.CountryCode.Value).Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void Subdivision_Seed_Data_Has_No_Duplicate_Codes()
    {
        AssertNoDuplicates(CountrySubdivisionRegistry.All.Select(subdivision => subdivision.Code.Value));
    }

    [Fact]
    public void Subdivision_Seed_Data_Has_No_Empty_Display_Metadata()
    {
        foreach (CountrySubdivisionInfo subdivision in CountrySubdivisionRegistry.All)
        {
            Assert.False(string.IsNullOrWhiteSpace(subdivision.EnglishName));
        }
    }

    [Fact]
    public void Subdivision_Seed_Codes_All_Pass_Syntax_Validation()
    {
        foreach (CountrySubdivisionInfo subdivision in CountrySubdivisionRegistry.All)
        {
            Assert.True(CountrySubdivisionCode.IsValidSyntax(subdivision.Code.Value));
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

    [Fact]
    public void Country_Display_Name_Data_Has_No_Duplicate_Country_Language_Kind_Keys()
    {
        AssertNoDuplicates(CountryNameRegistry.All.Select(name =>
            string.Join("|", name.CountryCode.Value, name.LanguageTag, name.Kind.ToString())));
    }

    [Fact]
    public void Country_Display_Name_Data_Refers_To_Known_Countries()
    {
        foreach (CountryDisplayName name in CountryNameRegistry.All)
        {
            Assert.True(CountryRegistry.TryGetByAlpha2(name.CountryCode, out _));
        }
    }

    [Fact]
    public void Country_Display_Name_Data_Has_No_Empty_Names_And_Is_Nfc_Normalized()
    {
        foreach (CountryDisplayName name in CountryNameRegistry.All)
        {
            Assert.False(string.IsNullOrWhiteSpace(name.LanguageTag));
            Assert.False(string.IsNullOrWhiteSpace(name.Name));
            Assert.Equal(name.Name.Normalize(System.Text.NormalizationForm.FormC), name.Name);
        }
    }

    [Fact]
    public void Country_Display_Name_Data_Has_Expected_Minimum_Coverage_And_Key_Scripts()
    {
        Assert.True(CountryNameRegistry.All.Count >= 2700);
        Assert.True(CountryNameRegistry.All.Select(name => name.LanguageTag).Distinct(StringComparer.OrdinalIgnoreCase).Count() >= 10);

        AssertDisplayName("DE", "de", "Deutschland");
        AssertDisplayName("JP", "ja", "日本");
        AssertDisplayName("GR", "el", "Ελλάδα");
        AssertDisplayName("CN", "zh-Hans", "中国");

        Assert.True(CountryNameRegistry.TryGetDisplayName(CountryAlpha2Code.Parse("SA"), "ar", out CountryDisplayName? arabic));
        Assert.Equal("المملكة العربية السعودية", arabic!.Name);
        Assert.True(arabic.IsRightToLeft);

        Assert.True(CountryNameRegistry.TryGetDisplayName(CountryAlpha2Code.Parse("IL"), "he", out CountryDisplayName? hebrew));
        Assert.Equal("ישראל", hebrew!.Name);
        Assert.True(hebrew.IsRightToLeft);
    }

    [Fact]
    public void Country_Alias_Data_Has_No_Empty_Fields_And_References_Known_Countries()
    {
        foreach (CountryAliasInfo alias in CountryAliasRegistry.All)
        {
            Assert.False(string.IsNullOrWhiteSpace(alias.Alias));
            Assert.False(string.IsNullOrWhiteSpace(alias.Source));

            if (alias.ReplacementCountryCode.HasValue)
            {
                Assert.True(CountryRegistry.TryGetByAlpha2(alias.ReplacementCountryCode.Value, out _));
            }
        }
    }

    [Fact]
    public void Country_Code_Element_Data_Has_No_Duplicate_Alpha2_Codes_And_No_Empty_Metadata()
    {
        AssertNoDuplicates(CountryCodeElementRegistry.All.Select(element => element.Alpha2.Value));

        foreach (CountryCodeElementInfo element in CountryCodeElementRegistry.All)
        {
            Assert.False(string.IsNullOrWhiteSpace(element.DisplayName));
            Assert.False(string.IsNullOrWhiteSpace(element.Source));
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

    private static void AssertDisplayName(string countryCode, string languageTag, string expectedName)
    {
        Assert.True(CountryNameRegistry.TryGetDisplayName(CountryAlpha2Code.Parse(countryCode), languageTag, out CountryDisplayName? name));
        Assert.Equal(expectedName, name!.Name);
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "ISOCodex.Countries.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not locate repository root.");
    }
}
