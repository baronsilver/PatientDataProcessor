namespace DataExtractor.Models;

public class Patient
{
    public string? Name { get; set; }
    public string? NHSNumber { get; set; }
}

public class JsonPatient
{
    public string? Name { get; set; }
    public int NHSNumber { get; set; }
}

public class DataInput
{
    public string? FirstFormat { get; set; }
    public string? SecondFormat { get; set; }
}