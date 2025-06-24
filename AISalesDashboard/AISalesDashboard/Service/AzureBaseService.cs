using Azure.AI.OpenAI;
using Azure;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.AI;


namespace AISalesDashboard
{
    public class AzureBaseService
    {
        #region Fields

        /// <summary>
        /// The EndPoint
        /// </summary>
        internal const string Endpoint = "YOUR_END_POINT_NAME";

        /// <summary>
        /// The Deployment name
        /// </summary>
        internal const string DeploymentName = "DEPLOYMENT_NAME";

        /// <summary>
        /// The Image Deployment name
        /// </summary>
        internal const string ImageDeploymentName = "IMAGE_DEPOLYMENT_NAME";

        /// <summary>
        /// The API key
        /// </summary>
        internal const string Key = "API_KEY";

        /// <summary>
        /// The already credential validated field
        /// </summary>
        private static bool isAlreadyValidated = false;

        #endregion

        public AzureBaseService()
        {
            ValidateCredential();
        }

        #region Properties

        /// <summary>
        /// The kernal
        /// </summary>
        internal IChatClient? Client { get; private set; }

        /// <summary>
        /// Gets or Set a value indicating whether credentials are valid or not.
        /// </summary>
        internal static bool IsCredentialValid { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Validate Azure Credentials
        /// </summary>
        private async void ValidateCredential()
        {
            GetAzureOpenAIKernel();

            if (isAlreadyValidated)
                return;

            try
            {
                if (Client != null)
                {
                    await Client.CompleteAsync("Hello, Test Check");
                    IsCredentialValid = true;
                    isAlreadyValidated = true;
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// To get the Azure open ai kernel method
        /// </summary>
        private void GetAzureOpenAIKernel()
        {
            try
            {
                var client = new AzureOpenAIClient(new Uri(Endpoint), new AzureKeyCredential(Key)).AsChatClient(modelId: DeploymentName);
                Client = client;
            }
            catch (Exception)
            {
                // Log or handle the exception according to your requirements
            }
        }

        internal async Task<string> GetAnswerFromGPT(string userPrompt)
        {
            try
            {
                if (IsCredentialValid && Client != null)
                {
                    var response = await Client.CompleteAsync(userPrompt);
                    return response.ToString();
                }
            }
            catch
            {
                return "Currently in offline mode.";
            }

            return "Currently in offline mode.";
        }

        public async Task<List<SalesPrediction>> PredictSalesTrend(List<SalesData> historicalData, DateRange predictionPeriod)
        {
            if (historicalData == null || !historicalData.Any())
                return new List<SalesPrediction>();

            try
            {
                // Limit JSON serialization to the latest 10 entries for efficiency
                var jsonData = JsonSerializer.Serialize(historicalData.OrderByDescending(d => d.Date).Take(10));

                string startDate = predictionPeriod.StartDate.ToString("yyyy-MM-dd");
                string endDate = predictionPeriod.EndDate.ToString("yyyy-MM-dd");

                var systemMessage = @"
        You are an advanced AI specialized in sales forecasting. Your task is to analyze historical sales data and generate accurate future sales predictions while ensuring data consistency.

        ✅ **Guidelines**:
            - Forecast revenue trends for each product and region.
            - Provide confidence intervals (`LowerBound`, `UpperBound`) for uncertainty estimation.
            - Detect and flag anomalies (`IsAnomaly = true`), explaining any unusual trends.
            - Ensure all predictions follow the JSON schema:

            ```json
            [
                {
                    ""Date"": ""yyyy-MM-dd"",
                    ""ProductId"": ""string"",
                    ""RegionId"": ""string"",
                    ""PredictedRevenue"": decimal,
                    ""LowerBound"": decimal,
                    ""UpperBound"": decimal,
                    ""Confidence"": decimal,
                    ""Explanation"": ""string"",
                    ""IsAnomaly"": true/false,
                    AnomalyExplanation"": ""string (must applicable)""
                }
            ]
            ```";

                var userMessage = $@"
            Generate daily sales predictions from {startDate} to {endDate}.
            Ensure:
            - Each date appears **only once**.
            - Revenue fluctuates **randomly** between **50,000 and 130,000**.  
            - Don't skip any date in the range. Provide whole month data.
            - The response **must contain exactly one entry for every date in this range**.
            - Upper and lower bounds vary **naturally** instead of using a fixed pattern.
            - RegionId and ProductId should start with R00 , P00 
            - Revenue can increase or decrease over time.

            ### Fields per entry:
            - **Date**: Should be formatted as ""yyyy-MM-dd"".
            - **Revenue**: 50,000 - 130,000 (random)
            - **Lower Bound**: Revenue minus (3,000 - 12,000)
            - **Upper Bound**: Revenue plus (3,000 - 12,000)
            - **AnomalyExplanation**: Not an empty string 
            - **IsAnomaly**: True/False, with explanation.";

                // Request prediction from AI
                string response = await GetAnswerFromGPT(systemMessage + "\n\n" + userMessage);
                string extractedJson = JsonExtractor.ExtractJson(response);

                return !string.IsNullOrEmpty(extractedJson)
                    ? JsonSerializer.Deserialize<List<SalesPrediction>>(extractedJson) ?? new List<SalesPrediction>()
                    : new List<SalesPrediction>();
            }
            catch (JsonException)
            {
                /*await Application.Current.DisplayAlert(
                            "404 JSON not found",
                            "Data has been loaded from the previously forecasted JSON file.",
                            "OK"
                        );*/

                return GetPredictionsFromEmbeddedJson(predictionPeriod.StartDate);
            }
        }

        public List<SalesPrediction> GetPredictionsFromEmbeddedJson(DateTime startDate)
        {
            var executingAssembly = typeof(App).GetTypeInfo().Assembly;

            using (var stream = executingAssembly.GetManifestResourceStream("AISalesDashboard.Resources.Raw.SalesPredictionData.json"))
            using (var textStream = new StreamReader(stream!))
            {
                // Read the JSON content from the embedded resource
                string json = textStream.ReadToEnd();

                var predictionData = JsonSerializer.Deserialize<List<SalesPrediction>>(json);

                for (int i = 0; i < predictionData!.Count; i++)
                {
                    predictionData[i].Date = startDate.AddDays(i);
                }

                return predictionData ?? new List<SalesPrediction>();
            }
        }

        #endregion
    }

    public class JsonExtractor
    {
        public static string ExtractJson(string response)
        {
            try
            {
                // Regex pattern to capture JSON, starting with `[ {` and ending with `} ]`
                Match match = Regex.Match(response, @"\[.*?\]", RegexOptions.Singleline);

                if (match.Success && !string.IsNullOrWhiteSpace(match.Value))
                {
                    return match.Value.Trim();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error extracting JSON: {ex.Message}");
            }

            return "Invalid or No JSON Found";
        }
    }
}