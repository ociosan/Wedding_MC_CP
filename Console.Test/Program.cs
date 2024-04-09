using Core;
using Data;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Text;


GenericRepository<Family>.GetColumnNames(
    new Family() 
    { 
        Id = 1, 
        ConfirmationDate = DateTime.Now, 
        EmailAddress = "aldoreyes@test.com", 
        InvitationCode = "NOVIO-01", 
        LastName = "Reyes Muñoz"
    });





public static class GenericRepository<T> where T : class
{
    public static void ConvertEntityProperties(T entity)
    {
        var headersOf = String.Join(", ", entity.GetType().GetProperties().Select(x => x.Name).ToList());
    }

    public static string GetColumnNames(T entity)
    {
        var result = String.Join(", ", entity.GetType().GetProperties().Select(x => x.Name).ToList());
        return result;
    }

    public static string GetColumnValues(T entity)
    {
        StringBuilder result = new StringBuilder();
        foreach(var item in entity.GetType().GetProperties())
        {
            switch(item.GetValue(entity))
            {
                case string:
                    result.Append($"'{ item.GetValue(entity)}'");
                    break;
                case int:
                    result.Append($"{item.GetValue(entity)}");
                    break;
                case DateTime:
                    result.Append($"'{item.GetValue(entity) }'");
                    break;
                case bool:
                    result.Append($"{item.GetValue(entity)}");
                    break;
            }

            result.Append(", ");
        }

        return result.ToString();


    }
}
