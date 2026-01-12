using SensorDataIngestion.API.Configuration;

namespace SensorDataIngestion.Tests.API.Configuration;

public class JwtSettingsTests
{
    [Fact]
    public void JwtSettings_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var settings = new JwtSettings();

        // Assert
        settings.Secret.Should().BeEmpty();
        settings.Issuer.Should().BeEmpty();
        settings.Audience.Should().BeEmpty();
        settings.ExpirationMinutes.Should().Be(60);
    }

    [Fact]
    public void JwtSettings_SectionName_ShouldBeJwt()
    {
        // Assert
        JwtSettings.SectionName.Should().Be("Jwt");
    }

    [Fact]
    public void JwtSettings_ShouldAllowChangingValues()
    {
        // Arrange
        var settings = new JwtSettings
        {
            Secret = "MySuperSecretSecureKey123!",
            Issuer = "MyIssuer",
            Audience = "MyAudience",
            ExpirationMinutes = 120
        };

        // Assert
        settings.Secret.Should().Be("MySuperSecretSecureKey123!");
        settings.Issuer.Should().Be("MyIssuer");
        settings.Audience.Should().Be("MyAudience");
        settings.ExpirationMinutes.Should().Be(120);
    }
}
