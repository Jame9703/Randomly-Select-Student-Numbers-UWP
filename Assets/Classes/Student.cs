using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Storage;
using 随机抽取学号.Views;

namespace 随机抽取学号.Classes
{
    public class Student
    {
        public int Id { get; set; }//学号
        public string Name { get; set; }//姓名
        public string  PhotoPath { get; set; }//图片路径
    }
    public class StudentManager
    {
        private const string FileName = "students.json";

        public async Task SaveStudentsAsync(ObservableCollection<Student> students)
        {
            try
            {
                var localFolder = ApplicationData.Current.LocalFolder;
                var file = await localFolder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);

                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    var serializer = new DataContractJsonSerializer(typeof(ObservableCollection<Student>));
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

        public async Task<ObservableCollection<Student>> LoadStudentsAsync()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            try
            {
                var file = await localFolder.GetFileAsync(FileName);
                using (var stream = await file.OpenStreamForReadAsync())
                {
                    var serializer = new DataContractJsonSerializer(typeof(ObservableCollection<Student>));
                    try
                    {
                        return (ObservableCollection<Student>)serializer.ReadObject(stream);
                    }
                    catch (Exception)
                    {
                        PopupNotice popupNotice = new PopupNotice("读取学生信息失败");
                        popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                        popupNotice.ShowPopup();
                        return new ObservableCollection<Student>();
                    }
                }
            }
            catch (FileNotFoundException)
            {
                PopupNotice popupNotice = new PopupNotice("找不到students.json,请不要移动、修改或删除此文件");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Informational;
                popupNotice.ShowPopup();
                await localFolder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);
                return new ObservableCollection<Student>();
            }
        }
    }
}
