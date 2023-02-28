using System.Reflection;

namespace Infrastructure.Services
{
    static class Comparer
    {
        ///<summary>
        ///Compare every property in the object with another object of the same type
        ///</summary>
        ///<returns>List of properties that have different values</returns>
        public static List<Variance> CompareObjects<T>(this T obj1, T obj2)
        {
            List<Variance> variances = new List<Variance>();
            if (obj1 is null)
                return new List<Variance>();
            List<FieldInfo> fieldInfos = obj1.GetType().GetRuntimeFields().ToList();
            for (int f = 0; f < fieldInfos.Count; f++)
            {
                Variance variance = new Variance();
                variance.PropertyName = fieldInfos[f].Name.Split("<")[1].Split(">")[0];
                object? v1 = fieldInfos[f].GetValue(obj1);
                object? v2 = fieldInfos[f].GetValue(obj2);
                if (v1 is not null && v2 is not null)
                {
                    variance.Value1 = v1;
                    variance.Value2 = v2;
                    if (!Equals(variance.Value1, variance.Value2))
                        variances.Add(variance);
                }
            }
            return variances;
        }
    }

    class Variance
    {
        public string PropertyName { get; set; }
        public object Value1 { get; set; }
        public object Value2 { get; set; }
    }
}

