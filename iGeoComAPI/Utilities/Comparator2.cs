using iGeoComAPI.Models;
using ObjectsComparer;
using System.Dynamic;

namespace iGeoComAPI.Utilities
{
    public static class Comparator2
    {

        public static List<IGeoComDeltaModel> CompareShop(List<IGeoComGrabModel> left, List<IGeoComGrabModel> right, string status)
        {
            List<IGeoComDeltaModel> result = new List<IGeoComDeltaModel>();
            if(left != null && right != null)
            {
                foreach(var lShop in left)
                {
                    foreach(var rShop in right)
                    {
                        if (lShop.E_Address == rShop.E_Address || lShop.C_Address == rShop.C_Address || lShop.Tel_No == rShop.Tel_No)
                        {
                            result.Add(CreateDeltaModel(lShop, status));
                            break;
                        }
                        else
                        {
                            if(lShop.E_Address == rShop.E_Address)
                            {

                            }
                        }

                    }
                }
            }

            return result;

        }

        public class ClassA
        {
            public string StringProperty { get; set; } = String.Empty;

            public int IntProperty { get; set; }
            public string Type { get; set;} = String.Empty;
            public SubClassA? SubClass { get; set; }
        }

        public class SubClassA
        {
            public bool BoolProperty { get; set; }
        }

        public class MyComaparer: AbstractValueComparer<string>
        {
            public override bool Compare(string obj1, string obj2, ComparisonSettings settings)
            {
                return obj1 == obj2; //Implement comparison logic here  
            }
        }


        public static bool TestingCompare()
        {
            var a1 = new ClassA { StringProperty = "String", IntProperty = 1, Type = "A" };
            var a2 = new ClassA { StringProperty = "String", IntProperty = 2, Type = "B" };
            var a3 = new ClassA { StringProperty = "String", IntProperty = 4, Type = "C" };
            var b1 = new ClassA { StringProperty = "String", IntProperty = 3, Type = "A" };
            var b2 = new ClassA { StringProperty = "String", IntProperty = 2, Type = "B" };
            var b3 = new ClassA { StringProperty = "String", IntProperty = 5, Type = "C" };
            var b4 = new ClassA { StringProperty = "String", IntProperty = 6, Type = "D" };
            var aList = new List<ClassA>();
            var bList = new List<ClassA>();
            aList.Add(a1);
            aList.Add(a2);
            aList.Add(a3);
            bList.Add(b1);
            bList.Add(b2);
            bList.Add(b3);
            bList.Add(b4);
            IEnumerable<Difference> differences;
            var comparer = new ObjectsComparer.Comparer<List<ClassA>>();
            var isEqual = comparer.Compare(aList, bList, out differences);
            var differencesList = differences.ToList();
            if (!isEqual)
            {
                Console.WriteLine(differencesList);
            }
            return isEqual;
        }

        public static IGeoComDeltaModel CreateDeltaModel(IGeoComGrabModel data, string status)
        {
            IGeoComDeltaModel newShop = new IGeoComDeltaModel();
            newShop.status = status;
            newShop.GeoNameId = data.GeoNameId;
            newShop.EnglishName = data.EnglishName;
            newShop.ChineseName = data.ChineseName;
            newShop.Class = data.Class;
            newShop.Type = data.Type;
            newShop.Subcat = data.Subcat;
            newShop.Easting = data.Easting;
            newShop.Northing = data.Northing;
            newShop.Source = data.Source;
            newShop.E_floor = data.E_floor;
            newShop.C_floor = data.C_floor;
            newShop.E_sitename = data.E_sitename;
            newShop.C_sitename = data.C_sitename;
            newShop.E_area = data.E_area;
            newShop.C_area = data.C_area;
            newShop.C_District = data.C_District;
            newShop.E_District = data.E_District;
            newShop.E_Region = data.E_Region;
            newShop.C_Region = data.C_Region;
            newShop.E_Address = data.E_Address;
            newShop.C_Address = data.C_Address;
            newShop.Tel_No = data.Tel_No;
            newShop.Fax_No = data.Fax_No;
            newShop.Web_Site = data.Web_Site;
            newShop.Rev_Date = data.Rev_Date;
            return newShop;
        }
    }
}
