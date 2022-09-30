
public class UIDefaults
{
  public static string staminaIco => @"<sprite name=""icoStamina"">";
  public static string gemIco => @"<sprite name=""icoGem"">";
  public static string coinIco => @"<sprite name=""icoCoin"">";

  public static string GetStaminaString(int value) => $"{staminaIco}{value}";
  public static string GetGemsString(int value) => $"{gemIco}{value}";
  public static string GetCoinsString(int value) => $"{coinIco}{value}";

  public static string GetResString(Item.ID id)
  {
    string str = "";
    int amount = GameData.Econo.GetResCount(id);
    if(id.kind == Item.Kind.Stamina)
      str = UIDefaults.GetStaminaString(amount);
    else if(id.kind == Item.Kind.Coin)
      str = UIDefaults.GetCoinsString(amount);
    else if(id.kind == Item.Kind.Gem)
      str = UIDefaults.GetGemsString(amount);
    
    return str;
  }

  public static string GetResSymbol(Item.ID id)
  {
    string str = "";
    if(id.kind == Item.Kind.Stamina)
      str = staminaIco;
    else if(id.kind == Item.Kind.Coin)
      str = coinIco;
    else if(id.kind == Item.Kind.Gem)
      str = gemIco;
    return str;  
  }
}
