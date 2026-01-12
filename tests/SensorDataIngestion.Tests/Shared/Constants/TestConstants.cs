namespace SensorDataIngestion.Tests.Shared.Constants;

/// <summary>
/// Constants used in tests
/// </summary>
public static class TestConstants
{
    public static class Sensors
    {
        public const string ValidSensorId = "SENSOR-001";
        public const string ValidApiKey = "api-key-sensor-001";
        public const string InvalidSensorId = "SENSOR-NONEXISTENT";
        public const string InvalidApiKey = "invalid-api-key";
    }

    public static class Telemetry
    {
        public const decimal ValidTemperature = 25.5m;
        public const decimal ValidHumidity = 60.0m;
        public const decimal MinTemperature = -50m;
        public const decimal MaxTemperature = 100m;
        public const decimal MinHumidity = 0m;
        public const decimal MaxHumidity = 100m;
        public const decimal InvalidTemperature = 150m;
        public const decimal InvalidHumidity = 150m;
    }

    public static class Jwt
    {
        public const string TestSecret = "SecretKeyForUnitTests123!@#";
        public const string TestIssuer = "TestIssuer";
        public const string TestAudience = "TestAudience";
    }
}
