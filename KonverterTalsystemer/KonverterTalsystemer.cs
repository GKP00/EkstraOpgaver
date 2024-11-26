using System.Text;

namespace EkstraOpgaver
{

  class KonverterTalsystemer
  {
    enum Talsystem
    {
      DEC = 10,
      BIN =  2,
      HEX = 16,
      OCT =  8,
    }

    static Dictionary<Talsystem, string[]> Prefixes = new Dictionary<Talsystem, string[]>()
    {
      {Talsystem.DEC, new string[]{"DEC"} },
      {Talsystem.BIN, new string[]{"BIN","0b"} },
      {Talsystem.HEX, new string[]{"HEX","0x"} },
      {Talsystem.OCT, new string[]{"OCT","0o"} },
    };

    //konverterer fra char til int værdi 
    static int CifferTilVærdi(char cifferChar)
    {
      //finder ciffer værdien ved at udregne cifferets offset i ASCII tabellen fra '0' når cifferet er mellem '0' og '9'
      //ved alfabetiske værdier (a-z) udregner jeg offsetet fra 'a' når cifferet er mellem 'a' og 'z' og lægger 10 til da efter '9'
      //vil 'a' være det 10. nummer
      return cifferChar >= '0' && cifferChar <= '9' ? 0  + (cifferChar - '0') :
             cifferChar >= 'a' && cifferChar <= 'z' ? 10 + (cifferChar - 'a') :
             cifferChar >= 'A' && cifferChar <= 'Z' ? 10 + (cifferChar - 'A') :
             throw new Exception($"ukendt ciffer '{cifferChar}', kunne ikke konvertere det til en værdi.");
    }

    //konverterer en værdi til et enkelt ciffer char (max værdi 35, da det går fra 0-9 og så a-z)
    static char VærdiTilCiffer(int værdi)
    {
      //finder characteren ved at bruge værdien som et offset fra hhv. '0' og 'A' i ASCII tabellen
      return værdi >= 0  && værdi <= 9  ? (char)('0' + værdi) : 
             værdi >= 10 && værdi <= 35 ? (char)('A' + værdi - 10) :
             throw new Exception($"ciffer værdi er for høj til at kunne representeres med 0-9 eller a-z");
    }

    //validerer at værdien er mellem 0 og talsystemets base
    static bool ValiderCiffer(Talsystem talsystem, int cifferVærdi)
    {
      return cifferVærdi >= 0 && cifferVærdi < (int)talsystem ? true :
             throw new Exception($"ukorrekt ciffer inden for {talsystem} talsystemet");
    }

    //konverterer en tal string til en int værdi
    static int TalTilVærdi(Talsystem talsystem, string tal)
    {
      int talVærdi = 0;

      //iterer fra højre til venste i strengen
      for (int i = tal.Length - 1; i >= 0; --i)
      {
        int plads = tal.Length - i - 1;  
        int pladsVærdi  = (int)Math.Pow((int)talsystem, plads); 
        int cifferVærdi = CifferTilVærdi(tal[i]);
        ValiderCiffer(talsystem, cifferVærdi);

        //talVærdien går på med cifferværdi gange med pladsværdi
        talVærdi += cifferVærdi * pladsVærdi;
      }

      return talVærdi;
    }

    //konverterer en int værdi til en tal string i et talsystem
    static string VærdiTilTal(Talsystem talsystem, int værdi)
    {
      StringBuilder sb = new StringBuilder();

      //hvor mange gange basen går op i værdien
      int nGårOpI  = værdi / (int)talsystem;

      //hvor meget der ikke går op i basen, dvs. hvad der må stå på den første plads i tallet
      int nTilbage = værdi % (int)talsystem;
      sb.Insert(0, VærdiTilCiffer(nTilbage));

      //i hver iteration dividere vi nGårOpI med basen igen, så det der er tilbage er hvad der skal stå på næste plads i tallet
      while(nGårOpI != 0)
      {
        nTilbage = nGårOpI % (int)talsystem;
        sb.Insert(0, VærdiTilCiffer(nTilbage));

        nGårOpI = nGårOpI / (int)talsystem;
      }

      sb.Append($" ({(int)talsystem})"); //til sidst appender jeg et postfix så man kan se hvilke base tallet er
      return sb.ToString();
    }

    //validerer at hver ciffer i tallet er inden for talsystemet
    static void ValiderTal(Talsystem talsystem, string tal)
    {
      foreach (char ciffer in tal)
        ValiderCiffer(talsystem, CifferTilVærdi(ciffer));
    }

    //Finder ud af hvilket talsystem inputtet er givet i ved at tjekke for et prefix eller postfix
    //returnerer talsystemet samt efterfølgende eller førfølgende string 
    static (Talsystem talsystem, string tal) AnalyserTal(string input)
    {
      //iterer gennem de definerede talsystemer "Talsystem" 
      foreach(Talsystem talsystem in Enum.GetValues(typeof(Talsystem)))
      {
        //led efter et prefix i de definierede Prefixes for talsystemet:
        foreach (string prefix in Prefixes[talsystem])
        {
          //tjek om inputtet begynder med prefixet
          if (input.StartsWith(prefix))
          {
            //lav en ny string fra inputtet uden prefixet
            string talVærdi = input.Substring(prefix.Length);
            talVærdi = talVærdi.TrimStart(); //fjern evt. mellemrum i starten
            talVærdi = talVærdi.TrimEnd();   //fjern evt. mellemrum i enden
            return (talsystem, talVærdi);
          }
        }

        //hver enum værdi er nummeret på talsystemets base, så postfix stringen kan bare konstrueres:
        string postfix = $"({(int)talsystem})";

        //tjek om inputtet slutter med postfixet
        if (input.EndsWith(postfix))
        {
          //lav en ny string fra inputtet uden postfix
          string talVærdi = input.Substring(0, input.Length-postfix.Length);
          talVærdi = talVærdi.TrimStart(); //fjern evt. mellemrum i starten
          talVærdi = talVærdi.TrimEnd();   //fjern evt. mellemrum i enden
          return (talsystem, talVærdi);
        }
      }

      //Hvis der ikke blev fundet noget prefix eller postfix antager vi at det er DEC
      return (Talsystem.DEC, input);
    }

    //konverterer mellem talsystemer ved først at udregne værdien af et tal i et talsystem via TalTilVærdi()
    //og derefter udregne tallet i et andet talsystem fra værdien via VærdiTilTal()
    static string KonverterTal(Talsystem fraSystem, string fraTal, Talsystem tilSystem)
    {
      return VærdiTilTal(tilSystem, TalTilVærdi(fraSystem, fraTal));
    }

    public static void Main() 
    {
      while(true)
      {
        try
        {
          Console.Write("Indtast et tal (fks: 42, BIN 10, 35 (8), 0xFF): ");
          var talOgSystem = AnalyserTal(Console.ReadLine());
          ValiderTal(talOgSystem.talsystem, talOgSystem.tal);
          Console.Write("Indtast den base du vil konverterer til som et nummer (fks: 2): ");
          var konverterTil = (Talsystem) int.Parse(Console.ReadLine());
          Console.WriteLine(KonverterTal(talOgSystem.talsystem, talOgSystem.tal, konverterTil));
        }
        catch(Exception e)
        {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine(e.Message);
          Console.ResetColor();
        }

      }

    }

  }


}


