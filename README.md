# Aesop.Tiger
Aesop.Tiger implements the three variants of the Tiger algorithm by Ross Anderson and Eli Biham. Example usage:
```cs
using (HashAlgorithm h = new Tiger192())
{
    string fileName = @"C:\TestFile.dat";
    FileInfo fi = new (fileName);

    await using (Stream s = new FileStream(
        fi.FullName,
        FileMode.Open,
        FileAccess.Read,
        FileShare.Read,
        (int)fi.Length,
        FileOptions.SequentialScan))
    {
        await h.ComputeHashAsync(s).ConfigureAwait(false);
    }
    
    await Out.WriteAsync(string.Format(CurrentCulture, "\"{0}\": ", fileName)).ConfigureAwait(false);
    await OutputHashAsync(h.Hash).ConfigureAwait(false);
}
```   
