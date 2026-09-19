using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace PremierPredictor
{
    public class PlayerData
    {
        [VectorType(1)]
        public float[] Features { get; set; }
        public float Stats { get; set; }
    }

    public class PlayerPrediction
    {
        public float Score { get; set; }
    }

    class machineLearning
    {
        string Statistic;

        public List<string> DataHandling(string playerID, string statistic)
        {
            Statistic = statistic;
            try
            {
                var connection = OpenDatabaseConnection();
                var dataTable = FetchDataFromDatabase(playerID, statistic, connection);
                var playerDataList = ConvertDataTableToList(dataTable, statistic);
                var mlContext = InitialiseMLContext();
                var model = TrainRegressionModel(mlContext, playerDataList);
                var metrics = EvaluateModel(mlContext, model, playerDataList);
                var prediction = MakePrediction(mlContext, model, playerDataList);
                var mape = CalculateMAPE(metrics, playerDataList);
                var resultMessages = PrepareMessages(prediction, mape);
                return resultMessages;
            }
            catch
            {
                return HandleError();
            }
        }

        private SqlConnection OpenDatabaseConnection()
        {
            // Updated to use |DataDirectory| instead of the hardcoded local path
            var connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\databaselibrary.mdf;Integrated Security=True";
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }

        private DataTable FetchDataFromDatabase(string playerID, string statistic, SqlConnection connection)
        {
            var query = "SELECT " + statistic + " FROM PerformanceFinal WHERE PlayerID = " + playerID + " ORDER BY Game DESC";
            var adapter = new SqlDataAdapter(query, connection);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);
            return dataTable;
        }

        private List<PlayerData> ConvertDataTableToList(DataTable dataTable, string statistic)
        {
            return dataTable.AsEnumerable()
                .Select(row => new PlayerData { Features = new float[] { Convert.ToSingle(row[statistic]) }, Stats = Convert.ToSingle(row[statistic]) })
                .ToList();
        }

        private MLContext InitialiseMLContext()
        {
            return new MLContext();
        }

        private ITransformer TrainRegressionModel(MLContext mlContext, List<PlayerData> playerDataList)
        {
            var dataView = mlContext.Data.LoadFromEnumerable(playerDataList);
            var pipeline = mlContext.Transforms.CopyColumns("Label", "Stats")
                .Append(mlContext.Transforms.NormalizeMinMax("Features"));
            var trainTestSplit = mlContext.Data.TrainTestSplit(dataView);
            var trainer = mlContext.Regression.Trainers.Sdca(labelColumnName: "Label", maximumNumberOfIterations: 100);
            
            var trainingPipeline = pipeline.Append(trainer);
            return trainingPipeline.Fit(trainTestSplit.TrainSet);
        }

        private RegressionMetrics EvaluateModel(MLContext mlContext, ITransformer model, List<PlayerData> playerDataList)
        {
            var testPredictions = model.Transform(mlContext.Data.LoadFromEnumerable(playerDataList));
            return mlContext.Regression.Evaluate(testPredictions, labelColumnName: "Label");
        }

        private PlayerPrediction MakePrediction(MLContext mlContext, ITransformer model, List<PlayerData> playerDataList)
        {
            var lastGameData = playerDataList.Last();
            var playerToPredict = new PlayerData { Features = new float[] { 0.0f } };
            var predictionEngine = mlContext.Model.CreatePredictionEngine<PlayerData, PlayerPrediction>(model);
            return predictionEngine.Predict(playerToPredict);
        }

        private double CalculateMAPE(RegressionMetrics metrics, List<PlayerData> playerDataList)
        {
            return metrics.MeanAbsoluteError / playerDataList.Average(pd => pd.Stats) * 100.0;
        }

        private List<string> PrepareMessages(PlayerPrediction prediction, double mape)
        {
            var returnedMessage = $"Predicted {Statistic} in the Next Game: {(prediction.Score * 100):N2}";
            var mapeRounded = mape.ToString("N2");
            var resultMessages = new List<string>();
            
            if (prediction.Score > 0)
            {
                resultMessages.Add(returnedMessage);
                resultMessages.Add(mapeRounded);
            }
            else
            {
                resultMessages.Add("There was an issue, please try again");
                resultMessages.Add("");
            }
            return resultMessages;
        }

        private List<string> HandleError()
        {
            string errorMessage = "There was an issue, please try again";
            List<string> errorMessages = new List<string>();
            errorMessages.Add(errorMessage);
            return errorMessages;
        }
    }
}
