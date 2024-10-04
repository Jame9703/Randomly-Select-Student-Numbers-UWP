using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Storage;

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
            var localFolder = ApplicationData.Current.LocalFolder;
            var file = await localFolder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);

            using (var stream = await file.OpenStreamForWriteAsync())
            {
                var serializer = new DataContractJsonSerializer(typeof(ObservableCollection<Student>));
                serializer.WriteObject(stream, students);
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
                        return new ObservableCollection<Student>();
                    }
                }
            }
            catch (FileNotFoundException)
            {
                return new ObservableCollection<Student>();
            }
        }
    }
}
