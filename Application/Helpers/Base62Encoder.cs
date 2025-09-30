namespace Application.Helpers
{
    public static class Base62Encoder
    {
        private const string Alphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const int Base = 62;

        public static string Encode(long number)
        {
            if (number == 0) return Alphabet[0].ToString();

            var sb = new System.Text.StringBuilder();

            while (number > 0)
            {
                sb.Insert(0, Alphabet[(int)(number % 62)]);
                number /= 62;
            }

            return sb.ToString();
        }

        public static long Decode(string shortCode)
        {
            long id = 0;
            long power = 1;

            for (var i = shortCode.Length - 1; i >= 0; i--)
            {
                var charIndex = Alphabet.IndexOf(shortCode[i]);

                if (charIndex == -1)
                {
                    throw new ArgumentException("Invalid short code character.", nameof(shortCode));
                }

                id += charIndex * power;
                power *= Base;
            }

            return id;
        }
    }
}
