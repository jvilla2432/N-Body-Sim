struct Settings
{
    public enum ForceCalculators { standard, orthtree, dynOrthtree};
    public Settings(bool multi, ForceCalculators calculator)
    {
        Multi = multi;
        Calculator = calculator;
    }

    public bool Multi { get; set; }
    public ForceCalculators Calculator { get; set; }
}