namespace task01;

public static class StringExtensions
{
    public static bool IsPalindrome(this string input)
    {
    if (string.IsNullOrEmpty(input))
    {
        return false;
    }

    string lowerInput = input.ToLower();

    var checkedString = new System.Text.StringBuilder();
    foreach (char c in lowerInput)
    {
        if (!char.IsWhiteSpace(c) && !char.IsPunctuation(c))
        {
            checkedString.Append(c);
        }
    }
    string cleaned = checkedString.ToString();
    if (cleaned.Length == 0)
    {
        return false;
    }

    int left = 0;
    int right = cleaned.Length - 1;
    while (left < right)
    {
        if (cleaned[left] != cleaned[right])
        {
            return false;
        }
        left++;
        right--;
    }
    return true;
    }
}
