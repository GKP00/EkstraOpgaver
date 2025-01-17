﻿using System.Text;

namespace EkstraOpgaver
{
  class KonverterTalsystemer
  {
    enum Talsystem
    {
      BIN =  2,
      OCT =  8,
      DEC = 10,
      HEX = 16,
    }

    static Dictionary<Talsystem, string[]> AlternativePrefixes = new Dictionary<Talsystem, string[]>
    {
      {Talsystem.BIN, new string[]{"0b"} },
      {Talsystem.OCT, new string[]{"0o"} },
      {Talsystem.HEX, new string[]{"0x"} },
    };

    //konverterer fra char til int værdi 
    static int CifferTilVærdi(char cifferChar)
    {
      //finder ciffer værdien ved at udregne cifferets offset i ASCII tabellen fra '0' når cifferet er mellem '0' og '9'
      //ved alfabetiske værdier (a-z) udregner jeg offsetet fra 'a' når cifferet er mellem 'a' og 'z' og lægger 10 til da efter '9'
      //vil 'a' være det 10. nummer
      return cifferChar >= '0' && cifferChar <= '9' ? (cifferChar - '0') :
             cifferChar >= 'a' && cifferChar <= 'z' ? (cifferChar - 'a') + 10 :
             cifferChar >= 'A' && cifferChar <= 'Z' ? (cifferChar - 'A') + 10 :
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
    static bool ValiderCiffer(Talsystem talsystem, char cifferChar)
    {
      int cifferVærdi = CifferTilVærdi(cifferChar);
      return cifferVærdi >= 0 && cifferVærdi < (int)talsystem ? true :
             throw new Exception($"ukorrekt ciffer '{cifferChar}' inden for {talsystem} talsystemet");
    }

    //konverterer en tal string til en int værdi
    static int TalTilVærdi(Talsystem talsystem, string tal)
    {
      int talVærdi = 0;
      bool negativ = tal.StartsWith('-');

      //string index er højre til venste, men vi tænker på ciffre i et tal som at gå fra venstre til højre
      //så jeg iterere fra enden a stringen til starten 
      for (int i = tal.Length-1; i >= (negativ ? 1 : 0); --i)
      {
        int plads = tal.Length-1 - i;
        int pladsVærdi  = (int)Math.Pow((int)talsystem, plads); 
        int cifferVærdi = CifferTilVærdi(tal[i]);
        ValiderCiffer(talsystem, tal[i]);

        //talVærdien går på med cifferværdi gange med pladsværdi
        talVærdi += cifferVærdi * pladsVærdi;
      }

      return negativ ? -talVærdi : talVærdi;
    }

    //konverterer en int værdi til en tal string i et talsystem
    static string VærdiTilTal(Talsystem talsystem, int værdi)
    {
      StringBuilder sb = new StringBuilder();

      //jeg håndterer negative værdier ved at lave dem om til positive, og så sætter jeg bare et '-' på inden jeg returnerer
      bool negativ = værdi < 0;
      værdi = Math.Abs(værdi);

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

      if(negativ)
        sb.Insert(0, '-');

      sb.Append($" ({(int)talsystem})"); //til sidst appender jeg et postfix så man kan se hvilke base tallet er
      return sb.ToString();
    }

    //validerer at hver ciffer i tallet er inden for talsystemet
    static void ValiderTal(Talsystem talsystem, string tal)
    {
      for (int i = 0; i < tal.Length; ++i)
      {
        //ignorer evt. minus tegn
        if (i == 0 && tal[i] == '-')
          continue;

        ValiderCiffer(talsystem, tal[i]);
      }
    }

    static (Talsystem talsystem, string tal)? LedEfterTalMedPrimærPrefix(string input, Talsystem talsystem)
    {
      //hver enum værdi i koden er allerede navngivet efter deres prefix, så jeg ToString()'er bare:
      string prefix = talsystem.ToString();

      if (input.StartsWith(prefix))
      {
        string talVærdi = input.Substring(prefix.Length);
        talVærdi = talVærdi.TrimStart(); //fjern evt. mellemrum (fks i "HEX FF")
        return (talsystem, talVærdi);
      }

      return null;
    }

    static (Talsystem talsystem, string tal)? LedEfterTalMedAlternativtPrefix(string input, Talsystem talsystem)
    {
      //led kun efter alternative prefixes hvis det findes for talsystemet:
      if (!AlternativePrefixes.ContainsKey(talsystem))
        return null;

      //de alternative prefixes må gerne have et minus tegn før (fks: -0xFF)
      //jeg håndterer dette ved at fjerne tegnet og så sætte det tilbage efter
      bool negativt = false;
      if (input.StartsWith('-'))
      {
        input = input.TrimStart('-');
        negativt = true;
      }

      //led efter et af de alternative prefixes:
      foreach (string alprefix in AlternativePrefixes[talsystem])
      {
        if (!input.StartsWith(alprefix))
          continue;

        //lav en ny string fra inputtet uden prefixet
        string talVærdi = input.Substring(alprefix.Length);

        //sætter minus tegnet tilbage hvis det var negativt
        if (negativt)
          talVærdi = '-' + talVærdi;

        return (talsystem, talVærdi);
      }

      return null;
    }

    static (Talsystem talsystem, string tal)? LedEfterTalMedPostfix(string input, Talsystem talsystem)
    {
      //hver enum værdi er nummeret på talsystemets base, så postfix stringen kan bare konstrueres:
      string postfix = $"({(int)talsystem})";

      //tjek om inputtet slutter med postfixet
      if (input.EndsWith(postfix))
      {
        //lav en ny string fra inputtet uden postfix
        string talVærdi = input.Substring(0, input.Length - postfix.Length);
        talVærdi = talVærdi.TrimEnd();   //fjern evt. mellemrum i enden (fks: "44 (8)")
        return (talsystem, talVærdi);
      }

      return null;
    }

    //Finder ud af hvilket talsystem inputtet er givet i ved at tjekke for et prefix eller postfix
    //returnerer talsystemet samt efterfølgende eller førfølgende string 
    static (Talsystem talsystem, string tal) AnalyserTal(string input)
    {
      foreach (Talsystem talsystem in Enum.GetValues(typeof(Talsystem)))
      {
        var talOgTalsystem = LedEfterTalMedPrimærPrefix(input,talsystem);
        if (talOgTalsystem != null)
          return talOgTalsystem.Value;

        talOgTalsystem = LedEfterTalMedAlternativtPrefix(input,talsystem);
        if (talOgTalsystem != null)
          return talOgTalsystem.Value;

        talOgTalsystem = LedEfterTalMedPostfix(input,talsystem);
        if (talOgTalsystem != null)
          return talOgTalsystem.Value;
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
          string input = Console.ReadLine().TrimStart().TrimEnd();


          var talOgSystem = AnalyserTal(input);
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


