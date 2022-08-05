using iGeoComAPI.Models;
using ObjectsComparer;
using System.Dynamic;

namespace iGeoComAPI.Utilities
{
    public static class Comparator2
    {

            public static List<bool> GetShopCompareList<T>(T self, T to, params string[] ignore) where T : class
            {
                if (self != null && to != null)
                {
                    Type type = typeof(T);
                    List<string> ignoreList = new List<string>(ignore);
                    var compareList = new List<bool>();
                    foreach (System.Reflection.PropertyInfo pi in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                    {
                        if (!ignoreList.Contains(pi.Name))
                        {
                            object selfValue = type.GetProperty(pi.Name).GetValue(self, null);
                            object toValue = type.GetProperty(pi.Name).GetValue(to, null);
                            if (selfValue != toValue && (selfValue == null || !selfValue.Equals(toValue)))
                            {
                                compareList.Add(false);
                            }
                            else
                            {
                                compareList.Add(true);
                            }
                        }
                    }
                    return compareList;
                }
                else
                {
                    return null;
                }
            }

            public static List<IGeoComGrabModel> addedOrRemovedList(List<IGeoComGrabModel> left, List<IGeoComGrabModel> right, params string[] ignoreList)
            {
                var leftLength = left.Count();
                var rightLenght = right.Count();
                var addedOrRemovedList = new List<IGeoComGrabModel>();
                foreach (var lShop in left)
                {
                    int counting = 0;
                    foreach (var rShop in right)
                    {
                        List<bool> exist = GetShopCompareList(lShop, rShop, ignoreList);
                        if (exist.Contains(true))
                        {
                            break;
                        }
                        else
                        {
                            counting++;
                        }
                    }
                    if (counting == rightLenght)
                    {
                        addedOrRemovedList.Add(lShop);
                    }
                }
                return addedOrRemovedList;
            }

            public static List<IGeoComGrabModel> IntersectionList(List<IGeoComGrabModel> list, List<IGeoComGrabModel> filterList, params string[] ignoreList)
            {
                var newResult = new List<IGeoComGrabModel>();
                newResult.AddRange(list);
                foreach (var item in list)
                {
                    foreach (var filter in filterList)
                    {
                        List<bool> exist = GetShopCompareList(item, filter, ignoreList);
                        if (!exist.Contains(false))
                        {
                            newResult.Remove(item);
                            break;
                        }
                    }
                }
                return newResult;
            }

            public static List<IGeoComGrabModel> ModifiedList(List<IGeoComGrabModel> left, List<IGeoComGrabModel> right, params string[] ignoreList)
            {
                var newResult = new List<IGeoComGrabModel>();
                foreach (var l in left)
                {
                    foreach (var r in right)
                    {
                        List<bool> exist = GetShopCompareList(l, r, ignoreList);
                        if (exist.Contains(true) && exist.Contains(false))
                        {
                            newResult.Add(l);
                            break;
                        }
                    }
                }
                return newResult;
            }

            public static List<IGeoComDeltaModel> GetComparedResult(List<IGeoComGrabModel> newResult, List<IGeoComGrabModel> oldResult, params string[] ignoreList)
            {
                var addedList = addedOrRemovedList(newResult, oldResult, ignoreList);
                var removedList = addedOrRemovedList(oldResult, newResult, ignoreList);
                var newIntersect = IntersectionList(newResult, addedList, ignoreList);
                var oldIntersect = IntersectionList(oldResult, removedList, ignoreList);
                var newModified = ModifiedList(newIntersect, oldIntersect, ignoreList);
                var oldModified = ModifiedList(oldIntersect, newIntersect, ignoreList);
                var newModifiedWithState = CreateDeltaModelList(newModified, "new").ToList();
                var oldModifiedWithState = CreateDeltaModelList(oldModified, "old").ToList();
                var modilfiedList = new List<IGeoComDeltaModel>();
                foreach(var (n, i) in newModifiedWithState.Select((n, i) => (n, i)))
                {
                    modilfiedList.Add(n);
                    foreach(var (m,i2) in oldModifiedWithState.Select((m, i2) => (m, i2)))
                    {
                        if(i == i2)
                        {
                            modilfiedList.Add(m);
                        }
                    }
                }
                var result = CreateDeltaModelList(addedList, "added").Concat(CreateDeltaModelList(removedList, "removed")).Concat(modilfiedList).ToList();
                return result;
            }

            //public static bool PublicInstancePropertiesEqual<T>(T self, T to, params string[] ignore) where T : class
            //{
            //    if (self != null && to != null)
            //    {
            //        Type type = typeof(T);
            //        List<string> ignoreList = new List<string>(ignore);
            //        foreach (System.Reflection.PropertyInfo pi in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            //        {
            //            if (!ignoreList.Contains(pi.Name))
            //            {
            //                object selfValue = type.GetProperty(pi.Name).GetValue(self, null);
            //                object toValue = type.GetProperty(pi.Name).GetValue(to, null);

            //                if (selfValue != toValue && (selfValue == null || !selfValue.Equals(toValue)))
            //                {
            //                    return false;
            //                }
            //            }
            //        }
            //        return true;
            //    }
            //   return self == to;
            //}

            public static List<IGeoComDeltaModel> CreateDeltaModelList(List<IGeoComGrabModel> list, string status)
            {
                var deltaList = new List<IGeoComDeltaModel>();
                foreach (var item in list)
                {
                    var delta = CreateDeltaModel(item, status);
                    deltaList.Add(delta);
                }
                return deltaList;
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
