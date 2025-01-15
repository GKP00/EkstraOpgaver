Console.Write("Indtast en IP: ");
var ipOgCidr = AnalyserIpInput(Console.ReadLine()!);

uint maksUndernetværk = MaksAntalUndernetværk(ipOgCidr.cidr);
Console.Write($"Indtast antal undernetværk du vil have: (1-{maksUndernetværk}): ");

uint antalUndernetværk = uint.Parse(Console.ReadLine()!);
Console.WriteLine();

if(antalUndernetværk > maksUndernetværk)
    throw new Exception("for mange undernetværk.");

uint subnetBits = NødvendigeBitsForAntalUndernetværk(antalUndernetværk);
Console.WriteLine($"Antal subnet bits: {subnetBits} ({Math.Pow(2, subnetBits)} subnets)");

uint anvendeligeAddresser = AntalAnvendeligeAddresser(ipOgCidr.cidr, subnetBits);
Console.WriteLine($"Antal anvendelige addresser per subnet: {anvendeligeAddresser}");
Console.WriteLine();

string ipSomBinær = string.Join("", ipOgCidr.octets.Select(e => Convert.ToString(e, 2).PadLeft(8,'0')));
if(ipSomBinær.Length != 32) 
    throw new Exception("der skete en fejl.");

Console.WriteLine("Her er din IP i farver:");

PrintMedFarve('X', ConsoleColor.Blue);
Console.WriteLine(" = netværk bits");
PrintMedFarve('X', ConsoleColor.Red);
Console.WriteLine(" = subnet bits");

Console.WriteLine();

for(int i = 0; i < ipSomBinær.Length; ++i)
{
  PrintMedFarve(ipSomBinær[i],
    i < ipOgCidr.cidr ? ConsoleColor.Blue :
    i < ipOgCidr.cidr+subnetBits ? ConsoleColor.Red : null);

  if(i+1 < ipSomBinær.Length && (i+1) % 8 == 0)
    Console.Write('.');
}

void PrintMedFarve(char str, ConsoleColor? farve)
{
  if (farve == null)
  {
    Console.ResetColor();
    Console.Write(str);
    return;
  }

  Console.ForegroundColor = farve.Value;
  Console.Write(str);
  Console.ResetColor();
}

uint NødvendigeBitsForAntalUndernetværk(uint undernetværk)
{
  uint subnetBits = 1;
  while (Math.Pow(2, subnetBits) < antalUndernetværk)
    ++subnetBits;

  return subnetBits;
}

uint AntalAnvendeligeAddresser(uint networkBits, uint subnetBits)
{
  return (uint)Math.Pow(2, 32 - (networkBits + subnetBits))-2;
}

uint MaksAntalUndernetværk(uint networkBits)
{
  return (uint)Math.Pow(2, ((32 - networkBits) - 2));
}

(uint[] octets, uint cidr) AnalyserIpInput(string input)
{
  string[] octetStrs = input.Split('.');
  string? cidrStr = null;

  if(octetStrs.Length != 4)
    throw new Exception("ugyldigt input.");

  //tjek om cidr er angivet i inputtet
  if (octetStrs[3].Contains('/'))
  {
    string[] sidsteOctetOgCidr = octetStrs[3].Split('/');
    octetStrs[3] = sidsteOctetOgCidr[0];
    cidrStr = sidsteOctetOgCidr[1];
  }

  uint[] octets = octetStrs.Select(e => uint.Parse(e)).ToArray();
  uint cidr = cidrStr != null ? uint.Parse(cidrStr) :
              octets[0] >= 192 ? (uint)24 :
              octets[0] >= 128 ? (uint)16 :
              octets[0] >= 1   ? (uint)8 : 
              throw new Exception("ugyldigt input.");

  return (octets, cidr);
}
