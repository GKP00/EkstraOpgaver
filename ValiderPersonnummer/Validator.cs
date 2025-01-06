using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValiderPersonnummer
{
  static class Validator
  {
    public static bool Validate(string cpr)
    {
      //kontroller at cpr stringen har en længde på 10 (antal kontrol vægte) og at de alle er numeriske tal
      if (cpr.Length != kontrolVægte.Length || cpr.Any(c => c < '0' || c > '9'))
        return false;

      //konverter fra text til talværdi
      int[] ciffere = cpr.Select(c => (int)(c - '0')).ToArray();

      //gang hver ciffer med dets korresponderende kontrol vægt og summer det hele
      int vægtetSum = ciffere.Zip(kontrolVægte, (ciffer, vægt) => ciffer*vægt).Sum();

      //personnummeret er validt hvis det går op med 11
      return (vægtetSum % 11) == 0;
    }

    private static int[] kontrolVægte = { 4, 3, 2, 7, 6, 5, 4, 3, 2, 1};
  }
}
