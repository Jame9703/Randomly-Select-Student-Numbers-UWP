using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Data;
using 随机抽取学号.Views;

namespace 随机抽取学号.Classes
{
    public class Student : INotifyPropertyChanged
    {
        private int studentNumber;
        private string name;
        private int gender;
        private string photoPath;

        public int StudentNumber//学号
        {
            get { return studentNumber; }
            set
            {
                if (studentNumber != value)
                {
                    studentNumber = value;
                    OnPropertyChanged(nameof(StudentNumber));
                }
            }
        }

        public string Name//姓名
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public int Gender//性别
        {
            get { return gender; }
            set
            {
                if (gender != value)
                {
                    gender = value;
                    OnPropertyChanged(nameof(Gender));
                }
            }
        }

        public string PhotoPath//照片路径
        {
            get { return photoPath; }
            set
            {
                if (photoPath != value)
                {
                    photoPath = value;
                    OnPropertyChanged(nameof(PhotoPath));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class Class : INotifyPropertyChanged
    {
        private string className;
        private string classEmblemPath;
        public string ClassName//班级名称
        {
            get { return className; }
            set
            {
                if (className != value)
                {
                    className = value;
                    OnPropertyChanged(nameof(ClassName));
                }
            }
        }
        public string ClassEmblemPath//班徽路径
        {
            get { return classEmblemPath; }
            set
            {
                if (classEmblemPath != value)
                {
                    classEmblemPath = value;
                    OnPropertyChanged(nameof(ClassEmblemPath));
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public static class StudentManager
    {
        #region 变量声明
        public static StorageFolder CurrentClassFolder;// 记录当前班级对应文件夹
        public static string ClassDataBasePath;// 记录班级信息数据库路径
        public static string StudentDataBasePath;// 记录学生信息数据库路径
        public static string CheckedStudentDataBasePath;// 记录抽取范围中的学生信息数据库路径
        public static int SaveStudentsProcess;// 记录保存学生信息进度
        public static int SaveCheckedStudentsProcess;// 记录保存抽取范围中的学生信息进度
        public static ObservableCollection<Class> ClassList = new ObservableCollection<Class>();// 记录所有班级信息
        public static ObservableCollection<Student> StudentList = new ObservableCollection<Student>();// 记录所有学生信息
        public static List<ItemIndexRange> SelectedRanges = new List<ItemIndexRange>();// 记录抽取范围中的连续范围，如[1,1],[2,3]等
        public static List<int> CheckedStudents = new List<int>();// 记录抽取范围中的学生的学号
        #endregion

        #region 初始化数据库
        public static async Task InitializeDatabase()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("CurrentClassName"))
            {
                //创建班级信息数据库，若已存在则打开
                StorageFile ClassDBFile = await localFolder.CreateFileAsync("classes.db", CreationCollisionOption.OpenIfExists);
                ClassDataBasePath = ClassDBFile.Path;
                //获取当前班级对应文件夹
                string CurrentClassName = ApplicationData.Current.LocalSettings.Values["CurrentClassName"].ToString();
                CurrentClassFolder = await localFolder.CreateFolderAsync(CurrentClassName, CreationCollisionOption.OpenIfExists);
                //创建学生信息数据库，若已存在则打开
                StorageFile StudentDBFile = await CurrentClassFolder.CreateFileAsync("students.db", CreationCollisionOption.OpenIfExists);
                StudentDataBasePath = StudentDBFile.Path;
                //创建抽取范围中的学生信息数据库，若已存在则打开
                StorageFile CheckedStudentDBFile = await CurrentClassFolder.CreateFileAsync("checkedstudents.db", CreationCollisionOption.OpenIfExists);
                CheckedStudentDataBasePath = CheckedStudentDBFile.Path;
                await CreateTablesAsync();
            }
        }
        private static async Task CreateTablesAsync()
        {
            try
            {
                using (SqliteConnection db = new SqliteConnection($"Filename={ClassDataBasePath}"))
                {
                    await db.OpenAsync();
                    string createStudentsTable = "CREATE TABLE IF NOT EXISTS Classes (ClassName NVARCHAR(2048),ClassEmblemPath NVARCHAR(2048))";
                    SqliteCommand createStudentsTableCmd = new SqliteCommand(createStudentsTable, db);
                    await createStudentsTableCmd.ExecuteNonQueryAsync();
                }

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
                    string createStudentsTable = "CREATE TABLE IF NOT EXISTS CheckedStudents (FirstIndex INT,LastIndex INT)";
                    SqliteCommand createStudentsTableCmd = new SqliteCommand(createStudentsTable, db);
                    await createStudentsTableCmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                PopupNotice popupNotice = new PopupNotice("创建数据库失败" + ex.Message);
                popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                popupNotice.ShowPopup();
            }
        }
        #endregion

        #region 读取和保存班级信息
        public static async Task<ObservableCollection<Class>> LoadClassesAsync()
        {
            try
            {
                ObservableCollection<Class> classes = new ObservableCollection<Class>();
                using (SqliteConnection db = new SqliteConnection($"Filename={ClassDataBasePath}"))
                {
                    await db.OpenAsync();
                    string selectCommand = "SELECT * FROM Classes";
                    SqliteCommand selectCmd = new SqliteCommand(selectCommand, db);
                    using (SqliteDataReader query = await selectCmd.ExecuteReaderAsync())
                    {
                        while (await query.ReadAsync())
                        {
                            Class _class = new Class
                            {
                                ClassName = query.GetString(0),
                                ClassEmblemPath = query.GetString(1),
                            };
                            classes.Add(_class);
                        }
                    }
                }
                return classes;
            }
            catch (SqliteException sqliteEx)
            {
                //处理数据库异常
                PopupNotice popupNotice = new PopupNotice("加载班级信息失败,发生数据库异常" + sqliteEx.Message);
                popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                popupNotice.ShowPopup();
            }
            catch (UnauthorizedAccessException unauthorizedEx)
            {
                // 处理权限不足异常
                PopupNotice popupNotice = new PopupNotice("加载班级信息失败,权限不足" + unauthorizedEx.Message);
                popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                popupNotice.ShowPopup();
            }
            catch (IOException ioEx)
            {
                // 处理输入输出异常，如文件损坏
                PopupNotice popupNotice = new PopupNotice("加载班级信息失败,发生文件操作异常" + ioEx.Message);
                popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                popupNotice.ShowPopup();
            }
            catch (Exception ex)
            {
                PopupNotice popupNotice = new PopupNotice("加载班级信息失败,发生未知错误" + ex.Message);
                popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                popupNotice.ShowPopup();
            }
            return new ObservableCollection<Class>();
        }
        public static async Task SaveClassesAsync(ObservableCollection<Class> classes)
        {
            using (SqliteConnection connection = new SqliteConnection($"Filename={ClassDataBasePath}"))
            {
                try
                {
                    await connection.OpenAsync();
                    // 开始事务
                    using (DbTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 清空数据库中的数据
                            string clearQuery = "DELETE FROM Classes";
                            using (SqliteCommand clearCommand = new SqliteCommand(clearQuery, connection, (SqliteTransaction)transaction))
                            {
                                await clearCommand.ExecuteNonQueryAsync();
                            }
                            string insertQuery = "INSERT INTO CLasses (ClassName, ClassEmblemPath) VALUES (@ClassName, @ClassEmblemPath)";
                            using (SqliteCommand insertCommand = new SqliteCommand(insertQuery, connection, (SqliteTransaction)transaction))
                            {
                                insertCommand.Parameters.Add("@ClassName", SqliteType.Text);
                                insertCommand.Parameters.Add("@ClassEmblemPath", SqliteType.Text);

                                foreach (var _class in classes)
                                {
                                    insertCommand.Parameters["@ClassName"].Value = _class.ClassName;
                                    insertCommand.Parameters["@ClassEmblemPath"].Value = _class.ClassEmblemPath;
                                    await insertCommand.ExecuteNonQueryAsync();
                                }
                            }
                            // 提交事务
                            transaction.Commit();
                        }
                        catch (SqliteException sqliteEx)
                        {
                            transaction.Rollback(); // 发生错误时回滚事务
                                                    //处理数据库异常
                            PopupNotice popupNotice = new PopupNotice("保存班级信息失败,发生数据库异常" + sqliteEx.Message);
                            popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                            popupNotice.ShowPopup();
                        }
                        catch (UnauthorizedAccessException unauthorizedEx)
                        {
                            transaction.Rollback(); // 发生错误时回滚事务
                                                    // 处理权限不足异常
                            PopupNotice popupNotice = new PopupNotice("保存班级信息失败,权限不足" + unauthorizedEx.Message);
                            popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                            popupNotice.ShowPopup();
                        }
                        catch (IOException ioEx)
                        {
                            transaction.Rollback(); // 发生错误时回滚事务
                                                    // 处理输入输出异常，如文件损坏
                            PopupNotice popupNotice = new PopupNotice("保存班级信息失败,发生文件操作异常" + ioEx.Message);
                            popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                            popupNotice.ShowPopup();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // 发生错误时回滚事务
                            PopupNotice popupNotice = new PopupNotice("保存班级信息数据库失败" + ex.Message);
                            popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                            popupNotice.ShowPopup();
                        }
                    }
                }
                catch (Exception ex)
                {
                    PopupNotice popupNotice = new PopupNotice("连接班级信息数据库失败" + ex.Message);
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                    popupNotice.ShowPopup();
                }
            }
        }
        #endregion

        #region 读取和保存学生信息
        public static async Task<ObservableCollection<Student>> LoadStudentsAsync()
        {
            try
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
            catch (SqliteException sqliteEx)
            {
                //处理数据库异常
                PopupNotice popupNotice = new PopupNotice("加载学生信息失败,发生数据库异常" + sqliteEx.Message);
                popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                popupNotice.ShowPopup();
            }
            catch (UnauthorizedAccessException unauthorizedEx)
            {
                // 处理权限不足异常
                PopupNotice popupNotice = new PopupNotice("加载学生信息失败,权限不足" + unauthorizedEx.Message);
                popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                popupNotice.ShowPopup();
            }
            catch (IOException ioEx)
            {
                // 处理输入输出异常，如文件损坏
                PopupNotice popupNotice = new PopupNotice("加载学生信息失败,发生文件操作异常" + ioEx.Message);
                popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                popupNotice.ShowPopup();
            }
            catch (Exception ex)
            {
                PopupNotice popupNotice = new PopupNotice("加载学生信息失败,发生未知错误" + ex.Message);
                popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                popupNotice.ShowPopup();
            }
            return new ObservableCollection<Student>();
        }

        public static async Task SaveStudentsAsync(ObservableCollection<Student> students)
        {
            using (SqliteConnection connection = new SqliteConnection($"Filename={StudentDataBasePath}"))
            {
                try
                {
                    await connection.OpenAsync();
                    // 开始事务
                    using (DbTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 清空数据库中的数据
                            string clearQuery = "DELETE FROM Students";
                            using (SqliteCommand clearCommand = new SqliteCommand(clearQuery, connection, (SqliteTransaction)transaction))
                            {
                                await clearCommand.ExecuteNonQueryAsync();
                            }
                            string insertQuery = "INSERT INTO Students (StudentNumber, Name, Gender, PhotoPath) VALUES (@StudentNumber, @Name, @Gender, @PhotoPath)";
                            using (SqliteCommand insertCommand = new SqliteCommand(insertQuery, connection, (SqliteTransaction)transaction))
                            {
                                insertCommand.Parameters.Add("@StudentNumber", SqliteType.Integer);
                                insertCommand.Parameters.Add("@Name", SqliteType.Text);
                                insertCommand.Parameters.Add("@Gender", SqliteType.Integer);
                                insertCommand.Parameters.Add("@PhotoPath", SqliteType.Text);

                                int totalCount = students.Count;
                                if (totalCount != 0)
                                {
                                    for (int i = 0; i < totalCount; i++)
                                    {
                                        Student student = students[i];
                                        insertCommand.Parameters["@StudentNumber"].Value = student.StudentNumber;
                                        insertCommand.Parameters["@Name"].Value = student.Name;
                                        insertCommand.Parameters["@Gender"].Value = student.Gender;
                                        insertCommand.Parameters["@PhotoPath"].Value = student.PhotoPath;

                                        await insertCommand.ExecuteNonQueryAsync();
                                        // 计算并报告进度
                                        SaveStudentsProcess = (int)((i + 1) * 100.0 / totalCount);
                                    }
                                }
                                else
                                {
                                    SaveStudentsProcess = -1;
                                }
                            }
                            // 提交事务
                            transaction.Commit();
                        }
                        catch (SqliteException sqliteEx)
                        {
                            transaction.Rollback(); // 发生错误时回滚事务
                                                    //处理数据库异常
                            PopupNotice popupNotice = new PopupNotice("保存学生信息失败,发生数据库异常" + sqliteEx.Message);
                            popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                            popupNotice.ShowPopup();
                        }
                        catch (UnauthorizedAccessException unauthorizedEx)
                        {
                            transaction.Rollback(); // 发生错误时回滚事务
                                                    // 处理权限不足异常
                            PopupNotice popupNotice = new PopupNotice("保存学生信息失败,权限不足" + unauthorizedEx.Message);
                            popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                            popupNotice.ShowPopup();
                        }
                        catch (IOException ioEx)
                        {
                            transaction.Rollback(); // 发生错误时回滚事务
                                                    // 处理输入输出异常，如文件损坏
                            PopupNotice popupNotice = new PopupNotice("保存学生信息失败,发生文件操作异常" + ioEx.Message);
                            popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                            popupNotice.ShowPopup();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // 发生错误时回滚事务
                            PopupNotice popupNotice = new PopupNotice("保存学生信息数据库失败" + ex.Message);
                            popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                            popupNotice.ShowPopup();
                        }
                    }
                }
                catch (Exception ex)
                {
                    PopupNotice popupNotice = new PopupNotice("连接学生信息数据库失败" + ex.Message);
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                    popupNotice.ShowPopup();
                }
            }
        }
        #endregion

        #region 读取和保存抽取范围中的学生

        public static async Task SaveCheckedStudentsAsync(List<ItemIndexRange> checkedstudents)
        {
            using (var connection = new SqliteConnection($"Filename={CheckedStudentDataBasePath}"))
            {
                await connection.OpenAsync();

                // 开始事务
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 先清空旧数据
                        var clearCommand = connection.CreateCommand();
                        clearCommand.CommandText = "DELETE  FROM CheckedStudents";
                        await clearCommand.ExecuteNonQueryAsync();

                        string insertQuery = "INSERT INTO CheckedStudents (FirstIndex, LastIndex) VALUES (@FirstIndex, @LastIndex)";
                        using (SqliteCommand insertCommand = new SqliteCommand(insertQuery, connection, (SqliteTransaction)transaction))
                        {
                            insertCommand.Parameters.Add("@FirstIndex", SqliteType.Integer);
                            insertCommand.Parameters.Add("@LastIndex", SqliteType.Integer);
                            int totalCount = checkedstudents.Count;
                            if (totalCount != 0)
                            {
                                for (int i = 0; i < totalCount; i++)
                                {
                                    ItemIndexRange range = checkedstudents[i];
                                    insertCommand.Parameters["@FirstIndex"].Value = range.FirstIndex;
                                    insertCommand.Parameters["@LastIndex"].Value = range.LastIndex;
                                    await insertCommand.ExecuteNonQueryAsync();
                                    // 计算并报告进度
                                    SaveCheckedStudentsProcess = (int)((i + 1) * 100.0 / totalCount);
                                }
                            }
                            else
                            {
                                SaveCheckedStudentsProcess = -1;
                            }
                            //提交事务
                            transaction.Commit();
                        }
                    }
                    catch (SqliteException sqliteEx)
                    {
                        transaction.Rollback(); // 发生错误时回滚事务
                                                //处理数据库异常
                        PopupNotice popupNotice = new PopupNotice("保存抽取范围信息失败,发生数据库异常" + sqliteEx.Message);
                        popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                        popupNotice.ShowPopup();
                    }
                    catch (UnauthorizedAccessException unauthorizedEx)
                    {
                        transaction.Rollback(); // 发生错误时回滚事务
                                                // 处理权限不足异常
                        PopupNotice popupNotice = new PopupNotice("保存抽取范围信息失败,权限不足" + unauthorizedEx.Message);
                        popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                        popupNotice.ShowPopup();
                    }
                    catch (IOException ioEx)
                    {
                        transaction.Rollback(); // 发生错误时回滚事务
                                                // 处理输入输出异常，如文件损坏
                        PopupNotice popupNotice = new PopupNotice("保存抽取范围信息失败,发生文件操作异常" + ioEx.Message);
                        popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                        popupNotice.ShowPopup();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback(); // 发生错误时回滚事务
                        PopupNotice popupNotice = new PopupNotice("保存抽取范围信息数据库失败" + ex.Message);
                        popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                        popupNotice.ShowPopup();
                    }
                }
            }

        }

        public static async Task<List<ItemIndexRange>> LoadCheckedStudentsAsync()
        {
            try
            {
                var ranges = new List<ItemIndexRange>();

                using (var connection = new SqliteConnection($"Filename={CheckedStudentDataBasePath}"))
                {
                    await connection.OpenAsync();

                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT * FROM CheckedStudents";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int firstIndex = reader.GetInt32(0);
                            int length = reader.GetInt32(1) - firstIndex + 1;
                            ranges.Add(new ItemIndexRange(firstIndex, (uint)length));
                        }
                    }
                }

                return ranges;
            }
            catch (SqliteException sqliteEx)
            {
                //处理数据库异常
                PopupNotice popupNotice = new PopupNotice("加载抽取范围信息失败,发生数据库异常" + sqliteEx.Message);
                popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                popupNotice.ShowPopup();
            }
            catch (UnauthorizedAccessException unauthorizedEx)
            {
                // 处理权限不足异常
                PopupNotice popupNotice = new PopupNotice("加载抽取范围信息失败,权限不足" + unauthorizedEx.Message);
                popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                popupNotice.ShowPopup();
            }
            catch (IOException ioEx)
            {
                // 处理输入输出异常，如文件损坏
                PopupNotice popupNotice = new PopupNotice("加载抽取范围信息失败,发生文件操作异常" + ioEx.Message);
                popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                popupNotice.ShowPopup();
            }
            catch (Exception ex)
            {
                PopupNotice popupNotice = new PopupNotice("加载抽取范围信息失败,发生未知错误" + ex.Message);
                popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                popupNotice.ShowPopup();
            }
            return new List<ItemIndexRange>();
        }
        #endregion
    }
}
