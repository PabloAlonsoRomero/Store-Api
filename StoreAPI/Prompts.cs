namespace StoreAPI;

public static class Prompts
{
    public static string GenerateOrdersPrompt(string jsonData)
    {
        return $@"
            Eres un analista experto de datos en retail.
            Analiza los siguientes datos de ordenes, productos y tiendas (en JSON)
            { jsonData }

            Debes responder **exclusivamente** en formato JSON y con esta estructura:
            {{
                ""topProducts"": {{ ""name"":string, ""unitsSold"": int, ""totalRevenue"": double}},
                ""topStore"": {{ ""name"":string, ""totalSales"": double, ""shareOfTotalSales"": double }},
                ""avgSpending"": double,
                ""patterns""[string]: 
            }}
            En el apartado de patterns agrega analisis como: cual es la tienda que más vende, 
            que productos son los que mas dinero dejan por orden.
            Si por alguna razon, no puedes generar esta respuesta valida (por ejemplo, te faltan datos o
            algun error en el formato), responde **SOLO** con el texto: error.
            No me saludes, no me des explicaciones, no me des comentaros y no incluyas texto adicional.
        ";
    }

    public static string GenerateInvoicesPrompt(string jsonData)
    {
        return $@"
            Eres un analista experto en datos financieros de negocios.
            Analiza el siguiente JSON de facturas registradas en el sistema:
            { jsonData }

            Debes responder **exclusivamente** en formato JSON y con esta estructura:
            {{
                ""totalInvoices"": int,
                ""paidInvoices"": int,
                ""unpaidInvoices"": int,
                ""totalRevenue"": double,
                ""averageInvoiceAmount"": double,
                ""commonCurrencies"": [string],
                ""patterns"": [string]
            }}
            En el apartado de patterns incluye observaciones como:
            - Qué porcentaje de facturas están pagadas
            - Qué moneda se usa más
            - Cualquier patrón relevante detectado en los datos

            Si por alguna razón no puedes generar esta respuesta valida (por ejemplo, faltan datos o hay un error en el formato),
            responde **SOLO** con el texto: erro.
            No me saludes, no me des explicaciones, no incluyas comentarios ni texto adicional
        ";
    }
}