using System;
using System.Data;
using Mono.Data.Sqlite;
using PatientTest.Scripts;


namespace Utility
{
    public class Sqlite
    {
        private const string DBUri = "URI=file:MyDatabase.sqlite";

        private static IDbConnection CreateAndOpenDatabase()
        {
            // Open database if it exists
            IDbConnection dbConnection = new SqliteConnection(DBUri);
            dbConnection.Open();

            // Create database and tables if they do not exist
            var dbCommandCreatePatientTable = dbConnection.CreateCommand();
            var dbCommandCreateTestTable = dbConnection.CreateCommand();
            dbCommandCreatePatientTable.CommandText = "CREATE TABLE IF NOT EXISTS PatientInfo (id INTEGER PRIMARY KEY, firstName TEXT, lastName TEXT, age INTEGER, sex TEXT)";
            dbCommandCreateTestTable.CommandText = "CREATE TABLE IF NOT EXISTS Tests (id INTEGER, eye TEXT, testtype TEXT, testdate INTEGER,  duration INTEGER, stimulussize TEXT, error INTEGER,FOREIGN KEY(id) REFERENCES PatientInfo(id))";
            dbCommandCreatePatientTable.ExecuteReader();
            dbCommandCreateTestTable.ExecuteReader();
            return dbConnection;
        }


        public void InsertPatient(string patientID, string patientFirstName, string patientLastName, string patientAge, string patientSex)
        {
            var dbConnection = CreateAndOpenDatabase();
            var insertCommand = dbConnection.CreateCommand();
            insertCommand.CommandText =
                $"INSERT OR REPLACE INTO PatientInfo (id, firstName, lastName, age, sex) VALUES ({patientID}, '{patientFirstName}', '{patientLastName}', {patientAge}, '{patientSex}')";
            insertCommand.ExecuteNonQuery();
            dbConnection.Close();
        }

        public void InsertTestResults(TestResultsDTO results)
        {
            var id = DataTransfer.patientDTO.ID;
            var date = (DateTime.Now - new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc)).TotalSeconds;
                var dbConnection = CreateAndOpenDatabase();
            var insertCommand = dbConnection.CreateCommand();
            insertCommand.CommandText =
                $"INSERT INTO Tests (id, eye, testtype, testdate, duration, stimulussize, error) VALUES ({id}, '{results.EyeTested}', '{results.Test}', {date}, {results.Duration}, '{results.StimulusSize.ToString()}', {results.ErrorRate})";
            insertCommand.ExecuteNonQuery();
            dbConnection.Close();
        }
    }
}
