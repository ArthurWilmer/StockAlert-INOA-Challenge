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

    // ---------------------------------
    // 3) TESTE DE SMTP (temporário)
    // ---------------------------------
    var emailService = new EmailService(config);

    Console.WriteLine("Enviando e-mail de teste...");
    emailService.Send(
        subject: "Teste SMTP - Stock Quote Alert",
        body:
            "Este é um e-mail de teste do projeto Stock Quote Alert.\n\n" +
            $"Ticker informado: {ticker}\n" +
            $"Preço venda: {sellPrice}\n" +
            $"Preço compra: {buyPrice}\n" +
            $"Data/Hora: {DateTime.Now}"
    );

    Console.WriteLine("E-mail de teste enviado com sucesso!");
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine("Erro ao carregar configuração ou enviar e-mail:");
    Console.WriteLine(ex.Message);
    Console.WriteLine("Verifique o arquivo config.json (host, porta, SSL e credenciais SMTP).");
    return 1;
}

