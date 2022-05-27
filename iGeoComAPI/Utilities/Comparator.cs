using iGeoComAPI.Models;

namespace iGeoComAPI.Utilities
{
    public static class Comparator
    {
        public static List<IGeoComDeltaModel> GetComparedResult(List<IGeoComGrabModel> newData, List<IGeoComGrabModel> previousData, string without = "")
        {
            var removedResult = CompareShop(previousData, newData, "removed", without);
            var addedResult = CompareShop(newData, previousData, "added", without);
            var newIntersectResult = Intersection(newData, addedResult, without);
            var previousIntersectResult = Intersection(previousData, removedResult, without);
            var newModified = CompareShop(newIntersectResult, previousIntersectResult, "new_modified", without);
            var orgModified = CompareShop(previousIntersectResult, newIntersectResult, "old_modified", without);
            var result = MergeResults(addedResult, removedResult, orgModified, newModified, without);
            return result;
        }
        public static List<IGeoComDeltaModel> CompareShop(List<IGeoComGrabModel> left, List<IGeoComGrabModel> right, string status, string without ="")
        {
            int leftLength = left.Count;
            int rightLength = right.Count;
            List<IGeoComDeltaModel> IGeoComList = new List<IGeoComDeltaModel>();

            for (int i = 0; i < leftLength; i++)
            {
                int j;
                {
                    if(without == "tel")
                    {
                        for (j = 0; j < rightLength; j++)
                            if (status.Contains("modified"))
                            {
                                if (left[i].Compare_E_Address == right[j].Compare_E_Address &&
                                    left[i].Compare_C_Address == right[j].Compare_C_Address)
                                    break;
                            }
                            else
                            {
                                if (left[i].Compare_E_Address == right[j].Compare_E_Address |
                                    left[i].Compare_C_Address == right[j].Compare_C_Address)
                                    break;
                            }
                    }
                    else if(without == "vango"){
                        for (j = 0; j < rightLength; j++)
                            if (status.Contains("modified"))
                            {
                                if (left[i].Compare_Tel == right[j].Compare_Tel &&
                                    left[i].Compare_C_Address == right[j].Compare_C_Address
                                    )
                                    break;
                            }
                            else
                            {
                                if (String.IsNullOrEmpty(left[i].Compare_Tel) && String.IsNullOrEmpty(right[j].Compare_Tel))
                                {
                                    if (
                                    left[i].Compare_C_Address == right[j].Compare_C_Address 
                                    )
                                        break;
                                }
                                else
                                {
                                    if (
                                        left[i].Compare_C_Address == right[j].Compare_C_Address |
                                        left[i].Compare_Tel == right[j].Compare_Tel
                                        )
                                        break;
                                }
                            }
                    }
                    else if(without == "ambulance")
                    {
                        for (j = 0; j < rightLength; j++)
                            if (status.Contains("modified"))
                            {
                                if (left[i].Compare_ChineseName == right[j].Compare_ChineseName &&
                                    left[i].Compare_EnglishName == right[j].Compare_EnglishName)
                                    break;
                            }
                            else
                            {
                                if (left[i].Compare_ChineseName == right[j].Compare_ChineseName |
                                    left[i].Compare_EnglishName == right[j].Compare_EnglishName)
                                    break;
                            }
                    }
                    else
                    {
                        for (j = 0; j < rightLength; j++)
                            if (status.Contains("modified"))
                            {
                                if (left[i].Compare_E_Address == right[j].Compare_E_Address &&
                                    left[i].Compare_C_Address == right[j].Compare_C_Address &&
                                    left[i].Compare_Tel == right[j].Compare_Tel
                                    )
                                    break;
                            }
                            else
                            {
                                if( String.IsNullOrEmpty(left[i].Compare_Tel) && String.IsNullOrEmpty(right[j].Compare_Tel))
                                {
                                    if (left[i].Compare_E_Address == right[j].Compare_E_Address |
                                        left[i].Compare_C_Address == right[j].Compare_C_Address) 
                                        break;
                                }
                                else
                                {
                                    if (left[i].Compare_E_Address == right[j].Compare_E_Address |
                                        left[i].Compare_C_Address == right[j].Compare_C_Address |
                                        left[i].Compare_Tel == right[j].Compare_Tel)
                                        break;
                                }

                            }
                    }

                    if (j == rightLength)
                    {
                        var newShop = CreateDeltaModel(left, i, status);
                        IGeoComList.Add(newShop);
                    }
                }

            }
            Console.WriteLine(IGeoComList.Count);
            return IGeoComList;
        }

        public static List<IGeoComGrabModel> Intersection(List<IGeoComGrabModel> left, List<IGeoComDeltaModel> right, string without = "")
        {
            int leftLength = left.Count;
            int rightLength = right.Count;
            List<IGeoComGrabModel> IntersectionIGeoComList = new List<IGeoComGrabModel>();

            for (int i = 0; i < leftLength; i++)
            {
                int j;
                if (without == "tel")
                {
                    for (j = 0; j < rightLength; j++)

                        if (left[i].Compare_E_Address ==  right[j].Compare_E_Address |
                            left[i].Compare_C_Address == right[j].Compare_C_Address
                            )
                            break;
                }
                else if (without == "vango")
                {
                    for (j = 0; j < rightLength; j++)

                        if (String.IsNullOrEmpty(left[i].Compare_Tel) && String.IsNullOrEmpty(right[j].Compare_Tel))
                        {
                            if (left[i].Compare_C_Address == right[j].Compare_C_Address )
                                break;
                        }
                        else
                        {
                            if (left[i].Compare_C_Address == right[j].Compare_C_Address |
                                left[i].Compare_Tel == right[j].Compare_Tel)
                                break;
                        }

                }
                else if (without == "ambulance")
                {
                    for (j = 0; j < rightLength; j++)

                            if (left[i].Compare_ChineseName == right[j].Compare_ChineseName |
                                left[i].Compare_EnglishName == right[j].Compare_EnglishName)
                                break;
                }
                else
                {
                    for (j = 0; j < rightLength; j++)

                        if (String.IsNullOrEmpty(left[i].Tel_No) && String.IsNullOrEmpty(right[j].Tel_No))
                        {
                            if (left[i].Compare_E_Address == right[j].Compare_E_Address |
                                left[i].Compare_C_Address == right[j].Compare_C_Address)
                                break;
                        }
                        else
                        {
                            if (left[i].Compare_E_Address == right[j].Compare_E_Address |
                                left[i].Compare_C_Address == right[j].Compare_C_Address |
                                left[i].Compare_Tel == right[j].Compare_Tel)
                                break;
                        }
                }
                if (j == rightLength)
                {
                    IntersectionIGeoComList.Add(left[i]);
                }

            }
            return IntersectionIGeoComList;
        }

        public static List<IGeoComDeltaModel> MergeResults(List<IGeoComDeltaModel> added, List<IGeoComDeltaModel> removed, List<IGeoComDeltaModel> newDelta, List<IGeoComDeltaModel> orgDelta, string without = "")
        {
            List<IGeoComDeltaModel> DeltaChange = new List<IGeoComDeltaModel>();
            List<IGeoComDeltaModel> mergeAddedAndRemoved = added.Concat(removed).ToList();

            foreach (var (v, i) in newDelta.Select((v, i) => (v, i)))
            {
                DeltaChange.Add(v);
                foreach (var item in orgDelta)
                {
                    if (without == "tel")
                    {
                       if (v.Compare_E_Address == item.Compare_E_Address |
                           v.Compare_C_Address == item.Compare_C_Address)
                        {
                            DeltaChange.Add(item);
                        }
                    }
                    else if (without == "vango")
                    {

                        if (String.IsNullOrEmpty(v.Tel_No) && String.IsNullOrEmpty(item.Tel_No))
                        {
                            if (v.Compare_C_Address == item.Compare_C_Address)
                            {
                                DeltaChange.Add(item);
                            }
                        }
                        else
                        {
                            if (v.Compare_C_Address == item.Compare_C_Address |
                                v.Compare_Tel == item.Compare_Tel)
                            {
                                DeltaChange.Add(item);
                            }
                        }

                    }
                    else if (without == "ambulance")
                    {
                            if (v.Compare_ChineseName == item.Compare_ChineseName |
                                v.Compare_EnglishName == item.Compare_EnglishName)
                                break;
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(v.Compare_Tel) && String.IsNullOrEmpty(item.Compare_Tel))
                        {
                            if (v.Compare_C_Address == item.Compare_C_Address |
                                v.Compare_E_Address == item.Compare_E_Address)
                            {
                                DeltaChange.Add(item);
                            }
                        }
                        else
                        {
                            if (v.Compare_C_Address == item.Compare_C_Address |
                                v.Compare_E_Address == item.Compare_E_Address |
                                v.Compare_Tel == item.Compare_Tel)
                            {
                                DeltaChange.Add(item);
                            }
                        }
                    }
                }
            }
            var results = mergeAddedAndRemoved.Concat(DeltaChange).ToList();
            return results;
        }

        public static IGeoComDeltaModel CreateDeltaModel(List<IGeoComGrabModel> data, int num, string status)
        {
           IGeoComDeltaModel newShop = new IGeoComDeltaModel();
           newShop.status = status;
           newShop.GeoNameId = data[num].GeoNameId;
           newShop.EnglishName = data[num].EnglishName;
           newShop.ChineseName = data[num].ChineseName;
           newShop.Class = data[num].Class;
           newShop.Type = data[num].Type;
           newShop.Subcat = data[num].Subcat;
           newShop.Easting = data[num].Easting;
           newShop.Northing = data[num].Northing;
           newShop.Source = data[num].Source;
           newShop.E_floor = data[num].E_floor;
           newShop.C_floor = data[num].C_floor;
           newShop.E_sitename = data[num].E_sitename;
           newShop.C_sitename = data[num].C_sitename;
           newShop.E_area = data[num].E_area;
           newShop.C_area = data[num].C_area;
           newShop.C_District = data[num].C_District;
           newShop.E_District = data[num].E_District;
           newShop.E_Region = data[num].E_Region;
           newShop.C_Region = data[num].C_Region;
           newShop.E_Address = data[num].E_Address;
           newShop.C_Address = data[num].C_Address;
           newShop.Tel_No = data[num].Tel_No;
           newShop.Fax_No = data[num].Fax_No;
           newShop.Web_Site = data[num].Web_Site;
           newShop.Rev_Date = data[num].Rev_Date;
           return newShop;
        }
    }
}
