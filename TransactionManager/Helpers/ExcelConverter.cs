using System.Data;
using System.Reflection;
using ClosedXML.Excel;

namespace TransactionManager.Helpers;

public static class ExcelConverter
{
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