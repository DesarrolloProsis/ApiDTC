using System.Collections.Generic;
using System.Linq;

namespace ApiDTC.Models
{
    public static class FormatUtil
    {
        public static string ReferenceFormat(string referenceNumber)
        {
            string[] referenceValue = referenceNumber.Split('-');
            return $"{referenceValue[0]}-{referenceValue[1]}";
        }

        public static string FieldTableFormat(string field)
        {
            List<string> rows = new List<string>();
            foreach(string item in field.Split(';'))
            {
                if(!rows.Contains(item))
                    rows.Add(item);
            }
            if(rows.Count == 1)
                return rows[0];
            else
            {
                string fieldResult = "";
                for(int i = 0; i < rows.Count; i++)
                {
                    if(i == (rows.Count - 1))
                        fieldResult = $"{fieldResult} - {rows[i]}";
                    else
                        fieldResult = $"{fieldResult} - {rows[i]} -";
                }
                return fieldResult;
            }
        }
    }
}