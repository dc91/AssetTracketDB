using AssetTracketDB;
using Microsoft.EntityFrameworkCore;

LoadQuotes();
static List<Office> LoadOffices()
{
    using var context = new MyDbContext();
    return context.Offices.ToList();
}

List<Office> officeList = LoadOffices();

// -----------------------------//
// -----------Main Menu---------//
// -----------------------------//
bool stay = true;
while (stay)
{
    Console.SetCursorPosition(0, 0);
    ClearLines();
    PrintMainMenu();

    Console.CursorVisible = false;
    Office? chosenOffice = null;

    ConsoleKey key = Console.ReadKey(intercept: true).Key;
    switch (key)
    {
        case ConsoleKey.D1:
            chosenOffice = ChooseOffice(ref officeList);
            if (chosenOffice != null)
                AddForOffice(ref chosenOffice);
            break;
        case ConsoleKey.D2:
            ListAssetsInOffice(ref officeList);
            break;
        case ConsoleKey.D3:
            ListAllAssets(ref officeList);
            break;
        case ConsoleKey.Escape:
            stay = false;
            break;
        default:
            break;
    }
}


// -----------------------------//
// ----------Adding Asset-------//
// -----------------------------//
static Office? ChooseOffice(ref List<Office> officeList)
{
    int selectedOffice = 0;
    bool stay = true;

    while (stay)
    {
        Console.SetCursorPosition(0, 0);
        ClearLines();
        Console.WriteLine("Which office?\n");

        for (int i = 0; i < officeList.Count; i++)
        {
            if (i == selectedOffice)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("> " + officeList[i].Name);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("  " + officeList[i].Name);
            }
        }

        ConsoleKey key = Console.ReadKey(intercept: true).Key;

        switch (key)
        {
            case ConsoleKey.DownArrow:
                if (selectedOffice < officeList.Count - 1)
                    selectedOffice++;
                break;
            case ConsoleKey.UpArrow:
                if (selectedOffice > 0)
                    selectedOffice--;
                break;
            case ConsoleKey.Enter:
                return officeList[selectedOffice];
            case ConsoleKey.Escape:
                stay = false;
                break;
        }
    }
    return null;
}

static void AddForOffice(ref Office office)
{
    Console.Clear();
    List<string> types = new() { "Laptop", "MobilePhone" };
    int selectedType = 0;

    bool stay = true;
    while (stay)
    {
        Console.CursorVisible = false;
        string? assetType = SelectAssetType(types, ref selectedType, office);
        if (assetType == null)
            break;

        Console.WriteLine();
        Console.CursorVisible = true;

        string assetManufacturer = GetInput("Manufacturer: ");
        if (assetManufacturer == string.Empty)
            break;

        string assetModelName = GetInput("Model name: ");
        if (assetModelName == string.Empty)
            break;

        decimal assetPrice = GetValidDecimal("Price (Expecting numbers with eventual comma to signify decimals): ");
        if (assetPrice == -1)
            break;

        string? priceCurrency = GetValidCurrency("Currency (USD, EUR or SEK): ");
        if (priceCurrency == null)
            break;

        using var context = new MyDbContext();
        decimal convertedPrice = assetPrice;
        if (priceCurrency != office.Currency)
        {
            string pair = priceCurrency + office.Currency;
            var conversion = context.Currencies.FirstOrDefault(c => c.Name == pair);
            if (conversion != null)
            {
                convertedPrice = assetPrice * conversion.Rate;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Currency conversion rate not found. Asset not added.");
                Console.ResetColor();
                return;
            }
        }

        DateOnly purchaseDate = GetValidDate("Purchase Date (yyyy-mm-dd): ");
        if (purchaseDate == default) break;

        Console.CursorVisible = false;
        if (assetType == "Laptop")
        {
            AddLaptop(office.Name, assetManufacturer, assetModelName, convertedPrice, office.Currency, purchaseDate);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\nLaptop added. Press any key to go back to Main Menu");
            Console.ResetColor();
            Console.ReadKey(intercept: true);
            return;
        }
        else if (assetType == "MobilePhone")
        {
            AddMobile(office.Name, assetManufacturer, assetModelName, convertedPrice, office.Currency, purchaseDate);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\nMobilePhone added. Press any key to go back to Main Menu");
            Console.ResetColor();
            Console.ReadKey(intercept: true);
            return;
        }
    }
}

static string? SelectAssetType(List<string> types, ref int selectedType, Office office)
{
    while (true)
    {
        Console.SetCursorPosition(0, 0);
        Console.WriteLine($"Adding an Asset for {office.Name} office. \nPress ESC to cancel.\n");
        Console.WriteLine("Laptop or Mobile phone: ");
        for (int i = 0; i < types.Count; i++)
        {
            if (i == selectedType)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("> " + types[i] + "\t");
                Console.ResetColor();
            }
            else
            {
                Console.Write("  " + types[i] + "\t");
            }
        }

        ConsoleKey key = Console.ReadKey(intercept: true).Key;
        switch (key)
        {
            case ConsoleKey.RightArrow:
                if (selectedType < types.Count - 1)
                    selectedType++;
                break;
            case ConsoleKey.LeftArrow:
                if (selectedType > 0)
                    selectedType--;
                break;
            case ConsoleKey.Enter:
                return types[selectedType];
            case ConsoleKey.Escape:
                return null;
        }
    }
}

// -----------------------------//
// ---------Listing ONE---------//
// -----------------------------//

static void ListAssetsInOffice(ref List<Office> officeList)
{
    using var context = new MyDbContext();

    Office office = officeList[0];
    int sortingOption = 1;
    bool filtered = false;
    bool stay = true;

    int selectedIndex = 0;

    ReloadOfficeAssets(context, office);

    string usdPair = office.Currency + "USD";
    var usdConversion = context.Currencies.FirstOrDefault(c => c.Name == usdPair);
    decimal usdRate = usdConversion?.Rate ?? 1m;

    while (stay)
    {
        Console.SetCursorPosition(0, 0);
        ClearLines();
        PrintSortOptions(office);

        List<Asset> sortedAssets = sortingOption switch
        {
            1 => office.Assets.OrderBy(a => a.Type).ToList(),
            2 => office.Assets.OrderBy(a => a.PurchaseDate).ToList(),
            _ => office.Assets
        };

        if (filtered)
            sortedAssets = sortedAssets.Where(a => a.EndOfLifeStatus != "Normal").ToList();

        if (selectedIndex >= sortedAssets.Count)
            selectedIndex = sortedAssets.Count - 1;
        if (selectedIndex < 0)
            selectedIndex = 0;

        PrintList(sortedAssets, office, usdRate, selectedIndex);

        ConsoleKey key = Console.ReadKey(intercept: true).Key;
        switch (key)
        {
            case ConsoleKey.D1:
                sortingOption = 1;
                break;
            case ConsoleKey.D2:
                sortingOption = 2;
                break;
            case ConsoleKey.U:
                office = officeList.FirstOrDefault(o => o.Name == "USA") ?? office;
                break;
            case ConsoleKey.S:
                office = officeList.FirstOrDefault(o => o.Name == "SWE") ?? office;
                break;
            case ConsoleKey.F:
                office = officeList.FirstOrDefault(o => o.Name == "FIN") ?? office;
                break;
            case ConsoleKey.N:
                filtered = !filtered;
                break;
            case ConsoleKey.UpArrow:
                selectedIndex--;
                break;
            case ConsoleKey.DownArrow:
                selectedIndex++;
                break;
            case ConsoleKey.Enter:
                if (sortedAssets.Count > 0)
                {
                    EditAsset(sortedAssets[selectedIndex]);

                    ReloadOfficeAssets(context, office);
                }
                break;
            case ConsoleKey.Delete:
                if (sortedAssets.Count > 0)
                {
                    DeleteAsset(sortedAssets[selectedIndex]);

                    ReloadOfficeAssets(context, office);
                }
                break;

            case ConsoleKey.Escape:
                stay = false;
                break;
        }

        ReloadOfficeAssets(context, office);

        usdPair = office.Currency + "USD";
        usdConversion = context.Currencies.FirstOrDefault(c => c.Name == usdPair);
        usdRate = usdConversion?.Rate ?? 1m;
    }
}

static void ReloadOfficeAssets(MyDbContext context, Office office)
{
    var laptops = context.Laptops.Where(l => l.OfficeId == office.Id).ToList<Asset>();
    var mobiles = context.Mobiles.Where(m => m.OfficeId == office.Id).ToList<Asset>();
    office.Assets = laptops.Concat(mobiles).ToList();
}

static void PrintList(List<Asset> sortedAssets, Office office, decimal usdRate, int selectedIndex)
{
    Console.WriteLine("| {0,-15} | {1,-20} | {2,-20} | {3,-20} | {4,-10} | {5,-15}",
        "Asset type:", "Manufacturer:", "Model name:", "Local price:", "USD price:", "Date:");
    Console.WriteLine(new string('-', 110));

    for (int i = 0; i < sortedAssets.Count; i++)
    {
        Asset asset = sortedAssets[i];

        if (asset.EndOfLifeStatus == "Past End of Life")
            Console.ForegroundColor = ConsoleColor.DarkGray;
        else if (asset.EndOfLifeStatus == "3 Months Left")
            Console.ForegroundColor = ConsoleColor.Red;
        else if (asset.EndOfLifeStatus == "6 Months Left")
            Console.ForegroundColor = ConsoleColor.Yellow;

        if (i == selectedIndex)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
        }

        decimal usdPrice = asset.Price * usdRate;

        Console.WriteLine("| {0,-15} | {1,-20} | {2,-20} | {3,-20} | {4,-10:N2} | {5,-15}",
            asset.Type,
            asset.Manufacturer,
            asset.ModelName,
            $"{Math.Round(asset.Price, 2)} {office.Currency}",
            $"{Math.Round(usdPrice, 2)}",
            asset.PurchaseDate.ToString());

        Console.ResetColor();
    }
}

// -----------------------------//
// ---------Listing ALL---------//
// -----------------------------//
static void ListAllAssets(ref List<Office> officeList)
{
    using var context = new MyDbContext();

    var usdRates = context.Currencies
        .Where(c => c.Name.EndsWith("USD"))
        .ToDictionary(c => c.Name.Substring(0, 3), c => c.Rate);
    usdRates["USD"] = 1m; // for "USD->USD"

    int selectedIndex = 0;

    int sortingOption = 1;
    bool filtered = false;
    bool stay = true;

    
    int tableStart = Console.CursorTop;

    foreach (var office in officeList)
    {
        var laptops = context.Laptops.Include(l => l.Office).Where(l => l.OfficeId == office.Id).ToList<Asset>();
        var mobiles = context.Mobiles.Include(m => m.Office).Where(m => m.OfficeId == office.Id).ToList<Asset>();
        office.Assets = laptops.Concat(mobiles).ToList();
    }

    while (stay)
    {
        List<Asset> allAssets = officeList.SelectMany(o => o.Assets).ToList();

        List<Asset> sortedAssets = sortingOption switch
        {
            1 => allAssets.OrderBy(a => a.Office.Name).ToList(),
            2 => allAssets.OrderBy(a => a.PurchaseDate).ToList(),
            _ => allAssets
        };

        if (filtered)
            sortedAssets = sortedAssets.Where(a => a.EndOfLifeStatus != "Normal").ToList();

        if (selectedIndex >= sortedAssets.Count)
            selectedIndex = sortedAssets.Count - 1;
        if (selectedIndex < 0)
            selectedIndex = 0;

        Console.SetCursorPosition(0, 0);
        ClearLines();
        PrintSortOptionsAll();
        PrintListAll(sortedAssets, usdRates, selectedIndex);

        ConsoleKey key = Console.ReadKey(intercept: true).Key;

        switch (key)
        {
            case ConsoleKey.D1:
                sortingOption = 1;
                break;
            case ConsoleKey.D2:
                sortingOption = 2;
                break;
            case ConsoleKey.N:
                filtered = !filtered;
                break;
            case ConsoleKey.UpArrow:
                selectedIndex--;
                break;
            case ConsoleKey.DownArrow:
                selectedIndex++;
                break;
            case ConsoleKey.Enter:
                if (sortedAssets.Count > 0)
                {
                    EditAsset(sortedAssets[selectedIndex]);

                    foreach (var office in officeList)
                    {
                        ReloadOfficeAssets(context, office);
                    }
                }
                break;
            case ConsoleKey.Delete:
                if (sortedAssets.Count > 0)
                {
                    DeleteAsset(sortedAssets[selectedIndex]);

                    foreach (var office in officeList)
                    {
                        ReloadOfficeAssets(context, office);
                    }
                }
                break;

            case ConsoleKey.Escape:
                stay = false;
                break;
        }
    }
}

static void PrintListAll(List<Asset> sortedAssets, Dictionary<string, decimal> usdRates, int selectedIndex)
{
    Console.WriteLine("| {0,-10} | {1,-15} | {2,-15} | {3,-20} | {4,-15} | {5,-10} | {6,-15}",
        "Office:", "Type:", "Manufacturer:", "Model name:", "Local price:", "USD price:", "Date:");
    Console.WriteLine(new string('-', 115));

    for (int i = 0; i < sortedAssets.Count; i++)
    {
        Asset asset = sortedAssets[i];

        if (asset.EndOfLifeStatus == "Past End of Life")
            Console.ForegroundColor = ConsoleColor.DarkGray;
        else if (asset.EndOfLifeStatus == "3 Months Left")
            Console.ForegroundColor = ConsoleColor.Red;
        else if (asset.EndOfLifeStatus == "6 Months Left")
            Console.ForegroundColor = ConsoleColor.Yellow;

        if (i == selectedIndex)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
        }

        decimal officeToUsd = 1m;
        if (usdRates.TryGetValue(asset.Currency, out decimal value))
            officeToUsd = value;
        decimal usdPrice = asset.Price * officeToUsd;

        Console.WriteLine("| {0,-10} | {1,-15} | {2,-15} | {3,-20} | {4,-15} | {5,-10:N2} | {6,-15}",
            asset.Office.Name,
            asset.Type,
            asset.Manufacturer,
            asset.ModelName,
            $"{Math.Round(asset.Price, 2)} {asset.Currency}",
            Math.Round(usdPrice, 2),
            asset.PurchaseDate.ToString());

        Console.ResetColor();
    }
}

static void PrintSortOptionsAll()
{
    Console.WriteLine("How would you like to sort the list?" + "\t\t\t" + "Color Code:");
    Console.Write("[1] By Office" + "\t\t\t\t\t\t");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("6 months until end-of-life");
    Console.ResetColor();
    Console.Write("[2] By Purchase Date" + "\t\t\t\t\t");
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("3 months until end-of-life");
    Console.ResetColor();
    Console.Write("[N] Toggle Danger Zone" + "\t\t\t\t\t");
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine("Past end-of-life");
    Console.ResetColor();
    Console.WriteLine("\n[ESC] Go Back");
    Console.WriteLine();
}


// -----------------------------//
// ------ Edit / Delete --------//
// -----------------------------//

static void EditAsset(Asset asset)
{
    string[] fields = new string[]
    {
        "Manufacturer",
        "Model Name",
        "Price",
        "Currency",
        "Purchase Date"
    };

    int selectedFieldIndex = 0;
    bool stay = true;

    int currPos = Console.CursorTop;
    while (stay)
    {
        Console.SetCursorPosition(0, 0);
        ClearLines();
        Console.WriteLine($"Editing Asset (ID={asset.Id}, Type={asset.Type})\n");
        Console.WriteLine("Use Up/Down arrows to select a field, Enter to edit, Esc to go back.\n");

        for (int i = 0; i < fields.Length; i++)
        {
            string fieldName = fields[i];

            string currentValue = fieldName switch
            {
                "Manufacturer" => asset.Manufacturer,
                "Model Name" => asset.ModelName,
                "Price" => asset.Price.ToString(),
                "Currency" => asset.Currency,
                "Purchase Date" => asset.PurchaseDate.ToString(),
                _ => "???"
            };

            if (i == selectedFieldIndex)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
                Console.WriteLine($"> {fieldName}: {currentValue}");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"  {fieldName}: {currentValue}");
            }
        }

        var key = Console.ReadKey(true).Key;
        switch (key)
        {
            case ConsoleKey.UpArrow:
                selectedFieldIndex--;
                if (selectedFieldIndex < 0) selectedFieldIndex = 0;
                break;
            case ConsoleKey.DownArrow:
                selectedFieldIndex++;
                if (selectedFieldIndex >= fields.Length) selectedFieldIndex = fields.Length - 1;
                break;
            case ConsoleKey.Enter:
                EditAssetField(asset, selectedFieldIndex);
                stay = false;
                break;
            case ConsoleKey.Escape:
                stay = false;
                break;
        }
    }
}

static void EditAssetField(Asset asset, int fieldIndex)
{
    using var context = new MyDbContext();
    int currPos = Console.CursorTop - 5 + fieldIndex;
    Console.SetCursorPosition(0, currPos);

    string fieldName;
    string oldValueStr;
    switch (fieldIndex)
    {
        case 0:
            fieldName = "Manufacturer";
            oldValueStr = asset.Manufacturer;
            break;
        case 1:
            fieldName = "Model Name";
            oldValueStr = asset.ModelName;
            break;
        case 2:
            fieldName = "Price";
            oldValueStr = asset.Price.ToString();
            break;
        case 3:
            fieldName = "Currency";
            oldValueStr = asset.Currency;
            break;
        case 4:
            fieldName = "Purchase Date";
            oldValueStr = asset.PurchaseDate.ToString();
            break;
        default:
            return;
    }
        
    string input = GetOptionalInput($"> {fieldName}: ");

    if (input == null || input.Trim() == "")
        return;

    bool valid = true;
    switch (fieldIndex)
    {
        case 0:
            asset.Manufacturer = input;
            break;
        case 1:
            asset.ModelName = input;
            break;
        case 2:
            if (decimal.TryParse(input, out decimal newPrice))
            {
                asset.Price = newPrice;
            }
            else
            {
                valid = false;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid decimal number. Press any key to continue...");
                Console.ResetColor();
                Console.ReadKey(true);
            }
            break;
        case 3:
            string upper = input.ToUpper();
            if (upper == "USD" || upper == "EUR" || upper == "SEK")
            {
                asset.Currency = upper;
            }
            else
            {
                valid = false;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid currency. Use USD, EUR, or SEK. Press any key to continue...");
                Console.ResetColor();
                Console.ReadKey(true);
            }
            break;
        case 4:
            if (DateOnly.TryParse(input, out DateOnly newDate))
            {
                asset.PurchaseDate = newDate;
            }
            else
            {
                valid = false;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid date. Format: yyyy-MM-dd. Press any key to continue...");
                Console.ResetColor();
                Console.ReadKey(true);
            }
            break;
    }

    if (valid)
    {
        if (asset is Laptop laptop)
            context.Laptops.Update(laptop);
        else if (asset is Mobile mobile)
            context.Mobiles.Update(mobile);

        context.SaveChanges();

        Console.SetCursorPosition(0, Console.CursorTop + 5 - fieldIndex);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{fieldName} updated successfully! Press any key to continue...");
        Console.ResetColor();
        Console.ReadKey(true);
    }
}

static void DeleteAsset(Asset asset)
{
    using var context = new MyDbContext();

    Console.SetCursorPosition(0, 0);
    ClearLines();
    Console.WriteLine($"Deleting Asset (ID = {asset.Id}):");
    Console.WriteLine($"{asset.Type} - {asset.Manufacturer} - {asset.ModelName}");
    Console.Write("\nAre you sure you want to DELETE this row? (Y/N): ");

    var key = Console.ReadKey(intercept: true);
    Console.WriteLine();
    if (key.Key == ConsoleKey.Y)
    {
        if (asset is Laptop)
        {
            context.Laptops.Remove((Laptop)asset);
        }
        else if (asset is Mobile)
        {
            context.Mobiles.Remove((Mobile)asset);
        }
        context.SaveChanges();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Asset deleted! Press any key to continue...");
        Console.ResetColor();
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Deletion canceled. Press any key to continue...");
        Console.ResetColor();
    }
    Console.ReadKey(true);
}

static string? GetOptionalInput(string prompt)
{
    Console.WriteLine(new string(' ', Console.WindowWidth));
    Console.SetCursorPosition(0, Console.CursorTop-1);
    Console.Write(prompt); 
    

    string input = string.Empty;
    while (true)
    {
        Console.CursorVisible = true;
        var key = Console.ReadKey(true);
        Console.CursorVisible = false;
        if (key.Key == ConsoleKey.Escape)
        {
            return null;
        }
        else if (key.Key == ConsoleKey.Enter)
        {
            Console.WriteLine();
            return input;
        }
        else if (key.Key == ConsoleKey.Backspace)
        {
            if (input.Length > 0)
            {
                input = input.Substring(0, input.Length - 1);
                int cursorLeft = Console.CursorLeft;
                if (cursorLeft > 0)
                {
                    Console.SetCursorPosition(cursorLeft - 1, Console.CursorTop);
                    Console.Write(' ');
                    Console.SetCursorPosition(cursorLeft - 1, Console.CursorTop);
                }
            }
        }
        else if (!char.IsControl(key.KeyChar) && key.KeyChar != '§')
        {
            Console.Write(key.KeyChar);
            input += key.KeyChar;
        }
    }
    
}



// -----------------------------//
// ----------DB Stuff-----------//
// -----------------------------//
static void LoadQuotes()
{
    using var context = new MyDbContext();
    var currencyQuotes = LiveCurrency.FetchRates()
                                     .ToDictionary(q => q.Key, q => q.Value);

    var codes = new[] { "EUR", "USD", "SEK" };

    var newRates = new List<Currencies>();
    foreach (var from in codes)
    {
        foreach (var to in codes)
        {
            if (from == to)
                continue;

            decimal rate = (from == "EUR" ? 1 : 1 / currencyQuotes[from])
                         * (to == "EUR" ? 1 : currencyQuotes[to]);

            string pairName = from + to;

            newRates.Add(new Currencies
            {
                Name = pairName,
                Rate = rate
            });
        }
    }

    var existingRows = context.Currencies
                              .ToDictionary(c => c.Name, c => c);

    foreach (var nr in newRates)
    {
        if (existingRows.TryGetValue(nr.Name, out var existingCurrency))
            existingCurrency.Rate = nr.Rate;
        else
            context.Currencies.Add(nr);
    }

    context.SaveChanges();
}

static void AddLaptop(string officeName, string manufacturer, string modelName, decimal price, string currency, DateOnly purchaseDate)
{
    using var context = new MyDbContext();
    var office = context.Offices.FirstOrDefault(o => o.Name == officeName);

    var newLaptop = new Laptop
    {
        Manufacturer = manufacturer,
        ModelName = modelName,
        Price = price,
        Currency = currency,
        PurchaseDate = purchaseDate,
        OfficeId = office.Id
    };

    context.Laptops.Add(newLaptop);
    context.SaveChanges();
}

static void AddMobile(string officeName, string manufacturer, string modelName, decimal price, string currency, DateOnly purchaseDate)
{
    using var context = new MyDbContext();
    var office = context.Offices.FirstOrDefault(o => o.Name == officeName);

    var newMobile = new Mobile
    {
        Manufacturer = manufacturer,
        ModelName = modelName,
        Price = price,
        Currency = currency,
        PurchaseDate = purchaseDate,
        OfficeId = office.Id
    };

    context.Mobiles.Add(newMobile);
    context.SaveChanges();
}


// -----------------------------//
// -------Input validation------//
// -----------------------------//

static decimal GetValidDecimal(string prompt)
{
    while (true)
    {
        string input = GetInput(prompt);
        if (input == string.Empty)
            return -1;

        if (decimal.TryParse(input, out decimal result))
            return result;

        Console.SetCursorPosition(0, Console.CursorTop - 2);
        Console.WriteLine(new string(' ', Console.WindowWidth));
        Console.WriteLine(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, Console.CursorTop - 2);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("Price not recognized. ");
        Console.ResetColor();
    }
}

static string? GetValidCurrency(string prompt)
{
    while (true)
    {
        string input = GetInput(prompt).ToUpper();
        if (input == string.Empty)
            return null;

        if (input == "USD" || input == "EUR" || input == "SEK")
            return input;

        Console.SetCursorPosition(0, Console.CursorTop - 2);
        Console.WriteLine(new string(' ', Console.WindowWidth));
        Console.WriteLine(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, Console.CursorTop - 2);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("Currency not recognized. ");
        Console.ResetColor();
    }
}

static DateOnly GetValidDate(string prompt)
{
    while (true)
    {
        string input = GetInput(prompt).Trim();
        if (input == string.Empty)
            return default;

        if (DateOnly.TryParse(input, out DateOnly result))
            return result;


        Console.SetCursorPosition(0, Console.CursorTop - 2);
        Console.WriteLine(new string(' ', Console.WindowWidth));
        Console.WriteLine(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, Console.CursorTop - 2);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("Date not recognized. ");
        Console.ResetColor();
    }
}

static string GetInput(string prompt)
{
    string input = string.Empty;
    Console.WriteLine(prompt);
    while (true)
    {
        ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
        if (keyInfo.Key == ConsoleKey.Escape)
            return string.Empty;
        else if (keyInfo.Key == ConsoleKey.Enter)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.WriteLine(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Does not accept empty input. ");
                Console.ResetColor();
                Console.WriteLine(prompt);
                continue;
            }
            else
            {
                Console.WriteLine();
                return input;
            }
        }
        else if (keyInfo.Key == ConsoleKey.Backspace)
        {
            if (input.Length > 0)
            {
                input = input.Substring(0, input.Length - 1);
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                Console.Write(' ');
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            }
        }
        else if (!char.IsControl(keyInfo.KeyChar) && keyInfo.KeyChar != '§')
        {
            Console.Write(keyInfo.KeyChar);
            input += keyInfo.KeyChar;
        }
    }
}


// -----------------------------//
// ----------Print stuff--------//
// -----------------------------//
static void PrintMainMenu()
{
    Console.ForegroundColor = ConsoleColor.Blue;
    Console.WriteLine("Welcome to Your Asset Tracker\n");
    Console.ResetColor();
    Console.WriteLine("[1] Add an Asset to an Office");
    Console.WriteLine("[2] View/Manage Assets per Office (CRUD)");
    Console.WriteLine("[3] View/Manage All Assets (CRUD)");
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine("\n[ESC] quit.");
    Console.ResetColor();
}

static void PrintSortOptions(Office office)
{
    Console.WriteLine($"Sort the assets of {office.Name} office?" + "\t\t\t" + "Change Office?" + "\t\t\t" + "Color Code:");
    Console.Write("[1] By Asset Type" + "\t\t\t\t" + "[U] USA Office" + "\t\t\t");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("6 months until end-of-life");
    Console.ResetColor();
    Console.Write("[2] By Purchase Date" + "\t\t\t\t" + "[S] SWE Office" + "\t\t\t");
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("3 months until end-of-life");
    Console.ResetColor();
    Console.Write("[N] Toggle Danger Zone" + "\t\t\t\t" + "[F] FIN Office" + "\t\t\t");
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine("Past end-of-life");
    Console.ResetColor();
    Console.WriteLine("\n[ESC] Go Back | [Up]/[Down] to Navigate | [Enter] to Edit | [Del] to Delete");
    Console.WriteLine();
}

static void ClearLines()
{
    var currPos = Console.GetCursorPosition();
    for (int i = 0; i < Console.WindowHeight - currPos.Top - 1; i++)
    {
        Console.WriteLine(new string(' ', Console.WindowWidth));
    }
    Console.SetCursorPosition(currPos.Left, currPos.Top);
}
