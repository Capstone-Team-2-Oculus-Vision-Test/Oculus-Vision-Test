using System;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using Newtonsoft.Json;
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
            IDbCommand dbCommandCreatePatientTable = dbConnection.CreateCommand();
            IDbCommand dbCommandCreateTestTable = dbConnection.CreateCommand();
            dbCommandCreatePatientTable.CommandText =
                "CREATE TABLE IF NOT EXISTS PatientInfo" + 
                " (id INTEGER PRIMARY KEY, firstName TEXT, lastName TEXT, age INTEGER, sex TEXT)";
            dbCommandCreateTestTable.CommandText =
                "CREATE TABLE IF NOT EXISTS Tests " + 
                "(id INTEGER, name TEXT, testDate DATE,results TEXT, FOREIGN KEY(id) REFERENCES PatientInfo(id))";
            dbCommandCreatePatientTable.ExecuteReader();
            dbCommandCreateTestTable.ExecuteReader();
            return dbConnection;
        }


        public static void InsertPatient(string patientID, string patientFirstName, string patientLastName,
            string patientAge, string patientSex)
        {
            IDbConnection dbConnection = CreateAndOpenDatabase();
            IDbCommand insertCommand = dbConnection.CreateCommand();
            insertCommand.CommandText =
                "INSERT OR REPLACE INTO PatientInfo (id, firstName, lastName, age, sex) " + 
                $" VALUES ({patientID}, '{patientFirstName}', '{patientLastName}', {patientAge}, '{patientSex}')";
            insertCommand.ExecuteNonQuery();
            dbConnection.Close();
        }

        public static void InsertTestResults()
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            string id = DataTransfer.patientDTO.ID;
            string name = DataTransfer.patientDTO.FirstName + " " + DataTransfer.patientDTO.LastName;
            var date = DateTime.Now.ToString("d");
            string resultJson = JsonConvert.SerializeObject(DataTransfer.resultsDTO, Formatting.Indented, settings);
            IDbConnection dbConnection = CreateAndOpenDatabase();
            IDbCommand insertCommand = dbConnection.CreateCommand();
            insertCommand.CommandText =
                $"INSERT INTO Tests (id, name, testDate, results) VALUES ({id},'{name}', '{date}', '{resultJson}')";
            insertCommand.ExecuteNonQuery();
            dbConnection.Close();
        }

        public static IEnumerable<TestListData> GetAllTests()
        {
            var testList = new List<TestListData>();
            IDbConnection dbConnection = CreateAndOpenDatabase();
            IDbCommand command = dbConnection.CreateCommand();
            command.CommandText = "SELECT * FROM Tests ORDER BY testDate DESC";
            IDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var test = new TestListData
                {
                    ID = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Date = reader.GetString(2),
                    Results = JsonConvert.DeserializeObject<TestResultsDTO>(reader.GetString(3))
                };
                testList.Add(test);
            }

            dbConnection.Close();
            return testList;
        }

        public static IEnumerable<PatientDTO> GetAllPatients()
        {
            var patientList = new List<PatientDTO>();
            IDbConnection dbConnection = CreateAndOpenDatabase();
            IDbCommand command = dbConnection.CreateCommand();
            command.CommandText = "SELECT * FROM PatientInfo";
            IDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var patient = new PatientDTO
                {
                    ID = reader.GetInt32(0).ToString(),
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    Age = reader.GetInt32(3).ToString(),
                    Sex = reader.GetString(4)
                };
                patientList.Add(patient);
            }

            dbConnection.Close();
            return patientList;
        }

        public static PatientDTO GetPatient(int id)
        {
            IDbConnection dbConnection = CreateAndOpenDatabase();
            IDbCommand command = dbConnection.CreateCommand();
            command.CommandText = $"SELECT * FROM PatientInfo where id = {id}";
            IDataReader reader = command.ExecuteReader();
            reader.Read();
            var patient = new PatientDTO
            {
                ID = reader.GetInt32(0).ToString(),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2),
                Age = reader.GetInt32(3).ToString(),
                Sex = reader.GetString(4)
            };
            dbConnection.Close();
            return patient;
        }
    }
}