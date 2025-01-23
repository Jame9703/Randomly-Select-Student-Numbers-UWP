using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Storage;
using 随机抽取学号.Views;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace 随机抽取学号.Classes
{
    public class Student
    {
        public int StudentNumber { get; set; }//学号
        public string Name { get; set; }//姓名
        public int Gender { get; set; }//性别
        public string  PhotoPath { get; set; }//图片路径
    }
    public static class StudentManager
    {
        private static string dbPath;
        public static ObservableCollection<Student> StudentList { get; set; } = new ObservableCollection<Student>();
        static  StudentManager()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile dbFile = Task.Run(async () => await localFolder.CreateFileAsync("students.db", CreationCollisionOption.OpenIfExists)).Result;
            dbPath = dbFile.Path;
            CreateTablesAsync();
        }
        private static async Task CreateTablesAsync()
        {
            using (SqliteConnection db = new SqliteConnection($"Filename={dbPath}"))
            {
                await db.OpenAsync();
                // 创建 Students 表，StudentNumber 作为主键
                string createStudentsTable = "CREATE TABLE IF NOT EXISTS Students (StudentNumber INT PRIMARY KEY, Name NVARCHAR(2048), Gender INT, PhotoPath NVARCHAR(2048))";
                SqliteCommand createStudentsTableCmd = new SqliteCommand(createStudentsTable, db);
                await createStudentsTableCmd.ExecuteNonQueryAsync();
            }
        }

        public static async Task InsertStudentAsync(Student student)
        {
            using (SqliteConnection db = new SqliteConnection($"Filename={dbPath}"))
            {
                await db.OpenAsync();
                string insertCommand = "INSERT INTO Students (StudentNumber, Name, Gender, PhotoPath) VALUES (@StudentNumber, @Name, @Gender, @PhotoPath)";
                SqliteCommand insertCmd = new SqliteCommand(insertCommand, db);
                insertCmd.Parameters.AddWithValue("@StudentNumber", student.StudentNumber);
                insertCmd.Parameters.AddWithValue("@Name", student.Name);
                insertCmd.Parameters.AddWithValue("@Gender", student.Gender);
                insertCmd.Parameters.AddWithValue("@PhotoPath", student.PhotoPath);
                await insertCmd.ExecuteNonQueryAsync();
            }
        }

        public static async Task InsertMultipleStudentsAsync(List<Student> students)
        {
            using (SqliteConnection db = new SqliteConnection($"Filename={dbPath}"))
            {
                await db.OpenAsync();
                using (SqliteTransaction transaction = db.BeginTransaction())
                {
                    try
                    {
                        foreach (Student student in students)
                        {
                            string insertCommand = "INSERT INTO Students (StudentNumber, Name, Gender, PhotoPath) VALUES (@StudentNumber, @Name, @Gender, @PhotoPath)";
                            SqliteCommand insertCmd = new SqliteCommand(insertCommand, db);
                            insertCmd.Parameters.AddWithValue("@StudentNumber", student.StudentNumber);
                            insertCmd.Parameters.AddWithValue("@Name", student.Name);
                            insertCmd.Parameters.AddWithValue("@Gender", student.Gender);
                            insertCmd.Parameters.AddWithValue("@PhotoPath", student.PhotoPath);
                            await insertCmd.ExecuteNonQueryAsync();
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }
        public static async Task DeleteStudentAsync(int studentNumber)
        {
            using (SqliteConnection db = new SqliteConnection($"Filename={dbPath}"))
            {
                await db.OpenAsync();
                string deleteCommand = "DELETE FROM Students WHERE StudentNumber = @StudentNumber";
                SqliteCommand deleteCmd = new SqliteCommand(deleteCommand, db);
                deleteCmd.Parameters.AddWithValue("@StudentNumber", studentNumber);
                await deleteCmd.ExecuteNonQueryAsync();
            }
        }
        public static void SelectStudentAsync(int studentNumber)
        {
            // 假设这里有一个表存储被选中的学生信息，仅作示例，你可以根据需求实现
            // 这里使用一个简单的列表存储选中的学生编号，实际应用可存储在数据库中
            List<int> selectedStudents = new List<int>();
            selectedStudents.Add(studentNumber);
        }

        public static async Task<ObservableCollection<Student>> LoadStudentsAsync()
        {
            ObservableCollection<Student> students = new ObservableCollection<Student>();
            using (SqliteConnection db = new SqliteConnection($"Filename={dbPath}"))
            {
                await db.OpenAsync();
                string selectCommand = "SELECT * FROM Students";
                SqliteCommand selectCmd = new SqliteCommand(selectCommand, db);
                using (SqliteDataReader query = await selectCmd.ExecuteReaderAsync())
                {
                    while (await query.ReadAsync())
                    {
                        Student student = new Student
                        {
                            StudentNumber = query.GetInt32(0),
                            Name = query.GetString(1),
                            Gender = query.GetInt32(2),
                            PhotoPath = query.GetString(3)
                        };
                        students.Add(student);
                    }
                }
            }
            return students;
        }

        public static async Task SaveStudentsAsync(ObservableCollection<Student> students)
        {
            using (SqliteConnection db = new SqliteConnection($"Filename={dbPath}"))
            {
                await db.OpenAsync();
                // 先清空表
                string clearCommand = "DELETE FROM Students";
                SqliteCommand clearCmd = new SqliteCommand(clearCommand, db);
                await clearCmd.ExecuteNonQueryAsync();

                foreach (Student student in students)
                {
                    string insertCommand = "INSERT INTO Students (StudentNumber, Name, Gender, PhotoPath) VALUES (@StudentNumber, @Name, @Gender, @PhotoPath)";
                    SqliteCommand insertCmd = new SqliteCommand(insertCommand, db);
                    insertCmd.Parameters.AddWithValue("@StudentNumber", student.StudentNumber);
                    insertCmd.Parameters.AddWithValue("@Name", student.Name);
                    insertCmd.Parameters.AddWithValue("@Gender", student.Gender);
                    insertCmd.Parameters.AddWithValue("@PhotoPath", student.PhotoPath);
                    await insertCmd.ExecuteNonQueryAsync();
                }
            }
        }

        private const string FileName = "CheckedStudents.json";

        public static async Task SaveCheckedStudentsAsync(List<string> students)
        {
            try
            {
                var localFolder = ApplicationData.Current.LocalFolder;
                var file = await localFolder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);

                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    var serializer = new DataContractJsonSerializer(typeof(List<string>));
                    serializer.WriteObject(stream, students);
                }
            }
            catch (Exception)
            {
                PopupNotice popupNotice = new PopupNotice("保存学生信息失败");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                popupNotice.ShowPopup();
            }

        }

        public static async Task<List<string>> LoadCheckedStudentsAsync()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            try
            {
                var file = await localFolder.GetFileAsync(FileName);
                using (var stream = await file.OpenStreamForReadAsync())
                {
                    var serializer = new DataContractJsonSerializer(typeof(List<string>));
                    try
                    {
                        return (List<string>)serializer.ReadObject(stream);
                    }
                    catch (Exception)
                    {
                        PopupNotice popupNotice = new PopupNotice("读取学生信息失败");
                        popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                        popupNotice.ShowPopup();
                        return new List<string>();
                    }
                }
            }
            catch (FileNotFoundException)
            {
                PopupNotice popupNotice = new PopupNotice("找不到students.json,请不要移动、修改或删除此文件");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Informational;
                popupNotice.ShowPopup();
                await localFolder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);
                return new List<string>();
            }
        }
    }
}
