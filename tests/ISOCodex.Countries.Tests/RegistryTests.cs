namespace ISOCodex.Countries.Tests;

public sealed class RegistryTests
{
    [Fact]
    public void Lookup_Gb_By_Alpha2_Returns_United_Kingdom()
    {
        CountryInfo country = CountryRegistry.GetByAlpha2(CountryAlpha2Code.Parse("GB"));

        Assert.Equal("United Kingdom", country.EnglishShortName);
    }

    [Fact]
    public void Lookup_Gbr_And_826_Return_Same_Entry_As_Gb()
    {
        CountryInfo alpha2 = CountryRegistry.GetByAlpha2(CountryAlpha2Code.Parse("GB"));
        CountryInfo alpha3 = CountryRegistry.GetByAlpha3(CountryAlpha3Code.Parse("GBR"));
        CountryInfo numeric = CountryRegistry.GetByNumeric(CountryNumericCode.Parse("826"));

        Assert.Same(alpha2, alpha3);
        Assert.Same(alpha2, numeric);
    }

    [Fact]
    public void Lookup_008_Preserves_Albania_Numeric_Code()
    {
        CountryInfo country = CountryRegistry.GetByNumeric(CountryNumericCode.Parse("008"));

        Assert.Equal("Albania", country.EnglishShortName);
        Assert.Equal("008", country.Numeric.ToString());
    }

    [Fact]
    public void Arbitrary_Unknown_Syntax_Valid_Code_Returns_False_And_Result_Failure()
    {
        Assert.False(CountryRegistry.TryGetByAlpha2("AA", out CountryInfo? country));
        Assert.Null(country);

        CountryCodeLookupResult result = CountryRegistry.Lookup("AA");

        Assert.False(result.Success);
        Assert.Equal(CountryCodeKind.Alpha2, result.DetectedKind);
        Assert.Equal(CountryCodeLookupFailureReason.Unknown, result.FailureReason);
        Assert.Equal("AA", result.NormalizedInput);
    }

    [Fact]
    public void GetBy_Throws_Clearly_For_Unknown_Codes()
    {
        KeyNotFoundException exception = Assert.Throws<KeyNotFoundException>(
            () => CountryRegistry.GetByAlpha2(CountryAlpha2Code.Parse("ZZ")));

        Assert.Contains("ZZ", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1!")]
    [InlineData("GBR1")]
    public void TryGetBy_Handles_Invalid_Input_Safely(string? input)
    {
        Assert.False(CountryRegistry.TryGetByAlpha2(input, out _));
        Assert.False(CountryRegistry.TryGetByAlpha3(input, out _));
        Assert.False(CountryRegistry.TryGetByNumeric(input, out _));
    }

    [Theory]
    [InlineData("GB", CountryCodeKind.Alpha2, "United Kingdom")]
    [InlineData("gbr", CountryCodeKind.Alpha3, "United Kingdom")]
    [InlineData("826", CountryCodeKind.Numeric, "United Kingdom")]
    public void Mixed_Lookup_Detects_Code_Kind(string input, CountryCodeKind expectedKind, string expectedName)
    {
        CountryCodeLookupResult result = CountryRegistry.Lookup(input);

        Assert.True(result.Success);
        Assert.Equal(expectedKind, result.DetectedKind);
        Assert.Equal(expectedName, result.Country?.EnglishShortName);
    }

    [Theory]
    [InlineData("EU")]
    [InlineData("QO")]
    [InlineData("XA")]
    [InlineData("XB")]
    [InlineData("XK")]
    [InlineData("ZZ")]
    public void Known_Non_Country_Shaped_Codes_Are_Not_Invalid_Syntax(string input)
    {
        CountryCodeLookupResult result = CountryRegistry.Lookup(input);

        Assert.False(result.Success);
        Assert.Equal(CountryCodeKind.Alpha2, result.DetectedKind);
        Assert.Equal(CountryCodeLookupFailureReason.ReservedButNotCountry, result.FailureReason);
        Assert.Equal(input, result.NormalizedInput);
    }

    [Fact]
    public void Uk_Remains_Unknown_Because_It_Is_A_Common_Alias_Not_A_Known_Non_Country_Code()
    {
        CountryCodeLookupResult result = CountryRegistry.Lookup("UK");

        Assert.False(result.Success);
        Assert.Equal(CountryCodeKind.Alpha2, result.DetectedKind);
        Assert.Equal(CountryCodeLookupFailureReason.Unknown, result.FailureReason);
        Assert.Equal("UK", result.NormalizedInput);
    }

    [Fact]
    public void Arbitrary_Syntax_Valid_Alpha2_Code_Remains_Unknown()
    {
        CountryCodeLookupResult result = CountryRegistry.Lookup("AA");

        Assert.False(result.Success);
        Assert.Equal(CountryCodeKind.Alpha2, result.DetectedKind);
        Assert.Equal(CountryCodeLookupFailureReason.Unknown, result.FailureReason);
        Assert.Equal("AA", result.NormalizedInput);
    }

    [Theory]
    [InlineData("AX", "ALA", "248", "Åland Islands")]
    [InlineData("BV", "BVT", "074", "Bouvet Island")]
    [InlineData("HM", "HMD", "334", "Heard & McDonald Islands")]
    public void Complete_Country_Seed_Includes_Current_Territory_Edge_Cases(
        string alpha2,
        string alpha3,
        string numeric,
        string expectedName)
    {
        CountryInfo country = CountryRegistry.GetByAlpha2(CountryAlpha2Code.Parse(alpha2));

        Assert.Equal(alpha3, country.Alpha3.Value);
        Assert.Equal(numeric, country.Numeric.Value);
        Assert.Equal(expectedName, country.EnglishShortName);
    }

    [Fact]
    public void Lookup_Invalid_Syntax_Does_Not_Throw()
    {
        CountryCodeLookupResult result = CountryRegistry.Lookup("1!");

        Assert.False(result.Success);
        Assert.Null(result.DetectedKind);
        Assert.Equal(CountryCodeLookupFailureReason.InvalidSyntax, result.FailureReason);
    }

    [Fact]
    public void Country_Name_Registry_Returns_English_And_Localised_Display_Names()
    {
        CountryAlpha2Code de = CountryAlpha2Code.Parse("DE");

        Assert.Equal("Germany", CountryNameRegistry.GetEnglishShortName(de));

        Assert.True(CountryNameRegistry.TryGetDisplayName(de, "de", out CountryDisplayName? german));
        Assert.Equal("Deutschland", german!.Name);
        Assert.True(german.IsEndonym);

        Assert.True(CountryNameRegistry.TryGetDisplayName(CountryAlpha2Code.Parse("JP"), "ja", out CountryDisplayName? japanese));
        Assert.Equal("日本", japanese!.Name);

        Assert.True(CountryNameRegistry.TryGetDisplayName(CountryAlpha2Code.Parse("GR"), "el", out CountryDisplayName? greek));
        Assert.Equal("Ελλάδα", greek!.Name);

        Assert.True(CountryNameRegistry.TryGetDisplayName(CountryAlpha2Code.Parse("SA"), "ar", out CountryDisplayName? arabic));
        Assert.Equal("المملكة العربية السعودية", arabic!.Name);
        Assert.True(arabic.IsRightToLeft);
    }

    [Fact]
    public void Country_Name_Lookup_Reports_Fallbacks()
    {
        CountryAlpha2Code de = CountryAlpha2Code.Parse("DE");

        CountryDisplayNameLookupResult parentFallback = CountryNameRegistry.LookupDisplayName(de, "pt-BR");

        Assert.True(parentFallback.Success);
        Assert.True(parentFallback.UsedFallback);
        Assert.Equal("pt-BR", parentFallback.RequestedLanguageTag);
        Assert.Equal("pt", parentFallback.ResolvedLanguageTag);
        Assert.Equal("Alemanha", parentFallback.DisplayName?.Name);

        CountryDisplayNameLookupResult englishFallback = CountryNameRegistry.LookupDisplayName(de, "cy");

        Assert.True(englishFallback.Success);
        Assert.True(englishFallback.UsedFallback);
        Assert.Equal("en", englishFallback.ResolvedLanguageTag);
        Assert.Equal("Germany", englishFallback.DisplayName?.Name);
    }

    [Fact]
    public void Country_Name_Lookup_Handles_Missing_Country_And_Language_Tag()
    {
        CountryDisplayNameLookupResult missingCountry = CountryNameRegistry.LookupDisplayName(CountryAlpha2Code.Parse("AA"), "en");

        Assert.False(missingCountry.Success);
        Assert.Equal(CountryCodeLookupFailureReason.Unknown, missingCountry.FailureReason);

        CountryDisplayNameLookupResult missingLanguage = CountryNameRegistry.LookupDisplayName(CountryAlpha2Code.Parse("DE"), " ");

        Assert.False(missingLanguage.Success);
        Assert.Equal(CountryCodeLookupFailureReason.Empty, missingLanguage.FailureReason);
    }

    [Fact]
    public void Country_Name_Registry_Exposes_Reviewed_Endonyms()
    {
        IReadOnlyList<CountryDisplayName> endonyms = CountryNameRegistry.GetEndonyms(CountryAlpha2Code.Parse("DE"));

        CountryDisplayName endonym = Assert.Single(endonyms);
        Assert.Equal("de", endonym.LanguageTag);
        Assert.Equal("Deutschland", endonym.Name);

        Assert.True(CountryNameRegistry.TryGetPrimaryEndonym(CountryAlpha2Code.Parse("DE"), out CountryDisplayName? primary));
        Assert.Same(endonym, primary);
    }

    [Fact]
    public void Alias_Registry_Resolves_Common_Names_Explicitly()
    {
        CountryAliasLookupResult britain = CountryAliasRegistry.Lookup("Britain");
        CountryAliasLookupResult greatBritain = CountryAliasRegistry.Lookup("Great Britain");

        Assert.True(britain.Success);
        Assert.Equal("GB", britain.Country?.Alpha2.Value);
        Assert.True(greatBritain.Success);
        Assert.Equal("GB", greatBritain.Country?.Alpha2.Value);
    }

    [Fact]
    public void Alias_Registry_Does_Not_Change_Canonical_Country_Lookup()
    {
        CountryCodeLookupResult aliasAsCountry = CountryRegistry.Lookup("Britain");
        CountryCodeLookupResult ukAsCountry = CountryRegistry.Lookup("UK");
        CountryAliasLookupResult ukAlias = CountryAliasRegistry.Lookup("UK");

        Assert.False(aliasAsCountry.Success);
        Assert.Equal(CountryCodeLookupFailureReason.InvalidSyntax, aliasAsCountry.FailureReason);

        Assert.False(ukAsCountry.Success);
        Assert.Equal(CountryCodeLookupFailureReason.Unknown, ukAsCountry.FailureReason);

        Assert.True(ukAlias.Success);
        Assert.Equal("GB", ukAlias.Country?.Alpha2.Value);
    }

    [Fact]
    public void Alias_Registry_Reports_Ambiguous_Deprecated_Cldr_Alias()
    {
        CountryAliasLookupResult result = CountryAliasRegistry.Lookup("AN");

        Assert.False(result.Success);
        Assert.True(result.Ambiguous);
        Assert.Equal(CountryCodeLookupFailureReason.Ambiguous, result.FailureReason);
        Assert.Equal(new[] { "BQ", "CW", "SX" }, result.Candidates.Select(country => country.Alpha2.Value).ToArray());
    }

    [Theory]
    [InlineData("EU", CountryCodeElementKind.RegionGrouping)]
    [InlineData("QO", CountryCodeElementKind.RegionGrouping)]
    [InlineData("XA", CountryCodeElementKind.PseudoTerritory)]
    [InlineData("XB", CountryCodeElementKind.PseudoTerritory)]
    [InlineData("XK", CountryCodeElementKind.UserAssigned)]
    [InlineData("ZZ", CountryCodeElementKind.UnknownRegion)]
    public void Country_Code_Element_Registry_Contains_Known_Special_Alpha2_Values(
        string value,
        CountryCodeElementKind expectedKind)
    {
        Assert.True(CountryCodeElementRegistry.TryGetByAlpha2(value, out CountryCodeElementInfo? element));
        Assert.Equal(expectedKind, element!.Kind);
        Assert.False(string.IsNullOrWhiteSpace(element.DisplayName));
    }

    [Fact]
    public void Country_Lookup_Uses_Code_Element_Registry_For_Special_Alpha2_Values()
    {
        CountryCodeLookupResult eu = CountryRegistry.Lookup("EU");
        CountryCodeLookupResult unknown = CountryRegistry.Lookup("AA");
        CountryCodeLookupResult invalid = CountryRegistry.Lookup("1!");

        Assert.Equal(CountryCodeLookupFailureReason.ReservedButNotCountry, eu.FailureReason);
        Assert.Equal(CountryCodeLookupFailureReason.Unknown, unknown.FailureReason);
        Assert.Equal(CountryCodeLookupFailureReason.InvalidSyntax, invalid.FailureReason);
    }

    [Fact]
    public void Country_Lookup_Reports_Subdivision_Code_Shapes_As_Unsupported()
    {
        CountryCodeLookupResult result = CountryRegistry.Lookup("GB-ENG");

        Assert.False(result.Success);
        Assert.Null(result.DetectedKind);
        Assert.Equal(CountryCodeLookupFailureReason.Unsupported, result.FailureReason);
        Assert.Equal("GB-ENG", result.NormalizedInput);
    }

    [Fact]
    public void Subdivision_Registry_Returns_Known_And_Unknown_Cases()
    {
        Assert.True(CountrySubdivisionRegistry.TryGetByCode("GB-ENG", out CountrySubdivisionInfo? subdivision));
        Assert.Equal("England", subdivision?.EnglishName);

        Assert.True(CountrySubdivisionRegistry.TryGetByCode("AD-02", out CountrySubdivisionInfo? andorra));
        Assert.Equal("Canillo", andorra?.EnglishName);

        Assert.False(CountrySubdivisionRegistry.TryGetByCode("GB-XYZ", out CountrySubdivisionInfo? unknown));
        Assert.Null(unknown);
    }

    [Fact]
    public void Subdivision_Lookup_Returns_Rich_Result_Semantics()
    {
        CountrySubdivisionLookupResult known = CountrySubdivisionRegistry.Lookup("gb-eng");
        CountrySubdivisionLookupResult unknown = CountrySubdivisionRegistry.Lookup("GB-XYZ");
        CountrySubdivisionLookupResult empty = CountrySubdivisionRegistry.Lookup(null);
        CountrySubdivisionLookupResult invalid = CountrySubdivisionRegistry.Lookup("1!");

        Assert.True(known.Success);
        Assert.Equal("GB-ENG", known.NormalizedInput);
        Assert.Equal("England", known.Subdivision?.EnglishName);

        Assert.False(unknown.Success);
        Assert.Equal("GB-XYZ", unknown.NormalizedInput);
        Assert.Equal(CountryCodeLookupFailureReason.Unknown, unknown.FailureReason);

        Assert.False(empty.Success);
        Assert.Equal(CountryCodeLookupFailureReason.Empty, empty.FailureReason);

        Assert.False(invalid.Success);
        Assert.Equal(CountryCodeLookupFailureReason.InvalidSyntax, invalid.FailureReason);
    }
}
