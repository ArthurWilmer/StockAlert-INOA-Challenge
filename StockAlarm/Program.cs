using System.Globalization;
using StockAlarm.Services;

static int PrintUsage()
{
    Console.WriteLine("Uso:");
    Console.WriteLine("  StockAlarm.exe <TICKER> <PRECO_VENDA> <PRECO_COMPRA>");
    Console.WriteLine("Exemplo:");
    Console.WriteLine("  StockAlarm.exe PETR4 22.67 22.59");
    return 1;
}

if (args.Length != 3)
    return PrintUsage();

var ticker = args[0].Trim().ToUpperInvariant();

if (!decimal.TryParse(args[1], NumberStyles.Number, CultureInfo.InvariantCulture, out var sellPrice))
{
    Console.WriteLine("PRECO_VENDA inválido. Use ponto como separador decimal (ex: 22.67).");
    return PrintUsage();
}

if (!decimal.TryParse(args[2], NumberStyles.Number, CultureInfo.InvariantCulture, out var buyPrice))
{
    Console.WriteLine("PRECO_COMPRA inválido. Use ponto como separador decimal (ex: 22.59).");
    return PrintUsage();
}

Console.WriteLine($"Monitorando {ticker} | venda: {sellPrice} | compra: {buyPrice}");

// ---------------------------------
// 2) Leitura do arquivo config.json
// ---------------------------------
var configPath = Path.Combine(AppContext.BaseDirectory, "config.json");

try
{
    var config = ConfigLoader.Load(configPath);
    Console.WriteLine($"Config carregado com sucesso. E-mails serão enviados para: {config.EmailTo}");

    // -----------------------------
    // 3) TESTE BRAPI (1 consulta)
    // -----------------------------
    try
    {
        using var httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };

        var quoteService = new QuoteService(httpClient);

        Console.WriteLine("Consultando preço na BRAPI...");
        var price = await quoteService.GetRegularMarketPriceAsync(ticker, config.BrapiToken, CancellationToken.None);

        Console.WriteLine($"Preço atual ({ticker}): R$ {price.ToString(CultureInfo.InvariantCulture)}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Falha ao consultar BRAPI: {ex.Message}");
    }
}

catch (Exception ex)
{
    Console.WriteLine("Erro ao carregar configuração ou enviar e-mail:");
    Console.WriteLine(ex.Message);
    Console.WriteLine("Verifique o arquivo config.json (host, porta, SSL e credenciais SMTP).");
    return 1;
}

return 0;
