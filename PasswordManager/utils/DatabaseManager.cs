using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography.Xml;
using PasswordManager.utils;

namespace PasswordManager.utils
{
    public class PasswordRecord : INotifyPropertyChanged
    {
        private  int _id;
        private string _label;
        private string _username;
        private string _password;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public int ID
        {
            get { return _id; }
            set
            {
                _id = value;
                PropertyChanged(this, new PropertyChangedEventArgs("id"));
            }
        }
        public string Label
        {
            get
            {
                return _label;
            }
            set
            {
                _label = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Label"));
            }
        }
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Username"));
            }
        }
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Password"));
            }
        }
        public PasswordRecord(int id, string label, string username, string password)
        {
             _id = id;
             _label = label;
             _username = username;
             _password = password;   
        } 
    }
    public static class DatabaseManager
    {
        // 常量区域
        private const string DatabaseFileName = "Passwords.sqlite";
        private const string ConnectionString = "Data Source=Passwords.sqlite;Version=3;";
        // 变量区域
        public static BindingList<PasswordRecord> passwordList = new BindingList<PasswordRecord>();
        public static string? encrypted_random_key;
        public static string? encrypted_verify_data;

        public static int InitializeDatabase()// 初始化数据库
        {
            int init_state = 0;
            if (!File.Exists(DatabaseFileName))
            {
                SQLiteConnection.CreateFile(DatabaseFileName);
                CreateTables();
                InitializeData();
                init_state = 1;
            }

            // 读取存储的信息
            SyncPasswordListFromDatabase();
            var res = getEncryptedKey();
            encrypted_random_key = res.Item1;
            encrypted_verify_data = res.Item2;  
            return init_state;
        }

        private static void CreateTables()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = "CREATE TABLE IF NOT EXISTS Accounts (Id INTEGER PRIMARY KEY, Label TEXT, Username TEXT, EncryptedPassword TEXT)";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE TABLE IF NOT EXISTS Admin (Id INTEGER PRIMARY KEY, EncryptedKey TEXT, VerifyData TEXT)";
                    command.ExecuteNonQuery();

                }
            }
        }

        private static void InitializeData()   // 初始化数据
        {
            byte[] key = CryptoAlgorithm.getRandomKey();
            string key_string = CryptoAlgorithm.BytesToHexString(key);

            // 初始化默认用户密钥
            string userKeyString = "admin";

            // 添加默认数据
            string enc_password = CryptoAlgorithm.EncryptPassword(key, "success");

            // 使用用户密钥对随机密钥加密并写入数据库
            string enc_key = CryptoAlgorithm.EncryptRandomKey(userKeyString, key);
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string sql = "INSERT INTO Admin(EncryptedKey, VerifyData)VALUES(@encrypted_key, @enc_password)";

                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@encrypted_key", enc_key);
                    command.Parameters.AddWithValue("@enc_password", enc_password);
                    command.ExecuteNonQuery();
                }
            }

        }


        public static void SyncPasswordListFromDatabase()
        {
            passwordList = getPasswordList();
        }

        private static BindingList<PasswordRecord>  getPasswordList()// 同步数据库存储至内存
        {
            BindingList<PasswordRecord> new_list = new BindingList<PasswordRecord>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = "SELECT Id, Label, Username, EncryptedPassword from Accounts";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            var label = reader.GetString(1);
                            var username = reader.GetString(2);
                            var encrypted_password = reader.GetString(3);
                            new_list.Add(new PasswordRecord(id, label,username,encrypted_password));
                        }
                    }
                }
            }

            return new_list;
        }

        // 将内存中的数据同步至数据库
        public static void SyncPasswordListToDatabase()
        {
            using(var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using(var command = new SQLiteCommand(connection))
                {
                    command.CommandText = "DELETE FROM Accounts";
                    command.ExecuteNonQuery();
                }

                string sql = "INSERT INTO Accounts(Id, Label, Username, EncryptedPassword)" +
                    "VALUES (@id, @label, @username, @enc_password)";
                foreach(var record in passwordList)
                {
                    using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", record.ID);
                        command.Parameters.AddWithValue("@label", record.Label);
                        command.Parameters.AddWithValue("@username", record.Username);
                        command.Parameters.AddWithValue("@enc_password", record.Password);
                        command.ExecuteNonQuery();
                    }
                }
            }

        }
           

        public static (string, string) getEncryptedKey()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = "SELECT EncryptedKey, VerifyData from Admin";
                    using (var reader = command.ExecuteReader())
                    {
                        reader.Read();
                        var encrypted_key = reader.GetString(0);
                        var verify_data = reader.GetString(1);
                        return (encrypted_key, verify_data);
                    }
                }
            }
        }

        public static bool setEncryptedKey(string encrypted_key, byte[] raw_key)
        {
            string enc_verify_data = CryptoAlgorithm.EncryptPassword(raw_key, "success");
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string sql = "UPDATE Admin SET EncryptedKey = @encrypted_key, VerifyData = @verify_data";

                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@encrypted_key", encrypted_key);
                    command.Parameters.AddWithValue("@verify_data", enc_verify_data);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                    return true;
                }
            }
        }

        public static int getUnusedMinInteger()
        {
            List<int> list = new List<int>();
            foreach(var item in passwordList)
            {
                list.Add(item.ID);
            }

            return FindSmallestPositiveMissingNumber(list); 
        }

        private static int FindSmallestPositiveMissingNumber(List<int> numbers)
        {
            HashSet<int> set = new HashSet<int>(numbers);

            int smallestMissingPositive = 1;

            while (set.Contains(smallestMissingPositive))
            {
                smallestMissingPositive++;
            }

            return smallestMissingPositive;
        }
    }




}

