using System.Collections.Generic;
using UnityEngine;

public static class WeaponPartMerger
{
    private static readonly Dictionary<(WeaponPartId, WeaponPartId), WeaponPartId> mergeRecipes;

    static WeaponPartMerger()
    {
        // Tarifleri doldururken, anahtarýn her zaman küçük olanýn önce gelmesini saðlayarak
        // simetrik giriþ ihtiyacýný ortadan kaldýrýyoruz.
        mergeRecipes = new Dictionary<(WeaponPartId, WeaponPartId), WeaponPartId>
        {
            { (WeaponPartId.Pen, WeaponPartId.Pen), WeaponPartId.LongPen },
            { (WeaponPartId.Apple, WeaponPartId.Apple), WeaponPartId.Pineapple },
            { (WeaponPartId.Apple, WeaponPartId.Pen), WeaponPartId.ApplePen },
            { (WeaponPartId.Pen, WeaponPartId.Pineapple), WeaponPartId.PineapplePen },
            { (WeaponPartId.ApplePen, WeaponPartId.PineapplePen), WeaponPartId.PPAP },
            { (WeaponPartId.LongPen, WeaponPartId.Pineapple), WeaponPartId.PPAP }
        };
    }

    /// <summary>
    /// Ýki parçanýn birleþtirilip birleþtirilemeyeceðini kontrol eder ve sonuç parçayý döndürür.
    /// </summary>
    public static bool CanMerge(WeaponPartId part1, WeaponPartId part2, out WeaponPartId resultPart)
    {
        if (part1 == WeaponPartId.None || part2 == WeaponPartId.None)
        {
            resultPart = WeaponPartId.None;
            return false;
        }

        // Anahtarý her zaman (küçük, büyük) sýrasýna göre oluþturarak simetriyi yönet.
        // Örn: (Apple, Pen) ve (Pen, Apple) ikisi de (Apple, Pen) anahtarýný kullanýr.
        var key = part1 < part2 ? (part1, part2) : (part2, part1);
        
        // Eðer parçalar aynýysa, sýralama mantýðý çalýþmaz, bu yüzden anahtarý doðrudan oluþtur.
        if (part1 == part2)
        {
            key = (part1, part2);
        }

        // Tarif defterinde anahtarý ara ve sonucu döndür.
        if (mergeRecipes.TryGetValue(key, out resultPart))
        {
            return true;
        }

        resultPart = WeaponPartId.None;
        return false;
    }
}

