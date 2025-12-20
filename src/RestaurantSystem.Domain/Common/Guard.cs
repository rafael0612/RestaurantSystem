namespace RestaurantSystem.Domain.Common
{
    public static class Guard
    {
        public static void AgainstNull(object? value, string message)
        {
            if (value is null) throw new DomainException(message);
        }

        public static void AgainstNullOrEmpty(string? value, string message)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainException(message);
        }

        public static void AgainstNegative(decimal value, string message)
        {
            if (value < 0) throw new DomainException(message);
        }

        public static void AgainstNonPositive(int value, string message)
        {
            if (value <= 0) throw new DomainException(message);
        }
    }
}
