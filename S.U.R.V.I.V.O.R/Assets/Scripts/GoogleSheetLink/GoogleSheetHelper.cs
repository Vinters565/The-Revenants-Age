#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Google.Apis.Sheets.v4;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4.Data;
using UnityEngine;

namespace GoogleSheetLink
{
    public class GoogleSheetHelper
    {
        private readonly string[] scopes = {SheetsService.Scope.Spreadsheets};
        private const string APPLICATION_NAME = "SURVIVOR";
        private readonly string spreadsheetId;
        private SheetsService Service { get; set; }
        
        public GoogleSheetHelper(string spreadsheetId, string jsonKeyFileName)
        {
            this.spreadsheetId = spreadsheetId;
            InitializeService(jsonKeyFileName);
        }
        
        private void InitializeService(string jsonKeyFileName)
        {
            var credential = GetCredentialsFromFile(jsonKeyFileName);
            Service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = APPLICATION_NAME
            });
        }

        private GoogleCredential GetCredentialsFromFile(string jsonKeyFileName)
        {
            using var stream = new FileStream(
                $@"{Application.streamingAssetsPath}/{jsonKeyFileName}",
                FileMode.Open,
                FileAccess.Read);
            var credential = GoogleCredential.FromStream(stream).CreateScoped(scopes);

            return credential;
        }
        
        public void CreateEntry(string range, IList<IList<object>> objectMatrix)
        {
            var valueRange = new ValueRange();
            valueRange.Values = objectMatrix;

            var appendRequest = Service.Spreadsheets.Values.Append(valueRange, spreadsheetId, range);
            appendRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendResponse = appendRequest.Execute();
        }

        public void DeleteEntry(string range)
        {
            var requestBody = new ClearValuesRequest();
            var deleteRequest = Service.Spreadsheets.Values.Clear(requestBody, spreadsheetId, range);
            var deleteResponse = deleteRequest.Execute();
        }

        public IList<IList<object>> ReadEntries(string range)
        {
            var request = Service.Spreadsheets.Values.Get(spreadsheetId, range);

            var response = request.Execute();
            var values = response.Values;

            if (values != null && values.Count > 0)
                return values;
            Debug.Log("Данные не найдены!");
            return null;
        }
    }
}
#endif