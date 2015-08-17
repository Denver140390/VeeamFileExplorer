using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using VeeamFileExplorer.Helpers;
using VeeamFileExplorer.Helpers.Messenger;
using VeeamFileExplorer.Models;

namespace VeeamFileExplorer.ViewModels
{
    class DirectoryContentViewModel : PropertyChangedBase
    {
        private ObservableCollection<FileModelBase> _content;

        public ObservableCollection<FileModelBase> Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value, () => Content); }
        }

        public DirectoryContentViewModel()
        {
            _content = new ObservableCollection<FileModelBase>();

            // Lifehack for DataGrid designing
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) LoadDummyData();

            Messenger.Default.Register<string>(this, LoadDirectoryContentAsync);
        }

        private void LoadDummyData()
        {
            var folderModel = new FolderModel
            {
                Name = "Folder",
                FullPath = "D:\\Folder",
                ChangedDate = DateTime.Now
            };

            _content.Add(folderModel);

            var fileModel = new FileModel
            {
                Name = "File",
                FullPath = "D:\\File",
                ChangedDate = DateTime.Now,
                Size = 123,
                Extension = ".none"
            };

            _content.Add(fileModel);
        }

        public async void LoadDirectoryContentAsync(string path)
        {
            _content.Clear();

            string[] directories;
            string[] files;
            try
            {
                directories = Directory.GetDirectories(path);
                files = Directory.GetFiles(path);
            }
            catch (Exception)
            {
                directories = new string[0];
                files = new string[0];
                //throw new Exception(e.Message);
            }

            foreach (var folderPath in directories)
            {
                var folderModel = new FolderModel();
                await Task.Run(() => CreateFolderModel(folderPath, folderModel));

                _content.Add(folderModel);
            }
            
            foreach (var filePath in files)
            {
                //Fixing an odd behaviour. I've got a file: D:\Autorun.inf\lpt1.UsbFix, which I can see in Windows File Explorer,
                //but which I can't delete. It says, this file does not exist anymore.
                //And of course such file causes exception, when trying to calculate its size. So, ignore the case.
                if (!File.Exists(filePath)) continue;

                var fileModel = new FileModel();
                await Task.Run(() => CreateFileModel(filePath, fileModel));
                var extractedIcon = Icon.ExtractAssociatedIcon(filePath);
                if (extractedIcon != null)
                {
                    var iconBitmap = extractedIcon.ToBitmap();
                    var iconBitmapImage = Bitmap2BitmapImage(iconBitmap);
                    fileModel.Icon = iconBitmapImage;
                }
                //CreateFileModelAsync(file, fileModel);
                _content.Add(fileModel);
            }

            //CalculateFileSizeAsync();
        }

        private void CreateFolderModel(string folderPath, FolderModel folder)
        {
            folder.Name = folderPath.Substring(folderPath.LastIndexOf("\\", StringComparison.Ordinal) + 1);
            folder.FullPath = folderPath;
            folder.ChangedDate = File.GetLastWriteTime(folderPath);

            //folder.Size = GetFolderSize(new DirectoryInfo(folder.FullPath)); //TODO ??? Folder size calculation
        }

        private void CreateFileModel(string filePath, FileModel file)
        {
            var fileInfo = new FileInfo(filePath);

            file.Name = filePath.Substring(filePath.LastIndexOf("\\", StringComparison.Ordinal) + 1);
            file.FullPath = filePath;
            file.ChangedDate = File.GetLastWriteTime(filePath);
            file.Size = fileInfo.Length;
            file.Extension = fileInfo.Extension;
            //file.Icon = iconBitmapImage;
        }

        private static long GetFolderSize(DirectoryInfo directoryInfo)
        {
            // Add file sizes
            var filesInfo = directoryInfo.GetFiles();
            long size = filesInfo.Sum(fileInfo => fileInfo.Length);

            // Add subdirectory sizes
            var directoriesInfo = directoryInfo.GetDirectories();
            size += directoriesInfo.Sum(subDirectoryInfo => GetFolderSize(subDirectoryInfo));

            return (size);
        }

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private BitmapSource Bitmap2BitmapImage(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapSource source;

            try
            {
                source = Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }

            return source;
        }
    }
}
