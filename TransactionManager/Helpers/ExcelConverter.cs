using System.Reflection;
using System.Data;
using ClosedXML.Excel;


namespace TransactionManager.Helpers;


/// <summary>
/// Provides methods for converting a list of entities to an Excel file in byte array format.
/// </summary>
public static class ExcelConverter
{
    /// <summary>
    /// Converts a list of entities to an Excel file and returns it as a byte array.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entities to be converted to Excel.</typeparam>
    /// <param name="items">The list of entities to be converted.</param>
    /// <returns>A byte array representing the Excel file.</returns>
    /// <exception cref="NullReferenceException">Thrown if there is an issue retrieving property values from the entities.</exception>
    public static byte[] ConvertToExcel<TEntity>(List<TEntity> items)
    {
        var dataTable = new DataTable(nameof(TEntity));
        
        var props = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        
        foreach (var prop in props)
        {
            dataTable.Columns.Add(prop.Name);
        }
        
        foreach (var item in items)
        {
            var values = new object[props.Length];
            for (var i = 0; i < props.Length; i++)
            {
                values[i] = props[i].GetValue(item, null) ??
                            throw new NullReferenceException("Unable to get the value of the property");
            }
            
            dataTable.Rows.Add(values);
        }
        
        var workbook = new XLWorkbook();
        workbook.Worksheets.Add(dataTable);
        
        using var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);

        return memoryStream.ToArray();
    }
}
