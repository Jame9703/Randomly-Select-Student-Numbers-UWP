using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Threading.Tasks;
using Windows.Storage;
using 随机抽取学号.Assets.Controls;
using 随机抽取学号.Views;

namespace 随机抽取学号.Classes
{
    public class Student
    {
        public int StudentNumber { get; set; }//学号
        public string Name { get; set; }//姓名
        public int Gender { get; set; }//性别
        public string  PhotoPath { get; set; }//图片路径
    }
    public class CheckedCheckBox
    {
        public int Index { get; set; }// 索引
        public string Name { get; set; }// 学生姓名
    }
    public static class StudentManager
    {
        public static string StudentDataBasePath;
        public static string CheckedStudentDataBasePath;
        public static int SaveProcess;
        public static ObservableCollection<Student> StudentList { get; set; } = new ObservableCollection<Student>();// 记录所有学生信息
        public static ObservableCollection<CheckBoxItem> CheckBoxItems = new ObservableCollection<CheckBoxItem>();// 记录每个CheckBox的状态:Name,IsChecked
        public static List<CheckedCheckBox> checkedCheckBoxes = new List<CheckedCheckBox>();// 记录被选中的CheckBox的索引，对应学生姓名
        static  StudentManager()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile StudentDBFile = Task.Run(async () => await localFolder.CreateFileAsync("students.db", CreationCollisionOption.OpenIfExists)).Result;
            StudentDataBasePath = StudentDBFile.Path;
            StorageFile CheckedStudentDBFile = Task.Run(async () => await localFolder.CreateFileAsync("checkedstudents.db", CreationCollisionOption.OpenIfExists)).Result;
            CheckedStudentDataBasePath = CheckedStudentDBFile.Path;
            CreateTablesAsync();
        }
        private static async Task CreateTablesAsync()
        {
            try
            {
                using (SqliteConnection db = new SqliteConnection($"Filename={StudentDataBasePath}"))
                {
                    await db.OpenAsync();
                    // 创建 Students 表，StudentNumber 作为主键
                    string createStudentsTable = "CREATE TABLE IF NOT EXISTS Students (StudentNumber INT PRIMARY KEY, Name NVARCHAR(2048), Gender INT, PhotoPath NVARCHAR(2048))";
                    SqliteCommand createStudentsTableCmd = new SqliteCommand(createStudentsTable, db);
                    await createStudentsTableCmd.ExecuteNonQueryAsync();
                }
                using (SqliteConnection db = new SqliteConnection($"Filename={CheckedStudentDataBasePath}"))
                {
                    await db.OpenAsync();
                    // 创建 CheckedStudents 表，Index 作为主键(注意Index是关键字)
                    string createCheckedStudentsTable = "CREATE TABLE IF NOT EXISTS CheckedStudents ([Index] INT PRIMARY KEY, Name NVARCHAR(2048))";
                    SqliteCommand createCheckedStudentsTableCmd = new SqliteCommand(createCheckedStudentsTable, db);
                    await createCheckedStudentsTableCmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception)
            {
                PopupMessage.ShowPopupMessage("错误","创建数据库失败",InfoBarSeverity.Error);
            }

        }

        public static async Task<ObservableCollection<Student>> LoadStudentsAsync()
        {
            ObservableCollection<Student> students = new ObservableCollection<Student>();
            using (SqliteConnection db = new SqliteConnection($"Filename={StudentDataBasePath}"))
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
            await Task.Run(() =>
            {
                using (SqliteConnection connection = new SqliteConnection($"Filename={StudentDataBasePath}"))
                {
                    try
                    {
                        connection.Open();

                        // 清空数据库中的数据
                        string clearQuery = "DELETE FROM Students";
                        using (SqliteCommand clearCommand = new SqliteCommand(clearQuery, connection))
                        {
                            clearCommand.ExecuteNonQuery();
                        }

                        // 开始事务
                        using (DbTransaction transaction = connection.BeginTransaction())
                        {
                            string insertQuery = "INSERT INTO Students (StudentNumber, Name, Gender, PhotoPath) VALUES (@StudentNumber, @Name, @Gender, @PhotoPath)";
                            using (SqliteCommand insertCommand = new SqliteCommand(insertQuery, connection, (SqliteTransaction)transaction))
                            {
                                insertCommand.Parameters.Add("@StudentNumber", SqliteType.Integer);
                                insertCommand.Parameters.Add("@Name", SqliteType.Text);
                                insertCommand.Parameters.Add("@Gender", SqliteType.Integer);
                                insertCommand.Parameters.Add("@PhotoPath", SqliteType.Text);

                                int totalCount = students.Count;
                                if(totalCount != 0)
                                {
                                    for (int i = 0; i < totalCount; i++)
                                    {
                                        Student student = students[i];
                                        insertCommand.Parameters["@StudentNumber"].Value = student.StudentNumber;
                                        insertCommand.Parameters["@Name"].Value = student.Name;
                                        insertCommand.Parameters["@Gender"].Value = student.Gender;
                                        insertCommand.Parameters["@PhotoPath"].Value = student.PhotoPath;

                                        insertCommand.ExecuteNonQuery();
                                        if (totalCount != 0)
                                        {
                                            // 计算并报告进度
                                            SaveProcess = (int)((i + 1) * 100.0 / totalCount);
                                        }
                                    }
                                }
                                else
                                {
                                    SaveProcess = -1;
                                }
                            }

                            // 提交事务
                            transaction.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        // 处理异常
                        PopupNotice popupNotice = new PopupNotice("保存学生信息失败");
                        popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                        popupNotice.ShowPopup();
                    }
                }
            });

        }

        public static async Task SaveCheckedStudentsAsync(List<CheckedCheckBox> checkedcheckboxes)
        {
            await Task.Run(() =>
            {
                using (SqliteConnection connection = new SqliteConnection($"Filename={CheckedStudentDataBasePath}"))
                {
                    try
                    {
                        connection.Open();

                        // 清空数据库中的数据
                        string clearQuery = "DELETE FROM CheckedStudents";
                        using (SqliteCommand clearCommand = new SqliteCommand(clearQuery, connection))
                        {
                            clearCommand.ExecuteNonQuery();
                        }

                        // 开始事务
                        using (DbTransaction transaction = connection.BeginTransaction())
                        {
                            string insertQuery = "INSERT INTO CheckedStudents ([Index],Name) VALUES (@Index, @Name)";
                            using (SqliteCommand insertCommand = new SqliteCommand(insertQuery, connection, (SqliteTransaction)transaction))
                            {
                                insertCommand.Parameters.Add("@Index", SqliteType.Integer);
                                insertCommand.Parameters.Add("@Name", SqliteType.Text);
                                int totalCount = checkedcheckboxes.Count;
                                foreach (var checkbox in checkedcheckboxes)
                                {
                                    insertCommand.Parameters["@Index"].Value = checkbox.Index;
                                    insertCommand.Parameters["@Name"].Value = checkbox.Name;
                                    insertCommand.ExecuteNonQuery();
                                }
                            }
                            // 提交事务
                            transaction.Commit();
                        }
                    }
                    catch (Exception)
                    {
                        // 处理异常
                        PopupNotice popupNotice = new PopupNotice("保存抽取范围失败");
                        popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                        popupNotice.ShowPopup();
                    }
                }
            });
        }

        public static async Task<List<CheckedCheckBox>> LoadCheckedStudentsAsync()
        {
            List<CheckedCheckBox> checkedcheckboxes = new List<CheckedCheckBox>();
            using (SqliteConnection db = new SqliteConnection($"Filename={CheckedStudentDataBasePath}"))
            {
                await db.OpenAsync();
                string selectCommand = "SELECT * FROM CheckedStudents";
                SqliteCommand selectCmd = new SqliteCommand(selectCommand, db);
                using (SqliteDataReader query = await selectCmd.ExecuteReaderAsync())
                {
                    while (await query.ReadAsync())
                    {
                        CheckedCheckBox checkbox = new CheckedCheckBox
                        {
                            Index = query.GetInt32(0),
                            Name = query.GetString(1),
                        };
                        checkedcheckboxes.Add(checkbox);
                    }
                }
            }
            return checkedcheckboxes;
        }
    }
}
