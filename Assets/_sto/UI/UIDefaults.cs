
public class UIDefaults
{
  public static string staminaIco => @"<sprite name=""icoStamina"">";
  public static string gemIco => @"<sprite name=""icoGem"">";
  public static string coinIco => @"<sprite name=""icoCoin"">";

  public static string GetStaminaString(int value) => $"{staminaIco}{value}";
  public static string GetGemsString(int value) => $"{gemIco}{value}";
  public static string GetCoinsString(int value) => $"{coinIco}{value}";
}
