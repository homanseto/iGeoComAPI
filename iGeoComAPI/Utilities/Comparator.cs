using iGeoComAPI.Models;

namespace iGeoComAPI.Utilities
{
    //public static class Comparator
    //{
    //    public static void FindDiffernet(List<IGeoComRepository> Left, List<IGeoComGrabModel> Right, string status, List<String> properties)
    //    {
    //        int LeftLength = Left.Count;
    //        int RightLength = Right.Count;
    //        int propertiesLength = properties.Count;
    //        List<IGeoComGrabModel> AddedWellcomeIGeoComList = new List<IGeoComGrabModel>();
           
    //        for (int i = 0; i < LeftLength; i++)
    //        {
    //           List<string> leftItem = new List<string>();
    //            foreach (var property in properties)
    //            {
    //                var leftProperty = Left[i].GetType().GetProperty(property);
    //                if (leftProperty != null)
    //                {
    //                    var leftValue = leftProperty.GetValue(Left[i], null);
    //                    if(leftValue!.GetType() == typeof(string))
    //                    {
    //                        Console.WriteLine(leftValue);
    //                        leftItem.Add((string)leftValue);
    //                    }
    //                }
    //            }
    //                int j;
    //                for (j = 0; j <= RightLength; j++)
    //            {
    //                List<string> rightItem = new List<string>();
    //                foreach (var property in properties)
    //                {
    //                    var rightProperty = Right[j].GetType().GetProperty(property);
    //                    if (rightProperty != null)
    //                    {
    //                        var rightValue = rightProperty.GetValue(Right[j], null);
    //                        if (rightValue!.GetType() == typeof(string))
    //                        {
    //                            Console.WriteLine(rightValue);
    //                            rightItem.Add((string)rightValue);
    //                        }
    //                    }
    //                }
    //                if (leftItem[0]?.Replace(",", "").Replace(" ", "") == rightItem[0]?.Replace(",", "").Replace(" ", "")|
    //                    leftItem[1]?.Replace(",", "").Replace(" ", "") == rightItem[1]?.Replace(",", "").Replace(" ", "")
    //                   )
    //                    break;
    //                if (j == RightLength)
    //                {
    //                    //Left[i].status = status;
    //                    Console.WriteLine(Left[i].E_Address);
    //                }
    //            }

    //        }
    //    }
    //    /*
    //    public List<IGeoComModel> FindDiffernet(List<IGeoComModel> Left, List<IGeoComModel> Right, string status)
    //    {
    //        int LeftLength = Left.Count;
    //        int RightLength = Right.Count;
    //        List<IGeoComModel> AddedWellcomeIGeoComList = new List<IGeoComModel>();

    //        for (int i = 0; i < LeftLength; i++)
    //        {
    //            int j;
    //            for (j = 0; j < RightLength; j++)
    //                if (Left[i].E_Address?.Replace(",", "").Replace(" ", "") == Right[j].E_Address?.Replace(",", "").Replace(" ", "") |
    //                    Left[i].Tel_No?.Replace(" ", "") == Right[j].Tel_No?.Replace(" ", "") |
    //                    Left[i].C_Address?.Replace(",", "").Replace(" ", "") == Right[j].C_Address?.Replace(",", "").Replace(" ", "")
    //                    )
    //                    break;
    //            if (j == RightLength)
    //            {
    //                //Left[i].status = status;
    //                AddedWellcomeIGeoComList.Add(Left[i]);
    //            }
    //        }
    //        return AddedWellcomeIGeoComList;
    //    }
    //    */
    //    public static List<IGeoComRepository> LeftIntersection(List<IGeoComRepository> newData, List<IGeoComRepository> added)
    //    {
    //        int newDataLength = newData.Count;
    //        int addedLength = added.Count;
    //        List<IGeoComRepository> LeftIntersectionIGeoComList = new List<IGeoComRepository>();

    //        for (int i = 0; i < newDataLength; i++)
    //        {
    //            int j;
    //            for (j = 0; j < addedLength; j++)
    //                if (newData[i].E_Address?.Replace(",", "").Replace(" ", "") == added[j].E_Address?.Replace(",", "").Replace(" ", "") |
    //                   newData[i].Tel_No?.Replace(" ", "") == added[j].Tel_No?.Replace(" ", "") |
    //                   newData[i].C_Address?.Replace(",", "").Replace(" ", "") == added[j].C_Address?.Replace(",", "").Replace(" ", "")
    //                    )
    //                    break;
    //            if (j == addedLength)
    //            {

    //                LeftIntersectionIGeoComList.Add(newData[i]);
    //            }
    //        }
    //        Console.WriteLine(LeftIntersectionIGeoComList.Count);
    //        return LeftIntersectionIGeoComList;
    //    }

    //    public static List<IGeoComRepository> RightIntersection(List<IGeoComRepository> previousData, List<IGeoComDeltaModel> removed)
    //    {
    //        int previousLength = previousData.Count;
    //        int removedLength = removed.Count;
    //        List<IGeoComRepository> RightIntersectionIGeoComList = new List<IGeoComRepository>();

    //        for (int i = 0; i < previousLength; i++)
    //        {
    //            int j;
    //            for (j = 0; j < removedLength; j++)
    //                if (previousData[i].E_Address?.Replace(",", "").Replace(" ", "") == removed[j].E_Address?.Replace(",", "").Replace(" ", "") |
    //                   previousData[i].Tel_No?.Replace(" ", "") == removed[j].Tel_No?.Replace(" ", "") |
    //                   previousData[i].C_Address?.Replace(",", "").Replace(" ", "") == removed[j].C_Address?.Replace(",", "").Replace(" ", "")
    //                    )
    //                    break;
    //            if (j == removedLength)
    //            {

    //                RightIntersectionIGeoComList.Add(previousData[i]);
    //            }
    //        }
    //        return RightIntersectionIGeoComList;
    //    }

    //    public static List<IGeoComRepository> newModified(List<IGeoComRepository> left, List<IGeoComRepository> right)
    //    {
    //        int leftLength = left.Count;
    //        int rightLength = right.Count;
    //        List<IGeoComRepository> ModifiedIGeoComList = new List<IGeoComRepository>();

    //        for (int i = 0; i < leftLength; i++)
    //        {
    //            int j;
    //            for (j = 0; j < rightLength; j++)
    //                if (left[i].E_Address?.Replace(",", "").Replace(" ", "") == right[j].E_Address?.Replace(",", "").Replace(" ", "") &&
    //                   left[i].Tel_No?.Replace(" ", "") == right[j].Tel_No?.Replace(" ", "") &&
    //                   left[i].C_Address?.Replace(",", "").Replace(" ", "") == right[j].C_Address?.Replace(",", "").Replace(" ", "")
    //                    )
    //                    break;
    //            if (j == rightLength)
    //            {
    //               //left[i].status = "new_modified";
    //                ModifiedIGeoComList.Add(left[i]);
    //            }
    //        }
    //        Console.WriteLine(ModifiedIGeoComList.Count);
    //        return ModifiedIGeoComList;
    //    }

    //    public static List<IGeoComRepository> orgModified(List<IGeoComRepository> previousData, List<IGeoComRepository> right)
    //    {
    //        int leftLength = previousData.Count;
    //        int rightLength = right.Count;
    //        List<IGeoComRepository> ModifiedIGeoComList = new List<IGeoComRepository>();

    //        for (int i = 0; i < leftLength; i++)
    //        {
    //            int j;
    //            for (j = 0; j < rightLength; j++)
    //                if (previousData[i].E_Address?.Replace(",", "").Replace(" ", "") == right[j].E_Address?.Replace(",", "").Replace(" ", "") &&
    //                   previousData[i].Tel_No?.Replace(" ", "") == right[j].Tel_No?.Replace(" ", "") &&
    //                   previousData[i].C_Address?.Replace(",", "").Replace(" ", "") == right[j].C_Address?.Replace(",", "").Replace(" ", "")
    //                    )
    //                    break;
    //            if (j == rightLength)
    //            {
    //                //previousData[i].status = "org_modified";
    //                ModifiedIGeoComList.Add(previousData[i]);
    //            }
    //        }
    //        Console.WriteLine(ModifiedIGeoComList.Count);
    //        return ModifiedIGeoComList;
    //    }

    //    public static List<IGeoComRepository> MergeResults(List<IGeoComRepository> added, List<IGeoComRepository> removed, List<IGeoComRepository> newDelta, List<IGeoComRepository> orgDelta)
    //    {
    //        List<IGeoComRepository> DeltaChange = new List<IGeoComRepository>();
    //        List<IGeoComRepository> mergeAddedAndRemoved = added.Concat(removed).ToList();

    //        foreach (var (v, i) in newDelta.Select((v, i) => (v, i)))
    //        {
    //            DeltaChange.Add(v);
    //            foreach (var item in orgDelta)
    //            {
    //                if (v.E_Address?.Replace(",", "").Replace(" ", "") == item.E_Address?.Replace(",", "").Replace(" ", "") |
    //                   v.Tel_No?.Replace(" ", "") == item.Tel_No?.Replace(" ", "") |
    //                   v.C_Address?.Replace(",", "").Replace(" ", "") == item.C_Address?.Replace(",", "").Replace(" ", "")
    //                    )
    //                {
    //                    DeltaChange.Add(item);
    //                }
    //            }
    //        }
    //        var results = mergeAddedAndRemoved.Concat(DeltaChange).ToList();
    //        /*
    //        foreach(var result in results)
    //        {
    //            result?.E_Address?.Replace(",", "");
    //            result?.C_Address?.Replace(",", "");
    //        }
    //        */
    //        return results;
    //    }
    //}
}
